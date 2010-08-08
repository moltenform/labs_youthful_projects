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

}
