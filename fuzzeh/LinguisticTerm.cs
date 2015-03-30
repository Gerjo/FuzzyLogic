using System;

namespace fuzzeh
{

	public class LinguisticTerm
	{
		private readonly string name;
		private readonly IMembershipFunction shape;

		public LinguisticTerm (string name, IMembershipFunction shape)
		{
			this.name  = name;
			this.shape = shape;
		}

		public string GetName() {
			return name;
		}

		public float Evaluate(float value) {
			return shape.Apply(value);
		}
	}
}