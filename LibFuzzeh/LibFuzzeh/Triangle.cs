using System;

namespace LibFuzzeh
{
	public class Triangle : Trapezoid
	{
		public Triangle (float start, float peak, float end) 
			: base(start, peak, peak, end)
		{
		}
	}
}

