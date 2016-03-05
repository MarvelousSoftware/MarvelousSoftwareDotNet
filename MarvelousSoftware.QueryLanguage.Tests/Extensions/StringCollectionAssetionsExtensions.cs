using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;

namespace MarvelousSoftware.QueryLanguage.Tests.Extensions
{
    public static class StringCollectionAssetionsExtensions
    {
        public static AndConstraint<StringCollectionAssertions> NotContainAny(this StringCollectionAssertions obj, IEnumerable<string> collection)
        {
            foreach (var element in collection)
            {
                obj.NotContain(element);
            }
            return new AndConstraint<StringCollectionAssertions>(obj);
        }
    }
}