CREATE TABLE dbo.[Products] (
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [IsEnabled] bit NOT NULL,
    [Price] decimal(19,4) NOT NULL
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);