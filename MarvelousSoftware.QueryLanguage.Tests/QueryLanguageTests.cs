using System;
using System.Linq;
using System.Linq.Expressions;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Lexing.Functions;
using MarvelousSoftware.QueryLanguage.Models;
using NUnit.Framework;
using static System.String;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    [TestFixture]
    public class QueryLanguageTests
    {
        public readonly IQueryable<Employee> Data = DataSample.GetEmployees();

        // TODO: missing features: 
        // - operator NOT, e.g. NOT starts with, NOT ends with
        // - DateTime manipulations, e.g. Created > now() - 2d

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingStatements()
        {
            Test("FirstName is empty", x => IsNullOrEmpty(x.FirstName));
            Test("FirstName is not empty", x => !IsNullOrEmpty(x.FirstName));
            Test("HasPersonInfo is true", x => x.HasPersonInfo);
            Test("HasPersonInfo is false", x => !x.HasPersonInfo);
            Test("IsAwesome is true", x => x.IsAwesome.HasValue && x.IsAwesome.Value);
            Test("IsAwesome is false", x => x.IsAwesome.HasValue && !x.IsAwesome.Value);
            Test("IsAwesome is empty", x => !x.IsAwesome.HasValue);
            Test("IsAwesome is not empty", x => x.IsAwesome.HasValue);

            Test("FirstName is not empty", x => IsNullOrEmpty(x.Person?.FirstName) == false);
            Test("Foo is not empty", x => IsNullOrEmpty(x.Foo) == false);
        }

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingLogicalOperators()
        {
            Test("FirstName is empty and LastName is empty", 
                x => IsNullOrEmpty(x.FirstName) && IsNullOrEmpty(x.LastName));

            Test("FirstName is empty or LastName is empty",
                x => IsNullOrEmpty(x.FirstName) || IsNullOrEmpty(x.LastName));

            // && has higher precedence in C#, in the query language it doesn't
            // that's why parens are needed
            // TODO: change that
            Test("FirstName = \"Damian\" or LastName = \"Kaminski\" and FirstName is empty", 
                x => (x.FirstName == "Damian" || x.LastName == "Kaminski") && IsNullOrEmpty(x.FirstName));

            Test("FirstName = \"Damian\" and LastName = \"Kaminski\" or FirstName is empty",
                x => x.FirstName == "Damian" && x.LastName == "Kaminski" || IsNullOrEmpty(x.FirstName));
        }

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingCompareOperatorsWithNumbers()
        {
            Test("Salary > 5000", x => x.Salary > 5000);
            Test("Salary >= 5000", x => x.Salary >= 5000);
            Test("Salary < 5000", x => x.Salary < 5000);
            Test("Salary <= 5000", x => x.Salary <= 5000);
            Test("Salary = 5000", x => x.Salary == 5000);
            Test("Salary != 5000", x => x.Salary != 5000);
            Test("Salary is empty", x => x.Salary == null);
            Test("Salary is not empty", x => x.Salary != null);
        }

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingCompareOperatorsWithStrings()
        {
            Test("FirstName = \"Damian\"", x => x.FirstName == "Damian");
            Test("FirstName != \"Damian\"", x => x.FirstName != "Damian");
            Test("FirstName contains \"D\"", x => x.FirstName != null && x.FirstName.Contains("D"));
            Test("FirstName starts with \"D\"", x => x.FirstName != null && x.FirstName.StartsWith("D"));
            Test("FirstName ends with \"n\"", x => x.FirstName != null && x.FirstName.EndsWith("n"));
        }

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingCompareOperatorsWithDates()
        {
            Test("Created > 25/1/2015", x => x.Created > new DateTime(2015, 1, 25));
            Test("Created >= 25/1/2015", x => x.Created >= new DateTime(2015, 1, 25));
            Test("Created < 25/1/2015", x => x.Created < new DateTime(2015, 1, 25));
            Test("Created <= 25/1/2015", x => x.Created <= new DateTime(2015, 1, 25));
            Test("Created = 25/1/2015", x => x.Created == new DateTime(2015, 1, 25));
            Test("Created != 25/1/2015", x => x.Created != new DateTime(2015, 1, 25));

            Test("Created > 25/1/2015 10:10", x => x.Created > new DateTime(2015, 1, 25, 10, 10, 0));
            Test("Created >= 25/1/2015 10:10", x => x.Created >= new DateTime(2015, 1, 25, 10, 10, 0));
            Test("Created < 25/1/2015 10:10", x => x.Created < new DateTime(2015, 1, 25, 10, 10, 0));
            Test("Created <= 25/1/2015 10:10", x => x.Created <= new DateTime(2015, 1, 25, 10, 10, 0));
            Test("Created = 25/1/2015 10:10", x => x.Created == new DateTime(2015, 1, 25, 10, 10, 0));
            Test("Created != 25/1/2015 10:10", x => x.Created != new DateTime(2015, 1, 25, 10, 10, 0));

            Test("Created is empty", x => x.Created == null);
            Test("Created is not empty", x => x.Created != null);
        }

        [Test]
        public void QueryLanguage_ShouldBeAbleToSearchUsingParens()
        {
            Test("Salary is not empty or ((FirstName is empty or LastName is empty) and Salary is empty) ",
                x => x.Salary.HasValue || ((IsNullOrEmpty(x.FirstName) || IsNullOrEmpty(x.LastName)) && x.Salary == null)
            );

            Test("Salary is not empty or (Salary is empty and (FirstName is empty or LastName is empty))",
                x => x.Salary.HasValue || (x.Salary == null && (IsNullOrEmpty(x.FirstName) || IsNullOrEmpty(x.LastName)))
            );
        }

        [Test]
        public void QueryLanguage_CombinedScenerios()
        {
            Test("Salary > 5000 AND Salary < 8000", x => x.Salary > 5000 && x.Salary < 8000);
            Test("Salary > lowSalary() AND Salary < highSalary()", x => x.Salary > 2000 && x.Salary < 15000);
        }

        [Test]
        public void QueryLanguage_ShouldReturnErrorInCaseOfIncompleteQuery()
        {
            var config = GetConfig();
            var language = new QueryLanguage<Employee>(config);

            var result = language.Filter(Data, "FirstName");

            result.HasError.Should().BeTrue();
        }

        [Test]
        public void QueryLanguage_ShouldReturnErrorInCaseOfUnknownToken()
        {
            var config = GetConfig();
            var language = new QueryLanguage<Employee>(config);

            var result = language.Filter(Data, "FirstName is empty unknown token");

            result.HasError.Should().BeTrue();
        }

        private void Test(string query, Func<Employee, bool> predicate)
        {
            var config = GetConfig();

            var language = new QueryLanguage<Employee>(config);
            var expected = Data.Where(predicate).ToArray();

            var result = language.Filter(Data, query);

            result.Errors.Should().BeNullOrEmpty();
            var received = result.Elements.ToArray();
            expected.Length.Should().NotBe(Data.Count(), "Test case should not return full collection of elements. May lead to false-positives.")
                .And.NotBe(0, "Test case should be desined to return at least one element.");
            try
            {
                received.Should().BeEquivalentTo<Employee>(expected);
            }
            finally
            {
                Console.WriteLine("Query:" + query);
                Console.WriteLine("Expected:");
                foreach (var employee in expected)
                {
                    Console.WriteLine(employee);
                }
                Console.WriteLine();
                Console.WriteLine("Received:");
                foreach (var employee in received)
                {
                    Console.WriteLine(employee);
                }   
            }
        }

        private static LanguageConfig<Employee> GetConfig()
        {
            return new LanguageConfig<Employee>()
                .AddColumn("Salary", x => x.Salary)
                .AddColumn("FirstName", x => x.Person.FirstName)
                .AddColumn("LastName", x => x.Person.LastName)
                .AddColumn("Created", x => x.Created)
                .AddColumn("HasPersonInfo", x => x.HasPersonInfo)
                .AddColumn("IsAwesome", x => x.IsAwesome)
                .AddColumn("Foo", x => x.Foo)
                .Functions(config => config.Define(new FunctionDefinition("lowSalary", ColumnType.Float, () => 2000)))
                .Functions(config => config.Define(new FunctionDefinition("highSalary", ColumnType.Float, () => 15000)));
        }
    }
}