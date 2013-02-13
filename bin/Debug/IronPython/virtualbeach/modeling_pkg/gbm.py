#import numpy as np
import random
import copy
from .. import utils
from .. import RDotNetWrapper as rdn
import string
import os
import array
import math

#Import the gbm library to R and import the R engine
rdn.r.EagerEvaluate("library(gbm)")
r = rdn.Wrap()


class Model(object): 
    '''represents a gbm (tree with boosting) model generated in R'''
    
    def __init__(self, **args):
        if "model_struct" in args: self.Deserialize( args['model_struct'], args["scratchdir"] )
        else: self.Create(**args)
    
    
    def Deserialize(self, model_struct, scratchdir=""):
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
        self.array_actual = array.array('d', self.actual)
        
    	#First, save the serialized R object to disk (so it can be read from within R)
    	robject_file = "gbm" + "".join(random.choice(string.letters) for i in xrange(10)) + ".robj"        
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
        
        #self.model=r.Call(function='gbm', **self.gbm_params).AsList()
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
        self.data_dictionary = data = copy.copy(args['data'])
        self.target = target = args['target']
        self.array_actual = array.array('d', data[target])
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
        std = utils.std(self.actual)
        print std
        print self.regulatory_threshold
        print self.actual
        deviation = [(x-self.regulatory_threshold)/std for x in self.actual]
        print 'deviation: ' + str(deviation)
        
        #Integer weighting: weight is the observation's rounded-up whole number of standard deviations from the threshold.
        if method == 1: 
            weights = [1 for i in deviation]
            breaks = range( int(math.floor(min(deviation))), int(math.ceil(max(deviation))) )

            for i in breaks:                
                #Find all the observations that meet both criteria simultaneously
                rows = [j for j in range(len(deviation)) if deviation[j] >= i and deviation[j] < i+1]
                
                #Decide how many times to replicate each slice of data
                if i<=0:
                    replicates = 0
                else:
                    replicates = 2*i
                    
                weights = [replicates+1 if k in rows else weights[k] for k in range(len(weights))]
                
        #Continuous weighting: weight is the observation's distance (in standard deviations) from the threshold.      
        elif method == 2:
            weights = [abs(x) for x in deviation]

        #put more weight on exceedances
        elif method == 3:
            #initialize all weights to one.
            weights = [1 for i in deviation]

            #apply weight to the exceedances
            rows = [i for i in range(len(deviation)) if deviation[i] > 0]
            weights = [self.cost[1] if i in rows else weights[i] for i in range(len(weights))]

            #apply weight to the non-exceedances
            rows = [i for i in range(len(deviation)) if deviation[i] <= 0]
            weights = [self.cost[0] if i in rows else weights[i] for i in range(len(weights))]

        #put more weight on exceedances AND downweight near the threshold
        elif method == 4:
            #initialize all weights to one.
            weights = [1 for i in deviation]

            #apply weight to the exceedances
            rows = [i for i in range(len(deviation)) if deviation[i] > 0]
            weights = [self.cost[1] if i in rows else weights[i] for i in range(len(weights))]

            #apply weight to the non-exceedances
            rows = [i for i in range(len(deviation)) if deviation[i] <= 0]
            weights = [self.cost[0] if i in rows else weights[i] for i in range(len(weights))]

            #downweight near the threshold
            rows = [i for i in range(len(deviation)) if abs(deviation[i]) <= 0.25]
            weights = [weights[i]/4. if i in rows else weights[i] for i in range(len(weights))]

        #No weights: all weights are one.
        else: weights = [1 for i in deviation]
            
        return array.array('d', weights)
            

    def Discretize(self, raw):
        '''Label observations as above or below the threshold.'''
        #discretized = np.zeros(raw.shape[0], dtype=int)
        #discretized[ raw >= self.regulatory_threshold ] = 1
        #discretized[ raw < self.regulatory_threshold ] = -1
        discretized = [int(x >= self.regulatory_threshold) for x in raw]
        
        return array.array('l', discretized)
        

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
        
        print prediction
        
        return [float(x) for x in prediction]
        
        
    def PredictExceedanceProbability(self, data_dictionary, threshold, **args):
        if not threshold:
            threshold=self.threshold
            
        #Get the predicted values and the error variance for those predictions
        se = math.sqrt(sum([x**2 for x in self.residuals]) / len(self.residuals))
        raw = self.Predict(data_dictionary, **args)
        adjusted = [(x-threshold)/se for x in raw]
        
        #Now produce the probability of exceedance:
        exceedance_probability = r.Call(function='pnorm', q=array.array('d', adjusted)).AsVector()        
        return [100*float(x) for x in exceedance_probability]
        

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
        
        #raw = np.array(raw)
        
        return raw
        
        
    def GetFitted(self, **params):
        params = {'object':self.model, 'n.trees':self.trees, 'newdata':self.data_frame}
        self.fitted = list(r.Call("predict", **params).AsNumeric())        
        self.array_fitted = array.array('d', self.fitted)
        print 'get residuals'
        self.residual = self.residuals = [self.actual[k] - self.fitted[k] for k in range(len(self.fitted))]
        print 'got residuals'
        self.actual = [self.residuals[k] + self.fitted[k] for k in range(len(self.fitted))]
        self.array_actual = array.array('d', self.actual)
        self.array_residuals = array.array('d', self.residuals)

        
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
            #non_exceedances = self.array_fitted[np.where(self.array_actual < self.regulatory_threshold)[0]]
            non_exceedances = [self.fitted[i] for i in range(len(self.fitted)) if self.actual[i] < self.regulatory_threshold]
            self.threshold = utils.Quantile(non_exceedances, specificity)
            self.specificity = float(len([x for x in non_exceedances if x < self.threshold])) / len(non_exceedances)

        #This error should only happen if somehow there are no non-exceedances in the training data.
        except IndexError: self.threshold = self.regulatory_threshold
        
        
    def Plot(self, **plotargs ):
        try:
            ncomp = plotargs['ncomp']
            if type(ncomp)==str: plotargs['ncomp']=self.ncomp
                
        except KeyError: pass
        
        r['''dev.new''']()
        r.plot(self.model, **plotargs)

        
    def Serialize(self, scratchdir=""):
       	#First, get the serialized gbm model object out of R (we have to write it to disk first)
    	robject_file = "gbm" + "".join(random.choice(string.letters) for i in xrange(10)) + ".robj"
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
        model_struct['model_type'] = 'gbm'
        elements_to_save = ["data_dictionary", "iterations", "threshold", "specificity", "target", "regulatory_threshold",
                                "cost", "depth", "shrinkage", "weights", 'trees', 'folds', 'fraction', 'actual', 'minobsinnode', "modelstring"]
        
        for element in elements_to_save:
            try: model_struct[element] = getattr(self, element)
            except KeyError: raise Exception('The required ' + element + ' was not found in the model to be serialized.')
            
        return model_struct
