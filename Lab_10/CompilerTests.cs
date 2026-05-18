using System;
using System.IO;

namespace Компилятор
{
    public class CompilerTests
    {
        public void RunAllTests()
        {

            TestSuccessfulCodeReading();
            TestErrorReportingAndPositioning();

        }

        private void TestSuccessfulCodeReading()
        {
            Console.WriteLine("--- Тест 1: Чтение корректного кода Паскаля ---");

            string pascalCode = "program Test;\nbegin\nend.";

            using var stringReader = new StringReader(pascalCode);
            InputOutput.Init(stringReader);

            Console.WriteLine($"Начальный символ: '{InputOutput.Ch}' ({InputOutput.PositionNow})");

            InputOutput.NextCh(); // 'r'
            InputOutput.NextCh(); // 'o'
            InputOutput.NextCh(); // 'g'

            Console.WriteLine($"Символ после 3-х NextCh(): '{InputOutput.Ch}' ({InputOutput.PositionNow})");

            while (InputOutput.Ch != '\0')
            {
                InputOutput.NextCh();
            }
            Console.WriteLine();
        }

        private void TestErrorReportingAndPositioning()
        {
            Console.WriteLine("--- Тест 2: Имитация ошибок сканера и их вывод под символом ---");

            string faultyCode = "var x: integer;\nbegin\n  x := 5 @ 10;\nend.";

            using var stringReader = new StringReader(faultyCode);
            InputOutput.Init(stringReader);

            while (InputOutput.Ch != '\0')
            {
                if (InputOutput.Ch == '@')
                {
                    InputOutput.Error(1, InputOutput.PositionNow);
                }

                InputOutput.NextCh();
            }
        }
    }
}
