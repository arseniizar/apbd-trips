namespace TripApp.Core.Model;

// This is a generic model used for returning paginated results. It contains all necessary metadata
public class PaginatedResult<T> where T : class
{
    // Represents current page number
    public int PageNum { get; set; }
    // Represents current page size
    public int PageSize { get; set; }
    // Represents amount of pages in total
    public int AllPages { get; set; }
    public List<T> Data { get; set; } = [];
}