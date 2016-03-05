using System.Collections.Generic;
using System.Runtime.Serialization;
using MarvelousSoftware.QueryLanguage.Config;

namespace MarvelousSoftware.Grid.DataSource
{
    public class DataSourceParams
    {
        [DataMember(Name = "fields")]
        public string[] Fields { get; set; }

        [DataMember(Name = "sortBy")]
        public SortInfo[] SortBy { get; set; }

        [DataMember(Name = "page")]
        public int? Page { get; set; }

        [DataMember(Name = "pageSize")]
        public int? PageSize { get; set; }

        [DataMember(Name = "getOnlyVisibleColumns")]
        public bool? GetOnlyVisibleColumns { get; set; }

        [DataMember(Name = "filtering")]
        public Dictionary<string, ColumnFilter[]> Filtering { get; set; }
    }

    public class ColumnFilter
    {
        [DataMember(Name = "compareOperator")]
        public KeywordType CompareOperator { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    public class SortInfo
    {
        [DataMember(Name = "member")]
        public string Member { get; set; }

        [DataMember(Name = "direction")]
        public string Direction { get; set; }
    }
}