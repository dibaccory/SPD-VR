using System;
using System.Linq;
using Utils;
using TileInfo;
using DungeonInstance;


namespace Levels.Rooms.Standard
{

    public abstract class StandardRoom : Room
    {
        public SizeCategory sizeCat;
        static RoomType roomType;

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


        //private static List<StandardRoom> roomTypes = new List<StandardRoom>();

        public class RoomTypeAttr : Attribute
        {
            internal RoomTypeAttr(float weight, int region)
            {
                this.weight = weight;
                this.region = region;
            }
            

            public float weightAtLevel => (region == Dungeon.region || region == 0) ? weight : 0;
            public bool usableInRegion => (region == Dungeon.region || region == 0);
            public float weight { get; private set; }
            public int region { get; private set; }
        }

        private enum RoomType
        {
            [RoomTypeAttr(10,0)]EmptyRoom,

            [RoomTypeAttr(10,1)]SewerPipeRoom,
            [RoomTypeAttr(10,1)]RingRoom,
            [RoomTypeAttr(5,1)]CircleBasinRoom,

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

            [RoomTypeAttr(1,0)]PlantsRoom,
            [RoomTypeAttr(1,0)]AquariumRoom,
            [RoomTypeAttr(1,0)]PlatformRoom,
            [RoomTypeAttr(1,0)]BurnedRoom,
            [RoomTypeAttr(1,0)]FissureRoom,
            [RoomTypeAttr(1,0)]GrassyGraveRoom,
            [RoomTypeAttr(1,0)]StripedRoom,
            [RoomTypeAttr(1,0)]StudyRoom,
            [RoomTypeAttr(1,0)]SuspiciousChestRoom,
            [RoomTypeAttr(1,0)]MinefieldRoom

        }

        public static StandardRoom Create()
        {
            //Filter RoomTypes to only applicable types in current region of the dungeon
            RoomTypeAttr[] applicableRoomTypes = (RoomTypeAttr[])Enum
                .GetValues(typeof(RoomType))
                .OfType<RoomTypeAttr>()
                .Where( r => r.usableInRegion );

            //Convert to float[] for RNG.Chances
            float[] roomWeights = Array.ConvertAll
                (
                    applicableRoomTypes,
                    new Converter<RoomTypeAttr, float>( r => r.weight )
                );

            string chosenRoomName = Enum.GetNames(typeof(RoomType))[RandomNumberGenerator.Chances(roomWeights)];
            var chosenRoomClass = Type.GetType(chosenRoomName);

            return (StandardRoom)Activator.CreateInstance(chosenRoomClass);
        }

        public override bool CanMerge(Level l, Vector2Int p, int mergeTerrain)
        {
            int cell = l.PointToCell(PointInside(p, 1));
            return !Terrain.TileHasFlag(l.map[cell], TileFlags.Solid);
        }


    }


}
