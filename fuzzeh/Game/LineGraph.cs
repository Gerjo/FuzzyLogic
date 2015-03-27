using System;
using Phantom.Core;
using Phantom.Graphics;
using Microsoft.Xna.Framework;

namespace fuzzeh
{
	public class Grahiefk : Phantom.Core.Entity
	{
		public Grahiefk()
			:base(Vector2.Zero)
		{
		}

		public override void Render (RenderInfo info)
		{
			var c = info.Canvas;
			c.FillColor = Color.White;
			c.FillCircle (new Microsoft.Xna.Framework.Vector2 (100, 100), 10);
			base.Render (info);
		}
	}

}

