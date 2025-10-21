-- =============================================
-- Script de creación de base de datos
-- Sistema ERP O2C - Supermercado
-- SQL Server 2022+
-- =============================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SUPERMERCADO')
BEGIN
    CREATE DATABASE SUPERMERCADO;
END
GO

USE SUPERMERCADO;
GO

-- =============================================
-- TABLAS PRINCIPALES
-- =============================================

-- Tabla de Roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Rols')
BEGIN
    CREATE TABLE Rols (
        rol_id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(100) NOT NULL UNIQUE,
        CONSTRAINT CK_Rols_nombre CHECK (LEN(nombre) > 0)
    );
END
GO

-- Tabla de Categorías de Productos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categoria_Productos')
BEGIN
    CREATE TABLE Categoria_Productos (
        categoriaId INT IDENTITY(1,1) PRIMARY KEY,
        descripcion NVARCHAR(250) NOT NULL UNIQUE,
        CONSTRAINT CK_Categoria_descripcion CHECK (LEN(descripcion) > 0)
    );
END
GO

-- Tabla de Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL,
        FullName NVARCHAR(100) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT CK_Users_Email CHECK (Email LIKE '%@%'),
        CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'User', 'Seller'))
    );
    CREATE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_IsActive ON Users(IsActive);
END
GO

-- Tabla de Clientes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        TaxId NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(150) NULL,
        Phone NVARCHAR(20) NULL,
        Address NVARCHAR(300) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT CK_Customers_Name CHECK (LEN(Name) > 0),
        CONSTRAINT CK_Customers_TaxId CHECK (LEN(TaxId) > 0)
    );
    CREATE INDEX IX_Customers_TaxId ON Customers(TaxId);
    CREATE INDEX IX_Customers_IsActive ON Customers(IsActive);
END
GO

-- Tabla de Productos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Sku NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        Price DECIMAL(18,2) NOT NULL,
        TaxRatePct DECIMAL(5,2) NOT NULL DEFAULT 0,
        StockQty INT NOT NULL DEFAULT 0,
        CategoriaId INT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Products_Categoria FOREIGN KEY (CategoriaId) 
            REFERENCES Categoria_Productos(categoriaId) ON DELETE SET NULL,
        CONSTRAINT CK_Products_Sku CHECK (LEN(Sku) > 0),
        CONSTRAINT CK_Products_Name CHECK (LEN(Name) > 0),
        CONSTRAINT CK_Products_Price CHECK (Price >= 0),
        CONSTRAINT CK_Products_TaxRatePct CHECK (TaxRatePct >= 0 AND TaxRatePct <= 100),
        CONSTRAINT CK_Products_StockQty CHECK (StockQty >= 0)
    );
    CREATE INDEX IX_Products_Sku ON Products(Sku);
    CREATE INDEX IX_Products_IsActive ON Products(IsActive);
    CREATE INDEX IX_Products_CategoriaId ON Products(CategoriaId);
END
GO

-- Tabla de Pedidos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Number NVARCHAR(50) NOT NULL UNIQUE,
        CustomerId INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'NEW',
        Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL DEFAULT 0,
        Notes NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ConfirmedAt DATETIME2 NULL,
        CancelledAt DATETIME2 NULL,
        CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerId) 
            REFERENCES Customers(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_Orders_Status CHECK (Status IN ('NEW', 'CONFIRMED', 'INVOICED', 'CANCELLED')),
        CONSTRAINT CK_Orders_Subtotal CHECK (Subtotal >= 0),
        CONSTRAINT CK_Orders_Tax CHECK (Tax >= 0),
        CONSTRAINT CK_Orders_Total CHECK (Total >= 0)
    );
    CREATE INDEX IX_Orders_Number ON Orders(Number);
    CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
    CREATE INDEX IX_Orders_Status ON Orders(Status);
    CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
END
GO

-- Tabla de Líneas de Pedido
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderLines')
BEGIN
    CREATE TABLE OrderLines (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Qty INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        TaxRatePct DECIMAL(5,2) NOT NULL,
        LineTotal DECIMAL(18,2) NOT NULL,
        LineTax DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderLines_Order FOREIGN KEY (OrderId) 
            REFERENCES Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderLines_Product FOREIGN KEY (ProductId) 
            REFERENCES Products(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_OrderLines_Qty CHECK (Qty > 0),
        CONSTRAINT CK_OrderLines_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_OrderLines_TaxRatePct CHECK (TaxRatePct >= 0 AND TaxRatePct <= 100)
    );
    CREATE INDEX IX_OrderLines_OrderId ON OrderLines(OrderId);
    CREATE INDEX IX_OrderLines_ProductId ON OrderLines(ProductId);
END
GO

-- Tabla de Facturas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoices')
BEGIN
    CREATE TABLE Invoices (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Number NVARCHAR(50) NOT NULL UNIQUE,
        OrderId INT NOT NULL,
        CustomerId INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
        Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL DEFAULT 0,
        DueDate DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PaidAt DATETIME2 NULL,
        CancelledAt DATETIME2 NULL,
        Notes NVARCHAR(500) NULL,
        CONSTRAINT FK_Invoices_Order FOREIGN KEY (OrderId) 
            REFERENCES Orders(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Invoices_Customer FOREIGN KEY (CustomerId) 
            REFERENCES Customers(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_Invoices_Status CHECK (Status IN ('PENDING', 'PAID', 'CANCELLED')),
        CONSTRAINT CK_Invoices_Subtotal CHECK (Subtotal >= 0),
        CONSTRAINT CK_Invoices_Tax CHECK (Tax >= 0),
        CONSTRAINT CK_Invoices_Total CHECK (Total >= 0)
    );
    CREATE INDEX IX_Invoices_Number ON Invoices(Number);
    CREATE INDEX IX_Invoices_OrderId ON Invoices(OrderId);
    CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
    CREATE INDEX IX_Invoices_Status ON Invoices(Status);
    CREATE INDEX IX_Invoices_DueDate ON Invoices(DueDate);
    CREATE INDEX IX_Invoices_CreatedAt ON Invoices(CreatedAt);
END
GO

-- Tabla de Movimientos de Inventario
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InventoryMoves')
BEGIN
    CREATE TABLE InventoryMoves (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NOT NULL,
        RefType NVARCHAR(50) NOT NULL,
        RefId INT NOT NULL,
        QtyDelta INT NOT NULL,
        StockAfter INT NOT NULL,
        Notes NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_InventoryMoves_Product FOREIGN KEY (ProductId) 
            REFERENCES Products(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_InventoryMoves_RefType CHECK (RefType IN ('ORDER', 'ADJUST', 'RETURN', 'INITIAL'))
    );
    CREATE INDEX IX_InventoryMoves_ProductId_CreatedAt ON InventoryMoves(ProductId, CreatedAt);
    CREATE INDEX IX_InventoryMoves_RefType ON InventoryMoves(RefType);
END
GO

PRINT 'Schema creado exitosamente';
GO
