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

    class PacketItem
    {
        private int? _value;
        private List<PacketItem>? _items;

        public int? Value => _value;
        public List<PacketItem>? Items => _items;
        public ItemType ItemType => Value == null ? ItemType.List : ItemType.Number;

        public PacketItem(int value)
        {
            _value = value;
        }

        public PacketItem(List<PacketItem> items)
        {
            _items = items;
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
            var line = input.Split("\r\n").First();

            var close = line.LastIndexOf(']');
            var test = line.Substring(1, close - 1);

            var test2 = PacketItem.Parse(line);

            return;
        }
    }
}
