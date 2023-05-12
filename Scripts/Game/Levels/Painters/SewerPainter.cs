using System.Collections.Generic;
using Levels.Rooms;
using TileInfo;
using Utils;

namespace Levels.Painters
{
	public class SewerPainter : RegularPainter
	{
		public SewerPainter()
		{
		}

        protected override void Decorate(Level level, List<Room> rooms)
        {

            Tile[] map = level.map;
            int w = level.Width;
            int l = level.Length;

            for (int i = 0; i < w; i++)
            {
                if (map[i] == Tile.Wall &&
                        map[i + w] == Tile.Water &&
                        RandomNumberGenerator.Int(4) == 0)
                {

                    map[i] = Tile.WallDecor;
                }
            }

            for (int i = w; i < l - w; i++)
            {
                if (map[i] == Tile.Wall &&
                        map[i - w] == Tile.Wall &&
                        map[i + w] == Tile.Water &&
                        RandomNumberGenerator.Int(2) == 0)
                {

                    map[i] = Tile.WallDecor;
                }
            }

            for (int i = w + 1; i < l - w - 1; i++)
            {
                if (map[i] == Tile.Empty)
                {

                    int count =
                            (map[i + 1] == Tile.Wall ? 1 : 0) +
                                    (map[i - 1] == Tile.Wall ? 1 : 0) +
                                    (map[i + w] == Tile.Wall ? 1 : 0) +
                                    (map[i - w] == Tile.Wall ? 1 : 0);

                    if (RandomNumberGenerator.Int(16) < count * count)
                    {
                        map[i] = Tile.EmptyDecor;
                    }
                }
            }
        }

    }
}
