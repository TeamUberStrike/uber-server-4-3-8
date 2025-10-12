-- Add multiple users to SQL Server with full admin rights (same as sa)
-- This script creates user logins and grants them sysadmin privileges

USE [master]

-- Create a table variable to hold usernames and passwords
DECLARE @Users TABLE (
    Username NVARCHAR(50),
    Password NVARCHAR(50)
);

-- Insert the users you want to create
INSERT INTO @Users (Username, Password) VALUES 
    ('CoreDbo', 'thunderbus888'),
    ('InstrumentationDbo', 'thunderbus888'),
    ('MvParadisePaintballDbo', 'thunderbus888'),
    ('CmuneMonitoringDbo', 'thunderbus888');

-- Variables for the loop
DECLARE @Username NVARCHAR(50);
DECLARE @Password NVARCHAR(50);
DECLARE @SQL NVARCHAR(MAX);

-- Cursor to iterate through users
DECLARE user_cursor CURSOR FOR 
SELECT Username, Password FROM @Users;

OPEN user_cursor;
FETCH NEXT FROM user_cursor INTO @Username, @Password;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '--- Processing user: ' + @Username + ' ---';
    
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
    
    PRINT @Username + ' setup complete.';
    PRINT '';
    
    -- Get next user
    FETCH NEXT FROM user_cursor INTO @Username, @Password;
END

CLOSE user_cursor;
DEALLOCATE user_cursor;

-- Verify the setup for all users
PRINT '--- Final verification for all users ---';
SELECT 
    sp.name AS LoginName,
    sp.type_desc AS LoginType,
    sp.is_disabled AS IsDisabled,
    sr.name AS ServerRole
FROM sys.server_principals sp
LEFT JOIN sys.server_role_members srm ON sp.principal_id = srm.member_principal_id
LEFT JOIN sys.server_principals sr ON srm.role_principal_id = sr.principal_id
WHERE sp.name IN (SELECT Username FROM @Users)
ORDER BY sp.name, sr.name;

PRINT 'All users setup complete. All users now have full server admin rights.'