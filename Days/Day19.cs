using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    record Resources(int Ore, int Clay, int Obisidian, int Geode) : IComparable<Resources>
    {
        public static Resources operator +(Resources a) => a;
        public static Resources operator -(Resources a) => new Resources(-a.Ore, -a.Clay, -a.Obisidian, -a.Geode);
        public static Resources operator+(Resources a, Resources b)
            => new Resources(a.Ore + b.Ore, a.Clay + b.Clay, a.Obisidian + b.Obisidian, a.Geode + b.Geode);
        public static Resources operator -(Resources a, Resources b)
            => new Resources(a.Ore - b.Ore, a.Clay - b.Clay, a.Obisidian - b.Obisidian, a.Geode - b.Geode);
        public bool All(Func<int, bool> f)
            => f(Ore) && f(Clay) && f(Obisidian) && f(Geode);

        public bool All(Resources other, Func<int, int, bool> f)
            => f(Ore, other.Ore) && f(Clay, other.Clay) && f(Obisidian, other.Obisidian) && f(Geode, other.Geode);

        public (int, int, int, int) AsTuple() => (Ore, Clay, Obisidian, Geode);

        public int CompareTo(Resources other)
        {
            if (Geode < other.Geode)
                return -1;
            if (Geode > other.Geode)
                return 1;
            if (Obisidian < other.Obisidian)
                return -1;
            if (Obisidian > other.Obisidian)
                return 1;
            if (Clay < other.Clay)
                return -1;
            if (Clay > other.Clay)
                return 1;
            if (Ore < other.Ore)
                return -1;
            if (Ore > other.Ore)
                return 1;

            return 0;
        }
    }

    class Blueprint
    {
        private Regex _blueprintParser = new Regex(@"Blueprint ([\d]+).*ore robot[\D]+([\d]+).*clay robot[\D]+([\d]+).*obsidian robot[\D]+([\d]+)[\D]+([\d]+).*geode robot[\D]+([\d]+)[\D]+([\d]+)", RegexOptions.Compiled);

        public int Id { get; }
        public (Resources, Resources)[] CostProduction { get; }

        public Blueprint(string blueprintString)
        {
            var groups = _blueprintParser.Match(blueprintString).Groups;

            Id = int.Parse(groups[1].Value);
            CostProduction = new (Resources, Resources)[5];

            CostProduction[0] = (
                new Resources(int.Parse(groups[2].Value), 0, 0, 0),
                new Resources(1, 0, 0, 0));

            CostProduction[1] = (
                new Resources(int.Parse(groups[3].Value), 0, 0, 0),
                new Resources(0, 1, 0, 0));

            CostProduction[2] = (
                new Resources(int.Parse(groups[4].Value), int.Parse(groups[5].Value), 0, 0),
                new Resources(0, 0, 1, 0));

            CostProduction[3] = (
                new Resources(int.Parse(groups[6].Value), 0, int.Parse(groups[7].Value), 0),
                new Resources(0, 0, 0, 1));

            CostProduction[4] = (new Resources(0, 0, 0, 0), new Resources(0, 0, 0, 0));
        }
    }

    public class Day19 : IPuzzleSolution
    {
        int FindOptimalOutput(Blueprint blueprint, int time)
        {
            var todo = new List<(Resources, Resources)>() { (new Resources(0, 0, 0, 0), new Resources(1, 0, 0, 0)) };
            for (int t = time; t > 0; t--)
            {
                var rawTodo = new List<(Resources, Resources)>();
                foreach ((var have, var make) in todo)
                {
                    foreach ((var cost, var prod) in blueprint.CostProduction)
                    {
                        if (have.Ore >= cost.Ore && have.Clay >= cost.Clay && have.Obisidian >= cost.Obisidian && have.Geode >= cost.Geode)
                        {
                            rawTodo.Add((have + make - cost, make + prod));
                        }
                    }
                }
                todo = rawTodo
                    .OrderByDescending(t => t.Item1 + t.Item2 + t.Item2)
                    .Take(10000)
                    .ToList();
            }
            var result = todo.Max((state) => state.Item1.Geode);
            return result;
        }

        public void Run(string input)
        {
            var sum = input.Split("\r\n")
                .Select(blueprint => new Blueprint(blueprint))
                .Aggregate(0, (acc, blueprint) => acc + (blueprint.Id * FindOptimalOutput(blueprint, 24)));

            Console.WriteLine($"Sum of qualities: {sum}");

            var product = input.Split("\r\n")
                .Take(3)
                .Select(blueprint => new Blueprint(blueprint))
                .Aggregate(1, (acc, blueprint) => acc * FindOptimalOutput(blueprint, 32));

            Console.WriteLine($"Product of first 3 blueprints: {product}");
        }
    }
}
