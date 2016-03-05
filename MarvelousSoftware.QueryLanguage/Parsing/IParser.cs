using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing
{
    public interface IParser
    {
        /// <summary>
        /// Adds new visitor. It will be used while parsing with <see cref="Parser.Parse"/> method.
        /// </summary>
        IParser AddVisitor(ExpressionVisitor visitor);

        ParsingResult Parse(TokenBase[] tokens);
    }
}