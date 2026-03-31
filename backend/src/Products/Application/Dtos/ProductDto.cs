namespace Products.Application.Dtos
{
  public record ProductDto
    (
    int Id,
    string Name,
    int Quantity,
    decimal Price,
    DateTime Expiration,
    decimal BRL,
    decimal EUR,
    decimal CAD,
    decimal MXN,
    decimal GBP,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt
  );
}
