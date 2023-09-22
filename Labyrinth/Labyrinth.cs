using System.Text;

namespace Labyrinth;

/// <summary>
/// The solution to the Labyrinth.
/// </summary>
public struct Solution
{
    /// <summary>
    /// The sum of the values in the cells of the matrix that are part of the path.
    /// </summary>
    public int Sum;
    /// <summary>
    /// The path from the top left corner to the bottom right corner of the matrix represented by Unicode arrows (↑, ↓, ←, →).
    /// </summary>
    public char[] Path;
}

public class Labyrinth
{
    private static Random _random = new();

    /// <summary>
    /// Represents changes in row indices moving in the same direction as ColChange.
    /// </summary>
    private static readonly int[] RowChange = { -1, 1, 0, 0 };
    /// <summary>
    /// Represents changes in column indices when moving in the same direction as RowChange.
    /// </summary>
    private static readonly int[] ColChange = { 0, 0, -1, 1 };

    private const char Up = '↑';
    private const char Down = '↓';
    private const char Left = '←';
    private const char Right = '→';
    /// <summary>
    /// Represents the Unicode arrows for the directions in RowChange and ColChange.
    /// </summary>
    private static char[] _arrowChars = { Up, Down, Left, Right };

    public int[,] Matrix
    {
        get => _matrix;
        set
        {
            _matrix = value;
            _solved = false;
        }
    }

    public List<Solution> Solutions
    {
        get
        {
            if (!_solved)
            {
                _solutions = SolveAllLowestCost();
                _solved = true;
            }
            return _solutions;
        }
    }

    private bool _solved = false;
    private List<Solution> _solutions;
    private int[,] _matrix = null!;

    private int numRows => Matrix.GetLength(0);
    private int numCols => Matrix.GetLength(1);



    /// <summary>
    /// Creates a new Labyrinth with a matrix of size 5x5 with random values between 1 and 9.
    /// </summary>
    public Labyrinth()
    {
        Matrix = GenerateRandomized2DMatrix(5, 5, 1..10, _random);
    }

    /// <summary>
    /// Creates a new Labyrinth with a custom matrix.
    /// </summary>
    /// <param name="matrix"></param>
    public Labyrinth(int[,] matrix)
    {
        Matrix = matrix;
    }
    /// <summary>
    /// Creates a new Labyrinth with a matrix of size sideLength x sideLength with random values from the range.
    /// </summary>
    /// <param name="sideLength"></param>
    /// <param name="range"></param>
    public Labyrinth(int width, int height, Range range)
    {
        Matrix = GenerateRandomized2DMatrix(width, height, range, _random);
    }

    public string DisplayMatrix()
    {
        StringBuilder sb = new();
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                sb.Append($"{Matrix[row, col]} ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// Returns a string with the same lines as DisplayMatrix, but with the path marked with the arrows of the direction the path took.
    /// </summary>
    /// <returns></returns>
    public string DisplayPath(int solutionIndex)
    {
        Solution solution = Solutions[solutionIndex];
        char[,] pathMatrix = new char[numRows, numCols];

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                pathMatrix[row, col] = ' ';
            }
        }
        pathMatrix[numRows - 1, numCols - 1] = '0';
        int currentRow = 0;
        int currentCol = 0;

        foreach (char direction in solution.Path)
        {
            pathMatrix[currentRow, currentCol] = direction;

            switch (direction)
            {
                case Up:
                    currentRow--;
                    break;
                case Down:
                    currentRow++;
                    break;
                case Left:
                    currentCol--;
                    break;
                case Right:
                    currentCol++;
                    break;
            }
        }

        StringBuilder pathBuilder = new StringBuilder();

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                pathBuilder.Append(pathMatrix[row, col]);
                pathBuilder.Append(' ');
            }
            pathBuilder.AppendLine();
        }

        return pathBuilder.ToString();
    }


    private static int[,] GenerateRandomized2DMatrix(int width, int height, Range range, Random random)
    {
        int[,] randomArray = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                randomArray[i, j] = random.Next(range.Start.Value, range.End.Value);
            }
        }
        return randomArray;
    }

    private bool IsValidCell(int row, int col, bool[,] visited)
    {
        return row >= 0 && row < numRows && col >= 0 && col < numCols && !visited[row, col];
    }


    /// <summary>
    /// Finds the shortest path from the top left corner to the bottom right corner of the matrix using aStar.
    /// </summary>
    //private Solutions Solve()
    //{
    //    // Create data structures
    //    bool[,] visited = new bool[numRows, numCols];
    //    int[,] gScore = new int[numRows, numCols];
    //    int[,] fScore = new int[numRows, numCols];
    //    (int, int)[,] cameFrom = new (int, int)[numRows, numCols];

    //    // Initialize scores
    //    for (int i = 0; i < numRows; i++)
    //    {
    //        for (int j = 0; j < numCols; j++)
    //        {
    //            gScore[i, j] = int.MaxValue;
    //            fScore[i, j] = int.MaxValue;
    //        }
    //    }

    //    gScore[0, 0] = 0;
    //    fScore[0, 0] = CalculateHeuristic(0, 0);

    //    PriorityQueue<(int, int), int> openSet = new ();
    //    openSet.Enqueue((0, 0), fScore[0, 0]);

    //    while (openSet.Count != 0)
    //    {
    //        var (currentRow, currentCol) = openSet.Dequeue();

    //        if (currentRow == numRows - 1 && currentCol == numCols - 1)
    //        {
    //            return ReconstructPath(cameFrom);
    //        }

    //        visited[currentRow, currentCol] = true;

    //        for (int dir = 0; dir < 4; dir++)
    //        {
    //            int newRow = currentRow + RowChange[dir];
    //            int newCol = currentCol + ColChange[dir];

    //            if (IsValidCell(newRow, newCol, visited))
    //            {
    //                int tentativeGScore = gScore[currentRow, currentCol] + Matrix[newRow, newCol];

    //                if (tentativeGScore < gScore[newRow, newCol])
    //                {
    //                    cameFrom[newRow, newCol] = (currentRow, currentCol);
    //                    gScore[newRow, newCol] = tentativeGScore;
    //                    fScore[newRow, newCol] = gScore[newRow, newCol] + CalculateHeuristic(newRow, newCol);

    //                    if (!openSet.UnorderedItems.Select(x => x.Element).Contains((newRow, newCol)))
    //                    {
    //                        openSet.Enqueue((newRow, newCol), fScore[newRow, newCol]);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // If no path is found, throw an exception
    //    throw new InvalidOperationException("No path found");
    //}

    /// <summary>
    /// Finds all solutions of the lowest cost from the top left corner to the bottom right corner of the matrix using A*.
    /// </summary>
    public List<Solution> SolveAllLowestCost()
    {
        List<Solution> solutions = new List<Solution>();
        int lowestCost = int.MaxValue;

        // Create data structures
        bool[,] visited = new bool[numRows, numCols];
        int[,] gScore = new int[numRows, numCols];
        int[,] fScore = new int[numRows, numCols];
        (int, int)[,] cameFrom = new (int, int)[numRows, numCols];

        // Initialize scores
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                gScore[i, j] = int.MaxValue;
                fScore[i, j] = int.MaxValue;
            }
        }

        gScore[0, 0] = 0;
        fScore[0, 0] = CalculateHeuristic(0, 0);

        PriorityQueue<(int, int), int> openSet = new();
        openSet.Enqueue((0, 0), fScore[0, 0]);

        while (openSet.Count != 0)
        {
            var (currentRow, currentCol) = openSet.Dequeue();

            if (currentRow == numRows - 1 && currentCol == numCols - 1)
            {
                int pathCost = gScore[currentRow, currentCol];

                if (pathCost <= lowestCost)
                {
                    if (pathCost < lowestCost)
                    {
                        // Found a new lowest cost, clear previous solutions
                        lowestCost = pathCost;
                        solutions.Clear();
                    }

                    // Reconstruct and add the current solution
                    Solution solution = ReconstructPath(cameFrom);
                    solutions.Add(solution);
                }
            }

            visited[currentRow, currentCol] = true;

            for (int dir = 0; dir < 4; dir++)
            {
                int newRow = currentRow + RowChange[dir];
                int newCol = currentCol + ColChange[dir];

                if (IsValidCell(newRow, newCol, visited))
                {
                    int tentativeGScore = gScore[currentRow, currentCol] + Matrix[newRow, newCol];

                    if (tentativeGScore < gScore[newRow, newCol])
                    {
                        cameFrom[newRow, newCol] = (currentRow, currentCol);
                        gScore[newRow, newCol] = tentativeGScore;
                        fScore[newRow, newCol] = gScore[newRow, newCol] + CalculateHeuristic(newRow, newCol);

                        if (!openSet.UnorderedItems.Select(x => x.Element).Contains((newRow, newCol)))
                        {
                            openSet.Enqueue((newRow, newCol), fScore[newRow, newCol]);
                        }
                    }
                }
            }
        }

        return solutions;
    }

    private Solution ReconstructPath((int, int)[,] cameFrom)
    {
        int currentRow = numRows - 1;
        int currentCol = numCols - 1;
        int sum = 0;
        List<char> path = new List<char>();

        while (currentRow != 0 || currentCol != 0)
        {
            var (parentRow, parentCol) = cameFrom[currentRow, currentCol];

            if (parentRow == currentRow - 1)
            {
                path.Add(Down);
            }
            else if (parentRow == currentRow + 1)
            {
                path.Add(Up);
            }
            else if (parentCol == currentCol - 1)
            {
                path.Add(Right);
            }
            else if (parentCol == currentCol + 1)
            {
                path.Add(Left);
            }
            sum += Matrix[parentRow, parentCol];
            currentRow = parentRow;
            currentCol = parentCol;
        }
        sum -= Matrix[0, 0];
        path.Reverse();
        return new Solution { Sum = sum, Path = path.ToArray() };
    }

    private int CalculateHeuristic(int rowIndex, int colIndex)
    {
        int d_row = Math.Abs(rowIndex - numRows);
        int d_col = Math.Abs(colIndex - numCols);
        int manhattan_distance = d_row + d_col;

        // Add the cell cost to the manhattan distance
        // Assume that the cost of each cell is some positive number, stored in _matrix[rowIndex, colIndex].
        return manhattan_distance + _matrix[rowIndex, colIndex];
    }

    public string DisplaySolutions()
    {
        StringBuilder sb = new();
        for (int i = 0; i < Solutions.Count; i++)
        {
            sb.AppendLine($"Solution {i + 1}:");
            sb.AppendLine(DisplayPath(i));
            sb.AppendLine($"The sum of the values in the path is {Solutions[i].Sum}.\n");
        }
        return sb.ToString();
    }
}
