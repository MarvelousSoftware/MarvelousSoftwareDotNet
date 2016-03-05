using System.Globalization;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Provides base implementation of <see cref="ISyntaxConfig"/>.
    /// </summary>
    public static class ISyntaxConfigExtensions
    {
        /// <summary>
        /// Gets array of language specific special characters used at the start of keywords.
        /// Used in the parser in order to find an end of column name.
        /// </summary>
        public static char[] GetLanguageSpecialChars(this ISyntaxConfig config)
        {
            var fromKeywords = config.Keywords
                .Where(x => char.IsLetterOrDigit(x.Syntax[0]) == false)
                .Select(x => x.Syntax[0])
                .ToArray();

            var chars = new char[fromKeywords.Length + 3];

            for (int index = 0; index < fromKeywords.Length; index++)
            {
                chars[index] = fromKeywords[index];
            }

            chars[fromKeywords.Length] = config.ParenOpen;
            chars[fromKeywords.Length + 1] = config.ParenClose;
            chars[fromKeywords.Length + 2] = config.StringLiteralIdentifier;

            return chars;
        }

        /// <summary>
        /// Gets culture which should be used for all culture related topics.
        /// </summary>
        public static CultureInfo GetCultureInfo(this ISyntaxConfig config)
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = config.NumberDecimalSeparator.ToString();
            return culture;
        }
    }
}