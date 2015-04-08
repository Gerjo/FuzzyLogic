using System;
using System.Collections.Generic;




namespace LibFuzzeh
{

	internal sealed class Token {
		public enum Type { Unknown, Number, Term, And, Or, Not, ParenOpen, ParenClose };
		public readonly Token.Type type = Token.Type.Unknown;

		public readonly string name;
		public readonly float value;

		public Token(float value) : this(Token.Type.Number, null) {
			this.value = value;
		}

		public Token(Token.Type type) : this(type, null) {}

		public Token(Token.Type type, string name) {
			this.type = type;
			this.name = name;
		}

	}

	public class Rule
	{
		private readonly string originalRule;
		private readonly Stack<Token> postfix;
		private readonly string name;
		private float lastScore = 0.0f;

		public Rule (string rule, string name)
		{
			var infix    = ParseTokens (this.originalRule = rule);
			this.postfix = InfixToPostfix (infix);
			this.name    = name;
		}

		public float GetLastScore() {
			return lastScore;
		}

        public string Name { get { return name;  } }

		public string GetName() {
			return name;
		}

		public string GetOriginalRule() {
			return originalRule;
		}

		private Stack<Token> ParseTokens(string rule) {

			Stack<Token> tokens = new Stack<Token>();

			string token = "";

			for (int i = 0; i < rule.Length; ++i) {
				char c = rule [i];

				if (c == ' ' || c == '(' || c == ')'||  i == rule.Length - 1) {
					
					if(c == '(') {
						tokens.Push (new Token(Token.Type.ParenOpen));
					}

					if (token.Length > 0) {
						
						if (token == "and") {
							tokens.Push (new Token (Token.Type.And));
						} else if (token == "or") {
							tokens.Push (new Token (Token.Type.Or));
						} else if (token == "not") {
							tokens.Push (new Token (Token.Type.Not));
						} else {

							// If required, append the last character of a rule
							// to complete the token.
							if (i == rule.Length - 1 && c != ' ' && c != '(' && c != ')') {
								token += c;
							}

							tokens.Push (new Token (Token.Type.Term, token));
						}
					}

					if(c == ')') {
						tokens.Push (new Token(Token.Type.ParenClose));
					}

					token = "";
				} else {

					// Strings are immutable, so this might be 'slow' (i.e. linear-time)?
					token += c;
				}

			}

			return tokens;
		}

		private Stack<Token> InfixToPostfix(Stack<Token> tokens) {
			Stack<Token> output = new Stack<Token>();
			Stack<Token> stack  = new Stack<Token>();

			foreach (Token token in tokens) {
			
				if (token.type == Token.Type.Term) {
					output.Push (token);
				
				} else if (token.type == Token.Type.ParenOpen || stack.Count == 0 || stack.Peek ().type == Token.Type.ParenOpen) {
					stack.Push (token);
				
				} else if (token.type == Token.Type.And || token.type == Token.Type.Or || token.type == Token.Type.Not) {
					stack.Push (token);

				} else if (token.type == Token.Type.ParenClose) {
					while (stack.Count > 0) {
						Token stackToken = stack.Pop ();

						if (stackToken.type == Token.Type.ParenClose) {
							break;
						}

						if (stackToken.type != Token.Type.ParenOpen) {
							output.Push (stackToken);
						}

					}
				}
			}


			// Push the remainder onto the output stack
			while(stack.Count > 0) {
				var token = stack.Pop();
				if(token.type != Token.Type.ParenOpen && token.type != Token.Type.ParenClose) {
					output.Push(token);
				}
			}

			return output;
		}

		public float Evaluate(IFuzzyOperators ops, IDictionary<string, float> terms) {
		
			var output = new Stack<Token>(postfix); // Intentional shallow copy
			var operators = new Stack<Token>();


			Func<Token, float> GetValue = delegate(Token t) {

				// Token represents a number, use as-is
				if (t.type == Token.Type.Number) {
					return t.value;

				// Lookup value in dictionary
				} else if (terms.ContainsKey (t.name)) {
					return terms [t.name];
				}

				throw new Exception ("Attempt to use undefined linguistic term: '" + t.name + "'");
			};

			while (output.Count > 0) {
				Token token = output.Pop ();

				if (token.type == Token.Type.Not) {
					float value = GetValue(operators.Pop ());
					
					Token result = new Token(ops.Negate(value));
					
					operators.Push (result);

				} else if(token.type == Token.Type.And || token.type == Token.Type.Or) {

					float[] values = new float[] { 
						GetValue(operators.Pop ()),
						GetValue(operators.Pop ())
					};

					Token result;

					if (token.type == Token.Type.And) {
						result = new Token (ops.And (values [0], values [1]));
					} else {
						result = new Token (ops.Or (values [0], values [1]));
					}

					operators.Push (result);

				} else {
					operators.Push(token);
				}
			}

			lastScore = GetValue (operators.Pop ());

			return lastScore;
		}
	}
}
