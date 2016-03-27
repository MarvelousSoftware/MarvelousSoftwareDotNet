using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{ 
    internal sealed class LiteralTokenizer<T> : ITokenizer<T>
    {
        private readonly List<Func<LexerRuntimeInfo<T>, Result>> _parsingStrategies
            = new List<Func<LexerRuntimeInfo<T>, Result>>()
        {
            TryParseNull,
            TryParseDate,
            TryParseInteger,
            TryParseFloat,
            TryParseString
        };

        public bool ShouldTokenizeAfter(TokenType? token) => token == TokenType.CompareOperator;

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            var parsing = new Result();
            foreach (var parsingStrategy in _parsingStrategies)
            {
                parsing = parsingStrategy(info);
                if (parsing.Success)
                {
                    break;
                }
            }

            if (parsing.Success == false)
            {
                var word = new QueryStringReader(info.Query, info.CurrentPosition).ReadTillEndOfWord();
                var msg = $"\"{word}\" is not a valid value.";
                return new TokenizationResult()
                {
                    Parsed = false,
                    Errors = new ErrorList().Add(msg, ErrorId.InvalidLiteral, ErrorType.Critical)
                };
            }

            var token = new LiteralToken(parsing.Value, parsing.StartsWithIdendifier, parsing.EndsWithIdendifier);
            token.SetPosition(info.CurrentPosition, parsing.ReadLength);

            result.ParsedToken = token;

            return result;
        }

        private static Result TryParseString(LexerRuntimeInfo<T> info)
        {
            if (info.LastColumn == null || info.LastColumn.ColumnType != ColumnType.String)
            {
                return new Result();
            }

            var quotesCount = 0;
            var reader = new QueryStringReader(info.Query, info.CurrentPosition);
            string value;
            var startsWithIdentifier = false;
            var endsWithIdentifier = false;

            if (info.CurrentChar == info.Lang.SyntaxConfig.Config.StringLiteralIdentifier)
            {
                startsWithIdentifier = true;
                quotesCount = 1;
                reader.MoveNext();
                value = reader.ReadTillIvalidChar(new[] {info.Lang.SyntaxConfig.Config.StringLiteralIdentifier});
            }
            else
            {
                if (info.AllowedToBeGreedy == false)
                {
                    return new Result();
                }

                value = reader.ReadTillEndOfWord();

                if (value.Any(x => info.Lang.SyntaxConfig.ReservedChars.Contains(x)))
                {
                    return new Result();
                }
            }

            if (reader.ReadLength > 0 && reader.CurrentChar == info.Lang.SyntaxConfig.Config.StringLiteralIdentifier)
            {
                endsWithIdentifier = true;
                quotesCount++;
            }

            return new Result(value, reader.ReadLength + quotesCount, startsWithIdentifier, endsWithIdentifier);
        }

        private static Result TryParseDate(LexerRuntimeInfo<T> info)
        {
            if (info.LastColumn == null || info.LastColumn.ColumnType != ColumnType.DateTime)
            {
                return new Result();
            }

            foreach (var dateTimeFormat in info.Lang.SyntaxConfig.Config.DateTimeFormats)
            {
                var reader = new QueryStringReader(info.Query, info.CurrentPosition);

                // date formats may contain whitespaces (e.g. "d/M/yyyy mm:hh")
                // therefore reader has number of whitespaces + 1 words
                // ("d/M/yyyy mm:hh" - 1 whitespace, 2 words)
                var words = dateTimeFormat.Where(char.IsWhiteSpace).Count() + 1;

                var value = reader.ReadTillEndOfXWords(words);

                DateTime date;
                if (DateTime.TryParseExact(value, dateTimeFormat, info.Lang.SyntaxConfig.Config.GetCultureInfo(), DateTimeStyles.None, out date))
                {
                    return new Result(date, reader.ReadLength);
                }
            }

            return new Result();
        }

        private static Result TryParseNull(LexerRuntimeInfo<T> info)
        {
            var reader = new QueryStringReader(info.Query, info.CurrentPosition);

            var config = info.Lang.SyntaxConfig.Config;
            var word = reader.ReadTillEndOfWord();

            if (config.NullConstantCaseSensitive == false)
            {
                word = word.ToLowerInvariant();
            }

            var nullConstant = config.NullConstantCaseSensitive
                ? config.NullConstant
                : config.NullConstant.ToLowerInvariant();

            if (word != nullConstant)
            {
                return new Result();
            }

            return new Result(null, reader.ReadLength);
        }

        private static Result TryParseInteger(LexerRuntimeInfo<T> info)
        {
            if (info.LastColumn == null || info.LastColumn.ColumnType != ColumnType.Integer)
            {
                return new Result();
            }

            if (char.IsDigit(info.CurrentChar) == false)
            {
                return new Result();
            }

            var reader = new QueryStringReader(info.Query, info.CurrentPosition);

            var value = reader.ReadTillEndOfNumber(info.Lang.SyntaxConfig.Config.NumberDecimalSeparator);

            var type = info.LastColumn.SystemType;
            object number;

            if (LiteralParserHelper.TryParseInteger(value, type, out number))
            {
                return new Result(number, reader.ReadLength);
            }

            return new Result();
        }

        private static Result TryParseFloat(LexerRuntimeInfo<T> info)
        {
            if (info.LastColumn == null || info.LastColumn.ColumnType != ColumnType.Float)
            {
                return new Result();
            }

            if (char.IsDigit(info.CurrentChar) == false)
            {
                return new Result();
            }

            var reader = new QueryStringReader(info.Query, info.CurrentPosition);

            var value = reader.ReadTillEndOfNumber(info.Lang.SyntaxConfig.Config.NumberDecimalSeparator);

            var type = info.LastColumn.SystemType;
            var cultureInfo = info.Lang.SyntaxConfig.Config.GetCultureInfo();
            object number;

            if (LiteralParserHelper.TryParseFloat(value, type, cultureInfo, out number))
            {
                return new Result(number, reader.ReadLength);
            }

            return new Result();
        }

        private class Result
        {
            public readonly bool Success;

            public readonly object Value;

            public readonly int ReadLength;

            public readonly bool StartsWithIdendifier;

            public readonly bool EndsWithIdendifier;

            public Result()
            {
            }

            public Result(object value, int readLength, bool startsWithIdendifier = true, bool endsWithIdendifier = true)
            {
                Value = value;
                ReadLength = readLength;
                Success = true;

                StartsWithIdendifier = startsWithIdendifier;
                EndsWithIdendifier = endsWithIdendifier;
            }
        }
    }
}