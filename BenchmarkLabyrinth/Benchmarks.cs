using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using System;
using Labyrinth;
namespace BenchmarkLabyrinth;

public class Benchmarks
{
    [Params(5, 32, 64)]
    public int N;
    [Benchmark]
    public void MatrixGeneration()
    {
        _ = new Labyrinth.Labyrinth(N,N,1..10);
    }

    [Benchmark]
    public void CreateAndSolve()
    {
        var lab = new Labyrinth.Labyrinth(N,N,1..10);
        _ = lab.Solutions;
    }

}
