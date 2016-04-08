using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MarvelousSoftware.Core.Extensions;
using MarvelousSoftware.QueryLanguage.AutoCompletion;
using MarvelousSoftware.QueryLanguage.Lexing;
using MarvelousSoftware.QueryLanguage.Lexing.Functions;
using MarvelousSoftware.QueryLanguage.Models;

namespace MarvelousSoftware.QueryLanguage.Config
{
    /// <summary>
    /// Contains language related configuration.
    /// </summary>
    /// <typeparam name="T">Model elements type.</typeparam>
    public class LanguageConfig<T>
    {
        internal AutoCompleteOptions<T> AutoCompleteOptions = new AutoCompleteOptions<T>();

        /// <summary>
        /// A list of added columns.
        /// </summary>
        public IReadOnlyList<ColumnInfo> Columns => _columns;

        /// <summary>
        /// Syntax configuration which is used mainly while parsing given queries.
        /// </summary>
        internal Syntax SyntaxConfig { get; private set; } = new Syntax(new SyntaxConfig());

        /// <summary>
        /// Definition of declared functions. Allows to declare new functions and override existing one.
        /// </summary>
        internal IFunctionDefinitions FunctionDefinitions { get; } = new FunctionDefinitions();

        /// <summary>
        /// Provider of tokenizers.
        /// </summary>
        public ITokenizersProvider<T> TokenizersProvider { get; private set; } = new TokenizersProvider<T>();

        private Type ModelType { get; } = typeof (T);
        private readonly List<ColumnInfo> _columns = new List<ColumnInfo>();

        /// <summary>
        /// Configures auto completion.
        /// </summary>
        public LanguageConfig<T> AutoCompletion(Action<AutoCompleteOptions<T>> config)
        {
            config(AutoCompleteOptions);
            return this;
        }

        /// <summary>
        /// Configures language syntax.
        /// </summary>
        public LanguageConfig<T> Syntax(Action<SyntaxConfig> config)
        {
            config((SyntaxConfig)SyntaxConfig.Config);
            SyntaxConfig = new Syntax(SyntaxConfig.Config);
            return this;
        }

        /// <summary>
        /// Configures available functions.
        /// </summary>
        public LanguageConfig<T> Functions(Action<IFunctionDefinitions> config)
        {
            config(FunctionDefinitions);
            return this;
        }

        /// <summary>
        /// Adds predefined functions to the language configuration.
        /// </summary>
        public LanguageConfig<T> WithPredefinedFunctions()
        {
            Functions(f =>
            {
                f.Define(new FunctionDefinition("today", ColumnType.DateTime, () => DateTime.Today));
                f.Define(new FunctionDefinition("now", ColumnType.DateTime, () => DateTime.Now));
                f.Define(new FunctionDefinition("tomorrow", ColumnType.DateTime, () => DateTime.Today.AddDays(1)));
                f.Define(new FunctionDefinition("yesterday", ColumnType.DateTime, () => DateTime.Today.AddDays(1)));
            });

            return this;
        } 

        /// <summary>
        /// Adds new column.
        /// </summary>
        /// <param name="columnName">Name of the column which will be available in the query.</param>
        /// <param name="property">Property in the model.</param>
        public LanguageConfig<T> AddColumn(string columnName, Expression<Func<T, object>> property)
        {
            return AddColumn(columnName, property.GetFullPropertyName());
        }

        /// <summary>
        /// Adds new column.
        /// </summary>
        /// <param name="property">Property in the model.</param>
        public LanguageConfig<T> AddColumn(Expression<Func<T, object>> property)
        {
            var columnName = property.GetFullPropertyName();
            return AddColumn(columnName, columnName);
        }

        /// <summary>
        /// Adds new column.
        /// </summary>
        /// <param name="columnName">Name of the column which will be available in the query.</param>
        /// <param name="propertyName">Name of the property in the model.</param>
        /// <exception cref="ArgumentException">When propertyName or columnName is invalid due to some restrictions.</exception>
        public LanguageConfig<T> AddColumn(string columnName, string propertyName)
        {
            if(GetColumnInfo(columnName) != null)
                throw new ArgumentException("Column with given name already exist.", nameof(columnName));

            var invalid = SyntaxConfig.Config.GetLanguageSpecialChars().ToArray();

            if (columnName.IndexOfAny(invalid) > 0)
            {
                var invalidChars = string.Join(", ", invalid);
                var msg = $"Given column name ('{columnName}') contains one of the invalid chars: {invalidChars}.";
                throw new ArgumentException(msg, nameof(columnName));
            }

            // TODO: support for fields
            var property = ModelType.GetNestedProperty(propertyName);

            if(property == null)
                throw new ArgumentException("Given property name does not exist.", nameof(propertyName));

            _columns.Add(new ColumnInfo(columnName, propertyName, property.PropertyType));
            return this;
        }

        /// <summary>
        /// Gets column by name. Uses <see cref="ISyntaxConfig.ColumnNameCaseSensitive"/> configuration.
        /// </summary>
        internal ColumnInfo GetColumnInfo(string columnName)
        {
            if(SyntaxConfig.Config.ColumnNameCaseSensitive)
                return Columns.FirstOrDefault(x => x.ColumnName == columnName);

            return Columns.FirstOrDefault(x => x.ColumnName.ToLower() == columnName.ToLower());
        }

        /// <summary>
        /// Gets keyword by name.
        /// </summary>
        internal Keyword GetKeyword(string keywordName)
        {
            return SyntaxConfig.Config.Keywords.FirstOrDefault(x => x.Syntax == keywordName);
        }
    }
}
