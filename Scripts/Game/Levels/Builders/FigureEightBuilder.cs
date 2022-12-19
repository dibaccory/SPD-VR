using System;
using System.Collections.Generic;
using Levels.Rooms;

namespace Levels.Builders
{
	/*
	public class FigureEightBuilder : RegularBuilder
	{
		public FigureEightBuilder()
		{

		}
		public ArrayList<Room> build(ArrayList<Room> rooms) {
			setupRooms(rooms);

			if (landmarkRoom == null){
				//prefer large and giant standard rooms over others
				for (Room r : mainPathRooms){
					if ( r.maxConnections(Room.ALL) >= 4 &&
							(landmarkRoom == null || landmarkRoom.minWidth()*landmarkRoom.minHeight() < r.minWidth()*r.minHeight())){
						landmarkRoom = r;
					}
				}
				//add another room to the path to compensate
				if (!multiConnections.isEmpty()){
					mainPathRooms.add(multiConnections.remove(0));
				}
			}
			mainPathRooms.remove(landmarkRoom);
			multiConnections.remove(landmarkRoom);

			float startAngle = RandomNumberGenerator.Float(0, 360);

			int roomsOnFirstLoop = mainPathRooms.size()/2;
			if (mainPathRooms.size() % 2 == 1) roomsOnFirstLoop += RandomNumberGenerator.Int(2);

			ArrayList<Room> roomsToLoop = (ArrayList<Room>) mainPathRooms.clone();

			ArrayList<Room> firstLoopTemp = new ArrayList<>();
			firstLoopTemp.add(landmarkRoom);
			for (int i = 0; i < roomsOnFirstLoop; i++){
				firstLoopTemp.add(roomsToLoop.remove(0));
			}
			firstLoopTemp.add((firstLoopTemp.size()+1)/2, entrance);

			float[] pathTunnels = pathTunnelChances.clone();

			firstLoop = new ArrayList<>();
			for (Room r : firstLoopTemp){
				firstLoop.add(r);

				int tunnels = RandomNumberGenerator.chances(pathTunnels);
				if (tunnels == -1){
					pathTunnels = pathTunnelChances.clone();
					tunnels = RandomNumberGenerator.chances(pathTunnels);
				}
				pathTunnels[tunnels]--;

				for (int j = 0; j < tunnels; j++){
					firstLoop.add(ConnectionRoom.createRoom());
				}
			}
			ArrayList<Room> secondLoopTemp = new ArrayList<>();
			secondLoopTemp.add(landmarkRoom);
			secondLoopTemp.addAll(roomsToLoop);
			secondLoopTemp.add((secondLoopTemp.size()+1)/2, exit);

			secondLoop = new ArrayList<>();
			for (Room r : secondLoopTemp){
				secondLoop.add(r);

				int tunnels = RandomNumberGenerator.chances(pathTunnels);
				if (tunnels == -1){
					pathTunnels = pathTunnelChances.clone();
					tunnels = RandomNumberGenerator.chances(pathTunnels);
				}
				pathTunnels[tunnels]--;

				for (int j = 0; j < tunnels; j++){
					secondLoop.add(ConnectionRoom.createRoom());
				}
			}

			landmarkRoom.setSize();
			landmarkRoom.setPos(0, 0);

			Room prev = landmarkRoom;
			float targetAngle;
			for (int i = 1; i < firstLoop.size(); i++){
				Room r = firstLoop.get(i);
				targetAngle = startAngle + targetAngle( i / (float)firstLoop.size());
				if (placeRoom(rooms, prev, r, targetAngle) != -1) {
					prev = r;
					if (!rooms.contains(prev))
						rooms.add(prev);
				} else {
					//FIXME this is lazy, there are ways to do this without relying on chance
					return null;
				}
			}

			//FIXME this is still fairly chance reliant
			// should just write a general function for stitching two rooms together in builder
			while (!prev.connect(landmarkRoom)){

				ConnectionRoom c = ConnectionRoom.createRoom();
				if (placeRoom(rooms, prev, c, angleBetweenRooms(prev, landmarkRoom)) == -1){
					return null;
				}
				firstLoop.add(c);
				rooms.add(c);
				prev = c;
			}

			prev = landmarkRoom;
			startAngle += 180f;
			for (int i = 1; i < secondLoop.size(); i++){
				Room r = secondLoop.get(i);
				targetAngle = startAngle + targetAngle( i / (float)secondLoop.size());
				if (placeRoom(rooms, prev, r, targetAngle) != -1) {
					prev = r;
					if (!rooms.contains(prev))
						rooms.add(prev);
				} else {
					//FIXME this is lazy, there are ways to do this without relying on chance
					return null;
				}
			}

			//FIXME this is still fairly chance reliant
			// should just write a general function for stitching two rooms together in builder
			while (!prev.connect(landmarkRoom)){

				ConnectionRoom c = ConnectionRoom.createRoom();
				if (placeRoom(rooms, prev, c, angleBetweenRooms(prev, landmarkRoom)) == -1){
					return null;
				}
				secondLoop.add(c);
				rooms.add(c);
				prev = c;
			}

			if (shop != null) {
				float angle;
				int tries = 10;
				do {
					angle = placeRoom(rooms, entrance, shop, RandomNumberGenerator.Float(360f));
					tries--;
				} while (angle == -1 && tries >= 0);
				if (angle == -1) return null;
			}

			firstLoopCenter = new PointF();
			for (Room r : firstLoop){
				firstLoopCenter.x += (r.left + r.right)/2f;
				firstLoopCenter.y += (r.top + r.bottom)/2f;
			}
			firstLoopCenter.x /= firstLoop.size();
			firstLoopCenter.y /= firstLoop.size();

			secondLoopCenter = new PointF();
			for (Room r : secondLoop){
				secondLoopCenter.x += (r.left + r.right)/2f;
				secondLoopCenter.y += (r.top + r.bottom)/2f;
			}
			secondLoopCenter.x /= secondLoop.size();
			secondLoopCenter.y /= secondLoop.size();

			ArrayList<Room> branchable = new ArrayList<>(firstLoop);
			branchable.addAll(secondLoop);
			branchable.remove(landmarkRoom); //remove once so it isn't present twice

			ArrayList<Room> roomsToBranch = new ArrayList<>();
			roomsToBranch.addAll(multiConnections);
			roomsToBranch.addAll(singleConnections);
			weightRooms(branchable);
			createBranches(rooms, branchable, roomsToBranch, branchTunnelChances);

			findNeighbours(rooms);

			for (Room r : rooms){
				for (Room n : r.neigbours){
					if (!n.connected.containsKey(r)
							&& RandomNumberGenerator.Float() < extraConnectionChance){
						r.connect(n);
					}
				}
			}

			return rooms;
		}

	}
	*/
}
