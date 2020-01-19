CREATE TABLE [dbo].[Temperatures] (
	[ID]            INT IDENTITY (1, 1)     NOT NULL,
    [LocationID]    UNIQUEIDENTIFIER        NOT NULL,
    [Timestamp]     DATETIME                NOT NULL,
    [Temperature]   DECIMAL                 NOT NULL,
    CONSTRAINT [PK_Temperatures] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Temperatures_LocationID] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([ID])
)
