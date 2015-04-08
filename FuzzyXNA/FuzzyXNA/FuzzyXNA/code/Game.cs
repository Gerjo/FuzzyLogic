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
        private float counter = float.PositiveInfinity;
        private Phantom.GameUI.UILayer ui;

        public Game() : base(620, 440, "Fuzzeh") {

            brain = new FuzzyLogic();

            brain.AddTermSet (
				property: 	"time", 
				min:	 	0, 
				max: 		60,
				terms: 		new [] { 
					new LinguisticTerm("long_Time",    new Ramp(1.0f)),
					new LinguisticTerm("med_time",     new Ramp(0.5f)),
					new LinguisticTerm("short_time",   new Ramp(0.0f))
				}
			);

            brain.AddTermSet(
                property: "entropy",
                min: 0,
                max: 1,
                terms: new[] { 
					new LinguisticTerm("very_entropy",  new Triangle(0.0f, 1.0f, 1.0f)),
					new LinguisticTerm("some_entropy",   new Triangle(0.0f, 0.5f, 1.0f)),
					new LinguisticTerm("no_entropy",   new Triangle(0.0f, 0.0f, 1.0f))
				}
            );

			brain.AddSubrule ("is_test", "med_time");

            brain.AddRuleSet("test rule", new[] { "med_time and some_entropy" },
				outcome: 	delegate {
					//System.Console.WriteLine("The other rule won.");

					return "";
				}
			);

        }

        protected override void Initialize()
        {
            var state    = new Phantom.Core.GameState();

            var renderer = new Phantom.Graphics.Renderer(1, Phantom.Graphics.Renderer.ViewportPolicy.Fill, Phantom.Graphics.Renderer.RenderOptions.Canvas | Phantom.Graphics.Renderer.RenderOptions.EnableClipping);
            
            state.AddComponent(layer = new Phantom.Core.RenderLayer(renderer));

            

            var font = Content.Load<SpriteFont>("Font");

           layer.AddComponent(new FuzzyXNA.FuzzyWindow(brain, font));


            Phantom.GameUI.UILayer.Font = font;

            ui = new Phantom.GameUI.UILayer(
                    new Phantom.Graphics.Renderer(1, Phantom.Graphics.Renderer.ViewportPolicy.Fill, Phantom.Graphics.Renderer.RenderOptions.Canvas | Phantom.Graphics.Renderer.RenderOptions.EnableClipping), 
                    1
                 );


            state.AddComponent(ui);

            PushState(state);

            base.Initialize();
        }

        public override void Update(float elapsed)
        {
            base.Update(elapsed);

            if ((counter += elapsed) > 1)
            {
                counter = 0;
                ++time;

                Dictionary<string, float> dict = new Dictionary<string, float>();

                dict["time"] = time;
                dict["entropy"] = (float)(new Random()).NextDouble();

                string uit = brain.Reason<string>(dict);
            }
        }
    
    }
}
