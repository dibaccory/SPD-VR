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
        public static int branch;
        public int gold;
        public int energy;
        public int challenges;
        public int mobsToChampion;

        //public Hero hero;
        public Level level;

        //public QuickSlot quickslot = new QuickSlot();

        public static int depth;
        public static int region => (int)Math.Ceiling((float)depth / 5);

        public static bool daily;
      	public static bool dailyReplay;
      	public static String customSeedText = "";
      	public static long seed;

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

          seed = SeedGenerator.RandomSeed();

          //RandomNumberGenerator.AddGenerator(seed+1);
          // Scroll.initLabels();
    			// Potion.initColors();
    			// Ring.initGems();
          //
    			// SpecialRoom.initForRun();
    			// SecretRoom.initForRun();
          RandomNumberGenerator.ResetGenerators();




          NewGame();
        }


        void NewGame()
        {
            //Load saved details if(level == null) then create new level
            level = new SewerLevel();
            level.Create();
        }

        public static long SeedCurDepth(){
          return SeedForDepth(depth, branch);
        }

        public static long SeedForDepth(int depth, int branch){
          int lookAhead = depth;
          lookAhead += 30*branch; //Assumes depth is always 1-30, and branch is always 0 or higher

          RandomNumberGenerator.AddGenerator( seed );

            for (int i = 0; i < lookAhead; i ++) {
              RandomNumberGenerator.Long(); //we don't care about these values, just need to go through them
            }
            long result = RandomNumberGenerator.Long();

          RandomNumberGenerator.PopGenerator();
          return result;
        }

    }
}
