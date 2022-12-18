using System;
using Levels;
using Terrain;
using Levels.Painters;
using Levels.Rooms;

namespace Levels.Rooms.Standard
{
	public class EmptyRoom : StandardRoom
	{
		public EmptyRoom()
		{
			
		}

        public override void Paint(Level level)
        {
            Painter.Fill(level, this, (int)Tile.Wall);
            Painter.Fill(level, this, 1, (int)Tile.Empty);

            foreach (Door door in connected.Values)
            {
                door.Set(Door.Type.REGULAR);
            }
        }
    }

}
