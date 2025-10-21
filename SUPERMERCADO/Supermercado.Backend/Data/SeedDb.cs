using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Repositories.Implementations;
using Supermercado.Shared.Entities;

namespace Supermercado.Backend.Data;

/// <summary>
/// Clase para creación de base de datos y datos iniciales del sistema ERP O2C
/// </summary>
public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckRolsAsync();
        await CheckCategoria_ProductosAsync();
        await CheckUsersAsync();
        await CheckSellersAsync();
        await CheckCustomersAsync();
        await CheckProductsAsync();
        await CheckOrdersAsync();
    }

    private async Task CheckRolsAsync()
    {
        if (!_context.Rols.Any())
        {
            _context.Rols.Add(new Rol { nombre = "Admin" });
            _context.Rols.Add(new Rol { nombre = "User" });
            _context.Rols.Add(new Rol { nombre = "Seller" });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckCategoria_ProductosAsync()
    {
        if (!_context.Categoria_Productos.Any())
        {
            _context.Categoria_Productos.Add(new Categoria_Producto { descripcion = "Bebidas" });
            _context.Categoria_Productos.Add(new Categoria_Producto { descripcion = "Lacteos" });
            _context.Categoria_Productos.Add(new Categoria_Producto { descripcion = "Aseo" });
            _context.Categoria_Productos.Add(new Categoria_Producto { descripcion = "Comida" });
            _context.Categoria_Productos.Add(new Categoria_Producto { descripcion = "Snacks" });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckUsersAsync()
    {
        if (!_context.Users.Any())
        {
            // Usuarios de prueba (password: "admin123" y "user123")
            _context.Users.Add(new User
            {
                Email = "admin@supermercado.com",
                PasswordHash = UserRepository.HashPassword("admin123"),
                Role = "Admin",
                FullName = "Administrador Sistema",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            _context.Users.Add(new User
            {
                Email = "vendedor@supermercado.com",
                PasswordHash = UserRepository.HashPassword("user123"),
                Role = "Seller",
                FullName = "Juan Vendedor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            _context.Users.Add(new User
            {
                Email = "user@supermercado.com",
                PasswordHash = UserRepository.HashPassword("user123"),
                Role = "User",
                FullName = "Usuario Regular",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckSellersAsync()
    {
        if (!_context.Sellers.Any())
        {
            var sellers = new List<Seller>
            {
                new Seller
                {
                    FullName = "Carlos Rodríguez",
                    Code = "VEND-001",
                    Email = "carlos.rodriguez@supermercado.com",
                    Phone = "+57 300 1234567",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Seller
                {
                    FullName = "María González",
                    Code = "VEND-002",
                    Email = "maria.gonzalez@supermercado.com",
                    Phone = "+57 301 2345678",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Seller
                {
                    FullName = "Pedro Martínez",
                    Code = "VEND-003",
                    Email = "pedro.martinez@supermercado.com",
                    Phone = "+57 302 3456789",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Seller
                {
                    FullName = "Ana López",
                    Code = "VEND-004",
                    Email = "ana.lopez@supermercado.com",
                    Phone = "+57 303 4567890",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Seller
                {
                    FullName = "Luis Fernández",
                    Code = "VEND-005",
                    Email = "luis.fernandez@supermercado.com",
                    Phone = "+57 304 5678901",
                    IsActive = false, // Vendedor inactivo para pruebas
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Sellers.AddRange(sellers);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckCustomersAsync()
    {
        if (!_context.Customers.Any())
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "Distribuidora El Sol S.A.",
                    TaxId = "900123456-1",
                    Email = "ventas@elsol.com",
                    Phone = "3001234567",
                    Address = "Calle 50 #25-30, Bogotá",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    Name = "Supermercados La Luna Ltda.",
                    TaxId = "900234567-2",
                    Email = "compras@laluna.com",
                    Phone = "3012345678",
                    Address = "Carrera 15 #80-45, Medellín",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    Name = "Comercializadora Estrella",
                    TaxId = "900345678-3",
                    Email = "info@estrella.com",
                    Phone = "3023456789",
                    Address = "Avenida 6 #12-20, Cali",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    Name = "Tiendas El Cometa",
                    TaxId = "900456789-4",
                    Email = "pedidos@elcometa.com",
                    Phone = "3034567890",
                    Address = "Calle 100 #15-25, Barranquilla",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    Name = "Minimarket Galaxia",
                    TaxId = "900567890-5",
                    Email = "contacto@galaxia.com",
                    Phone = "3045678901",
                    Address = "Carrera 7 #32-18, Cartagena",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckProductsAsync()
    {
        if (!_context.Products.Any())
        {
            var categorias = await _context.Categoria_Productos.ToListAsync();
            var bebidas = categorias.FirstOrDefault(c => c.descripcion == "Bebidas");
            var lacteos = categorias.FirstOrDefault(c => c.descripcion == "Lacteos");
            var aseo = categorias.FirstOrDefault(c => c.descripcion == "Aseo");
            var comida = categorias.FirstOrDefault(c => c.descripcion == "Comida");
            var snacks = categorias.FirstOrDefault(c => c.descripcion == "Snacks");

            var products = new List<Product>
            {
                new Product
                {
                    Sku = "BEB-001",
                    Name = "Coca Cola 2L",
                    Description = "Bebida gaseosa sabor cola 2 litros",
                    Price = 5500,
                    TaxRatePct = 19,
                    StockQty = 100,
                    CategoriaId = bebidas?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "BEB-002",
                    Name = "Agua Cristal 600ml",
                    Description = "Agua purificada sin gas 600ml",
                    Price = 1500,
                    TaxRatePct = 0,
                    StockQty = 200,
                    CategoriaId = bebidas?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "LAC-001",
                    Name = "Leche Entera Alpina 1L",
                    Description = "Leche entera pasteurizada 1 litro",
                    Price = 3800,
                    TaxRatePct = 0,
                    StockQty = 150,
                    CategoriaId = lacteos?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "LAC-002",
                    Name = "Yogurt Alpina Fresa 200g",
                    Description = "Yogurt sabor fresa 200 gramos",
                    Price = 2500,
                    TaxRatePct = 0,
                    StockQty = 80,
                    CategoriaId = lacteos?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "ASE-001",
                    Name = "Jabón Protex 120g",
                    Description = "Jabón antibacterial 120 gramos",
                    Price = 3200,
                    TaxRatePct = 19,
                    StockQty = 120,
                    CategoriaId = aseo?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "COM-001",
                    Name = "Arroz Diana 500g",
                    Description = "Arroz blanco premium 500 gramos",
                    Price = 2800,
                    TaxRatePct = 0,
                    StockQty = 90,
                    CategoriaId = comida?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "SNK-001",
                    Name = "Papas Margarita 150g",
                    Description = "Papas fritas sabor natural 150g",
                    Price = 4200,
                    TaxRatePct = 19,
                    StockQty = 75,
                    CategoriaId = snacks?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Sku = "SNK-002",
                    Name = "Chocolatina Jet 40g",
                    Description = "Chocolatina con maní 40 gramos",
                    Price = 1800,
                    TaxRatePct = 19,
                    StockQty = 150,
                    CategoriaId = snacks?.categoriaId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Crear movimientos iniciales de inventario
            foreach (var product in products)
            {
                _context.InventoryMoves.Add(new InventoryMove
                {
                    ProductId = product.Id,
                    RefType = "INITIAL",
                    RefId = 0,
                    QtyDelta = product.StockQty,
                    StockAfter = product.StockQty,
                    Notes = "Stock inicial del sistema",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckOrdersAsync()
    {
        if (!_context.Orders.Any())
        {
            var customers = await _context.Customers.ToListAsync();
            var products = await _context.Products.ToListAsync();
            var sellers = await _context.Sellers.Where(s => s.IsActive).ToListAsync();
            var random = new Random();

            // Crear 10 pedidos de ejemplo
            for (int i = 1; i <= 10; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var seller = sellers.Any() ? sellers[random.Next(sellers.Count)] : null;
                var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));
                
                var order = new Order
                {
                    Number = $"ORD-{orderDate:yyyyMMdd}-{i:D6}",
                    CustomerId = customer.Id,
                    SellerId = seller?.Id,
                    Status = i <= 6 ? "CONFIRMED" : (i <= 8 ? "NEW" : "INVOICED"),
                    CreatedAt = orderDate,
                    OrderLines = new List<OrderLine>()
                };

                // Agregar 2-4 líneas por pedido
                var lineCount = random.Next(2, 5);
                decimal subtotal = 0;
                decimal tax = 0;

                for (int j = 0; j < lineCount; j++)
                {
                    var product = products[random.Next(products.Count)];
                    var qty = random.Next(5, 20);
                    var lineSubtotal = product.Price * qty;
                    var lineTax = lineSubtotal * (product.TaxRatePct / 100);

                    order.OrderLines.Add(new OrderLine
                    {
                        ProductId = product.Id,
                        Qty = qty,
                        UnitPrice = product.Price,
                        TaxRatePct = product.TaxRatePct,
                        LineTotal = lineSubtotal + lineTax,
                        LineTax = lineTax
                    });

                    subtotal += lineSubtotal;
                    tax += lineTax;
                }

                order.Subtotal = subtotal;
                order.Tax = tax;
                order.Total = subtotal + tax;

                if (order.Status == "CONFIRMED" || order.Status == "INVOICED")
                {
                    order.ConfirmedAt = orderDate.AddHours(2);
                }

                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync();

            // Crear algunas facturas para pedidos confirmados
            var invoicedOrders = await _context.Orders
                .Include(o => o.OrderLines)
                .Where(o => o.Status == "INVOICED")
                .ToListAsync();

            foreach (var order in invoicedOrders)
            {
                var invoice = new Invoice
                {
                    Number = $"INV-{order.CreatedAt:yyyyMMdd}-{order.Id:D6}",
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    Status = "PENDING",
                    Subtotal = order.Subtotal,
                    Tax = order.Tax,
                    Total = order.Total,
                    DueDate = order.CreatedAt.AddDays(30),
                    CreatedAt = order.ConfirmedAt!.Value.AddHours(1),
                    Notes = "Factura generada automáticamente"
                };

                _context.Invoices.Add(invoice);
            }

            await _context.SaveChangesAsync();
        }
    }
}
