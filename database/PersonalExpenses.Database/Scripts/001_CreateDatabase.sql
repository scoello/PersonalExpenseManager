USE [master];
GO
IF DB_ID(N'PersonalExpenses') IS NULL
BEGIN
    CREATE DATABASE [PersonalExpenses];
END;
GO
ALTER DATABASE [PersonalExpenses] SET RECOVERY SIMPLE;
GO
