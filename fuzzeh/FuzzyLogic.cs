using System;
using System.Collections.Generic;

namespace fuzzeh
{
	public class FuzzyLogic
	{
		private readonly List<RuleSet> rulesets = new List<RuleSet>();
		private readonly List<Rule> subrules    = new List<Rule>();
		private readonly List<TermSet> termsets = new List<TermSet>();

		private readonly IFuzzyOperators operators;
		private readonly IDefuzzification defuzzification;

		public FuzzyLogic(IFuzzyOperators operators = null, IDefuzzification defuzzification = null) {
			this.operators = operators ?? new ZadehOperators();
			this.defuzzification = defuzzification ?? new MaxMin();
		}

		public void AddRule(string name, string rule, Action outcome) {
			AddRuleSet(name, new [] { rule }, (object) outcome);
		}

		public void AddRule<T>(string name, string rule, Func<T> outcome) {
			AddRuleSet(name, new [] { rule }, (object) outcome);
		}

		public void AddRule(string name, string rule, object outcome) {
			AddRuleSet(name, new [] { rule }, outcome);
		}

		public void AddRuleSet<T>(string name, string[] rules, Func<T> outcome) {
			AddRuleSet(name, rules, (object) outcome);
		}

		public void AddRuleSet(string name, string[] rules, Action outcome) {
			AddRuleSet (name, rules, (object) outcome);
		}

		public void AddRuleSet(string name, IEnumerable<string> rules, object outcome) {

			RuleSet ruleset = new RuleSet (name, outcome);


			foreach(string rule in rules) {
				ruleset.Add(new Rule(rule, outcome));
			}

			rulesets.Add (ruleset);
		}

		public void AddSubrule(string name, string rule) {
			subrules.Add (new Rule (rule, null));
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
			RuleSet winner = GetWinner(context);

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

			RuleSet winner = GetWinner(context);

			object outtype = winner.GetOutType ();

			if (outtype is Action) {
				(outtype as Action).Invoke ();
			} else {
				throw new Exception ("Strong documentation required here.");
			}
		}

		private RuleSet GetWinner(IFuzzyLogicContext context) {

			if (rulesets.Count == 0) {
				throw new Exception ("Cannot compute winning rule(set), there are no rule(sets) defined.");
			}

			var terms = ComputeTerms (context);

			return defuzzification.GetWinner(terms, rulesets, operators);
		}
	}
}

