using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.AutoCompletion;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Lexer;
using MarvelousSoftware.QueryLanguage.Lexer.Functions;
using MarvelousSoftware.QueryLanguage.Models;
using MarvelousSoftware.QueryLanguage.Tests.Extensions;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    [TestFixture]
    public class AutoCompleterTests
    {
        private readonly IDictionary<string, Expression<Func<Employee, object>>> _columns = new Dictionary<string, Expression<Func<Employee, object>>>()
        {
            { "FirstName", x => x.FirstName },
            { "LastName", x => x.LastName },
            { "Created", x => x.Created },
            { "Salary", x => x.Salary },
            { "FunLevel", x => x.FunLevel },
            { "testCase", x => x.TestCase },
            { "TestCase2", x => x.TestCase2 }
        };

        private LanguageConfig<Employee> _config;
        private AutoCompleter<Employee> _autoCompleter;

        // TODO: pagination tests

        [SetUp]
        public void SetUp()
        {
            _config = new LanguageConfig<Employee>();
            _config.Functions(config => config.Define(new FunctionDefinition("currentUser", ColumnType.String, () => "john")));

            foreach (var column in _columns)
            {
                _config.AddColumn(column.Key, column.Value);
            }
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithEmptyQuery()
        {
            var query = "";
            var result = GetAutoCompleter().Run(query, 0, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(_columns.Keys);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithCaretAtTheStart()
        {
            var query = "FirstName";
            var result = GetAutoCompleter().Run(query, 0, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(_columns.Keys);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithCaretInTheMiddle()
        {
            var query = "FirstName";
            var result = GetAutoCompleter().Run(query, 1, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "FirstName", "FunLevel" });
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithCaretInTheMiddleAndHalfEmptyQuery()
        {
            var query = "F";
            var result = GetAutoCompleter().Run(query, 1, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "FirstName", "FunLevel" });
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithCaretAtTheEnd()
        {
            var query = "FirstName";
            var result = GetAutoCompleter().Run(query, query.Length, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "FirstName" });
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithContainsWithStrategy()
        {
            var query = "Name";
            var result = GetAutoCompleter(c => c.ColumnCompletionStrategy = CompletionStrategy.Contains)
                .Run(query, 4, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "FirstName", "LastName" });
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_IgnoreCase()
        {
            var query = "t";
            var result = GetAutoCompleter(c => c.IgnoreCase = true).Run(query, 1, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "testCase", "TestCase2" });
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteColumns_MatchCase()
        {
            var query = "t";
            var result = GetAutoCompleter(c => c.IgnoreCase = false).Run(query, 1, 0);

            result.HasErrors.Should().BeFalse();
            result.GetTexts(CompletionGroup.Columns).Should().BeEquivalentTo(new List<string>() { "testCase" });
        }

        [TestCase("FirstName", 0, 0)]
        [TestCase("FirstName", 5, 0)]
        [TestCase("  FirstName  ", 2, 2)]
        [TestCase("  FirstName  ", 7, 2)]
        [TestCase("LastName is empty OR FirstName = \"Damian\"", 21, 21)]
        [TestCase("LastName is empty OR FirstName = \"Damian\"", 26, 21)]
        public void AutoCompleter_ShouldAutoCompleteColumns_WithCorrectStartPositionAndLength(string query, int caretPosition, int tokenStartPosition)
        {
            var column = "FirstName";

            var result = GetAutoCompleter().Run(query, caretPosition, 0);

            result.StartPosition.Should().Be(tokenStartPosition);
            result.Length.Should().Be(column.Length);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithEmpty()
        {
            const string Query = "FirstName ";
            var expected = new List<string>()
            {
                "=",
                "!=",
                "contains",
                "starts with",
                "ends with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter().Run(Query, Query.Length, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(0);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretAtTheStart()
        {
            var expected = new List<string>()
            {
                "=",
                "!=",
                "contains",
                "starts with",
                "ends with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter().Run("FirstName != \"Damian\"", 10, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(2);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretAtTheStartWithoutAnyCompareOperatorWritten()
        {
            var expected = new List<string>()
            {
                "=",
                "!=",
                "contains",
                "starts with",
                "ends with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter().Run("FirstName  \"Damian\"", 10, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(0);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretInTheMiddle()
        {
            var expected = new List<string>()
            {
                "starts with",
                "ends with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter(c => c.KeywordCompletionStrategy = CompletionStrategy.Contains)
                .Run("FirstName with \"Damian\"", 13, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(5);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretAtTheEnd()
        {
            var expected = new List<string>()
            {
                "starts with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter()
                .Run("FirstName starts with \"Damian\"", 21, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(11);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretAferSpace()
        {
            var expected = new List<string>()
            {
                "starts with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter()
                .Run("FirstName starts w \"Damian\"", 18, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(9);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithCaretRightAferSpace()
        {
            var expected = new List<string>()
            {
                "starts with"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter()
                .Run("FirstName starts  \"Damian\"", 17, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(8);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithIgnoreCase()
        {
            var expected = new List<string>()
            {
                "contains"
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            var result = GetAutoCompleter(c => c.IgnoreCase = true)
                .Run("FirstName CON", 12, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(3);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithMatchCase()
        {
            var autoCompleter = GetAutoCompleter(c => c.IgnoreCase = false);

            var result = autoCompleter.Run("FirstName CON", 12, 0);
            var result2 = autoCompleter.Run("FirstName con", 12, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().HaveCount(0);
            result2.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("contains");
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithNumbers()
        {
            var expected = new List<string>()
            {
                "=", "!=", ">", "<", "<=", ">="
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            const string query = "Salary ";
            var result = GetAutoCompleter().Run(query, query.Length, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(7);
            result.Length.Should().Be(0);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithDates()
        {
            var expected = new List<string>()
            {
                "=", "!=", ">", "<", "<=", ">="
            };
            var unexpected = GetCompareOperatorsExceptOf(expected);

            const string query = "Created ";
            var result = GetAutoCompleter().Run(query, query.Length, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().Contain(expected);
            result.GetTexts(CompletionGroup.Keywords).Should().NotContainAny(unexpected);
            result.StartPosition.Should().Be(query.Length);
            result.Length.Should().Be(0);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithContainsCompletionStrategy()
        {
            var autoCompleter = GetAutoCompleter(c => c.KeywordCompletionStrategy = CompletionStrategy.Contains);

            var result = autoCompleter.Run("FirstName with", 14, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("starts with", "ends with");
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(4);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteCompareOperators_WithStartsWithCompletionStrategy()
        {
            var autoCompleter = GetAutoCompleter(c => c.KeywordCompletionStrategy = CompletionStrategy.StartsWith);

            var result = autoCompleter.Run("FirstName con", 13, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("contains");
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(3);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteStatements()
        {
            var result = GetAutoCompleter().Run("FirstName is", 12, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("is empty", "is not empty");
            result.StartPosition.Should().Be(10);
            result.Length.Should().Be(2);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteEmptyStatementOnlyIfNullable()
        {
            var result = GetAutoCompleter().Run("TestCase is", 11, 0);
            var result2 = GetAutoCompleter().Run("Salary is", 9, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEmpty();
            result2.GetTexts(CompletionGroup.Keywords).Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteLiterals()
        {
            TestStringLiteralCompletion("FirstName = |", x => x.Select(e => e.FirstName));
            TestStringLiteralCompletion("FirstName = \"|", x => x.Select(e => e.FirstName));
            TestStringLiteralCompletion("FirstName = \"D|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("D")));
            TestStringLiteralCompletion("FirstName = D|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("D")));
            TestStringLiteralCompletion("FirstName = \"Damian|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("Damian")));
            TestStringLiteralCompletion("FirstName = Damian|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("Damian")));
            TestStringLiteralCompletion("FirstName = \"Damian\"|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("Damian")));
            TestStringLiteralCompletion("FirstName = \"Damian|\"", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("Damian")));
            TestStringLiteralCompletion("FirstName = \"Damian D|", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("Damian D")));
            TestStringLiteralCompletion("FirstName = \"D|amian\"", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("D")));
            TestStringLiteralCompletion("FirstName = D|amian", x => x.Select(e => e.FirstName).Where(e => e != null && e.StartsWith("D")));
            TestStringLiteralCompletion("FirstName = \"|Damian\"", x => x.Select(e => e.FirstName));
            TestStringLiteralCompletion("FirstName = |Damian", x => x.Select(e => e.FirstName));
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteFunctions()
        {
            var result = GetAutoCompleter().Run("FirstName = cu", 14, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("currentUser()");
            result.StartPosition.Should().Be(12);
            result.Length.Should().Be(2);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteFunctionsFrom_WithCaretInTheMiddle()
        {
            var result = GetAutoCompleter().Run("FirstName = curre", 14, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("currentUser()");
            result.StartPosition.Should().Be(12);
            result.Length.Should().Be(5);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteFunctionsFrom_WithCaretInTheMiddleAndFullFunction()
        {
            var result = GetAutoCompleter().Run("FirstName = currentUser()", 14, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEquivalentTo("currentUser()");
            result.StartPosition.Should().Be(12);
            result.Length.Should().Be("currentUser()".Length);
        }

        [Test]
        public void AutoCompleter_ShouldAutoCompleteFunctions_OnlyForCorrectColumnType()
        {
            var result = GetAutoCompleter().Run("DateOfBirth = cu", 14, 0);

            result.GetTexts(CompletionGroup.Keywords).Should().BeEmpty();
        }

        private IEnumerable<string> GetCompareOperatorsExceptOf(List<string> expected)
        {
            var unexpected = _config.Syntax.CompareOperators
                .Where(x => expected.Any(z => z.Equals(x.Syntax, StringComparison.InvariantCultureIgnoreCase)) == false)
                .Select(x => x.Syntax);
            return unexpected;
        }

        private void TestStringLiteralCompletion(string query, Func<IEnumerable<Employee>, IEnumerable<string>> selector)
        {
            var source = DataSample.GetEmployees();
            var autoCompleter = GetAutoCompleter(c => c.ValueDataSource = source);

            var caretPosition = query.IndexOf('|');
            query = query.Remove(caretPosition, 1);

            var result = autoCompleter.Run(query, caretPosition, 0);

            var expected = selector(source).Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Contains(" ") ? '"' + x + '"' : x);
            result.GetTexts(CompletionGroup.Values).Should().BeEquivalentTo(expected);
        }

        private AutoCompleter<Employee> GetAutoCompleter(Action<AutoCompleteOptions<Employee>> options = null)
        {
            if (options != null)
            {
                _config.AutoCompletion(options);
            }

            _autoCompleter = new AutoCompleter<Employee>(_config);
            return _autoCompleter;
        }
    }
}