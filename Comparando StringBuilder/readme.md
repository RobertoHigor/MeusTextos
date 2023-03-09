# `StringBuilder` vs `String` para manipular strings

## Introdução

Nesse artigo vou explicar as vantagens e diferenças entre a classe `StringBuilder` e a concatenação de strings com "+". Por ser uma ferramenta muito utilizada no dia a dia, é importante entender as diferenças de como as duas funcionam internamente.

## Mutável vs Imutável

A classe `String` é do tipo imutável, ou seja, após criado seu valor não pode ser alterado. Apesar de parecer que ao adicionar uma string a outra existente, estamos apenas incrementando a variável anterior com mais valores, na realidade é alocado e criado uma nova variável na memória contendo a junção das duas strings, por exemplo:

```csharp
string textoOriginal = "Olá, ";
string mundo = "Mundo";
// Internamente está sendo criado uma nova string e 
// alocado na variável já existente.
textoOriginal += mundo;
```

> Internamente, o compilador converte a operação "+" em `String`.Concat

Para processos onde é realizado manipulações em string em um alto volume, isso pode resultar em um impacto tanto na performance quanto no uso de memória, por ser necessário uma nova alocação a cada concatenação, resultando também em chamadas excessivas ao Garbage Collector.

Para casos onde precisamos de uma variável mutável, a alternativa é a classe `StringBuilder` que, diferente da `String`, trabalha com dados mutáveis. Ou seja, uma instância do StringBuilder pode ser modificada (incrementadas, substituidas etc) sem precisar criar uma nova instância. Essa é sua principal vantagem, que é feito através do uso de um buffer interno, no qual permite com que o valor da string aumente e diminua.

### Buffer

Diferente da `String`, o `StringBuilder` trabalha com um buffer para acomodar as modificações. Caso o buffer se esgote, será alocado um buffer maior, copiando os dados do buffer antigo. A alocação padrão do StringBuilder comporta 16 caracteres, tendo o máximo o valor da constante Int32.MaxValue, que ao atigindo, resulta em um OutOfMemoryException.

Para reduzir a frequência no qual é alocado mais memória para o buffer interno, pode-se utilizar o construtor que recebe o tamanho da memória no qual queremos alocar (em caracteres)

```csharp
// Inicia stringbuilder com tamanho para 100 caracteres
var sb = new StringBuilder(100)
```

É dito que uma alocação exata pode trazer uma melhorias na performance e redução na alocação de memória, no entanto nos exemplos realizados a versão com Buffer após um certo volume de dados trouxe performance pior do que a versão sem buffer.

```csharp
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
```

|                            Method |      Mean |     Error |    StdDev |    Gen0 |    Gen1 |    Gen2 | Allocated |
|---------------------------------- |----------:|----------:|----------:|--------:|--------:|--------:|----------:|
|  StringBuilderLoop1kComBufferTest |  3.846 us | 0.0176 us | 0.0156 us |  1.8005 |  0.1144 |       - |  29.47 KB |
|  StringBuilderLoop1kSemBufferTest |  4.383 us | 0.0256 us | 0.0214 us |  1.6174 |  0.0916 |       - |   26.5 KB |
| StringBuilderLoop10kComBufferTest | 94.036 us | 0.4714 us | 0.4410 us | 62.3779 | 62.3779 | 62.3779 | 211.13 KB |
| StringBuilderLoop10kSemBufferTest | 70.643 us | 0.9460 us | 0.8386 us | 31.1279 | 31.1279 | 31.1279 | 208.57 KB |

Para a execução do método Append() mil vezes, a versão com buffer teve uma performance um pouco melhor. No entanto, para a versão de 10 mil vezes, a performance foi consideravelmente pior.

### AppendJoin vs Join

Ambas as classes possuem também métodos de Join, sendo StringBuilder.AppendJoin e String.Join. Para casos onde temos um array de strings, o string.Concat pode ser uma alternativa melhor por conseguir determinar o tamanho final da string ao examinar o array. 

Em casos onde é necessário chamar várias vezes (como em um loop), o AppendJoin consegue se sair melhor pela vantagem da imutabilidade.

O StringBuilder.AppendJoin é uma combinação entre o String.Join e o StringBuilder.Append, por chamar o Append internamente. Para cada valor, também é chamado o ToString(), o que pode ser ruim para valores que não são string (como int e decimal).

A comparação pode ser vista no benchmark abaixo:

```csharp
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
```

|            Method |     Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------------ |---------:|----------:|----------:|-------:|-------:|----------:|
| StringBuilderJoin | 4.041 us | 0.0410 us | 0.0383 us | 5.1193 | 0.8469 |  83.71 KB |
|        StringJoin | 2.363 us | 0.0460 us | 0.0644 us | 2.0523 |      - |  33.69 KB |
|      StringConcat | 1.686 us | 0.0127 us | 0.0119 us | 2.0523 |      - |  33.69 KB |

Nesse caso o String.Concat teve uma performance melhor (pelo fato do exemplo utilizar um array), além de alocar menos memoria do que o StringBuilder.Join. Em seguida vem o StringJoin.

Para os cenários de Array com tamanho fixo, o próprio String.Concat consegue se sair melhor.

## Quando utilizar

O `StringBuilder` é recomendado quando se utiliza um alto volume de concatenações de string, como por exemplo em um loop. [Segundo a microsoft](https://learn.microsoft.com/pt-br/dotnet/api/system.text.stringbuilder?view=net-7.0#the-string-and-stringbuilder-types), a classe `String` deve ser utilizada em casos de:

* Quando o número de modificações na string for pequeno;
* Quando é realizado um número fixo de concatenações, pois nesse caso o compilador pode combinar em uma única operação;
* Quando é necessário utilizar métodos de pesquisas como IndesOf ou StartsWith, pelo fato do `StringBuilder` não suportar sem antes o converter em `String`, o que negaria seus benefícios.

Já para o caso de uso do `StringBuilder`:

* Quando o código pode fazer um número desconhecido de modificações (como em loops);
* Quando o código fará um número significativo de alterações em uma string.


Ao realizar a concatenação de uma palavra 100 mil vezes, a vantagem do `StringBuilder` fica bem evidente:

```csharp
   [Benchmark]
    public string StringBuilderLoopTest()
    {
        StringBuilder stringBuilder = new("Olá, ");
        for (int i = 0; i < 100000; i++)
            stringBuilder.Append("Mundo");
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string StringConcatLoopTest()
    {
        string textoOriginal = "Olá, ";
        string mundo = "Mundo";
        for (int i = 0; i < 100000; i++)
            textoOriginal += mundo;
        return textoOriginal;
    }
```

|                Method |                 Mean |              Error |             StdDev |          Gen0 |          Gen1 |          Gen2 |     Allocated |
|---------------------- |---------------------:|-------------------:|-------------------:|--------------:|--------------:|--------------:|--------------:|
|     StringBuilderTest |            18.642 ns |          0.3487 ns |          0.3261 ns |        0.0091 |             - |             - |         152 B |
|      StringConcatTest |             8.216 ns |          0.0610 ns |          0.0571 ns |        0.0029 |             - |             - |          48 B |
| StringBuilderLoopTest |       699,651.201 ns |      2,971.4637 ns |      2,779.5089 ns |      249.0234 |      249.0234 |      249.0234 |     2013684 B |
|  StringConcatLoopTest | 5,013,108,142.857 ns | 24,203,777.6872 ns | 21,456,018.4848 ns | 11800000.0000 | 11779000.0000 | 11776000.0000 | 50007779072 B |

Além do menor tempo de execução e memória alocada, com o `StringBuilder` houve menos chamadas ao Garbage Collector Gen 0, devido ao fato da concatenação alocar mais memória ao criar novos objetos de string, resultando em mais chamadas ao GC Generation 0.

## Quando não utilizar

Para strings pequenas, o `StringBuilder` pode não trazer um ganho muito significativo de memória, além de que utilizar o "+=" nesses casos possuí uma sintaxe mais simples e rápida de se desenvolver. 

Outro caso é quando já existe um array de strings de tamanho definido, como citado no capítulo sobre Join, em que o String.Concat consegue determinar de uma vez o quanto é preciso alocado.

Exemplo de quando não utilizar:

```csharp
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Domain.Benchmark;
[MemoryDiagnoser]
public class StringBuilderBenchmark
{
    [Benchmark]
    public string StringBuilderTest()
    {
        `StringBuilder` stringBuilder = new("Olá, ");
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
}
```

Nesse caso, o `StringBuilder` além de ter sido mais lento, alocou mais memória do que a concatenação normal.

|            Method |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------ |----------:|----------:|----------:|-------:|----------:|
| StringBuilderTest | 18.626 ns | 0.1947 ns | 0.1726 ns | 0.0091 |     152 B |
|  StringConcatTest |  8.320 ns | 0.0479 ns | 0.0425 ns | 0.0029 |      48 B |

## Conclusão

Esse artigo buscou mostrar de forma breve as vantagens e quando utilizar a classe `StringBuilder`, sendo uma grande aliada na performance de sistemas que executem um grande volume de concatenações de string. Seu uso deve ser mais para casos onde não sabemos o tamanho da string, pela sua propriedade de ser mutável. Em casos em que sabemos o tamanho, como ao juntar um array de strings, o string.Concat consegue uma vantagem.

Seguem os resultados dos benchmarks com tamanhos de string variados:

|                    Method |                 Mean |              Error |             StdDev | Rank |          Gen0 |          Gen1 |          Gen2 |     Allocated |
|-------------------------- |---------------------:|-------------------:|-------------------:|-----:|--------------:|--------------:|--------------:|--------------:|
|         StringBuilderTest |            19.107 ns |          0.2487 ns |          0.2205 ns |    2 |        0.0091 |             - |             - |         152 B |
|          StringConcatTest |             8.414 ns |          0.1637 ns |          0.1531 ns |    1 |        0.0029 |             - |             - |          48 B |
|   StringBuilderLoop1kTest |         4,503.729 ns |         85.7993 ns |         80.2567 ns |    3 |        1.6174 |        0.0916 |             - |       27136 B |
|    StringConcatLoop1kTest |       161,308.281 ns |      3,194.8901 ns |      4,154.2595 ns |    5 |      301.2695 |        7.5684 |             - |     5040000 B |
|  StringBuilderLoop10kTest |        70,641.925 ns |        698.0930 ns |        652.9966 ns |    4 |       31.1279 |       31.1279 |       31.1279 |      213579 B |
|   StringConcatLoop10kTest |    20,383,109.233 ns |    400,692.3340 ns |    492,086.1706 ns |    7 |    64593.7500 |    46625.0000 |    43625.0000 |   500414677 B |
| StringBuilderLoop100kTest |       709,105.225 ns |      7,222.7404 ns |      6,402.7713 ns |    6 |      249.0234 |      249.0234 |      249.0234 |     2013684 B |
|  StringConcatLoop100kTest | 5,111,168,506.667 ns | 76,749,055.3313 ns | 71,791,113.9755 ns |    8 | 11783000.0000 | 11762000.0000 | 11759000.0000 | 50007763560 B |

## Fonte

<https://www.stevejgordon.co.uk/creating-strings-with-no-allocation-overhead-using-string-create-csharp>
<https://learn.microsoft.com/pt-br/dotnet/api/system.text.stringbuilder?view=net-7.0#the-string-and-stringbuilder-types>
<https://medium.com/c-sharp-progarmming/stringbuilder-vs-concatenation-in-net-b817ccec331e>
<https://www.macoratti.net/21/11/c_benchmark1.htm>
<https://www.oreilly.com/library/view/c-cookbook/0596003390/ch02s22.html>
<https://www.infoworld.com/article/3616600/when-to-use-string-vs-stringbuilder-in-net-core.html>
<https://davecallan.com/improve-performance-stringbuilder-dotnet-setting-capacity/>
<https://andrewlock.net/a-deep-dive-on-stringbuilder-part-2-appending-strings-built-in-types-and-lists/>