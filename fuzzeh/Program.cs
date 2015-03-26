using System;

namespace fuzzeh
{
	class Entity : IFuzzyLogicContext {
		private float bar = 2f;
		private float foo = 3f;
		private float meh = 0.3f;

		public float getProperty(string name) {
			if (name == "bar") {
				return bar;
			} else if (name == "foo") {
				return foo;
			} else if (name == "meh") {
				return meh;
			}

			return 0;
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			var entity = new Entity ();

			var brain = new FuzzyLogic ();

			brain.AddTermSet (
				"bar",
				0,
				100,
				new LinguisticTerm("little_bar"),
				new LinguisticTerm("very_bar"),
				new LinguisticTerm("lots_bar")
			);

			brain.AddRule ("test rule", "foo and bar or meh", null);

			brain.Reason (entity);
		}
	}
}
