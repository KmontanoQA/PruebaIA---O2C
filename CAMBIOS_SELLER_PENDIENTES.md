# Cambios Pendientes para Implementación de Seller

## ✅ Archivos Ya Creados/Actualizados

1. ✅ `Supermercado.Shared/Entities/Seller.cs` - Entidad creada
2. ✅ `Supermercado.Shared/Entities/Order.cs` - Agregado `SellerId` y relación
3. ✅ `Supermercado.Shared/Entities/OrderAudit.cs` - Agregado `SellerId` y relación
4. ✅ `Supermercado.Shared/DTOs/SellerDTO.cs` - DTOs creados
5. ✅ `Supermercado.Shared/DTOs/OrderDTO.cs` - Agregados `SellerId` y `SellerName`
6. ✅ `Supermercado.Backend/Data/DataContext.cs` - Agregado `DbSet<Seller>` y configuraciones

## ⚠️ Archivos que Debes Crear Manualmente

### 1. ISellerRepository.cs
**Ruta**: `Supermercado.Backend/Repositories/Interfaces/ISellerRepository.cs`

```csharp
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface ISellerRepository : IGenericRepository<Seller>
{
    Task<ActionResponse<Seller>> GetByCodeAsync(string code);
    Task<ActionResponse<PaginationDTO<Seller>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
}
```

### 2. SellerRepository.cs
**Ruta**: `Supermercado.Backend/Repositories/Implementations/SellerRepository.cs`

Ver archivo completo en: `c:/Projects/Ordenes/SUPERMERCADO/Supermercado.Backend/Repositories/Implementations/SellerRepository.cs`

### 3. ISellerUnitOfWork.cs
**Ruta**: `Supermercado.Backend/UnitsOfWork/Interfaces/ISellerUnitOfWork.cs`

```csharp
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface ISellerUnitOfWork
{
    Task<ActionResponse<SellerDTO>> GetByIdAsync(int id);
    Task<ActionResponse<IEnumerable<SellerDTO>>> GetAllAsync();
    Task<ActionResponse<PaginationDTO<SellerDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
    Task<ActionResponse<SellerDTO>> CreateAsync(CreateSellerDTO dto);
    Task<ActionResponse<SellerDTO>> UpdateAsync(int id, SellerDTO dto);
    Task<ActionResponse<bool>> DeleteAsync(int id);
}
```

### 4. SellerUnitOfWork.cs
**Ruta**: `Supermercado.Backend/UnitsOfWork/Implementations/SellerUnitOfWork.cs`

Ver archivo completo en: `c:/Projects/Ordenes/SUPERMERCADO/Supermercado.Backend/UnitsOfWork/Implementations/SellerUnitOfWork.cs`

### 5. SellersController.cs
**Ruta**: `Supermercado.Backend/Controllers/SellersController.cs`

Ver archivo completo en: `c:/Projects/Ordenes/SUPERMERCADO/Supermercado.Backend/Controllers/SellersController.cs`

## ⚠️ Archivos que Debes Modificar

### 1. Program.cs
**Ruta**: `Supermercado.Backend/Program.cs`

**Buscar la sección de repositorios y agregar:**
```csharp
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
```

**Buscar la sección de Units of Work y agregar:**
```csharp
builder.Services.AddScoped<ISellerUnitOfWork, SellerUnitOfWork>();
```

### 2. OrderRepository.cs
**Ruta**: `Supermercado.Backend/Repositories/Implementations/OrderRepository.cs`

**En el método `GetOrderWithDetailsAsync`, cambiar:**
```csharp
var order = await _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Seller)  // ← AGREGAR ESTA LÍNEA
    .Include(o => o.OrderLines!)
        .ThenInclude(ol => ol.Product)
    .FirstOrDefaultAsync(o => o.Id == id);
```

**En el método `GetPaginatedAsync`, cambiar:**
```csharp
var query = _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Seller)  // ← AGREGAR ESTA LÍNEA
    .AsQueryable();
```

### 3. OrderUnitOfWork.cs
**Ruta**: `Supermercado.Backend/UnitsOfWork/Implementations/OrderUnitOfWork.cs`

**En el método `CreateOrderAsync`, después de validar el cliente, agregar:**
```csharp
// Validar vendedor si se proporciona
if (dto.SellerId.HasValue)
{
    var seller = await _context.Sellers.FindAsync(dto.SellerId.Value);
    if (seller == null)
    {
        return new ActionResponse<OrderDTO>
        {
            WasSuccess = false,
            Message = "Vendedor no encontrado"
        };
    }

    if (!seller.IsActive)
    {
        return new ActionResponse<OrderDTO>
        {
            WasSuccess = false,
            Message = "El vendedor está inactivo"
        };
    }
}
```

**Al crear el pedido, agregar:**
```csharp
var order = new Order
{
    Number = orderNumber,
    CustomerId = dto.CustomerId,
    SellerId = dto.SellerId,  // ← AGREGAR ESTA LÍNEA
    Status = "NEW",
    Notes = dto.Notes,
    CreatedAt = DateTime.UtcNow,
    OrderLines = new List<OrderLine>()
};
```

**En el método `MapToDTO`, agregar:**
```csharp
private OrderDTO MapToDTO(Order order)
{
    return new OrderDTO
    {
        Id = order.Id,
        Number = order.Number,
        CustomerId = order.CustomerId,
        CustomerName = order.Customer?.Name,
        SellerId = order.SellerId,           // ← AGREGAR ESTA LÍNEA
        SellerName = order.Seller?.FullName, // ← AGREGAR ESTA LÍNEA
        Status = order.Status,
        // ... resto de propiedades
    };
}
```

### 4. SeedDb.cs
**Ruta**: `Supermercado.Backend/Data/SeedDb.cs`

**En el método `SeedAsync`, agregar:**
```csharp
public async Task SeedAsync()
{
    await _context.Database.EnsureCreatedAsync();
    await CheckRolsAsync();
    await CheckCategoria_ProductosAsync();
    await CheckUsersAsync();
    await CheckSellersAsync();  // ← AGREGAR ESTA LÍNEA
    await CheckCustomersAsync();
    await CheckProductsAsync();
    await CheckOrdersAsync();
}
```

**Agregar el método completo:**
```csharp
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
```

**En el método `CheckOrdersAsync`, modificar:**
```csharp
private async Task CheckOrdersAsync()
{
    if (!_context.Orders.Any())
    {
        var customers = await _context.Customers.ToListAsync();
        var products = await _context.Products.ToListAsync();
        var sellers = await _context.Sellers.Where(s => s.IsActive).ToListAsync();  // ← AGREGAR ESTA LÍNEA
        var random = new Random();

        for (int i = 1; i <= 10; i++)
        {
            var customer = customers[random.Next(customers.Count)];
            var seller = sellers.Any() ? sellers[random.Next(sellers.Count)] : null;  // ← AGREGAR ESTA LÍNEA
            var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));
            
            var order = new Order
            {
                Number = $"ORD-{orderDate:yyyyMMdd}-{i:D6}",
                CustomerId = customer.Id,
                SellerId = seller?.Id,  // ← AGREGAR ESTA LÍNEA
                Status = i <= 6 ? "CONFIRMED" : (i <= 8 ? "NEW" : "INVOICED"),
                CreatedAt = orderDate,
                OrderLines = new List<OrderLine>()
            };
            // ... resto del código
        }
    }
}
```

## 🚀 Pasos Finales

1. **Compilar el proyecto** para verificar que no hay errores:
   ```powershell
   cd c:/Projects/Ordenes/PruebaIA---O2C/SUPERMERCADO
   dotnet build
   ```

2. **Crear la migración**:
   ```powershell
   dotnet ef migrations add AddSellerEntity --project Supermercado.Backend
   ```

3. **Aplicar la migración**:
   ```powershell
   dotnet ef database update --project Supermercado.Backend
   ```

4. **Ejecutar la aplicación**:
   ```powershell
   cd Supermercado.Backend
   dotnet run
   ```

5. **Verificar en Swagger**:
   - Abrir: `http://localhost:5000/swagger`
   - Buscar el controlador `Sellers`
   - Probar los endpoints

## 📝 Endpoints Disponibles

- `GET /api/v1/sellers` - Listar vendedores activos
- `GET /api/v1/sellers/paginated` - Paginado con filtros
- `GET /api/v1/sellers/{id}` - Obtener por ID
- `POST /api/v1/sellers` - Crear vendedor
- `PUT /api/v1/sellers/{id}` - Actualizar vendedor
- `DELETE /api/v1/sellers/{id}` - Eliminar vendedor

## ✅ Verificación

Después de aplicar todos los cambios, verifica:

1. ✅ La tabla `Sellers` existe en la base de datos
2. ✅ Los 5 vendedores de prueba se crearon
3. ✅ Los pedidos tienen `SellerId` asignado
4. ✅ Los endpoints de Sellers funcionan correctamente
5. ✅ Al crear un pedido puedes asignar un vendedor
6. ✅ La auditoría registra el vendedor en las acciones
