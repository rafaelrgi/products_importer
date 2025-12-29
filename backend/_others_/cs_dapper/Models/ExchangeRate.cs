namespace cssharp.Models
{
  public class ExchangeRate
  {
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Abbreviation { get; set; } = "";

    public decimal Rate { get; set; }
  }
}
