using System;

namespace LibFuzzeh
{
	public sealed class Crisp : IMembershipFunction
	{
		private readonly float degree;
		public Crisp (float degree) {
			this.degree = degree;
		}

		public float Apply (float value) {
			return degree;
		}

	}
}

