using System;
using System.Collections.Generic;
using System.Drawing;
using Levels.Rooms;
using UnityEngine;
using Utils;

namespace Levels.Builders
{
    public abstract class Builder
    {
        //If builders require additional parameters, they should
        // request them in their constructor or other methods

        //builders take a list of rooms and returns them as a connected map
        //returns null on failure
        public abstract List<Room> Build(List<Room> rooms);

        protected static void FindNeighbors(List<Room> rooms)
        {
            Room[] ra = rooms.ToArray();
            for (int i = 0; i < ra.Length - 1; i++)
            {
                for (int j = i + 1; j < ra.Length; j++)
                {
                    ra[i].AddNeighbor(ra[j]);
                }
            }
        }

        //returns a rectangle representing the maximum amount of free space from a specific start point
        protected static RectInt findFreeSpace(Vector2Int start, List<Room> collision, int maxSize)
        {
            RectInt space = new RectInt(start.x - maxSize, start.y - maxSize, start.x + maxSize, start.y + maxSize);

            //shallow copy
            List<Room> colliding = new List<Room>(collision);
            do
            {

                //remove empty rooms and any rooms we aren't currently overlapping
                IEnumerator<Room> it = (IEnumerator<Room>)colliding;
                do
                {
                    Room room = it.Current; //TODO ENSURE variable colliding is not empty
                    //if not colliding
                    if (room.IsEmpty
                            || Math.Max(space.x, room.left) >= Math.Min(space.xMax, room.right)
                            || Math.Max(space.y, room.top) >= Math.Min(space.yMax, room.bottom))
                    {
                        it.Dispose();
                    }
                }while (it.MoveNext());

                //iterate through all rooms we are overlapping, and find the closest one
                Room closestRoom = null;
                int closestDiff = Int32.MaxValue;
                bool inside = true;
                int curDiff = 0;
                foreach(Room curRoom in colliding)
                {

                    if (start.x <= curRoom.left)
                    {
                        inside = false;
                        curDiff += curRoom.left - start.x;
                    }
                    else if (start.x >= curRoom.right)
                    {
                        inside = false;
                        curDiff += start.x - curRoom.right;
                    }

                    if (start.y <= curRoom.top)
                    {
                        inside = false;
                        curDiff += curRoom.top - start.y;
                    }
                    else if (start.y >= curRoom.bottom)
                    {
                        inside = false;
                        curDiff += start.y - curRoom.bottom;
                    }

                    if (inside)
                    {
                        space.Set(start.x, start.y, start.x, start.y);
                        return space;
                    }

                    if (curDiff < closestDiff)
                    {
                        closestDiff = curDiff;
                        closestRoom = curRoom;
                    }

                }

                int wDiff, hDiff;
                if (closestRoom != null)
                {

                    wDiff = Int32.MaxValue;
                    if (closestRoom.left >= start.x)
                    {
                        wDiff = (space.xMax - closestRoom.left) * (space.height + 1);
                    }
                    else if (closestRoom.right <= start.x)
                    {
                        wDiff = (closestRoom.right - space.x) * (space.height + 1);
                    }

                    hDiff = Int32.MaxValue;
                    if (closestRoom.top >= start.y)
                    {
                        hDiff = (space.yMax - closestRoom.top) * (space.width + 1);
                    }
                    else if (closestRoom.bottom <= start.y)
                    {
                        hDiff = (closestRoom.bottom - space.y) * (space.width + 1);
                    }

                    //reduce by as little as possible to resolve the collision
                    if (wDiff < hDiff || wDiff == hDiff && RandomNumberGenerator.Int(2) == 0)
                    {
                        if (closestRoom.left >= start.x && closestRoom.left < space.xMax) space.xMax = closestRoom.left;
                        if (closestRoom.right <= start.x && closestRoom.right > space.x) space.x = closestRoom.right;
                    }
                    else
                    {
                        if (closestRoom.top >= start.y && closestRoom.top < space.yMax) space.yMax = closestRoom.top;
                        if (closestRoom.bottom <= start.y && closestRoom.bottom > space.y) space.y = closestRoom.bottom;
                    }
                    colliding.Remove(closestRoom);
                }
                else
                {
                    colliding.Clear();
                }

                //loop until we are no longer colliding with any rooms
            } while (colliding.Count > 0);

            return space;
        }
    }
}
