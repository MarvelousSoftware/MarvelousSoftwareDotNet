using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Lexer.Functions;
using MarvelousSoftware.QueryLanguage.Lexer.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexer.Tokens
{
    public class FunctionToken : TokenBase, IEvaluableToken
    {
        public override TokenType TokenType { get; } = TokenType.Function;

        /// <summary>
        /// Name of the function.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Parameters used along with function call.
        /// </summary>
        public IEnumerable<LiteralToken> Parameters { get; }

        /// <summary>
        /// Function assosicated with token.
        /// </summary>
        public FunctionDefinition Function { get; }

        public FunctionToken(string name, IEnumerable<LiteralToken> parameters, FunctionDefinition function)
        {
            Name = name;
            Parameters = parameters;
            Function = function;
        }

        public object Evaluate() => Function.Evaluator();

        public override string ToString()
        {
            return Name + "()";
        }
    }
}