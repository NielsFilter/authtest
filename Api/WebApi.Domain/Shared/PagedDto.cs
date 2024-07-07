namespace WebApi.Services;

public class PagedDto
{
    public string? SortBy { get; set; }
    public int PageSize { get; set; } = 10; 
    public int Index { get; set; }
}