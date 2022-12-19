using System;
using UnityEngine;
using Utils;
using Levels;
using TileInfo;
using Levels.Painters;
using Levels.Rooms;
using DungeonInstance;

namespace Levels.Rooms.Standard
{
	public class EntranceRoom : StandardRoom
	{
		public override int MinWidth => Math.Max(base.MinWidth, 5);
		public override int MinHeight =>  Math.Max(base.MinHeight, 5);


		public bool CanMerge(Level l, Vector2Int p, int mergeTerrain) => false;

		public override bool CanPlaceTrap(Vector2Int p) =>
			Dungeon.depth != 1 && base.CanPlaceTrap(p);

		public override void Paint(Level level )
		{

			Painter.Fill( level, this, Tile.Wall );
			Painter.Fill( level, this, 1, Tile.Empty );

			foreach(Room.Door door in connected.Values) {
				door.Set( Room.Door.Type.Regular );
			}

			int entrance = level.XYToCell(Random(2));
			//TODO REACTIVATE WHEN MOBS SPAWN
			//do
			//{
			//	entrance = level.XYToCell(Random(2));
			//} while (level.FindMob(entrance) != null);
			Painter.Set( level, entrance, Tile.Entrance );

			//TODO Entrance type
			//if (Dungeon.depth == 1){
			//	level.Transitions.Add(new LevelTransition(level, entrance, LevelTransition.Type.SURFACE));
			//} else {
			//	level.Transitions.Add(new LevelTransition(level, entrance, LevelTransition.Type.REGULAR_ENTRANCE));
			//}

			/* TODO: ADD TUTORIAL PAGES LATER
			//use a separate generator here so meta progression doesn't affect levelgen
			RandomNumberGenerator.AddGenerator();

			//places the first guidebook page on floor 1
			if (Dungeon.depth == 1 &&
					(!Document.ADVENTURERS_GUIDE.isPageRead(Document.GUIDE_INTRO) || SPDSettings.intro() )){
				int pos;
				do {
					//can't be on bottom row of tiles
					pos = level.XYToCell(new( RandomNumberGenerator.IntRange( left + 1, right - 1 ),
							RandomNumberGenerator.IntRange( top + 1, bottom - 2 )));
				} while (pos == level.entrance() || level.findMob(level.entrance()) != null);
				level.drop( new Guidebook(), pos );
			}

			//places the third guidebook page on floor 2
			if (Dungeon.depth == 2 && !Document.ADVENTURERS_GUIDE.isPageFound(Document.GUIDE_SEARCHING)){
				int pos;
				do
				{
					//can't be on bottom row of tiles
					pos = level.XYToCell(new( RandomNumberGenerator.IntRange( left + 1, right - 1 ),
							RandomNumberGenerator.IntRange( top + 1, bottom - 2 )));
				} while (pos == level.Entrance() || level.FindMob(level.Entrance()) != null);
				GuidePage p = new GuidePage();
				p.page(Document.GUIDE_SEARCHING);
				level.Drop( p, pos );
			}
			*/

			RandomNumberGenerator.AddGenerator();

		}
		//cannot connect to exit, otherwise works normally
		public override bool Connect(Room room) =>
		InstanceOf<ExitRoom>(room) ? false : base.Connect(room);
	}

}
