using InvoiceExtractor.Api.Configuration;
using InvoiceExtractor.Api.Database;
using InvoiceExtractor.Api.Endpoint;
using InvoiceExtractor.Api.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Invoice Extractor API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Config
    builder.Services.Configure<AiSettings>(builder.Configuration.GetSection("AiSettings"));

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // AI Service + Polly
    var retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    builder.Services.AddHttpClient<IAiExtractorService, OllamaService>()
        .AddPolicyHandler(retryPolicy);

    builder.Services.AddSingleton<IInvoiceParser, InvoiceParser>();

    // Add services to the container.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Auto-Migration
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            dbContext.Database.Migrate();
        }
        catch
        {
            /* Log */
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    // Mapping Endpoints
    app.MapInvoiceEndpoints();

    //app.UseHttpsRedirection();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}