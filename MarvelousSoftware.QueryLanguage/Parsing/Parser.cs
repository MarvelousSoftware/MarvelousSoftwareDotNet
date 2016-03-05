using System;
using System.Collections.Generic;
using System.Linq;
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
            var reversed = GetReversedAwesomeNotation(tokens);
            var expressions = new Stack<ExpressionBase>();

            for (int i = 0; i < reversed.Length; i++)
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
        /// General idea: 
        /// 1. Similar to Reversed Polish Notation
        /// 2. Stacks for logical operators and other
        /// 3. Logical operators poped on paren close
        /// </remarks>
        private static TokenBase[] GetReversedAwesomeNotation(IReadOnlyCollection<TokenBase> tokens)
        {
            var logicalOperatorsStack = new Stack<TokenBase>();
            var final = new TokenBase[tokens.Count];

            var i = 0;
            var operands = 0;
            foreach (var token in tokens)
            {
                while (final[i] != null)
                {
                    i++;
                }

                if (operands == 2)
                {
                    final[i] = logicalOperatorsStack.Pop();
                    i++;
                    operands = 1;
                }

                switch (token.TokenType)
                {
                    case TokenType.Column:
                        final[i] = token;
                        break;
                    case TokenType.Literal:
                    case TokenType.Function:
                    case TokenType.Statement:
                        final[i] = token;
                        operands++;
                        break;
                    case TokenType.CompareOperator:
                        final[i + 1] = token;
                        i--;
                        break;
                    case TokenType.ParenClose:
                        operands = 1;
                        continue;
                    case TokenType.LogicalOperator:
                        logicalOperatorsStack.Push(token);
                        continue;
                    case TokenType.ParenOpen:
                        operands = 0;
                        continue;
                    case TokenType.Whitespace:
                        continue;
                    default:
                        throw new NotSupportedException();
                }

                i++;
            }

            while (logicalOperatorsStack.Count != 0)
            {
                while (final[i] != null)
                {
                    i++;
                }

                final[i] = logicalOperatorsStack.Pop();
                i++;
            }

            var total = 0;
            while (final.Length > total && final[total] != null)
            {
                total++;
            }
            return new ArraySegment<TokenBase>(final, 0, total).ToArray();
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