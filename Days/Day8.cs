using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day8 : IPuzzleSolution
    {
        private int CoordToInt(int x, int y, int width)
        {
            return (y * width) + x;
        }

        public void Run(string input)
        {
            int width = input.IndexOf("\r\n");
            int height = input.Count(c => c.Equals('\n')) + 1;
            var trees = input.Replace("\r\n", "").Select(c => int.Parse(c.ToString())).ToArray();

            int visibleCount = 0;
            int maxScenicScore = 0;

            var timer = new Stopwatch();
            timer.Start();


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tree = trees[(y * width) + x];

                    var (left, visibleLeft) = CalcLeft(trees, x, y, width, tree);
                    var (right, visibleRight) = CalcRight(trees, x, y, width, tree);
                    var (up, visibleUp) = CalcUp(trees, x, y, width, tree);
                    var (down, visibleDown) = CalcDown(trees, x, y, width, height, tree);

                    if (visibleLeft || visibleRight || visibleUp || visibleDown)
                    {
                        visibleCount++;
                    }

                    maxScenicScore = Math.Max(maxScenicScore, left * right * up * down);
                }
            }

            Console.WriteLine($"Visible trees: {visibleCount}");
            Console.WriteLine($"Highest Scenic Score: {maxScenicScore}");
        }

        private (int, bool) CalcLeft(int[] trees, int x, int y, int gridWidth, int subjectTree)
        {
            int visibleCount = 0;
            bool visible = true;
            for (int xs = x - 1; xs >= 0; xs--)
            {
                var tree = trees[CoordToInt(xs, y, gridWidth)];
                if (tree <= subjectTree)
                {
                    visibleCount++;
                }
                if (tree >= subjectTree)
                {
                    visible = false;
                }

                if (!visible && tree == subjectTree) break;
            }

            return (visibleCount, visible);
        }

        private (int, bool) CalcRight(int[] trees, int x, int y, int gridWidth, int subjectTree)
        {
            int visibleCount = 0;
            bool visible = true;
            for (int xs = x + 1; xs < gridWidth; xs++)
            {
                var tree = trees[CoordToInt(xs, y, gridWidth)];
                if (tree <= subjectTree)
                {
                    visibleCount++;
                }
                if (tree >= subjectTree)
                {
                    visible = false;
                }

                if (!visible && tree == subjectTree) break;
            }

            return (visibleCount, visible);
        }
        private (int, bool) CalcUp(int[] trees, int x, int y, int gridWidth, int subjectTree)
        {
            int visibleCount = 0;
            bool visible = true;
            for (int ys = y - 1; ys >= 0; ys--)
            {
                var tree = trees[CoordToInt(x, ys, gridWidth)];
                if (tree <= subjectTree)
                {
                    visibleCount++;
                }
                if (tree >= subjectTree)
                {
                    visible = false;
                }

                if (!visible && tree == subjectTree) break;
            }

            return (visibleCount, visible);
        }

        private (int, bool) CalcDown(int[] trees, int x, int y, int gridWidth, int gridHeight, int subjectTree)
        {
            int visibleCount = 0;
            bool visible = true;
            for (int ys = y + 1; ys < gridHeight; ys++)
            {
                var tree = trees[CoordToInt(x, ys, gridWidth)];
                if (tree <= subjectTree)
                {
                    visibleCount++;
                }
                if (tree >= subjectTree)
                {
                    visible = false;
                }

                if (!visible && tree == subjectTree) break;
            }

            return (visibleCount, visible);
        }
    }
}
