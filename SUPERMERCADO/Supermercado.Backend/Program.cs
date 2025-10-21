using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Implementations;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Implementations;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using System.Text;


// Crear el builder para configurar la aplicación web
var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURACIÓN DE SERVICIOS =====

// Registrar los controladores de API para manejar las peticiones HTTP
builder.Services.AddControllers();

// Habilitar la exploración de endpoints para Swagger
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger con autenticación JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Supermercado ERP O2C API", 
        Version = "v1",
        Description = "API REST para sistema ERP Order-to-Cash con .NET 9"
    });
    
    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Puede enviar: \"Bearer {token}\" o solo \"{token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar Entity Framework con SQL Server
// Utiliza la cadena de conexión "DefaultConnection" del archivo appsettings.json
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKey_ChangeInProduction_MinLength32Characters!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SupermercadoAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SupermercadoClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    context.Token = authHeader.Trim();
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configurar CORS para permitir consumo desde Visual FoxPro y otros clientes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Total-Count");
    });
});

// Registrar el servicio para poblar datos iniciales en la base de datos
builder.Services.AddTransient<SeedDb>();

// ===== INYECCIÓN DE DEPENDENCIAS =====
// Implementación del patrón Repository y Unit of Work para separar la lógica de acceso a datos

// Registrar la unidad de trabajo genérica - coordina transacciones y operaciones sobre múltiples repositorios
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

// Registrar el repositorio genérico - permite operaciones CRUD reutilizables para cualquier entidad
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Registrar repositorios específicos
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Registrar Units of Work específicos
builder.Services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
builder.Services.AddScoped<ICustomerUnitOfWork, CustomerUnitOfWork>();
builder.Services.AddScoped<ISellerUnitOfWork, SellerUnitOfWork>();
builder.Services.AddScoped<IProductUnitOfWork, ProductUnitOfWork>();
builder.Services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();
builder.Services.AddScoped<IInvoiceUnitOfWork, InvoiceUnitOfWork>();

// Construir la aplicación con todas las configuraciones anteriores
var app = builder.Build();

// Ejecutar el proceso de población de datos iniciales al iniciar la aplicación y crecion de base de datos
SeedData(app);

/// <summary>
/// Método para poblar la base de datos con datos iniciales
/// Se ejecuta al iniciar la aplicación para asegurar que existan datos de prueba
/// </summary>
/// <param name="app">La aplicación web construida</param>
void SeedData(WebApplication app)
{
    // Obtener el factory para crear scopes de servicios
    var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

    // Crear un scope para resolver dependencias de forma controlada
    using var scope = scopeFactory.CreateScope();
    
    // Obtener el servicio SeedDb para poblar datos
    var service = scope.ServiceProvider.GetRequiredService<SeedDb>();
    
    // Ejecutar la población de datos de forma síncrona
    service!.SeedAsync().Wait();
}

// ===== CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE =====

// Habilitar CORS
app.UseCors("AllowAll");

// Solo habilitar Swagger en ambiente de desarrollo por seguridad
if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger para generar la documentación JSON de la API
    app.UseSwagger();
    
    // Habilitar Swagger UI para la interfaz web interactiva de la API
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supermercado ERP O2C API v1");
    });
}

// Habilitar autenticación y autorización JWT
app.UseAuthentication();
app.UseAuthorization();

// Mapear los controladores para que respondan a las rutas configuradas
app.MapControllers();

// Iniciar la aplicación y comenzar a escuchar peticiones
app.Run();
