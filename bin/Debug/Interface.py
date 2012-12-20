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
import numpy as np
import System
import pickle
import re

#Set the R_HOME environment variable
os.environ["R_HOME"] = cwd + '\\IronPython\\bin\\R-2.15.1'

from IronPython.virtualbeach import utils
import IronPython.virtualbeach.BeachController as Control



class BeachInterface(object):

    def __init__(self):
        self.u = utils

    def Validate(self, data, target, specificity='', folds='', method='PLS', **args):
        '''This is the main function in the script. It uses the PLS modeling classes to build a predictive model.'''
        
        #parse the inputs
        target = str(target)
        args['specificity'] = specificity
        
        #initialize the objects where we will drop the results
        result_list = list()
        combined = summary = []
        columns = ['specificity', 'true pos', 'true neg', 'false pos', 'false neg', 'total']
        
        #parse the modeling method and then call it
        Validate = getattr(Control, "Validate" + method)
        return Validate(data, target, **args)
        

    def SpecificityChart(self, validation_results):
        '''Just relay this call directly to the Controller.'''
        return Control.SpecificityChart(validation_results)
    

    def GetPossibleSpecificities(self, model):   
        '''Find out what values specificity could take if we count out one non-exceedance at a time.'''
        regulatory_threshold = model.regulatory_threshold
        thresholds = np.sort(model.array_fitted[np.where(model.array_actual < regulatory_threshold)[0]])
        specificities = [x/float(thresholds.shape[0]) for x in range(thresholds.shape[0])]
        return [[float(x) for x in thresholds], list(specificities)]
        
        
    def Serialize(self, model, scratchdir=""):
        '''Convert the model to a string that can be written to disk.'''
        model_struct = model.Serialize(scratchdir)
        serialized = pickle.dumps(model_struct, protocol=2)
        return serialized
        
        
    def Deserialize(self, model_string, scratchdir=""):
        print scratchdir
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
        data_dict = dict( zip(headers, np.transpose(data)) )
        predictions = model.Predict(data_dict)
        return list(predictions)
        
        
    def PredictExceedanceProbability(self, model, data):
        '''Use the model to predict the probability of exceeding the threshold.'''
        [headers, data] = utils.DotNetToArray(data)
        data_dict = dict( zip(headers, np.transpose(data)) )
        predictions = model.PredictExceedanceProbability(data_dict)
        return predictions
        
        
    def ProbabilityOfExceedance(self, prediction, threshold, se):
        return utils.ProbabilityOfExceedance(prediction, threshold, se)
        #exceedance_probability = 1-norm.cdf(x=prediction, loc=threshold, scale=se)
        #return double(exceedance_probability.squeeze())

    
Interface = BeachInterface()

