using System.Text;
using BenchmarkDotNet.Attributes;

namespace Domain.Benchmark;
[MemoryDiagnoser]
public class StringBuilderArrayJoin
{
    private static string[] _dataList = 
    System.IO.File.ReadAllLines(@"randomString200-1.txt");

    [Benchmark]
    public string StringBuilderJoin()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendJoin("", _dataList);
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringJoin()
    {
        return String.Join("", _dataList);
    }

     [Benchmark]
    public string StringConcat()
    {
        return String.Concat(_dataList);
    }
}