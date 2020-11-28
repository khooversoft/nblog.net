# Performance for Rule Implementation

One of the new features in C# 8.0 is the switch expression.  I have been using most of the new features
but I like this one the best.  This new C# feature was taken from F# or functional programming.

If you have not used this feature, see 
**<a href="https://github.com/dotnet/BenchmarkDotNet" target="_blank">C# 8.0 Switch Expression</a>**
for more details.

I was introduced to a kind of programming / test driven development exercise where a set of business rules are implemented
and test cases were written for. I was looking for a rule based logic pattens that I can used to compare with switch expression
and FizzBuzz seamed to fit.

FizzBuzz References...

- [Wikipedia](https://en.wikipedia.org/wiki/Fizz_buzz)
- [Tom Dalling](https://www.tomdalling.com/blog/software-design/fizzbuzz-in-too-much-detail/)
- [wiki.c2](https://wiki.c2.com/?FizzBuzzTest)


----------

#### Business Rules
FizzBuzz is a simple function that returns a specific value based an input after a set of rules have been evaluated.

 - if evenly divisible by 3 return "Fizz"
 - if evenly divisible by 5 return "Buzz"
 - if evenly divisible by 3 or 5 return "FizzBuzz"
 - If 0 or any other value return this number as string

#### Coding Strategies
Each of the below 6 strategies were tested.

1) If/else if logic
2) The new C# 8.0 "switch expression", pattern matching ("functional" programming)
3) A chain of handler, one handler for each rule, sometimes called chain of strategy
4) Code as data, lambda used for each rule
5) Code as data with Linq, lambda used for each rule

```
    The code is in this git repo.
```

---------------


### Results
BenchmarkDotNet was configured to track performance and memory and the following is the best 1 out of 3 runs.
The delta between each run was very small.

It appears that switch is just as fast as pure if/else logic, which was a little bit of a surprise, pleasant one.

 - If/Else and Switch are basically the same
 - Chain and Linq have the biggest error rates
 - Lambda and chain of handlers are about the same
 - Linq is showing its overhead but the least amount of instructions.


|                Method |       Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------- |-----------:|--------:|--------:|-------:|------:|------:|----------:|
|               If/Else |   111.5 ns | 0.26 ns | 0.25 ns | 0.0343 |     - |     - |     216 B |
|                Switch |   111.4 ns | 0.68 ns | 0.63 ns | 0.0343 |     - |     - |     216 B |
|                 Chain |   151.7 ns | 1.11 ns | 0.93 ns | 0.0534 |     - |     - |     336 B |
|         CodeAsDataFor |   161.5 ns | 0.14 ns | 0.12 ns | 0.0343 |     - |     - |     216 B |
|        CodeAsDataLinq | 1,135.2 ns | 1.60 ns | 1.33 ns | 0.2651 |     - |     - |    1672 B |

###### Code Size
Visual Studio 2019 has the ability us some to provide code metrics.  I am most interested in size of code,
coupling, and inheritance.  All of these test look about the same with regards to coupling and inheritance.
So this just leaves size of code.

|                Method | Lines of Executable Code | Percent Increase from Smallest |
|---------------------- |-------------------------:|------------------------------: |
|               If/Else |                       14 |                           % 14 |
|                Switch |                       17 |                           % 29 |
|                 Chain |                       45 |                           % 73 |
|         CodeAsDataFor |                       24 |                           % 50 |
|        CodeAsDataLinq |                       12 |                                |


--------------
### Code

###### If/else (fastest)
```
public string Evaluate(int value)
{
    if (value == 0) return value.ToString();
    else if (value % 3 == 0 && value % 5 == 0) return "FizzBuzz";
    else if (value % 3 == 0) return "Fizz";
    else if (value % 5 == 0) return "Buzz";

    return value.ToString();
}
```

###### C# switch expressions (fastest)
```
public string Evaluate(int value)
{
    return value switch
    {
        int v when v == 0 => value.ToString(),
        int v when v % 3 == 0 && v % 5 == 0 => "FizzBuzz",
        int v when v % 3 == 0 => "Fizz",
        int v when v % 5 == 0 => "Buzz",

        _ => value.ToString(),
    };
}
```

###### Chain of Handlers
```
public interface IFizzBuzzChain
{
    string Evaluate(int value);
}

public class FizzBuzzChain_End : IFizzBuzzChain
{
    public FizzBuzzChain_End() { }
    public string Evaluate(int value) => value.ToString();
}

public class FizzBuzzChain_Zero : IFizzBuzzChain
{
    private readonly IFizzBuzzChain _next;
    public FizzBuzzChain_Zero(IFizzBuzzChain next) => _next = next;
    public string Evaluate(int value) => value == 0 ? value.ToString() : _next.Evaluate(value);
}

public class FizzBuzzChain_Three : IFizzBuzzChain
{
    private readonly IFizzBuzzChain _next;
    public FizzBuzzChain_Three(IFizzBuzzChain next) => _next = next;
    public string Evaluate(int value) => value % 3 == 0 ? "Fizz" : _next.Evaluate(value);
}

public class FizzBuzzChain_Five : IFizzBuzzChain
{
    private readonly IFizzBuzzChain _next;
    public FizzBuzzChain_Five(IFizzBuzzChain next) => _next = next;
    public string Evaluate(int value) => value % 5 == 0 ? "Buzz" : _next.Evaluate(value);
}

public class FizzBuzzChain_ThreeFive : IFizzBuzzChain
{
    private readonly IFizzBuzzChain _next;
    public FizzBuzzChain_ThreeFive(IFizzBuzzChain next) => _next = next;
    public string Evaluate(int value) => value % 3 == 0 && value % 5 == 0 ? "FizzBuzz" : _next.Evaluate(value);
}

public class FuzzBuzzChain : IFizzBuzzChain
{
    private readonly IFizzBuzzChain _next;
    public FuzzBuzzChain() => _next = new FizzBuzzChain_Zero(
            new FizzBuzzChain_ThreeFive(
                new FizzBuzzChain_Three(
                    new FizzBuzzChain_Five(
                        new FizzBuzzChain_End()))));
    public string Evaluate(int value) => _next.Evaluate(value);
}
```

###### Code as Data
```
public class FizzBuzzCodeAsData
{
    private static Func<int, string>[] _evaulations = new Func<int, string>[]
    {
        x => x == 0 ? x.ToString() : null,
        x => x % 3 == 0 && x % 5 == 0 ? "FizzBuzz" : null,
        x => x % 3 == 0 ? "Fizz" : null,
        x => x % 5 == 0 ? "Buzz" : null,
        x => x.ToString(),
    };

    public FizzBuzzCodeAsData() { }

    public string Evaluate(int value)
    {
        for (int i = 0; i < _evaulations.Length; i++)
        {
            string result = _evaulations[i](value);
            if (result != null) return result;
        }

        throw new InvalidOperationException("should not be here");
    }
}
```

###### Test function
All the strategies use the same test function.

```
public void Test()
{
    var testData = new (int value, string expected)[]
    {
        (0, "0"),
        (1, "1"),
        (3, "Fizz"),
        (4, "4"),
        (5, "Buzz"),
        (15, "FizzBuzz"),
        (16, "16"),
    };

    var fizzBuzz = new FuzzBuzzChain();

    foreach (var test in testData)
    {
        string result = fizzBuzz.Evaluate(test.value);
        if (test.expected != result) throw new InvalidOperationException("Test failed");
    }
}
```
