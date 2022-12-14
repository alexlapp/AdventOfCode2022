using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(Coord b)
        {
            return X == b.X && Y == b.Y;
        }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.X + b.X, a.Y + b.Y);
        }
    }

    class Map
    {
        public char[] Squares { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Map(char[] squares, int width, int height)
        {
            Squares = squares;
            Width = width;
            Height = height;
        }

        public char GetSquare(int index)
        {
            return Squares[index];
        }

        public char GetSquare(Coord coord)
        {
            return Squares[CoordToIndex(coord)];
        }

        public Coord IndexToCoord(int index)
        {
            return new Coord(index % Width, index / Width);
        }

        public int CoordToIndex(Coord coord)
        {
            return (coord.Y * Width) + coord.X;
        }

        public Coord? Above(Coord start)
        {
            if (start.Y == 0) return null;

            return new Coord(start.X, start.Y - 1 );
        }

        public Coord? Below(Coord start)
        {
            if (start.Y == Height - 1) return null;

            return new Coord(start.X, start.Y + 1);
        }

        public Coord? Left(Coord start)
        {
            if (start.X == 0) return null;

            return new Coord(start.X - 1, start.Y);
        }

        public Coord? Right(Coord start)
        {
            if (start.X == Width - 1) return null;

            return new Coord(start.X + 1, start.Y);
        }
    }

    class VisitCoord
    {
        public Coord Coord { get; set; }
        public int Distance { get; set; }

        public VisitCoord(Coord coord, int distance)
        {
            Coord = coord;
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
            var squares = rawSquares.ToCharArray();
            var map = new Map(squares, width, height);

            // Part 1

            var startingCoord = map.IndexToCoord(rawSquares.IndexOf('S'));
            var targetCoord = map.IndexToCoord(rawSquares.IndexOf('E'));

            Console.WriteLine($"Distance from start to end: {ShortestPathLength(map, startingCoord, targetCoord)}");

            // Part 2

            var possibleStarts = new List<Coord>();
            for (int i = 0; i < rawSquares.Length; i++)
            {
                if (rawSquares[i] == 'a') { possibleStarts.Add(map.IndexToCoord(i)); }
            }

            var shortestDistance = possibleStarts
                .Select(start => ShortestPathLength(map, start, targetCoord))
                .OrderBy(length => length)
                .Where(length => length > 0) // -1 means it can't be reached
                .First();

            Console.WriteLine($"Shortest starting path from any 'a': {shortestDistance}");
        }

        private int ShortestPathLength(Map map, Coord start, Coord target)
        {
            var visitedNodes = new HashSet<int>();
            var nodesToVisit = new Queue<VisitCoord>();
            nodesToVisit.Enqueue(new VisitCoord(start, 0));

            while (nodesToVisit.Any())
            {
                var node = nodesToVisit.Dequeue();

                if (visitedNodes.Contains(map.CoordToIndex(node.Coord))) { continue; }
                visitedNodes.Add(map.CoordToIndex(node.Coord));

                if (node.Coord.Equals(target))
                {
                    return node.Distance;
                }

                var above = map.Above(node.Coord);
                var below = map.Below(node.Coord);
                var left = map.Left(node.Coord);
                var right = map.Right(node.Coord);

                if (above != null && CanTraverse(map.GetSquare(node.Coord), map.GetSquare(above)))
                { nodesToVisit.Enqueue(new VisitCoord(above, node.Distance + 1)); }

                if (below != null && CanTraverse(map.GetSquare(node.Coord), map.GetSquare(below)))
                { nodesToVisit.Enqueue(new VisitCoord(below, node.Distance + 1)); }

                if (left != null && CanTraverse(map.GetSquare(node.Coord), map.GetSquare(left)))
                { nodesToVisit.Enqueue(new VisitCoord(left, node.Distance + 1)); }

                if (right != null && CanTraverse(map.GetSquare(node.Coord), map.GetSquare(right)))
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
