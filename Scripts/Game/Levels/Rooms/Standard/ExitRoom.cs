using System;
using Levels;
using Terrain;
using Levels.Painters;
using Levels.Rooms;

namespace Levels.Rooms.Standard
{
	public class ExitRoom : StandardRoom
	{
		public ExitRoom()
		{
				public override int minWidth() {
				return Math.max(super.minWidth(), 5);
			}

				public override int minHeight() {
				return Math.max(super.minHeight(), 5);
			}

				public void paint(Level level) {

				Painter.fill( level, this, Terrain.WALL );
				Painter.fill( level, this, 1, Terrain.EMPTY );

				for (Room.Door door : connected.values()) {
					door.set( Room.Door.Type.REGULAR );
				}

				int exit = level.pointToCell(random( 2 ));
				Painter.set( level, exit, Terrain.EXIT );
				level.transitions.add(new LevelTransition(level, exit, LevelTransition.Type.REGULAR_EXIT));
			}

				public override boolean canPlaceCharacter(Point p, Level l) {
				return super.canPlaceCharacter(p, l) && l.pointToCell(p) != l.exit();
			}

				public override boolean connect(Room room) {
				//cannot connect to entrance, otherwise works normally
				if (room instanceof EntranceRoom)   return false;
				else                            return super.connect(room);
			}
		}
	}

}
