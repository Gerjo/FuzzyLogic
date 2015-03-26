using System;
using System.Collections.Generic;

namespace fuzzeh
{
	public class TermSet
	{
		private readonly List<LinguisticTerm> terms = new List<LinguisticTerm>();
		private readonly float min;
		private readonly float max;
		private readonly string property;

		public TermSet (string property, float min, float max, IEnumerable<LinguisticTerm> terms)
		{
			this.property = property;
			this.min      = min;
			this.max      = max;

			// TODO: lookup how to insert a whole range without an explicit foreach loop.
			foreach(var term in terms) {
				this.terms.Add(term);
			}
		}

		public void Evaluate(ref IDictionary<string, float> output) {
			foreach(var term in terms) {
				output [term.getName ()] = 9.99f;
			}
		}
	}
}

