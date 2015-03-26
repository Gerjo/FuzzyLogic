using System;

namespace fuzzeh
{
	public class LinguisticTerm
	{
		readonly string name;
		readonly object shape;

		public LinguisticTerm (string name)
		{
			this.name = name;
			shape = null;
		}

		public string getName() {
			return name;
		}

		public object Evaluate(float value) {
			return value;
		}
	}
}

