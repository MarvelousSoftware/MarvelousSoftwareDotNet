﻿using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    /// <summary>
    /// Represents statements like "is empty" or "is not empty".
    /// </summary>
    public class StatementToken : KeywordTokenBase
    {
        public override TokenType TokenType => TokenType.Statement;

        public StatementToken(Keyword keyword) : base(keyword)
        {
        }
    }
}