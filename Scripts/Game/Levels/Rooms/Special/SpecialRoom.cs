using System;
using Levels.Rooms;
using System.Linq;
using DungeonInstance;
using Utils;
using Levels.Rooms.Standard;
using UnityEditor;

namespace Levels.Rooms.Special
{
    public abstract class SpecialRoom : Room
    {
        public static SpecialRoom Create()
        {

            //if (Dungeon.depth == pitNeededDepth)
            //{
            //    pitNeededDepth = -1;

            //    useType( PitRoom.class );
            //    return new PitRoom();

            //}
            //else if (floorSpecials.contains(LaboratoryRoom.class))
            //{

            //    useType(LaboratoryRoom.class);
            //    return new LaboratoryRoom();

            //}
            //else
            //{

            //    if (Dungeon.bossLevel(Dungeon.depth + 1))
            //    {
            //        floorSpecials.remove(WeakFloorRoom.class);
            //    }

            //    //60% chance for front of queue, 30% chance for next, 10% for one after that
                int index = RandomNumberGenerator.Chances(new float[]{6, 3, 1});
                //while (index >= floorSpecials.size()) index--;
                string chosenRoomName = Enum.GetNames(typeof(RoomType))[index];
                var chosenRoomClass = Type.GetType(RoomType.PitRoom.ToString());
                return (SpecialRoom)Activator.CreateInstance(chosenRoomClass); //Reflection.newInstance(floorSpecials.get( index ));

            //    if (r instanceof WeakFloorRoom)
            //    {
            //        pitNeededDepth = Dungeon.depth + 1;
            //    }

            //    useType( r.getClass() );

            //}
        }

        private enum RoomType
        {
          //TODO: Remove duplicates, customize RoomTypeAttr
          //9 special rooms which give equipment more often than consumables (or as often as)
          WeakFloorRoom,
          CryptRoom,
          PoolRoom,
          ArmoryRoom,
          SentryRoom,
    			StatueRoom,
           CrystalVaultRoom,
           CrystalChoiceRoom,
           SacrificeRoom,

           //10 special rooms which give consumables more often than equipment
         	//note that alchemy rooms are spawned separately
          RunestoneRoom,
           GardenRoom,
           LibraryRoom,
           StorageRoom,
    			TreasuryRoom,
           MagicWellRoom,
           ToxicGasRoom,
           MagicalFireRoom,
    			    TrapsRoom,
           CrystalPathRoom,

           	//only one special that uses crystal keys per floor
            PitRoom,
            //CrystalVaultRoom,
            //CrystalChoiceRoom,
            //CrystalPathRoom,

            //only one special that generates a potion per floor
            //PoolRoom,
            //SentryRoom,
            //StorageRoom,
            //ToxicGasRoom,
            //MagicalFireRoom,
            //TrapsRoom
        }
    }
}
