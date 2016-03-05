using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;

namespace MarvelousSoftware.QueryLanguage.Parsing.Models
{
    /// <summary>
    /// Result of parsing.
    /// </summary>
    public class ParsingResult
    {
        /// <summary>
        /// Root of the Abstract Syntax Tree.
        /// </summary>
        public ExpressionBase Root { get; set; }

        public ErrorList Errors { get; set; } = new ErrorList();
    }
}