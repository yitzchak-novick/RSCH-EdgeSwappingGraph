using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSwappingSearch
{
    public class RandomGenerator
    {
        private static Lazy<Random> SharedRandom = new Lazy<Random>(() => new Random());
        private Random random;

        private RandomGenerator()
        {
            this.random = new Random(SharedRandom.Value.Next());
        }

        // not sure why, I prefer to hide the constructor to make it obvious to the 
        // caller that we are generating a new one from a random number
        public static RandomGenerator NextRandomGenerator()
        {
            return new RandomGenerator();
        }

        public int NextInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public int NextInt()
        {
            return random.Next();
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }

        public bool GetTrueWithProbability(double p)
        {
            return random.NextDouble() < p;
        }

        public bool NextBool()
        {
            return GetTrueWithProbability(0.5);
        }
    }
}
