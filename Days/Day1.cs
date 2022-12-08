using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day1 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var elves = input.Split("\r\n\r\n");

            int max1 = -1;
            int max2 = -1;
            int max3 = -1;
            foreach (var elf in elves)
            {
                int calories = elf.Split("\r\n").Aggregate<string, int>(0, (acc, next) =>
                {
                    acc += int.Parse(next);

                    return acc;
                });

                if (calories > max1)
                {
                    max3 = max2;
                    max2 = max1;
                    max1 = calories;
                }
                else if (calories > max2)
                {
                    max3 = max2;
                    max2 = calories;
                }
                else if (calories > max3)
                {
                    max3 = calories;
                }
            }

            Console.WriteLine($"Elf holding most calories is holding {max1} calories");
            Console.WriteLine($"Top 3 elves are carring a total of {max1 + max2 + max3} calories");
        }
    }
}
