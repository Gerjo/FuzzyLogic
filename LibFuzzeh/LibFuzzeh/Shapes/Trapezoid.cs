using System;

namespace LibFuzzeh
{
	public class Trapezoid : IMembershipFunction
	{
		public readonly float Left;
        public readonly float First;
        public readonly float Second;
        public readonly float Right;

		public Trapezoid (float left, float first, float second, float right) {
			this.Left   = left;
			this.First  = first;
			this.Second = second;
			this.Right  = right;
		}

		public float Apply (float value) {
			if(value < Left || value > Right) {
				return 0.0f;
			}

			if(value >= First && value <= Second) {
				return 1.0f;
			}

			if(value >= Left && value < First) {
				return (value - Left) / (First - Left);
			}

			return (1.0f - value - (1.0f - Right)) / (Right - Second);
		}
	}
}

