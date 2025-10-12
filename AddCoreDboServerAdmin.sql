-- Add user to SQL Server with full admin rights (same as sa)
-- This script creates the user login and grants it sysadmin privileges

-- Set the username here - change this to switch between users
-- Change 'InstrumentationDbo' to 'CoreDbo' if needed
USE [master]

DECLARE @Username NVARCHAR(50) = 'InstrumentationDbo';
DECLARE @Password NVARCHAR(50) = 'thunderbus888';
DECLARE @SQL NVARCHAR(MAX);

-- Create login if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = @Username)
BEGIN
    SET @SQL = 'CREATE LOGIN [' + @Username + '] WITH PASSWORD = ''' + @Password + ''', 
                                DEFAULT_DATABASE=[master], 
                                CHECK_EXPIRATION=OFF, 
                                CHECK_POLICY=OFF;';
    EXEC sp_executesql @SQL;
    PRINT @Username + ' login created successfully.'
END
ELSE
BEGIN
    -- Update password if login already exists
    SET @SQL = 'ALTER LOGIN [' + @Username + '] WITH PASSWORD = ''' + @Password + ''';';
    EXEC sp_executesql @SQL;
    PRINT @Username + ' login already exists - password updated.'
END

-- Add user to sysadmin role (gives full server access like sa)
IF NOT EXISTS (SELECT 1 FROM sys.server_role_members rm 
               JOIN sys.server_principals r ON rm.role_principal_id = r.principal_id 
               JOIN sys.server_principals m ON rm.member_principal_id = m.principal_id 
               WHERE r.name = 'sysadmin' AND m.name = @Username)
BEGIN
    SET @SQL = 'ALTER SERVER ROLE [sysadmin] ADD MEMBER [' + @Username + '];';
    EXEC sp_executesql @SQL;
    PRINT @Username + ' added to sysadmin role successfully.'
END
ELSE
BEGIN
    PRINT @Username + ' is already a member of sysadmin role.'
END

-- Verify the setup
SELECT 
    sp.name AS LoginName,
    sp.type_desc AS LoginType,
    sp.is_disabled AS IsDisabled,
    sr.name AS ServerRole
FROM sys.server_principals sp
LEFT JOIN sys.server_role_members srm ON sp.principal_id = srm.member_principal_id
LEFT JOIN sys.server_principals sr ON srm.role_principal_id = sr.principal_id
WHERE sp.name = @Username
ORDER BY sp.name, sr.name;

PRINT @Username + ' setup complete. User now has full server admin rights.'