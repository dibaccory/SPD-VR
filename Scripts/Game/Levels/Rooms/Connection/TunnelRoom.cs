using System;
using TileInfo;
using Levels;
using Levels.Rooms;
using DungeonInstance;
using Utils;
using Levels.Painters;
using System.Drawing;
using UnityEngine;

namespace Levels.Rooms.Connection
{
    public class TunnelRoom : ConnectionRoom
    {
        public TunnelRoom()
        {
        }

        public override void Paint(Level level)
        {

            Tile floor = level.TunnelTile();

            RectInt c = GetConnectionSpace();

            foreach (Door door in connected.Values)
            {

                Vector2Int start;
                Vector2Int mid;
                Vector2Int end;

                start = new Vector2Int(door.pos.x, door.pos.y);
                if (start.x == left) start.x++;
                else if (start.y == top) start.y++;
                else if (start.x == right) start.x--;
                else if (start.y == bottom) start.y--;

                int rightShift;
                int downShift;

                if (start.x < c.x) rightShift = c.x - start.x;
                else if (start.x > c.xMax) rightShift = c.xMax - start.x;
                else rightShift = 0;

                if (start.y < c.y) downShift = c.y - start.y;
                else if (start.y > c.yMax) downShift = c.yMax - start.y;
                else downShift = 0;

                //always goes inward first
                if (door.pos.x == left || door.pos.x == right)
                {
                    mid = new(start.x + rightShift, start.y);
                    end = new(mid.x, mid.y + downShift);

                }
                else
                {
                    mid = new(start.x, start.y + downShift);
                    end = new(mid.x + rightShift, mid.y);

                }

                Painter.DrawLine(level, start, mid, floor);
                Painter.DrawLine(level, mid, end, floor);
            }
        }

        //returns the space which all doors must connect to (usually 1 cell, but can be more)
        //Note that, like rooms, this space is inclusive to its right and bottom sides
        protected RectInt GetConnectionSpace()
        {
            Vector2Int c = GetDoorCenter();

            return new RectInt(c.x, c.y, c.x, c.y);
        }

        //returns a point equidistant from all doors this room has
        protected Vector2Int GetDoorCenter()
        {
            Vector2 doorCenter = new(0, 0);

            foreach (Door door in connected.Values)
            {
                doorCenter.x += door.pos.x;
                doorCenter.y += door.pos.y;
            }

            Vector2Int c = new((int)doorCenter.x / connected.Count, (int)doorCenter.y / connected.Count);
            if (RandomNumberGenerator.Double() < doorCenter.x % 1) c.x++;
            if (RandomNumberGenerator.Double() < doorCenter.y % 1) c.y++;
            c.x = (int)Tools.Gate(left + 1, c.x, right - 1);
            c.y = (int)Tools.Gate(top + 1, c.y, bottom - 1);

            return c;
        }

    }
}