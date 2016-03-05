using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MarvelousSoftware.QueryLanguage.Models
{
    [DataContract]
    public class QueryLanguageFilterResult<T>
    {
        [DataMember(Name = "data")]
        public IQueryable<T> Data { get; set; }

        [DataMember(Name = "errors")]
        public IEnumerable<string> Errors { get; set; }

        public QueryLanguageFilterResult()
        {
            Data = new T[0].AsQueryable();
            Errors = new string[0];
        }
    }
}