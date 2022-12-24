using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class VisitCoord
    {
        public Point Point { get; set; }
        public int Distance { get; set; }

        public VisitCoord(Point point, int distance)
        {
            Point = point;
            Distance = distance;
        }
    }

    public class Day12 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var width = input.IndexOf('\r');
            var height = input.Count(c => c.Equals('\n')) + 1;
            var rawSquares = input.Replace("\r\n", "");
            var squares = rawSquares.ToList();
            var map = new Grid<char>(width, height, squares);

            // Part 1

            var startingCoord = map.PointFromIndex(rawSquares.IndexOf('S'));
            var targetCoord = map.PointFromIndex(rawSquares.IndexOf('E'));

            Console.WriteLine($"Distance from start to end: {ShortestPathLength(map, startingCoord, targetCoord)}");

            // Part 2

            var possibleStarts = new List<Point>();
            for (int i = 0; i < rawSquares.Length; i++)
            {
                if (rawSquares[i] == 'a') { possibleStarts.Add(map.PointFromIndex(i)); }
            }

            var shortestDistance = possibleStarts
                .Select(start => ShortestPathLength(map, start, targetCoord))
                .OrderBy(length => length)
                .Where(length => length > 0) // -1 means it can't be reached
                .First();

            Console.WriteLine($"Shortest starting path from any 'a': {shortestDistance}");
        }

        private int ShortestPathLength(Grid<char> map, Point start, Point target)
        {
            var visitedNodes = new HashSet<int>();
            var nodesToVisit = new Queue<VisitCoord>();
            nodesToVisit.Enqueue(new VisitCoord(start, 0));

            while (nodesToVisit.Any())
            {
                var node = nodesToVisit.Dequeue();

                if (visitedNodes.Contains(map.IndexFromPoint(node.Point))) { continue; }
                visitedNodes.Add(map.IndexFromPoint(node.Point));

                if (node.Point.Equals(target))
                {
                    return node.Distance;
                }

                var above = map.Above(node.Point);
                var below = map.Below(node.Point);
                var left = map.Left(node.Point);
                var right = map.Right(node.Point);

                if (above != null && CanTraverse(map.GetSquare(node.Point), map.GetSquare(above)))
                { nodesToVisit.Enqueue(new VisitCoord(above, node.Distance + 1)); }

                if (below != null && CanTraverse(map.GetSquare(node.Point), map.GetSquare(below)))
                { nodesToVisit.Enqueue(new VisitCoord(below, node.Distance + 1)); }

                if (left != null && CanTraverse(map.GetSquare(node.Point), map.GetSquare(left)))
                { nodesToVisit.Enqueue(new VisitCoord(left, node.Distance + 1)); }

                if (right != null && CanTraverse(map.GetSquare(node.Point), map.GetSquare(right)))
                { nodesToVisit.Enqueue(new VisitCoord(right, node.Distance + 1)); }
            }

            return -1;
        }

        private bool CanTraverse(char start, char end)
        {
            var realStart = start.Equals('S') ? 'a' : start;
            var realEnd = end.Equals('E') ? 'z' : end;
            return (int)realEnd - (int)realStart <= 1;
        }
    }
}
