using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Rooms;
using Utils;
using Unity.VisualScripting;
using UnityEditor;

namespace Levels.Builders
{
    //Introduces the concept of a main path, and branches
    // with tunnels padding rooms placed in them
    public abstract class RegularBuilder : Builder
    {
        // *** Parameter values for level building logic ***
        // note that implementations do not have to use al of these variables

        protected float pathVariance = 45f;

        public RegularBuilder setPathVariance(float var)
        {
            pathVariance = var;
            return this;
        }

        //path length is the percentage of pathable rooms that are on the main path
        protected float pathLength = 0.25f;
        //The chance weights for extra rooms to be added to the path
        protected float[] pathLenJitterChances = new float[] { 0, 0, 0, 1 };
        //default is 25% of multi connection rooms, plus 3

        public RegularBuilder setPathLength(float len, float[] jitter)
        {
            pathLength = len;
            pathLenJitterChances = jitter;
            return this;
        }

        protected float[] pathTunnelChances = new float[] { 2, 2, 1 };
        protected float[] branchTunnelChances = new float[] { 1, 1, 0 };

        public RegularBuilder setTunnelLength(float[] path, float[] branch)
        {
            pathTunnelChances = path;
            branchTunnelChances = branch;
            return this;
        }

        //each adjacency is processed twice, so this gives a ~50% chance to connect two adjacent rooms
        protected float extraConnectionChance = 0.30f;

        public RegularBuilder setExtraConnectionChance(float chance)
        {
            extraConnectionChance = chance;
            return this;
        }

        // *** Room Setup ***

        protected Room entrance = null;
        protected Room exit = null;
        protected Room shop = null;

        protected List<Room> mainPathRooms = new List<Room>();

        protected List<Room> multiConnections = new List<Room>();
        protected List<Room> singleConnections = new List<Room>();

        protected void setupRooms(List<Room> rooms)
        {
            foreach (Room r in rooms)
            {
                r.zero();
            }

            entrance = exit = shop = null;
            mainPathRooms.Clear();
            singleConnections.Clear();
            multiConnections.Clear();
            foreach (Room r in rooms)
            {
                Type rType = r.GetType();
                if (rType.IsInstanceOfType(typeof(EntranceRoom))) entrance = r;
                else if (rType.IsInstanceOfType(typeof(ExitRoom))) exit = r;
                else if (rType.IsInstanceOfType(typeof(ShopRoom)) && r.MaxConnections(Room.ALL) == 1) shop = r;
                else if (r.MaxConnections(Room.ALL) > 1) multiConnections.Add(r);
                else if (r.MaxConnections(Room.ALL) == 1) singleConnections.Add(r);
            }


            //this weights larger rooms to be much more likely to appear in the main loop, by placing them earlier in the multiconnections list
            WeightRooms(multiConnections);
            RandomNumberGenerator.Shuffle(multiConnections);
            multiConnections = new List<Room>(new LinkedList<Room>(multiConnections));
            //shuffle one more time to ensure that the actual ordering of the path doesn't put big rooms early
            RandomNumberGenerator.Shuffle(multiConnections);

            int roomsOnMainPath = (int)(multiConnections.Count * pathLength) + RandomNumberGenerator.Chances(pathLenJitterChances);

            while (roomsOnMainPath > 0 && multiConnections.Count > 0)
            {
                Room r = multiConnections.First();
                multiConnections.RemoveAt(0);
                Type rType = r.GetType();
                if (rType.IsInstanceOfType(typeof(StandardRoom)))
                {
                    roomsOnMainPath -= (int)((StandardRoom)r).sizeCat;
                }
                else
                {
                    roomsOnMainPath--;
                }
                mainPathRooms.Add(r);
            }
        }

        // *** Branch Placement ***

        void WeightRooms(List<Room> rooms)
        {
            foreach (Room r in rooms)
            {
                Type rType = r.GetType();
                if (rType.IsInstanceOfType(typeof(StandardRoom)))
                {
                    for (int i = 1; i < ((StandardRoom)r).ConnectionWeight; i++)
                        rooms.Add(r);
                }
            }
        }

        //places the rooms in roomsToBranch into branches from rooms in branchable.
        //note that the three arrays should be separate, they may contain the same rooms however
        protected void CreateBranches(List<Room> rooms, List<Room> branchable, List<Room> roomsToBranch, float[] connChances)
        {

            int i = 0;
            float angle;
            int tries;
            Room curr;
            List<Room> connectingRoomsThisBranch = new List<Room>();

            float[] connectionChances = (float[])connChances.Clone();
            while (i < roomsToBranch.Count)
            {

                Room r = roomsToBranch[i];
                Type rType = r.GetType();
                bool isSecretRoom = rType.IsInstanceOfType(typeof(SecretRoom));
                connectingRoomsThisBranch.Clear();

                do
                {
                    curr = RandomNumberGenerator.Element(branchable);
                } while (isSecretRoom && curr.GetType().IsInstanceOfType(typeof(ConnectionRoom)));

                int connectingRooms = RandomNumberGenerator.Chances(connectionChances);
                if (connectingRooms == -1)
                {
                    connectionChances = (float[])connChances.Clone();
                    connectingRooms = RandomNumberGenerator.Chances(connectionChances);
                }
                connectionChances[connectingRooms]--;

                for (int j = 0; j < connectingRooms; j++)
                {
                    ConnectionRoom t = isSecretRoom ? new MazeConnectionRoom() : ConnectionRoom.createRoom();
                    tries = 3;

                    do
                    {
                        angle = PlaceRoom(rooms, curr, t, RandomBranchAngle(curr));
                        tries--;
                    } while (angle == -1 && tries > 0);

                    if (angle == -1)
                    {
                        t.clearConnections();
                        foreach (Room c in connectingRoomsThisBranch)
                        {
                            c.ClearConnections();
                            rooms.Remove(c);
                        }
                        connectingRoomsThisBranch.Clear();
                        break;
                    }
                    else
                    {
                        connectingRoomsThisBranch.Add(t);
                        rooms.Add(t);
                    }

                    curr = t;
                }

                if (connectingRoomsThisBranch.Count != connectingRooms)
                {
                    continue;
                }
                tries = 10;

                do
                {
                    angle = PlaceRoom(rooms, curr, r, RandomBranchAngle(curr));
                    tries--;
                } while (angle == -1 && tries > 0);

                if (angle == -1)
                {
                    r.ClearConnections();
                    foreach (Room t in connectingRoomsThisBranch)
                    {
                        t.ClearConnections();
                        rooms.Remove(t);
                    }
                    connectingRoomsThisBranch.Clear();
                    continue;
                }

                for (int j = 0; j < connectingRoomsThisBranch.Count; j++)
                {
                    if (RandomNumberGenerator.Int(3) <= 1) branchable.Add(connectingRoomsThisBranch[j]);
                }
                if (r.MaxConnections(Room.ALL) > 1 && RandomNumberGenerator.Int(3) == 0)
                {
                    if (rType.IsInstanceOfType(typeof(StandardRoom)))
                    {
                        for (int j = 0; j < ((StandardRoom)r).ConnectionWeight; j++)
                        {
                            branchable.Add(r);
                        }
                    }
                    else
                    {
                        branchable.Add(r);
                    }
                }

                i++;
            }
        }

        protected float RandomBranchAngle(Room r) => (float)RandomNumberGenerator.Double((double)360);
    }
}
