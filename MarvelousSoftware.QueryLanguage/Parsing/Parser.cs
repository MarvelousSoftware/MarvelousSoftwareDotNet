using System;
using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions;
using MarvelousSoftware.QueryLanguage.Parsing.Expressions.Abstract;
using MarvelousSoftware.QueryLanguage.Parsing.Models;

namespace MarvelousSoftware.QueryLanguage.Parsing
{
    public class Parser : IParser
    {
        private readonly List<ExpressionVisitor> _visitors = new List<ExpressionVisitor>();

        public IParser AddVisitor(ExpressionVisitor visitor)
        {
            _visitors.Add(visitor);
            return this;
        }

        public ParsingResult Parse(TokenBase[] tokens)
        {
            if (tokens.Any() == false)
            {
                return new ParsingResult();
            }

            var result = new ParsingResult();
            var reversed = GetPostfixNotation(tokens);
            var expressions = new Stack<ExpressionBase>();

            for (var i = 0; i < reversed.Length; i++)
            {
                var token = reversed[i];

                switch (token.TokenType)
                {
                    case TokenType.CompareOperator:
                        var compareExpression = new CompareExpression(reversed[i - 2].As<ColumnToken>(),
                            token.As<CompareOperatorToken>(),
                            (IEvaluableToken)reversed[i - 1]);
                        expressions.Push(compareExpression);
                        NotifyVisitors(x => x.Visit(compareExpression));
                        result.Root = result.Root ?? compareExpression;
                        break;

                    case TokenType.Statement:
                        var statementExpression = new StatementExpression(reversed[i - 1].As<ColumnToken>(), token.As<StatementToken>());
                        expressions.Push(statementExpression);
                        NotifyVisitors(x => x.Visit(statementExpression));
                        result.Root = result.Root ?? statementExpression;
                        break;

                    case TokenType.LogicalOperator:
                        if (expressions.Count < 2)
                        {
                            var msg = $"Incomplete query. Right side of '{token}' is missing or is incomplete.";
                            result.Errors.Add(msg, ErrorId.LogicalOperatorRightOperandMissing, ErrorType.Critical);
                            return result;
                        }
                        var right = expressions.Pop();
                        var left = expressions.Pop();
                        var binaryExpression = new BinaryExpression(left, token.As<LogicalOperatorToken>(), right);
                        expressions.Push(binaryExpression);
                        NotifyVisitors(x => x.Visit(binaryExpression));
                        result.Root = binaryExpression;
                        break;

                    case TokenType.Column:
                    case TokenType.Literal:
                    case TokenType.Function:
                        continue;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (result.Root == null)
            {
                var msg = $"Incomplete query.";
                result.Errors.Add(msg, ErrorId.QueryWithoutEvenSingleExpression, ErrorType.Critical);
            }

            return result;
        }

        /// <remarks>
        /// (FN = 'D' and (LN = 'K' or (LN = 'Z' and A = 14)))
        /// FN 'D' = LN 'K' = LN 'Z' = A 14 = and or and
        /// 
        /// (FN = 'D' and((LN = 'Z' and A = 14) or LN = 'K'))
        /// FN 'D' = LN 'Z' = A 14 = and LN 'K' = or and
        /// 
        /// General idea is similar to Reversed Polish Notation.
        /// </remarks>
        private static TokenBase[] GetPostfixNotation(IReadOnlyCollection<TokenBase> tokens)
        {
            var precedences = new Dictionary<KeywordType, short>()
            {
                { KeywordType.Or, 0},
                { KeywordType.And, 1}
            };

            var output = new List<TokenBase>(tokens.Count);
            var stack = new Stack<TokenBase>();

            foreach (var token in tokens.Where(x => x.TokenType != TokenType.Whitespace))
            {
                switch (token.TokenType)
                {
                    case TokenType.Column:
                    case TokenType.Literal:
                    case TokenType.Function:
                    case TokenType.Statement:
                        output.Add(token);
                        if (stack.Any() && stack.Peek().TokenType == TokenType.CompareOperator)
                            output.Add(stack.Pop());
                        break;
                    case TokenType.CompareOperator:
                        stack.Push(token);
                        break;

                    case TokenType.LogicalOperator:
                        var tokenKeyword = token.As<LogicalOperatorToken>().KeywordType;
                        while (stack.Any() && stack.Peek().TokenType == TokenType.LogicalOperator)
                        {
                            var stackKeyword = stack.Peek().As<LogicalOperatorToken>().KeywordType;
                            if (precedences[tokenKeyword] > precedences[stackKeyword])
                            {
                                break;
                            }

                            output.Add(stack.Pop());
                        }

                        stack.Push(token);
                        break;

                    case TokenType.ParenOpen:
                        stack.Push(token);
                        break;
                    case TokenType.ParenClose:
                        while (stack.Any() && stack.Peek().TokenType != TokenType.ParenOpen)
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Pop();
                        break;
                    
                    default:
                        throw new NotSupportedException();
                }
            }

            while (stack.Count != 0)
            {
                output.Add(stack.Pop());
            }

            return output.ToArray();
        }

        private void NotifyVisitors(Action<ExpressionVisitor> action)
        {
            foreach (var visitor in _visitors)
            {
                action(visitor);
            }
        }
    }
}