using System;
using System.Collections.Generic;
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
        protected static Rectangle FindFreeSpace(Vector2Int start, List<Room> collision, int maxSize)
        {
            Rectangle space = new(start.x - maxSize, start.y - maxSize, start.x + maxSize, start.y + maxSize);

            //shallow copy
            List<Room> colliding = collision.GetRange(0, collision.Count);
            do
            {

                //remove empty rooms and any rooms we aren't currently overlapping
                //IEnumerator<Room> it = colliding.GetEnumerator();
                for(int i = 0; i < colliding.Count; i++)
                {
                    Room room = colliding[i];
                    //if not colliding
                    if (room.IsEmpty()
                            || Math.Max(space.left, room.left) >= Math.Min(space.right, room.right)
                            || Math.Max(space.top, room.top) >= Math.Min(space.bottom, room.bottom))
                    {
                        colliding.Remove(room);
                        i--;
                    }
                }

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

                    wDiff = Math.Abs(space.right - closestRoom.left) * (space.height + 1);
                    hDiff = Math.Abs(space.bottom - closestRoom.top) * (space.width + 1);
                    

                    //reduce by as little as possible to resolve the collision
                    if (wDiff < hDiff || wDiff == hDiff && RandomNumberGenerator.Int(2) == 0)
                    {
                        if (closestRoom.left >= start.x && closestRoom.left < space.right) space.right = closestRoom.left;
                        if (closestRoom.right <= start.x && closestRoom.right > space.left) space.left = closestRoom.right;
                    }
                    else
                    {
                        if (closestRoom.top >= start.y && closestRoom.top < space.bottom) space.bottom = closestRoom.top;
                        if (closestRoom.bottom <= start.y && closestRoom.bottom > space.top) space.top = closestRoom.bottom;
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

        private static readonly double RADIAN = 180 / Math.PI;

        //returns the angle in degrees made by the centerpoints of 2 rooms, with 0 being straight up.
        protected static float AngleBetweenRooms(Room from, Room to)
        {
            Vector2 fromCenter = new((from.left + from.right) / 2f, (from.top + from.bottom) / 2f);
            Vector2 toCenter = new((to.left + to.right) / 2f, (to.top + to.bottom) / 2f);
            return AngleBetweenRoomCenters(fromCenter, toCenter);
        }

        protected static float AngleBetweenRoomCenters(Vector2 from, Vector2 to)
        {
            double m = (to.y - from.y) / (to.x - from.x);

            float angle = (float)(RADIAN * (Math.Atan(m) + Math.PI / 2.0));
            if (from.x > to.x) angle -= 180f;
            return angle;
        }

        //Attempts to place a room such that the angle between the center of the previous room
        // and it matches the given angle ([0-360), where 0 is straight up) as closely as possible.
        //Note that getting an exactly correct angle is harder the closer that angle is to diagonal.
        //Returns the exact angle between the centerpoints of the two rooms, or -1 if placement fails.
        protected static float PlaceRoom(List<Room> collision, Room prev, Room next, float angle)
        {


            //wrap angle around to always be [0-360)
            angle %= 360f;
            if (angle < 0)
            {
                angle += 360f;
            }

            Vector2 prevCenter = new((prev.left + prev.right) / 2f, (prev.top + prev.bottom) / 2f);

            // calculating using y = mx+b, straight line formula
            double m = Math.Tan(angle / RADIAN + Math.PI / 2.0);
            double b = prevCenter.y - m * prevCenter.x;

            //using the line equation, we find the point along the prev room where the line exists
            Vector2Int start;
            int direction;
            if (Math.Abs(m) >= 1)
            {
                if (angle < 90 || angle > 270)
                {
                    direction = Room.TOP;
                    start = new Vector2Int((int)Math.Round((prev.top - b) / m), prev.top);
                }
                else
                {
                    direction = Room.BOTTOM;
                    start = new Vector2Int((int)Math.Round((prev.bottom - b) / m), prev.bottom);
                }
            }
            else
            {
                if (angle < 180)
                {
                    direction = Room.RIGHT;
                    start = new Vector2Int(prev.right, (int)Math.Round(m * prev.right + b));
                }
                else
                {
                    direction = Room.LEFT;
                    start = new Vector2Int(prev.left, (int)Math.Round(m * prev.left + b));
                }
            }

            //cap it to a valid connection point for most rooms
            if (direction == Room.TOP || direction == Room.BOTTOM)
            {
                start.x = (int)Tools.Gate(prev.left + 1, start.x, prev.right - 1);
            }
            else
            {
                start.y = (int)Tools.Gate(prev.top + 1, start.y, prev.bottom - 1);
            }

            //space checking
            Rectangle space = FindFreeSpace(start, collision, Math.Max(next.MaxWidth, next.MaxHeight));
            if (!next.SetSizeWithLimit(space.width + 1, space.height + 1))
            {
                return -1;
            }

            //find the ideal center for this new room using the line equation and known dimensions
            Vector2 targetCenter = new();
            if (direction == Room.TOP)
            {
                targetCenter.y = prev.top - (next.height - 1) / 2f;
                targetCenter.x = (float)((targetCenter.y - b) / m);
                next.SetPos((int)Math.Round(targetCenter.x - (next.width - 1) / 2f ), prev.top - (next.height - 1));

            }
            else if (direction == Room.BOTTOM)
            {
                targetCenter.y = prev.bottom + (next.height - 1) / 2f;
                targetCenter.x = (float)((targetCenter.y - b) / m);
                next.SetPos((int)Math.Round(targetCenter.x - (next.width - 1) / 2f), prev.bottom);

            }
            else if (direction == Room.RIGHT)
            {
                targetCenter.x = prev.right + (next.width - 1) / 2f;
                targetCenter.y = (float)(m * targetCenter.x + b);
                next.SetPos(prev.right, (int)Math.Round(targetCenter.y - (next.height - 1) / 2f));


            }
            else if (direction == Room.LEFT)
            {
                targetCenter.x = prev.left - (next.width - 1) / 2f;
                targetCenter.y = (float)(m * targetCenter.x + b);
                next.SetPos(prev.left - (next.width - 1), (int)Math.Round(targetCenter.y - (next.height - 1) / 2f));

            }

            //perform connection bounds and target checking, move the room if necessary
            if (direction == Room.TOP || direction == Room.BOTTOM)
            {
                if (next.right < prev.left + 2) next.Shift(prev.left + 2 - next.right, 0);
                else if (next.left > prev.right - 2) next.Shift(prev.right - 2 - next.left, 0);

                if (next.right > space.right) next.Shift(space.right - next.right, 0);
                else if (next.left < space.left) next.Shift(space.left - next.left, 0);
            }
            else
            {
                if (next.bottom < prev.top + 2) next.Shift(0, prev.top + 2 - next.bottom);
                else if (next.top > prev.bottom - 2) next.Shift(0, prev.bottom - 2 - next.top);

                if (next.bottom > space.bottom) next.Shift(0, space.bottom - next.bottom);
                else if (next.top < space.top) next.Shift(0, space.top - next.top);
            }

            //attempt to connect, return the result angle if successful.
            if (next.Connect(prev))
            {
                return AngleBetweenRooms(prev, next);
            }
            else
            {
                return -1;
            }
        }

    }
}
