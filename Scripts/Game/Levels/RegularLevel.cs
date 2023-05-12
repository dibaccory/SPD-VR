using System;
using System.Collections.Generic;
using Levels.Rooms;
using Levels.Rooms.Standard;
using Levels.Builders;
using Levels.Painters;
using Utils;
using DungeonInstance;

namespace Levels
{
    public abstract class RegularLevel : Level
    {

        protected List<Room> rooms;
        protected Builder builder;
        protected Room roomEntrance;
        protected Room roomExit;

        
        protected override bool Build()
        {
            builder = Builder();
            List<Room> initRooms = CreateRooms();
            RandomNumberGenerator.Shuffle(initRooms);

            do
            {
                foreach(Room r in initRooms)
                {
        				r.neighbors.Clear();
        				r.connected.Clear();
        		}
        	    rooms = builder.Build(new List<Room>(initRooms));
                //Console.WriteLine("{0}      ----    {1}",rooms);
        	} while (rooms == null);

            return false; //create RegularPainter.paint()
        }

        protected List<Room> CreateRooms()
        {
            List<Room> initRooms = new()
            {
                (roomEntrance = new EntranceRoom()),
                (roomExit = new ExitRoom())
            };

            //force max standard rooms and multiple by 1.5x for large levels
            int standards = 10; //TODO: StandardRooms(feeling == Feeling.LARGE);
            // if (feeling == Feeling.LARGE)
            // {
            //     standards = (int)Math.Ceiling(standards * 1.5f);
            // }
            for (int i = 0; i < standards; i++)
            {
                StandardRoom s;
                do
                {
                    s = StandardRoom.Create();
                } while (!s.SetSizeCat(standards - i));
                Console.WriteLine("We have escaped with a room! {0,3}", s.ToString());
                i += s.sizeCat.GetAttribute<SizeCategoryAttr>().RoomValue - 1;
                initRooms.Add(s);
                Console.WriteLine("initRooms count {0}", initRooms.Count);
            }
            Console.WriteLine("WE GOT ALL THE INIT ROOMS");
            return initRooms;
        }

        protected Builder Builder()
        {

          return new LoopBuilder()
              .SetLoopShape( 2 ,
                  (float)RandomNumberGenerator.Double(0f, 0.65f),
                  (float)RandomNumberGenerator.Double(0f, 0.50f));
          //TODO: Add this after optimizing LoopBuilder and FigureEightBuilder
      		// if (RandomNumberGenerator.Int(2) == 0)
          // {
      		// 	return new LoopBuilder()
      		// 			.setLoopShape( 2 ,
      		// 					RandomNumberGenerator.Float(0f, 0.65f),
      		// 					RandomNumberGenerator.Float(0f, 0.50f));
      		// }
          // else
          // {
          //
      		// 	return new FigureEightBuilder()
      		// 			.setLoopShape( 2 ,
      		// 					RandomNumberGenerator.Float(0.3f, 0.8f),
      		// 					0f);
      		// }

      	}

        protected abstract Painter Painter();

        protected int nTraps()
        {
            return RandomNumberGenerator.NormalIntRange(2, 3 + (Dungeon.depth / 5));
        }

        //protected T[] trapClasses() where T : Trap
        //{
        //    return new T[] { WornDartTrap.class};
        //}

        //protected float[] trapChances()
        //{
        //    return new float[] { 1 };
        //}

        protected virtual int StandardRooms(bool forceMax) { return 0; }

        protected virtual int SpecialRooms(bool forceMax)
        {
      		return 0;
      	}

        protected override void CreateMobs() { }

        protected override void CreateItems() { }


    }
}
