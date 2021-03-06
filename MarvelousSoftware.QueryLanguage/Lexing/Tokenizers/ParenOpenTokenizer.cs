﻿using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    internal sealed class ParenOpenTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token) =>
            token == TokenType.ParenOpen || token == TokenType.LogicalOperator;

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            if (info.CurrentChar != info.Lang.SyntaxConfig.Config.ParenOpen)
            {
                result.Parsed = false;
                var msg = $"\"{info.CurrentChar}\" is not a open parenthesis.";
                var error = new Error(msg, ErrorId.ParenOpenNotFound, ErrorType.Critical);
                result.Errors.Add(error);
                return result;
            }

            var token = new ParenOpenToken();
            token.SetPosition(info.CurrentPosition, 1);

            result.ParsedToken = token;

            info.OpenedParensCount++;

            return result;
        }
    }
}