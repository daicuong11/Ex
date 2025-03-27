CREATE DATABASE EX1
GO

USE EX1
GO

CREATE TABLE Authors (
    AuthorId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Bio NVARCHAR(500) NULL
)
GO

CREATE TABLE Books (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    AuthorId INT FOREIGN KEY REFERENCES Authors(AuthorId),
    PublishedDate DATETIME NULL
)
GO

CREATE PROCEDURE GetAllBooks
AS
BEGIN
    SELECT 
        b.BookId, b.Title, b.Price, b.PublishedDate, 
        a.AuthorId, a.Name AS AuthorName, a.Bio AS AuthorBio
    FROM Books b
    INNER JOIN Authors a ON b.AuthorId = a.AuthorId;
END
GO


CREATE PROCEDURE GetBookById
    @BookId INT
AS
BEGIN
    SELECT 
		b.BookId, b.Title, b.Price, b.PublishedDate, 
        a.AuthorId, a.Name AS AuthorName, a.Bio AS AuthorBio
    FROM Books b
    INNER JOIN Authors a ON b.AuthorId = a.AuthorId
    WHERE b.BookId = @BookId;
END
GO

CREATE PROCEDURE GetAllAuthors
AS
BEGIN
    SELECT * FROM Authors ORDER BY AuthorId;
END
GO

CREATE PROCEDURE GetAuthorById
    @AuthorId INT
AS
BEGIN
    SELECT * FROM Authors WHERE AuthorId = @AuthorId;
END
GO

CREATE PROCEDURE GetBooksByFilter
    @searchKey NVARCHAR(200) = NULL,
    @authorId INT = NULL,
    @fromPublishedDate DATETIME = NULL,
    @toPublishedDate DATETIME = NULL,
    @pageSize INT = 10,
    @pageIndex INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@pageIndex - 1) * @pageSize;
    
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*)
    FROM Books b
    WHERE
        (@searchKey IS NULL OR b.Title LIKE '%' + @searchKey + '%')
        AND (@authorId IS NULL OR b.AuthorId = @authorId)
        AND (@fromPublishedDate IS NULL OR b.PublishedDate >= @fromPublishedDate)
        AND (@toPublishedDate IS NULL OR b.PublishedDate <= @toPublishedDate);

    SELECT 
        b.BookId, 
        b.Title, 
        b.Price, 
        b.PublishedDate, 
        a.AuthorId, 
        a.Name AS AuthorName,
        a.Bio AS AuthorBio,  
        @TotalCount AS TotalCount 
    FROM Books b
    INNER JOIN Authors a ON b.AuthorId = a.AuthorId
    WHERE
        (@searchKey IS NULL OR b.Title LIKE '%' + @searchKey + '%')
        AND (@authorId IS NULL OR b.AuthorId = @authorId)
        AND (@fromPublishedDate IS NULL OR b.PublishedDate >= @fromPublishedDate)
        AND (@toPublishedDate IS NULL OR b.PublishedDate <= @toPublishedDate)
    ORDER BY b.PublishedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @pageSize ROWS ONLY;
END
GO

