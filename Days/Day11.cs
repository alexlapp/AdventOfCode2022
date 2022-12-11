using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class Operation
    {
        public char _operator;
        private long? _rightOperand;

        public Operation(string operationString)
        {
            var operations = operationString.Trim().Split(" ").TakeLast(2).ToList();
            _operator = operations[0][0];
            _rightOperand = long.TryParse(operations[1], out long rightOperand) ? rightOperand : null;
        }

        public long Operate(long old)
        {
            switch (_operator)
            {
                case '+': return old + (_rightOperand ?? old);
                case '-': return old - (_rightOperand ?? old);
                case '*': return old * (_rightOperand ?? old);
                case '/': return old / (_rightOperand ?? old);
            }

            return old;
        }
    }

    class Monkey
    {
        private List<long> _items;
        private Operation _operation;
        private long _testNumber;
        private int _trueTarget;
        private int _falseTarget;

        public long InspectionCount { get; set; }
        public int TargetMonkey => (_items.First() % TestNumber == 0) ? _trueTarget : _falseTarget;

        public long TestNumber { get => _testNumber; }

        public Monkey(List<long> items, Operation operation, int trueTarget, int falseTarget, long testNumber)
        {
            _items = items;
            _operation = operation;
            _trueTarget = trueTarget;
            _falseTarget = falseTarget;
            _testNumber = testNumber;

            InspectionCount = 0;
        }

        public static Monkey Parse(string block)
        {
            List<long> monkeyItems = new List<long>();

            var info = block.Split("\r\n");
            foreach(var word in info[1].Replace(",", "").Split(" "))
            {
                if (long.TryParse(word, out var item)) { monkeyItems.Add(item); }
            }

            var operation = new Operation(info[2]);
            
            var test = long.Parse(info[3].Split(" ").Last());
            var trueTarget = int.Parse(info[4].Split(" ").Last());
            var falseTarget = int.Parse(info[5].Split(" ").Last());

            return new Monkey(monkeyItems, operation, trueTarget, falseTarget, test);
        }

        public void InspectItem(long lcm)
        {
            InspectionCount++;
            _items[0] = _operation.Operate(_items[0]) % lcm;// / 3;
        }

        public bool HasItem()
        {
            return _items.Any();
        }

        public void ReceiveItem(long item)
        {
            _items.Add(item);
        }

        public long ThrowItem()
        {
            var result = _items[0];
            _items.RemoveAt(0);
            return result;
        }
    }

    public class Day11 : IPuzzleSolution
    {
        public void Run(string input)
        {
            var monkeyInfo = input.Split("\r\n\r\n");
            var monkeys = new List<Monkey>();

            foreach (var m in monkeyInfo)
            {
                monkeys.Add(Monkey.Parse(m));
            }

            long lcm = monkeys.Aggregate((long)1, (acc, m) => acc * m.TestNumber);

            for (int i = 0; i < 10000; i++)
            {
                foreach (var monkey in monkeys)
                {
                    while (monkey.HasItem())
                    {
                        monkey.InspectItem(lcm);
                        monkeys[monkey.TargetMonkey].ReceiveItem(monkey.ThrowItem());
                    }
                }
            }

            var monkeyBusiness = monkeys
                .Select(m => m.InspectionCount)
                .OrderByDescending(ic => ic)
                .Take(2)
                .Aggregate((long)1, (acc, ic) => acc * ic);

            Console.WriteLine($"Monkey Business value: {monkeyBusiness}");
        }
    }
}
