using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    public class TokenizationResult
    {
        /// <summary>
        /// True if parsing has been successful; otherwise, false.
        /// </summary>
        public bool Parsed { get; set; } = true;

        /// <summary>
        /// Parsed expression. Might be null if <see cref="Parsed"/> is false.
        /// </summary>
        public TokenBase ParsedToken { get; set; }

        /// <summary>
        /// A list of errors.
        /// </summary>
        public ErrorList Errors { get; set; } = new ErrorList();

        /// <summary>
        /// If true then current token tokenization will be terminated and any further tokenizer won't be invoked.
        /// </summary>
        public bool Terminate { get; set; } = false;
    }
}