#import numpy as np
import array
import random
import copy
import re
from random import choice
from .. import utils
from .. import RDotNetWrapper as rdn
import string
import os

#Import the pls library into R, and connect python to R.
rdn.r.EagerEvaluate("library(pls)") 
r = rdn.Wrap()


class Model(object): 
    '''represents a PLS model generated in R'''

    def __init__(self, **args):
        if "model_struct" in args: self.Deserialize( args['model_struct'], args["scratchdir"] )
        else: self.Create(**args)
        
        
    def Deserialize(self, model_struct, scratchdir=""):
        #Unpack the model_struct dictionary
        self.data_dictionary = model_struct['data_dictionary']
        self.target = model_struct['target']
        self.specificity = model_struct['specificity']
        
        #Get the data into R 
        self.data_frame = utils.DictionaryToR(self.data_dictionary)
        self.data_dictionary = copy.copy(self.data_dictionary)        
        self.num_predictors = len(self.data_dictionary.keys()) - 1
        
        #First, save the serialized R object to disk (so it can be read from within R)
        robject_file = "pls" + "".join(random.choice(string.letters) for i in xrange(10)) + ".robj"        
        if scratchdir:
            scratchdir = scratchdir.split(os.sep)
            scratchdir.append(robject_file)
            robject_file = os.sep.join(scratchdir)        
        robject_file = robject_file.replace("\\", "\\\\")

        modelstring = model_struct["modelstring"]
    	f = open(robject_file, "wb")
    	f.write(modelstring)
    	f.close()
    	
    	#Read the serialized model object into R:
    	load_params = {'file' : robject_file}
        objects = r.Call(function='load', **load_params).AsVector()
        get_params = {'x' : str(objects[0])}
        self.model = r.Call(function="get", **get_params).AsList()
    	os.remove(robject_file)
        
        #Generate a PLS model in R.
        self.formula = r.Call('as.formula', obj=utils.SanitizeVariableName(self.target) + '~.')
        self.pls_params = {'formula' : self.formula, \
            'data' : self.data_frame, \
            'validation' : 'LOO', \
            'x' : True }
        #self.model = r.Call(function='plsr', **self.pls_params).AsList()
                
        #Get the number of columns from the validation step
        #(Might be fewer than the number of predictor variables if n<p)
        self.ncomp_max = int(list(r.Call(function="dim", x=self.model['validation'].AsList()['pred']).AsNumeric())[2])
            
        #Use cross-validation to find the best number of components in the model.
        self.GetActual()
        self.ncomp = model_struct['ncomp']
        self.GetFitted()
        
        #Establish a decision threshold
        self.specificity = model_struct['specificity']
        self.threshold = model_struct['threshold']
        self.regulatory_threshold = model_struct['regulatory_threshold']
    
    
    def Create(self, **args):
        #Check to see if a threshold has been specified in the function's arguments
        if 'threshold' in args: self.threshold = args['threshold']
        else: self.threshold = 2.3711   # if there is no 'threshold' key, then use the default (2.3711)
        self.regulatory_threshold = self.threshold
        
        if 'specificity' in args: specificity=args['specificity']
        else: specificity=0.9
        
        #Get the data into R
        self.target = args['target']
        data = args['data']
        self.data_frame = utils.DictionaryToR(data)
        self.data_dictionary = copy.copy(data)
        self.num_predictors = len(self.data_dictionary.keys()) - 1
        
        #Generate a PLS model in R.
        self.formula = r.Call('as.formula', obj=utils.SanitizeVariableName(self.target) + '~.')
        self.pls_params = {'formula' : self.formula, \
            'data' : self.data_frame, \
            'validation' : 'LOO', \
            'x' : True }    
        self.model = r.Call(function='plsr', **self.pls_params).AsList()
        
        #Get the number of columns from the validation step
        #(Might be fewer than the number of predictor variables if n<p)
        self.ncomp_max = int(list(r.Call(function="dim", x=self.model['validation'].AsList()['pred']).AsNumeric())[2])
        
        #Use cross-validation to find the best number of components in the model.
        self.GetActual()
        self.CrossValidation(**args)
        self.GetFitted()
        
        #Establish a decision threshold
        self.Threshold(specificity)


    def Extract(self, model_part, **args):
        try: container = args['extract_from']
        except KeyError: container = self.model
        
        #use R's coef function to extract the model coefficients
        if model_part == 'coef':
            part = list(r.Call(function='coef', object=self.model, ncomp=self.ncomp, intercept=True).AsVector())
            
        #use R's MSEP function to estimate the variance.
        elif model_part == 'MSEP':
            #part = list( r.Call(function='MSEP', object=self.model).AsVector() )
            part = sum([(self.fitted[i] - self.actual[i])**2 for i in range(len(self.fitted))]) / len(self.fitted)
            #part = part['val'].AsVector()[self.ncomp]
            
        #use R's RMSEP function to estimate the standard error.
        elif model_part == 'RMSEP':
            part = (sum([(self.fitted[i] - self.actual[i])**2 for i in range(len(self.fitted))]) / len(self.fitted))**0.5
            #part = part['val'].AsVector()[self.ncomp]
            #part = list( r.Call(function='RMSEP', object=self.model).AsList() )
        
        #Get the variable names, ordered as R sees them.
        elif model_part == 'names':
            part = ["Intercept"]
            part.extend( self.data_frame.ColumnNames )
            try: part.remove( utils.SanitizeVariableName(self.target) )
            except: pass
        
        #otherwise, go to the data structure itself
        else:
            part = container[model_part]
            
        return part


    def PredictValues(self, data_dictionary, **args):
        data_frame = utils.DictionaryToR(data_dictionary)
        prediction_params = {'object': self.model, 'newdata': data_frame }
        
        prediction = r.Call(function='predict', **prediction_params).AsVector()
        prediction = array.array('d', prediction)
        #prediction = np.array( prediction, dtype=float )
        
        #Reshape the vector of predictions
        columns = min(self.num_predictors, self.ncomp_max)
        rows = len(prediction) / columns        
        #prediction.shape = (columns, rows)
        
        pp = []
        for k in range(int(columns)):
            b = k * rows
            e = b + rows
            pp.append(array.array('d', prediction[b:e]))
        prediction = pp
        
        #prediction = prediction.transpose()
        return prediction
        
        
    def PredictExceedances(self, data_dictionary, **args):
        prediction = self.PredictValues(data_dictionary)
        return [int(p) >= self.threshold for p in prediction[self.ncomp-1]]
        
        
    def PredictExceedanceProbability(self, data_dictionary, threshold, **args):
        if not threshold:
            threshold = self.threshold
        
        prediction = self.PredictValues(data_dictionary)[self.ncomp-1]    
        se = self.Extract('RMSEP')
        
        nonexceedance_probability = r.Call(function='pnorm', q=array.array('d', [(threshold-x)/se for x in prediction])).AsVector()
        exceedance_probability = [100*float(1-item) for item in nonexceedance_probability]
        return exceedance_probability
    
    
    def Predict(self, data_dictionary, **args):
        prediction = self.PredictValues(data_dictionary)
        return [float(item) for item in prediction[self.ncomp-1]]
    
        
    def CrossValidation(self, cv_method=0, **args):
        '''Select ncomp by the requested CV method'''
        validation = self.model['validation'].AsDataFrame()
        
        #method 0: select the fewest components with PRESS within 1 stdev of the least PRESS (by the bootstrap)
        if cv_method == 0: #Use the bootstrap to find the standard deviation of the MSEP
            #Get the leave-one-out CV error from R:
            columns = min(self.num_predictors, self.ncomp_max)
            cv = array.array('d', validation['pred'].AsVector())
            rows = len(cv) / columns
            cc = []
            for k in range(int(columns)):
                b = k * rows
                e = b + rows
                cc.append(array.array('d', cv[b:e]))
            cv = cc
            
            #PRESS = map(lambda x: sum((cv[:,x]-self.array_actual)**2), range(cv.shape[1]))
            PRESS = [sum([(cv[i][j]-self.actual[j])**2 for j in range(rows)]) for i in range(int(columns))]
            #ncomp = np.argmin(PRESS)
            ncomp = [i for i in range(len(PRESS)) if PRESS[i]==min(PRESS)][0]
            
            #cv_squared_error = (cv[:,ncomp]-self.array_actual)**2
            cv_squared_error = [(cv[ncomp][j]-self.actual[j])**2 for j in range(int(rows))]
            sample_space = xrange(rows)
            
            PRESS_stdev = list()
            
            #Cache random number generator and int's constructor for a speed boost
            _random, _int = random.random, int
            
            for i in range(100):
                PRESS_bootstrap = list()
                
                for j in range(100):
                    PRESS_bootstrap.append( sum([cv_squared_error[_int(_random()*rows)] for i in sample_space]) )
                    
                PRESS_stdev.append( utils.std(PRESS_bootstrap) )
                
            med_stdev = utils.median(PRESS_stdev)
            
            #Maximum allowable PRESS is the minimum plus one standard deviation
            good_ncomp = [i for i in range(len(PRESS)) if PRESS[i] < min(PRESS) + med_stdev]
            self.ncomp = int( min(good_ncomp)+1 )
            
        #method 1: select the fewest components w/ PRESS less than the minimum plus a 4% of the range
        if cv_method==1:
            #PRESS stands for predicted error sum of squares
            PRESS0 = validation['PRESS0'][0]
            PRESS = list( validation['PRESS'] )
    
            #the range is the difference between the greatest and least PRESS values
            PRESS_range = abs(PRESS0 - min(PRESS))
            
            #Maximum allowable PRESS is the minimum plus a fraction of the range.
            max_CV_error = min(PRESS) + PRESS_range/25
            good_ncomp = [i for i in range(len(PRESS)) if PRESS[i] < max_CV_error]
    
            #choose the most parsimonious model that satisfies that criterion
            self.ncomp = int( min(good_ncomp)+1 )
        

    def Threshold(self, specificity=0.9):
        self.specificity = specificity
    
        if not hasattr(self, 'actual'):
            self.GetActual()
        
        if not hasattr(self, 'fitted'):
            self.GetFitted()

        #Decision threshold is the [specificity] quantile of the fitted values for non-exceedances in the training set.
        try:
            #non_exceedances = self.array_fitted[np.where(self.array_actual < self.regulatory_threshold)[0]]
            non_exceedances = [self.fitted[i] for i in range(len(self.fitted)) if self.actual[i] < self.regulatory_threshold]
            self.threshold = utils.Quantile(non_exceedances, specificity)
            self.specificity = float(len([x for x in non_exceedances if x < self.threshold])) / len(non_exceedances)

        #This error should only happen if somehow there are no non-exceedances in the training data.
        except IndexError: self.threshold = self.regulatory_threshold


    def GetActual(self):
        #Get the fitted counts from the model.
        columns = min(self.num_predictors, self.ncomp_max)
        fitted = array.array('d', self.model['fitted.values'].AsVector())
        rows = len(fitted) / columns
        
        ff = []
        for k in range(columns):
            b = k * rows
            e = b + rows
            ff.append(array.array('d', fitted[b:e]))
        fitted = ff[0]
        
        #Recover the actual counts by adding the residuals to the fitted counts.
        residuals = array.array('d', self.model['residuals'].AsVector())
        rr = []
        for k in range(columns):
            b = k * rows
            e = b + rows
            rr.append(array.array('d', residuals[b:e]))
        residuals = rr[0]
        
        self.array_actual = array.array('d',  [fitted[i] + residuals[i] for i in range(len(fitted))])
        self.actual = list(self.array_actual)
        
        
    def GetFitted(self, **params):
        try: ncomp = params['ncomp']
        except KeyError:
            try: ncomp = self.ncomp
            except AttributeError: ncomp=1
            
        #Get the fitted counts from the model so we can compare them to the actual counts.
        columns = min(self.num_predictors, self.ncomp_max)
        fitted = array.array('d', self.model['fitted.values'].AsVector())
        rows = len(fitted) / columns
        
        ff = []
        for k in range(columns):
            b = k * rows
            e = b + rows
            ff.append(array.array('d', fitted[b:e]))
        fitted = ff[self.ncomp-1]
        
        self.array_fitted = fitted
        self.array_residual = array.array('d', [self.array_actual[i] - self.array_fitted[i] for i in range(rows)])
        
        self.fitted = list(self.array_fitted)
        self.residual = self.residuals = list(self.array_residual)
        
        
    def GetInfluence(self):
        #Get the covariate names
        self.names = self.data_dictionary.keys()
        self.names.remove(self.target)

        #Now get the model coefficients from R.
        coefficients = array.array('d', self.Extract('coef'))
        
        #Get the standard deviations (from the data_dictionary) and package the influence in a dictionary.
        raw_influence = list()
        
        for i in range( len(self.names) ):
            standard_deviation = utils.std( self.data_dictionary[self.names[i]] )
            raw_influence.append( float(abs(standard_deviation * coefficients[i+1])) )
        
        self.influence = dict( zip(self.names, [float(x/sum(raw_influence)) for x in raw_influence]) )
        return self.influence
            
            
    def Count(self):
        #Count the number of true positives, true negatives, false positives, and false negatives.
        self.GetActual()
        self.GetFitted()
        
        #initialize counts to zero:
        t_pos = 0
        t_neg = 0
        f_pos = 0
        f_neg = 0
        
        for obs in range( len(self.fitted) ):
            if self.fitted[obs] >= self.threshold:
                if self.actual[obs] >= 2.3711: t_pos += 1
                else: f_pos += 1
            else:
                if self.actual[obs] >= 2.3711: f_neg += 1
                else: t_neg += 1
        
        return [t_pos, t_neg, f_pos, f_neg]
        
        
    def Serialize(self, scratchdir=""):
       	#First, get the serialized gbm model object out of R (we have to write it to disk first)
    	robject_file = "pls" + "".join(random.choice(string.letters) for i in xrange(10)) + ".robj"
        if scratchdir:
            scratchdir = scratchdir.split(os.sep)
            scratchdir.append(robject_file)
            robject_file = os.sep.join(scratchdir)
        robject_file = robject_file.replace("\\", "\\\\")
        
    	save_params = {'save' : self.model, \
            'file' : robject_file, \
            'ascii' : True }
        r.Call(function='save', **save_params)
    	f = open(robject_file, "r")
    	self.modelstring = f.read()
    	f.close()
    	os.remove(robject_file)
    	
    	#Now pack the model state into a dictionary.
        model_struct = dict()
        model_struct['model_type'] = 'pls'
        elements_to_save = ["data_dictionary", "ncomp", "threshold", "specificity", "target", "regulatory_threshold", "modelstring"]
        
        for element in elements_to_save:
            try: model_struct[element] = getattr(self, element)
            except KeyError: raise Exception('The required ' + element + ' was not found in the model to be serialized.')
            
        return model_struct
        
        
    def ToString(self):
        return "PLS model"
