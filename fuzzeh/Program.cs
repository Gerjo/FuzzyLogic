using System;

using Phantom;

namespace fuzzeh
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (Game game = new Game ()) {
				game.Run ();
			}

		}
	}
}
