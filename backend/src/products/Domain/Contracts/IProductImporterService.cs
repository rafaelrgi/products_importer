namespace products.Domain.Contracts
{
  public interface IProductImporterService
  {
    public Task<(int processed, int rejected)> ImportCsv(IFormFile file);    
  }
}