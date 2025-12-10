using InvoiceExtractor.Api.Database;
using System.Text.Json;

namespace InvoiceExtractor.Api.Services;

public interface IInvoiceParser
{
    void MapJsonToEntity(string json, InvoiceRecord record);
}

public class InvoiceParser : IInvoiceParser
{
    public void MapJsonToEntity(string json, InvoiceRecord record)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Mapping difensivo
            if (root.TryGetProperty("total", out var totalEl))
            {
                var val = totalEl.ToString().Replace(",", ".");
                if (decimal.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                    record.TotalAmount = d;
            }

            if (root.TryGetProperty("number", out var numEl))
                record.InvoiceNumber = numEl.ToString();

            if (root.TryGetProperty("supplier", out var supEl))
                record.SupplierName = supEl.ToString();

            if (root.TryGetProperty("date", out var dateEl))
            {
                if (DateTime.TryParse(dateEl.ToString(), out var dt))
                    record.InvoiceDate = dt;
            }
        }
        catch (JsonException)
        {
            // Logic: if the JSON is broken, we don't crash, we just save what we can (or nothing).
            // The parsing error should not stop the raw record from being saved.
        }
    }
}