using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    enum CaveTileTypes
    {
        Air,
        Rock,
        Sand,
        None
    }

    enum MoveState
    {
        OutOfBounds,
        Settled,
        Moving
    }

    class MoveResult
    {
        public MoveState MoveState { get; set; }
        public Point Position { get; set; }
    }

    class Cave
    {
        public CaveTileTypes[] Tiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Cave(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = Enumerable.Repeat(CaveTileTypes.Air, Width * Height).ToArray();
        }

        public MoveResult? TryMove(Point sandCoord, Point targetCoord)
        {
            switch (GetTileType(targetCoord))
            {
                case CaveTileTypes.Air:
                    Tiles[CoordToIndex(sandCoord)] = CaveTileTypes.Air;
                    Tiles[CoordToIndex(targetCoord)] = CaveTileTypes.Sand;
                    return new MoveResult() { MoveState = MoveState.Moving, Position = targetCoord };
                case CaveTileTypes.None:
                    Tiles[CoordToIndex(sandCoord)] = CaveTileTypes.Air;
                    return new MoveResult() { MoveState = MoveState.OutOfBounds };
            }

            return null;
        }

        public MoveResult MoveSand(Point sandCoord)
        {
            return (TryMove(sandCoord, sandCoord + new Point(0, 1)) 
                ?? TryMove(sandCoord, sandCoord + new Point(-1, 1))
                ?? TryMove(sandCoord, sandCoord + new Point(1, 1))
                ?? new MoveResult() { MoveState = MoveState.Settled });
        }

        public CaveTileTypes GetTileType(Point coord)
        {
            if (coord.X >= 0 && coord.X < Width && coord.Y >= 0 && coord.Y < Height)
            {
                return Tiles[CoordToIndex(coord)];
            }

            return CaveTileTypes.None;
        }

        public void AddRocks(Point start, Point end)
        {
            if (start.X == end.X)
            {
                var target = new Point(start.X, start.Y); 
                var direction = start.Y < end.Y ? 1 : -1;
                while (target.Y != end.Y)
                {
                    Tiles[CoordToIndex(target)] = CaveTileTypes.Rock;
                    target = target + new Point(0, direction);
                }
                Tiles[CoordToIndex(target)] = CaveTileTypes.Rock;
            }
            else if (start.Y == end.Y)
            {
                var target = new Point(start.X, start.Y);
                var direction = start.X < end.X ? 1 : -1;
                while (target.X != end.X)
                {
                    Tiles[CoordToIndex(target)] = CaveTileTypes.Rock;
                    target = target + new Point(direction, 0);
                }
                Tiles[CoordToIndex(target)] = CaveTileTypes.Rock;
            }
            else
            {
                throw new Exception("Rocks are not in a straight line");
            }
        }

        public bool AddSand(Point addPoint)
        {
            if (Tiles[CoordToIndex(addPoint)] == CaveTileTypes.Sand) { return false; }

            Tiles[CoordToIndex(addPoint)] = CaveTileTypes.Sand;
            return true;
        }

        public Point IndexToCoord(int index)
        {
            return new Point(index % Width, index / Width);
        }

        public int CoordToIndex(Point coord)
        {
            return (coord.Y * Width) + coord.X;
        }
    }

    public class Day14 : IPuzzleSolution
    {
        Point ParseCoord(string stringCoord)
        {
            var values = stringCoord.Split(",");
            return new Point(int.Parse(values[0]), int.Parse(values[1]));
        }

        public void Run(string input)
        {
            int xMax = -1;
            int yMax = -1;
            foreach (var line in input.Split("\r\n"))
            {
                foreach (var coords in (line.Split(" -> ")))
                {
                    var coord = coords.Split(",").Select(x => int.Parse(x)).ToList();

                    if (coord[0] > xMax) { xMax = coord[0]; }
                    if (coord[1] > yMax) { yMax = coord[1]; }
                }
            }

            var cave = new Cave(xMax + 500, yMax + 3);

            foreach (var line in input.Split("\r\n"))
            {
                var points = line.Split(" -> ").Select(point => ParseCoord(point)).ToList();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    cave.AddRocks(points[i], points[i + 1]);
                }
            }
            cave.AddRocks(new Point(0, cave.Height - 1), new Point(cave.Width - 1, cave.Height - 1));

            Point sandCoord;
            MoveResult moveResult;

            var continueDropping = true;
            while (continueDropping)
            {
                sandCoord = new Point(500, 0);
                continueDropping = cave.AddSand(sandCoord);
                do
                {
                    moveResult = cave.MoveSand(sandCoord);
                    switch (moveResult.MoveState)
                    {
                        case MoveState.Moving:
                            sandCoord = moveResult.Position;
                            continue;
                        case MoveState.OutOfBounds:
                            continueDropping = false;
                            continue;

                    }
                } 
                while (moveResult.MoveState == MoveState.Moving);
            }

            var sandCount = cave.Tiles.Aggregate(0, (acc, tile) =>
            {
                if (tile == CaveTileTypes.Sand) { acc++; }
                return acc;
            });

            Console.WriteLine($"{sandCount} pieces of sand in the cave");
        }
    }
}
