USE [PersonalExpenses];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
IF OBJECT_ID(N'dbo.Expenses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Expenses
    (
        Id UNIQUEIDENTIFIER NOT NULL,
        UserId UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(80) NOT NULL,
        CONSTRAINT PK_Expenses PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Expenses_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT CK_Expenses_Amount_Positive CHECK (Amount > 0),
        CONSTRAINT CK_Expenses_Category_NotBlank CHECK (LEN(LTRIM(RTRIM(Category))) > 0)
    );
END;
GO
