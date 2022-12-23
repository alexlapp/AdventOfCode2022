using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public static class Extensions
    {
        public static List<T> Move<T>(this List<T> subject, int subjectIndex, int targetIndex)
        {
            return subject;
        }
    }

    internal class Day20 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var nums = input.Split("\r\n").Select((n, i) => (startIndex: i, value: int.Parse(n))).ToList();

            for (int i = 0; i < nums.Count; i++)
            {
                var currIndex = nums.FindIndex(n => n.startIndex == i);
                (var startIndex, var value) = nums[currIndex];

                if (value < 0)
                {
                    
                }
                else if (value > 0)
                {

                }
            }

            var zeroIndex = nums.FindIndex(n => n.value == 0);
            var sumOfGroveCoordinates = (new int[] { 1000, 2000, 3000 })
                .Aggregate(0, (acc, offset) => acc + nums[(zeroIndex + offset) % nums.Count].value);

            Console.WriteLine($"The sum of coordinates is: {sumOfGroveCoordinates}");
        }
    }
}
