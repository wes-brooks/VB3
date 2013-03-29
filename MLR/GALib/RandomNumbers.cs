using System;
using System.Collections.Generic;
using System.Text;

namespace GALib
{
    
    public class RandomNumbers
    {
        static Random _random;

        static public void SetRandomSeed()
        {            
            _random = new Random();            
        }
        static public void SetRandomSeed(int seed)
        {
            _random = new Random(seed);            
        }

        static public int NextInteger()
        {
            checkGenerator();
            return _random.Next();
        }

        static public int NextInteger(int maxValue)
        {
            checkGenerator();
            return _random.Next(maxValue);
        }

        static public int NextInteger(int minValue, int maxValue)
        {
            checkGenerator();
            return _random.Next(minValue, maxValue);
        }

        static public double NextDouble()
        {
            checkGenerator();
            return _random.NextDouble();
        }

        static public void checkGenerator()
        {
            if (_random == null)
                throw new Exception("Random number generator has not been initialized.");
        }
    }
}
