-- Problem 1. Create a database with two tables: 
-- Persons(Id(PK), FirstName, LastName, SSN) and Accounts(Id(PK), PersonId(FK), Balance).

USE master
GO

CREATE DATABASE Bank
GO

USE Bank
GO

CREATE TABLE Persons(
  Id INT PRIMARY KEY IDENTITY NOT NULL,
  FirstName NVARCHAR(50) NOT NULL,
  LastName NVARCHAR(50) NOT NULL,
  SSN NVARCHAR(11) NOT NULL
  )
GO

CREATE TABLE Accounts(
  Id INT PRIMARY KEY IDENTITY NOT NULL,
  Balance MONEY NOT NULL,
  CustomerID INT FOREIGN KEY REFERENCES Persons(Id) NOT NULL
)
GO

INSERT INTO Persons
	VALUES ('Ivan', 'Ivanov', '123-45-678'),
		('Pesho', 'Peshev', '234-56-789'),
		('Gosho', 'Goshev', '345-67-890'),
		('John', 'Doe', '321-54-547'),
		('Mara', 'Obshtata', '696-69-696'),
		('Penka', 'Petrova', '223-45-678'),
		('Jane', 'Doe', '100-00-001')
GO

INSERT INTO Accounts
	VALUES (1000.00, 1),
		(2000.00, 2),
		(100.00, 3),
		(0.00, 4),
		(2500.00, 5),
		(600.00, 6),
		(500.00, 7)
GO

CREATE PROC usp_SelectPersonsFullnames
AS
	SELECT FirstName + ' ' + LastName AS Fullname
	FROM Persons
GO

EXEC usp_SelectPersonsFullnames
GO

-- Problem 2. Create a stored procedure that accepts a number as a parameter and returns all 
-- persons who have more money in their accounts than the supplied number.

CREATE PROC usp_SelectPersonsByAccountBalance(@minAccountBalance money = 500.0)
AS
	SELECT p.FirstName + ' ' + p.LastName AS Person, a.Balance
	FROM Persons p JOIN Accounts a ON p.Id = a.CustomerID
	WHERE a.Balance > @minAccountBalance
GO

EXEC usp_SelectPersonsByAccountBalance
GO

EXEC usp_SelectPersonsByAccountBalance 1200 -- just to show it works with other than default
GO

-- Problem 3.Create a function that accepts as parameters – sum, yearly interest rate and number of months.
-- It should calculate and return the new sum.
-- Write a SELECT to test whether the function works as expected.
USE Bank
GO

CREATE FUNCTION ufn_CalcInterest
	(@sum money, @yearlyInterestRate float, @numberOfMonths tinyint)
  RETURNS money
AS
BEGIN
  RETURN @sum + @sum * @yearlyInterestRate * @numberOfMonths / (12 * 100)
END
GO

SELECT dbo.ufn_CalcInterest(1000, 5.5, 7) AS 'Sum with interest'
GO

-- Problem 4. Create a stored procedure that uses the function from the previous example
--			  to give an interest to a person's account for one month.
CREATE PROC usp_AppendMonthlyInterest(@userAccount INT, @yearlyInterestRate float)
AS
	UPDATE Accounts
	SET Balance = dbo.ufn_CalcInterest(Accounts.Balance, @yearlyInterestRate, 1)
	WHERE Accounts.Id = @userAccount
GO

EXEC usp_AppendMonthlyInterest 1, 10.0
GO


-- Problem 5 Add two more stored procedures WithdrawMoney(AccountId, money) and 
--			 DepositMoney(AccountId, money) that operate in transactions.
CREATE PROC usp_DepositMoney(@userAccount INT, @sum MONEY)
AS
	BEGIN TRAN
		DECLARE @startingBalance MONEY
		SET @startingBalance = 
			(SELECT Balance FROM Accounts  
			WHERE Accounts.Id = @userAccount)

		UPDATE Accounts 
			SET Balance = Balance + @sum
			WHERE Accounts.Id = @userAccount

		IF (@startingBalance + @sum = (SELECT Balance FROM Accounts WHERE Accounts.Id = @userAccount))
			BEGIN
				COMMIT
			END
		ELSE
			BEGIN
				ROLLBACK
			END
GO

CREATE PROC usp_WithdrawMoney(@userAccount INT, @sum MONEY)
AS
	BEGIN TRAN
		DECLARE @startingBalance MONEY
		SET @startingBalance = 
			(SELECT Balance FROM Accounts  
			WHERE Accounts.Id = @userAccount)

		UPDATE Accounts 
			SET Balance = Balance - @sum
			WHERE Accounts.Id = @userAccount

		IF (@startingBalance - @sum >= 0 AND @startingBalance - @sum = (SELECT Balance FROM Accounts  WHERE Accounts.Id = @userAccount))
			BEGIN
				COMMIT TRAN
			END
		ELSE
			BEGIN
				RAISERROR('Transaction cancelled, no funds withdrawn', 16, 1)
				ROLLBACK TRAN
			END
GO

EXEC usp_DepositMoney 4, 3333
GO
EXEC usp_WithdrawMoney 4, 33
GO
EXEC usp_WithdrawMoney 4, 5000 --this should raise error
GO

-- 6. Create another table – Logs(LogID, AccountID, OldSum, NewSum).
-- Add a trigger to the Accounts table that enters a new entry into the Logs table every time the sum on an account changes.

CREATE TABLE Logs(
  Id INT PRIMARY KEY IDENTITY NOT NULL,
  AccountID INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
  OldSum MONEY NOT NULL,
  NewSum MONEY NOT NULL
)
GO

CREATE TRIGGER tr_AccountBalanceUpdate ON Accounts AFTER UPDATE
AS	
	IF(UPDATE(Balance))
		BEGIN
			INSERT INTO Logs 
			VALUES ((SELECT Id FROM INSERTED), (SELECT Balance FROM DELETED), (SELECT Balance FROM INSERTED))
		END
GO

EXEC usp_DepositMoney 1, 1000
GO

EXEC usp_WithdrawMoney 1, 1000
GO

-- Problem 7. Define a function in the database TelerikAcademy that returns all Employee's names (first or middle or last name)
--			  and all town's names that are comprised of given set of letters.
--			  Example: 'oistmiahf' will return 'Sofia', 'Smith', … but not 'Rob' and 'Guy'.

USE TelerikAcademy
GO

CREATE FUNCTION ufn_CheckName(@nameToCheck NVARCHAR(50), @letters NVARCHAR(50))
RETURNS int AS
BEGIN
    DECLARE @i int = 1
	DECLARE @currentChar NVARCHAR(1)
    WHILE (@i <= LEN(@nameToCheck))
		BEGIN
			SET @currentChar = SUBSTRING(@nameToCheck,@i, 1)
				IF (CHARINDEX(LOWER(@currentChar),LOWER(@letters)) <= 0) 
					BEGIN  
						RETURN 0
					END
			SET @i = @i + 1
		END
    RETURN 1
END
GO

CREATE FUNCTION ufn_FindEmploeeysAndTownsByStringCondition(@format NVARCHAR(50))
RETURNS @table TABLE
	([Name] NVARCHAR(50) NOT NULL)
AS
BEGIN
	INSERT @table
	SELECT newTbl.LastName FROM
		(SELECT LastName FROM Employees
		UNION
		SELECT Name FROM Towns) as newTbl
		WHERE dbo.ufn_CheckName(newTbl.LastName, @format) > 0
	 RETURN
END
GO

SELECT * FROM ufn_FindEmploeeysAndTownsByStringCondition('oistmiahf')

-- Problem 8. Using database cursor write a T-SQL script that scans all employees and 
--			  their addresses and prints all pairs of employees that live in the same town.

DECLARE empCursor CURSOR READ_ONLY FOR
  SELECT e.FirstName, e.LastName, t.Name FROM Employees e
  JOIN Addresses a
  ON e.AddressID = a.AddressID
  JOIN Towns t
  ON a.TownID = t.TownID

OPEN empCursor
DECLARE @firstName NVARCHAR(50), @lastName NVARCHAR(50), @town NVARCHAR(50)
FETCH NEXT FROM empCursor INTO @firstName, @lastName, @town

WHILE @@FETCH_STATUS = 0
  BEGIN
  			  DECLARE empCursor1 CURSOR READ_ONLY FOR
			  SELECT e.FirstName, e.LastName, t.Name FROM Employees e
			  JOIN Addresses a
			  ON e.AddressID = a.AddressID
			  JOIN Towns t
			  ON a.TownID = t.TownID

			OPEN empCursor1
			DECLARE @firstName1 NVARCHAR(50), @lastName1 NVARCHAR(50), @town1 NVARCHAR(50)
			FETCH NEXT FROM empCursor1 INTO @firstName1, @lastName1, @town1

			WHILE @@FETCH_STATUS = 0
			  BEGIN
			  IF(@town=@town1 AND @firstName != @firstName1 AND @lastName != @lastName1)
				  BEGIN
					PRINT @town+' - '+ @firstName + ' ' + @lastName + ':' + @firstName1 + ' ' + @lastName1 
				  END
				FETCH NEXT FROM empCursor1 INTO @firstName1, @lastName1, @town1
			  END

			CLOSE empCursor1
			DEALLOCATE empCursor1

    FETCH NEXT FROM empCursor  INTO @firstName, @lastName, @town
  END

CLOSE empCursor
DEALLOCATE empCursor