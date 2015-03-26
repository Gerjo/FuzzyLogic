using System;
using System.Collections.Generic;

namespace fuzzeh
{
	public class FuzzyLogic
	{
		private readonly List<Rule> rules    = new List<Rule>();
		private readonly List<Rule> subrules = new List<Rule>();
		private readonly List<TermSet> termsets   = new List<TermSet>();

		private readonly IFuzzyOperators operators;

		public FuzzyLogic(IFuzzyOperators operators = null)
		{
			this.operators = operators ?? new ZadehOperators();
		}

		public void AddRule(string name, string rule, object outcome) {
			Console.WriteLine (name + ": " + rule);

			rules.Add (new Rule (rule));
		}

		public void AddSubrule(string name, string rule) {
			subrules.Add (new Rule (rule));
		}

		public void AddTermSet(string property, float min, float max, params LinguisticTerm[] terms) {

			Console.WriteLine ("Term: " + property);

			var set = new TermSet (property, min, max, terms);

			foreach(var term in terms) {
				Console.WriteLine("    " + term.getName());
			}

			termsets.Add (set);
		}
			
		public IDictionary<string, float> ComputeTerms() {
			IDictionary<string, float> dict = new Dictionary<string, float>();

			foreach(var set in termsets) {
				set.Evaluate(ref dict);
			}

			return dict;
		}

		public object Reason(IDictionary<string, float> context) {
			// TODO: implement this.
			return new object();
		}

		public object Reason(IFuzzyLogicContext context) {

			IDictionary<string, float> termValues = ComputeTerms();

			foreach (var pair in termValues) {
				Console.WriteLine (pair.Key + "\t: " + pair.Value);
			}

			Dictionary<Rule, float> scores = new Dictionary<Rule, float>();

			foreach (var rule in rules) {
				scores [rule] = rule.Evaluate (termValues);
			}

			return new object();
		}
	}
}

