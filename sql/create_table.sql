CREATE TABLE [Server].[Users] (
    [Id]        INT            NOT NULL,
    [LoginName] CHAR (20)      NOT NULL,
    [Nickname]  NCHAR (20)     NOT NULL,
    [Email]     NCHAR (50)     NOT NULL,
    [pwd]       NVARCHAR (MAX) NOT NULL,
    [salt]      NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
	UNIQUE ([Email])
);

