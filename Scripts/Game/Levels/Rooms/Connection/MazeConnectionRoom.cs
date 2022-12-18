using System;
using Levels.Features;
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
			Painter.Fill(level, this, 1, Tile.Empty);

			//true = space, false = wall
			Maze.allowDiagonals = false;
			bool[][] maze = Maze.Generate(this);

			Painter.Fill(level, this, 1, Tile.Empty);
			for (int x = 0; x < maze.Length; x++)
				for (int y = 0; y < maze[0].Length; y++) {
					if (maze[x][y] == Maze.FILLED) {
						Painter.Fill(level, x + left, y + top, 1, 1, Tile.Wall);
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
