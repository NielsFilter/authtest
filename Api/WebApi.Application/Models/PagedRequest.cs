namespace WebApi.Models;

public class PagedRequest
{
    public int PageSize { get; set; } = 10; 
    public int Index { get; set; }
}