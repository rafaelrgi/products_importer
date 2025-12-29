namespace cs_ef.src.Domain.Contracts
{
  public interface IImportProductService
  {
    public Task<(int processed, int rejected)> ImportCsv(IFormFile file);    
  }
}