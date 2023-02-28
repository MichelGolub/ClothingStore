
namespace WebApi.Filters
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 15;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            if (pageSize < 1 || pageSize > 50)
            {
                PageSize = 10;
            }
            else
            {
                PageSize = pageSize;
            }
        }
    }
}
