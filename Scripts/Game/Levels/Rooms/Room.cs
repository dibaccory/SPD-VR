using System;
using System.Collections.Generic;
using Levels;
using Levels.Painters;
using System.Drawing;
//using System.Drawing;
using UnityEngine;
using Utils;

namespace Levels.Rooms
{
    public abstract class Room
    {
        public List<Room> neighbors = new List<Room>();
        public Dictionary<Room, Door> connected = new Dictionary<Room, Door>();
        public int distance;
        public int price = 1;
        public int left, top, right, bottom;

        //+1 Because Rooms are inclusive to their right and bottom
        public int Width => right - left + 1;
        public int Height => bottom - top + 1;
        public int Area => Width * Height;

        public Room() : this(RectIntExtensions.zero) { }


        public Room(RectInt r)
        {
            left   = r.x;
            top    = r.y;
            right  = r.xMax;
            bottom = r.yMax;
        }

        public Room zero() => Set(0, 0, 0, 0);

        public Room Set( int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;

            return this;
        }

        public Room Set(Room other)
        {
            Set(other.left, other.top, other.right, other.bottom);

            foreach (Room r in other.neighbors)
            {
                neighbors.Add(r);
                r.neighbors.Remove(other);
                r.neighbors.Add(this);
            }
            foreach (var c in other.connected)
            {
                Room r = c.Key;
                Door d = c.Value;
                r.connected.Remove(other);
                r.connected.Add(this, d);
                connected.Add(r, d);
            }
            return this;
        }

        // **** Spatial logic ****

        //Note: when overriding these YOU MUST store any randomly decided values.
        //With the same room and the same parameters these should always return
        //the same value over multiple calls, even if there's some randomness initially.
        public virtual int MinWidth  { get; private set; } = -1;
        public virtual int MaxWidth  { get; private set; } = -1;
        public virtual int MinHeight { get; private set; } = -1;
        public virtual int MaxHeight { get; private set; } = -1;

        public bool IsEmpty => right <= left || bottom <= top;
        

        public bool ForceSize(int w, int h)
        {
            return SetSize(w, w, h, h);
        }

        public Room Resize(int w, int h)
        {
            return Set(left, top, left+w, top+h);
        }

        public bool SetSize()
        {
            return SetSize(MinWidth, MaxWidth, MinHeight, MaxHeight);
        }

        protected bool SetSize(int minW, int maxW, int minH, int maxH)
        {
            if (minW < MinWidth
                    || maxW > MaxWidth
                    || minH < MinHeight
                    || maxH > MaxHeight
                    || minW > maxW
                    || minH > maxH)
            {
                return false;
            }
            else
            {
                //subtract one because rooms are inclusive to their right and bottom sides
                Resize(RandomNumberGenerator.NormalIntRange(minW, maxW) - 1,
                        RandomNumberGenerator.NormalIntRange(minH, maxH) - 1);
                return true;
            }
        }

        

        public bool SetSizeWithLimit(int w, int h)
        {
            if (w < MinWidth || h < MinHeight)
            {
                return false;
            }
            else
            {
                SetSize();

                if (Width > w || Height > h)
                {
                    Resize(Math.Min(Width, w) - 1, Math.Min(Height, h) - 1);
                }

                return true;
            }
        }

        public bool Inside(Vector2Int p)
        {
            return p.x >= left && p.x < right && p.y >= top && p.y < bottom;
        }

        public Vector2Int PointInside(Vector2Int from, int n)
        {
            int px = n * (-1 * (from.x - right < 0 ? -1 : 1) * (from.x % right == 0 ? 1 : 0));
            int py = n * (-1 * (from.y - bottom < 0 ? -1 : 1) * (from.x % bottom == 0 ? 1 : 0));

            return new Vector2Int(from.x + px, from.y + py);
        }

        public Vector2Int Random()
        {
            return Random(1);
        }

        public Vector2Int Random(int m)
        {
            return new Vector2Int
                (
                    RandomNumberGenerator.IntRange((int)left + m, (int)right - m),
                    RandomNumberGenerator.IntRange((int)top + m, (int)bottom - m)
                );
        }

        // **** Connection logic ****

        public static readonly int ALL = 0;
        public static readonly int LEFT = 1;
        public static readonly int TOP = 2;
        public static readonly int RIGHT = 3;
        public static readonly int BOTTOM = 4;

        //public Room Intersect(Room other)
        //{
        //   return 
        //        (
        //            Math.Max(left, other.left),
        //            Math.Max(top, other.top),
        //            Math.Min(right, other.right),
        //            Math.Min(bottom, other.bottom)
        //        ); 
        //}
        public List<Vector2Int> GetPoints()
        {
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = (int)left; i <= right; i++)
                for (int j = (int)top; j <= bottom; j++)
                    points.Add(new Vector2Int(i, j));
            return points;
        }

        public int MinConnections(int direction) => Convert.ToInt32(direction == ALL);

        public int CurConnections(int direction)
        {
            if (direction == ALL)
            {
                return connected.Count;

            }
            else
            {
                int total = 0;
                foreach (Room r in connected.Keys)
                {
                    RectInt i = new RectInt().Intersect(r);
                    if      (direction == LEFT && i.width == 0 && i.x == left) total++;
                    else if (direction == TOP && i.height == 0 && i.y == top) total++;
                    else if (direction == RIGHT && i.width == 0 && i.xMax == right) total++;
                    else if (direction == BOTTOM && i.height == 0 && i.yMax == bottom) total++;
                }
                return total;
            }
        }

        public int RemConnections(int direction)
        {
            return (CurConnections(ALL) < MaxConnections(ALL))
                ? MaxConnections(direction) - CurConnections(direction)
                : 0;
        }

        public int MaxConnections(int direction)
        {
            return (direction == ALL) ? 16 : 4;
        }

        //only considers point-specific limits, not direction limits
        public bool CanConnect(Vector2Int p)
        {
            //point must be along exactly one edge, no corners.
            return (p.x == left || p.x == right) != (p.y == top || p.y == bottom);
        }

        //only considers direction limits, not point-specific limits
        public bool CanConnect(int direction)
        {
            return RemConnections(direction) > 0;
        }

        //considers both direction and point limits
        public bool CanConnect(Room r)
        {
            RectInt i = new RectInt().Set(this).Intersect(r);

            bool foundPoint = false;
            foreach(Vector2Int p in i.GetPoints())
            {
                if (CanConnect(p) && r.CanConnect(p))
                {
                    foundPoint = true;
                    break;
                }
            }
            if (!foundPoint) return false;

            if (i.width == 0 && i.x == left)            return CanConnect(LEFT) && r.CanConnect(RIGHT);
            else if (i.height == 0 && i.y == top)       return CanConnect(TOP) && r.CanConnect(BOTTOM);
            else if (i.width == 0 && i.xMax == right)   return CanConnect(RIGHT) && r.CanConnect(LEFT);
            else if (i.height == 0 && i.yMax == bottom) return CanConnect(BOTTOM) && r.CanConnect(TOP);
            else
                return false;
        }

        public bool CanMerge(Level l, Point p, int mergeTerrain)
        {
            return false;
        }

        //can be overriden for special merge logic between rooms
        public void Merge(Level l, Room other, RectInt merge, int mergeTerrain)
        {
            Painter.Fill(l, merge, mergeTerrain);
        }

        public bool AddNeighbor(Room other)
        {
            if (neighbors.Contains(other))
                return true;

            RectInt i = new RectInt().Intersect(other);
            if ((i.width == 0 && i.height >= 2) ||
                (i.height == 0 && i.width >= 2))
            {
                neighbors.Add(other);
                other.neighbors.Add(this);
                return true;
            }
            return false;
        }

        public bool Connect(Room room)
        {
            if ((neighbors.Contains(room) || AddNeighbor(room))
                    && !connected.ContainsKey(room) && CanConnect(room))
            {
                connected.Add(room, null);
                room.connected.Add(this, null);
                return true;
            }
            return false;
        }

        public void ClearConnections()
        {
            foreach(Room r in neighbors)
            {
                r.neighbors.Remove(this);
            }
            neighbors.Clear();
            foreach(Room r in connected.Keys)
            {
                r.connected.Remove(this);
            }
            connected.Clear();
        }

        // **** Painter Logic ****

        public abstract void Paint(Level level);

        //whether or not a painter can make its own modifications to a specific point
        public bool CanPlaceWater(Vector2 p)
        {
            return true;
        }

        public List<Vector2Int> WaterPlaceablePoints(){
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if (CanPlaceWater(p)) points.Add(p);
                }
            }
            return points;
        }

        //whether or not a painter can make place grass at a specific point
        public bool CanPlaceGrass(Vector2Int p)
        {
            return true;
        }

        public List<Vector2Int> GrassPlaceablePoints(){
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if (CanPlaceGrass(p)) points.Add(p);
                }
            }
            return points;
        }

        //whether or not a painter can place a trap at a specific point
        public bool CanPlaceTrap(Vector2Int p)
        {
            return true;
        }

        public List<Vector2Int> TrapPlaceablePoints(){
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if (CanPlaceTrap(p)) points.Add(p);
                }
            }
            return points;
        }

        //whether or not an item can be placed here (usually via randomDropCell)
        public bool CanPlaceItem(Vector2Int p, Level l)
        {
            return Inside(p);
        }

        public List<Vector2Int> ItemPlaceablePoints(Level l){
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if (CanPlaceItem(p, l)) points.Add(p);
                }
            }
            return points;
        }

        //whether or not a character can be placed here (usually via spawn, tele, or wander)
        public bool CanPlaceCharacter(Vector2Int p, Level l)
        {
            return Inside(p);
        }

        public List<Vector2Int> CharPlaceablePoints(Level l){
            List<Vector2Int> points = new List<Vector2Int>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if (CanPlaceCharacter(p, l)) points.Add(p);
                }
            }
            return points;
        }

        public List<Room> Edges() {
            List<Room> edges = new List<Room>();
            foreach(Room r in connected.Keys)
            {
                Door d = connected[r];
                //for the purposes of path building, ignore all doors that are locked, blocked, or hidden
                if (d.type == Door.Type.EMPTY || d.type == Door.Type.TUNNEL
                        || d.type == Door.Type.UNLOCKED || d.type == Door.Type.REGULAR)
                {
                    edges.Add(r);
                }
            }
            return edges;
        }

        public void OnLevelLoad( Level l) { }

        public sealed class Door
        {
            public Vector3 pos;

            public enum Type
            {
                EMPTY, TUNNEL, WATER, REGULAR, UNLOCKED, HIDDEN, BARRICADE, LOCKED, CRYSTAL
            }

            public Type type = Type.EMPTY;

            public Door()
            {
            }

            public Door(Vector3 p)
            {
                this.pos = p;
            }

            public Door(int x, int z)
            {
                this.pos.x = x;
                this.pos.z = z;
            }

            public void Set(Type type)
            {
                if (type.CompareTo(this.type) > 0)
                {
                    this.type = type;
                }

            }
        }

        public class SizeCategoryAttr : Attribute
        {
            internal SizeCategoryAttr(int min, int max, int val)
            {
                this.MinDim = min;
                this.MaxDim = max;
                this.RoomValue = val;
            }

            internal int ConnectionWeight() { return this.RoomValue * this.RoomValue; }
            public int MinDim { get; private set; }
            public int MaxDim { get; private set; }
            public int RoomValue { get; private set; }

        }

        public enum SizeCategory
        {
            [SizeCategoryAttr(4, 10, 1)] NORMAL,
            [SizeCategoryAttr(10, 14, 2)] LARGE,
            [SizeCategoryAttr(14, 18, 3)] GIANT
        }

    }
}
