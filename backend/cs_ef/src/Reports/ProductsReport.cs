using cs_ef.src.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

//FIXME: refactor, create base class, etc
namespace cs_ef.src.Reports
{
  public class ProductsReport : IDocument
  {
    readonly IEnumerable<Product> _rows;

    public ProductsReport(IEnumerable<Product> rows)
    {
      _rows = rows;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(1, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(11));


        page.Header()
            .Text("Products Report")
            .SemiBold().FontSize(16).AlignCenter();

        page.Content()
        .Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.ConstantColumn(80);
            columns.RelativeColumn();
            columns.ConstantColumn(90);
            columns.ConstantColumn(100);
          });

          table.Header(header =>
          {
            header.Cell().BorderBottom(2).Padding(4).Text("Qty");
            header.Cell().BorderBottom(2).Padding(4).Text("Product");
            header.Cell().BorderBottom(2).Padding(4).Text("Expires");
            header.Cell().BorderBottom(2).Padding(4).AlignRight().Text("Price");
          });

          foreach (var row in _rows)
          {
            var price = Math.Round(Random.Shared.NextDouble() * 100, 2);

            table.Cell().Padding(3).Text(row.Quantity.ToString()).ClampLines(1);
            table.Cell().Padding(3).Text(row.Name).ClampLines(1);
            table.Cell().Padding(3).Text(row.Expiration.ToString("yyyy-MM-dd")).ClampLines(1);
            table.Cell().Padding(3).AlignRight().Text(row.Price.ToString()).ClampLines(1);
          }
        });

        page.Footer()
            .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Medium))
            .Row(row =>
            {
              row.RelativeItem()
              .Text(text =>
              {
                text.Span("Page ").FontSize(10).FontColor(Colors.Grey.Medium);
                text.CurrentPageNumber().FontSize(10).FontColor(Colors.Grey.Medium);
                text.Span(" of ").FontSize(10).FontColor(Colors.Grey.Medium);
                text.TotalPages().FontSize(10).FontColor(Colors.Grey.Medium);
              });

              row.RelativeItem().AlignRight()
              .Text(DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                  .FontSize(10)
                  .FontColor(Colors.Grey.Medium); ;
            });
      });
    }
  }
}
