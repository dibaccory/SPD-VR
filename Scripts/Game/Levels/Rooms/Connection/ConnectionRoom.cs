using System;
using Levels.Rooms;
using DungeonInstance;
using Utils;

namespace Levels.Rooms.Connection
{
	public abstract class ConnectionRoom : Room
	{
		public override int MinWidth => 3;
		public override int MaxWidth => 10;


		public override int MinHeight => 3;
		public override int MaxHeight => 10;


		public override int MinConnections(int direction) {
			if (direction == ALL)   return 2;
			else                    return 0;
		}

		// //FIXME this is a very messy way of handing variable connection rooms
		// private static ArrayList<Class<?extends ConnectionRoom>> rooms = new ArrayList<>();
		// static {
		// 	rooms.add(TunnelRoom.class);
		// 	rooms.add(BridgeRoom.class);
		//
		// 	rooms.add(PerimeterRoom.class);
		// 	rooms.add(WalkwayRoom.class);
		//
		// 	rooms.add(RingTunnelRoom.class);
		// 	rooms.add(RingBridgeRoom.class);
		// }
		//
		// private static float[][] chances = new float[27][];
		// static {
		// 	chances[1] =  new float[]{20, 1,    0, 2,       2, 1};
		// 	chances[4] =  chances[3] = chances[2] = chances[1];
		// 	chances[5] =  new float[]{20, 0,    0, 0,       0, 0};
		//
		// 	chances[6] =  new float[]{0, 0,     22, 3,      0, 0};
		// 	chances[10] = chances[9] = chances[8] = chances[7] = chances[6];
		//
		// 	chances[11] = new float[]{12, 0,    0, 5,       5, 3};
		// 	chances[15] = chances[14] = chances[13] = chances[12] = chances[11];
		//
		// 	chances[16] = new float[]{0, 0,     18, 3,      3, 1};
		// 	chances[20] = chances[19] = chances[18] = chances[17] = chances[16];
		//
		// 	chances[21] = chances[5];
		//
		// 	chances[22] = new float[]{15, 4,    0, 2,       3, 2};
		// 	chances[26] = chances[25] = chances[24] = chances[23] = chances[22];
		// }

		private enum RoomType
		{
			[RoomTypeAttr(0,0)]TunnelRoom,
			[RoomTypeAttr(0,0)]BridgeRoom,
			[RoomTypeAttr(0,0)]PerimeterRoom,
			[RoomTypeAttr(0,0)]WalkwayRoom,
			[RoomTypeAttr(0,0)]RingTunnelRoom,
			[RoomTypeAttr(0,0)]RingBridgeRoom,
		}

		public static ConnectionRoom Create()
		{
			string chosenRoomName = Enum.GetNames(typeof(RoomType))[RandomNumberGenerator.Chances(new float[] {0,0,0})];
			var chosenRoomClass = Type.GetType(chosenRoomName);

			return (ConnectionRoom)Activator.CreateInstance(chosenRoomClass);
		}
	}
}
