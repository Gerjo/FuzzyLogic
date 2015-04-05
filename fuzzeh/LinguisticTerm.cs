using System;

namespace fuzzeh
{

	public class LinguisticTerm
	{
		private readonly string name;
		private readonly IMembershipFunction shape;
		private float lastScore = 0.0f;

		public LinguisticTerm (string name, IMembershipFunction shape)
		{
			this.name  = name;
			this.shape = shape;
		}

		public float GetLastScore() {
			return lastScore;
		}

		public string GetName() {
			return name;
		}

		public float Evaluate(float value) {
			lastScore = shape.Apply(value);
			return lastScore;
		}
	}
}