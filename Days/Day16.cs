using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class Valve
    {
        public string Id { get; set; }
        public int FlowRate { get; set; }
        public bool IsOpen { get; set; }
        public List<string> Tunnels { get; set; }

        public int PotentialPressure(int timeRemaining) => IsOpen ? 0 : (timeRemaining - 1) * FlowRate;

        public Valve(string id, int flowRate, List<string> tunnels)
        {
            Id = id;
            FlowRate = flowRate;
            Tunnels = tunnels;
            IsOpen = false;
        }

        public void AddTunnel(string tunnelId)
        {
            Tunnels.Add(tunnelId);
        }
    }

    class ValveTunnels
    {
        public Dictionary<string, Valve> Valves { get; set; }

        public ValveTunnels()
        {
            Valves = new Dictionary<string, Valve>();
            _distanceCache = new Dictionary<(string, string), int>();
        }

        public void AddValve(string id, int flowRate, List<string> tunnels)
        {
            Valves.Add(id, new Valve(id, flowRate, tunnels));
        }

        public Valve GetValve(string id)
        {
            Valves.TryGetValue(id, out var valve);
            if (valve == null) { throw new NullReferenceException($"Valve {id} not found."); }

            return valve;
        }

        public List<Valve> GetValveConnections(string id)
        {
            return GetValve(id).Tunnels.Select(valve => GetValve(valve)).ToList();
        }

        public int FindDistance(string start, string end)
        {
            var walkQueue = new Queue<(string ValveId, int Distance)>();
            walkQueue.Enqueue((start, 0));

            while (walkQueue.Any())
            {
                (var valveId, int distance) = walkQueue.Dequeue();
                if (valveId.Equals(end)) { return distance; }

                foreach (var tunnel in GetValve(valveId).Tunnels)
                {
                    walkQueue.Enqueue((tunnel, distance + 1));
                }
            }

            return -1;
        }

        private Dictionary<(string, string), int> _distanceCache;
        public int FindDistanceMemo(string start, string end)
        {
            if (_distanceCache.TryGetValue((start, end), out var valve))
            {
                return valve;
            }
            else
            {
                var distance = FindDistance(start, end);
                _distanceCache.Add((start, end), distance);
                return distance;
            }
        }
    }

    public class Day16 : IPuzzleSolution
    {
        private Regex _valveIdRegex = new Regex(@"Valve\s([^\s]*)");
        private Regex _flowRateRegex = new Regex(@"rate=([\d]+)");
        private Regex _tunnelsRegex = new Regex(@"lead[s]? to valve[s]? (.*)");

        public void Run(string input)
        {
            var tunnels = new ValveTunnels();
            foreach (var valveString in input.Split("\r\n"))
            {
                var valveId = _valveIdRegex.Match(valveString).Groups[1].Value;
                var flowRate = int.Parse(_flowRateRegex.Match(valveString).Groups[1].Value);
                var valveTunnels = _tunnelsRegex.Match(valveString).Groups[1].Value.Split(", ").ToList();

                tunnels.AddValve(valveId, flowRate, valveTunnels);
            }


            int maximumPressure = 0;
            // It's not about finding the single optimal next valve, it's about finding the optimal path through

            var functionalValves = tunnels.Valves
                .Where(kvp => kvp.Value.FlowRate > 0)
                .Select(v => v.Value.Id)
                .ToList();



            maximumPressure = DoThing(tunnels, "AA", 30, functionalValves);

            var powersetCount = 1 << functionalValves.Count;
            List<List<string>> subsets = new List<List<string>>();
            for (int setMask = 0; setMask < powersetCount; setMask++)
            {
                List<string> subset = new List<string>();
                for (int i = 0; i < functionalValves.Count; i++)
                {
                    if ((setMask & (1 << i)) > 0)
                        subset.Add(functionalValves[i]);
                }
                subsets.Add(subset);
            }

            List<List<string>> complementSubsets = new List<List<string>>();
            foreach (var set in subsets)
            {
                List<string> complement = new List<string>(functionalValves);
                foreach (var valve in set)
                {
                    complement.Remove(valve);
                }
                complementSubsets.Add(complement);
            }

            var setPairs = subsets.Zip(complementSubsets).ToList();
            var maxWithElephant = 0;
            foreach (var tuple in setPairs)
            {
                var subset = tuple.First;
                var complement = tuple.Second;

                var pairPressure = DoThing(tunnels, "AA", 26, subset) + DoThing(tunnels, "AA", 26, complement);
                if (pairPressure > maximumPressure)
                    maxWithElephant = pairPressure;
            }

            Console.WriteLine($"Maximum Potential Pressure to be released: {maximumPressure}");
            Console.WriteLine($"Maximum Potential Pressure to be released with Elephant: {maxWithElephant}");
        }

        private int DoThing(ValveTunnels tunnels, string valveId, int timeRemaining, List<string> options)
        {
            return options
                .Select(option =>
                {
                    var distance = tunnels.FindDistanceMemo(valveId, option);
                    var timeRemainingAtValve = timeRemaining - distance;
                    if (timeRemainingAtValve <= 1) { return 0; }

                    var potentialPressure = tunnels.GetValve(option).PotentialPressure(timeRemainingAtValve);

                    var newOptions = new List<string>(options);
                    newOptions.Remove(option);

                    if (newOptions.Any())
                    {
                        return potentialPressure + DoThing(tunnels, option, timeRemainingAtValve - 1, newOptions);
                    }
                    else
                    {
                        return potentialPressure;
                    }
                })
                .OrderByDescending(x => x)
                .FirstOrDefault(0);
        }
    }
}
