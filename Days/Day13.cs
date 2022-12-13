using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    enum ItemType
    {
        List,
        Number
    }

    class PacketItem : IComparable<PacketItem>
    {
        private int? _value;
        private List<PacketItem>? _items;

        public int? Value => _value;
        public List<PacketItem>? Items => _items;
        public ItemType ItemType => Value == null ? ItemType.List : ItemType.Number;
        public PacketItem AsList => ItemType == ItemType.List ? this : new PacketItem(new List<PacketItem>(){ new PacketItem((int)_value) } );

        public PacketItem(int value)
        {
            _value = value;
        }

        public PacketItem(List<PacketItem> items)
        {
            _items = items;
        }

        public bool Equals(PacketItem other)
        {
            if (ItemType != other.ItemType) { return false; }
            if (ItemType == ItemType.Number && other.ItemType == ItemType.Number)
            {
                return Value == other.Value;
            }
            else if (ItemType == ItemType.List && other.ItemType == ItemType.List)
            {
                if (Items.Count != other.Items.Count) { return false; }
                for (int i = 0; i < Items.Count; i++)
                {
                    return Items[i].Equals(other.Items[i]);
                }
            }

            return true;
        }

        public int CompareTo(PacketItem other)
        {
            if (ItemType == ItemType.Number && other.ItemType == ItemType.Number)
            {
                if (Value < other.Value) { return -1; }
                if (Value > other.Value) { return 1; }
                return 0;
            }
            else if (ItemType != other.ItemType)
            {
                return AsList.CompareTo(other.AsList);
            }

            // Both are Lists
            int compareIndex = 0;
            while (Items.Count > compareIndex && other.Items.Count > compareIndex)
            {
                var result = Items[compareIndex].CompareTo(other.Items[compareIndex]);
                if (result != 0)
                {
                    return result;
                }

                compareIndex++;
            }

            if (Items.Count < other.Items.Count) { return -1; }
            if (Items.Count > other.Items.Count) { return 1; }

            return 0;
        }

        public static PacketItem Parse(string packet)
        {
            PacketItem result;
            if (packet.StartsWith('['))
            {
                result = new PacketItem(new List<PacketItem>());

                var regex = new Regex(@"\[(?>\[(?<c>)|[^[\]]+|\](?<-c>))*(?(c)(?!))\]|[0-9]+");

                var substring = packet.Substring(1, packet.Length - 2);
                foreach (var match in regex.Matches(substring))
                {
                    result.Items.Add(PacketItem.Parse(match.ToString()));
                }
            }
            else
            {
                result = new PacketItem(int.Parse(packet));
            }

            return result;
        }
    }

    public class Day13 : IPuzzleSolution
    {
        public void Run(string input)
        {
            // var lines = input.Split("\r\n");
            var pairs = input.Split("\r\n\r\n");

            var index = 1;
            var sumOfIndices = 0;

            var watch = new List<int>() { 1, 2, 4, 6 };

            foreach (var pair in pairs)
            {
                var parsedPair = pair.Split("\r\n");
                var left = PacketItem.Parse(parsedPair[0]);
                var right = PacketItem.Parse(parsedPair[1]);

                if (left.CompareTo(right) < 0) 
                { 
                    sumOfIndices += index; 
                }

                index++;
            }

            Console.WriteLine("Part 1: ");
            Console.WriteLine($"Sum of indices: {sumOfIndices}");
            Console.WriteLine();


            var packets = input.Replace("\r\n\r\n", "\r\n").Split("\r\n").Select(packet => PacketItem.Parse(packet)).ToList();

            var dividerOne = PacketItem.Parse("[[2]]");
            var dividerTwo = PacketItem.Parse("[[6]]");

            packets.Add(dividerOne);
            packets.Add(dividerTwo);

            packets.Sort();
            var indexOne = packets.FindIndex(packet => packet.Equals(dividerOne)) + 1;
            var indexTwo = packets.FindIndex(packet => packet.Equals(dividerTwo)) + 1;

            Console.WriteLine("Part 2: ");
            Console.WriteLine($"The product of the indices is: {indexOne * indexTwo}");
        }
    }
}
