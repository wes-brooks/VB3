from modeling_pkg import pls, gbm, gam#, logistic, pls_parallel
methods = {'pls':pls, 'boosting':gbm, 'gbm':gbm, 'gam':gam}

import utils
from utils import Event

import numpy as np
import copy

boosting_iterations = 2000

ProgressEvent = Event()

def ValidatePLS(data, target, folds='', **args):
    '''Creates a PLS model and tests its performance with cross-validation.'''
    
    #convert the data from a .NET DataTable or DataView into a numpy array
    [headers, data] = utils.DotNetToArray(data)
    target = str(target)
    regulatory = args['regulatory_threshold']
    
    #randomly assign the data to cross-validation folds
    if not folds: folds = 5 
    fold = utils.Partition(data, folds)
    data_dict = dict( zip(headers, np.transpose(data)) )
    folds = np.arange(folds)+1
    
    #Make a model for each fold and validate it.
    results = list()
    for f in folds:
        model_data = data[fold!=f,:]
        validation_data = data[fold==f,:]
        
        model_dict = dict( zip(headers, np.transpose(model_data)) )
        validation_dict = dict( zip(headers, np.transpose(validation_data)) )

        model = pls.Model(data=model_dict, target=target, **args)
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " built.", progress=(f-0.5)/len(folds))

        predictions = np.array(model.Predict(validation_dict))
        validation_actual = validation_dict[ target ]
        exceedance = np.array(validation_actual > regulatory, dtype=bool)
        
        fitted = np.array(model.fitted)
        actual = np.array(model.actual)
        candidates = fitted[np.where(actual < regulatory)]
        num_candidates = float(candidates.shape[0])
        
        specificity = list()
        sensitivity = list()
        threshold = list()
        tpos = list()
        tneg = list()
        fpos = list()
        fneg = list()
        total = model_data.shape[0]
        non_exceedances = float(sum(exceedance == False))
        exceedances = float(sum(exceedance == True))
        
        for prediction in predictions:
            tp = np.where(validation_actual[predictions >= prediction] >= regulatory)[0].shape[0]
            fp = np.where(validation_actual[predictions >= prediction] < regulatory)[0].shape[0]
            tn = np.where(validation_actual[predictions < prediction] < regulatory)[0].shape[0]
            fn = np.where(validation_actual[predictions < prediction] >= regulatory)[0].shape[0]
        
            tpos.append(tp)
            fpos.append(fp)
            tneg.append(tn)
            fneg.append(fn)
            
            try: candidate_threshold = np.max(candidates[np.where(candidates <= prediction)])
            except: candidate_threshold = np.min(candidates)
            specificity.append(np.where(fitted[actual < regulatory] < candidate_threshold)[0].shape[0] / num_candidates)
            sensitivity.append(np.where(fitted[actual >= regulatory] >= candidate_threshold)[0].shape[0] / np.where(actual >= regulatory)[0].shape[0])
            
            #the first candidate threshold that would be below this threshold
            try: threshold.append(max(fitted[fitted < prediction]))
            except: threshold.append(max(fitted))
        
        specificity = np.array(specificity)
        sensitivity = np.array(sensitivity)
        ProgressEvent(message="Model " + str(f) + " of " + str(folds[-1]) + " validated.", progress=float(f)/len(folds))
        
        tpos = np.array(tpos)
        tneg = np.array(tneg)
        fpos = np.array(fpos)
        fneg = np.array(fneg)
        
        result = dict(threshold=threshold, sensitivity=sensitivity, specificity=specificity, tpos=tpos, tneg=tneg, fpos=fpos, fneg=fneg )
        results.append(result)

    model = pls.Model(data=data_dict, target=target, **args)
    
    return (results, model)
    
    
def SpecificityChart(results):
    '''Produces a list of lists that Virtual Beach turns into a chart of performance in prediction as we sweep the specificity parameter.'''
    specificities = list()    
    [specificities.extend(fold['specificity']) for fold in results]
    specificities = np.unique( np.sort(specificities) )
    
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
            indx = list(np.where(fold['specificity'] <= specificity)[0])
            if indx:
                indx = indx[ np.argmax(fold['specificity'][indx]) ]
            
                tpos[-1] += fold['tpos'][indx]
                fpos[-1] += fold['fpos'][indx]
                tneg[-1] += fold['tneg'][indx]
                fneg[-1] += fold['fneg'][indx]
            else:
                tpos[-1] = tpos[-1] + fold['tpos'][0] + fold['fneg'][0] #all exceedances correctly classified
                fpos[-1] = fpos[-1] + fold['tneg'][0] + fold['fpos'][0] #all non-exceedances incorrectly classified
        
    return [spec, tpos, tneg, fpos, fneg]
        
 
def ValidateGAM(data, target, folds='', **args):
    '''Creates prospective models using Generalized Additive Models and uses cross-validation to assess their performance in prediction.'''
    
    #convert the data from a .NET DataTable or DataView into a numpy array
    [headers, data] = utils.DotNetToArray(data)
    #[headers, data] = [data.keys(), np.array(data.values()).T]
    target = str(target)
    regulatory = args['regulatory_threshold']
    
    #randomly assign the data to cross-validation folds
    if not folds: folds = 5 
    fold = utils.Partition(data, folds)
    data_dict = dict( zip(headers, np.transpose(data)) )
    folds = np.arange(folds)+1
    
    #Make a model for each fold and validate it.
    results = list()
    for f in folds:
        model_data = data[fold!=f,:]
        validation_data = data[fold==f,:]
        
        model_dict = dict( zip(headers, np.transpose(model_data)) )
        validation_dict = dict( zip(headers, np.transpose(validation_data)) )

        model = gam.Model(data=model_dict, target=target, **args)  

        predictions = np.array(model.Predict(validation_dict))
        validation_actual = validation_dict[target]
        exceedance = np.array(validation_actual > regulatory, dtype=bool)
        
        fitted = np.array(model.fitted)
        actual = np.array(model.actual)
        candidates = fitted[np.where(actual < regulatory)]
        num_candidates = float(candidates.shape[0])

        specificity = list()
        sensitivity = list()
        threshold = list()
        tpos = list()
        tneg = list()
        fpos = list()
        fneg = list()
        total = model_data.shape[0]
        non_exceedances = float(sum(exceedance == False))
        exceedances = float(sum(exceedance == True))
        
        for prediction in predictions:
            tp = np.where(validation_actual[predictions >= prediction] >= regulatory)[0].shape[0]
            fp = np.where(validation_actual[predictions >= prediction] < regulatory)[0].shape[0]
            tn = np.where(validation_actual[predictions < prediction] < regulatory)[0].shape[0]
            fn = np.where(validation_actual[predictions < prediction] >= regulatory)[0].shape[0]
        
            tpos.append(tp)
            fpos.append(fp)
            tneg.append(tn)
            fneg.append(fn)
            
            try: candidate_threshold = np.max(candidates[np.where(candidates <= prediction)])
            except: candidate_threshold = np.min(candidates)
            specificity.append(np.where(fitted[actual < regulatory] < candidate_threshold)[0].shape[0] / num_candidates)
            sensitivity.append(np.where(fitted[actual >= regulatory] >= candidate_threshold)[0].shape[0] / np.where(actual >= regulatory)[0].shape[0])
            
            #the first candidate threshold that would be below this threshold
            try: threshold.append(max(fitted[fitted < prediction]))
            except: threshold.append(max(fitted))
        
        specificity = np.array(specificity)
        sensitivity = np.array(sensitivity)
        
        tpos = np.array(tpos)
        tneg = np.array(tneg)
        fpos = np.array(fpos)
        fneg = np.array(fneg)
        
        result = dict(threshold=threshold, sensitivity=sensitivity, specificity=specificity, tpos=tpos, tneg=tneg, fpos=fpos, fneg=fneg)
        results.append(result)

    model = gam.Model(data=data_dict, target=target, **args)
    
    return (results, model)
    

def ValidateLogistic(model_dict, validation_dict, target, **args):
    '''Creates and tests prospective models using logisitic regression.'''
    
    #Pick the model building parameters out of args
    try: weights = list( args['weights'] )   #Logistic regression, weighted away from the threshold.
    except KeyError: weights = ['discrete']

    try: limits=utils.flatten([args['specificity']])
    except KeyError: limits = np.arange(11.)/100 + 0.85     #Default: test specificity limits from 0.85 to 0.95 

    results = list()

    #Test models w/ midseason split
    for weight in weights:
        for limit in limits:
        
            l=logistic.Model(model_dict, target,  specificity=limit, weights=weight)
                
            summary = Summarize(l, validation_dict, **args)
            summary.insert( 1, weight)
            summary.insert( 1, np.nan)
            
            results.append( summary )

    return results


def ValidateGBM(data, target, folds='', **args):    
    '''Creates a PLS model and tests its performance with cross-validation.'''
    
    #convert the data from a .NET DataTable or DataView into a numpy array
    [headers, data] = utils.DotNetToArray(data)
    target = str(target)
    regulatory = args['regulatory_threshold']
    
    #randomly assign the data to cross-validation folds
    if not folds: folds = 5 
    fold = utils.Partition(data, folds)
    data_dict = dict( zip(headers, np.transpose(data)) )
    folds = np.arange(folds)+1
    
    #Make a model for each fold and validate it.
    results = list()
    for f in folds:
        model_data = data[fold!=f,:]
        validation_data = data[fold==f,:]
        
        model_dict = dict( zip(headers, np.transpose(model_data)) )
        validation_dict = dict( zip(headers, np.transpose(validation_data)) )

        model = gbm.Model(data=model_dict, target=target, **args)

        predictions = np.array(model.Predict(validation_dict))
        validation_actual = validation_dict[ target ]
        exceedance = np.array(validation_actual > regulatory, dtype=bool)
        
        fitted = np.array(model.fitted)
        actual = np.array(model.actual)
        candidates = fitted[np.where(actual < regulatory)]
        num_candidates = float(candidates.shape[0])
        
        specificity = list()
        sensitivity = list()
        threshold = list()
        tpos = list()
        tneg = list()
        fpos = list()
        fneg = list()
        total = model_data.shape[0]
        non_exceedances = float(sum(exceedance == False))
        exceedances = float(sum(exceedance == True))
        
        for prediction in predictions:
            tp = np.where(validation_actual[predictions >= prediction] >= regulatory)[0].shape[0]
            fp = np.where(validation_actual[predictions >= prediction] < regulatory)[0].shape[0]
            tn = np.where(validation_actual[predictions < prediction] < regulatory)[0].shape[0]
            fn = np.where(validation_actual[predictions < prediction] >= regulatory)[0].shape[0]
        
            tpos.append(tp)
            fpos.append(fp)
            tneg.append(tn)
            fneg.append(fn)
            
            try: candidate_threshold = np.max(candidates[np.where(candidates <= prediction)])
            except: candidate_threshold = np.min(candidates)
            specificity.append(np.where(fitted[actual < regulatory] < candidate_threshold)[0].shape[0] / num_candidates)
            sensitivity.append(np.where(fitted[actual >= regulatory] >= candidate_threshold)[0].shape[0] / np.where(actual >= regulatory)[0].shape[0])
            
            #the first candidate threshold that would be below this threshold
            try: threshold.append(max(fitted[fitted < prediction]))
            except: threshold.append(max(fitted))
        
        specificity = np.array(specificity)
        sensitivity = np.array(sensitivity)
        
        tpos = np.array(tpos)
        tneg = np.array(tneg)
        fpos = np.array(fpos)
        fneg = np.array(fneg)
        
        result = dict(threshold=threshold, sensitivity=sensitivity, specificity=specificity, tpos=tpos, tneg=tneg, fpos=fpos, fneg=fneg )
        results.append(result)

    model = gbm.Model(data=data_dict, target=target, **args)
    
    return (results, model)

    
def Model(data_dict, target='', **args):
    '''Creates a Model object of the desired class, with the specified parameters.'''
    try: method = args['method']
    except KeyError: return "Error: did not specify a modeling method to Beach_Controller.Model"
    
    method = methods[ method.lower() ]
    model = method.Model(data=data_dict, target=target, **args)
    
    return model
    

def Summarize(model, validation_dict, **args):
    '''Summarizes the prediction results'''
    raw = model.Validate(validation_dict)
    
    if hasattr(model, 'breakpoint'): split = float( model.breakpoint )
    else: split = np.nan

    spec_lim = model.specificity
    
    if 'fold' in args: year = float( args['fold'] )
    elif 'year' in args: year = float( args['year'] )
    else: year = np.nan
    
    tp = float( sum(raw[:,0]) )  #True positives
    tn = float( sum(raw[:,1]) )  #True negatives
    fp = float( sum(raw[:,2]) )  #False positives
    fn = float( sum(raw[:,3]) )  #False negatives
    total = tp+tn+fp+fn
    
    return [spec_lim, tp, tn, fp, fn, total]
    
    
def Deserialize(model_struct, scratchdir="", **args):
    '''Turns the model_struct into a Model object, using the method provided by model_struct['model_type']'''
    method = methods[ model_struct['model_type'] ]
    return method.Model(model_struct=model_struct, scratchdir=scratchdir)
