using System;
using System.Collections.Generic;


namespace LibFuzzeh
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

