using System;
using System.Collections.Generic;
using System.Text;

namespace FeedEffects
{
    // all of these are meant to be drop-in replacements for Math.sin to make it easy.
    public abstract class PeriodicAlternative
    {
        public virtual double GetValue(double x) { return 0;  }
    }

    public class PASin : PeriodicAlternative
    {
        public override double GetValue(double x) { return Math.Sin(x); }
    }

    public class PASquare : PeriodicAlternative
    {
        public override double GetValue(double x) { return Math.Sign(Math.Sin(x)); } //pretty inefficent...
    }

    public class PASawtooth : PeriodicAlternative
    {
        public override double GetValue(double x)
        {
            x = x % (Math.PI * 2);
            return ((x / (Math.PI * 2)) * 2) - 1;
        } 
    }
    public class PATri : PeriodicAlternative
    {
        public override double GetValue(double x)
        {
            x = x % (Math.PI * 2);
            x = x / (Math.PI*2); // now x goes from 0 to 1
            if (x > 0.5)
            {
                return -1 + (x - 0.5) * 4;
            }
            else
            {
                return 1 - (x) * 4;
            }
        }
    }

    public class PAChangeSquare : PeriodicAlternative
    {
        private int count = 0;
        private double freq = 0.25;
        private PeriodicAlternative palt = new PASin();
        public PAChangeSquare() { }
        public PAChangeSquare(double f) { this.freq = f; }
        public override double GetValue(double x) 
        {
            count++; // note that this has state !! !! !!
            x = x % (Math.PI * 2);
            double cutoff = palt.GetValue(count * freq * 2.0 * Math.PI / (double)44100.0) * 0.45 + 0.5;//0.5;
            
            cutoff = cutoff * 2.0 * Math.PI;
            if (x > cutoff)
                return 1;
            else
                return -1;
        }
    }
}
