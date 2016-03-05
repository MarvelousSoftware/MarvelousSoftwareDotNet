using System.Collections.Generic;
using MarvelousSoftware.QueryLanguage.Lexing.Tokens.Abstract;

namespace MarvelousSoftware.QueryLanguage.Lexing
{
    public static class TokensExtensions
    {
        /// <summary>
        /// Finds token of given type starting from the end of a list.
        /// </summary>
        /// <param name="tokens">List of tokens.</param>
        /// <param name="deepLevel">Indicates how many tokens should be checked.</param>
        public static T LastOfType<T>(this List<TokenBase> tokens, int deepLevel) where T : TokenBase
        {
            var index = tokens.Count - 1;
            if (index < 0)
            {
                return null;
            }

            var current = tokens[index].As<T>();
            while (current == null)
            {
                deepLevel--;
                index--;
                if (index == -1 || deepLevel == 0)
                {
                    return null;
                }

                current = tokens[index].As<T>();
            }

            return current;
        }
    }
}