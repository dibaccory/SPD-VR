using System;
using System.Collections.Generic;
using Levels;
using Newtonsoft.Json.Linq;
using Terrain;
using Levels.Rooms;
using System.Drawing;
using UnityEngine;

namespace Levels.Painters
{
	public abstract class Painter
	{
        //If painters require additional parameters, they should
        // request them in their constructor or other methods

        //Painters take a level and its collection of rooms, and paint all the specific tile values
        public abstract bool Paint(Level level, List<Room> rooms);

        // Static methods

        public static void Set(Level level, int cell, Tile value)
        {
            level.map[cell] = value;
        }

        public static void Set(Level level, int x, int y, Tile value)
        {
            Set(level, x + y * level.Width, value);
            
        }

        public static void Set(Level level, Vector2Int p, Tile value)
        {
            Set(level, p.x, p.y, value);
        }

        public static void Fill(Level level, int x, int y, int w, int h, Tile value)
        {

            int width = level.Width;

            int pos = y * width + x;
            for (int i = y; i < y + h; i++, pos += width)
            {
                Array.Fill(level.map, value, pos, w);
            }
        }

        public static void Fill(Level level, Room r, Tile value)
        {
            Fill(level, r.left, r.top, r.Width, r.Height, value);
        }

        public static void Fill(Level level, Room r, int m, Tile value)
        {
            Fill(level, r.left + m, r.top + m, r.Width - m * 2, r.Height - m * 2, value);
        }

        public static void Fill(Level level, Room room, int l, int t, int r, int b, Tile value)
        {
            Fill(level, room.left + l, room.top + t, room.Width - (l + r), room.Height - (t + b), value);
        }
    }
}