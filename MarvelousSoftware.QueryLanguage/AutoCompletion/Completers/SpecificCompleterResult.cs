using System;

namespace MarvelousSoftware.QueryLanguage.AutoCompletion.Completers
{
    public class SpecificCompleterResult
    {
        public readonly string Value;
        public readonly bool QuotesIfContainsWhiteSpaces;

        public SpecificCompleterResult(string value, bool quotesIfContainsWhiteSpaces = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException();
            }

            Value = value;
            QuotesIfContainsWhiteSpaces = quotesIfContainsWhiteSpaces;
        }

        public string GetText(char quote)
        {
            if (!QuotesIfContainsWhiteSpaces)
            {
                return Value;
            }

            return Value.Contains(" ") ? $"{quote}{Value}{quote}" : Value;
        }
    }
}