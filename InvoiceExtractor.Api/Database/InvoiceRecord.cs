using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceExtractor.Api.Database;

public class InvoiceRecord
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string FileName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }

    [MaxLength(200)]
    public string? SupplierName { get; set; }

    public string RawJson { get; set; } = string.Empty;
}