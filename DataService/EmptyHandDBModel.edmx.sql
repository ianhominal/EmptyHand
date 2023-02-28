
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/28/2023 16:10:33
-- Generated from EDMX file: C:\Repo\EmptyHand\EmptyHand\DataService\EmptyHandDBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EmptyHandDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_GameHeader_GameRound]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GameHeader] DROP CONSTRAINT [FK_GameHeader_GameRound];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[GameHeader]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GameHeader];
GO
IF OBJECT_ID(N'[dbo].[GameRound]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GameRound];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'GameHeaders'
CREATE TABLE [dbo].[GameHeaders] (
    [GameId] uniqueidentifier  NOT NULL,
    [PlayerId] nvarchar(50)  NOT NULL,
    [Player2Id] nvarchar(50)  NULL,
    [PlayerPoints] int  NOT NULL,
    [PlayerRoundsWins] int  NOT NULL,
    [Player2Points] int  NOT NULL,
    [Player2RoundsWins] int  NOT NULL,
    [RoundsCount] int  NOT NULL,
    [GameRoundId] uniqueidentifier  NULL,
    [PlayerName] nvarchar(max)  NOT NULL,
    [PlayerPhoto] nvarchar(max)  NOT NULL,
    [Player2Name] nvarchar(max)  NOT NULL,
    [Player2Photo] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'GameRounds'
CREATE TABLE [dbo].[GameRounds] (
    [GameRoundId] uniqueidentifier  NOT NULL,
    [AvailableCards] nvarchar(max)  NOT NULL,
    [PlayerCards] nvarchar(max)  NOT NULL,
    [PlayerLifeCards] nvarchar(max)  NOT NULL,
    [Player2Cards] nvarchar(max)  NOT NULL,
    [Player2LifeCards] nvarchar(max)  NOT NULL,
    [PlayerTurnId] nvarchar(50)  NULL,
    [CardPits] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [GameId] in table 'GameHeaders'
ALTER TABLE [dbo].[GameHeaders]
ADD CONSTRAINT [PK_GameHeaders]
    PRIMARY KEY CLUSTERED ([GameId] ASC);
GO

-- Creating primary key on [GameRoundId] in table 'GameRounds'
ALTER TABLE [dbo].[GameRounds]
ADD CONSTRAINT [PK_GameRounds]
    PRIMARY KEY CLUSTERED ([GameRoundId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GameRoundId] in table 'GameHeaders'
ALTER TABLE [dbo].[GameHeaders]
ADD CONSTRAINT [FK_GameHeader_GameRound]
    FOREIGN KEY ([GameRoundId])
    REFERENCES [dbo].[GameRounds]
        ([GameRoundId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GameHeader_GameRound'
CREATE INDEX [IX_FK_GameHeader_GameRound]
ON [dbo].[GameHeaders]
    ([GameRoundId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------