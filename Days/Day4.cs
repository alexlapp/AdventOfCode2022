using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Range
    {
        int Start { get; set; }
        int End { get; set; }

        public bool Contains(Range other)
        {
            return Start <= other.Start && End >= other.End;
        }

        public bool Overlaps(Range other)
        {
            return (Start >= other.Start && Start <= other.End)
                || (End >= other.Start && End <= other.End);
        }

        public static Range Parse(string toParse)
        {
            var values = toParse.Split("-");

            return new Range()
            {
                Start = int.Parse(values[0]),
                End = int.Parse(values[1])
            };
        }
    }

    public class Day4 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var sets = input.Split("\r\n");

            var containedCount = 0;
            var overlapCount = 0;
            foreach (var set in sets)
            {
                var ranges = set.Split(",");
                var rangeOne = Range.Parse(ranges[0]);
                var rangeTwo = Range.Parse(ranges[1]);

                if (rangeOne.Contains(rangeTwo) || rangeTwo.Contains(rangeOne))
                {
                    containedCount++;
                }

                if (rangeOne.Overlaps(rangeTwo) || rangeTwo.Overlaps(rangeOne))
                {
                    overlapCount++;
                }
            }

            Console.WriteLine($"Ranges contained by a pair: {containedCount}");
            Console.WriteLine($"Ranges overlapping with their pair: {overlapCount}");
        }
    }
}
