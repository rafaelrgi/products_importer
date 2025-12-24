namespace cs_api.src.Application.Dtos
{
  public class UsdRatesDto
  {
    // Brazilian Reais
    public decimal Brl { get; set; }
    // Euro
    public decimal Eur { get; set; }
    // Canadian dollar
    public decimal Cad { get; set; }
    // Mexican Pesos
    public decimal Mxn { get; set; }
    // Libras
    public decimal Gbp { get; set; }
  }
  public class ExchangeRatesDto
  {
    public DateTime? Date { get; set; }
    public UsdRatesDto? Rates { get; set; }
  }
}
