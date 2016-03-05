using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.Models
{
    public class FilterResult<T>
    {
        public IQueryable<T> Elements { get; set; } = new T[0].AsQueryable();
        public ErrorList Errors { get; set; } = new ErrorList();
        public bool HasError => Errors != null && Errors.Any();
    }
}