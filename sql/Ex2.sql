CREATE DATABASE EX2
GO

USE EX2
GO

CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL
)
GO

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATE NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
)
GO

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL
)
GO

CREATE TABLE OrderItems (
    OrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
)
GO

CREATE TABLE Shipments (
    ShipmentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ShipmentDate DATE NOT NULL DEFAULT GETDATE(),
    DeliveryStatus NVARCHAR(50) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
)
GO

CREATE PROCEDURE GetComplexOrderReport
    @StartDate DATE,              
    @EndDate DATE,                
    @CustomerID INT = NULL,        
    @MinAmount DECIMAL(10,2) = NULL,  
    @MaxAmount DECIMAL(10,2) = NULL,  
    @OrderStatus NVARCHAR(50) = NULL,  
    @ShipmentStatus NVARCHAR(50) = NULL 
AS
BEGIN
    SET NOCOUNT ON

    SELECT 
        o.OrderID,
        o.OrderDate,
        c.Name AS CustomerName,
        SUM(oi.Quantity * oi.Price) AS TotalAmount,   
        SUM(oi.Quantity) AS TotalQuantity,            
        s.ShipmentDate,
        s.DeliveryStatus,
        o.Status AS OrderStatus
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
    INNER JOIN OrderItems oi ON o.OrderID = oi.OrderID
    LEFT JOIN Shipments s ON o.OrderID = s.OrderID
    WHERE 
        o.OrderDate BETWEEN @StartDate AND @EndDate  
        AND (@CustomerID IS NULL OR o.CustomerID = @CustomerID)  
        AND (@OrderStatus IS NULL OR o.Status = @OrderStatus)    
        AND (@ShipmentStatus IS NULL OR s.DeliveryStatus = @ShipmentStatus)
    GROUP BY 
        o.OrderID, o.OrderDate, c.Name, s.ShipmentDate, s.DeliveryStatus, o.Status
    HAVING 
        (@MinAmount IS NULL OR SUM(oi.Quantity * oi.Price) >= @MinAmount)  
        AND (@MaxAmount IS NULL OR SUM(oi.Quantity * oi.Price) <= @MaxAmount)
    ORDER BY o.OrderDate DESC  
END
GO

-- Chèn dữ liệu mẫu vào bảng Customers (KHÔNG chỉ định CustomerID)
INSERT INTO Customers (Name, Email)
VALUES 
('Nguyen Van A', 'nguyenvana@example.com'),
('Tran Thi B', 'tranthib@example.com'),
('Le Van C', 'levanc@example.com')

-- Chèn dữ liệu mẫu vào bảng Products
INSERT INTO Products (ProductName, Category, Price, Stock)
VALUES 
('Laptop Dell XPS 13', 'Electronics', 25000000, 10),
('iPhone 15', 'Mobile Phones', 30000000, 20),
('Samsung Galaxy S23', 'Mobile Phones', 28000000, 15),
('MacBook Air M2', 'Electronics', 32000000, 8)

-- Chèn dữ liệu mẫu vào bảng Orders
INSERT INTO Orders (CustomerID, OrderDate, Status)
VALUES 
(1, '2024-03-01', 'Completed'),
(1, '2024-03-05', 'Pending'),
(2, '2024-03-10', 'Completed'),
(3, '2024-03-15', 'Shipped')

-- Chèn dữ liệu mẫu vào bảng OrderItems
INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price)
VALUES 
(1, 1, 1, 25000000),
(1, 2, 1, 30000000),
(2, 3, 2, 28000000),
(3, 4, 1, 32000000),
(4, 2, 1, 30000000)

-- Chèn dữ liệu mẫu vào bảng Shipments
INSERT INTO Shipments (OrderID, ShipmentDate, DeliveryStatus)
VALUES 
(1, '2024-03-03', 'Delivered'),
(3, '2024-03-12', 'In Transit'),
(4, '2024-03-18', 'Delivered')

-- Kiểm thử Stored Procedure
--Mong đợi: Hiển thị tất cả đơn hàng từ ngày 01/03 đến 31/03.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = NULL, 
    @MinAmount = NULL, 
    @MaxAmount = NULL, 
    @OrderStatus = NULL, 
    @ShipmentStatus = NULL

-- Mong đợi: Chỉ hiển thị đơn hàng của khách hàng CustomerID = 1.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = 1, 
    @MinAmount = NULL, 
    @MaxAmount = NULL, 
    @OrderStatus = NULL, 
    @ShipmentStatus = NULL

-- Mong đợi: Chỉ hiển thị đơn hàng có tổng giá trị từ 28 triệu trở lên.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = NULL, 
    @MinAmount = 28000000, 
    @MaxAmount = NULL, 
    @OrderStatus = NULL, 
    @ShipmentStatus = NULL

-- Mong đợi: Chỉ hiển thị đơn hàng có tổng giá trị dưới hoặc bằng 30 triệu.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = NULL, 
    @MinAmount = NULL, 
    @MaxAmount = 30000000, 
    @OrderStatus = NULL, 
    @ShipmentStatus = NULL

-- Mong đợi: Chỉ hiển thị các đơn hàng có trạng thái 'Completed'.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = NULL, 
    @MinAmount = NULL, 
    @MaxAmount = NULL, 
    @OrderStatus = 'Completed', 
    @ShipmentStatus = NULL

-- Mong đợi: Chỉ hiển thị các đơn hàng có tình trạng giao hàng là 'Delivered'.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = NULL, 
    @MinAmount = NULL, 
    @MaxAmount = NULL, 
    @OrderStatus = NULL, 
    @ShipmentStatus = 'Delivered'


-- Chỉ hiển thị đơn hàng của CustomerID = 1, có tổng tiền từ 25 triệu đến 60 triệu, có trạng thái 'Completed' và đã được giao hàng 'Delivered'.
EXEC GetComplexOrderReport 
    @StartDate = '2024-03-01', 
    @EndDate = '2024-03-31', 
    @CustomerID = 1, 
    @MinAmount = 25000000, 
    @MaxAmount = 60000000, 
    @OrderStatus = 'Completed', 
    @ShipmentStatus = 'Delivered'
