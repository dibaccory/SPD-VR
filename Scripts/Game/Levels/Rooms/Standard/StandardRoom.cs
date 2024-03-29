﻿using System;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using Utils;
using TileInfo;
using DungeonInstance;


namespace Levels.Rooms.Standard
{

    public abstract class StandardRoom : Room
    {
        public SizeCategory sizeCat;
        static readonly RoomType roomType;

        //Note that if a room wishes to allow itself to be forced to a certain size category,
        //but would (effectively) never roll that size category, consider using Float.MIN_VALUE
        public float[] SizeCatProbs => new float[] { 1, 0, 0 };

        public bool SetSizeCat()
        {
            return SetSizeCat(0, Enum.GetNames(typeof(SizeCategory)).Length - 1);
        }

        //assumes room value is always ordinal+1
        public bool SetSizeCat(int maxRoomValue)
        {
            return SetSizeCat(0, maxRoomValue - 1);
        }

        //returns false if size cannot be set
        public bool SetSizeCat(int minOrdinal, int maxOrdinal)
        {
            float[] probs = SizeCatProbs;
            int nCategories = Enum.GetNames(typeof(SizeCategory)).Length;

            if (probs.Length != nCategories) return false;

            for (int i = 0; i < minOrdinal; i++) probs[i] = 0;
            for (int i = maxOrdinal + 1; i < nCategories; i++) probs[i] = 0;

            //return true;
            int ordinal = RandomNumberGenerator.Chances(probs);

            if (ordinal != -1)
            {
                sizeCat = (SizeCategory)ordinal;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int MinWidth => sizeCat.GetAttribute<SizeCategoryAttr>().MinDim;
        public override int MaxWidth => sizeCat.GetAttribute<SizeCategoryAttr>().MaxDim;

        public override int MinHeight => sizeCat.GetAttribute<SizeCategoryAttr>().MinDim;
        public override int MaxHeight =>  sizeCat.GetAttribute<SizeCategoryAttr>().MaxDim;

        public int ConnectionWeight => ((int)sizeCat+1) * ((int)sizeCat + 1);
   

        private enum RoomType
        {
            [RoomTypeAttr(10,0)]EmptyRoom,

            //[RoomTypeAttr(10,1)]SewerPipeRoom,
            //[RoomTypeAttr(10,1)]RingRoom,
            //[RoomTypeAttr(5,1)]CircleBasinRoom,

            [RoomTypeAttr(10,2)]SegmentedRoom,
            [RoomTypeAttr(10,2)]PillarsRoom,
            [RoomTypeAttr(5,2)]CellBlockRoom,

            [RoomTypeAttr(10,3)]CaveRoom,
            [RoomTypeAttr(10,3)]CavesFissureRoom,
            [RoomTypeAttr(5,3)]CirclePitRoom,

            [RoomTypeAttr(10,4)]HallwayRoom,
            [RoomTypeAttr(10,4)]StatuesRoom,
            [RoomTypeAttr(5,4)]SegmentedLibraryRoom,

            [RoomTypeAttr(10,5)]RuinsRoom,
            [RoomTypeAttr(10,5)]ChasmRoom,
            [RoomTypeAttr(5,5)]SkullsRoom,
            //TODO ADD ROOMS WHEN PROCEDURAL GEN IS DONE
            //[RoomTypeAttr(1,0)]PlantsRoom,
            //[RoomTypeAttr(1,0)]AquariumRoom,
            //[RoomTypeAttr(1,0)]PlatformRoom,
            //[RoomTypeAttr(1,0)]BurnedRoom,
            //[RoomTypeAttr(1,0)]FissureRoom,
            //[RoomTypeAttr(1,0)]GrassyGraveRoom,
            //[RoomTypeAttr(1,0)]StripedRoom,
            //[RoomTypeAttr(1,0)]StudyRoom,
            //[RoomTypeAttr(1,0)]SuspiciousChestRoom,
            //[RoomTypeAttr(1,0)]MinefieldRoom

        }

        public static StandardRoom Create()
        {

            //Console.WriteLine("DUNGEON REGION --- ", Dungeon.region);
            //Console.WriteLine("DUNGEON DEPTH --- ", Dungeon.depth);
            //Filter RoomTypes to only applicable types in current region of the dungeon
            //RoomTypeAttr[] applicableRoomTypes = (RoomTypeAttr[])Enum
            //    .GetValues(typeof(RoomType))
            //    .OfType<RoomTypeAttr>()
            //    .Where( r => r.usableInRegion );

            // Create a list to store the applicable room types
            List<RoomTypeAttr> applicableRoomTypes = new();

            // Iterate over the values of the RoomType enum
            foreach (RoomType e in Enum.GetValues(typeof(RoomType)))
            {
                // Get the RoomTypeAttr attribute for the room type

                RoomTypeAttr usableRoomType = e.GetAttribute<RoomTypeAttr>();
       
                
                if (usableRoomType.usableInRegion)
                {
                    // Add the attribute to the list if it's usable in the region
                    applicableRoomTypes.Add(usableRoomType);

                }
            }


            //Convert to float[] for RNG.Chances
            float[] roomWeights = Array.ConvertAll
                (
                    applicableRoomTypes.ToArray(),
                    new Converter<RoomTypeAttr, float>( r => r.weight )
                );
            
            string chosenRoomName = Enum.GetNames(typeof(RoomType))[RandomNumberGenerator.Chances(roomWeights)];
            var chosenRoomClass = Type.GetType("Levels.Rooms.Standard."+chosenRoomName);
            //Console.WriteLine("chosenRoomName {0} ---- chosenRoomClass --- {1}", chosenRoomName, chosenRoomClass.ToString());
            return (StandardRoom)Activator.CreateInstance(chosenRoomClass);
        }

        public bool CanMerge(Level l, Vector2Int p, int mergeTerrain)
        {
            int cell = l.XYToCell(PointInside(p, 1));
            return !TileInfo.Terrain.TileHasFlag(l.map[cell], TileFlags.Solid);
        }


    }


}
