using Microsoft.AspNetCore.Mvc;

namespace DotNetService.Http.API.Version1.Requests
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
            SortOrder = SortOrderEnum.Desc;
        }

        [FromQuery(Name = "search")]
        public string Search { get; set; }

        [FromQuery(Name = "pagination")]
        public bool Pagination { get; set; }

        [FromQuery(Name = "per_page")]
        public int PerPage { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; }

        [FromQuery(Name = "sort_by")]
        public string SortBy { get; set; }

        [FromQuery(Name = "sort_order")]
        public SortOrderEnum SortOrder { get; set; }
    }
}