namespace cssharp.Models
{
  public class Product
  {
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public decimal Price { get; set; }

    public DateTime Expiration { get; set; }

    public DateTime CreatedAt { get; set; }

    // Brazilian Reais  
    public decimal BRL { get; set; }

    // Euro
    public decimal EUR { get; set; }

    // Canadian dollar
    public decimal CAD { get; set; }

    // Mexican Pesos
    public decimal MXN { get; set; }

    // Libras
    public decimal GBP { get; set; }
  }
}
