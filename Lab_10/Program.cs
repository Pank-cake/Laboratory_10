using System;
using Компилятор;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var tests = new CompilerTests();
        tests.RunAllTests();

        Console.WriteLine("\nНажмите Enter, чтобы выйти...");
        Console.ReadLine();
    }
}
