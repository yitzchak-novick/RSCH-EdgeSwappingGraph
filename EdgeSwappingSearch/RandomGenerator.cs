using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSwappingSearch
{
    public class RandomGenerator
    {
        private static Random SharedRandom = new Random();
        private Random random;

        public RandomGenerator()
        {
            this.random = new Random(SharedRandom.Next());    
        }

        public int NextInt => random.Next();
        public double NextDouble => random.NextDouble();
        public bool GetTrueWithProbability(double p) => random.NextDouble() < p;
        public bool NextBool => GetTrueWithProbability(0.5);
    }
}
