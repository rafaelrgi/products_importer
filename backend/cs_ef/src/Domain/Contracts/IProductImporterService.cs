namespace cs_ef.src.Domain.Contracts
{
  public interface IProductImporterService
  {
    public Task<(int processed, int rejected)> ImportCsv(IFormFile file);    
  }
}