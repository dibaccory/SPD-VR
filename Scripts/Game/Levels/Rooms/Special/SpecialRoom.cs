using System;
using Levels, Levels.Room;
using DungeonInstance;

namespace Levels.Rooms.Special
{
  public abstract class SpecialRoom : Room
  {
    public static SpecialRoom createRoom()
    {
        if (Dungeon.depth == pitNeededDepth)
        {
            pitNeededDepth = -1;

            useType( PitRoom.class );
            return new PitRoom();

        }
        else if (floorSpecials.contains(LaboratoryRoom.class))
        {

            useType(LaboratoryRoom.class);
            return new LaboratoryRoom();

        }
        else
        {

            if (Dungeon.bossLevel(Dungeon.depth + 1))
            {
                floorSpecials.remove(WeakFloorRoom.class);
            }

            //60% chance for front of queue, 30% chance for next, 10% for one after that
            int index = Random.chances(new float[]{6, 3, 1});
            while (index >= floorSpecials.size()) index--;

            Room r = Reflection.newInstance(floorSpecials.get( index ));

            if (r instanceof WeakFloorRoom)
            {
                pitNeededDepth = Dungeon.depth + 1;
            }

            useType( r.getClass() );
            return (SpecialRoom)r;
        }
    }
  }
}
