using System.Text;
using BenchmarkDotNet.Attributes;

namespace Domain.Benchmark;
[MemoryDiagnoser]
public class StringBuilderBufferPerformance
{

    [Benchmark]
    public string StringBuilderLoop1kComBufferTest()
    {
        StringBuilder stringBuilder = new("Olá, ", 1000 * "Mundo".Length);
        for (int i = 0; i < 1000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringBuilderLoop1kSemBufferTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 1000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

        [Benchmark]
    public string StringBuilderLoop10kComBufferTest()
    {
        StringBuilder stringBuilder = new("Olá, ", 10000 * "Mundo".Length);
        for (int i = 0; i < 10000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringBuilderLoop10kSemBufferTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 10000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }
}