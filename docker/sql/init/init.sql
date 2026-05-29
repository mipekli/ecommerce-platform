-- Identity Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IdentityDb')
BEGIN
    CREATE DATABASE [IdentityDb];
END
GO

USE [IdentityDb];
GO

-- Product Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProductDb')
BEGIN
    CREATE DATABASE [ProductDb];
END
GO

-- Order Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderDb')
BEGIN
    CREATE DATABASE [OrderDb];
END
GO

-- Inventory Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InventoryDb')
BEGIN
    CREATE DATABASE [InventoryDb];
END
GO
