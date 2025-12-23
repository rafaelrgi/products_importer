using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs_api.Models
{
  [Table("product")]
  public class Product
  {
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime Expiration { get; set; }

    [Required]
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
