using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MarvelousSoftware.QueryLanguage.Config;
using MarvelousSoftware.QueryLanguage.SimplifiedFiltering;
using MarvelousSoftware.QueryLanguage.Tests.Model;
using NUnit.Framework;

namespace MarvelousSoftware.QueryLanguage.Tests
{
    public class SimplifiedFiltererTests
    {
        private readonly IQueryable<Employee> Data = DataSample.GetEmployees();

        [Test]
        public void SimplifiedFilterer_HandleStringValues()
        {
            var model = new SimplifiedFilteringModel
            {
                ColumnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>
                {
                    {
                        "FirstName", new List<SimpleColumnFilter>
                        {
                            new SimpleColumnFilter
                            {
                                CompareOperator = KeywordType.Equal,
                                Value = "Damian"
                            }
                        }
                    }
                }
            };

            Test(model, x => x.FirstName == "Damian");
        }

        [Test]
        public void SimplifiedFilterer_HandleFloatValues()
        {
            var model = new SimplifiedFilteringModel
            {
                NumberDecimalSeparator = ",",
                ColumnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>
                {
                    {
                        "Salary", new List<SimpleColumnFilter>
                        {
                            new SimpleColumnFilter
                            {
                                CompareOperator = KeywordType.GreaterThan,
                                Value = "5 000,5"
                            }
                        }
                    }
                }
            };

            Test(model, x => x.Salary > 5000.5M);
        }

        [Test]
        public void SimplifiedFilterer_HandleIntegerValues()
        {
            var model = new SimplifiedFilteringModel
            {
                ColumnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>
                {
                    {
                        "TestCase", new List<SimpleColumnFilter>
                        {
                            new SimpleColumnFilter
                            {
                                CompareOperator = KeywordType.GreaterThanOrEqual,
                                Value = "200"
                            }
                        }
                    }
                }
            };

            Test(model, x => x.TestCase >= 200);
        }

        [Test]
        public void SimplifiedFilterer_HandleDateValues()
        {
            var model = new SimplifiedFilteringModel
            {
                DateFormats = new[] { "d/M/yyyy" },
                ColumnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>
                {
                    {
                        "Created", new List<SimpleColumnFilter>
                        {
                            new SimpleColumnFilter
                            {
                                CompareOperator = KeywordType.GreaterThanOrEqual,
                                Value = "2/3/2015"
                            }
                        }
                    }
                }
            };

            Test(model, x => x.Created >= new DateTime(2015, 3, 2));
        }

        [Test]
        public void SimplifiedFilterer_HandleBooleanValues()
        {
            var model = new SimplifiedFilteringModel
            {
                TrueValues = new []{ "yup" },
                FalseValues = new[] { "nope" },
                ColumnsFilters = new Dictionary<string, IEnumerable<SimpleColumnFilter>>
                {
                    {
                        "IsAwesome", new List<SimpleColumnFilter>
                        {
                            new SimpleColumnFilter
                            {
                                CompareOperator = KeywordType.Equal,
                                Value = "yup"
                            }
                        }
                    }
                }
            };

            Test(model, x => x.IsAwesome == true);
        }

        private void Test(SimplifiedFilteringModel model, Expression<Func<Employee, bool>> predicate)
        {
            var config = new LanguageConfig<Employee>();

            foreach (var columnName in model.ColumnsFilters.Keys)
            {
                config.AddColumn(columnName, columnName);
            }

            var filterer = new SimplifiedFilterer();
            var expected = Data.Where(predicate).ToArray();

            var result = filterer.Filter(model, Data);

            var received = result.ToArray();
            expected.Length.Should()
                .NotBe(Data.Count(),
                    "Test case should not return full collection of elements. May lead to false-positives.")
                .And.NotBe(0, "Test case should be desined to return at least one element.");

            received.Should().BeEquivalentTo<Employee>(expected);
        }
    }
}