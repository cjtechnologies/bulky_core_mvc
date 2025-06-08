CREATE TABLE Categories (
    Id INT NOT NULL IDENTITY(1, 1),
    Name NVARCHAR(MAX) NOT NULL,
    DisplayOrder INT NOT NULL,
    CONSTRAINT Pk_Categories_Id PRIMARY KEY (Id)
);
--##
INSERT INTO bulky.dbo.Categories (Name, DisplayOrder) VALUES 
(N'Action', 1),
(N'SciFi', 2),
(N'History', 3);
--##
CREATE TABLE Products (
    Id INT NOT NULL IDENTITY(1, 1),
    Title NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX),
    ISBN NVARCHAR(MAX) NOT NULL,
    Author NVARCHAR(MAX) NOT NULL,
    ListPrice FLOAT NOT NULL,
    Price FLOAT NOT NULL,
    Price50 FLOAT NOT NULL,
    Price100 FLOAT NOT NULL,
    CONSTRAINT Pk_Products_Id PRIMARY KEY (Id)
);
--##
INSERT INTO bulky.dbo.Products (Title, Author, Description, ISBN, ListPrice, Price, Price50, Price100) VALUES
('Fortune of Time', 'Billy Spark','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','SWD9999001',99,90,85,80),
('Dark Skies', 'Nancy Hoover','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','CAW777777701',40,30,25,20),
('Vanish in the Sunset', 'Julian Button','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','RITO5555501',55,50,40,35),
('Cotton Candy', 'Abby Muscles','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','WS3333333301',70,65,60,55),
('Rock in the Ocean', 'Ron Parker','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','SOTJ1111111101',30,27,25,20),
('Leaves and Wonders', 'Laura Phantom','Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.','FOT000000001',25,23,22,20);
--##
ALTER TABLE Products ADD CategoryId INT;
GO
UPDATE Products SET CategoryId=1 WHERE ID>0;
GO
ALTER TABLE Products ALTER COLUMN CategoryId INT NOT NULL;
GO
ALTER TABLE Products ADD CONSTRAINT FK_CategoryId FOREIGN KEY (CategoryId) REFERENCES Categories(Id);
--##
ALTER TABLE Products DROP CONSTRAINT FK_CategoryId;
ALTER TABLE Products DROP COLUMN CategoryId;
--##
ALTER TABLE Products ADD ImageUrl NVARCHAR(MAX);
--##
SELECT * FROM Products;

