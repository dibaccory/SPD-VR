using System;
using Levels;
using Terrain;
using Levels.Painters;
using Levels.Rooms;
using DungeonInstance;

namespace Levels.Rooms.Standard
{
	public class EntranceRoom : StandardRoom
	{
		public EntranceRoom()
		{
				public override int minWidth() {
				return Math.max(super.minWidth(), 5);
			}

				public override int minHeight() {
				return Math.max(super.minHeight(), 5);
			}

				public override boolean canMerge(Level l, Point p, int mergeTerrain) {
				return false;
			}

				public override boolean canPlaceTrap(Point p) {
				if (Dungeon.depth == 1) {
					return false;
				} else {
					return super.canPlaceTrap(p);
				}
			}

			public void paint(Level level ) {

				Painter.fill( level, this, Terrain.WALL );
				Painter.fill( level, this, 1, Terrain.EMPTY );

				for (Room.Door door : connected.values()) {
					door.set( Room.Door.Type.REGULAR );
				}

				int entrance;
				do {
					entrance = level.pointToCell(random(2));
				} while (level.findMob(entrance) != null);
				Painter.set( level, entrance, Terrain.ENTRANCE );

				if (Dungeon.depth == 1){
					level.transitions.add(new LevelTransition(level, entrance, LevelTransition.Type.SURFACE));
				} else {
					level.transitions.add(new LevelTransition(level, entrance, LevelTransition.Type.REGULAR_ENTRANCE));
				}

				//use a separate generator here so meta progression doesn't affect levelgen
				Random.pushGenerator();

				//places the first guidebook page on floor 1
				if (Dungeon.depth == 1 &&
						(!Document.ADVENTURERS_GUIDE.isPageRead(Document.GUIDE_INTRO) || SPDSettings.intro() )){
					int pos;
					do {
						//can't be on bottom row of tiles
						pos = level.pointToCell(new Point( Random.IntRange( left + 1, right - 1 ),
								Random.IntRange( top + 1, bottom - 2 )));
					} while (pos == level.entrance() || level.findMob(level.entrance()) != null);
					level.drop( new Guidebook(), pos );
				}

				//places the third guidebook page on floor 2
				if (Dungeon.depth == 2 && !Document.ADVENTURERS_GUIDE.isPageFound(Document.GUIDE_SEARCHING)){
					int pos;
					do {
						//can't be on bottom row of tiles
						pos = level.pointToCell(new Point( Random.IntRange( left + 1, right - 1 ),
								Random.IntRange( top + 1, bottom - 2 )));
					} while (pos == level.entrance() || level.findMob(level.entrance()) != null);
					GuidePage p = new GuidePage();
					p.page(Document.GUIDE_SEARCHING);
					level.drop( p, pos );
				}

				Random.popGenerator();

			}

				public override boolean connect(Room room) {
				//cannot connect to exit, otherwise works normally
				if (room instanceof ExitRoom)   return false;
				else                            return super.connect(room);
			}
		}
	}

}
