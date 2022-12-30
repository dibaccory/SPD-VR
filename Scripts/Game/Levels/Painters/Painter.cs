using System;
using System.Collections.Generic;
using Levels;
using Newtonsoft.Json.Linq;
using TileInfo;
using Levels.Rooms;
using Utils;
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

        public static void Fill(Level level, Rectangle r, Tile value)
        {
            Fill(level, r.left, r.top, r.width, r.height, value);
        }

        public static void Fill(Level level, Rectangle r, int m, Tile value)
        {
            Fill(level, r.left + m, r.top + m, r.width - m * 2, r.height - m * 2, value);
        }

        public static void Fill(Level level, Rectangle room, int l, int t, int r, int b, Tile value)
        {
            Fill(level, room.left + l, room.top + t, room.width - (l + r), room.height - (t + b), value);
        }

        public static void DrawLine(Level level, Vector2Int from, Vector2Int to, Tile value)
        {
            float x = from.x;
            float y = from.y;
            float dx = to.x - from.x;
            float dy = to.y - from.y;

            bool movingbyX = Math.Abs(dx) >= Math.Abs(dy);
            //normalize
            if (movingbyX)
            {
                dy /= Math.Abs(dx);
                dx /= Math.Abs(dx);
            }
            else
            {
                dx /= Math.Abs(dy);
                dy /= Math.Abs(dy);
            }

            Set(level, (int)Math.Round(x), (int)Math.Round(y), value);
            while ((movingbyX && to.x != x) || (!movingbyX && to.y != y))
            {
                x += dx;
                y += dy;
                Set(level, (int)Math.Round(x), (int)Math.Round(y), value);
            }
        }
    }
}