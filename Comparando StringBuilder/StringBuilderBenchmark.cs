using System.Text;
using BenchmarkDotNet.Attributes;

namespace Domain.Benchmark;
[RankColumn]
[MemoryDiagnoser]
public class StringBuilderBenchmark
{
    [Benchmark]
    public string StringBuilderTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringConcatTest()
    {
        string textoOriginal = "Olá, ";
        string mundo = "Mundo";
        return textoOriginal + mundo;
    }

    [Benchmark]
    public string StringBuilderLoop1kTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 1000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringConcatLoop1kTest()
    {
        string textoOriginal = "Olá, ";
        string mundo = "Mundo";
        for (int i = 0; i < 1000; i++)
            textoOriginal += mundo;
        return textoOriginal;
    }

    [Benchmark]
    public string StringBuilderLoop10kTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 10000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringConcatLoop10kTest()
    {
        string textoOriginal = "Olá, ";
        string mundo = "Mundo";
        for (int i = 0; i < 10000; i++)
            textoOriginal += mundo;
        return textoOriginal;
    }

    
    [Benchmark]
    public string StringBuilderLoop100kTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 100000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringConcatLoop100kTest()
    {
        string textoOriginal = "Olá, ";
        string mundo = "Mundo";
        for (int i = 0; i < 100000; i++)
            textoOriginal += mundo;
        return textoOriginal;
    }
}