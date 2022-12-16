using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Utils
{
    public static class RectIntExtensions
    {
        public static int left    (this RectInt r) => r.x;
        public static int top     (this RectInt r) => r.y;
        public static int right   (this RectInt r) => r.xMax;
        public static int bottom  (this RectInt r) => r.yMax;

        public static RectInt zero => new RectInt(0, 0, 0, 0);

        public static bool Inside(this RectInt r, Vector2Int p)
        {
            return p.x >= r.x && p.x < r.xMax && p.y >= r.y && p.y < r.yMax;
        }

        public static RectInt Intersect(this RectInt r, RectInt other)
        {
            r.x     = Math.Max(r.x, other.x);
            r.y     = Math.Max(r.y, other.y);
            r.xMax  = Math.Min(r.xMax, other.xMax);
            r.yMax  = Math.Min(r.yMax, other.yMax);

            return r;
        }

        public static RectInt Set(this RectInt r, int x, int y, int width, int height)
        {
            r.x = x;
            r.y = y;
            r.width = width;
            r.height = height;
            return r;
        }


        public static RectInt Intersect(this RectInt r, Levels.Rooms.Room other)
        {
            r.x = Math.Max(r.x, other.left);
            r.y = Math.Max(r.y, other.top);
            r.xMax = Math.Min(r.xMax, other.right);
            r.yMax = Math.Min(r.yMax, other.bottom);

            return r;
        }

        public static RectInt Set(this RectInt rect, Levels.Rooms.Room r)
        {
            return rect.Set(r.left, r.top, r.right, r.bottom);
        }

        //Every unit point within the Rect
        public static List<Vector2Int> GetPoints(this RectInt r)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = r.x; i <= r.xMax; i++)
                for (int j = r.y; j <= r.yMax; j++)
                    points.Add(new Vector2Int(i, j));
            return points;
        }
    }


    public class Rectangle
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        //public static float Left(this Rect r) => r.x;
        //public static float Right(this Rect r) => r.xMax;
        //public static float Top(this Rect r) => r.y;
        //public static float Bottom(this Rect r) => r.yMax;

        public Rectangle() : this(0, 0, 0, 0) { }

        public Rectangle(Rectangle rect) : this(rect.left, rect.top, rect.right, rect.bottom) {}

        public Rectangle(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public int Width()
        {
            return right - left;
        }

        public int Depth()
        {
            return bottom - top;
        }

        public int Area()
        {
            return Width() * Depth();
        }

        public Rectangle Set(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            return this;
        }

        public Rectangle Set(Rectangle rect)
        {
            return Set(rect.left, rect.top, rect.right, rect.bottom);
        }

        public Rectangle SetPos(int x, int z) => Set(x, z, x + (right - left), z + (bottom - top));

        public Rectangle Shift(int x, int z) => Set(left + x, top + z, right + x, bottom + z);

        //USE xMax and yMax instead
        //public Rectangle Resize(int w, int d) => Set(left, top, left + w, top + d);

        public bool IsEmpty() => right <= left || bottom <= top;

        public Rectangle SetEmpty()
        {
            left = right = top = bottom = 0;
            return this;
        }

        

        public Rectangle Union(Rectangle other)
        {
            Rectangle result = new Rectangle
            {
                left = Math.Max(left, other.left),
                right = Math.Min(right, other.right),
                top = Math.Max(top, other.top),
                bottom = Math.Min(bottom, other.bottom)
            };
            return result;
        }

        public Rectangle Union(int x, int z)
        {
            if (IsEmpty())
            {
                return Set(x, z, x + 1, z + 1);
            }
            else
            {
                if (x < left)
                {
                    left = x;
                }
                else if (x >= right)
                {
                    right = x + 1;
                }
                if (z < top)
                {
                    top = z;
                }
                else if (z >= bottom)
                {
                    bottom = z + 1;
                }
                return this;
            }
        }

        public Rectangle Union(Vector3 p)
        {
            return Union((int)p.x,(int)p.z);
        }

        public bool Inside(Vector3 p)
        {
            return p.x >= left && p.x < right && p.y >= top && p.y < bottom;
        }

        public Vector3 Center()
        {
            return new Vector3
                (
                    (float)(((left + right) / 2) + (((right - left) % 2) == 0 ? Utils.RandomNumberGenerator.Int(2) : 0)),
                    0,
                    (float)(((top + bottom) / 2) + (((bottom - top) % 2) == 0 ? Utils.RandomNumberGenerator.Int(2) : 0))
                );
        }

        public Rectangle Shrink(int d)
        {
            return new Rectangle(left + d, top + d, right - d, bottom - d);
        }

        public Rectangle Shrink()
        {
            return Shrink(1);
        }

        public Rectangle Scale(int d) => new Rectangle(left * d, top * d, right * d, bottom * d);

        public List<Vector3> GetPoints()
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = left; i <= right; i++)
                for (int j = top; j <= bottom; j++)
                    points.Add(new Vector3((float)i, 0 ,(float)j));
            return points;
        }

    }
}
