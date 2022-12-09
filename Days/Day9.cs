using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day9 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var twoRope = new RopeSegment(2);
            var twoVisited = new HashSet<(int, int)>();

            var tenRope = new RopeSegment(10);
            var tenVisted = new HashSet<(int, int)>();

            foreach (var line in input.Split("\r\n"))
            {
                var values = line.Split(" ");
                var dir = values[0][0];
                var amount = int.Parse(values[1]);

                for (int i = 0; i < amount; i++)
                {
                    twoRope.MoveHead(dir);
                    twoVisited.Add(twoRope.TailCoordinates());

                    tenRope.MoveHead(dir);
                    tenVisted.Add(tenRope.TailCoordinates());
                }
            }

            Console.WriteLine($"Visited {twoVisited.Count} cells");
            Console.WriteLine($"Visited {tenVisted.Count} cells");
        }
    }

    class Knot
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    class RopeSegment
    {
        public Knot Head { get; set; }
        public RopeSegment Next { get; set; }

        public RopeSegment(int length)
        {
            Head = new Knot { X = 0, Y = 0 };
            if (length > 1)
            {
                Next = new RopeSegment(length - 1);
            }
        }

        public void FollowSegment(RopeSegment leader)
        {
            if (Math.Abs(leader.Head.X - Head.X) <= 1 && Math.Abs(leader.Head.Y - Head.Y) <= 1) return;

            // STRAIGHT MOVEMENTS
            if (leader.Head.X == Head.X)
            {
                Head.Y += (leader.Head.Y > Head.Y) ? 1 : -1;
            }
            else if (leader.Head.Y == Head.Y)
            {
                Head.X += (leader.Head.X > Head.X) ? 1 : -1;
            }
            // DIAGONAL MOVEMENTS
            else
            {
                Head.X += (leader.Head.X > Head.X) ? 1 : -1;
                Head.Y += (leader.Head.Y > Head.Y) ? 1 : -1;
            }

            if (Next != null)
            {
                Next.FollowSegment(this);
            }
        }

        public void MoveHead(char direction)
        {
            switch (direction)
            {
                case 'L':
                    Head.X -= 1;
                    break;
                case 'R':
                    Head.X += 1;
                    break;
                case 'D':
                    Head.Y -= 1;
                    break;
                case 'U':
                    Head.Y += 1;
                    break;
            }

            if (Next != null) Next.FollowSegment(this);
        }

        public (int, int) TailCoordinates()
        {
            if (Next == null)
                return (Head.X, Head.Y);

            return Next.TailCoordinates();
        }
    }

    class OldRope
    {
        public Knot Head { get; set; }
        public Knot Tail { get; set; }

        public OldRope()
        {
            Head = new Knot { X = 0, Y = 0 };
            Tail = new Knot { X = 0, Y = 0 };
        }

        public void MoveHead(char direction)
        {
            switch (direction)
            {
                case 'L':
                    Head.X -= 1;
                    break;
                case 'R':
                    Head.X += 1;
                    break;
                case 'D':
                    Head.Y -= 1;
                    break;
                case 'U':
                    Head.Y += 1;
                    break;
            }

            if (Math.Abs(Head.X - Tail.X) <= 1 && Math.Abs(Head.Y - Tail.Y) <= 1) return;

            if (Head.X == Tail.X)
            {
                Tail.Y += (Head.Y > Tail.Y) ? 1 : -1;
            }
            else if (Head.Y == Tail.Y)
            {
                Tail.X += (Head.X > Tail.X) ? 1 : -1;
            }
            else if (Head.X - Tail.X == -2)
            {
                Tail.Y = Head.Y;
                Tail.X -= 1;
            }
            else if (Head.X - Tail.X == 2)
            {
                Tail.Y = Head.Y;
                Tail.X += 1;
            }
            else if (Head.Y - Tail.Y == -2)
            {
                Tail.X = Head.X;
                Tail.Y -= 1;
            }
            else if (Head.Y - Tail.Y == 2)
            {
                Tail.X = Head.X;
                Tail.Y += 1;
            }
        }

        public (int, int) TailCoordinates()
        {
            return (Tail.X, Tail.Y);
        }
    }
}
