using Advent_Of_Code;
using Advent_Of_Code.Days;
using System.Reflection;

IPuzzleSolution puzzle;

Console.WriteLine("Please enter the day to run:");
Console.Write("Day");
string dayString = Console.ReadLine() ?? "";
Console.WriteLine();

var type = Assembly.GetExecutingAssembly().GetType($"Advent_Of_Code.Days.Day{dayString}");
if (type == null) return;

puzzle = (IPuzzleSolution)Activator.CreateInstance(type);

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Input\", $"{puzzle.GetType().Name}.txt");
string inputFile = System.IO.File.ReadAllText(filePath);

puzzle.Run(inputFile);