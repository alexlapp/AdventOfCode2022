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
        private BigInteger? _rightOperand;

        public Operation(string operationString)
        {
            var operations = operationString.Trim().Split(" ").TakeLast(2).ToList();
            _operator = operations[0][0];
            _rightOperand = BigInteger.TryParse(operations[1], out BigInteger rightOperand) ? rightOperand : null;
        }

        public BigInteger Operate(BigInteger old)
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
        private List<BigInteger> _items;
        private Operation _operation;
        private BigInteger _testNumber;
        private int _trueTarget;
        private int _falseTarget;

        public BigInteger InspectionCount { get; set; }
        public int TargetMonkey => (_items.First() % _testNumber == 0) ? _trueTarget : _falseTarget;

        public Monkey(List<BigInteger> items, Operation operation, int trueTarget, int falseTarget, BigInteger testNumber)
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
            List<BigInteger> monkeyItems = new List<BigInteger>();

            var info = block.Split("\r\n");
            foreach(var word in info[1].Replace(",", "").Split(" "))
            {
                if (BigInteger.TryParse(word, out var item)) { monkeyItems.Add(item); }
            }

            var operation = new Operation(info[2]);
            
            var test = BigInteger.Parse(info[3].Split(" ").Last());
            var trueTarget = int.Parse(info[4].Split(" ").Last());
            var falseTarget = int.Parse(info[5].Split(" ").Last());

            return new Monkey(monkeyItems, operation, trueTarget, falseTarget, test);
        }

        public void InspectItem()
        {
            InspectionCount++;
            _items[0] = _operation.Operate(_items[0]);// / 3;
        }

        public bool HasItem()
        {
            return _items.Any();
        }

        public void ReceiveItem(BigInteger item)
        {
            _items.Add(item);
        }

        public BigInteger ThrowItem()
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

            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i);
                foreach (var monkey in monkeys)
                {
                    while (monkey.HasItem())
                    {
                        monkey.InspectItem();
                        monkeys[monkey.TargetMonkey].ReceiveItem(monkey.ThrowItem());
                    }
                }
            }

            var monkeyBusiness = monkeys
                .Select(m => m.InspectionCount)
                .OrderByDescending(ic => ic)
                .Take(2)
                .Aggregate((BigInteger)1, (acc, ic) => acc * ic);

            Console.WriteLine(monkeys.Select(x => x.InspectionCount).OrderByDescending(x => x).Select(x => x.ToString()).Aggregate("", (acc, t) => acc += ", " + t));

            Console.WriteLine($"Monkey Business value: {monkeyBusiness}");
        }
    }
}
