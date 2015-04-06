using System;

namespace LibFuzzeh
{
	public class ZadehOperators : IFuzzyOperators
	{
		public float And(float a, float b) {
			return Math.Min(a, b);
		}

		public float Or(float a, float b) {
			return Math.Max(a, b);
		}

		public float Negate(float a) {
			return 1.0f - a;
		}
	}
}

