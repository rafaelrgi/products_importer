namespace cs_api.Dtos
{
  public class PaginationDto<T>
  {
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int PageCount { get; set; }
    public int RecordCount { get; set; }
    public bool HasData { get { return Data != null && Data.Any(); } }
    public IEnumerable<T>? Data { get; set; } = null;
        
  }
}
