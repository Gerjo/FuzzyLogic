using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LibFuzzeh;
using Phantom.Graphics;

namespace FuzzyXNA
{
    public class FuzzyRenderer : Phantom.Core.Entity
    {
        private FuzzyLogic brain;

        public FuzzyRenderer(FuzzyLogic brain) : base(Vector2.Zero) {
            this.brain = brain;
            
        }

        public override void Render(RenderInfo info) {
            Canvas canvas = info.Canvas;

            canvas.FillColor = Color.White;
            canvas.FillCircle(new Vector2(23, 23), 10);

            base.Render(info);
        }
    }
}
