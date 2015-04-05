using System;

namespace LibFuzzeh
{
	public interface IFuzzyLogicContext
	{
		// TODO: fix misleading name. Input is not -per se- fuzzy.
		float GetFuzzyProperty(string name);
	}
}
