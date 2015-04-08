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
    public sealed class FuzzyWindow : Phantom.GameUI.UIElement
    {
        private readonly FuzzyLogic brain;
        private readonly SpriteFont font;

        private Vector2 windowSize;

        private float graphPadding = 5;

        private Vector2 size;
        private Vector2 boxSize;
        private Vector2 boxhSize;
        private Vector2 hsize;

        private float[] scrolloffset = new [] { 0f, 0f };
        Color[] colors = new[] { Color.Red, Color.Blue, Color.Purple, Color.Red };

        // TODO: I'm not keen on this form of dependency injection. Refactor 
        //       when functionality of this class works.
        public FuzzyWindow(FuzzyLogic brain, SpriteFont font, float width = 200f, float height = 320f) 
            : base("Fuzzy Logic", new Vector2(15, 15), new Phantom.Shapes.OABB(new Vector2(width/2.0f, height/2.0f))) {

            this.brain = brain;
            this.font  = font;


            windowSize = (this.Shape as Phantom.Shapes.OABB).HalfSize * 2.0f;

            boxSize = new Vector2((float) Math.Round((windowSize.X - graphPadding * 3) / 2.0f), 90);
            size = boxSize - new Vector2(25, 25);

            boxhSize = boxSize * 0.5f;
            hsize = size * 0.5f;
        }


        /// <summary>
        /// Draw the fuzzy logic background window and handlebar.
        /// </summary>
        private void DrawWindow(RenderInfo info) {

            Canvas canvas     = info.Canvas;
            SpriteBatch batch = info.Batch;

            canvas.FillColor = Color.Gray;
            canvas.StrokeColor = Color.Black;
            canvas.LineWidth = 3.0f;
            canvas.FillRect(Position + windowSize * 0.5f, windowSize * 0.5f, angle: 0);
            canvas.StrokeRect(Position + windowSize * 0.5f, windowSize * 0.5f, angle: 0);
        }

        private Vector2 DrawGraphBackground(RenderInfo info, Vector2 position, string title, string a, string b) {
            Canvas canvas = info.Canvas;
            SpriteBatch batch = info.Batch;

            // Draw background
            canvas.FillColor = Color.White;
            canvas.FillRect(position + boxhSize, boxhSize, angle: 0);

            // Draw backdrop
            canvas.FillColor = Color.Gainsboro;
            canvas.FillRect(position + boxhSize, hsize, angle: 0);

            position += new Vector2((float) Math.Round(boxhSize.X - hsize.X), (float)Math.Round(boxhSize.Y - hsize.Y));

            // Draw a centered graph title.
            float titleWidth = font.MeasureString(title).X;
            // titleSize.x is casted to an integer to disable subpixel rendering.
            Vector2 fontPosition = position + new Vector2((int)(hsize.X - titleWidth * 0.5f), -12);
            batch.DrawString(font, title, fontPosition, Color.Black);

            // Draw the axis
            canvas.Begin();
            canvas.StrokeColor = Color.Black;
            canvas.MoveTo(position);
            canvas.LineTo(position + new Vector2(0, size.Y));
            canvas.LineTo(position + new Vector2(size.X, size.Y));
            canvas.Stroke();


            // Draw the y-axis' labels:
            batch.DrawString(font, "1", position + new Vector2(-8, 0), Color.Black);
            batch.DrawString(font, "0", position + new Vector2(-8, size.Y - 12), Color.Black);

            // Draw the x-axis' labels:
            batch.DrawString(font, a, position + new Vector2(0, size.Y), Color.Black);
            // Right align the label
            float labelWidth = font.MeasureString(b).X;
            batch.DrawString(font, b, position + new Vector2(size.X - labelWidth, size.Y), Color.Black);

            return position;
        }

        private void DrawLinquisticTerms(RenderInfo info, Vector2 origin, TermSet termset)
        {
            Canvas canvas = info.Canvas;
            SpriteBatch batch = info.Batch;

            Vector2 fontPos = origin + new Vector2(10, 10);

                int i = 0;
                foreach (LinguisticTerm term in termset.GetTerms())
                {

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
                        canvas.MoveTo(origin + new Vector2(size.X * 0, size.Y));
                        canvas.LineTo(origin + new Vector2(size.X * trapezoid.Left, size.Y));
                        canvas.LineTo(origin + new Vector2(size.X * trapezoid.First, 0));
                        canvas.LineTo(origin + new Vector2(size.X * trapezoid.Second, 0));
                        canvas.LineTo(origin + new Vector2(size.X * trapezoid.Right, size.Y));
                        canvas.Stroke();

                        canvas.FillColor = color;
                        canvas.FillCircle(origin + new Vector2(
                            termset.LastScore * size.X,
                            size.Y - term.LastScore * size.Y
                        ), 3);

                    } else {
                        // Someone defined a custom shape. Ergo requires custom drawing logic.
                        throw new NotImplementedException("The renderer does not understand the membership function.");
                    }

                }
        }

        private void DrawRuleHistory(RenderInfo info, Vector2 origin, Queue<float> history) { 
            Canvas canvas = info.Canvas;

            canvas.Begin();
            canvas.StrokeColor = Color.Black;
            canvas.FillColor = Color.Black;

            int i = 0;

            foreach(float value in history) {
                float x = origin.X + size.X / (history.Count - 1) * i;
                float y = origin.Y + size.Y - (value * size.Y);

                if (i == 0)
                    canvas.MoveTo(x, y);
                else
                    canvas.LineTo(x, y);

                ++i;
            }

            canvas.Stroke();
        }

        public override void Render(RenderInfo info) {
            Canvas canvas = info.Canvas;
            SpriteBatch batch = info.Batch;

            Rectangle defaultScrissorRectangle = batch.GraphicsDevice.ScissorRectangle;

            // Enable clipping window.
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(
                (int)Position.X, 
                (int)Position.Y, 
                (int)Math.Min(windowSize.X, batch.GraphicsDevice.Viewport.Width),
                (int)Math.Min(windowSize.Y, batch.GraphicsDevice.Viewport.Height)
            );

            DrawWindow(info);

            canvas.LineWidth = 1.0f;

            Vector2 position = Position + new Vector2(graphPadding, graphPadding);

            foreach (var res in brain.GetTermHistory()) {
                TermSet termset = res.Key;

                // History of each term, we're not plotting this right now.
                Dictionary<LinguisticTerm, Queue<float>> terms = res.Value;

                // Draw line graph background.
                Vector2 origin = DrawGraphBackground(info, position, termset.Name, termset.Min.ToString(), termset.Max.ToString());

                // Draw linuistic term specifics.
                DrawLinquisticTerms(info, origin, termset);

                // Offset downwards.
                position += new Vector2(0, boxSize.Y + graphPadding);
            }

            position = Position + new Vector2(boxSize.X + graphPadding * 2, graphPadding);

            foreach (var res in brain.GetRuleHistory()) { 
                RuleSet ruleset = res.Key;
               
                foreach (var subres in res.Value) {
                    Rule rule = subres.Key;
                    Queue<float> history = subres.Value;

                    Vector2 origin = DrawGraphBackground(info, position, rule.Name, "0", "1");

                    DrawRuleHistory(info, origin, history);

                    // Offset downwards.
                    position += new Vector2(0, boxSize.Y + graphPadding);
                }

            }

            // ugh, this is cached, so doesn't work.
            //batch.GraphicsDevice.ScissorRectangle = defaultScrissorRectangle;

            base.Render(info);
        }
    }
}
