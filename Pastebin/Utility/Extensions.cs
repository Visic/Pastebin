using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pastebin {
    public static class StringExtensions {
        public static string Replace_IgnoreCase(this string target, string oldValue, string newValue) {
            return Regex.Replace(target, oldValue, newValue, RegexOptions.IgnoreCase);
        }

        public static void Println(this string target, ConsoleColor color) {
            Print(target, color);
            Console.WriteLine();
        }

        public static void Println(this string target) {
            Print(target);
            Console.WriteLine();
        }

        public static void Println(this string target, string formatString, ConsoleColor color) {
            Println(string.Format(formatString, target), color);
        }

        public static void Println(this string target, string formatString) {
            Println(string.Format(formatString, target));
        }

        public static void Print(this string target, ConsoleColor color) {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(target);
            Console.ForegroundColor = previousColor;
        }

        public static void Print(this string target) {
            Console.Write(target);
        }

        public static void Print(this string target, string formatString, ConsoleColor color) {
            Print(string.Format(formatString, target), color);
        }

        public static void Print(this string target, string formatString) {
            Print(string.Format(formatString, target));
        }

        public static void PrintToLocation(this string target, int left, int top) {
            PrintToLocation(target, left, top, Console.ForegroundColor);
        }

        public static void PrintToLocation(this string target, int left, int top, string formatString, ConsoleColor color) {
            PrintToLocation(string.Format(formatString, target), left, top, color);
        }

        public static void PrintToLocation(this string target, int left, int top, ConsoleColor color) {
            var currentLeft = Console.CursorLeft;
            var currentTop = Console.CursorTop;

            Console.SetCursorPosition(left, top);
            target.Print(color);
            Console.SetCursorPosition(currentLeft, currentTop);
        }
    }

    public static class DictionaryExtensions {
        public static Option<VT> TryGetValue<KT, VT>(this Dictionary<KT, VT> src, KT key) {
            VT val = default(VT);
            if (src.TryGetValue(key, out val)) return new Option<VT>(val);
            else return new Option<VT>();
        }
    }

    public static class IEnumerableExtensions {
        public delegate bool AccPred<T, AccT>(T ele, ref AccT acc);

        public static string ToStr(this IEnumerable<char> src) {
            return new string(src.ToArray());
        }

        public static IEnumerable<RT> TraversingSelect<T, RT>(this IEnumerable<IEnumerable<T>> src, Func<IEnumerable<T>, RT> transform, T padValue) {
            int i = 0;
            while (true) {
                if (!src.Any(x => x.Skip(i).HasNext())) yield break;

                var eles = src.Select(x => x.Skip(i).DefaultIfEmpty(padValue).First());
                yield return transform(eles);
                ++i;
            }
        }

        public static IEnumerable<T> MoveLast<T>(this IEnumerable<T> src, Func<T, bool> predicate) {
            return src.Where(x => !predicate(x)).Concat(src.Where(predicate));
        }

        public static bool HasNext(this IEnumerable src) {
            return src.GetEnumerator().MoveNext();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> src, T defaultValue) {
            return src.HasNext() ? src.Single() : defaultValue;
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> src, int index, Func<T, T> oldValToNewVal) {
            var parts = SplitBefore_FromStart(src, (T e, ref int i) => i++ == index, 0, 1);
            return parts.First().Append(oldValToNewVal(src.ElementAt(index))).Concat(parts.Last().Skip(1));
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> src, int index, T newValue) {
            var parts = SplitBefore_FromStart(src, (T e, ref int i) => i++ == index, 0, 1);
            return parts.First().Append(newValue).Concat(parts.Last().Skip(1));
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> src, int index, T value) {
            var parts = SplitBefore_FromStart(src, (T e, ref int i) => i++ == index, 0, 1);
            return parts.First().Append(value).Concat(parts.Last());
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> src, Func<T, bool> predicate, T value) {
            foreach (var ele in src) {
                if (predicate(ele)) yield return value;
                yield return ele;
            }
            yield break;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> src, params T[] args) {
            return src.Concat(args ?? new T[0]);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> src, params T[] args) {
            return (args ?? new T[0]).Concat(src);
        }

        public static IEnumerable<T> SkipEnd<T>(this IEnumerable<T> src, int numberToSkip) {
            return src.TakeWhile((x, i) => i < src.Count() - numberToSkip);
        }

        public static void ForEach<T>(this IEnumerable<T> src, Action<T> action) {
            foreach (var ele in src) {
                action(ele);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> action) {
            int i = 0;
            foreach (var ele in src) {
                action(ele, i++);
            }
        }

        public static IEnumerable<RT> Collate<T, RT>(this IEnumerable<T> src, Func<T, RT> transform) {
            return src.Aggregate(new List<RT>(), (acc, x) => {
                acc.Add(transform(x));
                return acc;
            });
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> src, string delimiter) {
            return string.Join(delimiter, src);
        }

        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> src, params T[] args) {
            if (args == null || args.Count() == 0) return src;
            return src.Where(x => !args.Contains(x));
        }

        public static IEnumerable<string> PrependEach(this IEnumerable<string> src, string str) {
            return src.Select(x => str + x);
        }

        public static IEnumerable<string> AppendEach(this IEnumerable<string> src, string str) {
            return src.Select(x => x + str);
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromStart<T>(this IEnumerable<T> src, Func<T, bool> predicate, int numberOfSplits = int.MaxValue) {
            return src.Split(predicate, numberOfSplits, false);
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromStart<T>(this IEnumerable<T> src, Func<T, bool> predicate, int numberOfSplits = int.MaxValue) {
            return src.Split(predicate, numberOfSplits, true);
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromEnd<T>(this IEnumerable<T> src, Func<T, bool> predicate, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitBefore_FromStart(predicate, numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromEnd<T>(this IEnumerable<T> src, Func<T, bool> predicate, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitAfter_FromStart(predicate, numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromStart<T, AccT>(this IEnumerable<T> src, AccPred<T, AccT> predicate, AccT acc, int numberOfSplits = int.MaxValue) {
            return src.Split(predicate, acc, numberOfSplits, false);
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromStart<T, AccT>(this IEnumerable<T> src, AccPred<T, AccT> predicate, AccT acc, int numberOfSplits = int.MaxValue) {
            return src.Split(predicate, acc, numberOfSplits, true);
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromEnd<T, AccT>(this IEnumerable<T> src, AccPred<T, AccT> predicate, AccT acc, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitBefore_FromStart(predicate, acc, numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromEnd<T, AccT>(this IEnumerable<T> src, AccPred<T, AccT> predicate, AccT acc, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitAfter_FromStart(predicate, acc, numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromStart<T>(this IEnumerable<T> src, T splitOnVal, int numberOfSplits = int.MaxValue) {
            return src.Split(x => x.Equals(splitOnVal), numberOfSplits, false);
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromStart<T>(this IEnumerable<T> src, T splitOnVal, int numberOfSplits = int.MaxValue) {
            return src.Split(x => x.Equals(splitOnVal), numberOfSplits, true);
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter_FromEnd<T>(this IEnumerable<T> src, T splitOnVal, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitAfter_FromStart(x => x.Equals(splitOnVal), numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore_FromEnd<T>(this IEnumerable<T> src, T splitOnVal, int numberOfSplits = int.MaxValue) {
            return src.Reverse().SplitBefore_FromStart(x => x.Equals(splitOnVal), numberOfSplits).Select(x => x.Reverse()).Reverse();
        }

        private static IEnumerable<IEnumerable<T>> Split<T, AccT>(this IEnumerable<T> src, AccPred<T, AccT> predicate, AccT acc, int numberOfSplits, bool before) {
            var offset = before ? 0 : 1;
            var startIndex = 0;
            var curIndex = 0;
            var splitsMade = 0;

            foreach (var ele in src) {
                if (predicate(ele, ref acc)) {
                    yield return src.Skip(startIndex).Take(curIndex - startIndex + offset);
                    startIndex = curIndex + offset;

                    if (++splitsMade == numberOfSplits) {
                        yield return src.Skip(startIndex);
                        yield break;
                    }
                }

                ++curIndex;
            }

            if (startIndex != curIndex) yield return src.Skip(startIndex).Take(curIndex - startIndex + offset);

            yield break;
        }

        private static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> src, Func<T, bool> predicate, int numberOfSplits, bool before) {
            return Split(src, (T e, ref int i) => predicate(e), 0, numberOfSplits, before);
        }
    }
}