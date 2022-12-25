using Advent_Of_Code.Helpers.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    enum MapTiles
    {
        NONE,
        GROUND,
        WALL
    }

    public enum Direction
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3,
    }

    public static class DirectionExtensions
    {
        public static Direction TurnLeft(this Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.RIGHT;
                case Direction.RIGHT:
                    return Direction.UP;
                case Direction.UP:
                    return Direction.LEFT;
            }

            return direction;
        }

        public static Direction TurnRight(this Direction direction)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.UP;
                case Direction.UP:
                    return Direction.RIGHT;
            }

            return direction;
        }

        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.UP:
                    return Direction.DOWN;
            }

            return direction;
        }

    }

    public class Day22 : IPuzzleSolution
    {
        private Point MoveFlat(Grid<MapTiles> board, Point startingPoint, Direction facing, int distance)
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
                    while (board.TryGetSquare(wrapPoint + wrapOffset, out square) && square != MapTiles.NONE)
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

        private Point MoveCube(Grid<MapTiles> board, Point startingPoint, Direction facing, int distance, int faceLength)
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
                var foundSquare = board.TryGetSquare(lookaheadPoint + offset, out square);
                if (!foundSquare || square == MapTiles.NONE)
                {
                    
                    if (foundSquare)
                    {
                        var rotatedOffset = new Point(offset.Y, offset.X);
                        var isFacingCorrect = board.TryGetSquare((lookaheadPoint + offset) + (rotatedOffset * faceLength), out square) && square != MapTiles.NONE;

                        if (!isFacingCorrect)
                        {
                            rotatedOffset = rotatedOffset * -1;
                            isFacingCorrect = board.TryGetSquare((lookaheadPoint + offset) + (rotatedOffset * faceLength), out square) && square != MapTiles.NONE;
                        }

                        if (isFacingCorrect)
                        {
                            // We are now facing the edge we will walk to 
                            var edgeDistance = 1;
                            var wrapPoint = new Point(lookaheadPoint + offset);
                            while(board.TryGetSquare(wrapPoint + rotatedOffset, out square) && square == MapTiles.NONE)
                            {
                                edgeDistance++;
                                wrapPoint += rotatedOffset;
                            }

                            wrapPoint += offset * edgeDistance;
                            offset = rotatedOffset;
                            lookaheadPoint = wrapPoint;
                        }
                        else
                        {
                            // We are wrapping more complicated
                            var newOffset = offset * -1;
                        }

                    }
                    else
                    {
                        var newOffset = offset * -1;

                        // Face could be on opposite end
                        var oppositePoint = (lookaheadPoint + offset) + (newOffset * (faceLength * 4));
                        var isOnOpposite = board.TryGetSquare(oppositePoint, out _);
                        if (isOnOpposite)
                        {
                            offset = newOffset;
                            lookaheadPoint = oppositePoint;
                        }
                        else
                        {
                            // Going to have to look for wings
                        }
                    }
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

            var location = board.PointFromIndex(board.Squares.FindIndex(x => x == MapTiles.GROUND));
            var facing = Direction.RIGHT;

            var builder = new StringBuilder();
            for (int i = 0; i < directions.Count(); i++)
            {
                if (directions[i].Equals('L'))
                {
                    facing = facing.TurnLeft();
                }
                else if (directions[i].Equals('R'))
                {
                    facing = facing.TurnRight();
                }
                else
                {
                    builder.Append(directions[i]);
                    if (i + 1 == directions.Count() || directions[i + 1].Equals('L') || directions[i + 1].Equals('R'))
                    {
                        var moveDistance = int.Parse(builder.ToString());
                        location = MoveFlat(board, location, facing, moveDistance);
                        builder.Clear();
                    }
                }
            }

            Console.WriteLine($"Password is: {(1000 * (location.Y + 1)) + (4 * (location.X + 1)) + (int)facing}");
        }
    }

    class CubeFace
    {
        public int Id { get; }
        public Point UpperLeft { get; }
        public int Length { get; }

        public Point UpperRight => UpperLeft + new Point(Length - 1, 0);
        public Point LowerLeft => UpperLeft + new Point(0, Length - 1);
        public Point LowerRight => UpperLeft + new Point(Length - 1, Length - 1);


        public Point GridToLocal(Point p) => p - UpperLeft;
        public Point LocalToGrid(Point p) => p + UpperLeft;
        public bool Contains(Point p) => p >= UpperLeft && p <= LowerRight;

        public CubeFace(int id, Point upperLeft, int length)
        {
            Id = id;
            UpperLeft = upperLeft;
            Length = length;
        }
    }

    record struct Edge(int FaceId, Direction Direction);

    enum EdgeType
    {
        Connected,
        Short,
        WalkBack,
        CornerTraversal
    }

    class FlatCube<T>
    {
        public CubeFace[] Faces { get; }
        public Dictionary<Edge, (Edge, EdgeType)> EdgeMappings { get; };
        public Grid<T> Grid { get; }

        public FlatCube(Grid<T> grid, int faceLength, Func<T, bool> isFaceSqaure)
        {
            Grid = grid;

            Faces = new CubeFace[6];
            int faceId = 0;
            for (int y = 0; y < grid.Height; y += faceLength)
            {
                for (int x = 0; x < grid.Width; x += faceLength)
                {
                    if (isFaceSqaure(grid.GetSquare(new Point(x, y))))
                    {
                        Faces[faceId] = new CubeFace(faceId, new Point(x, y), faceLength);
                        faceId++;
                    }
                }
            }

            EdgeMappings = new Dictionary<Edge, (Edge, EdgeType)>();
            MapConnectedEdges(isFaceSqaure);
            MapShortEdges(isFaceSqaure);
            MapWalkBacks(isFaceSqaure);
            MapCornerTraversals(isFaceSqaure);
        }

        private void AddOppositeMapping(Edge key, Edge value, EdgeType type)
        {
            if (EdgeMappings.ContainsKey(key)) return;

            EdgeMappings.Add(
                new Edge(value.FaceId, value.Direction.Opposite()),
                (new Edge(key.FaceId, key.Direction.Opposite()), type));
        }

        private void MapConnectedEdges(Func<T, bool> isFaceSquare)
        {
            foreach (var face in Faces)
            {
                // UP
                var upKey = new Edge(face.Id, Direction.UP);
                if (!EdgeMappings.ContainsKey(upKey))
                {
                    var upTestPoint = face.UpperLeft + new Point(0, -face.Length);
                    var connectedFace = Faces.First(f => f.Id != face.Id && f.Contains(upTestPoint));
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.UP);
                        EdgeMappings.Add(upKey, (connectedEdge, EdgeType.Connected));
                        AddOppositeMapping(upKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // RIGHT
                var rightKey = new Edge(face.Id, Direction.RIGHT);
                if (!EdgeMappings.ContainsKey(rightKey))
                {
                    var rightTestPoint = face.UpperLeft + new Point(face.Length, 0);
                    var connectedFace = Faces.First(f => f.Id != face.Id && f.Contains(rightTestPoint));
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.RIGHT);
                        EdgeMappings.Add(rightKey, (connectedEdge, EdgeType.Connected));
                        AddOppositeMapping(upKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // DOWN
                var downKey = new Edge(face.Id, Direction.DOWN);
                if (!EdgeMappings.ContainsKey(downKey))
                {
                    var downTestPoint = face.UpperLeft + new Point(0, face.Length);
                    var connectedFace = Faces.First(f => f.Id != face.Id && f.Contains(downTestPoint));
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.DOWN);
                        EdgeMappings.Add(downKey, (connectedEdge, EdgeType.Connected));
                        AddOppositeMapping(upKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // LEFT
                var leftKey = new Edge(face.Id, Direction.LEFT);
                if (!EdgeMappings.ContainsKey(leftKey))
                {
                    var downTestPoint = face.UpperLeft + new Point(0, face.Length);
                    var connectedFace = Faces.First(f => f.Id != face.Id && f.Contains(downTestPoint));
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.LEFT);
                        EdgeMappings.Add(leftKey, (connectedEdge, EdgeType.Connected));
                        AddOppositeMapping(upKey, connectedEdge, EdgeType.Connected);
                    }
                }
            }
        }

        private void MapShortEdges(Func<T, bool> isFaceSquare)
        {

        }

        private void MapWalkBacks(Func<T, bool> isFaceSquare)
        {

        }

        private void MapCornerTraversals(Func<T, bool> isFaceSquare)
        {

        }
    }
}
