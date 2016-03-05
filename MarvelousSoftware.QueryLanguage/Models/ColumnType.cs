using System;
using MarvelousSoftware.QueryLanguage.Attributes;

namespace MarvelousSoftware.QueryLanguage.Models
{
    public enum ColumnType
    {
        [SystemTypes(typeof(int), typeof(int?), typeof(long), typeof(long?), typeof(short), typeof(short?))]
        Integer,

        [SystemTypes(typeof(decimal), typeof(decimal?), 
            typeof(float), typeof(float?), 
            typeof(double), typeof(double?))]
        Float,

        [SystemTypes(typeof(DateTime), typeof(DateTime?))]
        DateTime,

        [SystemTypes(typeof(string))]
        String,

        [SystemTypes(typeof(bool), typeof(bool?))]
        Boolean
    }
}