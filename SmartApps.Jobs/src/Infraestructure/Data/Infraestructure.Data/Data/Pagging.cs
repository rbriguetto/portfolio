namespace Infraestructure.Data;

public class Pagging
{
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 1;
    public int Total { get; set; } = 0;
    public string SortColumn { get; set; } = "";
    public string SortDirection { get; set; } = "asc";
    public bool DisableCount { get; set; } = false;

    public int GetStartIndex()
    {
        return (Page) * PageSize;
    }

    public int GetEndIndex()
    {
        var endIndex = ((Page) * PageSize) + PageSize;
        if (endIndex > Total)
        {
            endIndex = Total;
        }
        return endIndex;
    }

    public int GetLastPage()
    {
        var pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Total / PageSize)));
        if (Total % PageSize > 0)
        {
            pageCount++;
        }
        return pageCount;
    }
}