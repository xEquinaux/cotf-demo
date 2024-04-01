using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf_rewd
{
    public class rand
    {
        static Random random;
        static int seed;
        public rand()
        {
            random = new Random(seed);
        }
        public void Seed()
        {
            if (seed < int.MaxValue)
                seed++;
            else seed = int.MinValue;
            random = new Random(seed);
        }
        public int Next(int max)
        {
            return random.Next(Math.Max(1, max));
        }
        public int Next(int min, int max)
        {
            return random.Next(min, max);
        }
        public bool NextBool()
        {
            return random.Next(2) == 1;
        }
        public double NextFloat()
        {
            return (float)random.NextDouble();
        }
        public double NextDouble()
        {
            return random.NextDouble();
        }
    }
}
