﻿using System;
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

        protected int width;
        protected int depth;
        protected int length;

        protected static readonly float TIME_TO_RESPAWN = 50;

        public int version;

        public int[] map;
        public bool[] visited;
        public bool[] mapped;
        public bool[] discoverable;

        //public int viewDistance = Dungeon.isChallenged(Challenges.DARKNESS) ? 2 : 8;

        public bool[] heroFOV;

        public bool[] passable;
        public bool[] losBlocking;
        public bool[] flamable;
        public bool[] secret;
        public bool[] solid;
        public bool[] avoid;
        public bool[] water;
        public bool[] pit;
        public bool[] openSpace;
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

        public void Create()
        {
            //TODO if (NOT Boss Level..) Line 190

            do
            {
                width = depth = length = 0;

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

            //Random.popGenerator();
        }

        public void SetSize(int w, int d)
        {

            width = w;
            depth = d;
            length = w * d;

            map = new int[length];
            //Arrays.fill(map, feeling == Level.Feeling.CHASM ? Terrain.CHASM : Terrain.WALL);
            heroFOV = new bool[length];
            visited = new bool[length];
            mapped = new bool[length];
            passable = new bool[length];
            losBlocking = new bool[length];
            flamable = new bool[length];
            secret = new bool[length];
            solid = new bool[length];
            avoid = new bool[length];
            water = new bool[length];
            pit = new bool[length];

            openSpace = new bool[length];

            //PathFinder.setMapSize(w, d);
        }

        abstract protected bool Build();
        abstract protected void CreateMobs();
        abstract protected void CreateItems();

        public void BuildFlagMaps() { /*TODO*/ }

           public void CleanWalls() { /*TODO*/ }

        public int Width()
        {
            return width;
        }

        public int Depth()
        {
            return depth;
        }

        public int Length()
        {
            return length;
        }

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

    }
}
