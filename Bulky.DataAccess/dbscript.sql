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
--##
CREATE TABLE [AspNetRoles] (
    [Id] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
--##
CREATE TABLE [AspNetUsers] (
    [Id] NVARCHAR(450) NOT NULL,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
--##
ALTER TABLE [AspNetUsers]
ADD 
    [Name] NVARCHAR(MAX) NULL,
    [StreetAddress] NVARCHAR(MAX) NULL,
    [City] NVARCHAR(MAX) NULL,
    [State] NVARCHAR(MAX) NULL,
    [PostalCode] NVARCHAR(MAX) NULL,
    [Discriminator] NVARCHAR(21) NOT NULL CONSTRAINT [DF_AspNetUsers_Discriminator] DEFAULT '';
--##
CREATE TABLE [AspNetRoleClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);
--##
CREATE TABLE [AspNetUserClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
--##
CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [ProviderKey] NVARCHAR(128) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
--##
CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);
--##
CREATE TABLE [AspNetUserTokens] (
    [UserId] NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(128) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
--##
-- Index on AspNetRoleClaims.RoleId
CREATE INDEX [IX_AspNetRoleClaims_RoleId]
ON [AspNetRoleClaims] ([RoleId]);
--##
-- Unique index on AspNetRoles.NormalizedName where NormalizedName IS NOT NULL
CREATE UNIQUE INDEX [RoleNameIndex]
ON [AspNetRoles] ([NormalizedName])
WHERE [NormalizedName] IS NOT NULL;
--##
-- Index on AspNetUserClaims.UserId
CREATE INDEX [IX_AspNetUserClaims_UserId]
ON [AspNetUserClaims] ([UserId]);
--##
-- Index on AspNetUserLogins.UserId
CREATE INDEX [IX_AspNetUserLogins_UserId]
ON [AspNetUserLogins] ([UserId]);
--##
-- Index on AspNetUserRoles.RoleId
CREATE INDEX [IX_AspNetUserRoles_RoleId]
ON [AspNetUserRoles] ([RoleId]);
--##
-- Index on AspNetUsers.NormalizedEmail
CREATE INDEX [EmailIndex]
ON [AspNetUsers] ([NormalizedEmail]);
--##
-- Unique index on AspNetUsers.NormalizedUserName where NormalizedUserName IS NOT NULL
CREATE UNIQUE INDEX [UserNameIndex]
ON [AspNetUsers] ([NormalizedUserName])
WHERE [NormalizedUserName] IS NOT NULL;
--##
--## ============== COMPANY ===============
CREATE TABLE Companies (
    Id INT NOT NULL IDENTITY(1, 1),
    [Name] NVARCHAR(MAX) NOT NULL,
    [StreetAddress] NVARCHAR(MAX) NULL,
    [City] NVARCHAR(MAX) NULL,
    [State] NVARCHAR(MAX) NULL,
    [PostalCode] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,    
    CONSTRAINT Pk_Companies_Id PRIMARY KEY (Id)
);
--##
INSERT INTO bulky.dbo.Companies (Name, StreetAddress, City, State, PostalCode, PhoneNumber) VALUES 
(N'Tech Solution', '123, Tech Street', 'Tech City', 'IL', '121212','654321987'),
(N'Vivid Books', '456, Vivid Street', 'Vivid City', 'TN', '321321','789546123'),
(N'Readers Club', '321, Club Street', 'Club City', 'NY', '652562','693258741');
--##
ALTER TABLE [AspNetUsers]
ADD 
    [CompanyId] INT NULL,
    CONSTRAINT [FK_AspNetUsers_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([Id]) ON DELETE SET NULL;
--##
CREATE TABLE ShoppingCarts (
    Id INT NOT NULL IDENTITY(1, 1),
    ProductId INT NOT NULL,
    Count INT NOT NULL,
    ApplicationUserId NVARCHAR(450) NOT NULL,    
    CONSTRAINT Pk_ShoppingCarts_Id PRIMARY KEY (Id),
    CONSTRAINT [FK_ShoppingCarts_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ShoppingCarts_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
--##
--10. Shopping Cart
----8. Shopping Cart UI