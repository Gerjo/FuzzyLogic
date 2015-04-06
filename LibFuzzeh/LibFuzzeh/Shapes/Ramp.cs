using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibFuzzeh.Shapes
{
    public class Ramp : Trapezoid
    {
        public Ramp(float peak) : base(0.0f, peak, peak, 1.0f) { 
        
        }
    }
}
