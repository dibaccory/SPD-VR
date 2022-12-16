using System;
using System.Collections.Generic;
using Levels.Rooms;
using static Levels.Rooms.Room;
using Utils;

namespace Levels
{
    public abstract class RegularLevel : Level
    {

        protected List<Room> rooms;
        protected Room roomEntrance;
        protected Room roomExit;

        override
        protected bool Build()
        {


            return false;
        }

        protected List<Room> CreateRooms()
        {
            List<Room> r = new List<Room>();
            //r.Add(roomEntrance = new EntranceRoom());
            //r.Add(roomExit = new ExitRoom());

            //force max standard rooms and multiple by 1.5x for large levels
            int standards = StandardRooms(feeling == Feeling.LARGE);
            if (feeling == Feeling.LARGE)
            {
                standards = (int)Math.Ceiling(standards * 1.5f);
            }
            for (int i = 0; i < standards; i++)
            {
                StandardRoom s;
                do
                {
                    s = StandardRoom.Create();
                } while (!s.SetSizeCat(standards - i));
                i += s.sizeCat.GetAttribute<SizeCategoryAttr>().RoomValue - 1;
                r.Add(s);
            }

            return r;
        }

        protected int StandardRooms(bool forceMax) { return 0; }

        override
        protected void CreateMobs() { }

        override
        protected void CreateItems() { }


    }
}
