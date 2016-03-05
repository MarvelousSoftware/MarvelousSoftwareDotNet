using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MarvelousSoftware.Grid.DataSource
{
    [DataContract]
    public class DataSourceResult<T> : Dictionary<string, object>
    {
        public int Total
        {
            get { return (int)this["total"]; }
            set { this["total"] = value; }
        }

        public IQueryable<T> Data
        {
            get { return (IQueryable<T>)this["data"]; }
            set { this["data"] = value; }
        }

        public IDictionary<string, IEnumerable<string>> Errors
        {
            get { return (IDictionary<string, IEnumerable<string>>)this["errors"]; }
            set { this["errors"] = value; }
        }

        public DataSourceResult()
        {
            Total = 0;
            Data = new T[0].AsQueryable();
            Errors = new Dictionary<string, IEnumerable<string>>();
        }
    }
}