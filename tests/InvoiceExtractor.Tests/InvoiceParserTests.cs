using InvoiceExtractor.Api.Database;
using InvoiceExtractor.Api.Services;

namespace InvoiceExtractor.Tests;

public class InvoiceParserTests
{
    private readonly InvoiceParser _parser;

    public InvoiceParserTests()
    {
        _parser = new InvoiceParser();
    }

    [Fact]
    public void MapJsonToEntity_Should_Map_Valid_Json()
    {
        // Arrange
        var json = """
                   {
                       "supplier": "Amazon EU",
                       "number": "INV-12345",
                       "date": "2023-10-25",
                       "total": 45.50
                   }
                   """;
        var record = new InvoiceRecord();

        // Act
        _parser.MapJsonToEntity(json, record);

        // Assert
        Assert.Equal("Amazon EU", record.SupplierName);
        Assert.Equal("INV-12345", record.InvoiceNumber);
        Assert.Equal(45.50m, record.TotalAmount);
        Assert.Equal(new DateTime(2023, 10, 25), record.InvoiceDate);
    }

    [Fact]
    public void MapJsonToEntity_Should_Handle_Decimal_With_Comma()
    {
        // Arrange
        var json = """ { "total": "45,99" } """;
        var record = new InvoiceRecord();

        // Act
        _parser.MapJsonToEntity(json, record);

        // Assert
        Assert.Equal(45.99m, record.TotalAmount);
    }

    [Fact]
    public void MapJsonToEntity_Should_Be_Resilient_To_Empty_Json()
    {
        // Arrange
        var json = "{}";
        var record = new InvoiceRecord();

        // Act
        _parser.MapJsonToEntity(json, record);

        // Assert
        Assert.Null(record.TotalAmount); 
    }
}
