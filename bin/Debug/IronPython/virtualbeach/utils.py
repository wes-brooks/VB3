import random
#import numpy as np
import datetime
import random
#from numpy import where, nonzero

from dateutil import parser, relativedelta
from datetime import datetime
import re

import RDotNetWrapper as rdn
from System import Array
import array
import math

import System
import System.Threading
from System.Threading import SynchronizationContext, WaitCallback, ThreadPool, SendOrPostCallback

#Defines a decorator that causes a function to execute on a background thread
def BGThread(fun):
    def argUnpacker(args):
        oldSyncContext = SynchronizationContext.Current
        try:
            SynchronizationContext.SetSynchronizationContext(args[-1])
            fun(*args[:-1])
        finally: SynchronizationContext.SetSynchronizationContext(oldSyncContext)

    def wrapper(*args):
        args2 = args + (SynchronizationContext.Current,)
        ThreadPool.QueueUserWorkItem(WaitCallback(argUnpacker), args2)

    return wrapper
    
    
#Defines a decorator that causes a function to execute on the UI thread
def UIThread(fun):
    def unpack(args):
        ret = fun(*args)
        if ret != None:
            import warnings
            warnings.warn(fun.__name__ + " function returned " + str(ret) + " but that return value isn't propigated to the calling thread")
            
    def wrapper(*args):
        if SynchronizationContext.Current == None: fun(*args)
        else: SynchronizationContext.Current.Send(SendOrPostCallback(unpack), args)

    return wrapper


class ProgressEvent():
    def __init__(self):
        self.handlers = set()

    def Handle(self, handler):
        self.handlers.add(handler)
        return self

    def Unhandle(self, handler):
        try:
            self.handlers.remove(handler)
        except:
            raise ValueError("Handler is not handling this event, so cannot unhandle it.")
        return self

    def Fire(self, message='', progress=0):
        for handler in self.handlers:
            handler(message, progress)

    def GetHandlerCount(self):
        return len(self.handlers)

    __iadd__ = Handle
    __isub__ = Unhandle
    __call__ = Fire
    __len__  = GetHandlerCount


def MakeConverters(headers):
    '''Numpy's loadtxt uses 'converters' to cast data to floats.
    This function generates a converter for each column.'''
    converters = dict()
    for i in range(len(headers)):
        converters[i] = lambda val: Converter(val)
    return converters

   
def Converter(value):
    '''If value cannot be immediately converted to a float, then return a NaN.'''
    try: return float(value or 'nan')
    except ValueError: return float('nan')
    
    
def std(values):
    mean = sum(values) / len(values)
    SS = sum([(x - mean)**2 for x in values])
    return (1.0/(len(values)-1)) * SS

    
def median(values):
    values.sort()
    if len(values)==0:
        median = float('nan')
    elif len(values)%2 == 0:
        #have to take avg of middle two
        i = len(values)/2
        median = (values[i] + values[i-1])/2
    else:
        #find the middle (remembering that lists start at 0)
        i = (len(values)-1)/2
        median = values[i]
    return median

    
def DotNetToArray(data):
    '''Copy the contents of a .NET DataView into a numpy array with a list of headers'''
    
    #First import the libraries that are needed just for this step:
    import System
    import System.Data
    import DotNetExtensions

    #Determine whether we got a DataTable or a DataView
    if isinstance(data, System.Data.DataTable): data_view = data.AsDataView()
    elif isinstance(data, System.Data.DataView): data_view = data
    else: return "data was passed to Python in an invalid type"
    
    #Extract the column headers:
    headers = [column.Caption for column in data_view.Table.Columns]
    
    #Find which rows of the DataView contain NaNs:
    nan_rows = [ sum( [isinstance(item, System.DBNull) for item in row] ) for row in data_view ]
    flags = [not bool(nan_rows[i]) for i in range(len(nan_rows))]
    
    #Now copy the NaN-free rows of the DataView into an array:
    raw_table = [list(data_view[i]) for i in range(len(data_view)) if flags[i]]
    data_array = [array.array('d', row) for row in raw_table]
    #data_array = np.array(data_array, dtype=float, ndmin=2)

    return [headers, data_array]
    
    
def SanitizeVariableName(var):
    #First remove any leading characters that are not letters, then any other characters that are not alphanumeric.
    var = re.sub("^[^a-zA-Z]+", "", var)
    return re.sub("[^a-zA-Z0-9]+", "", var)
    

def DictionaryToR(data_dictionary, name=''):
    '''Moves a python dictionary into an R data frame'''
    
    #link to R:
    r = rdn.r
    
    #set up variables we'll need later
    df = dict()
    cols = dict()
    command = "data.frame("
    
    #each column of the dictionary should be a numpy array
    for col in data_dictionary:
        if data_dictionary[col].typecode in ['f', 'd']:
            df[col] = r.CreateNumericVector( Array[float](data_dictionary[col]) ).AsVector()
        else:
            df[col] = r.CreateCharacterVector( Array[str](data_dictionary[col]) ).AsVector()
        
        #Update the command and give the column a random name in R
        r_col_name = SanitizeVariableName(col)
        col_name = "col_" + str(random.random())[2:-4]
        command = command + r_col_name + "=" + col_name + ","
        r.SetSymbol(col_name, df[col])
        
    #create the data frame in R
    command = command[:-1] + ")"
    data_frame = r.EagerEvaluate(command).AsDataFrame()

    #if a name was supplied, then assign it to the R data.frame object:
    if name: r.SetSymbol(name, data_frame)

    return data_frame
 
 
def Draw(count, max):
    '''draw random numbers without replacement'''
    result = []
    iterations = range(count)
    for i in iterations:
        index = int(max * random.random())
        if index in result: iterations.append(1)
        else: result.append(index)

    return result
 

def Quantile(list, q):
    '''Find the value at the specified quantile of the list.'''
    if q>1 or q<0:
        return float('nan')
    else:
        list.sort()
        position = int(math.ceil(q * (len(list)-1)))
        print list[position]
        return list[position]
        

def NonStandardDeviation(list, pivot):
    var = 0
    for item in list:
        var = var + (item-pivot)**2
        
    return math.sqrt( var/len(list) )

    



def Partition(data, folds):
    '''Partition the data set into random, equal-sized folds for cross-validation'''
    
    #If we've called for leave-one-out CV (folds will be like 'n' or 'LOO' or 'leave-one-out')
    if str(folds).lower()[0]=='l' or str(folds).lower()[0]=='n' or folds>len(data):
        fold = range(len(data))
    
    #Otherwise, randomly permute the data, then use contiguously-permuted chunks for CV
    else:
        #Initialization
        indices = range(len(data))
        fold = [folds for i in range(len(data))]
        quantiles = [float(x) / folds for x in range(folds)]
        
        #Proceed through the quantiles in reverse order, labelling the ith fold at each step. Ignore the zeroth quantile.
        for i in range(folds)[::-1][:-1]:
            q = Quantile(indices, quantiles[i])+1
            fold[:q] = [i for j in range(q)]
            
        #Now permute the fold assignments
        random.shuffle(fold)
        
    return fold
    

def ObjectifyTime(time_string):
    '''Create a time object from from a time string'''
    [hour, minute, second] = time_string.split(':')
    time_obj = datetime.time(hour=hour, minute=minute, second=second)
    return time_obj
 
 
def ObjectifyDate(date_string):
    '''Create a date object from from a date string'''
    try:
        #Try the easy way first
        date_obj = parser.parse(date_string)
        
    #Tokenize the string and make sure the tokens are integers
    except ValueError:
        try:
            date_string.index('/')
            values = date_string.split('/')
        except ValueError:
            date_string.index('.')
            values = date_string.split('.')
            values = map(int, values)
        
        #Create and return the date object
        try:
            date_obj = datetime.date(month=values[0], day=values[1], year=values[2])
        except ValueError:
            date_obj = datetime.date(month=values[1], day=values[2], year=values[0])
            
    return date_obj
 
 
def Julian(date):
    '''Get the number of days since the start of this year'''
    year_start = datetime.date(month=1, day=1, year=date.year)
    julian = (date.date() - year_start).days + 1
    return julian

 
def MatchDictionaries(dict1, dict2):
    '''Create a new dictionary that combines data from the matching keys of two separate dictionaries'''
    dict_matched = dict()
    
    for key in dict1.keys():
        if key in dict2:
            val_list = [list(dict1[key]), list(dict2[key])]
            val_list = flatten(val_list)
            dict_matched[key] = val_list

    return dict_matched
    
    
def flatten(x):
    '''flatten(sequence) -> list

    Returns a single, flat list which contains all elements retrieved
    from the sequence and all recursively contained sub-sequences
    (iterables).

    Examples:
    >>> [1, 2, [3,4], (5,6)]
    [1, 2, [3, 4], (5, 6)]
    >>> flatten([[[1,2,3], (42,None)], [4,5], [6], 7, MyVector(8,9,10)])
    [1, 2, 3, 42, None, 4, 5, 6, 7, 8, 9, 10]'''

    result = []
    for el in x:
        #if isinstance(el, (list, tuple)):
        if hasattr(el, "__iter__") and not isinstance(el, basestring):
            result.extend(flatten(el))
        else:
            result.append(el)
    return result

    
def ProbabilityOfExceedance(prediction, threshold, se):
    r = rdn.Wrap()
    return r.Call(function='pnorm', q=(prediction-threshold)/se).AsVector()[0]