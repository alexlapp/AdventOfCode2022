using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public static class Extensions
    {
        public static void Move<T>(this List<T> subject, int subjectIndex, int targetIndex)
        {
            var value = subject[subjectIndex];
            subject.RemoveAt(subjectIndex);
            subject.Insert(targetIndex, value);
        }
    }

    internal class Day20 : IPuzzleSolution
    {
        private void Mix(List<(int startIndex, long value)> nums)
        {
            for (int i = 0; i < nums.Count; i++)
            {
                var currIndex = nums.FindIndex(n => n.startIndex == i);
                var value = nums[currIndex].value;

                var offsetIndex = currIndex + value;
                if (offsetIndex < 0)
                    offsetIndex += (Math.Abs(offsetIndex / (nums.Count - 1)) + 1) * (nums.Count - 1);

                var targetIndex = (int)(offsetIndex % (nums.Count - 1));

                nums.Move(currIndex, targetIndex);
            }
        }

        public void Run(string input)
        {
            var nums = input
                .Split("\r\n")
                .Select((n, i) => (startIndex: i, value: long.Parse(n)))
                .ToList();

            Mix(nums);

            var zeroIndex = nums.FindIndex(n => n.value == 0);
            var sumOfGroveCoordinates = (new int[] { 1000, 2000, 3000 })
                .Aggregate((long)0, (acc, offset) => acc + nums[(zeroIndex + offset) % nums.Count].value);

            Console.WriteLine($"The sum of coordinates is: {sumOfGroveCoordinates}");

            long decryptionKey = 811589153;
            nums = input
                .Split("\r\n")
                .Select((n, i) => (startIndex: i, value: long.Parse(n) * decryptionKey))
                .ToList();

            for (int i = 0; i < 10; i++)
            {
                Mix(nums);
            }

            zeroIndex = nums.FindIndex(n => n.value == 0);
            sumOfGroveCoordinates = (new int[] { 1000, 2000, 3000 })
                .Aggregate((long)0, (acc, offset) => acc + nums[(zeroIndex + offset) % nums.Count].value);

            Console.WriteLine($"The sum of decrypted coordinates is: {sumOfGroveCoordinates}");
        }
    }
}
