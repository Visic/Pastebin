using System;
using System.Collections.Generic;

namespace Pastebin {
    public static class Utility {
        public static string GetPassword(string prompt) {
            Console.Write(prompt);
            var originalLeft = Console.CursorLeft;
            var index = 0;
            var chars = new List<char>();
            Action eraseLastChar = () => " ".PrintToLocation(originalLeft + chars.Count, Console.CursorTop);

            while (true) {
                var key = Console.ReadKey(true);
                switch (key.Key) {
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    return chars.ToStr();
                case ConsoleKey.Backspace:
                    if (index == 0) break;

                    chars.RemoveAt(chars.Count - 1);
                    --index;
                    eraseLastChar();
                    break;
                case ConsoleKey.LeftArrow:
                    if (index == 0) break;
                    --index;
                    break;
                case ConsoleKey.RightArrow:
                    if (index == chars.Count) break;
                    ++index;
                    break;
                case ConsoleKey.Delete:
                    if (index == chars.Count) break;

                    chars.RemoveAt(index);
                    eraseLastChar();
                    break;
                default:
                    chars.Add(key.KeyChar);
                    ++index;
                    Console.Write('*');
                    break;
                }

                Console.SetCursorPosition(originalLeft + index, Console.CursorTop);
            }
        }
    }
}