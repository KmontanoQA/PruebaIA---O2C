namespace Supermercado.Shared.DTOs;

public class SalesReportDTO
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalOrders { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalNet { get; set; }
    public List<SalesReportLineDTO> Details { get; set; } = new();
}

public class SalesReportLineDTO
{
    public DateTime Date { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public string OrderNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = null!;
}

public class ReceivablesReportDTO
{
    public DateTime AsOf { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalOverdue { get; set; }
    public List<ReceivablesReportLineDTO> Details { get; set; } = new();
}

public class ReceivablesReportLineDTO
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public decimal Total { get; set; }
    public int DaysOverdue { get; set; }
    public string Status { get; set; } = null!;
}

public class InventoryReportDTO
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int CurrentStock { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public List<InventoryMoveDTO> RecentMoves { get; set; } = new();
}

public class InventoryMoveDTO
{
    public int Id { get; set; }
    public string RefType { get; set; } = null!;
    public int RefId { get; set; }
    public int QtyDelta { get; set; }
    public int StockAfter { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
}
