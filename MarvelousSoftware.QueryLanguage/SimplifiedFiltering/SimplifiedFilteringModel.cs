using System.Collections.Generic;

namespace MarvelousSoftware.QueryLanguage.SimplifiedFiltering
{
    public class SimplifiedFilteringModel
    {
        /// <summary>
        /// Dictionary of filters where key is column name and value is list of filters.
        /// </summary>
        public Dictionary<string, IEnumerable<SimpleColumnFilter>> ColumnsFilters { get; set; } 
            = new Dictionary<string, IEnumerable<SimpleColumnFilter>>();

        /// <summary>
        /// Date formats used for parsing dates. Order matter. First correcly parsed will be used.
        /// </summary>
        public IEnumerable<string> DateFormats { get; set; } = new[] { "d/M/yyyy H:m", "d/M/yyyy" };

        /// <summary>
        /// Values used for parsing 'true'.
        /// </summary>
        public IEnumerable<string> TrueValues { get; set; } = new[] { "true" };

        /// <summary>
        /// Values used for parsing 'false'.
        /// </summary>
        public IEnumerable<string> FalseValues { get; set; } = new [] { "false" };

        /// <summary>
        /// Separator used while parsing numbers.
        /// </summary>
        public string NumberDecimalSeparator { get; set; } = ",";
    }
}