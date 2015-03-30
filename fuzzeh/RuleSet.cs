using System;
using System.Collections.Generic;

namespace fuzzeh
{
	public class RuleSet
	{
		private readonly List<Rule> rules = new List<Rule>();
		private readonly object outtype;
		private readonly string name;

		public RuleSet (string name, object outtype) {
			this.name    = name;
			this.outtype = outtype;
		}

		public IEnumerable<Rule> GetRules() {
			return rules;
		}

		public void Add(Rule rule) {
			rules.Add(rule);
		}

		public bool Remove(Rule rule) {
			return rules.RemoveAll (tmp => tmp == rule) > 0;
		}

		public bool Has(Rule rule) {
			return rules.Contains (rule);
		}

		public object GetOutType() {
			return outtype;
		}

		public string GetName() {
			return name;
		}
	}
}
