using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    static class Resources
    {
        public const int ORE = 0;
        public const int CLAY = 1;
        public const int OBSIDIAN = 2;
        public const int GEODE = 3;
    }

    class FactoryState
    {
        public int Time { get; set; }
        public List<int> RobotCount { get; set; }
        public List<int> ResourceCount { get; set; }

        public FactoryState(int time, List<int> botCount, List<int> resCount)
        {
            Time = time;
            RobotCount = new List<int>(botCount);
            ResourceCount = new List<int>(resCount);
        }
    }

    class BuildCost
    {
        public int OreCost { get; set; }
        public int ClayCost { get; set; }
        public int ObsidianCost { get; set; }

        public BuildCost()
        {
            OreCost = 0;
            ClayCost = 0;
            ObsidianCost = 0;
        }
    }

    class RobotFactory
    {
        private Regex _blueprintParser = new Regex(@"ore robot[\D]+([\d]+).*clay robot[\D]+([\d]+).*obsidian robot[\D]+([\d]+)[\D]+([\d]+).*geode robot[\D]+([\d]+)[\D]+([\d]+)", RegexOptions.Compiled);

        private List<int> _robotCounts;
        private List<int> _resourceCounts;
        private List<BuildCost> _buildCosts;

        private int _time;
        private Stack<FactoryState> _states;

        public RobotFactory(string blueprint)
        {
            _robotCounts = Enumerable.Repeat(0, 4).ToList();
            _robotCounts[Resources.ORE]++;

            _resourceCounts = Enumerable.Repeat(0, 4).ToList();

            _time = 0;
            _states = new Stack<FactoryState>();
            SaveState();

            var regexGroups = _blueprintParser.Match(blueprint).Groups;

            _buildCosts = new List<BuildCost>()
            {
                new BuildCost() { OreCost = int.Parse(regexGroups[1].Value) },
                new BuildCost() { OreCost = int.Parse(regexGroups[2].Value) },
                new BuildCost() { OreCost = int.Parse(regexGroups[3].Value), ClayCost = int.Parse(regexGroups[4].Value) },
                new BuildCost() { OreCost = int.Parse(regexGroups[5].Value), ClayCost = int.Parse(regexGroups[6].Value) },
            };
        }

        private void SaveState()
        {
            _states.Push(new FactoryState(_time, _robotCounts, _resourceCounts));
        }

        private void Gather()
        {
            for (int r = 0; r < 4; r++)
            {
                _resourceCounts[r] += _robotCounts[r];
            }
        }

        private bool CanBuildRobot(int type)
        {
            var buildCost = _buildCosts[type];
            return _resourceCounts[Resources.ORE] >= buildCost.OreCost
                && _resourceCounts[Resources.CLAY] >= buildCost.ClayCost
                && _resourceCounts[Resources.OBSIDIAN] >= buildCost.ObsidianCost;
        }

        private void BuildRobot(int type)
        {
            while(!CanBuildRobot(type))
            {
                Gather();
                _time++;
            }

            Gather();

            var buildCost = _buildCosts[type];
            _resourceCounts[Resources.ORE] -= buildCost.OreCost;
            _resourceCounts[Resources.CLAY] -= buildCost.ClayCost;
            _resourceCounts[Resources.OBSIDIAN] -= buildCost.ObsidianCost;

            _robotCounts[type]++;
            _time++;

            SaveState();
        }
    
        private void PassTime(int step = 1)
        {
            for (int i = 0; i < step; i++)
            {
                Gather();
                _time++;
            }
        }

        private void PassUntilTime(int targetTime)
        {
            var stepCount = targetTime - _time;
            PassTime(stepCount);
        }

        private void UndoBuild()
        {
            var state = _states.Pop();
            _time = _states.Peek().Time;
            _robotCounts = new List<int>(_states.Peek().RobotCount);
            _resourceCounts = new List<int>(_states.Peek().ResourceCount);
        }

        private int TimeToBuild(int type)
        {
            var buildCost = _buildCosts[type];

            return Math.Max(
                Math.Max(
                    buildCost.OreCost - _resourceCounts[Resources.ORE], 
                    buildCost.ClayCost - _resourceCounts[Resources.CLAY]), 
                    buildCost.ObsidianCost - _resourceCounts[Resources.OBSIDIAN]);
        }

        public List<int> PotentialRobotTargets()
        {
            var result = new List<int>() { Resources.ORE, Resources.CLAY };

            if (_robotCounts[Resources.CLAY] > 0)
                result.Add(Resources.OBSIDIAN);

            if (_robotCounts[Resources.OBSIDIAN] > 0)
                result.Add(Resources.GEODE);

            return result;
        }
    }

    public class Day19 : IPuzzleSolution
    {
        public void Run(string input)
        {
            // EXAAMPLE PARSING ONLY
            // PRE-PARSE TO ACTUAL DATA INPUT
            var actualInput = string.Join("\r\n", input
                .Split("\r\n\r\n")
                .Select(blueprint => string.Join(" ", blueprint.Split("\r\n").Select(line => line.Trim())))
            );

            foreach (var line in actualInput.Split("\r\n"))
            {
            }
        }
    }
}
