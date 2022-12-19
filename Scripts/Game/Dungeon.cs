using System;
using Levels;
using Utils;

namespace DungeonInstance
{
    public class Dungeon
    {

        //determines path the hero is on. Current uses:
        // 0 is the default path
        // Other numbers are currently unused
        public int branch;
        public int gold;
        public int energy;
        public int challenges;
        public int mobsToChampion;

        //public Hero hero;
        public Level level;

        //public QuickSlot quickslot = new QuickSlot();

        public static int depth;
        public static int region => (int)Math.Ceiling((float)depth / 5);

        public enum Regions
        {
            ANY,
            SEWER,
            PRISON,
            CAVES,
            CITY,
            HALLS
        }

        public float[][] standardRoomChances = new float[27][];

        public Dungeon()
        {
          depth = 1;
          branch = 0;
          gold = 0;
          energy = 0;
          level = null;
          Start();
        }


        void Start()
        {
            //Load saved details if(level == null) then create new level
            level = new SewerLevel();
            level.Create();
        }

    }
}
