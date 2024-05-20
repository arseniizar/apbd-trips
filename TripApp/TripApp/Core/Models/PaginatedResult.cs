namespace TripApp.Core.Models;

public class PaginatedResult<T> where T : class
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<T> Data { get; set; } = [];
}