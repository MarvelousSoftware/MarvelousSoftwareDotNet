using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MarvelousSoftware.Common.Extensions;
using MarvelousSoftware.QueryLanguage.Attributes;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Provides basic info about column.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Name of the column in provided query.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Underlaying member name in given model type.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Type of member.
        /// </summary>
        public Type SystemType { get; private set; }

        /// <summary>
        /// Is <see cref="SystemType"/> is nullable.
        /// </summary>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// Type of a column.
        /// </summary>
        public ColumnType ColumnType { get; private set; }

        private readonly Dictionary<ColumnType, KeywordType[]> _allowedColumnKeywords = new Dictionary<ColumnType, KeywordType[]>
        {
            {
                ColumnType.DateTime, 
                new []
                {
                    KeywordType.Empty, KeywordType.NotEmpty,
                    KeywordType.Equal, KeywordType.NotEqual,
                    KeywordType.GreaterThan, KeywordType.GreaterThanOrEqual, 
                    KeywordType.LessThan, KeywordType.LessThanOrEqual
                }
            },
            {
                ColumnType.Float, 
                new []
                {
                    KeywordType.Empty, KeywordType.NotEmpty,
                    KeywordType.Equal, KeywordType.NotEqual, 
                    KeywordType.GreaterThan, KeywordType.GreaterThanOrEqual, 
                    KeywordType.LessThan, KeywordType.LessThanOrEqual
                }
            },
            {
                ColumnType.Integer, 
                new []
                {
                    KeywordType.Empty, KeywordType.NotEmpty,
                    KeywordType.Equal, KeywordType.NotEqual, 
                    KeywordType.GreaterThan, KeywordType.GreaterThanOrEqual, 
                    KeywordType.LessThan, KeywordType.LessThanOrEqual
                }
            },
            {
                ColumnType.String, 
                new []
                {
                    KeywordType.Empty, KeywordType.NotEmpty,
                    KeywordType.Equal, KeywordType.NotEqual, 
                    KeywordType.Contains, 
                    KeywordType.StartsWith, KeywordType.EndsWith
                }
            },
            {
                ColumnType.Boolean,
                new []
                {
                    KeywordType.Empty, KeywordType.NotEmpty,
                    KeywordType.IsTrue, KeywordType.IsFalse
                }
            }
        };

        public ColumnInfo(string columnName, string memberName, Type systemType)
        {
            ColumnName = columnName;
            MemberName = memberName;
            SystemType = systemType;
            IsNullable = systemType.IsNullable();

            // todo: create a unit test for finding ColumnType property!
            bool columnTypeFound = false;
            foreach (object column in Enum.GetValues(typeof (ColumnType)))
            {
                if(columnTypeFound)
                    break;

                var attr = typeof (ColumnType).GetTypeInfo()
                    .GetDeclaredField(column.ToString())
                    .GetCustomAttribute<SystemTypesAttribute>();

                if (attr.Types.Any(type => type == systemType))
                {
                    ColumnType = (ColumnType) column;
                    columnTypeFound = true;
                }
            }

            if (columnTypeFound == false)
            {
                var msg = $"Column '{columnName}' has unsupported type.";
                throw new QueryLanguageException(msg);
            }
        }

        /// <summary>
        /// Checks if given compare operator is allowed for this column type.
        /// </summary>
        /// <returns>True if allowed; false otherwise.</returns>
        public bool IsCompareOperatorAllowed(Keyword compareOperatorKeyword)
        {
            var compareOperator = compareOperatorKeyword.KeywordType;

            if (compareOperatorKeyword.TokenType != TokenType.CompareOperator)
            {
                var msg = $"Keyword '{compareOperatorKeyword.Syntax}' is not a compare operator.";
                throw new ArgumentException(msg);
            }
            
            return _allowedColumnKeywords[ColumnType].Contains(compareOperator);
        }

        /// <summary>
        /// Checks if given statement is allowed for this column type.
        /// </summary>
        /// <returns>True if allowed; false otherwise.</returns>
        public bool IsStatementAllowed(Keyword statementKeyword)
        {
            var compareOperator = statementKeyword.KeywordType;

            if (statementKeyword.TokenType != TokenType.Statement)
            {
                var msg = $"Keyword '{statementKeyword.Syntax}' is not a statement.";
                throw new ArgumentException(msg);
            }

            switch (compareOperator)
            {
                case KeywordType.Empty:
                case KeywordType.NotEmpty:
                    return SystemType.IsNullable();

                case KeywordType.IsFalse:
                case KeywordType.IsTrue:
                    return SystemType == typeof (bool) || SystemType == typeof (bool?);
            }

            return false;
        }
    }
}