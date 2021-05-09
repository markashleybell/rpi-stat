CREATE TABLE [dbo].[Locations] (
	[ID]        UNIQUEIDENTIFIER        NOT NULL,
    [Name]      NVARCHAR(64)            NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY NONCLUSTERED ([ID] ASC)
)
