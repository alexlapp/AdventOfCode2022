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

        public static Point AsPoint(this Direction direction)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    return new Point(1, 0);
                case Direction.LEFT:
                    return new Point(-1, 0);
                case Direction.DOWN:
                    return new Point(0, 1);
                case Direction.UP:
                    return new Point(0, -1);
            }

            return new Point(0, 0);
        }
    }

    public class Day22 : IPuzzleSolution
    {
        private (Point, Direction) MoveFlat(Grid<MapTiles> board, Point startingPoint, Direction facing, int distance)
        {
            var offset = facing.AsPoint();

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

                if (board.GetSquare(lookaheadPoint) == MapTiles.WALL) { return (resultPoint, facing); }

                resultPoint = lookaheadPoint;
            }
            return (resultPoint, facing);
        }

        private (Point, Direction) MoveCube(FlatCube<MapTiles> cube, Grid<MapTiles> board, Point startingPoint, Direction facing, int distance)
        {
            var currentFacing = facing;

            var resultPoint = new Point(startingPoint);
            var lookaheadPoint = resultPoint;

            MapTiles square; // Define here so we aren't allocating multiple times in loop
            for (int i = 0; i < distance; i++)
            {
                var foundSquare = board.TryGetSquare(lookaheadPoint + currentFacing.AsPoint(), out square);
                if (!foundSquare || square == MapTiles.NONE)
                {
                    // Wrapping required
                    (var newLookaheadPoint, var newFacing) = cube.Wrap(lookaheadPoint, currentFacing);

                    lookaheadPoint = newLookaheadPoint;
                    currentFacing = newFacing;
                }
                else { lookaheadPoint += currentFacing.AsPoint(); }

                if (board.GetSquare(lookaheadPoint) == MapTiles.WALL) { return (resultPoint, currentFacing); }

                resultPoint = lookaheadPoint;
            }
            return (resultPoint, currentFacing);
        }

        private (Point, Direction) Traverse(char[] directions, Grid<MapTiles> board, Func<Grid<MapTiles>, Point, Direction, int, (Point, Direction)> traversalStrategy)
        {
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
                        (var newLocation, var newFacing) = traversalStrategy(board, location, facing, moveDistance);
                        location = newLocation;
                        facing = newFacing;
                        builder.Clear();
                    }
                }
            }

            return (location, facing);
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

            (var flatFinalLocation, var flatFinalDirection) = Traverse(directions, board, MoveFlat);
            Console.WriteLine($"Password is: {(1000 * (flatFinalLocation.Y + 1)) + (4 * (flatFinalLocation.X + 1)) + (int)flatFinalDirection}");

            var cube = new FlatCube<MapTiles>(board, 4, (tile) => tile != MapTiles.NONE);
            (var cubeFinalLocation, var cubeFinalDirection) = Traverse(directions, board, (board, p, d, i) => MoveCube(cube, board, p, d, i));
            Console.WriteLine($"Password is: {(1000 * (cubeFinalLocation.Y + 1)) + (4 * (cubeFinalLocation.X + 1)) + (int)cubeFinalDirection}");
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

        public bool WalkingOffEdge(Point p, Direction d)
        {
            return Contains(p) && !Contains(p + d.AsPoint());
        }

        public CubeFace(int id, Point upperLeft, int length)
        {
            Id = id;
            UpperLeft = upperLeft;
            Length = length;
        }
    }

    record struct Edge(int FaceId, Direction Direction)
    {
        public Edge Opposite()
        {
            return new Edge(FaceId, Direction.Opposite());
        }
    };

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
        public Dictionary<Edge, (Edge, EdgeType)> EdgeMappings { get; }
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

        public bool TryGetEdge(Point p, Direction d, out Edge edge)
        {
            var face = FindFaceContainingPoint(p);
            if (face == null)
            {
                edge = default;
                return false;
            }

            edge = new Edge(face.Id, d);
            return true;
        }

        public (Point, Direction) Wrap(Point start, Direction facing)
        {
            if (!TryGetEdge(start, facing, out var edge)) { throw new Exception("This should never happen"); }
            var startFace = Faces[edge.FaceId];

            if (!startFace.WalkingOffEdge(start, facing)) { return (start + facing.AsPoint(), facing); }

            (var connectedEdge, var edgeType) = EdgeMappings[edge];
            var connectedFace = Faces[connectedEdge.FaceId];

            var resultPoint = new Point(start);
            var resultFacing = facing;

            switch (edgeType)
            {
                case EdgeType.Connected:
                    resultFacing = connectedEdge.Direction;
                    resultPoint = resultPoint + resultFacing.AsPoint();
                    break;

                case EdgeType.Short:
                    {
                        var startLocal = startFace.GridToLocal(start);
                        var endLocal = new Point(startLocal.Y, startLocal.X);
                        resultPoint = connectedFace.LocalToGrid(endLocal);
                        resultFacing = connectedEdge.Direction;
                        break;
                    }

                case EdgeType.WalkBack:
                    {
                        if (facing == connectedEdge.Direction)
                        {
                            var startLocal = startFace.GridToLocal(start);

                            var axisMask = Point.Abs(facing.AsPoint());
                            var axisValue = (startLocal * axisMask == new Point(0, 0)) ? axisMask * (startFace.Length - 1) : new Point(0, 0); ;

                            var offAxisMask = Point.Abs(new Point(facing.AsPoint().Y, facing.AsPoint().X));
                            var offAxisValue = offAxisMask * startLocal;


                            resultPoint = connectedFace.LocalToGrid(offAxisValue + axisValue);
                            resultFacing = connectedEdge.Direction;
                        }
                        else if (facing.Opposite() == connectedEdge.Direction)
                        {
                            throw new Exception("Not implemented and maybe not possible");
                        }
                        else
                        {
                            var startLocal = startFace.GridToLocal(start);

                            var startAxisMask = Point.Abs(facing.AsPoint());
                            var startAxisValue = startAxisMask * startLocal;

                            var firstValue = new Point(startAxisValue.Y, startAxisValue.X);

                            if (!((facing == Direction.RIGHT && connectedEdge.Direction == Direction.DOWN)
                                || (facing == Direction.DOWN && connectedEdge.Direction == Direction.RIGHT)))
                            {
                                firstValue = startAxisMask * firstValue.Transform(x => (startFace.Length - 1) - x);
                            }

                            var secondValue = connectedEdge.Direction.Opposite().AsPoint().Transform(x => x > 0 ? 1 : 0) * (startFace.Length - 1);

                            resultPoint = firstValue + secondValue;
                            resultFacing = connectedEdge.Direction;
                        }
                        break;
                    }

                case EdgeType.CornerTraversal:
                    {

                        var startLocal = startFace.GridToLocal(start);

                        var startAxisMask = Point.Abs(facing.AsPoint());
                        var startAxisValue = startAxisMask * startLocal.Transform(x => (startFace.Length - 1) - x); ;

                        var firstValue = new Point(startAxisValue.Y, startAxisValue.X);

                        var secondValue = connectedEdge.Direction.Opposite().AsPoint().Transform(x => x > 0 ? 1 : 0) * (startFace.Length - 1);

                        resultPoint = firstValue + secondValue;
                        resultFacing = connectedEdge.Direction;

                        break;
                    }
            }

            return (resultPoint, resultFacing);
        }

        private List<Edge> UnmappedEdges()
        {
            var result = new List<Edge>();

            for (int i = 0; i < Faces.Length; i++)
            {
                foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
                {
                    var edge = new Edge(i, dir);
                    if (!EdgeMappings.ContainsKey(edge)) { result.Add(edge); }
                }
            }

            return result;
        }

        private void AddMapping(Edge edgeA, Edge edgeB, EdgeType type)
        {
            if (EdgeMappings.ContainsKey(edgeA)) return;

            EdgeMappings.Add(edgeA, (edgeB, type));

            var edgeBFlipped = new Edge(edgeB.FaceId, edgeB.Direction.Opposite());
            if (EdgeMappings.ContainsKey(edgeBFlipped)) return;

            EdgeMappings.Add(
                edgeBFlipped,
                (new Edge(edgeA.FaceId, edgeA.Direction.Opposite()), type));
        }

        private CubeFace? FindFaceContainingPoint(Point p)
        {
            foreach (var face in Faces)
            {
                if (face.Contains(p)) { return face; }
            }

            return null;
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
                    var connectedFace = FindFaceContainingPoint(upTestPoint);
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.UP);
                        AddMapping(upKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // RIGHT
                var rightKey = new Edge(face.Id, Direction.RIGHT);
                if (!EdgeMappings.ContainsKey(rightKey))
                {
                    var rightTestPoint = face.UpperLeft + new Point(face.Length, 0);
                    var connectedFace = FindFaceContainingPoint(rightTestPoint);
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.RIGHT);
                        AddMapping(rightKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // DOWN
                var downKey = new Edge(face.Id, Direction.DOWN);
                if (!EdgeMappings.ContainsKey(downKey))
                {
                    var downTestPoint = face.UpperLeft + new Point(0, face.Length);
                    var connectedFace = FindFaceContainingPoint(downTestPoint);
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.DOWN);
                        AddMapping(downKey, connectedEdge, EdgeType.Connected);
                    }
                }

                // LEFT
                var leftKey = new Edge(face.Id, Direction.LEFT);
                if (!EdgeMappings.ContainsKey(leftKey))
                {
                    var leftTestPoint = face.UpperLeft + new Point(-face.Length, 0);
                    var connectedFace = FindFaceContainingPoint(leftTestPoint);
                    if (connectedFace != null)
                    {
                        var connectedEdge = new Edge(connectedFace.Id, Direction.LEFT);
                        AddMapping(leftKey, connectedEdge, EdgeType.Connected);
                    }
                }
            }
        }

        private void MapShortEdges(Func<T, bool> isFaceSquare)
        {
            foreach (var edge in UnmappedEdges())
            {
                if (EdgeMappings.ContainsKey(edge)) { continue; }

                (var faceId, var direction) = edge;
                var face = Faces[faceId];

                var connectedFace = FindFaceContainingPoint(face.UpperLeft + (direction.AsPoint() * face.Length));
                if (connectedFace != null) continue;

                var shortEdgeOffsetCCW = direction.AsPoint() + direction.TurnLeft().AsPoint();
                shortEdgeOffsetCCW *= face.Length;

                var ccwFace = FindFaceContainingPoint(face.UpperLeft + shortEdgeOffsetCCW);
                if (ccwFace != null)
                {
                    var connection = new Edge(ccwFace.Id, direction.TurnLeft());
                    AddMapping(edge, connection, EdgeType.Connected);
                }
                else
                {
                    var shortEdgeOffsetCW = direction.AsPoint() + direction.TurnRight().AsPoint();
                    shortEdgeOffsetCW *= face.Length;
                    
                    var cwFace = FindFaceContainingPoint(face.UpperLeft + shortEdgeOffsetCW);
                    if (cwFace != null)
                    {
                        var connection = new Edge(cwFace.Id, direction.TurnRight());
                        AddMapping(edge, connection, EdgeType.Connected);
                    }
                }
            }
        }

        private Edge? WalkbckEdge(Edge edge)
        {
            var currentEdge = edge.Opposite();
            for (int i = 0; i < 3; i++)
            {
                if (EdgeMappings.TryGetValue(currentEdge, out var connection))
                {
                    currentEdge = connection.Item1;
                }
                else
                {
                    return null;
                }
            }

            return currentEdge.Opposite();
        }
        
        private void MapWalkBacks(Func<T, bool> isFaceSquare)
        {
            foreach (var edge in UnmappedEdges())
            {
                if (EdgeMappings.ContainsKey(edge)) { continue; }

                var connection = WalkbckEdge(edge);
                if (connection != null)
                {
                    AddMapping(edge, (Edge)connection, EdgeType.WalkBack);
                }
            }
        }

        private Edge? CornerTraversalCW(Edge edge)
        {
            var upperEdge = new Edge(edge.FaceId, edge.Direction.TurnLeft());

            if (!EdgeMappings.TryGetValue(upperEdge, out var upperConnection)) { return null; }
            var followedEdge = upperConnection.Item1;
            var edgeToConnection = new Edge(followedEdge.FaceId, followedEdge.Direction.TurnRight());

            if (!EdgeMappings.TryGetValue(edgeToConnection, out var connectedUpperEdgeValue)) { return null; }
            var connectedUpperEdge = connectedUpperEdgeValue.Item1;

            return new Edge(connectedUpperEdge.FaceId, connectedUpperEdge.Direction.TurnLeft());
        }

        private Edge? CornerTraversalCCW(Edge edge)
        {

            var lowerEdge = new Edge(edge.FaceId, edge.Direction.TurnRight());

            if (!EdgeMappings.TryGetValue(lowerEdge, out var lowerConnection)) { return null; }
            var followedEdge = lowerConnection.Item1;
            var edgeToConnection = new Edge(followedEdge.FaceId, followedEdge.Direction.TurnLeft());

            if (!EdgeMappings.TryGetValue(edgeToConnection, out var connectedLowerEdgeValue)) { return null; }
            var connectedLowerEdge = connectedLowerEdgeValue.Item1;

            return new Edge(connectedLowerEdge.FaceId, connectedLowerEdge.Direction.TurnRight());
        }

        private void MapCornerTraversals(Func<T, bool> isFaceSquare)
        {
            foreach (var edge in UnmappedEdges())
            {
                if (EdgeMappings.ContainsKey(edge)) { continue; }

                var connectionCW = CornerTraversalCW(edge);
                if (connectionCW != null)
                {
                    AddMapping(edge, (Edge)connectionCW, EdgeType.CornerTraversal);
                    continue;
                }

                var connectionCCW = CornerTraversalCCW(edge);
                if (connectionCCW != null)
                {
                    AddMapping(edge, (Edge)connectionCCW, EdgeType.CornerTraversal);
                }
            }
        }
    }
}
