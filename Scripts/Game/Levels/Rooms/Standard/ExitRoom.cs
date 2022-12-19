using System;
using Utils;
using UnityEngine;
using TileInfo;
using Levels.Painters;
using Levels.Rooms;

namespace Levels.Rooms.Standard
{
	public class ExitRoom : StandardRoom
	{
		public override int MinWidth => Math.Max(base.MinWidth, 5);

		public override int MinHeight => Math.Max(base.MinHeight, 5);

		public override void Paint(Level level)
		{

			Painter.Fill( level, this, Tile.Wall );
			Painter.Fill( level, this, 1, Tile.Empty );

			foreach(Room.Door door in connected.Values)
			{
				door.Set( Room.Door.Type.Regular );
			}

			int exit = level.XYToCell(Random( 2 ));
			Painter.Set( level, exit, Tile.Exit );
			//TODO fix whenever u can
			//level.Transitions.add(new LevelTransition(level, exit, LevelTransition.Type.REGULAR_EXIT));
		}

		public override bool CanPlaceCharacter(Vector2Int p, Level l) {
		return base.CanPlaceCharacter(p, l) && l.XYToCell(p) != l.Exit();
	}

		public override bool Connect(Room room) {
		//cannot connect to entrance, otherwise works normally
		if (Room.InstanceOf<EntranceRoom>(room))   return false;
		else                            return base.Connect(room);
		}
		}
	}

}
