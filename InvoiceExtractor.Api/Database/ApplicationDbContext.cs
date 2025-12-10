using Microsoft.EntityFrameworkCore;

namespace InvoiceExtractor.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<InvoiceRecord> Invoices { get; set; }
}