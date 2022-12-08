using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day8 : IPuzzleSolution
    {
        public void Run(string input)
        {
            int width = input.IndexOf("\r\n");
            int height = input.Count(c => c.Equals('\n')) + 1;
            var trees = input.Replace("\r\n", "").Select(c => int.Parse(c.ToString())).ToArray();
            var visible = Enumerable.Repeat(false, trees.Length).ToArray();

            for (int x  = 0; x < width; x++)
            {
                bool scanDown = true, scanUp = true;
                int heightDown = -1, heightUp = -1;
                for (int scan = 0; scan < height; scan++)
                {
                    if (!scanDown && !scanUp) break;

                    if (scanDown)
                    {
                        int i = x + (scan * width);
                        if (heightDown < trees[i])
                        {
                            heightDown = trees[i];
                            visible[i] = true;
                        }
                    }

                    if (scanUp)
                    {
                        int i = ((height - 1 - scan) * width) + x;
                        if (heightUp < trees[i])
                        {
                            heightUp = trees[i];
                            visible[i] = true;
                        }
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                bool scanLeft = true, scanRight = true;
                int heightLeft = -1, heightRight = -1;
                for (int scan = 0; scan < width; scan++)
                {
                    if (!scanLeft && !scanRight) break;

                    if (scanLeft)
                    {
                        int i = (y * width) + scan;
                        if (heightLeft < trees[i])
                        {
                            heightLeft = trees[i];
                            visible[i] = true;
                        }
                    }

                    if (scanRight)
                    {
                        int i = (y * width) + (width - 1) - scan;
                        if (heightRight < trees[i])
                        {
                            heightRight = trees[i];
                            visible[i] = true;
                        }
                    }
                }
            }

            int visibleCount = visible.Where(v => v).Count();
            Console.WriteLine($"Visible trees: {visibleCount}");

            var scenicScores = new int[trees.Length];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int treeHeight = trees[x + (y * width)];
                    int up = 0, down = 0, left = 0, right = 0;


                    for (int xs = x - 1; xs >= 0; xs--)
                    {
                        if (trees[xs + (y * width)] < treeHeight)
                        {
                            left++;
                        }
                        else
                        {
                            left++;
                            break;
                        }
                    }

                    for (int xs = x + 1; xs < width; xs++)
                    {
                        if (trees[xs + (y * width)] < treeHeight)
                        {
                            right++;
                        }
                        else
                        {
                            right++;
                            break;
                        }
                    }

                    for (int ys = y - 1; ys >= 0; ys--)
                    {
                        if (trees[x + (ys * width)] < treeHeight)
                        {
                            up++;
                        }
                        else
                        {
                            up++;
                            break;
                        }
                    }

                    for (int ys = y + 1; ys < height; ys++)
                    {
                        if (trees[x + (ys * width)] < treeHeight)
                        {
                            down++;
                        }
                        else
                        {
                            down++;
                            break;
                        }
                    }

                    scenicScores[x + (y * width)] = up * down * left * right;
                }
            }

            int score = scenicScores.Aggregate(-1, (max, score) => Math.Max(max, score));
            Console.WriteLine($"Highest Scenic Score: {score}");
        }
    }
}
