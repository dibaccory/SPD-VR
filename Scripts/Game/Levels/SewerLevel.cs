using System;
using Utils;

namespace Levels
{
	public class SewerLevel : RegularLevel
	{
		public SewerLevel()
		{
		}

		protected override int StandardRooms(bool forceMax) {
			if (forceMax) return 6;
			//4 to 6, average 5
			return 4+RandomNumberGenerator.Chances(new float[]{1, 3, 1});
		}

		protected override int SpecialRooms(bool forceMax) {
			if (forceMax) return 2;
			//1 to 2, average 1.8
			return 1+RandomNumberGenerator.Chances(new float[]{1, 4});
		}

		// protected override Painter Painter() {
		// 	return new SewerPainter()
		// 			.setWater(feeling == Feeling.WATER ? 0.85f : 0.30f, 5)
		// 			.setGrass(feeling == Feeling.GRASS ? 0.80f : 0.20f, 4)
		// 			.setTraps(nTraps(), trapClasses(), trapChances());
		// }

	}

}
