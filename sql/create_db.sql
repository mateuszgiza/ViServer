sp_configure 'show advanced options',1
GO
RECONFIGURE WITH OVERRIDE
GO
sp_configure 'contained database authentication', 1
GO
RECONFIGURE WITH OVERRIDE
GO

CREATE DATABASE [db_ViServer] CONTAINMENT = PARTIAL;