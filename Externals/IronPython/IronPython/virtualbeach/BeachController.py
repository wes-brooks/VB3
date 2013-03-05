from modeling_pkg import pls, gbm#, gam#, logistic, pls_parallel
methods = {'pls':pls, 'boosting':gbm, 'gbm':gbm} #, 'gam':gam}

import utils
#from utils import Event

#import numpy as np
import copy
import array

boosting_iterations = 2000

ProgressEvent = utils.ProgressEvent()
ModelValidationCompleteEvent = utils.ModelValidationCompleteEvent()

from utils import UIThread, BGThread
    

@BGThread
def ValidatePLS(data, target, threshold, specificity, folds='', callback='', **args):
    '''Creates a PLS model and tests its performance with cross-validation.'''
    
    #convert the data from a .NET DataTable or DataView into a numpy array
    [headers, data] = utils.DotNetToArray(data)
    target = str(target)
    regulatory = copy.copy(threshold)
    
    #randomly assign the data to cross-validation folds
    if not folds: folds = 5 
    fold = utils.Partition(data, folds)
    data_dict = dict( zip(headers, [array.array('d', [row[i] for row in data]) for i in range(len(data[0]))] ))
    folds = [i+1 for i in range(folds)]
    
    #Make a model for each fold and validate it.
    results = list()
    for f in folds:
        model_data = [data[i] for i in range(len(data)) if fold[i]!=f]
        validation_data = [data[i] for i in range(len(data)) if fold[i]==f]
        
        model_dict = dict( zip(headers, [array.array('d', [row[i] for row in model_data]) for i in range(len(model_data[0]))]) )
        validation_dict = dict( zip(headers, [array.array('d', [row[i] for row in validation_data]) for i in range(len(validation_data[0]))]) )

        model = pls.Model(data=model_dict, target=target, threshold=regulatory, specificity=specificity, **args)
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " built.", progress=(f-0.5)/len(folds))

        predictions = list(model.Predict(validation_dict))
        validation_actual = list(validation_dict[target])
        exceedance = [validation_actual[i] > regulatory for i in range(len(validation_actual))]
        
        fitted = list(model.fitted)
        actual = list(model.actual)
        candidates = [fitted[i] for i in range(len(fitted)) if actual[i]<regulatory]
        num_candidates = float(len(candidates))
        
        spec = list()
        sensitivity = list()
        threshold = list()
        tpos = list()
        tneg = list()
        fpos = list()
        fneg = list()
        total = len(model_data)
        non_exceedances = float(len(exceedance) - sum(exceedance))
        exceedances = float(sum(exceedance))
        
        for prediction in predictions:
            tp = len([i for i in range(len(predictions)) if predictions[i] >= prediction and validation_actual[i] >= regulatory])
            fp = len([i for i in range(len(predictions)) if predictions[i] >= prediction and validation_actual[i] < regulatory])
            tn = len([i for i in range(len(predictions)) if predictions[i] < prediction and validation_actual[i] < regulatory])
            fn = len([i for i in range(len(predictions)) if predictions[i] < prediction and validation_actual[i] >= regulatory])
        
            tpos.append(tp)
            fpos.append(fp)
            tneg.append(tn)
            fneg.append(fn)
            
            try: candidate_threshold = max([x for x in candidates if x <= prediction])
            except: candidate_threshold = min(candidates)
            spec.append(len([i for i in range(len(fitted)) if actual[i]<regulatory and fitted[i]<candidate_threshold]) / num_candidates)
            sensitivity.append(len([i for i in range(len(fitted)) if actual[i]>=regulatory and fitted[i]>=candidate_threshold]) / float(len(actual) - num_candidates))
            
            #the first candidate threshold that would be below this threshold
            try: threshold.append(max([x for x in fitted if x <= prediction]))
            except: threshold.append(max(fitted))
        
        #spec = np.array(spec)
        #sensitivity = np.array(sensitivity)
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " validated.", progress=float(f)/len(folds))
        
        #tpos = np.array(tpos)
        #tneg = np.array(tneg)
        #fpos = np.array(fpos)
        #fneg = np.array(fneg)
        
        result = dict(threshold=threshold, sensitivity=sensitivity, specificity=spec, tpos=tpos, tneg=tneg, fpos=fpos, fneg=fneg )
        results.append(result)

    model = pls.Model(data=data_dict, target=target, threshold=regulatory, specificity=specificity, **args)
    
    if not callback: return (results, model)
    else: ModelValidationCompleteEvent(result=(results, model), callback=callback)
    
    
@BGThread
def ValidateGBM(data, target, threshold, specificity, folds='', callback='', **args):
    '''Creates a GBM model and tests its performance with cross-validation.'''
    
    #convert the data from a .NET DataTable or DataView into a numpy array
    [headers, data] = utils.DotNetToArray(data)
    target = str(target)
    regulatory = copy.copy(threshold)
    
    #randomly assign the data to cross-validation folds
    if not folds: folds = 5 
    fold = utils.Partition(data, folds)
    data_dict = dict( zip(headers, [array.array('d', [row[i] for row in data]) for i in range(len(data[0]))] ))
    folds = [i+1 for i in range(folds)]
    
    #Make a model for each fold and validate it.
    results = list()
    for f in folds:
        model_data = [data[i] for i in range(len(data)) if fold[i]!=f]
        validation_data = [data[i] for i in range(len(data)) if fold[i]==f]
        
        model_dict = dict( zip(headers, [array.array('d', [row[i] for row in model_data]) for i in range(len(model_data[0]))]) )
        validation_dict = dict( zip(headers, [array.array('d', [row[i] for row in validation_data]) for i in range(len(validation_data[0]))]) )

        model = gbm.Model(data=model_dict, target=target, threshold=regulatory, specificity=specificity, **args)
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " built.", progress=(f-0.5)/len(folds))

        predictions = list(model.Predict(validation_dict))
        validation_actual = list(validation_dict[target])
        exceedance = [validation_actual[i] > regulatory for i in range(len(validation_actual))]
        
        fitted = list(model.fitted)
        actual = list(model.actual)
        candidates = [fitted[i] for i in range(len(fitted)) if actual[i]<regulatory]
        num_candidates = float(len(candidates))
        
        spec = list()
        sensitivity = list()
        threshold = list()
        tpos = list()
        tneg = list()
        fpos = list()
        fneg = list()
        total = len(model_data)
        non_exceedances = float(len(exceedance) - sum(exceedance))
        exceedances = float(sum(exceedance))
        
        for prediction in predictions:
            tp = len([i for i in range(len(predictions)) if predictions[i] >= prediction and validation_actual[i] >= regulatory])
            fp = len([i for i in range(len(predictions)) if predictions[i] >= prediction and validation_actual[i] < regulatory])
            tn = len([i for i in range(len(predictions)) if predictions[i] < prediction and validation_actual[i] < regulatory])
            fn = len([i for i in range(len(predictions)) if predictions[i] < prediction and validation_actual[i] >= regulatory])
        
            tpos.append(tp)
            fpos.append(fp)
            tneg.append(tn)
            fneg.append(fn)
            
            try: candidate_threshold = max([x for x in candidates if x <= prediction])
            except: candidate_threshold = min(candidates)
            spec.append(len([i for i in range(len(fitted)) if actual[i]<regulatory and fitted[i]<candidate_threshold]) / num_candidates)
            sensitivity.append(len([i for i in range(len(fitted)) if actual[i]>=regulatory and fitted[i]>=candidate_threshold]) / float(len(actual) - num_candidates))
            
            #the first candidate threshold that would be below this threshold
            try: threshold.append(max([x for x in fitted if x <= prediction]))
            except: threshold.append(max(fitted))
        
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " validated.", progress=float(f)/len(folds))
        
        result = dict(threshold=threshold, sensitivity=sensitivity, specificity=spec, tpos=tpos, tneg=tneg, fpos=fpos, fneg=fneg )
        results.append(result)

    model = gbm.Model(data=data_dict, target=target, threshold=regulatory, specificity=specificity, **args)
    
    if not callback: return (results, model)
    else: ModelValidationCompleteEvent(result=(results, model), callback=callback)    

    
    
def SpecificityChart(results):
    '''Produces a list of lists that Virtual Beach turns into a chart of performance in prediction as we sweep the specificity parameter.'''
    specificities = list()    
    [specificities.extend(fold['specificity']) for fold in results]
    specificities = sorted(list(set(specificities)))
    
    spec = []
    tpos = []
    tneg = []
    fpos = []
    fneg = []
    
    for specificity in specificities:
        tpos.append(0)
        tneg.append(0)
        fpos.append(0)
        fneg.append(0)
        spec.append(specificity)
        
        for fold in results:
            indx = [i for i in range(len(fold['specificity'])) if fold['specificity'][i] <= specificity]
            if indx:
                indx = [i for i in indx if fold['specificity'][i] == max([fold['specificity'][j] for j in indx])][0]
            
                tpos[-1] += fold['tpos'][indx]
                fpos[-1] += fold['fpos'][indx]
                tneg[-1] += fold['tneg'][indx]
                fneg[-1] += fold['fneg'][indx]
            else:
                tpos[-1] = tpos[-1] + fold['tpos'][0] + fold['fneg'][0] #all exceedances correctly classified
                fpos[-1] = fpos[-1] + fold['tneg'][0] + fold['fpos'][0] #all non-exceedances incorrectly classified
        
    return [spec, tpos, tneg, fpos, fneg]
        
    
def Deserialize(model_struct, scratchdir="", **args):
    '''Turns the model_struct into a Model object, using the method provided by model_struct['model_type']'''
    method = methods[ model_struct['model_type'] ]
    return method.Model(model_struct=model_struct, scratchdir=scratchdir)
