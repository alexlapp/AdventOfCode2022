using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class Point
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

    class Rock
    {

        public int Width => _width;
        public int Height => _shape.Length / _width;
        public int CharAtPoint(Point p) => _shape[p.X + (p.Y * _width)];


        private char[] _shape;
        private int _width;

        public Rock(string shapeString, int width)
        {
            _shape = shapeString.ToCharArray();
            _width = width;
        }
    }

    class FallingRock
    {
        public Rock Shape { get; set; }
        public Point LowerLeft { get; set; }
        public bool IsFalling { get; set; }

        public FallingRock(Rock shape, Point upperLeft)
        {
            Shape = shape;
            LowerLeft = upperLeft;
            IsFalling = true;
        }
    }

    class FallingRockCave
    {
        private int _width;
        private int _height => (Cave?.Count ?? 0) / _width;
        public List<char> Cave { get; set; }

        private long _heightOffset { get; set; }
        public long HeightOffset => _heightOffset;

        public FallingRockCave(int width = 7)
        {
            _width = width;
            Cave = Enumerable.Repeat('.', _width).ToList();
        }

        public int[] CurrentSkyline()
        {
            var result = Enumerable.Repeat(0, _width).ToArray();

            var peak = RockPeak();
            if (peak == null)
                return result;

            for (int x = 0; x < _width; x++)
            {
                var distanceFromPeak = 0;
                var startIndex = PointToIndex(new Point(x, peak.Y));
                for (int i = startIndex; i >= 0; i -= _width)
                {
                    if (Cave[i] == '#')
                    {
                        result[x] = distanceFromPeak;
                        break;
                    }
                    distanceFromPeak++;
                }
            }

            return result;
        }

        public void Print(FallingRock? rock = null)
        {
            List<Point> fallingRockPoints = new List<Point>();
            if (rock != null)
            {
                for (int rx = 0; rx < rock.Shape.Width; rx++)
                {
                    for (int ry = 0; ry < rock.Shape.Height; ry++)
                    {
                        var rockPoint = new Point(rx, ry);
                        var cavePoint = rock.LowerLeft + rockPoint;

                        if (rock.Shape.CharAtPoint(rockPoint).Equals('#'))
                            fallingRockPoints.Add(cavePoint);
                    }
                }
            }

            for (int y = _height - 1; y >= 0; y--)
            {
                Console.Write("|");
                for (int x = 0; x < _width; x++)
                {
                    var point = new Point(x, y);
                    if (fallingRockPoints.Where(p => p.X == point.X && p.Y == point.Y).Any())
                        Console.Write('@');
                    else
                        Console.Write(CaveAtPoint(point));
                }
                Console.Write("|\n");
            }
            Console.WriteLine("+" + String.Join("", Enumerable.Repeat('-', _width)) + "+");
            Console.Write("\n");
        }

        public void AddHeight(int rows = 1)
        {
            Cave.AddRange(Enumerable.Repeat('.', _width * rows));
        }

        public Point? RockPeak()
        {
            var index = Cave.LastIndexOf('#');
            if (index > 0)
                return IndexToPoint(index);
                
            return null;
        }

        public char CaveAtPoint(Point p)
        {
            var index = PointToIndex(p);

            // If null we are outside of bounds and since this is a cave that means we hit the cave wall
            return index < 0 ? '#' : Cave[index];
        }

        public FallingRock AddRock(Rock shape)
        {
            var firstOpenRow = 0;

            var rockPeak = RockPeak();
            if (rockPeak != null)
                firstOpenRow = rockPeak.Y + 1;

            var rockLowerY = firstOpenRow + 3;
            var rockUpperY = rockLowerY + (shape.Height - 1);

            while (rockUpperY >= _height)
            {
                AddHeight();
            }

            return new FallingRock(shape, new Point(2, rockLowerY));
        }

        public bool DoesRockIntersect(FallingRock rock)
        {
            for (int rx = 0; rx < rock.Shape.Width; rx++)
            {
                for (int ry = 0; ry < rock.Shape.Height; ry++)
                {
                    var rockPoint = new Point(rx, ry);
                    var cavePoint = rock.LowerLeft + rockPoint;

                    if (rock.Shape.CharAtPoint(rockPoint).Equals('#') && CaveAtPoint(cavePoint).Equals('#'))
                        return true;
                }
            }

            return false;
        }

        public void SettleFallingRock(FallingRock rock)
        {
            for (int rx = 0; rx < rock.Shape.Width; rx++)
            {
                for (int ry = 0; ry < rock.Shape.Height; ry++)
                {
                    var rockPoint = new Point(rx, ry);
                    var cavePoint = rock.LowerLeft + rockPoint;

                    if (rock.Shape.CharAtPoint(rockPoint).Equals('#'))
                        Cave[PointToIndex(cavePoint)] = '#';
                }
            }
        }

        public int PointToIndex(Point p)
        {
            if (p.X < 0 || p.X >= _width || p.Y < 0 || p.Y >= _height)
                return -1;

            return (p.X + (p.Y * _width));
        }

        public Point IndexToPoint(int index)
        {
            return new Point(index % _width, index / _width);
        }
    }

    class LoopingGenerator<T>
    {
        private T[] _options;
        private int _index;

        public int Index => _index;

        public LoopingGenerator(IEnumerable<T> options)
        {
            _options = options.ToArray();
            _index = 0;
        }

        public T Next()
        {
            var result = _options[_index];
            _index = (_index + 1) % _options.Length;

            return result;
        }
    }

    public class Day17 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var rockShapes = new List<Rock>()
            {
                new Rock("####", 4),
                new Rock(".#.###.#.", 3),
                new Rock("###..#..#", 3),
                new Rock("####", 1),
                new Rock("####", 2)
            };

            var windGen = new LoopingGenerator<char>(input);
            var rockGen = new LoopingGenerator<Rock>(rockShapes);

            var cave = new FallingRockCave();

            var seenStates = new Dictionary<(string Skyline, int RockIndex, int WindIndex), (long RockCount, long CaveHeight)>();
            long cyclePredictionHeight = 0;

            long numberOfRocksToDrop = 1000000000000;
            for (long i = 0; i < numberOfRocksToDrop; i++)
            {
                var fallingRock = cave.AddRock(rockGen.Next());

                while (fallingRock.IsFalling)
                {
                    // Moved by wind
                    var windDirection = (windGen.Next().Equals('<')) ? -1 : 1;
                    var windOffset = new Point(windDirection, 0);

                    fallingRock.LowerLeft += windOffset;
                    if (cave.DoesRockIntersect(fallingRock))
                    {
                        fallingRock.LowerLeft -= windOffset;
                    }

                    // Moved by gravity
                    var gravityOffset = new Point(0, -1);
                    fallingRock.LowerLeft += gravityOffset;
                    if (cave.DoesRockIntersect(fallingRock))
                    {
                        fallingRock.LowerLeft -= gravityOffset;
                        fallingRock.IsFalling = false;
                        cave.SettleFallingRock(fallingRock);
                    }
                }

                var skyline = string.Join("", cave.CurrentSkyline());
                var state = (Skyline: skyline, RockIndex: rockGen.Index, WindIndex: windGen.Index);
                if (seenStates.ContainsKey(state))
                {
                    (var countAtLastCycle, var heightAtLastCycle) = seenStates[state];
                    long rockCountInCycle = i - countAtLastCycle;
                    long heightGained = (cave.RockPeak().Y + 1) - heightAtLastCycle;

                    var cyclesToSimulate = (numberOfRocksToDrop - i) / rockCountInCycle;
                    i += rockCountInCycle * cyclesToSimulate;
                    cyclePredictionHeight = cyclesToSimulate * heightGained;

                    seenStates.Clear();
                }
                else
                {
                    seenStates.Add(state, (RockCount: i, CaveHeight: cave.RockPeak().Y + 1));
                }
            }

            var peakPoint = cave.RockPeak() ?? cave.IndexToPoint(cave.Cave.Count);
            Console.WriteLine($"Tower is {peakPoint.Y + 1 + cyclePredictionHeight} units tall");
        }
    }
}
