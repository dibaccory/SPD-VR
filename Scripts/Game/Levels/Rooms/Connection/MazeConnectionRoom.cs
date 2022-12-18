using System;
using Levels;
using Levels.Painters;
using Terrain;
using Levels.Rooms;

namespace Levels.Rooms.Connection
{
	public class MazeConnectionRoom : ConnectionRoom
	{
		public MazeConnectionRoom()
		{
		}

		public override void Paint(Level level) {
			Painter.fill(level, this, 1, Tile.Empty);

			//true = space, false = wall
			Maze.allowDiagonals = false;
			boolean[][] maze = Maze.generate(this);

			Painter.fill(level, this, 1, Tile.Empty);
			for (int x = 0; x < maze.length; x++)
				for (int y = 0; y < maze[0].length; y++) {
					if (maze[x][y] == Maze.FILLED) {
						Painter.fill(level, x + left, y + top, 1, 1, Tile.Wall);
					}
				}

			for (Door door : connected.values()) {
				door.set( Door.Type.HIDDEN );
			}
		}

		@Override
		public int maxConnections(int direction) {
			return 2;
		}
	}
}
