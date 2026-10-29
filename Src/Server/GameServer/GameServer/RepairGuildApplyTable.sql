-- Repair the GuildApplies -> TGuildApplies name change without rebuilding game tables.
-- Run this script in SSMS instead of Entities.edmx.sql to repair the failed rebuild.
-- Existing application rows, primary key, foreign key, and indexes stay on the renamed table.
-- An existing TGuildApplies table is removed only if it is empty and has the same columns.

USE [ExtremWorld];
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @OldTableId int;
    DECLARE @NewTableId int;
    SELECT @OldTableId = OBJECT_ID(N'[dbo].[GuildApplies]', N'U'),
           @NewTableId = OBJECT_ID(N'[dbo].[TGuildApplies]', N'U');

    IF @OldTableId IS NOT NULL
    BEGIN
        IF @NewTableId IS NOT NULL
        BEGIN
            DECLARE @HasRows bit;
            EXEC sys.sp_executesql
                N'SELECT @HasRows = CASE WHEN EXISTS
                    (SELECT 1 FROM [dbo].[TGuildApplies] WITH (TABLOCKX, HOLDLOCK))
                    THEN 1 ELSE 0 END;',
                N'@HasRows bit OUTPUT', @HasRows OUTPUT;

            IF @HasRows = 1
                RAISERROR(N'TGuildApplies contains data. No tables were changed; merge the two application tables before retrying.', 16, 1);

            IF EXISTS (
                SELECT name, column_id, user_type_id, max_length, [precision], [scale],
                       collation_name, is_nullable, is_identity, is_computed
                FROM sys.columns WHERE object_id = @OldTableId
                EXCEPT
                SELECT name, column_id, user_type_id, max_length, [precision], [scale],
                       collation_name, is_nullable, is_identity, is_computed
                FROM sys.columns WHERE object_id = @NewTableId
            ) OR EXISTS (
                SELECT name, column_id, user_type_id, max_length, [precision], [scale],
                       collation_name, is_nullable, is_identity, is_computed
                FROM sys.columns WHERE object_id = @NewTableId
                EXCEPT
                SELECT name, column_id, user_type_id, max_length, [precision], [scale],
                       collation_name, is_nullable, is_identity, is_computed
                FROM sys.columns WHERE object_id = @OldTableId
            )
                RAISERROR(N'The application tables have different columns. No tables were changed.', 16, 1);

            DROP TABLE [dbo].[TGuildApplies];
        END;

        EXEC sys.sp_rename N'dbo.GuildApplies', N'TGuildApplies', N'OBJECT';
    END;

    SET @NewTableId = OBJECT_ID(N'[dbo].[TGuildApplies]', N'U');

    IF @NewTableId IS NULL
        RAISERROR(N'Neither application table exists. This script only repairs an existing application table.', 16, 1);

    IF NOT EXISTS (
        SELECT 1 FROM sys.key_constraints
        WHERE object_id = OBJECT_ID(N'[dbo].[PK_TGuildApplies]', N'PK')
          AND parent_object_id = @NewTableId
    )
        RAISERROR(N'PK_TGuildApplies is not on the expected table. The repair was rolled back.', 16, 1);

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE object_id = OBJECT_ID(N'[dbo].[FK_GuildGuildApply]', N'F')
          AND parent_object_id = @NewTableId
          AND referenced_object_id = OBJECT_ID(N'[dbo].[Guilds]', N'U')
          AND is_disabled = 0 AND is_not_trusted = 0
    )
        RAISERROR(N'FK_GuildGuildApply is not an enabled, trusted foreign key to Guilds. The repair was rolled back.', 16, 1);

    COMMIT TRANSACTION;
    PRINT N'Guild application table repaired. Existing game data was preserved.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage nvarchar(4000);
    SET @ErrorMessage = ERROR_MESSAGE();
    RAISERROR(N'%s', 16, 1, @ErrorMessage);
END CATCH;
GO
