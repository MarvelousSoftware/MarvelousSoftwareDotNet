using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.QueryLanguage.Lexer
{
    /// <summary>
    /// A wrapper for <see cref="ISyntaxConfig"/>.
    /// </summary>
    public class Syntax
    {
        public Syntax(ISyntaxConfig config)
        {
            Config = config;

            _compareOperators = Config.Keywords.Where(x => x.TokenType == TokenType.CompareOperator).ToArray();
            _logicalOperators = Config.Keywords.Where(x => x.TokenType == TokenType.LogicalOperator).ToArray();
            _statements = Config.Keywords.Where(x => x.TokenType == TokenType.Statement).ToArray();
            _functions = Config.Keywords.Where(x => x.TokenType == TokenType.Function).ToArray();
            _reservedChars = new[] { config.ParamsClose, config.ParamsOpen, config.ParenClose, config.ParenOpen };
        }

        public ISyntaxConfig Config { get; }

        private readonly Keyword[] _compareOperators;
        private readonly Keyword[] _logicalOperators;
        private readonly Keyword[] _statements;
        private readonly Keyword[] _functions;
        private readonly char[] _reservedChars;

        public IReadOnlyCollection<Keyword> CompareOperators => _compareOperators;
        public IReadOnlyCollection<Keyword> LogicalOperators => _logicalOperators;
        public IReadOnlyCollection<Keyword> Statements => _statements;
        public IReadOnlyCollection<Keyword> Functions => _functions;
        public IReadOnlyCollection<char> ReservedChars => _reservedChars;
    }
}
