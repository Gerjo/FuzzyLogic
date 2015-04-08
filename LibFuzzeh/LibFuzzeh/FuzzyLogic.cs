using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LibFuzzeh
{
	public class FuzzyLogic
	{
		private readonly List<RuleSet> rulesets = new List<RuleSet>();
		private readonly List<TermSet> termsets = new List<TermSet>();
		private readonly List<KeyValuePair<string, Rule>> subrules = new List<KeyValuePair<string, Rule>>();

		private readonly IFuzzyOperators operators;
		private readonly IDefuzzification defuzzification;

		private readonly int historySize;

		private readonly Dictionary<TermSet, Dictionary<LinguisticTerm, Queue<float>>> historyTerms;
		private readonly Dictionary<RuleSet, Dictionary<Rule, Queue<float>>> historyRules;
		private readonly Dictionary<Rule, Queue<float>> historySubRules;


		public FuzzyLogic(	IFuzzyOperators operators = null, 
							IDefuzzification defuzzification = null,
							int historySize = 40
		) {
			this.operators       = operators ?? new ZadehOperators();
			this.defuzzification = defuzzification ?? new MaxMin();
			this.historySize     = historySize;

			historyTerms 	= new Dictionary<TermSet, Dictionary<LinguisticTerm, Queue<float>>> ();
			historyRules	= new Dictionary<RuleSet, Dictionary<Rule, Queue<float>>> ();
			historySubRules = new Dictionary<Rule, Queue<float>> ();
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

			int i = 0;

			foreach(string rule in rules) {
				string uniqueName = name + i++.ToString ();
				ruleset.Add(new Rule(rule, uniqueName));
			}

			rulesets.Add (ruleset);
		}

		public void AddSubrule(string name, string rule) {

			if (subrules.Exists (pair => pair.Key == name)) {
				throw new Exception ("Cannot add subrule with name '" + name + "', it already exists.");
			}

			subrules.Add (
				new KeyValuePair<string, Rule>(name, new Rule(rule, name))
			);
		}

		public void AddTermSet(string property, float min, float max, IEnumerable<LinguisticTerm> terms) {

			Console.WriteLine ("Term: " + property);

			var set = new TermSet (property, min, max, terms);

			foreach(var term in terms) {
				Console.WriteLine("    " + term.GetName());
			}

			termsets.Add (set);
		}
			
		private IDictionary<string, float> ComputeTerms(IFuzzyLogicContext context) {
			IDictionary<string, float> dict = new Dictionary<string, float>();

			// TODO: Fix consistency issues. One foreach edits by ref, the other
			// directly sets key -> value pairs.

			// Compute all linqustic terms:
			foreach(TermSet set in termsets) {
				set.Evaluate(this, context, ref dict);
			}

			// Evaluate all sub rules, i.e., a rule defined as a linqustic term.
			foreach (var subrule in subrules) {
				float value = subrule.Value.Evaluate (operators, dict);
				dict [subrule.Key] = value;
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
				// TODO: Tell the programmer (me) what to do.
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

		private void RecordHistory() {
			// TODO: ring buffers.
			// TODO: create entries when terms/rules are created. This will reduce
			//       the number of .Contains calls.

			foreach (TermSet termset in termsets) {

				if ( ! historyTerms.ContainsKey (termset)) {
					historyTerms [termset] = new Dictionary<LinguisticTerm, Queue<float>> ();
				}

				foreach (LinguisticTerm term in termset.GetTerms()) {
					if ( ! historyTerms[termset].ContainsKey (term)) {
						historyTerms [termset][term] = new Queue<float> ();
					}

					for (historyTerms [termset][term].Enqueue (term.GetLastScore()); historyTerms [termset][term].Count > historySize;) {
						historyTerms [termset] [term].Dequeue ();
					}
				}
			}

			foreach (RuleSet ruleset in rulesets) {

				if ( ! historyRules.ContainsKey (ruleset)) {
					historyRules [ruleset] = new Dictionary<Rule, Queue<float>> ();
				}

				foreach (Rule rule in ruleset.GetRules()) {
					if ( ! historyRules[ruleset].ContainsKey (rule)) {
						historyRules [ruleset][rule] = new Queue<float> ();
					}

					for (historyRules [ruleset][rule].Enqueue (rule.GetLastScore()); historyRules [ruleset][rule].Count > historySize;) {
						historyRules [ruleset] [rule].Dequeue ();
					}
				}
			}

			foreach (var pair in subrules) {
				Rule subrule = pair.Value;

				if ( ! historySubRules.ContainsKey (subrule)) {
					historySubRules [subrule] = new Queue<float> ();
				}

				for (historySubRules [subrule].Enqueue (subrule.GetLastScore()); historySubRules [subrule].Count > historySize;) {
					historySubRules [subrule].Dequeue ();
				}
			}
		}

		private RuleSet GetWinner(IFuzzyLogicContext context) {

			if (rulesets.Count == 0) {
				throw new Exception ("Cannot compute winning rule(set), there are no rule(sets) defined.");
			}

			var terms = ComputeTerms (context);

			RuleSet winner = defuzzification.GetWinner(terms, rulesets, operators);

			if (historySize > 0) {
				RecordHistory ();
			}

			return winner;
		}

		public Dictionary<TermSet, Dictionary<LinguisticTerm, Queue<float>>> GetTermHistory() {
			return historyTerms;
		}

		public Dictionary<RuleSet, Dictionary<Rule, Queue<float>>> GetRuleHistory() {
			return historyRules;
		}

		public Dictionary<Rule, Queue<float>> GetSubRuleHistory() {
			return historySubRules;
		}
	}
}

