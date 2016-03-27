using System.Collections.Generic;
using System.Linq;
using MarvelousSoftware.QueryLanguage.Lexing.Tokenizers.Abstract;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Tokenizers
{
    public class FunctionTokenizer<T> : ITokenizer<T>
    {
        public bool ShouldTokenizeAfter(TokenType? token) => token == TokenType.CompareOperator;

        public TokenizationResult Tokenize(LexerRuntimeInfo<T> info)
        {
            var result = new TokenizationResult();

            var reader = new QueryStringReader(info.Query, info.CurrentPosition)
            {
                ResetLengthOnEachRead = false
            };

            var syntax = info.Lang.SyntaxConfig.Config;
            var functionName = reader.ReadTill(x => x == syntax.ParamsOpen || x == syntax.StringLiteralIdentifier);

            if (reader.IsInRange() == false || reader.CurrentChar != syntax.ParamsOpen)
            {
                result.Parsed = false;
                // not a function, but probably not even intended to be function (params open indicator is missing)
                // therefore error is not added
                return result;
            }

            if (info.LastColumn == null)
            {
                // column has been not parsed therefore function won't work anyway
                result.Parsed = false;
                return result;
            }

            if (functionName.Any(char.IsWhiteSpace))
            {
                result.Parsed = false;
                result.Errors.Add($"\"{functionName}\" is not a function", ErrorId.InvalidFunctionName, ErrorType.Critical);
                return result;
            }

            // reads the parameters including open and close indicators
            var closeFound = false;
            var paramsScope = reader.ReadTill(x =>
            {
                if (closeFound) return true;
                if (x == syntax.ParamsClose) closeFound = true;
                return false;
            });
            
            if (paramsScope.Length == 0 || paramsScope[paramsScope.Length - 1] != syntax.ParamsClose)
            {
                result.Parsed = false;
                result.Errors.Add($"\"{functionName}\" function has unclosed parameters.", ErrorId.FunctionParamsCloseMissing, ErrorType.Critical);
                return result;
            }

            // TODO: case insensitive option
            var function = info.Lang.FunctionDefinitions.Get(functionName);
            if (function == null)
            {
                result.Parsed = false;
                result.Errors.Add($"\"{functionName}\" function doesn't exist.", ErrorId.FunctionNotDefined, ErrorType.Critical);
                result.Terminate = true;
                return result;
            }

            if (function.ColumnType != info.LastColumn.ColumnType)
            {
                result.Parsed = false;
                result.Errors.Add($"\"{functionName}\" function returns a value which is incompatible with \"{info.LastColumn.ColumnType}\" type.", ErrorId.FunctionReturnsIncompatibleType, ErrorType.Critical);
                result.Terminate = true;
                return result;
            }

            var parameters = new List<LiteralToken>();
            // TODO: implement parameters support
            /*var paramsInnerScope = paramsScope.Substring(1, paramsScope.Length - 2);
            if (paramsInnerScope.Any())
            {
                throw new NotImplementedException();
            }*/

            var token = new FunctionToken(functionName, parameters, function);
            token.SetPosition(info.CurrentPosition, reader.ReadLength);

            result.ParsedToken = token;

            return result;
        }
    }
}