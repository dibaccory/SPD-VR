﻿using System;
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
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor;
using System.Drawing;

namespace Levels.Painters
{

    public abstract class RegularPainter : Painter
    {
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
				PaintWater( level, rooms );
			}
		
			if (grassFill > 0f){
				PaintGrass( level, rooms );
			}
		
			if (nTraps > 0){
				PaintTraps( level, rooms );
			}
		
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
					RectInt i = r.rect.Intersect( n );
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
                            MergeRooms(l, r, n, r.connected[n], Tile.Empty))
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

        protected bool mergeRooms(Level l, Room r, Room n, Vector2Int start, Tile mergeTerrain)
        {

            RectInt intersect = r.rect.Intersect(n);
            if (intersect.width == 0)
            {

                RectInt merge = new();
                merge.x = merge.xMax = intersect.x;
                merge.y = merge.yMax = start != null ? start.y : (int)intersect.center.y;

                Vector2Int p = new Vector2Int(merge.left(), merge.top());
                while (merge.top() > intersect.top() && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.y--;
                    p.y--;
                }
                p.y = merge.bottom();
                while (merge.bottom() < intersect.bottom() && n.CanMerge(l, p, mergeTerrain) && r.CanMerge(l, p, mergeTerrain))
                {
                    merge.yMax++;
                    p.y++;
                }

                if (merge.height >= 3)
                {
                    r.Merge(l, n, new RectInt(merge.x, merge.y + 1, merge.x + 1, merge.height), mergeTerrain);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (intersect.top == intersect.bottom)
            {

                Rect merge = new Rect();
                merge.left = merge.right = start != null ? start.x : intersect.center.x;
                merge.top = merge.bottom = intersect.top;

                Point p = new Point(merge.left, merge.top);
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
                    r.merge(l, n, new Rect(merge.left + 1, merge.top, merge.right, merge.top + 1), mergeTerrain);
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

    }

}