using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using TileInfo;


namespace Levels
{
    public abstract class Level
    {
        public enum Feeling
        {
            NONE,
            CHASM,
            WATER,
            GRASS,
            DARK,
            LARGE,
            TRAPS,
            SECRETS
        }

        public int Width  { get; private set; }
        public int Height { get; private set; }
        public int Length { get => Width * Height; set => Length = value; }

        protected static readonly float TIME_TO_RESPAWN = 50;

        public int version;

        public Tile[] map;
        public bool[] heroFOV;
        public bool[] visited;
        public bool[] mapped;
        public bool[] discoverable;
        public bool[] openSpace;

        //public int viewDistance = Dungeon.isChallenged(Challenges.DARKNESS) ? 2 : 8;

        //STATIC FLAGS. WILL DELETE
        //public bool[] passable;
        //public bool[] losBlocking;
        //public bool[] flamable;
        //public bool[] secret;
        //public bool[] solid;
        //public bool[] avoid;
        //public bool[] water;
        //public bool[] pit;
       
        public Feeling feeling = Feeling.NONE;
        public int entrance;
        public int exit;

        //public ArrayList<LevelTransition> transitions;
        public bool locked = false;
        /*
        public HashSet<Mob> mobs;
        public SparseArray<Heap> heaps;
        public HashMap<Class<? extends Blob>, Blob> blobs;
        public SparseArray<Plant> plants;
        public SparseArray<Trap> traps;
        public HashSet<CustomTilemap> customTiles;
        public HashSet<CustomTilemap> customWalls;

        protected ArrayList<Item> itemsToSpawn = new ArrayList<>();

        protected Group visuals;
        */

        public int color1 = 0x004400;
        public int color2 = 0x88CC44;

        public Vector2Int CellToXY(int cell) => new(cell % Width, cell / Width);
        public int XYToCell(Vector2Int p) => p.x + p.y * Width;
       

        public void Create()
        {
            //TODO if (NOT Boss Level..) Line 190

            do
            {
                Width = Height = Length = 0;

                //transitions = new ArrayList<>();

                //mobs = new HashSet<>();
                //heaps = new SparseArray<>();
                //blobs = new HashMap<>();
                //plants = new SparseArray<>();
                //traps = new SparseArray<>();
                //customTiles = new HashSet<>();
                //customWalls = new HashSet<>();

            } while (!Build());

            BuildFlagMaps();
            CleanWalls();

            CreateMobs();
            CreateItems();

            RandomNumberGenerator.PopGenerator();
        }

        public void SetSize(int w, int h)
        {

            Width = w;
            Height = h;
            //Length = w * h; TODO CHECK IF NEEDED

            //Arrays.fill(map, feeling == Level.Feeling.CHASM ? Terrain.CHASM : Terrain.WALL);
            map = new Tile[Length];

            //These are dynamic flags... Change off of gamestate
            heroFOV = new bool[Length];
            visited = new bool[Length];
            mapped = new bool[Length];
            //openSpace can be static, but it also considers mobs in places where large mobs cannot access...
            openSpace = new bool[Length];

            //PathFinder.setMapSize(w, d);
        }

        abstract protected bool Build();
        abstract protected void CreateMobs();
        abstract protected void CreateItems();

        public void BuildFlagMaps() { /*TODO*/ }

           public void CleanWalls() { /*TODO*/ }

        /*
        public int tunnelTile()
        {
            return feeling == Feeling.CHASM ? Terrain.EMPTY_SP : Terrain.EMPTY;
        }
        */

        public String TilesTex()
        {
            return null;
        }

        public String WaterTex()
        {
            return null;
        }

        public int Entrance()
        {
            //LevelTransition l = getTransition(null);
            //if (l != null)
            //{
            //    return l.cell();
            //}
            return 0;
        }

        public int Exit()
        {
            //LevelTransition l = getTransition(LevelTransition.Type.REGULAR_EXIT);
            //if (l != null)
            //{
            //    return l.cell();
            //}
            return 0;
        }

       //public LevelTransition getTransition(LevelTransition.Type type {}

       //public LevelTransition getTransition(int cell)
 

        public void Seal()
        {
            if (!locked)
            {
                locked = true;
                //Buff.affect(Dungeon.hero, LockedFloor.class);
            }
       }

        public void Unseal()
        {
            if (locked)
            {
                locked = false;
                //if (Dungeon.hero.buff(LockedFloor.class) != null)
                //{
                //    Dungeon.hero.buff(LockedFloor.class).detach();
                //}
            }
        }

        public void Reset()
        {

            //for (Mob mob : mobs.toArray(new Mob[0]))
            //{
            //    if (!mob.reset())
            //    {
            //        mobs.remove(mob);
            //    }
            //}
            CreateMobs();
        }

        public int MobLimit() => 0;
        //TODO Mob stuff later
        //public int MobCount()
        //{
        //    float count = 0;
        //    for (Mob mob : Dungeon.level.mobs.toArray(new Mob[0]))
        //    {
        //        if (mob.alignment == Char.Alignment.ENEMY && !mob.properties().contains(Char.Property.MINIBOSS))
        //        {
        //            count += mob.spawningWeight();
        //        }
        //    }
        //    return Math.round(count);
        //}

        //public Mob findMob(int pos)
        //{
        //    for (Mob mob : mobs)
        //    {
        //        if (mob.pos == pos)
        //        {
        //            return mob;
        //        }
        //    }
        //    return null;
        //}

    }
}
