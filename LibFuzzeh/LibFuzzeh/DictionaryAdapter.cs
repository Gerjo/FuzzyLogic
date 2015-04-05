using System;
using System.Collections.Generic;


namespace LibFuzzeh
{
	public class DictionaryAdapter : IFuzzyLogicContext
	{
		private readonly IDictionary<string, float> lookuptable;

		public DictionaryAdapter (IDictionary<string, float> lookuptable) {
			this.lookuptable = lookuptable;
		}
			
		public float GetFuzzyProperty (string name) {

			if (lookuptable.ContainsKey (name)) {
				return lookuptable [name];
			}

			throw new Exception ("Dictionary is missing value for key: '" + name + "'.");
		}
	}
}

