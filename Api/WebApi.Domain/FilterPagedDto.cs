namespace WebApi.Services;

public class FilterPagedDto
{
    public int PageSize { get; set; } = 10; 
    public int Index { get; set; }
}