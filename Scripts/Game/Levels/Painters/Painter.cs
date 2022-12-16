using System;
using System.Collections.Generic;
using Levels;
using Newtonsoft.Json.Linq;
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

        public static void Set(Level level, int cell, int value)
        {
            level.map[cell] = value;
        }

        public static void Set(Level level, int x, int y, int value)
        {
            Set(level, x + y * level.Width(), value);
            
        }

        public static void Set(Level level, Vector2Int p, int value)
        {
            Set(level, p.x, p.y, value);
        }

        public static void Fill(Level level, int x, int y, int w, int h, int value)
        {

            int width = level.Width();

            int pos = y * width + x;
            for (int i = y; i < y + h; i++, pos += width)
            {
                Array.Fill(level.map, value, pos, w);
            }
        }

        public static void Fill(Level level, RectInt rect, int value)
        {
            Fill(level, rect.x, rect.y, rect.width, rect.height, value);
        }

        public static void Fill(Level level, RectInt rect, int m, int value)
        {
            Fill(level, rect.x + m, rect.y + m, rect.width - m * 2, rect.height - m * 2, value);
        }

        public static void Fill(Level level, RectInt rect, int l, int t, int r, int b, int value)
        {
            Fill(level, rect.x + l, rect.y + t, rect.width - (l + r), rect.height - (t + b), value);
        }
    }
}