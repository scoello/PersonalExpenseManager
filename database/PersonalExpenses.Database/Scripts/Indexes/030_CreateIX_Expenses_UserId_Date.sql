USE [PersonalExpenses];
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Expenses_UserId_Date' AND object_id = OBJECT_ID(N'dbo.Expenses'))
BEGIN
    CREATE INDEX IX_Expenses_UserId_Date ON dbo.Expenses(UserId, [Date] DESC);
END;
GO
