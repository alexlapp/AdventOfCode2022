using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    record MonkeyOperation(string LeftName, char Operator, string RightName);

    class MathMonkey
    {
        public string Name { get; set; }
        public long? Number { get; set; }
        public MonkeyOperation? Operation { get; set; }

        public MathMonkey(string name, long number)
        {
            Name = name;
            Number = number;
        }

        public MathMonkey(string name, MonkeyOperation operation)
        {
            Name = name;
            Operation = operation;
        }

        public long GetValue(Dictionary<string, MathMonkey> lookup)
        {
            if (Number != null) { return (long)Number; }

            var value = CalcValue(lookup);
            Number = value;

            return value;
        }

        public long CalcValue(Dictionary<string, MathMonkey> lookup)
        {
            var leftValue = lookup[Operation.LeftName].GetValue(lookup);
            var rightValue = lookup[Operation.RightName].GetValue(lookup);

            switch (Operation.Operator)
            {
                case '+':
                    return leftValue + rightValue;
                case '-':
                    return leftValue - rightValue;
                case '*':
                    return leftValue * rightValue;
                case '/':
                    return leftValue / rightValue;
            }

            return 0;
        }

        public bool DoesTreeContainMonkey(Dictionary<string, MathMonkey> lookup, string name)
        {
            if (Name.Equals(name)) { return true; }

            if (Operation != null)
            {
                return lookup[Operation.LeftName].DoesTreeContainMonkey(lookup, name) 
                    || lookup[Operation.RightName].DoesTreeContainMonkey(lookup, name);
            }

            return false;
        }

        public long? ReverseCalc(Dictionary<string, MathMonkey> lookup, string target, long targetEval)
        {
            if (Name == target) { return targetEval; }
            if (Operation == null) { return null; }

            var IsOnLeft = lookup[Operation.LeftName].DoesTreeContainMonkey(lookup, target);
            if (IsOnLeft)
            {
                var rightValue = lookup[Operation.RightName].GetValue(lookup);
                long newTarget = 0;

                switch (Operation.Operator)
                {
                    case '+':
                        newTarget = targetEval - rightValue;
                        break;
                    case '-':
                        newTarget = targetEval + rightValue;
                        break;
                    case '*':
                        newTarget = targetEval / rightValue;
                        break;
                    case '/':
                        newTarget = targetEval * rightValue;
                        break;
                }

                return lookup[Operation.LeftName].ReverseCalc(lookup, target, newTarget);
            }

            var IsOnRight = lookup[Operation.RightName].DoesTreeContainMonkey(lookup, target);
            if (IsOnRight)
            {
                var leftValue = lookup[Operation.LeftName].GetValue(lookup);
                long newTarget = 0;

                switch (Operation.Operator)
                {
                    case '+':
                        newTarget = targetEval - leftValue;
                        break;
                    case '-':
                        newTarget = leftValue - targetEval;
                        break;
                    case '*':
                        newTarget = targetEval / leftValue;
                        break;
                    case '/':
                        newTarget = leftValue / targetEval;
                        break;
                }

                return lookup[Operation.RightName].ReverseCalc(lookup, target, newTarget);
            }

            return null;
        }

        public static MathMonkey Parse(string monkey)
        {
            var sides = monkey.Split(":").Select(m => m.Trim()).ToList();

            if (long.TryParse(sides[1], out var number))
            {
                return new MathMonkey(sides[0], number);
            }

            var operationValues = sides[1].Split(" ").ToList();
            var operation = new MonkeyOperation(operationValues[0], operationValues[1][0], operationValues[2]);
            return new MathMonkey(sides[0], operation);
        }
    }

    public class Day21 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var monkeys = new Dictionary<string, MathMonkey>();

            foreach (var monkeyString in input.Split("\r\n"))
            {
                var monkey = MathMonkey.Parse(monkeyString);
                monkeys.Add(monkey.Name, monkey);
            }

            Console.WriteLine($"Root monkey value: {monkeys["root"].GetValue(monkeys)}");

            var leftMonkey = monkeys["root"].Operation.LeftName;
            var rightMonkey = monkeys["root"].Operation.RightName;

            var isRootOnLeft = monkeys[leftMonkey].DoesTreeContainMonkey(monkeys, "humn");
            var humanSide = isRootOnLeft ? monkeys[leftMonkey] : monkeys[rightMonkey];
            var targetValue = isRootOnLeft ? monkeys[rightMonkey].GetValue(monkeys) : monkeys[leftMonkey].GetValue(monkeys);

            Console.WriteLine($"Human should equal: {humanSide.ReverseCalc(monkeys, "humn", targetValue)}");
        }
    }
}
