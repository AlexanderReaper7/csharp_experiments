namespace TestLabyrinth;
using Labyrinth;
public class LabyrinthTests
{
    private static readonly List<Solution> TestSolutions = new()
    {
        new Solution { Sum = 16, Path = "→→↓↓←↓↓→→→".ToCharArray()},
        new Solution { Sum = 16, Path = "→→↓↓→↓→↓".ToCharArray()},
    };

    private static readonly int[,] TestMatrix =
    {
        {1, 3, 2, 5, 9},
        {6, 5, 1, 3, 3},
        {4, 2, 1, 4, 5},
        {8, 2, 8, 4, 1},
        {7, 1, 2, 2, 3},
    };
    Labyrinth labyrinth = new(TestMatrix);
    List<Solution> solutions;

    public LabyrinthTests()
    {
        solutions = labyrinth.Solutions;
    }

    [Fact]
    public void TwoSolutions()
    {
        Assert.Equal(2, solutions.Count);
    }

    [Fact]
    public void CorrectSums()
    {
        Assert.True(solutions.All(s => s.Sum == 16));
    }

    [Fact]
    public void CorrectPaths()
    {
        Console.WriteLine($"sol1:{solutions[0].Path.ToString()}");
        Console.WriteLine($"sol2:{solutions[1].Path.ToString()}");

        Assert.Contains(solutions.Select(x => x.Path), s => s == TestSolutions[0].Path);
        Assert.Contains(solutions.Select(x => x.Path), s => s == TestSolutions[1].Path);
    }
}
