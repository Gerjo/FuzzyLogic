using System;

namespace fuzzeh
{
	public interface IFuzzyOperators
	{
		float And(float a, float b);
		float Or(float a, float b);
		float Negate(float a);
	}
}

