BEGIN TRANSACTION;
GO

ALTER TABLE [Files] ADD [Type] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20221118233405_Second Migration', N'6.0.10');
GO

COMMIT;
GO

