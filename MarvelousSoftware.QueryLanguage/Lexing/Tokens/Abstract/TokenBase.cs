namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract
{
    /// <summary>
    /// A base class for all tokens.
    /// </summary>
    public abstract class TokenBase
    {
        /// <summary>
        /// Type of a token. Each token should have their own <see cref="TokenType"/>. 
        /// Allows to ensure token type without using casting.
        /// </summary>
        public abstract TokenType TokenType { get; }

        /// <summary>
        /// Start index in the query.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// Length of token in the query.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// End index in the query.
        /// </summary>
        public int EndPosition => StartPosition + Length;

        private bool _positionSet;

        public void SetPosition(int startPosition, int length)
        {
            if (_positionSet)
            {
                throw new QueryLanguageException("Possition already set. Tokens are immutable structures and cannot be changed.");
            }

            StartPosition = startPosition;
            Length = length;
            _positionSet = true;
        }

        public T As<T>() where T : TokenBase
        {
            return this as T;
        }

        public string GetText(string query)
        {
            return query.Substring(StartPosition, Length);
        }

        /// <summary>
        /// Gets text skiping things like quotes.
        /// </summary>
        public virtual string GetSliceOfText(string query, int length)
        {
            return GetText(query).Substring(0, length);
        }
    }
}
