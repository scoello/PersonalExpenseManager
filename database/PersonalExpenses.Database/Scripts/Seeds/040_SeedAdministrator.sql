USE [PersonalExpenses];
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin')
BEGIN
    INSERT INTO dbo.Users (Id, Username, PasswordHash, Role)
    VALUES (NEWID(), N'admin', N'AQIDBAUGBwgJCgsMDQ4PEB3F7gCtdEo1iyT6dn4zyxg2c2AVB47i04/1VD4JnxLh', N'Admin');
END;
GO
