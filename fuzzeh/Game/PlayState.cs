using System;
using Phantom.Core;
using Phantom.Graphics;

namespace fuzzeh
{
	public class PlayState : GameState
	{
		private RenderLayer renderer;

		public PlayState ()
		{
			this.renderer = new RenderLayer (new Renderer (1, Renderer.ViewportPolicy.Fit, Renderer.RenderOptions.Canvas));
			this.AddComponent (this.renderer);

			this.renderer.AddComponent (new LineGraph ());
		}
	}
}

