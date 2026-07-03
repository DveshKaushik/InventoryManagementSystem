CREATE DATABASE IF NOT EXISTS InventoryManagementDB;
USE InventoryManagementDB;

-- 1. Create Categories Table
CREATE TABLE IF NOT EXISTS Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(500) NULL
);

-- 2. Create Suppliers Table
CREATE TABLE IF NOT EXISTS Suppliers (
    SupplierId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Contact VARCHAR(100) NULL,
    Phone VARCHAR(50) NULL,
    Email VARCHAR(150) NULL,
    Address VARCHAR(255) NULL
);

-- 3. Create Products Table
CREATE TABLE IF NOT EXISTS Products (
    ProductId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryId INT NOT NULL,
    SupplierId INT NOT NULL,
    Name VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    MinStockLevel INT NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    Description VARCHAR(500) NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
);

-- 4. Create StockMovements Table
CREATE TABLE IF NOT EXISTS StockMovements (
    MovementId INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    MovementType VARCHAR(10) NOT NULL, -- 'IN' or 'OUT'
    Quantity INT NOT NULL,
    MovementDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Reason VARCHAR(255) NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- 5. Seed Initial Data
INSERT INTO Categories (Name, Description) VALUES
('Raw Materials', 'Petrochemical and industrial raw materials'),
('Packaging', 'Containers, drums, and packaging items'),
('Safety Equipment', 'Personal protective equipment and safety tools')
ON DUPLICATE KEY UPDATE Name=Name;

INSERT INTO Suppliers (Name, Contact, Phone, Email, Address) VALUES
('Reliance Industries', 'Alok Mehta', '+91 98765 43210', 'alok.mehta@ril.com', 'Mumbai, India'),
('Supreme Packaging', 'Karan Johar', '+91 87654 32109', 'karan@supremepack.com', 'Gujarat, India'),
('3M Safety Solutions', 'Sarah Conner', '+1 555-0199', 'support@3msafety.com', 'Minnesota, USA')
ON DUPLICATE KEY UPDATE Name=Name;

INSERT INTO Products (Name, CategoryId, SupplierId, Quantity, MinStockLevel, UnitPrice, Description) VALUES
('Benzene', 1, 1, 500, 100, 85.50, 'Industrial solvent and chemical intermediate'),
('Plastic Drums 200L', 2, 2, 45, 50, 1200.00, 'Heavy-duty blue plastic storage drums'),
('Safety Goggles', 3, 3, 120, 20, 350.00, 'Anti-fog protective eyewear'),
('Ethylene', 1, 1, 12, 30, 95.00, 'Compressed gaseous hydrocarbon')
ON DUPLICATE KEY UPDATE Name=Name;

INSERT INTO StockMovements (ProductId, MovementType, Quantity, Reason) VALUES
(1, 'IN', 500, 'Initial inventory load'),
(2, 'IN', 50, 'Purchase from Supreme Packaging'),
(2, 'OUT', 5, 'Damaged during unloading'),
(3, 'IN', 120, 'Safety audit compliance stock'),
(4, 'IN', 12, 'Initial production batch')
ON DUPLICATE KEY UPDATE ProductId=ProductId;
