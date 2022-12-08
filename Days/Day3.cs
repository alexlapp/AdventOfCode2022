using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day3 : IPuzzleSolution
    {
        private int GetPriorityValue(char c)
        {
            if (c < 97)
            {
                return c - 38;
            }
            else
            {
                return c - 96;
            }
        }

        private int GetDuplicatedItemPriority(string a, string b)
        {
            foreach (char c in a)
            {
                if (b.Contains(c))
                {
                    return GetPriorityValue(c);
                }
            }

            return 0;
        }

        private char GetGroupBadge(string a, string b, string c)
        {
            foreach (char ac in a)
            {
                if (b.Contains(ac) && c.Contains(ac)) return ac;
            }

            return 'a';
        }

        public void Run(string input)
        {
            var rucksacks = input.Split("\r\n");

            int prioritySum = 0;
            foreach (var sack in rucksacks)
            {
                var first = sack.Substring(0, (sack.Length / 2));
                var second = sack.Substring((sack.Length / 2), (sack.Length / 2));

                prioritySum += GetDuplicatedItemPriority(first, second);
            }

            Console.WriteLine($"Priority Sum is {prioritySum}");

            prioritySum = 0;
            for (int i = 0; i < rucksacks.Length; i += 3)
            {
                prioritySum += GetPriorityValue(GetGroupBadge(rucksacks[i], rucksacks[i + 1], rucksacks[i + 2]));
            }

            Console.WriteLine($"Group Badge priority sum is {prioritySum}");
        }
    }
}
