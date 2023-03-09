// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Domain.Benchmark;
Console.WriteLine("Iniciando benchmark");

//var resultado = BenchmarkRunner.Run<StringBuilderBenchmark>();
var resultado = BenchmarkRunner.Run<StringBuilderBufferPerformance>();
//var resultado = BenchmarkRunner.Run<StringBuilderArrayJoin>();