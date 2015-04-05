using System;

namespace LibFuzzeh
{
	public class Trapezoid : IMembershipFunction
	{
		private readonly float left;
		private readonly float first;
		private readonly float second;
		private readonly float right;

		public Trapezoid (float left, float first, float second, float right) {
			this.left   = left;
			this.first  = first;
			this.second = second;
			this.right  = right;
		}

		public float Apply (float value) {
			if(value < left || value > right) {
				return 0.0f;
			}

			if(value >= first && value <= second) {
				return 1.0f;
			}

			if(value >= left && value < first) {
				return (value - left) / (first - left);
			}

			return (1.0f - value - (1.0f - right)) / (right - second);
		}
	}
}

