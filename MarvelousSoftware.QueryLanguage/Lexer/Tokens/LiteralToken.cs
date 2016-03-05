using System;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
{
    /// <summary>
    /// A token which describes literal value.
    /// </summary>
    public class LiteralToken : TokenBase, IEvaluableToken
    {
        public override TokenType TokenType => TokenType.Literal;

        /// <summary>
        /// Value of a literal.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Type of <see cref="Value"/>.
        /// </summary>
        public Type Type => Value?.GetType();

        /// <summary>
        /// If true then token's text starts with identifier.
        /// </summary>
        public readonly bool StartsWithIdentifier;

        /// <summary>
        /// If true then token's text ends with identifier.
        /// </summary>
        public readonly bool EndsWithIdentifier;

        public LiteralToken(object value, bool startsWithIdentifier = false, bool endsWithIdentifier = false)
        {
            Value = value;
            StartsWithIdentifier = startsWithIdentifier;
            EndsWithIdentifier = endsWithIdentifier;
        }

        public object Evaluate()
        {
            return Value;
        }

        public override string GetSliceOfText(string query, int length)
        {
            if (Type == typeof (string))
            {
                var text = Value.ToString();

                if (StartsWithIdentifier)
                {
                    length--;
                }
                if(EndsWithIdentifier && text.Length < length)
                {
                    length--;
                }
                return text.Substring(0, length);

            }

            return base.GetSliceOfText(query, length);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
