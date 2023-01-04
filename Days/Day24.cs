using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day24 : IPuzzleSolution
    {

        private int _valleyLeftX;
        private int _valleyRightX;
        private int _valleyTopY;
        private int _valleyBottomY;

        public void Run(string input)
        {
            var valleyWidth = input.IndexOf("\r\n");
            var valleyHeight = input.Count(c => c.Equals('\n')) + 1;

            _valleyLeftX = 0;
            _valleyRightX = valleyWidth - 1;
            _valleyTopY = 0;
            _valleyBottomY = valleyHeight - 1;

            var start = new Point(input.IndexOf('.'), 0);
            var exit = new Point(input.Replace("\r\n", "").LastIndexOf('.') % valleyWidth, valleyHeight - 1);
            var blizzards = ParseBlizzards(input);

            int minutes = FindExitRecur(start, exit, blizzards);

            Console.WriteLine($"Number of minutes to reach exit: {minutes}");
            return;
        }

        void PrintBlizzards(int width, int height, List<Blizzard> blizzards)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == _valleyBottomY || y == _valleyTopY || x == _valleyLeftX || x == _valleyRightX)
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        var point = new Point(x, y);
                        var blizzardsAtPoint = blizzards.Where(b => b.Position == point).ToList();

                        if (!blizzardsAtPoint.Any())
                            Console.Write('.');
                        else if (blizzardsAtPoint.Count == 1)
                            Console.Write(blizzardsAtPoint.First().GetChar());
                        else
                            Console.Write(blizzardsAtPoint.Count);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private int FindExitRecur(Point startingPosition, Point target, List<Blizzard> blizzards)
        {
            var moveBizzardsAt = 0;

            var moveQueue = new Queue<(int minutes, Point pos)>();
            moveQueue.Enqueue((minutes: 0, pos: startingPosition));

            while (moveQueue.Count > 0)
            {
                (var minutes, var position) = moveQueue.Dequeue();

                if (position == target)
                    return minutes;

                if (minutes == moveBizzardsAt)
                {
                    moveBizzardsAt = minutes + 1;
                    MoveBlizzards(blizzards);
                }

                foreach (var move in PossibleNewPositions(position, target, blizzards))
                    moveQueue.Enqueue((minutes: minutes + 1, move));

                if (position == startingPosition && moveQueue.Count == 0)
                    moveQueue.Enqueue((minutes + 1, position));
            }

            return -1;
        }

        private List<Point> _cardinalDirecions = new List<Point> { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
        private List<Point> PossibleNewPositions(Point currentPos, Point exit, List<Blizzard> blizzards)
        {
            var result = new List<Point>();

            foreach (var dir in _cardinalDirecions)
            {
                var newPos = currentPos + dir;

                if (newPos == exit)
                    return new List<Point>() { newPos };

                if (newPos.X <= _valleyLeftX || newPos.X >= _valleyRightX || newPos.Y <= _valleyTopY || newPos.Y >= _valleyBottomY)
                    continue;

                if (blizzards.Exists(b => b.Position == newPos))
                    continue;

                result.Add(newPos);
            }

            return result;
        }

        private void MoveBlizzards(List<Blizzard> blizzards)
        {
            foreach (var blizzard in blizzards)
            {
                var newPos = blizzard.Position + blizzard.Direction;
                if (newPos.X == _valleyLeftX)
                {
                    newPos.X = _valleyRightX - 1;
                }
                else if (newPos.X == _valleyRightX)
                {
                    newPos.X = _valleyLeftX + 1;
                }
                else if (newPos.Y == _valleyTopY)
                {
                    newPos.Y = _valleyBottomY - 1;
                }
                else if (newPos.Y == _valleyBottomY)
                {
                    newPos.Y = _valleyTopY + 1;
                }

                blizzard.Position = newPos;
            }
        }

        private List<Blizzard> ParseBlizzards(string input)
        {
            var result = new List<Blizzard>();

            var lines = input.Split("\r\n").ToList();
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    var c = lines[y][x];
                    if (c == 'v' || c == '^' || c == '<' || c == '>')
                    {
                        result.Add(new Blizzard(x, y, c));
                    }
                }
            }

            return result;
        }

        class Blizzard
        {
            private Point _direction;

            public Point Position { get; set; }
            public Point Direction => _direction;

            public Blizzard(int x, int y, char inputChar)
            {
                Position = new Point(x, y);
                _direction = inputChar switch
                {
                    '<' => new Point(-1, 0),
                    '>' => new Point(1, 0),
                    '^' => new Point(0, -1),
                    'v' => new Point(0, 1),
                    _ => new Point(0, 0)
                };
            }

            public char GetChar()
            {
                if (_direction == new Point(-1, 0))
                    return '<';
                if (_direction == new Point(1, 0))
                    return '>';
                if (_direction == new Point(0, -1))
                    return '^';
                if (_direction == new Point(0, 1))
                    return 'v';

                return '.';
            }
        }
    }
}
