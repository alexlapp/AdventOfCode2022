using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day10 : IPuzzleSolution
    {
        public void Run(string input)
        {
            int register = 1;
            int cycle = 1;

            int signalStrengthSum = 0;
            int[] signalSamples = { 20, 60, 100, 140, 180, 220 };

            foreach (var line in input.Split("\r\n"))
            {
                bool increment = false;
                foreach(var word in line.Split(" "))
                {
                    // CRT Draw Code
                    if (Math.Abs(((cycle-1) % 40) - register) <= 1) { Console.Write("#"); }
                    else { Console.Write("."); }

                    if (signalSamples.Contains(cycle)) { signalStrengthSum += cycle * register; }
                    cycle++;
                    if (increment) { register += int.Parse(word); }
                    increment = true;

                    if ((cycle -1 ) % 40 == 0) { Console.WriteLine(); } // Move CRT to next line
                }
            }

            Console.WriteLine();
            Console.WriteLine($"The sum of signal strengths is: {signalStrengthSum}");
        }
    }
}
