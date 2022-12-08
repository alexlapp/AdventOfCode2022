using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day6 : IPuzzleSolution
    {
        private int FindMarkerCharacter(string message, int markerLength)
        {
            var window = new Queue<char>();

            int lastIndex = -1;
            for (int i = 0; i < message.Length; i++)
            {
                window.Enqueue(message[i]);
                if (window.Count == markerLength)
                {
                    if ((new Queue<char>(window)).Distinct().ToList().Count == markerLength)
                    {
                        return i + 1;
                    }

                    window.Dequeue();
                }
            }

            return -1;
        }

        public void Run(string input)
        {
            Console.WriteLine($"Start of Packet marker appears after character: {FindMarkerCharacter(input, 4)}");

            Console.WriteLine($"Start of Message marker appears after character: {FindMarkerCharacter(input, 14)}");
        }
    }
}
