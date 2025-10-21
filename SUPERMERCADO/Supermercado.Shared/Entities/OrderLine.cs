using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermercado.Shared.Entities;

public class OrderLine
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int Qty { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRatePct { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTax { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
