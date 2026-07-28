-- DebugMe v1.0 - MySQL Initialization Script
-- Execute this script on a fresh MySQL 8.0+ database to set up all tables.
-- Usage: mysql -u root -p DebugMeDb < init-mysql.sql

CREATE DATABASE IF NOT EXISTS DebugMeDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE DebugMeDb;

-- EF Core migrations history table
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` VARCHAR(150) NOT NULL,
    `ProductVersion` VARCHAR(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

-- Users table
CREATE TABLE `Users` (
    `Id` CHAR(36) NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Email` VARCHAR(150) NOT NULL,
    `PasswordHash` LONGTEXT NOT NULL,
    `RefreshToken` LONGTEXT NULL,
    `RefreshTokenExpiry` DATETIME NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME NULL,
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `IX_Users_Email` (`Email`)
);

-- Emotions table
CREATE TABLE `Emotions` (
    `Id` CHAR(36) NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Description` VARCHAR(250) NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME NULL,
    PRIMARY KEY (`Id`)
);

-- EventLogs table
CREATE TABLE `EventLogs` (
    `Id` CHAR(36) NOT NULL,
    `UserId` CHAR(36) NOT NULL,
    `EmotionId` CHAR(36) NOT NULL,
    `Description` VARCHAR(500) NULL,
    `Intensity` INT NOT NULL,
    `EventDate` DATETIME NOT NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_EventLogs_UserId` (`UserId`),
    INDEX `IX_EventLogs_EmotionId` (`EmotionId`),
    CONSTRAINT `FK_EventLogs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users`(`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EventLogs_Emotions_EmotionId` FOREIGN KEY (`EmotionId`) REFERENCES `Emotions`(`Id`) ON DELETE CASCADE
);

