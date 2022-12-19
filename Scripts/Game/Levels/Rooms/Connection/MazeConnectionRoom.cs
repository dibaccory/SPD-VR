using System;
using Levels.Features;
using Levels.Painters;
using TileInfo;
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
			bool[,] maze = Maze.Generate(this);

			Painter.Fill(level, this, 1, Tile.Empty);
			for (int x = 0; x < maze.Length; x++)
				for (int y = 0; y < maze.GetLength(0); y++) {
					if (maze[x,y] == Maze.FILLED) {
						Painter.Fill(level, x + left, y + top, 1, 1, Tile.Wall);
					}
				}

			foreach(Door door in connected.Values) {
				door.Set( Door.Type.Hidden );
			}
		}
		
		public override int MaxConnections(int direction) {
			return 2;
		}
	}
}
