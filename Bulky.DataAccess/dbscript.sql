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