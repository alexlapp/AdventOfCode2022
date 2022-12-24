using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day22 : IPuzzleSolution
    {
        enum MapTiles
        {
            NONE,
            GROUND,
            WALL
        }

        enum Direction
        {
            RIGHT = 0,
            DOWN = 1,
            LEFT = 2,
            UP = 3,
        }

        private Point Move(Grid<MapTiles> board, Point startingPoint, Direction facing, int distance)
        {
            var offset = facing switch
            {
                Direction.RIGHT => new Point(1, 0),
                Direction.DOWN => new Point(0, 1),
                Direction.LEFT => new Point(-1, 0),
                Direction.UP => new Point(0, -1),
                _ => new Point(0, 0)
            };

            var resultPoint = new Point(startingPoint);
            var lookaheadPoint = resultPoint;

            MapTiles square; // Define here so we aren't allocating multiple times in loop
            for (int i = 0; i < distance; i++)
            {
                if (!board.TryGetSquare(lookaheadPoint + offset, out square) || square == MapTiles.NONE)
                {
                    // actual look ahead point is wrapped around to other side
                    var wrapOffset = offset * -1;
                    var wrapPoint = new Point(resultPoint);
                    while (board.TryGetSquare(lookaheadPoint + wrapOffset, out square) && square != MapTiles.NONE)
                    {
                        wrapPoint += wrapOffset;
                    }
                    lookaheadPoint = wrapPoint;
                }
                else { lookaheadPoint += offset; }

                if (board.GetSquare(lookaheadPoint) == MapTiles.WALL) { return resultPoint; }

                resultPoint = lookaheadPoint;
            }
            return resultPoint;
        }

        public void Run(string input)
        {
            var groups = input.Split("\r\n\r\n").ToList();
            var mapString = groups[0];
            var directions = groups[1].ToCharArray();

            var boardWidth = mapString.Split("\r\n").Max(l => l.Length);
            var board = new Grid<MapTiles>(boardWidth, mapString.Count(x => x == '\n') + 1, 
                string.Join("", mapString.Split("\r\n").Select(x => x.PadRight(boardWidth))).Select(x =>
                {
                    return x switch
                    {
                        '.' => MapTiles.GROUND,
                        '#' => MapTiles.WALL,
                        _ => MapTiles.NONE,
                    };
                }).ToList());

            //var mapLines = mapString.Split("\r\n").ToList();
            //for (int y = 0; y < mapLines.Count; y++)
            //{
            //    var line = mapLines[y];
            //    for (int x = 0; x < line.Length; x++)
            //    {
            //        var tile = line[x] switch
            //        {
            //            '.' => MapTiles.GROUND,
            //            '#' => MapTiles.WALL,
            //            _ => MapTiles.NONE,
            //        };

            //        board.Squares[(y * board.Width) + x] = tile;
            //    }
            //}

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var c = board.GetSquare(new Point(x, y)) switch
                    {
                        MapTiles.GROUND => '.',
                        MapTiles.WALL => '#',
                        _ => ' ',
                    };
                    Console.Write(c);
                }
                Console.WriteLine();
            }

            var location = board.PointFromIndex(board.Squares.FindIndex(x => x == MapTiles.GROUND));
            var facing = Direction.RIGHT;

            var builder = new StringBuilder();
            for (int i = 0; i < directions.Count(); i++)
            {
                if (directions[i].Equals('L'))
                {
                    switch (facing)
                    {
                        case Direction.LEFT:
                            facing = Direction.DOWN;
                            break;
                        case Direction.DOWN:
                            facing = Direction.RIGHT;
                            break;
                        case Direction.RIGHT:
                            facing = Direction.UP;
                            break;
                        case Direction.UP:
                            facing = Direction.LEFT;
                            break;
                    }
                }
                else if (directions[i].Equals('R'))
                {
                    switch (facing)
                    {
                        case Direction.LEFT:
                            facing = Direction.UP;
                            break;
                        case Direction.UP:
                            facing = Direction.RIGHT;
                            break;
                        case Direction.RIGHT:
                            facing = Direction.DOWN;
                            break;
                        case Direction.DOWN:
                            facing = Direction.LEFT;
                            break;
                    }
                }
                else
                {
                    builder.Append(directions[i]);
                    if (i + 1 == directions.Count() || directions[i + 1].Equals('L') || directions[i + 1].Equals('R'))
                    {
                        var moveDistance = int.Parse(builder.ToString());
                        location = Move(board, location, facing, moveDistance);
                        builder.Clear();
                    }
                }
            }

            Console.WriteLine($"Password is: {(1000 * (location.X + 1)) + (4 * (location.Y + 1)) + (int)facing}");
        }
    }
}
