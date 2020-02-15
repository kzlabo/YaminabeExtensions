﻿CREATE TABLE [dbo].[BulkCopyMapTestTable]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [StringValue] NVARCHAR(2000) NULL, 
    [ShortValue] SMALLINT NULL, 
    [ShortWithNullable ] SMALLINT NULL, 
    [IntValue] INT NOT NULL, 
    [IntWithNullable ] INT NULL, 
    [LongValue] BIGINT NOT NULL, 
    [LongWithNullable ] BIGINT NULL, 
    [FloatValue] FLOAT NOT NULL, 
    [FloatWithNullable ] FLOAT NULL, 
    [DoubleValue] FLOAT NOT NULL, 
    [DoubleWithNullable ] FLOAT NULL, 
    [DecimalValue] DECIMAL(30, 4) NOT NULL, 
    [DecimalWithNullable ] DECIMAL(30, 4) NULL, 
    [DateTimeValue] DATETIME NOT NULL, 
    [DateTimeWithNullable ] DATETIME NULL, 
    [ByteValues] VARBINARY(MAX) NULL, 
    [BoolValue] BIT NOT NULL, 
    [BoolWithNullable ] BIT NULL, 
    [GuidValue] UNIQUEIDENTIFIER NOT NULL, 
    [GuidWithNullable] UNIQUEIDENTIFIER NULL
)
