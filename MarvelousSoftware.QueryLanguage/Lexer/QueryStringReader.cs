using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.Lexer
{
    /// <summary>
    /// Provides methods which reads a query using different scenarios, e.g. till the end of whitespace, or till the first whitespace.
    /// Not thread safe.
    /// </summary>
    internal class QueryStringReader
    {
        public QueryStringReader(string query, int currentPosition)
        {
            CurrentPosition = currentPosition;
            Query = query;
            QueryLength = query.Length;
        }

        public string Query { get; private set; }
        public int CurrentPosition { get; private set; }

        /// <summary>
        /// If true then length is being reset on each `ReadSomething` method invokation.
        /// </summary>
        public bool ResetLengthOnEachRead { get; set; } = true;

        /// <summary>
        /// A number of the chars which has been read by 'ReadTill...' methods.
        /// </summary>
        public int ReadLength { get; private set; }

        private int QueryLength { get; set; }

        public char CurrentChar => Query[CurrentPosition];

        /// <summary>
        /// Reads the <see cref="Query"/> till first invokation of <see cref="predicate"/> with `true` as a result.
        /// </summary>
        public string ReadTill(Func<char, bool> predicate)
        {
            return ReadWhile(c => !predicate(c));
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till first non whitespace occurrence.
        /// </summary>
        public string ReadTillEndOfWhitespace()
        {
            return ReadWhile(Char.IsWhiteSpace);
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till first whitespace occurence.
        /// </summary>
        /// <returns></returns>
        public string ReadTillEndOfWord()
        {
            return ReadWhile(x => Char.IsWhiteSpace(x) == false);
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till end of number.
        /// </summary>
        /// <returns></returns>
        public string ReadTillEndOfNumber(char seperator)
        {
            return ReadWhile(x => Char.IsDigit(x) || x == seperator);
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till <see cref="x"/> whitespace occurences.
        /// </summary>
        /// <returns></returns>
        public string ReadTillEndOfXWords(int x)
        {
            var counter = 0;

            return ReadWhile(c =>
            {
                if (Char.IsWhiteSpace(c))
                {
                    counter++;
                }

                return counter < x;
            });
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till the first occurrance of invalid char.
        /// </summary>
        public string ReadTillIvalidChar(IEnumerable<char> invalidChars)
        {
            return ReadWhile(x => invalidChars.Contains(x) == false);
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till the first occurrance of invalid char.
        /// </summary>
        public string ReadTillIvalidCharOrWhitespace(IEnumerable<char> invalidChars)
        {
            return ReadWhile(x => invalidChars.Contains(x) == false && char.IsWhiteSpace(x) == false);
        }

        /// <summary>
        /// Reads the <see cref="Query"/> till the end of keyword.
        /// </summary>
        public string ReadTillEndOfKeyword(IEnumerable<Keyword> possibleKeyword, char[] specialChars, bool caseSensitive)
        {
            var result = string.Empty;
            var keywordNames = possibleKeyword.Select(x => x.Syntax).OrderByDescending(x => x.Length).ToList();

            foreach (var keyword in keywordNames)
            {
                var keywordName = caseSensitive == false ? keyword.ToLowerInvariant() : keyword;

                var prevIndex = CurrentPosition - 1;
                var nextIndex = CurrentPosition + keywordName.Length;
                var prevChar = prevIndex < 0 ? null as char? : Query[prevIndex];
                var nextChar = IsInRange(nextIndex) == false ? null as char? : Query[nextIndex];

                // keyword out of range of provided query
                if (IsInRange(nextIndex-1) == false)
                {
                   continue;
                }

                // keyword not found
                var queryPart = Query.Substring(CurrentPosition, keywordName.Length);
                queryPart = caseSensitive == false ? queryPart.ToLowerInvariant() : queryPart;
                if (keywordName != queryPart)
                {
                    continue;
                }

                // keyword starts with letter or digit and previous char isn't special character or whitespace
                // e.g. Namestarts with
                //          ^
                if (char.IsLetterOrDigit(keywordName.First()) && CurrentPosition > 0 
                    && char.IsWhiteSpace(prevChar.Value) == false
                    && specialChars.Contains(prevChar.Value) == false)
                {
                    continue;
                }

                // keyword ends with letter or digit and special character or whitespace after is missing
                // e.g. Name starts withDamian
                //           ^
                if (char.IsLetterOrDigit(keywordName.Last()) && IsInRange(nextIndex)
                    && char.IsWhiteSpace(nextChar.Value) == false
                    && specialChars.Contains(nextChar.Value) == false)
                {
                    continue;
                }

                result = keywordName;
                break;
            }

            ReadLength = result.Length;
            return result;
        }

        /// <summary>
        /// Checks wheter <see cref="CurrentPosition"/> is in range of <see cref="Query"/>.
        /// </summary>
        public bool IsInRange()
        {
            return IsInRange(CurrentPosition);
        }

        /// <summary>
        /// Checks wheter given <see cref="position"/> is in range of <see cref="Query"/>.
        /// </summary>
        public bool IsInRange(int position)
        {
            return QueryLength > position;
        }

        /// <summary>
        /// Checks wheter everything has been already read.
        /// </summary>
        public bool IsEndOfQuery() => IsInRange(CurrentPosition + 1) == false;

        /// <summary>
        /// Moves to next char in the <see cref="Query"/>.
        /// </summary>
        public bool MoveNext()
        {
            return MoveBy(1);
        }

        /// <summary>
        /// Moves <see cref="CurrentPosition"/> and <see cref="ReadLength"/> by <see cref="x"/>.
        /// </summary>
        private bool MoveBy(int x)
        {
            var newPosition = CurrentPosition + x;

            if (IsInRange(newPosition) == false)
            {
                return false;
            }

            CurrentPosition = newPosition;
            return true;
        }

        /// <summary>
        /// Reads query while predicates returns true.
        /// </summary>
        private string ReadWhile(Func<char, bool> predicate)
        {
            var result = new StringBuilder();

            if (ResetLengthOnEachRead)
            {
                ReadLength = 0;
            }

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop - CurrentChar is changed by MoveNext method
            while (predicate(CurrentChar))
            {
                result.Append(CurrentChar);
                ReadLength += 1;

                if (MoveNext() == false)
                {
                    break;
                }
            }

            return result.ToString();
        }
    }
}
