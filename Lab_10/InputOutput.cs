using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public struct TextPosition
    {
        public uint LineNumber { get; set; }
        public byte CharNumber { get; set; }

        public TextPosition(uint lineNumber = 0, byte charNumber = 0)
        {
            LineNumber = lineNumber;
            CharNumber = charNumber;
        }

        public override string ToString() => $"Строка: {LineNumber}, Позиция: {CharNumber}";
    }

    public struct Err
    {
        public TextPosition ErrorPosition { get; set; }
        public byte ErrorCode { get; set; }

        public Err(TextPosition errorPosition, byte errorCode)
        {
            ErrorPosition = errorPosition;
            ErrorCode = errorCode;
        }
    }

    public static class InputOutput
    {
        private const byte Errmax = 9;
        private static string _line = string.Empty;
        private static int _lastInLine = 0;
        private static uint _errCount = 0;
        private static bool _isEof = false;

        public static char Ch { get; set; }
        public static TextPosition PositionNow = new TextPosition(0, 0);
        public static List<Err> ErrList { get; set; } = new List<Err>();

        // Изменено на TextReader? для совместимости и устранения CS8618
        public static TextReader? File { get; set; }

        // Ручная таблица текстовых описаний ошибок
        private static readonly Dictionary<byte, string> ErrorMessages = new()
        {
            { 1, "Недопустимый символ в исходном коде Паскаля." },
            { 2, "Незавершенный строковый литерал." },
            { 3, "Ожидалось имя переменной или ключевое слово." },
            { 4, "Число константы вышло за допустимые пределы." }
        };

        /// <summary>
        /// Инициализация модуля перед началом компиляции.
        /// </summary>
        public static void Init(TextReader streamReader)
        {
            File = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
            PositionNow = new TextPosition(0, 0);
            _errCount = 0;
            _isEof = false;
            ErrList.Clear();

            ReadNextLine();
            if (!_isEof && _line != null && _line.Length > 0)
            {
                Ch = _line[0];
            }
        }

        /// <summary>
        /// Функция nextch — модуль ввода-вывода.
        /// </summary>
        public static void NextCh()
        {
            if (_isEof)
            {
                Ch = '\0';
                return;
            }

            if (PositionNow.CharNumber >= _lastInLine)
            {
                ListThisLine();
                if (ErrList.Count > 0)
                {
                    ListErrors();
                }

                ReadNextLine();

                if (_isEof)
                {
                    Ch = '\0';
                    return;
                }

                PositionNow.LineNumber++;
                PositionNow.CharNumber = 0;
            }
            else
            {
                PositionNow.CharNumber++;
            }

            Ch = _line[PositionNow.CharNumber];
        }

        private static void ListThisLine()
        {
            Console.WriteLine($"{PositionNow.LineNumber:D4} | {_line.TrimEnd('\n', '\r', ' ')}");
        }

        private static void ReadNextLine()
        {
            // Исправленное чтение через базовый TextReader
            string? nextLine = File?.ReadLine();

            if (nextLine != null)
            {
                _line = nextLine + " ";
                _lastInLine = _line.Length - 1;
                ErrList = new List<Err>();
            }
            else
            {
                _isEof = true;
                End();
            }
        }

        private static void End()
        {
            Console.WriteLine($"\n[Система]: Компиляция завершена. Всего ошибок обнаружено: {_errCount}!");
        }

        private static void ListErrors()
        {
            foreach (Err item in ErrList)
            {
                _errCount++;
                string s = "**";
                if (_errCount < 10) s += "0";
                s += $"{_errCount}**";

                int prefixOffset = 7;
                s = s.PadRight(prefixOffset + item.ErrorPosition.CharNumber);

                string msg = ErrorMessages.TryGetValue(item.ErrorCode, out var message)
                    ? message
                    : $"Неизвестная ошибка (код {item.ErrorCode})";

                s += $"^ Ошибка: {msg}";
                Console.WriteLine(s);
            }
            ErrList.Clear();
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (ErrList.Count <= Errmax)
            {
                var e = new Err(position, errorCode);
                ErrList.Add(e);
            }
        }
    }
}