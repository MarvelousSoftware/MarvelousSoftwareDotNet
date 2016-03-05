using System.Collections.Generic;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Configuration of syntax.
    /// </summary>
    public interface ISyntaxConfig
    {
        /// <summary>
        /// Collection of all available keywords. 
        /// </summary>
        IEnumerable<Keyword> Keywords { get; }

        /// <summary>
        /// If true then column searching will be case sensitive.
        /// </summary>
        bool ColumnNameCaseSensitive { get; }

        /// <summary>
        /// If true then all keywords will be paresed using case sensitive comparison.
        /// </summary>
        bool KeywordCaseSensitive { get; }

        /// <summary>
        /// Date formats.
        /// </summary>
        IEnumerable<string> DateTimeFormats { get; }

        /// <summary>
        /// A constant which represents a null value.
        /// </summary>
        string NullConstant { get; }

        /// <summary>
        /// Allows to configure if <see cref="NullConstant"/> is parsed with case sensivity.
        /// </summary>
        bool NullConstantCaseSensitive { get;}

        /// <summary>
        /// Open parenthesis char.
        /// </summary>
        char ParenOpen { get; }

        /// <summary>
        /// Close parenthesis char.
        /// </summary>
        char ParenClose { get; }

        /// <summary>
        /// Used for parsing strings. Each string has to start and end with this char.
        /// </summary>
        char StringLiteralIdentifier { get; }

        /// <summary>
        /// Number decimal separator. Used while parsing decimal numbers.
        /// </summary>
        char NumberDecimalSeparator { get; }

        /// <summary>
        /// Start of function's parameters.
        /// </summary>
        char ParamsOpen { get; set; }

        /// <summary>
        /// End of function's parameters.
        /// </summary>
        char ParamsClose { get; set; }
    }
}
