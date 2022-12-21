using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day18 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var cubes = new HashSet<(int X, int Y, int Z)>();
            foreach (var cube in input.Split("\r\n"))
            {
                var cubeCoords = cube.Split(',').Select(x => int.Parse(x)).ToList();
                cubes.Add((X: cubeCoords[0], Y: cubeCoords[1], Z: cubeCoords[2]));
            }

            foreach (var cube in cubes)
            {

            }
        }
    }
}
