namespace DotNetService.Http.API.Version1
{
    public class PaginationMeta
    {
        public int TotalPage;
        public int Total;
        public int Page;
        public int PerPage;
    }

    public class PaginationModel: PaginationMeta
    {       
        public object Data;
    }    
}