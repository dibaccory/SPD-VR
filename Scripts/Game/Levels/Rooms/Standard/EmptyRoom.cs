using System;
using Levels;
using TileInfo;
using Levels.Painters;
using Levels.Rooms;

namespace Levels.Rooms.Standard
{
	public class EmptyRoom : StandardRoom
	{

        public override void Paint(Level level)
        {
            Painter.Fill(level, this, Tile.Wall);
            Painter.Fill(level, this, 1, Tile.Empty);

            foreach (Door door in connected.Values)
            {
                door.Set(Door.Type.Regular);
            }
        }
    }

}
