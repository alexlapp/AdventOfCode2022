using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day18 : IPuzzleSolution
    {
        private (int, int, int) bounds = (0, 0, 0);
        private HashSet<(int, int, int)> cubes = new HashSet<(int, int, int)>();
        private Dictionary<(int, int, int), bool> innerMemo = new Dictionary<(int, int, int), bool>();

        public void Run(string input)
        {
            foreach (var cube in input.Split("\r\n"))
            {
                var cubeCoords = cube.Split(',').Select(x => int.Parse(x)).ToList();
                cubes.Add((cubeCoords[0], cubeCoords[1], cubeCoords[2]));

                if (bounds.Item1 < cubeCoords[0])
                    bounds.Item1 = cubeCoords[0];
                if (bounds.Item2 < cubeCoords[1])
                    bounds.Item2 = cubeCoords[1];
                if (bounds.Item3 < cubeCoords[2])
                    bounds.Item3 = cubeCoords[2];
            }

            var surfaceArea = 0;
            foreach ((var x, var y, var z) in cubes)
            {
                
                foreach (int offset in new int[] { -1, 1 })
                {
                    if (IsEmptyAndExterior((x + offset, y, z)))
                        surfaceArea++;

                    if (IsEmptyAndExterior((x, y + offset, z)))
                        surfaceArea++;

                    if (IsEmptyAndExterior((x, y, z + offset)))
                        surfaceArea++;
                }
            }

            Console.WriteLine($"Total exposed surface area: {surfaceArea}");
        }

        private bool IsEmptyAndExterior((int, int, int) testCoords)
        {
            if (cubes.Contains(testCoords))
                return false;

            if (IsInnerMemo(testCoords))
                return false;

            return true;
        }

        private bool IsInnerMemo((int, int, int) testCoords)
        {
            if (innerMemo.ContainsKey(testCoords))
                return innerMemo[testCoords];

            innerMemo.Add(testCoords, IsInner(testCoords));
            return innerMemo[testCoords];
        }

        private bool IsInner((int, int, int) testCoords)
        {
            for (int x = testCoords.Item1; x >= 0; x--)
            {
                if (cubes.Contains((x, testCoords.Item2, testCoords.Item3)))
                    break;

                if (x == 0)
                    return false;
            }
            for (int x = testCoords.Item1; x <= bounds.Item1; x++)
            {
                if (cubes.Contains((x, testCoords.Item2, testCoords.Item3)))
                    break;

                if (x == bounds.Item1)
                    return false;
            }

            for (int y = testCoords.Item2; y >= 0; y--)
            {
                if (cubes.Contains((testCoords.Item1, y, testCoords.Item3)))
                    break;

                if (y == 0)
                    return false;
            }
            for (int y = testCoords.Item2; y <= bounds.Item2; y++)
            {
                if (cubes.Contains((testCoords.Item1, y, testCoords.Item3)))
                    break;

                if (y == bounds.Item2)
                    return false;
            }

            for (int z = testCoords.Item3; z >= 0; z--)
            {
                if (cubes.Contains((testCoords.Item1, testCoords.Item2, z)))
                    break;

                if (z == 0)
                    return false;
            }
            for (int z = testCoords.Item3; z <= bounds.Item3; z++)
            {
                if (cubes.Contains((testCoords.Item1, testCoords.Item2, z)))
                    break;

                if (z == bounds.Item3)
                    return false;
            }

            return true;
        }
    }
}
