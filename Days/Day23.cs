using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day23 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var elves = new Dictionary<int, Point>();
            var lines = input.Split("\r\n");
            var elfId = 0;
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        elves.Add(elfId, new Point(x, y));
                        elfId++;
                    }
                }
            }

            var moveAndCheckOffsets = new Queue<List<Point>>();
            var northOffsets = new List<Point>()
            {
                new Point(0, -1),
                new Point(-1, -1),
                new Point(1, -1),
            };
            var southOffsets = new List<Point>()
            {
                new Point(0, 1),
                new Point(-1, 1),
                new Point(1, 1),
            };
            var westOffsets = new List<Point>()
            {
                new Point(-1, 0),
                new Point(-1, -1),
                new Point(-1, 1),
            };
            var eastOffsets = new List<Point>()
            {
                new Point(1, 0),
                new Point(1, -1),
                new Point(1, 1),
            };
            moveAndCheckOffsets.Enqueue(northOffsets);
            moveAndCheckOffsets.Enqueue(southOffsets);
            moveAndCheckOffsets.Enqueue(westOffsets);
            moveAndCheckOffsets.Enqueue(eastOffsets);

            var surroundingOffsets = new List<Point>();
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    surroundingOffsets.Add(new Point(x, y));
                }
            }

            var currentRound = 0;
            var checkInRound = 10;
            var stopMoving = false;
            while (!stopMoving)
            {
                currentRound++;
                stopMoving = true;

                // SETUP ROUND VARIABLES
                var currentElfLocations = elves.Select(kvp => kvp.Value).ToList();
                var moveTargets = new List<(int elfId, Point targetPoint)>();
                var offLimitsTargets = new List<Point>();

                // FIRST HALF - DETERMINE MOVE TARGETS
                foreach (var subject in elves)
                {
                    var isElfNear = false;
                    foreach (var surroundingOffset in surroundingOffsets)
                    {
                        if (currentElfLocations.Exists(p => p == (subject.Value + surroundingOffset)))
                        {
                            isElfNear = true;
                        }
                    }

                    if (!isElfNear)
                        continue;

                    // If any elf needs to move, we cannot stop moving
                    stopMoving = false;

                    foreach (var offsets in moveAndCheckOffsets)
                    {
                        var moveTarget = subject.Value + offsets.First();

                        var moveThisWay = true;
                        foreach (var offset in offsets)
                        {
                            if (currentElfLocations.Exists(p => p == subject.Value + offset))
                            {
                                moveThisWay = false;
                                break;
                            }
                        }
                        
                        if (moveThisWay && !offLimitsTargets.Exists(p => p == moveTarget))
                        {
                            var existingIndex = moveTargets.FindIndex(mt => mt.targetPoint == moveTarget);
                            if (existingIndex >= 0)
                            {
                                moveTargets.RemoveAt(existingIndex);
                                offLimitsTargets.Add(moveTarget);
                            }
                            else
                            {
                                moveTargets.Add((elfId: subject.Key, targetPoint: moveTarget));
                            }
                            break;
                        }
                    }
                }

                // SECOND HALF - MOVE
                foreach(var movingElf in moveTargets)
                {
                    elves[movingElf.elfId] = movingElf.targetPoint;
                }

                if (currentRound == checkInRound)
                {
                    var minX = 0;
                    var minY = 0;
                    var maxX = 0;
                    var maxY = 0;
                    var elfCount = 0;
                    foreach (var elf in elves)
                    {
                        var elfPos = elf.Value;
                        if (elfPos.X < minX) minX = elfPos.X;
                        if (elfPos.Y < minY) minY = elfPos.Y;
                        if (elfPos.X > maxX) maxX = elfPos.X;
                        if (elfPos.Y > maxY) maxY = elfPos.Y;

                        elfCount++;
                    }

                    var elfWidth = maxX - minX + 1;
                    var elfHeight = maxY - minY + 1;
                    var elfArea = elfWidth * elfHeight;
                    var emptySquares = elfArea - elfCount;
                    Console.WriteLine($"The number of empty ground tiles is {emptySquares}");
                }

                moveAndCheckOffsets.Enqueue(moveAndCheckOffsets.Dequeue());
            }

            Console.WriteLine($"The number of rounds ran was: {currentRound}");
        }

        void printElves(Dictionary<int, Point> elves)
        {
            var elfPositions = elves.Select(kvp => kvp.Value).ToList();

            var minX = 0;
            var minY = 0;
            var maxX = 0;
            var maxY = 0;
            foreach (var elfPos in elfPositions)
            {
                if (elfPos.X < minX) minX = elfPos.X;
                if (elfPos.Y < minY) minY = elfPos.Y;
                if (elfPos.X > maxX) maxX = elfPos.X;
                if (elfPos.Y > maxY) maxY = elfPos.Y;
            }
            
            for (int y = minY; y < maxY + 1; y++)
            {
                for (int x = minX; x < maxX + 1; x++)
                {
                    if (elfPositions.Exists(p => p == new Point(x, y)))
                        Console.Write('#');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
