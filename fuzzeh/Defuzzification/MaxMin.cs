using System;
using System.Collections.Generic;


namespace fuzzeh
{
	public class MaxMin : IDefuzzification
	{
		public MaxMin () {
		}

		public RuleSet GetWinner (
			IDictionary<string, float> terms,
			IEnumerable<RuleSet> rulesets,
			IFuzzyOperators ops
		) {

			RuleSet winner = null;
			float bestRuleSetScore = float.NegativeInfinity;

			foreach(RuleSet ruleset in rulesets) {
				float bestRuleScore = float.NegativeInfinity;

				// Find max rule
				foreach (Rule rule in ruleset.GetRules()) {
					float score = rule.Evaluate(ops, terms);

					if (score > bestRuleScore) {
						bestRuleScore = score;
					}
				}

				// Find max ruleset
				if (bestRuleScore > bestRuleSetScore) {
					winner           = ruleset;
					bestRuleSetScore = bestRuleScore;
				}
			}

			return winner;
		}
	}
}

