using System;

namespace fuzzeh
{
	public interface IFuzzyLogicContext
	{
		// TODO: fix misleading name. Input is not -per se- fuzzy.
		float GetFuzzyProperty(string name);
	}
}
