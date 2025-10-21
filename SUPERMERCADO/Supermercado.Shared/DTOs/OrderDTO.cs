using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? SellerId { get; set; }
    public string? SellerName { get; set; }
    public string Status { get; set; } = "NEW";
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public List<OrderLineDTO>? OrderLines { get; set; }
}

public class OrderLineDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRatePct { get; set; }
    public decimal LineTotal { get; set; }
    public decimal LineTax { get; set; }
}

public class CreateOrderDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int CustomerId { get; set; }

    public int? SellerId { get; set; } // Vendedor que crea el pedido

    public string? Notes { get; set; }

    [Required(ErrorMessage = "Debe incluir al menos una línea de pedido")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de pedido")]
    public List<CreateOrderLineDTO> OrderLines { get; set; } = new();
}

public class CreateOrderLineDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Qty { get; set; }
}

public class UpdateOrderDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int CustomerId { get; set; }

    public int? SellerId { get; set; } // Vendedor que modifica el pedido

    public string? Notes { get; set; }

    [Required(ErrorMessage = "Debe incluir al menos una línea de pedido")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de pedido")]
    public List<CreateOrderLineDTO> OrderLines { get; set; } = new();
}
