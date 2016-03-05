using System;

namespace MarvelousSoftware.QueryLanguage
{
    public class QueryLanguageException : Exception
    {
        public QueryLanguageException(string message) : base(message)
        {
        }
    }
}