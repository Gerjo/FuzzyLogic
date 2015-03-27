using System;
using System.Collections.Generic;

namespace fuzzeh
{
	public class FuzzyLogic
	{
		private readonly List<Rule> rules       = new List<Rule>();
		private readonly List<Rule> subrules    = new List<Rule>();
		private readonly List<TermSet> termsets = new List<TermSet>();

		private readonly IFuzzyOperators operators;

		public FuzzyLogic(IFuzzyOperators operators = null)
		{
			this.operators = operators ?? new ZadehOperators();
		}

		public void AddRule(string name, string rule, Action outcome) {
			rules.Add (	new Rule (rule, outcome));
		}

		public void AddRule(string name, string rule, object outcome) {
			rules.Add (	new Rule (rule, outcome));
		}

		public void AddRule<T>(string name, string rule, Func<T> outcome) {
			rules.Add (	new Rule (rule, outcome));
		}

		public void AddSubrule(string name, string rule) {
			subrules.Add (new Rule (rule));
		}

		public IFuzzyOperators GetOperators() {
			return operators;
		}

		public void AddTermSet(string property, float min, float max, IEnumerable<LinguisticTerm> terms) {

			Console.WriteLine ("Term: " + property);

			var set = new TermSet (property, min, max, terms);

			foreach(var term in terms) {
				Console.WriteLine("    " + term.GetName());
			}

			termsets.Add (set);
		}
			
		public IDictionary<string, float> ComputeTerms(IFuzzyLogicContext context) {
			IDictionary<string, float> dict = new Dictionary<string, float>();

			foreach(TermSet set in termsets) {
				set.Evaluate(this, context, ref dict);
			}

			return dict;
		}

		public void Reason(IDictionary<string, float> context) {
			Reason(new DictionaryAdapter(context));
		}

		public T Reason<T>(IFuzzyLogicContext context) {
			Rule winner = GetWinner (context);

			object outtype = winner.GetOutType ();

			if (outtype is Func<T>) {
				return (outtype as Func<T>).Invoke ();
			}

			if( ! (outtype is T)) {
				throw new Exception ("Template does not match rule return type.");
			}

			return (T)outtype;
		}

		public T Reason<T>(IDictionary<string, float> context) {
			return Reason<T>(new DictionaryAdapter (context));
		}

		public void Reason(IFuzzyLogicContext context) {

			Rule winner = GetWinner (context);

			object outtype = winner.GetOutType ();

			if (outtype is Action) {
				(outtype as Action).Invoke ();
			} else {
				throw new Exception ("Strong documentation required here.");
			}
		}

		private Rule GetWinner(IFuzzyLogicContext context) {
			IDictionary<string, float> termValues = ComputeTerms(context);

			foreach (var pair in termValues) {
				Console.WriteLine (pair.Key + "\t: " + pair.Value);
			}

			Dictionary<Rule, float> scores = new Dictionary<Rule, float>();

			float scoreBest = float.NegativeInfinity;
			Rule winner = null;

			foreach (var rule in rules) {
				float score   = rule.Evaluate (this.operators, termValues);
				scores [rule] = score;

				// Max defuzzification
				if (winner == null || score > scoreBest) {
					scoreBest = score;
					winner    = rule;
				}
			}

			return winner;
		}
	}
}

