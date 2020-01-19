CREATE TABLE [dbo].[TemperatureReadings] (
	[ID]                    INT IDENTITY (1, 1)     NOT NULL,
    [LocationID]            UNIQUEIDENTIFIER        NOT NULL,
    [Timestamp]             DATETIME                NOT NULL,
    [Temperature]           DECIMAL (18, 4)         NOT NULL,
    CONSTRAINT [PK_TemperatureReadings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TemperatureReadings_LocationID] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([ID])
)
