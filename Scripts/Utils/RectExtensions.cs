using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Utils
{ 

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

        public virtual int width => right - left;
        public virtual int height => bottom - top;
        public int area => width * height;
        

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

        public Rectangle SetPos(int x, int y) => Set(x, y, x + (right - left), y + (bottom - top));

        public Rectangle Shift(int x, int y) => Set(left + x, top + y, right + x, bottom + y);

        //USE xMax and yMax instead
        public Rectangle Resize(int w, int h) => Set(left, top, left + w, top + h);

        public bool IsEmpty() => right <= left || bottom <= top;

        public Rectangle SetEmpty()
        {
            left = right = top = bottom = 0;
            return this;
        }

        public Rectangle Intersect(Rectangle other)
        {
            Rectangle r = new();
            r.left = Math.Max(r.left, other.left);
            r.top = Math.Max(r.top, other.top);
            r.right = Math.Min(r.right, other.right);
            r.bottom = Math.Min(r.bottom, other.bottom);

            return r;
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

        public Rectangle Union(int x, int y)
        {
            if (IsEmpty())
            {
                return Set(x, y, x + 1, y + 1);
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
                if (y < top)
                {
                    top = y;
                }
                else if (y >= bottom)
                {
                    bottom = y + 1;
                }
                return this;
            }
        }

        public Rectangle Union(Vector2Int p)
        {
            return Union(p.x,p.y);
        }

        public bool Inside(Vector2Int p)
        {
            return p.x >= left && p.x < right && p.y >= top && p.y < bottom;
        }

        public Vector2Int Center()
        {
            return new Vector2Int
                (
                    (((left + right) / 2) + (((right - left) % 2) == 0 ? Utils.RandomNumberGenerator.Int(2) : 0)),
                    (((top + bottom) / 2) + (((bottom - top) % 2) == 0 ? Utils.RandomNumberGenerator.Int(2) : 0))
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

        public List<Vector2Int> GetPoints()
        {
            List<Vector2Int> points = new();
            for (int i = left; i <= right; i++)
                for (int j = top; j <= bottom; j++)
                    points.Add(new Vector2Int(i, j));
            return points;
        }

    }
}
