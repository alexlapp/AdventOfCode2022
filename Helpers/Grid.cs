using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Helpers.Maps
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b)
            => new Point(a.X - b.X, a.Y - b.Y);
        public override string ToString() { return $"({X}, {Y})"; }
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

        public Point PointFromIndex(int index)
        {
            return new Point(index % _width, index / _height);
        }

        public int IndexFromPoint(Point p)
        {
            return (p.Y * _width) + p.X;
        }
    }
}
