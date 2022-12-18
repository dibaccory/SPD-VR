using System;
using System.Collections.Generic;

namespace Terrain
{
    public enum Tile
    {
        Chasm,
        Empty,
        Grass,
        EmptyWell,
        Water,
        Wall,
        Door,
        OpenDoor,
        Entrance,
        Exit,
        Embers,
        LockedDoor,
        CrystalDoor,
        Pedestal,
        WallDecor,
        Barricade,
        EmptySp,
        HighGrass,
        FurrowedGrass,
        SecretDoor,
        SecretTrap,
        Trap,
        InactiveTrap,
        EmptyDecor,
        LockedExit,
        UnlockedExit,
        Well,
        Statue,
        StatueSp,
        Bookshelf,
        Alchemy
    }

    [Flags]
    public enum TileFlags
    {
        Passable    = 0x01,
        LOSBlocking = 0x02, //Line-Of-Sight Blocking
        Flammable   = 0x04,
        Secret      = 0x08,
        Solid       = 0x10,
        Avoid       = 0x20,
        Liquid      = 0x40,
        Pit         = 0x80
    }

    public static class Terrain
    {
        public static Tile Discover(Tile t)
        {
            if (t == Tile.SecretDoor) t = Tile.Door;
            else if (t == Tile.SecretTrap) t = Tile.Trap;
            return t;
        }

        public static Dictionary<Tile, TileFlags> flags = new()
        {
            { Tile.Chasm,           TileFlags.Avoid | TileFlags.Pit },
            { Tile.Empty,           TileFlags.Passable },
            { Tile.EmptyWell,       TileFlags.Passable },
            { Tile.EmptySp,         TileFlags.Passable },
            { Tile.EmptyDecor,      TileFlags.Passable },
            { Tile.Entrance,        TileFlags.Passable },
            { Tile.InactiveTrap,    TileFlags.Passable },
            { Tile.Embers,          TileFlags.Passable },
            { Tile.Exit,            TileFlags.Passable },
            { Tile.UnlockedExit,    TileFlags.Passable },
            { Tile.Pedestal,        TileFlags.Passable },
            { Tile.LockedExit,      TileFlags.Solid },
            { Tile.Statue,          TileFlags.Solid },
            { Tile.StatueSp,        TileFlags.Solid },
            { Tile.Alchemy,         TileFlags.Solid },
            { Tile.CrystalDoor,     TileFlags.Solid },
            { Tile.Well,            TileFlags.Avoid },
            { Tile.Trap,            TileFlags.Avoid },
            { Tile.Water,           TileFlags.Passable | TileFlags.Liquid },
            { Tile.OpenDoor,        TileFlags.Passable | TileFlags.Flammable },
            { Tile.Grass,           TileFlags.Passable | TileFlags.Flammable },
            { Tile.SecretTrap,      TileFlags.Passable | TileFlags.Secret },
            { Tile.WallDecor,       TileFlags.LOSBlocking | TileFlags.Solid},
            { Tile.Wall,            TileFlags.LOSBlocking | TileFlags.Solid},
            { Tile.LockedDoor,      TileFlags.LOSBlocking | TileFlags.Solid},
            { Tile.SecretDoor,      TileFlags.LOSBlocking | TileFlags.Solid | TileFlags.Secret },
            { Tile.Bookshelf,       TileFlags.LOSBlocking | TileFlags.Solid | TileFlags.Flammable  },
            { Tile.Barricade,       TileFlags.LOSBlocking | TileFlags.Solid | TileFlags.Flammable  },
            { Tile.Door,            TileFlags.LOSBlocking | TileFlags.Solid | TileFlags.Flammable | TileFlags.Passable },
            { Tile.HighGrass,       TileFlags.LOSBlocking | TileFlags.Flammable | TileFlags.Passable},
            { Tile.FurrowedGrass,   TileFlags.LOSBlocking | TileFlags.Flammable | TileFlags.Passable}
        };


    }

}
