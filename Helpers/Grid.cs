using Advent_Of_Code.Days;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Helpers.Maps
{
    public class Point : IComparable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b)
            => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, int m)
            => new Point(a.X * m, a.Y * m);

        public static bool operator >(Point a, Point b)
            => a.X > b.X && a.Y > b.Y;
        public static bool operator <(Point a, Point b)
            => a.X < b.X && a.Y < b.Y;
        public static bool operator >=(Point a, Point b)
            => a.X >= b.X && a.Y >= b.Y;
        public static bool operator <=(Point a, Point b)
            => a.X <= b.X && a.Y <= b.Y;

        public override string ToString() { return $"({X}, {Y})"; }

        public int CompareTo(Point? other)
        {
            if (other == null) return 1;
            if (X < other.X || Y < other.Y) return -1;
            if (X > other.X || Y > other.Y) return 1;

            return 0;
        }
    }

    public class Grid<T>
    {
        private int _width;
        private int _height;
        private List<T> _squares;

        public int Width => _width;
        public int Height => _height;
        public List<T> Squares => _squares;

        public Grid(int width, int height, List<T> squares)
        {
            _width = width;
            _height = height;
            _squares = squares;
        }

        public Grid(int width, int height, T seed)
        {
            _width = width;
            _height = height;
            _squares = Enumerable.Repeat(seed, width * height).ToList();
        }

        public T GetSquare(int index)
        {
            return Squares[index];
        }

        public T GetSquare(Point p)
        {
            return GetSquare(IndexFromPoint(p));
        }

        public bool TryGetSquare(int index, out T? square)
        {
            if (index < 0 || index >= _squares.Count()) { square = default; return false; }

            square = GetSquare(index);
            return true;
        }

        public bool TryGetSquare(Point p, out T? square)
        {
            if (p.X < 0 || p.X >= _width || p.Y < 0 || p.Y >= _height) { square = default; return false; }

            return TryGetSquare(IndexFromPoint(p), out square);
        }

        public Point PointFromIndex(int index)
        {
            return new Point(index % _width, index / _height);
        }

        public int IndexFromPoint(Point p)
        {
            return (p.Y * _width) + p.X;
        }
        public Point? Above(Point start)
        {
            if (start.Y == 0) return null;

            return new Point(start.X, start.Y - 1);
        }

        public Point? Below(Point start)
        {
            if (start.Y == Height - 1) return null;

            return new Point(start.X, start.Y + 1);
        }

        public Point? Left(Point start)
        {
            if (start.X == 0) return null;

            return new Point(start.X - 1, start.Y);
        }

        public Point? Right(Point start)
        {
            if (start.X == Width - 1) return null;

            return new Point(start.X + 1, start.Y);
        }
    }
}
