using System;
using System.Collections.Generic;
using UnityEngine;
using Levels.Rooms;
using Levels.Rooms.Connection;
using Utils;

namespace Levels.Builders
{
	public class LoopBuilder : RegularBuilder
	{
		public LoopBuilder()
		{

		}


		//These methods allow for the adjusting of the shape of the loop
		//by default the loop is a perfect circle, but it can be adjusted

		//increasing the exponent will increase the the curvature, making the loop more oval shaped.
		private int curveExponent = 0;

		//This is a percentage (range 0-1) of the intensity of the curve function
		// 0 makes for a perfect linear curve (circle)
		// 1 means the curve is completely determined by the curve exponent
		private float curveIntensity = 1;

		//Adjusts the starting point along the loop.
		// a common example, setting to 0.25 will make for a short fat oval instead of a long one.
		private float curveOffset = 0;

		public LoopBuilder SetLoopShape(int exponent, float intensity, float offset){
			this.curveExponent = Math.Abs(exponent);
			curveIntensity = intensity % 1f;
			curveOffset = offset % 0.5f;
			return this;
		}

		private float TargetAngle( float percentAlong ){
			percentAlong += curveOffset;
			return 360f * (float)(
							curveIntensity * CurveEquation(percentAlong)
							+ (1-curveIntensity)*(percentAlong)
							- curveOffset);
		}

		private double CurveEquation( double x ){
			return Math.Pow(4, 2*curveExponent)
					*(Math.Pow((x % 0.5f )-0.25, 2*curveExponent + 1))
					+ 0.25 + 0.5*Math.Floor(2*x);
		}

		private Vector2 loopCenter;

		public override List<Room> Build(List<Room> rooms) {

			setupRooms(rooms);

			if (entrance == null){
				return null;
			}

			entrance.SetSize();
			entrance.SetPos(0, 0);

			float startAngle = (float)RandomNumberGenerator.Double(0, 360);

			mainPathRooms.Insert(0, entrance);
			mainPathRooms.Insert((mainPathRooms.Count+1)/2, exit);

			List<Room> loop = new();
			float[] pathTunnels = (float[])pathTunnelChances.Clone();
			foreach(Room r in mainPathRooms){
				loop.Add(r);

				int tunnels = RandomNumberGenerator.Chances(pathTunnels);
				if (tunnels == -1){
					pathTunnels = (float[])pathTunnelChances.Clone();
					tunnels = RandomNumberGenerator.Chances(pathTunnels);
				}
				pathTunnels[tunnels]--;

				for (int j = 0; j < tunnels; j++){
					loop.Add(ConnectionRoom.Create());
				}
			}

			Room prev = entrance;
			float targetAngle;
			for (int i = 1; i < loop.Count; i++){
				Room r = loop[i];
				targetAngle = startAngle + TargetAngle( i / (float)loop.Count);
				if (PlaceRoom(rooms, prev, r, targetAngle) != -1) {
					prev = r;
					if (!rooms.Contains(prev))
						rooms.Add(prev);
				} else {
					//FIXME this is lazy, there are ways to do this without relying on chance
					return null;
				}
			}

			//FIXME this is still fairly chance reliant
			// should just write a general function for stitching two rooms together in builder
			while (!prev.Connect(entrance)){

				ConnectionRoom c = ConnectionRoom.Create();
				if (PlaceRoom(loop, prev, c, AngleBetweenRooms(prev, entrance)) == -1){
					return null;
				}
				loop.Add(c);
				rooms.Add(c);
				prev = c;
			}

			if (shop != null) {
				float angle;
				int tries = 10;
				do {
					angle = PlaceRoom(loop, entrance, shop, (float)RandomNumberGenerator.Double(360f));
					tries--;
				} while (angle == -1 && tries >= 0);
				if (angle == -1) return null;
			}

			loopCenter = new();
			foreach(Room r in loop){
				loopCenter.x += (r.left + r.right)/2f;
				loopCenter.y += (r.top + r.bottom)/2f;
			}
			loopCenter.x /= loop.Count;
			loopCenter.y /= loop.Count;

			List<Room> branchable = new(loop);

			List<Room> roomsToBranch = new();
			roomsToBranch.AddRange(multiConnections);
			roomsToBranch.AddRange(singleConnections);
			WeightRooms(branchable);
			CreateBranches(rooms, branchable, roomsToBranch, branchTunnelChances);

			FindNeighbors(rooms);

			foreach(Room r in rooms){
				foreach(Room n in r.neighbors){
					if (!n.connected.ContainsKey(r)
							&& RandomNumberGenerator.Double() < extraConnectionChance){
						r.Connect(n);
					}
				}
			}

			return rooms;
		}

		protected override float RandomBranchAngle( Room r ) {
			if (loopCenter == null)
				return base.RandomBranchAngle( r );
			else {
				//generate four angles RandomNumberGeneratorly and return the one which points closer to the center
				float toCenter = AngleBetweenRoomCenters( new Vector2((r.left + r.right)/2f, (r.top + r.bottom)/2f), loopCenter);
				if (toCenter < 0) toCenter += 360f;

				float currAngle = (float)RandomNumberGenerator.Double(360f);
				for( int i = 0; i < 4; i ++){
					float newAngle = (float)RandomNumberGenerator.Double(360f);
					if (Math.Abs(toCenter - newAngle) < Math.Abs(toCenter - currAngle)){
						currAngle = newAngle;
					}
				}
				return currAngle;
			}
		}

	}
}
