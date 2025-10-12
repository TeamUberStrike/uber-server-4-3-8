-- Add CoreDbo user to SQL Server with full admin rights (same as sa)
-- This script creates the CoreDbo login and grants it sysadmin privileges

USE [master]
GO

-- Create CoreDbo login if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'CoreDbo')
BEGIN
    CREATE LOGIN [CoreDbo] WITH PASSWORD = 'thunderbus888', 
                                DEFAULT_DATABASE=[master], 
                                CHECK_EXPIRATION=OFF, 
                                CHECK_POLICY=OFF;
    PRINT 'CoreDbo login created successfully.'
END
ELSE
BEGIN
    -- Update password if login already exists
    ALTER LOGIN [CoreDbo] WITH PASSWORD = 'thunderbus888';
    PRINT 'CoreDbo login already exists - password updated.'
END
GO

-- Add CoreDbo to sysadmin role (gives full server access like sa)
IF NOT EXISTS (SELECT 1 FROM sys.server_role_members rm 
               JOIN sys.server_principals r ON rm.role_principal_id = r.principal_id 
               JOIN sys.server_principals m ON rm.member_principal_id = m.principal_id 
               WHERE r.name = 'sysadmin' AND m.name = 'CoreDbo')
BEGIN
    ALTER SERVER ROLE [sysadmin] ADD MEMBER [CoreDbo];
    PRINT 'CoreDbo added to sysadmin role successfully.'
END
ELSE
BEGIN
    PRINT 'CoreDbo is already a member of sysadmin role.'
END
GO

-- Verify the setup
SELECT 
    sp.name AS LoginName,
    sp.type_desc AS LoginType,
    sp.is_disabled AS IsDisabled,
    sr.name AS ServerRole
FROM sys.server_principals sp
LEFT JOIN sys.server_role_members srm ON sp.principal_id = srm.member_principal_id
LEFT JOIN sys.server_principals sr ON srm.role_principal_id = sr.principal_id
WHERE sp.name = 'CoreDbo'
ORDER BY sp.name, sr.name;

PRINT 'CoreDbo setup complete. User now has full server admin rights.'