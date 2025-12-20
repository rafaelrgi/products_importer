namespace cssharp.Repositories
{
  public abstract class BaseRepository<T> where T : class
  {
    protected readonly string connectionString;

    protected BaseRepository() { 
      bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_DOCKER") == "true";
      string connection = isRunningInDocker? "DockerConnection" : "DefaultConnection";
      var builder = WebApplication.CreateBuilder();
      connectionString = builder.Configuration.GetConnectionString(connection) ?? "";    
    }   

  }
}
