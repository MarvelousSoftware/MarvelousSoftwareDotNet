using System;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Lexing.Functions
{
    public class FunctionDefinition
    {
        public string Name { get; }

        /// <summary>
        /// Function can be applied only to columns with proper type.
        /// </summary>
        public ColumnType ColumnType { get; }

        /// <summary>
        /// Evaluator should create value of a function invokation. It should be compatible with
        /// column type.
        /// </summary>
        public Func<object> Evaluator { get; }

        public FunctionDefinition(string name, ColumnType columnType, Func<object> evaluator)
        {
            ColumnType = columnType;
            Evaluator = evaluator;
            Name = name;
        }
    }
}