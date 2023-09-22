namespace Labyrinth;
/*
 * Labyrinth
 * The goal of this project is to find the shortest path(s) from the start to the end of a Matrix containing randomized integers from 1 to 9.
 * The path starts in the top left corner and ends in the bottom right corner.
 * The path can only move horizontally or vertically, not diagonally.
 * The path may not visit the same cell twice.
 * All possible paths of the minimum length shall be found.
 * The resulting path shall be shown graphically or by the directions taken using Unicode arrows (↑, ↓, ←, →).
 */
using System;
using System.Text;

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Creating the testing Labyrinth...");
        int[,] testMatrix =
        {
            {1, 3, 2, 5, 9},
            {6, 5, 1, 3, 3},
            {4, 2, 1, 4, 5},
            {8, 2, 8, 4, 1},
            {7, 1, 2, 2, 3},
        };
        var labyrinth = new Labyrinth(testMatrix);
        PrintLabyrinthAndSolutions(labyrinth);
        Console.WriteLine(@"ref: →→↓↓←↓↓→→→ and →→↓↓→↓→↓ ");
        Console.WriteLine("Creating a new random Labyrinth...");
        labyrinth = new();
        PrintLabyrinthAndSolutions(labyrinth);

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    static void PrintLabyrinthAndSolutions(Labyrinth labyrinth)
    {
        Console.WriteLine("The Labyrinth:");
        Console.WriteLine(labyrinth.DisplayMatrix());

        Console.WriteLine("The Solutions:");
        Console.WriteLine(labyrinth.DisplaySolutions());
    }
}
