using Example;

namespace ExampleMain;

internal static class Program
{
    public static void Main(string[] _)
    {
        int a = 5;
        int b = 3;

        int sum = Func.Add(a, b);
        int difference = Func.Sub(a, b);

        Thread.Sleep(5000);

        Console.WriteLine($"Sum: {sum}");
        Console.WriteLine($"Difference: {difference}");
    }
}
