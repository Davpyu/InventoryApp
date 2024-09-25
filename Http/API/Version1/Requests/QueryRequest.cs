using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DotNetService.Http.API.Version1
{
    public enum SortOrderEnum
    {
        Asc,
        Desc
    }

    public class Query
    {
        public Query()
        {
            Order = SortOrderEnum.Desc;
            PerPage = 10;
            Page = 1;
        }

        [FromQuery(Name = "search")]
        public string Search { get; set; }

        [FromQuery(Name = "per_page")]
        [Range(1, 100)]
        public int PerPage { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; }

        [FromQuery(Name = "sort_by")]
        public string SortBy { get; set; }

        [FromQuery(Name = "order")]
        public SortOrderEnum Order { get; set; }
    }
}
