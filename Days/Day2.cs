using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    public class Day2 : IPuzzleSolution
    {
        private enum Play
        {
            Rock,
            Paper,
            Scissors
        }

        private Play ParseOpponentPlay(string code)
        {
            if (code.Equals("A")) return Play.Rock;
            if (code.Equals("B")) return Play.Paper;
            return Play.Scissors;
        }

        private Play ParsePlayOne(string code)
        {
            if (code.Equals("X")) return Play.Rock;
            if (code.Equals("Y")) return Play.Paper;
            return Play.Scissors;
        }

        private Play ParsePlayTwo(Play opponent, string code)
        {
            if (code.Equals("X"))
            { // Lose
                if (opponent == Play.Rock) return Play.Scissors;
                if (opponent == Play.Paper) return Play.Rock;
                if (opponent == Play.Scissors) return Play.Paper;
            }

            if (code.Equals("Z"))
            { // Win
                if (opponent == Play.Rock) return Play.Paper;
                if (opponent == Play.Paper) return Play.Scissors;
                if (opponent == Play.Scissors) return Play.Rock;
            }
                
            // Draw
            return opponent;
        }

        private int GetOutcomeScore(Play opponent, Play me)
        {
            if (opponent == me) return 3;

            if ((opponent == Play.Rock && me == Play.Paper)
                || (opponent == Play.Paper && me == Play.Scissors)
                || (opponent == Play.Scissors && me == Play.Rock))
                return 6;

            return 0;
        }

        private int GetPlayScore(Play play)
        {
            switch (play)
            {
                case Play.Rock: return 1;
                case Play.Paper: return 2;
                default: return 3;
            }
        }

        public void Run(string input)
        {
            var rounds = input.Split("\r\n");

            int score = 0;
            foreach (var round in rounds)
            {
                var plays = round.Split(" ");
                var opponentPlay = ParseOpponentPlay(plays[0]);
                var myPlay = ParsePlayTwo(opponentPlay, plays[1]);

                score += GetOutcomeScore(opponentPlay, myPlay) + GetPlayScore(myPlay);
            }

            Console.WriteLine($"Total score following guide: {score}");
        }
    }
}
