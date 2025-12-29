using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs_api.src.Domain.Entities
{
  [Table("exchange_rate")]
  public class ExchangeRate
  {    
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }

    [Required, Column(TypeName = "char(3)"), MaxLength(3) ]
    public string Abbreviation { get; set; } = "";

    [Required]
    public decimal Rate { get; set; }
  }
}
