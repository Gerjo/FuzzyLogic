using System;
using System.Collections.Generic;

namespace LibFuzzeh
{
	public sealed class TermSet
	{
		private readonly List<LinguisticTerm> terms = new List<LinguisticTerm>();
		private readonly float min;
		private readonly float max;
		private readonly float range;
		private readonly string property;

        private float lastScore = 0;

        public string Name { get { return property; } }
        public float Min { get { return min; } }
        public float Max { get { return max; } }
        public float LastScore { get { return lastScore; } }

		public TermSet (string property, float min, float max, IEnumerable<LinguisticTerm> terms) {
			this.property   = property;
			this.min        = min;
			this.max        = max;
			this.range      = max - min;

			// TODO: lookup how to insert a whole range without an explicit foreach loop.
			foreach(var term in terms) {
				this.terms.Add(term);
			}
		}

		public IEnumerable<LinguisticTerm> GetTerms() {
			return terms;
		}

		public void Evaluate(FuzzyLogic controller, IFuzzyLogicContext context, ref IDictionary<string, float> output) {
			
			float value = context.GetFuzzyProperty (property);

			float normalized = (value - min) / this.range;

			// Clamp to [0,1] range
			if (normalized < 0.0f) {
				normalized = 0.0f;
			} else if (normalized > 1.0f) {
				normalized = 1.0f;
			}

            lastScore = normalized;

			foreach(var term in terms) {
				output [term.GetName ()] = term.Evaluate(normalized);
			}
		}
	}
}
