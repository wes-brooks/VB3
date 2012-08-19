import numpy as np
from .. import utils
from copy import deepcopy

#Fire up the interface to R
import rpy2
import rpy2.robjects as rob
import rpy2.robjects.numpy2ri
r = rob.r


class Model(object): 
    '''represents a logistic regression model generated in R'''

    def __init__(self, data_dictionary, model_target, **args):
        #Create a logistic model object
    
        #Check to see if a threshold has been specified in the function's arguments
        try: self.regulatory_threshold = args['threshold']
        except KeyError: self.regulatory_threshold=2.3711   # if there is no 'threshold' key, then use the default (2.3711)
        
        #Check to see if a specificity has been specified in the function's arguments
        try: self.specificity = args['specificity']
        except KeyError: self.specificity=0.9
        
        #Store some object data
        self.data_dictionary = deepcopy(data_dictionary)
        self.model_target = model_target
                
        #Check to see if a weighting method has been specified in the function's arguments
        try:
            #integer (discrete) weighting
            if str(args['weights']).lower()[0] in ['d', 'i']: 
                self.weights = self.AssignWeights(method=1)
                
            #float (continuous) weighting
            elif str(args['weights']).lower()[0] in ['c', 'f']: 
                self.weights = self.AssignWeights(method=2)
                
            else: self.weights = self.AssignWeights(method=0) 
                
        #If there is no 'weights' key, set all weights to one.
        except KeyError: 
            self.weights = self.AssignWeights(method=0) 
        
        #Label the exceedances in the training set.
        self.data_dictionary[model_target] = self.AssignLabels(self.data_dictionary[model_target])
        
        #Get the data into R 
        self.data_frame = utils.DictionaryToR(self.data_dictionary)

        #Generate a logistic regression model in R.
        self.formula = r['as.formula'](self.model_target + '~.')
        self.logistic_params = {'formula' : self.formula, \
            'family' : 'binomial', \
            'data' : self.data_frame, \
            'weights' : self.weights, \
            'x' : True }
        self.model=r.glm(**self.logistic_params)
        
        #Select model components and a decision threshold
        self.SelectModel()
        self.Threshold(self.specificity)


    def AssignWeights(self, method=0):
        #Weight the observations in the training set based on their distance from the threshold.
        deviation = (self.data_dictionary[self.model_target]-self.regulatory_threshold)/np.std(self.data_dictionary[self.model_target])
        
        #Integer weighting: weight is the observation's rounded-up whole number of standard deviations from the threshold.
        if method == 1: 
            weights = np.ones( len(deviation) )
            breaks = range( int( np.floor(min(deviation)) ), int( np.ceil(max(deviation)) ) )

            for i in breaks:
                #find all observations that meet the upper and lower criteria, separately
                first_slice = np.where(deviation > i)[0]
                second_slice = np.where(deviation < i+1)[0]
                
                #now find all the observations that meet both criteria simultaneously
                rows = filter( lambda x: x in first_slice, second_slice )
                
                #Decide how many times to replicate each slice of data
                if i<0:
                    replicates = (abs(i) - 1)
                else:
                    replicates = i
                    
                weights[rows] = replicates + 1
                
        #Continuous weighting: weight is the observation's distance (in standard deviations) from the threshold.      
        elif method == 2:
            weights = abs(deviation)
            
        #No weights: all weights are one.
        else: weights = np.ones( len(deviation) )
            
        return weights
            
            
            
    def AssignLabels(self, raw):
        #Label observations as above or below the threshold.
        raw = np.array(raw >= self.regulatory_threshold, dtype=int)
        return raw
        
        
    def SelectModel(self, direction='back'):
        self.model = r.step(self.model, direction=direction)
        

    def Extract(self, model_part, **args):
        try: container = args['extract_from']
        except KeyError: container = self.model
        
        #use R's coef function to extract the model coefficients
        if model_part == 'coef':
            part = r.coef(self.model, intercept=True)
        
        #otherwise, go to the data structure itself
        else:
            names = list(container.names)
            index = names.index(model_part)
            part = container[index]
            
        return part


    def Predict(self, data_dictionary):
        data_frame = utils.DictionaryToR(data_dictionary)
        prediction_params = {'object': self.model, 'newdata': data_frame }
        prediction = r.predict(**prediction_params)

        #Translate the R output to a type that can be navigated in Python
        prediction = np.array(prediction).squeeze()
        
        #transform log odds to probability of exceedance
        prob = np.exp(prediction)/(1+np.exp(prediction))
        
        return prob
        
        
    def _PredictExceedances(self, data_dictionary):        
        prediction = self.Predict(data_dictionary)
        return np.array(prediction >= self.threshold, dtype=int)


    def Threshold(self, specificity=0.9):
        #Find the optimal decision threshold
        fitted = np.array( self.model[ np.where(np.array(self.model.names)=='fitted.values')[0] ] )
        self.threshold = utils.Quantile(fitted[self.data_dictionary[self.model_target]==0], specificity)


    def Validate(self, data_dictionary):
        predictions = self.Predict(data_dictionary)
        actual = data_dictionary[self.model_target]

        p = predictions

        raw = list()
    
        for k in range(len(predictions)):
            t_pos = int(predictions[k] >= self.threshold and actual[k] >= self.regulatory_threshold)
            t_neg = int(predictions[k] < self.threshold and actual[k] < self.regulatory_threshold)
            f_pos = int(predictions[k] >= self.threshold and actual[k] < self.regulatory_threshold)
            f_neg = int(predictions[k] < self.threshold and actual[k] >= self.regulatory_threshold)
            raw.append([t_pos, t_neg, f_pos, f_neg])
        
        raw = np.array(raw)
        
        return raw


    def GetActual(self):
        #Get the fitted counts from the model.
        fitted_values = np.array(self.Extract('fitted.values'))
        fitted_values = np.squeeze(fitted_values)

        #Recover the actual counts by adding the residuals to the fitted counts.
        residual_values = np.array(self.Extract('residuals'))
        residual_values = np.squeeze(residual_values)
        
        self.actual = np.array( fitted_values[:,0] + residual_values[:,0] ).squeeze()
        
        
    def GetFitted(self, **params):
        #Get the fitted counts from the model so we can compare them to the actual counts.
        fitted_values = np.array(self.Extract('fitted.values'))
        fitted_values = np.squeeze(fitted_values)
        self.fitted = np.array( fitted_values )
        self.residual = self.actual-self.fitted
        
        
    def GetInfluence(self):
        #Get the model terms from R's model object
        terms = self.Extract('terms')
        terms = str(terms)
        
        #Get the covariate names
        self.names = self.data_dictionary.keys()
        self.names.remove(self.model_target)

        #Now get the model coefficients from R.
        coefficients = np.array( self.Extract('coef') )
        coefficients = coefficients.flatten()
        
        #Get the standard deviations (from the data_dictionary) and package the influence in a dictionary.
        raw_influence = list()
        
        for i in range( len(self.names) ):
            standard_deviation = np.std( self.data_dictionary[self.names[i]] )
            raw_influence.append( abs(standard_deviation * coefficients[i+1]) )

            
        self.influence = dict( zip(raw_influence/np.sum(raw_influence), self.names) )
            
            
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
        
        
    def Plot(self, **plotargs ):
        try:
            ncomp = plotargs['ncomp']
            if type(ncomp)==str: plotargs['ncomp']=self.ncomp
                
        except KeyError: pass
        
        r['''dev.new''']()
        r.plot(self.model, **plotargs)

