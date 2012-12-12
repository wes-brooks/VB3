import numpy as np
import random
import copy
from .. import utils
from .. import RDotNetWrapper as rdn

#Import the gbm library to R and import the R engine
rdn.r.EagerEvaluate("library(gbm)")
r = rdn.Wrap()


class Model(object): 
    '''represents a gbm (tree with boosting) model generated in R'''
    
    def __init__(self, **args):
        if "model_struct" in args: self.Deserialize( args['model_struct'] )
        else: self.Create(**args)
    
    
    def Deserialize(self, model_struct):
        '''Recreate a gbm model from a serialized object'''
    
        #Load saved parameters from the serialized object.
        self.iterations = model_struct['iterations']
        self.cost = model_struct['cost']
        self.depth = model_struct['depth']
        self.minobsinnode = model_struct['minobsinnode']
        self.shrinkage = model_struct['shrinkage']
        self.data_dictionary = model_struct['data_dictionary']
        self.target = model_struct['target']
        self.weights = model_struct['weights']  
        self.fraction = model_struct['fraction']
        self.folds = model_struct['folds']
        self.trees = model_struct['trees']
        self.actual = model_struct['actual']
        self.array_actual = np.array(self.actual)
        
        #Get the data into R 
        self.data_frame = utils.DictionaryToR(self.data_dictionary)

        #Generate a gbm model in R.
        self.formula = r.Call('as.formula', obj=utils.SanitizeVariableName(self.target) + '~.')
        self.gbm_params = {'formula' : self.formula, \
            'distribution' : 'gaussian', \
            'data' : self.data_frame, \
            'weights' : self.weights, \
            'interaction.depth' : self.depth, \
            'shrinkage' : self.shrinkage, \
            'n.trees' : self.iterations, \
            'bag.fraction' : self.fraction, \
            'n.minobsinnode' : self.minobsinnode, \
            'cv.folds' : self.folds }
        
        self.model=r.Call(function='gbm', **self.gbm_params).AsList()
        self.GetFitted()
        
        #Establish a decision threshold
        self.specificity = model_struct['specificity']
        self.threshold = model_struct['threshold']
        self.regulatory_threshold = model_struct['regulatory_threshold']

    def Create(self, **args):
        '''Create a new gbm model object'''
    
        #Check to see if a threshold has been specified in the function's arguments
        try: self.regulatory_threshold = args['threshold']
        except KeyError: self.regulatory_threshold = 2.3711   # if there is no 'threshold' key, then use the default (2.3711)
        self.threshold = 0   #decision threshold

        try: self.iterations = args['iterations']
        except KeyError: self.iterations = 10000   # if there is no 'iterations' key, then use the default (2000)

        #Cost: two values - the first is the cost of a false positive, the second is the cost of a false negative.
        try: self.cost = args['cost']
        except KeyError: self.cost = [1,1]   # if there is no 'cost' key, then use the default [1,1] (=equal weight)
        
        #
        try: self.specificity = args['specificity']
        except KeyError: self.specificity = 0.9   # if there is no 'specificity' key, then use the default 0.9  

        #depth: how many branches should be allowed per decision tree?
        try: self.depth = args['depth']
        except KeyError: self.depth = 5   # if there is no 'depth' key, then use the default 1 (decision stumps)  
        
        #n.minobsinnode: what is the fewest observations per node in the tree?
        try: self.minobsinnode = args['minobsinnode']
        except KeyError: self.minobsinnode = 5

        #shrinkage: learning rate parameter
        try: self.shrinkage = args['shrinkage']
        except KeyError: self.shrinkage = 0.001   # if there is no 'shrinkage' key, then use the default 0.01
        
        #bagging fraction: proportion of data used at each step
        try: self.fraction = args['fraction']
        except KeyError: self.fraction = 0.5   # if there is no 'fraction' key, then use the default 0.5
        
        #shrinkage: learning rate parameter
        try: self.folds = args['folds']
        except KeyError: self.folds = 5   # if there is no 'folds' key, then use the default 5-fold CV

        #Store some object data
        self.data_dictionary = data = copy.deepcopy(args['data'])
        self.target = target = args['target']
        self.array_actual = data[target][np.sum(np.isnan(data.values()), axis=0)==0]
        self.actual = list(self.array_actual)
                
        #Check to see if a weighting method has been specified in the function's arguments
        try:
            #integer (discrete) weighting
            if str(args['weights']).lower()[0] in ['d', 'i']: 
                self.weights = self.AssignWeights(method=1)
                
            #float (continuous) weighting
            elif str(args['weights']).lower()[0] in ['f']: 
                self.weights = self.AssignWeights(method=2)
                
            #cost-based weighting
            elif str(args['weights']).lower()[0] in ['c']: 
                self.weights = self.AssignWeights(method=3)

            #cost-based weighting, and down-weight the observations near the threshold
            elif str(args['weights']).lower()[0] in ['b']: 
                self.weights = self.AssignWeights(method=4)

            else: self.weights = self.AssignWeights(method=0) 
                
        #If there is no 'weights' key, set all weights to one.
        except KeyError: 
            self.weights = self.AssignWeights(method=0) 
        
        #Label the exceedances in the training set.
        #self.data_dictionary[target] = self.Discretize(self.data_dictionary[target])
        
        #Get the data into R 
        self.data_frame = utils.DictionaryToR(self.data_dictionary)

        #Generate a gbm model in R.
        self.formula = r.Call('as.formula', obj=utils.SanitizeVariableName(self.target) + '~.')
        self.gbm_params = {'formula' : self.formula, \
            'distribution' : 'gaussian', \
            'data' : self.data_frame, \
            'weights' : self.weights, \
            'interaction.depth' : self.depth, \
            'shrinkage' : self.shrinkage, \
            'n.trees' : self.iterations, \
            'bag.fraction' : self.fraction, \
            'n.minobsinnode' : self.minobsinnode, \
            'cv.folds' : self.folds }
        
        self.model = r.Call(function='gbm', **self.gbm_params).AsList()
        
        #Find the best number of iterations for predictive performance. Prefer to use CV.
        perf_params = {'object':self.model, 'plot.it':False}
        if self.folds > 1: perf_params['method'] = 'cv'
        else: perf_params['method'] = 'OOB'
        
        try: self.trees = r.Call(function='gbm.perf', **perf_params).AsNumeric()[0]
        except ValueError: self.trees = self.iterations
        
        self.GetFitted()
        self.Threshold(self.specificity)


    def AssignWeights(self, method=0):
        '''Weight the observations in the training set based on their distance from the threshold.'''
        deviation = (self.data_dictionary[self.target]-self.regulatory_threshold)/np.std(self.data_dictionary[self.target])
        
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
                if i<=0:
                    replicates = 0
                else:
                    replicates = 2*i
                    
                weights[rows] = replicates + 1
                
        #Continuous weighting: weight is the observation's distance (in standard deviations) from the threshold.      
        elif method == 2:
            weights = abs(deviation)

        #put more weight on exceedances
        elif method == 3:
            #initialize all weights to one.
            weights = np.ones( len(deviation) )

            #apply weight to the exceedances
            rows = np.where( deviation > 0 )[0]
            weights[rows] = self.cost[1]

            #apply weight to the non-exceedances
            rows = np.where( deviation <= 0 )[0]
            weights[rows] = self.cost[0]

        #put more weight on exceedances AND downweight near the threshold
        elif method == 4:
            #initialize all weights to one.
            weights = np.ones( len(deviation) )

            #apply weight to the exceedances
            rows = np.where( deviation > 0 )[0]
            weights[rows] = self.cost[1]

            #apply weight to the non-exceedances
            rows = np.where( deviation <= 0 )[0]
            weights[rows] = self.cost[0]

            #downweight near the threshold
            rows = np.where( abs(deviation) <= 0.25 )[0]
            weights[rows] = weights[rows]/4.

        #No weights: all weights are one.
        else: weights = np.ones( len(deviation) )
            
        return weights
            

    def Discretize(self, raw):
        '''Label observations as above or below the threshold.'''
        #discretized = np.zeros(raw.shape[0], dtype=int)
        #discretized[ raw >= self.regulatory_threshold ] = 1
        #discretized[ raw < self.regulatory_threshold ] = -1
        discretized = np.array(raw >= self.regulatory_threshold, dtype=int)
        
        return discretized
        

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


    def Predict(self, data_dictionary, **args):
        data_frame = utils.DictionaryToR(data_dictionary)
        prediction_params = {'object': self.model, 'newdata': data_frame, 'n.trees': self.trees }
        prediction = r.Call(function='predict', **prediction_params).AsVector()
        prediction = np.array(prediction, dtype=float).squeeze()
        
        return [float(item) for item in prediction]
        
        
    def PredictExceedanceProbability(self, data_dictionary, **args):
        raw = self.Predict(data_dictionary, **args)
        adjusted = [item-self.threshold for item in raw]
        prediction = [np.exp(item) / (1 + np.exp(item)) for item in adjusted]
        
        return [float(item) for item in prediction]
        

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
        
        
    def GetFitted(self, **params):
        params = {'object':self.model, 'n.trees':self.trees, 'newdata':self.data_frame}
        self.fitted = list(r.Call("predict", **params).AsNumeric())
        self.array_fitted = np.array(self.fitted, dtype='float')

        
    def GetInfluence(self):
        summary = r.Call(function='summary.gbm', object=self.model, plotit=False).AsList()
        indx = [int(i) for i in summary[0].AsVector()]

        influence = list(summary[1].AsVector())
        levels = r.Call(function='levels', x=summary[0]).AsVector()  
        vars = [levels[i-1] for i in indx]
        
        #Create a dictionary with all the influences and a list of those variables with influence greater than 1%.
        self.influence = dict(zip(vars, influence))
        self.vars = [str(vars[k]) for k in range(len(vars)) if influence[k]>5]
        return self.influence
        

    def Threshold(self, specificity=0.9):
        self.specificity = specificity
        
        if not hasattr(self, 'fitted'):
            self.GetFitted()

        #Decision threshold is the [specificity] quantile of the fitted values for non-exceedances in the training set.
        try:
            non_exceedances = self.array_fitted[np.where(self.array_actual < self.regulatory_threshold)[0]]
            self.threshold = utils.Quantile(non_exceedances, specificity)
            self.specificity = float(sum(non_exceedances < self.threshold))/non_exceedances.shape[0]

        #This error should only happen if somehow there are no non-exceedances in the training data.
        except ZeroDivisionError:
            self.threshold = 0        
            self.specificity = 1
        
        
    def Plot(self, **plotargs ):
        try:
            ncomp = plotargs['ncomp']
            if type(ncomp)==str: plotargs['ncomp']=self.ncomp
                
        except KeyError: pass
        
        r['''dev.new''']()
        r.plot(self.model, **plotargs)

        
    def Serialize(self):
        model_struct = dict()
        model_struct['model_type'] = 'gbm'
        elements_to_save = ["data_dictionary", "iterations", "threshold", "specificity", "target", "regulatory_threshold",
                                "cost", "depth", "shrinkage", "weights", 'trees', 'folds', 'fraction', 'actual', 'minobsinnode']
        
        for element in elements_to_save:
            try: model_struct[element] = getattr(self, element)
            except KeyError: raise Exception('The required ' + element + ' was not found in the model to be serialized.')
            
        return model_struct
