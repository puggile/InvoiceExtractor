using InvoiceExtractor.Api.Configuration;
using InvoiceExtractor.Api.Database;
using InvoiceExtractor.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InvoiceExtractor.Api.Endpoint;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices")
                       .WithTags("Invoices")
                       .WithOpenApi()
                       .DisableAntiforgery();

        group.MapGet("/", GetAllInvoices);

        group.MapPost("/analyze", AnalyzeInvoice);
    }

    private static async Task<IResult> GetAllInvoices(
        ApplicationDbContext db,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("InvoiceEndpoints.Get");

        logger.LogInformation("Retrieving all invoices...");

        var invoices = await db.Invoices
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        logger.LogInformation("Found {Count} invoices.", invoices.Count);

        return Results.Ok(invoices);
    }

    private static async Task<IResult> AnalyzeInvoice(
        IFormFile file,
        IAiExtractorService aiService,
        ApplicationDbContext db,
        IInvoiceParser parser, 
        IOptions<AiSettings> settings,
        ILogger<InvoiceRecord> logger
        )
    {
        if (file.Length == 0)
        {
            logger.LogWarning("Upload attempt with empty file");
            return Results.BadRequest("Nessun file caricato");
        }

        logger.LogInformation("Starting analysis for file: {FileName} ({Size} bytes)", file.FileName, file.Length);

        // Basic extension validation
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            logger.LogWarning("Upload invalid format file: {ext}", ext);
            return Results.BadRequest("Formato non supportato. Usa JPG o PNG.");
        }
            
        string jsonResult;
        try
        {
            await using var stream = file.OpenReadStream();
            jsonResult = await aiService.ExtractDataAsync(stream, settings.Value.SystemPrompt);
            logger.LogDebug("AI Response received. Length: {Length}", jsonResult.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI Extraction failed for file {FileName}", file.FileName);
            return Results.Problem($"Errore durante l'analisi AI: {ex.Message}");
        }

        // Entity creation
        var record = new InvoiceRecord
        {
            FileName = file.FileName,
            RawJson = jsonResult,
            CreatedAt = DateTime.UtcNow
        };

        // Parsing Logic
        parser.MapJsonToEntity(jsonResult, record);

        db.Invoices.Add(record);
        await db.SaveChangesAsync();

        logger.LogInformation("Invoice saved successfully. ID: {Id}, Supplier: {Supplier}", record.Id, record.SupplierName);

        return Results.Ok(record);
    }
}