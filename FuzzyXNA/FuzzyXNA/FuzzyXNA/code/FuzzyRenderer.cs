using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LibFuzzeh;
using Phantom.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace FuzzyXNA
{
    public sealed class FuzzyRenderer : Phantom.Core.Entity
    {
        private readonly FuzzyLogic brain;
        private readonly SpriteFont font;



        // TODO: I'm not keen on this form of dependency injection. Refactor 
        //       when functionality of this class works.
        public FuzzyRenderer(FuzzyLogic brain, SpriteFont font) : base(Vector2.Zero) {
            this.brain = brain;
            this.font  = font;
        }

        public override void Render(RenderInfo info) {
            Canvas canvas = info.Canvas;
            SpriteBatch batch = info.Batch;

            float padding   = 10;
            Vector2 size    = new Vector2(250, 90);
            Vector2 boxSize = size + new Vector2(25, 25);

            Vector2 boxhSize = boxSize * 0.5f;
            Vector2 hsize    = size * 0.5f;
            Vector2 position = Position + new Vector2(20, 20);

            Color[] colors = new[] { Color.Orange, Color.Blue, Color.Yellow, Color.Red };

            foreach (var res in brain.GetTermHistory()) {
                TermSet termset = res.Key;

                // History of each term, we're not plotting this right now.
                Dictionary<LinguisticTerm, Queue<float>> terms = res.Value;

                // Draw background
                canvas.FillColor = Color.White;
                canvas.FillRect(position + boxhSize - (boxSize - size) * 0.5f, boxhSize, angle: 0);

                // Draw background
                canvas.FillColor = Color.LightSlateGray;
                canvas.FillRect(position + hsize, hsize, angle: 0);

                // Draw a centered graph title.
                Vector2 titleSize = font.MeasureString(termset.Name);
                // titleSize.x is casted to an integer to disable subpixel rendering.
                Vector2 fontPosition = position + new Vector2(hsize.X - (int)(titleSize.X * 0.5f), 0);
                batch.DrawString(font, termset.Name, fontPosition, Color.Black);

                // Draw the axis
                canvas.Begin();
                canvas.StrokeColor = Color.Black;
                canvas.MoveTo(position);
                canvas.LineTo(position + new Vector2(0, size.Y));
                canvas.LineTo(position + new Vector2(size.X, size.Y));
                canvas.Stroke();

                // Draw the axis' labels:
                batch.DrawString(font, "1", position + new Vector2(-8, 0), Color.Black);
                batch.DrawString(font, "0", position + new Vector2(-8, size.Y - 12), Color.Black);

                batch.DrawString(font, termset.Min.ToString(), position + new Vector2(0, size.Y), Color.Black);
                // Right align the label
                Vector2 labelSize = font.MeasureString(termset.Max.ToString());
                batch.DrawString(font, termset.Max.ToString(), position + new Vector2(size.X - labelSize.X, size.Y), Color.Black);

                Vector2 fontPos = position + new Vector2(10, 10);

                int i = 0;
                foreach (LinguisticTerm term in termset.GetTerms()) {

                    // Each term received a unique color.
                    Color color = colors[i++ % colors.Length];
                    
                    // Draw name of the linguistic term and its value.
                    batch.DrawString(font, term.Name + " " + term.LastScore.ToString("0.00"), fontPos, color);
                    fontPos += new Vector2(0, 14);

                    // Everything thus far is a trapezoid :D
                    if (term.Shape is Trapezoid) {
                        Trapezoid trapezoid = term.Shape as Trapezoid;

                        canvas.Begin();
                        canvas.StrokeColor = color;
                        canvas.MoveTo(position + new Vector2(size.X * 0,                size.Y));
                        canvas.LineTo(position + new Vector2(size.X * trapezoid.Left,   size.Y));
                        canvas.LineTo(position + new Vector2(size.X * trapezoid.First,  0));
                        canvas.LineTo(position + new Vector2(size.X * trapezoid.Second, 0));
                        canvas.LineTo(position + new Vector2(size.X * trapezoid.Right,  size.Y));
                        canvas.Stroke();

                        canvas.FillColor = color;
                        canvas.FillCircle(position + new Vector2(
                            termset.LastScore * size.X,
                            size.Y - term.LastScore * size.Y
                        ), 5);

                    } else {
                        // Someone defined a custom shape. Ergo requires custom drawing logic.
                        throw new NotImplementedException("The renderer does not understand the membership function.");
                    }

                }

                // Offset downwards.
                position += new Vector2(0, boxSize.Y + padding);
            }


            base.Render(info);
        }
    }
}
