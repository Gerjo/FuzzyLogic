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
using LibFuzzeh.Shapes;

namespace FuzzyXNA
{
    public class Game : PhantomGame {

        private FuzzyLogic brain;
        private int time = 0;
        private Phantom.Core.RenderLayer layer;

        public Game() : base(320, 240, "Fuzzeh") {

            brain = new FuzzyLogic();

            brain.AddTermSet (
				property: 	"time", 
				min:	 	0, 
				max: 		10000,
				terms: 		new [] { 
					new LinguisticTerm("long_Time",    new Ramp(1.0f)),
					new LinguisticTerm("med_time",     new Ramp(0.5f)),
					new LinguisticTerm("short_time",   new Ramp(0.0f))
				}
			);

            brain.AddTermSet(
                property: "entropy",
                min: 0,
                max: 500,
                terms: new[] { 
					new LinguisticTerm("very_lost",  new Triangle(0.0f, 1.0f, 1.0f)),
					new LinguisticTerm("med_lost",   new Triangle(0.0f, 0.5f, 1.0f)),
					new LinguisticTerm("not_lost",   new Triangle(0.0f, 0.0f, 1.0f))
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

        protected override void Initialize()
        {
            var state    = new Phantom.Core.GameState();
            
            var renderer = new Phantom.Graphics.Renderer(1, Phantom.Graphics.Renderer.ViewportPolicy.Fill, Phantom.Graphics.Renderer.RenderOptions.Canvas);
            
            state.AddComponent(layer = new Phantom.Core.RenderLayer(renderer));

            PushState(state);

            var font = Content.Load<SpriteFont>("Font");

            layer.AddComponent(new FuzzyXNA.FuzzyRenderer(brain, font));

            base.Initialize();
        }

        public override void Update(float elapsed)
        {
            base.Update(elapsed);

            ++time;

            Dictionary<string, float> dict = new Dictionary<string, float>();

            dict["time"] = time;
            dict["entropy"] = time;

            string uit = brain.Reason<string>(dict);
        }
    
    }
}
