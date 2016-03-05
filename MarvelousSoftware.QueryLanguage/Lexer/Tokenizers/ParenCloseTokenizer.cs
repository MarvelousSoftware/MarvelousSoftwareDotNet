using MarvelousSoftware.QueryLanguage.Lexer.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokenizers
{
    internal sealed class ParenCloseTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token) =>
            token == TokenType.Statement || token == TokenType.Literal || token == TokenType.Function || token == TokenType.ParenClose;

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            if (info.CurrentChar != info.Lang.Syntax.Config.ParenClose)
            {
                result.Parsed = false;
                var msg = $"\"{info.CurrentChar}\" is not a close parenthesis.";
                var error = new Error(msg, ErrorId.ParenCloseNotFound, ErrorType.Critical);
                result.Errors.Add(error);
                return result;
            }

            var token = new ParenCloseToken();
            token.SetPosition(info.CurrentPosition, 1);

            result.ParsedToken = token;

            if (info.OpenedParensCount < 1)
            {
                result.Parsed = false;
                result.Errors.Add("Open parenthesis not found.", ErrorId.ParenCloseWithoutParenOpen, ErrorType.Critical);
            }

            info.OpenedParensCount--;

            return result;
        }
    }
}