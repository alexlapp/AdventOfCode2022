using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent_Of_Code.Days
{
    class Folder
    {
        public string Name { get; set; }
        public List<File> Files{ get; set; }
        public List<Folder> SubFolders { get; set; }
        public Folder? Parent { get; set; }

        public int Size => Files.Aggregate(0, (acc, item) => acc + item.Size) + SubFolders.Aggregate(0, (acc, item) => acc + item.Size);

        public Folder(string name, Folder? parent = null)
        {
            Name = name;
            Files = new List<File>();
            SubFolders = new List<Folder>();
            Parent = parent;
        }

        public void AddFolder(string name)
        {
            SubFolders.Add(new Folder(name, this));
        }

        public void AddItem(string name, int size)
        {
            Files.Add(new File(name, size, this));
        }

        public Folder? GetFolder(string name)
        {
            return SubFolders.Find((s) => s.Name.Equals(name));
        }
    }

    class File
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public Folder Parent { get; set; }

        public File(string name, int size, Folder parent)
        {
            Name = name;
            Size = size;
            Parent = parent;
        }
    }

    public class Day7 : IPuzzleSolution
    {
        private int SumFolders(List<Folder> folders)
        {
            var result = 0;
            foreach (var folder in folders)
            {
                int folderSize = folder.Size;
                if (folderSize <= 100000) { result += folderSize; }

                if (folder.SubFolders.Count > 0)
                {
                    result += SumFolders(folder.SubFolders);
                }
            }

            return result;
        }

        private Folder FindSmallestFolder(Folder testee, Folder current, int neededSpace)
        {
            Folder result = current;

            if (testee.Size >= neededSpace && testee.Size < current.Size)
            {
                result = testee;
            }

            if (testee.SubFolders.Count > 0)
            {
                foreach (var folder in testee.SubFolders)
                {
                    result = FindSmallestFolder(folder, result, neededSpace);
                }
            }

            return result;
        }

        public void Run(string input)
        {
            var root = new Folder("/");
            Folder current = root;

            foreach (var line in input.Split("\r\n"))
            {
                if (line.StartsWith("$"))
                { // Handle commands
                    var commands = line.Substring(2).Split(" ");
                    if (commands[0].Equals("cd"))
                    {
                        if (commands[1].Equals("/"))
                        {
                            current = root;
                        }
                        else if (commands[1].Equals(".."))
                        {
                            current = current.Parent;
                        }
                        else
                        {
                            current = current.GetFolder(commands[1]);
                        }
                    }
                }
                else
                { // Handle output
                    var itemData = line.Split(" ");
                    if (itemData[0].Equals("dir"))
                    {
                        current.AddFolder(itemData[1]);
                    }
                    else
                    {
                        current.AddItem(itemData[1], int.Parse(itemData[0]));
                    }
                }
            }

            int sum = SumFolders(root.SubFolders);
            Console.WriteLine($"Sum of folders under size 100000: {sum}");

            const int totalSpace  = 70000000;
            const int neededSpace = 30000000;

            int spaceToFree = neededSpace - (totalSpace - root.Size);
            Folder folderToDelete = FindSmallestFolder(root, root, spaceToFree);
            Console.WriteLine($"Size of smallest folder that can be deleted: {folderToDelete.Size}");
        }
    }
}
