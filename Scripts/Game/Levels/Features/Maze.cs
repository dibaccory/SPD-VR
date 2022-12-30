using System;
using UnityEngine;
using Levels.Rooms;
using Utils;
using TileInfo;

namespace Levels.Features
{
    public static class Maze
    {

        public static readonly bool EMPTY = false;
        public static readonly bool FILLED = true;
        private static int width, height;

        private static bool inBounds (int x, int y) => (x > 0 && x < width && y > 0 && y < height);
        private static bool isEdge (int x, int y) => (x == 0 || x == width - 1 || y == 0 || y == height - 1);
        private static bool isDirAdjToOtherFills (bool[,] maze, int x,int y, int dx, int dy) => (maze[x,y] || maze[x + dx,y + dy] || maze[x - dx,y - dy]);


        public static bool[,] Generate(Room r)
        {
            width = r.Width;
            height = r.Height;
            bool[,] maze = new bool[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (isEdge(x,y))
                    {
                        maze[x,y] = FILLED;
                    }
                }
            }

            //set spaces where there are doors
            foreach(Room.Door d in r.connected.Values)
            {
                maze[d.pos.x - r.left, d.pos.y - r.top] = EMPTY;
            }

            return Generate(maze);
        }

        public static bool[,] Generate(Rectangle r) => Generate(r.width + 1, r.height + 1);
        public static bool[,] Generate(int w, int h)
        {
            width = w;
            height = h;
            return Generate(new bool[w, h]);
        }

        public static bool[,] Generate(bool[,] maze)
        {
            int fails = 0;
            int x, y, moves;
            int[] mov;
            while (fails < 2500)
            {

                //find a RandomNumberGenerator wall point
                do
                {
                    x = RandomNumberGenerator.Int(width);
                    y = RandomNumberGenerator.Int(height);
                } while (!maze[x,y]);

                //decide on how we're going to move
                mov = DecideDirection(maze, x, y);
                if (mov == null)
                {
                    fails++;
                }
                else
                {
                    fails = 0;
                    moves = 0;
                    do
                    {
                        x += mov[0];
                        y += mov[1];
                        maze[x,y] = FILLED;
                        moves++;
                    } while (RandomNumberGenerator.Int(moves) == 0 && CheckValidMove(maze, x, y, mov));

                }

            }

            return maze;
        }


        public static bool[,] Generate(Room r, Tile[] terrain, int width, Tile filledTerrainType)
        {
            bool[,] maze = new bool[r.Width,r.Height];
            //Fill edges of Maze Room
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (terrain[x + r.left + (y + r.top) * width] == filledTerrainType)
                    {
                        maze[x,y] = FILLED;
                    }
                }
            }

            return Generate(maze);
        }

        //This is to build the maze WALLS
        private static int[] DecideDirection(bool[,] maze, int x, int y)
        {

            //attempts to move up
            if (RandomNumberGenerator.Int(4) == 0 && //1 in 4 chance
                    CheckValidMove(maze, x, y, new int[] { 0, -1 }))
            {
                return new int[] { 0, -1 };
            }

            //attempts to move right
            if (RandomNumberGenerator.Int(3) == 0 && //1 in 3 chance
                    CheckValidMove(maze, x, y, new int[] { 1, 0 }))
            {
                return new int[] { 1, 0 };
            }

            //attempts to move down
            if (RandomNumberGenerator.Int(2) == 0 && //1 in 2 chance
                    CheckValidMove(maze, x, y, new int[] { 0, 1 }))
            {
                return new int[] { 0, 1 };
            }

            //attempts to move left
            if (
                    CheckValidMove(maze, x, y, new int[] { -1, 0 }))
            {
                return new int[] { -1, 0 };
            }

            return null;
        }

        public static bool allowDiagonals = false;

        //We want walls to branch out randomly
        private static bool CheckValidMove(bool[,] maze, int x, int y, int[] mov)
        {
            int sideX = 1 - Math.Abs(mov[0]);
            int sideY = 1 - Math.Abs(mov[1]);

            x += mov[0];
            y += mov[1];

            //Check if new wall is one tile thick
            if(!inBounds(x,y)) return false;
            else if(isDirAdjToOtherFills(maze,x,y, sideX, sideY))
            {
                return false;
            }

            x += mov[0];
            y += mov[1];

            if (!inBounds(x,y)) return false;
            else if(!allowDiagonals && isDirAdjToOtherFills(maze, x,y, sideX, sideY))
            {
                return false;
            }

            return true;
        }
    }

}
