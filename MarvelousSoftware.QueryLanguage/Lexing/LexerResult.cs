using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    /// <summary>
    /// Result of tokenization.
    /// </summary>
    public class LexerResult
    {
        public LexerResult()
        {
            Errors = new ErrorList();
            Tokens = new List<TokenBase>();;
        }

        /// <summary>
        /// List of errors which occurred while tokenizing.
        /// </summary>
        public ErrorList Errors { get; private set; }

        /// <summary>
        /// List of successfully created tokens.
        /// </summary>
        public List<TokenBase> Tokens { get; private set; }
    }
}