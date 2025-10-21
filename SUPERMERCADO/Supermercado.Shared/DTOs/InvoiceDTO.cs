using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class InvoiceDTO
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string Status { get; set; } = "PENDING";
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateInvoiceDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public DateTime DueDate { get; set; }

    public string? Notes { get; set; }
}
