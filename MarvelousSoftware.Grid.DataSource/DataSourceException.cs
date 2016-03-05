using System;

namespace MarvelousSoftware.Grid.DataSource
{
    public class DataSourceException : Exception
    {
        public DataSourceException(string message) : base(message)
        {
        }
    }
}