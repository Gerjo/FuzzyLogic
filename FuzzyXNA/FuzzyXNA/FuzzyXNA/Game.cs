using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Phantom;
using LibFuzzeh;

namespace FuzzyXNA
{
    public class Game : PhantomGame {

        private FuzzyLogic brain;
        private int time = 0;

        public Game() : base(320, 240, "Fuzzeh") {
            brain = new FuzzyLogic();

            brain.AddTermSet (
				property: 	"time", 
				min:	 	0, 
				max: 		10000,
				terms: 		new [] { 
					new LinguisticTerm("short_time",  new Triangle(0.0f, 1.0f, 1.0f)),
					new LinguisticTerm("med_time",    new Triangle(0.0f, 0.5f, 1.0f)),
					new LinguisticTerm("long_Time",   new Triangle(0.0f, 0.0f, 1.0f))
				}
			);

			brain.AddSubrule ("is_test", "med_time");

			brain.AddRuleSet ("test rule", new [] { "is_test", "short_time", "not short_time" },
				outcome: 	delegate {
					System.Console.WriteLine("The other rule won.");

					return "";
				}
			);

        }

        public override void Update(float elapsed)
        {
            base.Update(elapsed);

            ++time;

            Dictionary<string, float> dict = new Dictionary<string, float>();

            dict["time"] = time;

            string uit = brain.Reason<string>(dict);


            string str = "";

            foreach (var wrapper in brain.GetRuleHistory())
            {

                foreach (KeyValuePair<Rule, Queue<float>> pair in wrapper.Value)
                {
                    Rule rule = pair.Key;
                    IEnumerable<float> history = pair.Value;

                    foreach (float value in history)
                    {
                        str += " " + value;
                    }
                }
            }

            System.Console.WriteLine(str);
        }
    
    }
}
