using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokens
{
    /// <summary>
    /// Token for columns. Each column has to be mapped to object property.
    /// </summary>
    public class ColumnToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Column;

        /// <summary>
        /// Basic column info.
        /// </summary>
        public readonly ColumnInfo ColumnInfo;

        public ColumnToken(ColumnInfo columnInfo)
        {
            ColumnInfo = columnInfo;
        }

        public override string ToString()
        {
            return ColumnInfo.ColumnName;
        }
    }
}
