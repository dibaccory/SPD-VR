using System;
using System.Collections.Generic;
using Utils;
using UnityEngine;
using Levels;
using Levels.Rooms;
using Levels.Rooms.Special;
using Levels.Rooms.Connection;
using DungeonInstance;
using TileInfo;
using Levels.Rooms.Standard;
using Unity.VisualScripting;
using UnityEditor;
using TMPro;
using UnityEngine.UIElements;

namespace Levels.Painters
{

    public abstract class RegularPainter : Painter
    {

        private float waterFill = 0f;
        private int waterSmoothness;

        public RegularPainter SetWater(float fill, int smoothness)
        {
            waterFill = fill;
            waterSmoothness = smoothness;
            return this;
        }

        private float grassFill = 0f;
        private int grassSmoothness;

        public RegularPainter SetGrass(float fill, int smoothness)
        {
            grassFill = fill;
            grassSmoothness = smoothness;
            return this;
        }

        private int nTraps = 0;
        private Type[] trapClasses;
        private float[] trapChances;

        public RegularPainter SetTraps<T>(int num, T[] classes, float[] chances) //where T : Trap
        {
            nTraps = num;
            trapClasses = classes as Type[];
            trapChances = chances;
            return this;
        }


        public override bool Paint(Level level, List<Room> rooms)
		{
		
		//painter can be used without rooms
		    if (rooms != null) {
			
			    int padding = level.feeling == Level.Feeling.CHASM ? 2 : 1;
			
			    int leftMost = Int32.MaxValue, topMost = Int32.MaxValue;
			
			    foreach(Room r in rooms) {
				    if (r.left < leftMost) leftMost = r.left;
				    if (r.top < topMost) topMost = r.top;
			    }
			
			    leftMost -= padding;
			    topMost -= padding;
			
			    int rightMost = 0, bottomMost = 0;
			
			    foreach(Room r in rooms) {
				    r.Shift(-leftMost, -topMost);
				    if (r.right > rightMost) rightMost = r.right;
				    if (r.bottom > bottomMost) bottomMost = r.bottom;
			    }
			
			    rightMost += padding;
			    bottomMost += padding;
			
			    //add 1 to account for 0 values
			    level.SetSize(rightMost + 1, bottomMost + 1);
		    }
			else
			{
			    //check if the level's size was already initialized by something else
			    if (level.Length == 0) return false;
			
			    //easier than checking for null everywhere
			    rooms = new();
		    }
		
			RandomNumberGenerator.Shuffle(rooms);
		
			foreach(Room r in rooms) {
				if (r.connected.Count == 0)
				{
					//Game.reportException( new RuntimeException("Painting a room with no connections! Room:" + r.getClass().getSimpleName() + " Seed:" + Dungeon.seed + " Depth:" + Dungeon.depth));
					if (r is SpecialRoom) return false;
				}
				PlaceDoors( r );
				r.Paint( level );
			}
		
			PaintDoors( level, rooms );
		
			if (waterFill > 0f) {
				//PaintWater( level, rooms );
			}
		
			if (grassFill > 0f){
				//PaintGrass( level, rooms );
			}
		
			//if (nTraps > 0){
			//	PaintTraps( level, rooms );
			//}
		
			Decorate( level, rooms );
		
			return true;
		}
	
		protected abstract void Decorate(Level level, List<Room> rooms);
	
		private void PlaceDoors( Room r )
		{
			foreach(Room n in r.connected.Keys)
			{
				Room.Door door = r.connected[n];
				if (door == null)
				{
					Rectangle i = r.rect.Intersect( n );
					List<Vector2Int> doorSpots = new();
					foreach(Vector2Int p in i.GetPoints())
					{
						if (r.CanConnect(p) && n.CanConnect(p))
							doorSpots.Add(p);
					}
					if (doorSpots.Count == 0)
					{
						//ShatteredPixelDungeon.reportException(
						//		new RuntimeException("Could not place a door! " +
						//				"r=" + r.getClass().getSimpleName() +
						//				" n=" + n.getClass().getSimpleName()));
						continue;
					}
					door = new Room.Door(RandomNumberGenerator.Element(doorSpots));
				
					r.connected.Add( n, door );
					n.connected.Add( r, door );
				}
			}
		}

        protected void PaintDoors(Level l, List<Room> rooms)
        {

            float hiddenDoorChance = 0;
            if (Dungeon.depth > 1)
            {
                //chance for a hidden door scales from 2/20 on floor 2 to 20/20 on floor 20
                hiddenDoorChance = Math.Min(1f, Dungeon.depth / 20f);
            }
            if (l.feeling == Level.Feeling.SECRETS)
            {
                //pull the value of extra secret doors toward 50% on secrets level feel
                hiddenDoorChance = (0.5f + hiddenDoorChance) / 2f;
            }

			Dictionary<Room, Room> roomMerges = new();

            foreach(Room r in rooms)
            {
                foreach(Room n in r.connected.Keys)
                {

                    //normal sized rooms can be merged at most once. Large and Giant rooms can be merged many times
                    if (roomMerges[r] == n || roomMerges[n] == r)
                    {
                        continue;
                    }
                    else if (!roomMerges.ContainsKey(r) && !roomMerges.ContainsKey(n) &&
                            MergeRooms(l, r, n, r.connected[n].pos, Tile.Empty))
                    {
                        if (((StandardRoom)r).sizeCat == SizeCategory.Normal) roomMerges.Add(r, n);
                        if (((StandardRoom)n).sizeCat == SizeCategory.Normal) roomMerges.Add(n, r);
                        continue;
                    }

                    Room.Door d = r.connected[n];
                    int door = d.pos.x + d.pos.y * l.Width;

                    if (d.type == Room.Door.Type.Regular)
                    {
                        if (RandomNumberGenerator.Double() < hiddenDoorChance)
                        {
                            d.type = Room.Door.Type.Hidden;
                            //all standard rooms must have an unbroken path to all other standard rooms
                            if (l.feeling != Level.Feeling.SECRETS)
                            {
                                //TODO: Graph.BuildDistanceMap(rooms, r);
                                if (n.distance == Int32.MaxValue)
                                {
                                    d.type = Room.Door.Type.Unlocked;
                                }
                                //on a secrets level, rooms just have to not be totally isolated
                            }
                            else
                            {
                                int roomsInGraph = 0;
                                //TODO: Graph.buildDistanceMap(rooms, r);
                                foreach(Room rDest in rooms)
                                {
                                    if (rDest.distance != Int32.MaxValue && !(rDest is ConnectionRoom))
									{
										roomsInGraph++;
									}
								}
								if (roomsInGraph< 2)
								{
									d.type = Room.Door.Type.Unlocked;
								}
								else
								{
									roomsInGraph = 0;
									// TODO Graph.buildDistanceMap(rooms, n);
									foreach(Room nDest in rooms)
									{
										if (nDest.distance != Int32.MaxValue && !(nDest is ConnectionRoom))
										{
											roomsInGraph++;
										}
									}
									if (roomsInGraph < 2)
									{
										d.type = Room.Door.Type.Unlocked;
									}
								}
							}
							// TODO Graph.buildDistanceMap(rooms, r);
							//don't hide if it would make this room only accessible by hidden doors
							//unless we're on a secrets depth
							if (l.feeling != Level.Feeling.SECRETS && n.distance == Int32.MaxValue)
							{
								d.type = Room.Door.Type.Unlocked;
							}
						}
						else
						{
							d.type = Room.Door.Type.Unlocked;
						}

						//entrance doors on floor 1 are hidden during tutorial
						//entrance doors on floor 2 are hidden if the player hasn't picked up 2nd guidebook page
						if (r is EntranceRoom || n is EntranceRoom)
						{
							if ((Dungeon.depth == 1 /*&& SPDSettings.intro()*/)
								|| (Dungeon.depth == 2 /*&& !Document.ADVENTURERS_GUIDE.isPageFound(Document.GUIDE_SEARCHING)*/))
							{
								d.type = Room.Door.Type.Hidden;
							}
						}
					}
				
					switch (d.type)
					{
						case Room.Door.Type.Empty:
							l.map[door] = Tile.Empty;
							break;
						case Room.Door.Type.Tunnel:
							l.map[door] = l.TunnelTile();
							break;
						case Room.Door.Type.Water:
							l.map[door] = Tile.Water;
							break;
						case Room.Door.Type.Unlocked:
							l.map[door] = Tile.Door;
							break;
						case Room.Door.Type.Hidden:
							l.map[door] = Tile.SecretDoor;
							break;
						case Room.Door.Type.Barricade:
							l.map[door] = Tile.Barricade;
							break;
						case Room.Door.Type.Locked:
							l.map[door] = Tile.LockedDoor;
							break;
						case Room.Door.Type.Crystal:
							l.map[door] = Tile.CrystalDoor;
							break;
					}
				}
			}
		}

        protected bool MergeRooms(Level l, Room r, Room n, Vector2Int start, Tile mergeTerrain)
        {

            Rectangle intersect = r.rect.Intersect(n);
            if (intersect.width == 0)
            {

                Rectangle merge = new();
                merge.left = merge.right = intersect.left;
                merge.top = merge.bottom = start != null ? start.y : intersect.Center().y;

                Vector2Int p = new Vector2Int(merge.left, merge.top);
                while (merge.top > intersect.top && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.bottom--;
                    p.y--;
                }
                p.y = merge.bottom;
                while (merge.bottom < intersect.bottom && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.bottom++;
                    p.y++;
                }

                if (merge.height >= 3)
                {
                    r.Merge(l, n, new Rectangle(merge.left, merge.top + 1, merge.left + 1, merge.height), mergeTerrain);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (intersect.top == intersect.bottom)
            {

                Rectangle merge = new();
                merge.left = merge.right = start != null ? start.x : intersect.Center().x;
                merge.top = merge.bottom = intersect.top;

                Vector2Int p = new(merge.left, merge.top);
                while (merge.left > intersect.left && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.left--;
                    p.x--;
                }
                p.x = merge.right;
                while (merge.right < intersect.right && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.right++;
                    p.x++;
                }

                if (merge.width >= 3)
                {
                    r.Merge(l, n, new Rectangle(merge.left + 1, merge.top, merge.right, merge.top + 1), mergeTerrain);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        //protected void PaintWater(Level l, List<Room> rooms)
        //{
        //    bool[] lake = Patch.Generate(l.Width, l.Height, waterFill, waterSmoothness, true);

        //    if (rooms.Count != 0)
        //    {
        //        foreach(Room r in rooms)
        //        {
        //            foreach(Vector2Int p in r.WaterPlaceablePoints())
        //            {
        //                int i = l.XYToCell(p);
        //                if (lake[i] && l.map[i] == Tile.Empty)
        //                {
        //                    l.map[i] = Tile.Water;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < l.Length; i++)
        //        {
        //            if (lake[i] && l.map[i] == Tile.Empty)
        //            {
        //                l.map[i] = Tile.Water;
        //            }
        //        }
        //    }

        //}

        //protected void PaintGrass(Level l, List<Room> rooms)
        //{
        //    bool[] grass = Patch.generate(l.Width, l.Height, grassFill, grassSmoothness, true);

        //    List<Int32> grassCells = new();

        //    if (rooms.Count == 0)
        //    {
        //        foreach(Room r in rooms)
        //        {
        //            foreach(Vector2Int p in r.GrassPlaceablePoints())
        //            {
        //                int i = l.XYToCell(p);
        //                if (grass[i] && l.map[i] == Tile.Empty)
        //                {
        //                    grassCells.Add(i);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < l.Length; i++)
        //        {
        //            if (grass[i] && l.map[i] == Tile.Empty)
        //            {
        //                grassCells.Add(i);
        //            }
        //        }
        //    }

        //    //Adds chaos to grass height distribution. Ratio of high grass depends on fill and smoothing
        //    //Full range is 8.3% to 75%, but most commonly (20% fill with 3 smoothing) is around 60%
        //    //low smoothing, or very low fill, will begin to push the ratio down, normally to 50-30%
        //    foreach(int i in grassCells)
        //    {
        //        //TODO
        //        //if (l.Heaps[i] != null || l.FindMob(i) != null)
        //        //{
        //        //    l.map[i] = Tile.Grass;
        //        //    continue;
        //        //}

        //        int count = 1;
        //        foreach(int n in PathFinder.NEIGHBOURS8)
        //        {
        //            if (grass[i + n])
        //            {
        //                count++;
        //            }
        //        }
        //        l.map[i] = (RandomNumberGenerator.Double() < count / 12f) ? Tile.HighGrass : Tile.Grass;
        //    }
        //}

        //protected void paintTraps(Level l, List<Room> rooms)
        //{
        //    List<int> validCells = new();

        //    if (rooms.Count != 0)
        //    {
        //        foreach(Room r in rooms)
        //        {
        //            foreach(Vector2Int p in r.TrapPlaceablePoints())
        //            {
        //                int i = l.XYToCell(p);
        //                if (l.map[i] == Tile.Empty)
        //                {
        //                    validCells.Add(i);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < l.Length; i++)
        //        {
        //            if (l.map[i] == Tile.Empty)
        //            {
        //                validCells.Add(i);
        //            }
        //        }
        //    }

        //    //no more than one trap every 5 valid tiles.
        //    nTraps = Math.Min(nTraps, validCells.Count / 5);

        //    //for traps that want to avoid being in hallways
        //    List<int> validNonHallways = new();

        //    //temporarily use the passable array for the next step
        //    for (int i = 0; i < l.Length; i++)
        //    {
        //        l.passable[i] = (TileInfo.Terrain.flags[l.map[i]] & TileFlags.Passable) != 0;
        //    }

        //    for (int i : validCells)
        //    {
        //        if ((l.passable[i + PathFinder.CIRCLE4[0]] || l.passable[i + PathFinder.CIRCLE4[2]])
        //                && (l.passable[i + PathFinder.CIRCLE4[1]] || l.passable[i + PathFinder.CIRCLE4[3]]))
        //        {
        //            validNonHallways.Add(i);
        //        }
        //    }

        //    //no more than one trap every 5 valid tiles.
        //    nTraps = Math.Min(nTraps, validCells.Count / 5);

        //    //5x traps on traps level feeling, but the extra traps are all visible
        //    for (int i = 0; i < (l.feeling == Level.Feeling.TRAPS ? 5 * nTraps : nTraps); i++)
        //    {

        //        Trap trap = Reflection.newInstance(trapClasses[RandomNumberGenerator.Chances(trapChances)]);

        //        int trapPos;
        //        if (trap.avoidsHallways && validNonHallways.Count != 0)
        //        {
        //            trapPos = RandomNumberGenerator.Element(validNonHallways);
        //        }
        //        else
        //        {
        //            trapPos = RandomNumberGenerator.Element(validCells);
        //        }
        //        //removes the integer object, not at the index
        //        validCells.Remove(trapPos);
        //        validNonHallways.Remove(trapPos);

        //        if (i < nTraps) trap.hide();
        //        else trap.reveal();

        //        l.SetTrap(trap, trapPos);
        //        //some traps will not be hidden
        //        l.map[trapPos] = trap.visible ? Tile.Trap : Tile.SecretTrap;
        //    }
        //}

    }

}