using System;
using System.Collections.Generic;


namespace fuzzeh
{
	public interface IDefuzzification
	{
		RuleSet GetWinner(	
			IDictionary<string, float> terms,
			IEnumerable<RuleSet> rulesets,
			IFuzzyOperators ops
		);
	}
}

