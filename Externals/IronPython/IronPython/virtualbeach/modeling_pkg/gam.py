import numpy as np
import random
import copy
from .. import utils
from .. import RDotNetWrapper as rdn


#Import the gbm library to R and import the R engine
rdn.r.EagerEvaluate("library(mgcv)")
r = rdn.Wrap()


class Model(object): 
    '''represents a GAM (Generalized Additive Model) generated in R'''
    
    def __init__(self, **args):
        if "model_struct" in args: self.Deserialize( args['model_struct'] )
        else: self.Create(**args)
    
    
    def Deserialize(self, model_struct):
        '''Use the model_struct dictionary to recreate a model object'''
    
        #Unpack the model_struct dictionary
        self.data_dictionary = model_struct['data_dictionary']
        self.target = model_struct['target']
        self.specificity = model_struct['specificity']
        
        #Get the data into R 
        self.data_frame = utils.Dictionary_to_RDotNet(self.data_dictionary)
        self.data_dictionary = copy.deepcopy(self.data_dictionary)
        self.predictors = len(self.data_dictionary.keys()) - 1
        
        #Generate a gam model in R.
        self.predictors = predictors = self.data_dictionary.keys()
        try: predictors.pop(self.target)
        except: pass
        
        formula = self.target + "~"
        for predictor in predictors:
            formula += "s(" + predictor + ")+"
        formula = formula[:-1]
        
        self.formula = r.Call('as.formula', obj=formula)
        self.gbm_params = {'formula' : self.formula, \
            'family' : 'gaussian', \
            'data' : self.data_frame }        
        self.model=r.Call(function='gam', **self.gbm_params).AsList()

        #Use cross-validation to find the best number of components in the model.
        self.Get_Actual()
        self.Get_Fitted()
        
        #Establish a decision threshold
        self.threshold = model_struct['threshold']
        self.regulatory_threshold = model_struct['regulatory_threshold']
    

    def Create(self, **args):
        '''Create a new gbm model object'''
    
        #Check to see if a threshold has been specified in the function's arguments
        try: self.regulatory_threshold = args['threshold']
        except KeyError: self.regulatory_threshold=2.3711   # if there is no 'threshold' key, then use the default (2.3711)
        self.threshold = 0   #decision threshold
        
        if 'specificity' in args: specificity=args['specificity']
        else: specificity=0.9

        #Store some object data
        self.data_dictionary = copy.deepcopy(args['data'])
        self.target = target = args['target']
                        
        #Get the data into R 
        self.data_frame = utils.Dictionary_to_RDotNet(self.data_dictionary)

        #Generate a gam model in R.
        self.predictors = predictors = self.data_dictionary.keys()
        try: predictors.remove(self.target)
        except: pass
        
        formula = self.target + "~"
        for predictor in predictors:
            formula += "s(" + predictor + ")+"
        formula = formula[:-1]
        
        self.formula = r.Call('as.formula', obj=formula)
        self.gbm_params = {'formula' : self.formula, \
            'family' : 'gaussian', \
            'data' : self.data_frame }        
        self.model=r.Call(function='gam', **self.gbm_params).AsList()
        
        #Use cross-validation to find the best number of components in the model.
        self.Get_Actual()
        self.Get_Fitted()
        
        #Establish a decision threshold
        self.Threshold(specificity)

        
    def Threshold(self, specificity=0.92):
        self.specificity = specificity
    
        if not hasattr(self, 'actual'):
            self.GetActual()
        
        if not hasattr(self, 'fitted'):
            self.GetFitted()

        #Decision threshold is the [specificity] quantile of...
        #...the fitted values for non-exceedances in the training set.
        try:
            #non_exceedances = self.fitted[mlab.find(self.actual < 2.3711)]
            non_exceedances = self.fitted[np.where(self.actual < 2.3711)[0]]
            self.threshold = utils.Quantile(non_exceedances, specificity)
            self.specificity = float(sum(non_exceedances < self.threshold))/non_exceedances.shape[0]

        #This error should only happen if somehow there are no non-exceedances in the training data.
        except IndexError: self.threshold = 2.3711
        

    def Extract(self, model_part, **args):
        try: container = args['extract_from']
        except KeyError: container = self.model
        
        #use R's coef function to extract the model coefficients
        if model_part == 'coef':
            part = r.Call( function='coef', object=self.model, intercept=True )
        
        #otherwise, go to the data structure itself
        else:
            part = container.model_part
            
        return part


    def Predict(self, data_dictionary):
        data_frame = utils.Dictionary_to_RDotNet(data_dictionary)
        prediction_params = {'object': self.model, 'newdata': data_frame }
        prediction = r.Call(function='predict', **prediction_params).AsVector()

        #Translate the R output to a type that can be navigated in Python
        prediction = np.array(prediction).squeeze()
        
        return list(prediction)
        

    def Validate(self, data_dictionary):
        predictions = self.Predict(data_dictionary)
        actual = data_dictionary[self.target]

        p = predictions

        raw = list()
    
        for k in range(len(predictions)):
            t_pos = int(predictions[k] >= self.threshold and actual[k] >= self.regulatory_threshold)
            t_neg = int(predictions[k] <  self.threshold and actual[k] < self.regulatory_threshold)
            f_pos = int(predictions[k] >= self.threshold and actual[k] < self.regulatory_threshold)
            f_neg = int(predictions[k] <  self.threshold and actual[k] >= self.regulatory_threshold)
            raw.append([t_pos, t_neg, f_pos, f_neg])
        
        raw = np.array(raw)
        
        return raw

        
    def GetActual(self):
        #Get the fitted counts from the model.
        fitted_values = np.array( self.model['fitted.values'].AsVector() ).transpose()
        
        #If this is the second stage of an AR model, then incorporate the AR predictions.
        if hasattr(self, 'AR_part'):
            mask = np.ones( self.AR_part.shape[0], dtype=bool )
            #nan_rows = mlab.find( np.isnan(self.AR_part[:,0]) )
            #nan_rows = np.where( np.isnan(self.AR_part[:,0]) )[0]
            
            mask[ nan_rows ] = False
            fitted_values += self.AR_part[mask,0]

        #Recover the actual counts by adding the residuals to the fitted counts.
        residual_values = np.array( self.model['residuals'].AsVector() ).transpose()
        self.actual = np.array( fitted_values + residual_values ).squeeze()
        
        
    def GetFitted(self, **params):
            
        #Get the fitted counts from the model so we can compare them to the actual counts.
        fitted_values = np.array( self.model['fitted.values'].AsVector() )
        fitted_values = fitted_values.transpose()
        
        #If this is the second stage of an AR model, then incorporate the AR predictions.
        if hasattr(self, 'AR_part'):
            mask = np.ones( self.AR_part.shape[0], dtype=bool )
            #nan_rows = mlab.find( np.isnan(self.AR_part[:,0]) )
            nan_rows = np.where( np.isnan(self.AR_part[:,0]) )[0]
            mask[ nan_rows ] = False
            fitted_values += self.AR_part[mask,0]
        
        self.fitted = fitted_values
        self.residual = self.actual-self.fitted
        
                    
    def Count(self):
        #Count the number of true positives, true negatives, false positives, and false negatives.
        self.Get_Actual()
        self.Get_Fitted()
        
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

        
    def Plot(self, **plotargs ):
        try:
            ncomp = plotargs['ncomp']
            if type(ncomp)==str: plotargs['ncomp']=self.ncomp
                
        except KeyError: pass
        
        r['''dev.new''']()
        r.plot(self.model, **plotargs)

        
    def Serialize(self):
        model_struct = dict()
        model_struct['model_type'] = 'gam'
        elements_to_save = ["data_dictionary", "threshold", "specificity", "target", "regulatory_threshold"]
        
        for element in elements_to_save:
            try: model_struct[element] = getattr(self, element)
            except KeyError: raise Exception('The required ' + element + ' was not found in the model to be serialized.')
            
        return model_struct
