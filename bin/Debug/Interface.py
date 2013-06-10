'''
Created on May 27, 2011
@author: wrbrooks
'''
#Import the PLS modeling classes
import sys
import clr 

#Set the paths to IronPython based on the current working directory
sys.path.insert(0, '.\\IronPython\\bin\\IronPython 2.7\\Lib\\site-packages' )
sys.path.insert(0, '.\\IronPython\\bin\\IronPython 2.7\\DLLs' )
sys.path.insert(0, '.\\IronPython\\bin\\IronPython 2.7\\Lib' )
sys.path.insert(0, '.\\IronPython\\bin\\IronPython 2.7' )

#We must link to the IronPython libraries before we can load the os module.
clr.AddReference("IronPython")
clr.AddReference("IronPython.Modules")
import os
import copy

#For some reason, numpy is unable to find the mtrand library on its own.
cwd = os.getcwd()
sys.path[3] = cwd + '\\IronPython\\bin\\IronPython 2.7\\Lib\\site-packages'
sys.path[2] = cwd + '\\IronPython\\bin\\IronPython 2.7\\DLLs'
sys.path[1] = cwd + '\\IronPython\\bin\\IronPython 2.7\\Lib'
sys.path[0] = cwd + '\\IronPython\\bin\\IronPython 2.7'
clr.AddReference("mtrand.dll")
clr.AddReference("System.Data")
clr.AddReference("DotNetExtensions")
import System
import pickle
import re
import array

#Set the R_HOME environment variable
os.environ["R_HOME"] = cwd + '\\IronPython\\bin\\R-2.15.1'

from IronPython.virtualbeach import utils
from IronPython.virtualbeach.utils import UIThread, BGThread
import IronPython.virtualbeach.BeachController as Control

import System.Threading
from System.Threading import SynchronizationContext, WaitCallback, ThreadPool, SendOrPostCallback, CancellationToken, CancellationTokenSource
from System import OperationCanceledException

class BeachInterface(object):
    def __init__(self):
        self.u = utils
        self.ProgressEvent = utils.ProgressEvent()
        Control.ProgressEvent += self.HandleProgressEvent
        Control.ModelValidationCompleteEvent += self.HandleModelValidationCompleteEvent
        Control.ModelCancelledEvent += self.HandleModelCancelledEvent

        
    def Validate(self, data, target, threshold, specificity='', folds='', method='PLS', callback='', cancellationCallback='', **args):
        '''This is the main function in the script. It uses the PLS modeling classes to build a predictive model.'''        
        #parse the inputs
        target = str(target)
        
        #initialize the objects where we will drop the results
        result_list = list()
        combined = summary = []
        columns = ['specificity', 'true pos', 'true neg', 'false pos', 'false neg', 'total']
        
        #Generate a new CancellationTokenSource that can be used to cancel the modeling operation.
        self.tokensource = CancellationTokenSource()
        token = self.tokensource.Token
        
        #parse the modeling method and then call it
        Validate = getattr(Control, "Validate" + method)
        result = Validate(data, target, threshold, specificity, folds, callback, cancellationCallback, token, **args)
        
        
    def CancelModel(self):
        try: self.tokensource.Cancel()
        except NameError: pass


    def SpecificityChart(self, validation_results):
        '''Just relay this call directly to the Controller.'''
        return Control.SpecificityChart(validation_results)
    

    def GetPossibleSpecificities(self, model):   
        '''Find out what values specificity could take if we count out one non-exceedance at a time.'''
        regulatory_threshold = model.regulatory_threshold
        thresholds = sorted([model.fitted[i] for i in range(len(model.fitted)) if model.actual[i] <= regulatory_threshold])
        specificities = [x/float(len(thresholds)) for x in range(len(thresholds))]
        return [[float(x) for x in thresholds], list(specificities)]
        
        
    def Serialize(self, model, scratchdir=""):
        '''Convert the model to a string that can be written to disk.'''
        model_struct = model.Serialize(scratchdir)
        serialized = pickle.dumps(model_struct, protocol=2)
        return serialized
        
        
    def Deserialize(self, model_string, scratchdir=""):
        '''Take a string and turn it into a model object.'''
        model_struct = pickle.loads(model_string)
        model = Control.Deserialize(model_struct, scratchdir)
        return model

    
    def GetPredictors(self, model):
        '''Return a list of the predictor variables that are used in this model.'''
        predictors = model.data_dictionary.keys()
        predictors.remove( model.target )
        return predictors
        
        
    def GetModelExpression(self, model):
        '''Return a list of the predictor variables that are used in this model.'''
        predictors = model.data_dictionary.keys()
        predictors.remove( model.target )
        
        expression = predictors[0]
        for predictor in predictors[1:]:
            expression = expression + " + " + predictor #utils.SanitizeVariableName(predictor)
        
        return expression
        
    
    def Predict(self, model, data):
        '''Use the model to predict the value that its output will take over the observations in the data.'''
        [headers, data] = utils.DotNetToArray(data)
        data_dict = dict( zip(headers, [array.array('d', [row[i] for row in data]) for i in range(len(data[0]))]) )
        predictions = model.Predict(data_dict)
        return list(predictions)
        
        
    def PredictExceedanceProbability(self, model, data, threshold=''):
        '''Use the model to predict the probability of exceeding the threshold.'''
        [headers, data] = utils.DotNetToArray(data)
        data_dict = dict( zip(headers, [array.array('d', [row[i] for row in data]) for i in range(len(data[0]))]) )
        predictions = model.PredictExceedanceProbability(data_dict, threshold)
        return predictions
        
        
    def ProbabilityOfExceedance(self, prediction, threshold, se):
        return utils.ProbabilityOfExceedance(prediction, threshold, se)
        #exceedance_probability = 1-norm.cdf(x=prediction, loc=threshold, scale=se)
        #return double(exceedance_probability.squeeze())
        
    
    @utils.UIThread
    def HandleProgressEvent(self, message='', progress=0):
        self.ProgressEvent(message, progress)

        
    @utils.UIThread
    def HandleModelValidationCompleteEvent(self, result='', callback=''):
        callback(result)
    
    @utils.UIThread
    def HandleModelCancelledEvent(self, message='', callback=''):
        callback(message)


Interface = BeachInterface()

