using System.Text;

namespace Labyrinth;

/// <summary>
/// The solution to the Labyrinth.
/// </summary>
public record struct Solution
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
            var sum = 0;
            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    sum += _matrix[row, col];
                }
            }
            _matrixAverage = sum / (numRows * numCols);
        }
    }

    private int _matrixAverage;


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
    private List<Solution> _solutions = null!;
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
        // set top left and bottom right to 0 as they are not part of the path
        Matrix[0, 0] = 0;
        Matrix[numRows - 1, numCols - 1] = 0;
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
        for (var row = 0; row < numRows; row++)
        {
            for (var col = 0; col < numCols; col++)
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
        var solution = Solutions[solutionIndex];
        var pathMatrix = new char[numRows, numCols];

        for (var row = 0; row < numRows; row++)
        {
            for (var col = 0; col < numCols; col++)
            {
                pathMatrix[row, col] = ' ';
            }
        }
        pathMatrix[numRows - 1, numCols - 1] = '0';
        var currentRow = 0;
        var currentCol = 0;

        foreach (var direction in solution.Path)
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

        var pathBuilder = new StringBuilder();

        for (var row = 0; row < numRows; row++)
        {
            for (var col = 0; col < numCols; col++)
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
        var randomArray = new int[height, width];
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                randomArray[i, j] = random.Next(range.Start.Value, range.End.Value);
            }
        }

        // set start and end to 0
        randomArray[0, 0] = 0;
        randomArray[height - 1, width - 1] = 0;
        return randomArray;
    }

    private bool IsValidCell(int row, int col, bool[,] visited)
    {
        return row >= 0 && row < numRows && col >= 0 && col < numCols && !visited[row, col];
    }

    /// <summary>
    /// Finds all solutions of the lowest cost from the top left corner to the bottom right corner of the matrix using A*.
    /// Modified to unvisit cells when starting a new solution search.
    /// </summary>
    public List<Solution> SolveAllLowestCost()
    {
        // List to hold possible lowest cost solutions.
        var solutions = new List<Solution>();

        // The current lowest cost. Initialized as the highest possible integer value.
        var lowestCost = int.MaxValue;

        // Arrays created to hold costs of visiting cells. Initialized to maximum int value.
        var gScore = new int[numRows, numCols];
        var fScore = new int[numRows, numCols];

        // An array to keep track of from where each cell was reached.
        var cameFrom = new (int, int)[numRows, numCols];

        // Initialize scores
        for (var i = 0; i < numRows; i++)
        {
            for (var j = 0; j < numCols; j++)
            {
                gScore[i, j] = int.MaxValue;
                fScore[i, j] = int.MaxValue;
            }
        }

        // Assign lower costs to start cell. 
        gScore[0, 0] = 0;
        fScore[0, 0] = CalculateHeuristic(0, 0);

        // Priority queue holds cells to be visited in order of priority, determined by fScore.
        PriorityQueue<(int, int), int> openSet = new();
        openSet.Enqueue((0, 0), fScore[0, 0]);

        while (openSet.Count != 0)
        {
            // Refresh the visited cells at the start of each new solution search.
            var visited = new bool[numRows, numCols];

            // Remove cell for visit
            var (currentRow, currentCol) = openSet.Dequeue();

            // If current cell is target, remember this path if it has the lowest cost found so far.
            if (currentRow == numRows - 1 && currentCol == numCols - 1)
            {
                var pathCost = gScore[currentRow, currentCol];

                if (pathCost <= lowestCost)
                {
                    if (pathCost < lowestCost)
                    {
                        // Found a new lowest cost, clear previous solutions
                        lowestCost = pathCost;
                        solutions.Clear();
                    }

                    // Reconstruct and add the current solution
                    var solution = ReconstructPath(cameFrom);
                    solutions.Add(solution);
                }
            }

            // If hasn't been visited, mark current cell as visited before exploring its neighbors.
            if (!visited[currentRow, currentCol])
            {
                visited[currentRow, currentCol] = true;

                // Explore all four directions around the current cell.
                for (var dir = 0; dir < 4; dir++)
                {
                    var newRow = currentRow + RowChange[dir];
                    var newCol = currentCol + ColChange[dir];

                    // If valid cell and not visited then consider the cell for movement.
                    if (IsValidCell(newRow, newCol, visited))
                    {
                        // Calculate tentative gScore for this neighbor cell
                        var tentativeGScore = gScore[currentRow, currentCol] + Matrix[newRow, newCol];

                        // Compare tentative gScore to current gScore. If less, this becomes the new path.
                        if (tentativeGScore <= gScore[newRow, newCol])
                        {
                            // Update path predecessor.
                            cameFrom[newRow, newCol] = (currentRow, currentCol);

                            // Update scores for this cell.
                            gScore[newRow, newCol] = tentativeGScore;
                            fScore[newRow, newCol] = gScore[newRow, newCol] + CalculateHeuristic(newRow, newCol);

                            // Add this cell to the openSet to be explored, if not already included.
                            if (!openSet.UnorderedItems.Select(x => x.Element).Contains((newRow, newCol)))
                            {
                                openSet.Enqueue((newRow, newCol), fScore[newRow, newCol]);
                            }
                        }
                    }
                }
            }
        }
        // Return the discovered solutions with the lowest cost.
        return solutions;
    }

    private Solution ReconstructPath((int, int)[,] cameFrom)
    {
        var currentRow = numRows - 1;
        var currentCol = numCols - 1;
        var sum = 0;
        var path = new List<char>();

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
        path.Reverse();
        return new Solution { Sum = sum, Path = path.ToArray() };
    }

    private int CalculateHeuristic(int rowIndex, int colIndex)
    {
        var dRow = Math.Abs(rowIndex - numRows);
        var dCol = Math.Abs(colIndex - numCols);
        var manhattanDistance = dRow + dCol;

        return manhattanDistance * _matrixAverage;
    }

    public string DisplaySolutions()
    {
        StringBuilder sb = new();
        for (var i = 0; i < Solutions.Count; i++)
        {
            sb.AppendLine($"Solution {i + 1}:");
            sb.AppendLine(DisplayPath(i));
            sb.AppendLine($"The sum of the values in the path is {Solutions[i].Sum}.");
            sb.AppendLine($"The shortened solution is {new string(Solutions[i].Path)}");
        }
        return sb.ToString();
    }
}


