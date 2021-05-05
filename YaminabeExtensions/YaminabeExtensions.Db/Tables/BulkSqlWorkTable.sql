CREATE TABLE [dbo].[BulkSqlWorkTable]
(
    [Id] INT NOT NULL , 
    [SubId] INT NOT NULL ,
    [Name] NVARCHAR(50) NULL, 
    [Age] INT NULL, 
    CONSTRAINT [PK_BulkSqlWorkTable] PRIMARY KEY ([Id], [SubId])
)
