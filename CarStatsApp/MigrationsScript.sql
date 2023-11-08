IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CarStats] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [Speed] float NOT NULL,
    [RegistrationNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_CarStats] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231030154443_InitialCreate', N'7.0.13');
GO

COMMIT;
GO

