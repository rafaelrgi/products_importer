namespace cssharp.Repositories
{
  public abstract class BaseRepository<T> where T : class
  {
    protected readonly string connectionString;

    protected BaseRepository() { 
      var builder = WebApplication.CreateBuilder();
      connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";    
    }   

  }
}
