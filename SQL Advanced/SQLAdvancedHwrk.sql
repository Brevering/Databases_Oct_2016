-- Problem 1. Write a SQL query to find the names and salaries of the employees that take the minimal salary in the company.
--            Use a nested SELECT statement.
SELECT FirstName, LastName, Salary FROM Employees
WHERE Salary = (SELECT MIN(Salary) FROM Employees)

-- Problem 2. Write a SQL query to find the names and salaries of the employees
--            that have a salary that is up to 10% higher than the minimal salary for the company.
SELECT FirstName, LastName, Salary FROM Employees
WHERE Salary <= (SELECT MIN(Salary) FROM Employees)*1.1

-- Problem 3. Write a SQL query to find the full name, salary and department of the employees
--            that take the minimal salary in their department.
--            Use a nested SELECT statement.
SELECT e.FirstName + ' ' + e.LastName AS "Full name", e.Salary, d.[Name] AS Department 
FROM Employees e INNER JOIN Departments d 
ON e.DepartmentID = d.DepartmentID
WHERE Salary = 
	(SELECT MIN(Salary) FROM Employees
	 WHERE DepartmentID = e.DepartmentID)
ORDER BY e.DepartmentID

-- Problem 4. Write a SQL query to find the average salary in the department #1.
SELECT DepartmentID, AVG(Salary) AS 'Average Salary' FROM Employees
GROUP BY DepartmentID
HAVING DepartmentID = 1

-- Problem 5. Write a SQL query to find the average salary in the "Sales" department.
SELECT d.Name AS "Department name", AVG(e.Salary) AS 'Average Salary'
FROM Employees e JOIN Departments d 
ON e.DepartmentID = d.DepartmentID
GROUP BY d.Name
HAVING d.Name = 'Sales'

-- Problem 6. Write a SQL query to find the number of employees in the "Sales" department.
SELECT d.Name AS 'Department', COUNT(*) AS 'Number of employees'
FROM Employees e JOIN Departments d
ON e.DepartmentID = d.DepartmentID
GROUP BY d.Name
HAVING d.Name = 'Sales'

-- Problem 7. Write a SQL query to find the number of all employees that have manager.
SELECT COUNT(*) AS 'Employees with a manager'
FROM Employees
WHERE ManagerID IS NOT NULL

-- Problem 8. Write a SQL query to find the number of all employees that have no manager.
SELECT COUNT(*) AS 'Employees withOUT a manager'
FROM Employees
WHERE ManagerID IS NULL

-- Problem 9. Write a SQL query to find all departments and the average salary for each of them.
SELECT d.Name AS "Department name", AVG(e.Salary) AS 'Average Salary'
FROM Employees e JOIN Departments d 
ON e.DepartmentID = d.DepartmentID
GROUP BY d.Name

-- Problem 10. Write a SQL query to find the count of all employees in each department and for each town
SELECT d.[Name] AS 'Department', t.[Name] AS 'Town', COUNT(*) AS 'Number of employees'
FROM Employees e 
	JOIN Addresses a ON e.AddressID = a.AddressID 
	JOIN Towns t ON a.TownID = t.TownID 
	JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY t.[Name], d.[Name]

-- Problem 11. Write a SQL query to find all managers that have exactly 5 employees. Display their first name and last name.
SELECT m.FirstName, m.LastName, m.EmployeeID AS 'ID'
FROM Employees e 
	INNER JOIN Employees m ON e.ManagerID = m.EmployeeID
GROUP BY m.FirstName, m.LastName, m.EmployeeID
HAVING COUNT(*) = 5

-- Problem 12. Write a SQL query to find all employees along with their managers. 
--			   For employees that do not have manager display the value "(no manager)".
SELECT	e.FirstName + ' ' + e.LastName AS 'Employee name', 
		ISNULL(m.FirstName + ' ' + m.LastName, 'no manager') AS 'Manager name'
FROM Employees e 
	LEFT OUTER JOIN Employees m ON e.ManagerID = m.EmployeeID

-- Problem 13. Write a SQL query to find the names of all employees whose last name is exactly 5 characters long.
-- Use the built-in LEN(str) function.
SELECT FirstName + ' ' + LastName AS 'Name' FROM Employees
WHERE len(LastName) = 5

-- Problem 14. Write a SQL query to display the current date and time in the following format 
-- "day.month.year hour:minutes:seconds:milliseconds".
SELECT CONVERT(NVARCHAR(50),GETDATE(),113)

-- Problem 15. Write a SQL statement to create a table Users. Users should have username, password, full name and last login time.
CREATE TABLE Users(
Id INT IDENTITY PRIMARY KEY,
FullName NVARCHAR(100) NOT NULL,
Username NVARCHAR(50) NOT NULL,
[Password] NVARCHAR(50) NOT NULL,
LastLogin DATETIME,
CONSTRAINT UniqueContentConstraint_Username UNIQUE(Username),
CONSTRAINT MinLengthConstraint_Password CHECK(len([Password])>=5)
)

-- Problem 16. Write a SQL statement to create a view that displays the users from the Users table that have been in the system today.
-- We need to populate the table first
INSERT INTO Users VALUES	('Ivan Ivanov', 'vankata', '******', GETDATE()),
							('Mariika Ivanova', 'obshtata', '******', GETDATE()),
	-- Adding user that will not appear in the view Users from today
							('Pesho Peshev', 'peshkata', '******', DATEADD(DD, -2, GETDATE()))
GO
CREATE VIEW [Users from today] AS
(SELECT Username, FullName 
FROM Users
WHERE CONVERT(date, LastLogin) = CONVERT(date, GETDATE()))--convert to get rid of hours, minutes etc.
GO
-- Testing if view works correctly
SELECT * FROM [Users from today]

--Problem 17. Write a SQL statement to create a table Groups. Groups should have unique name (use unique constraint).
CREATE TABLE Groups(
Id INT IDENTITY PRIMARY KEY,
[Name] NVARCHAR(100) NOT NULL,
CONSTRAINT UniqueContentConstraint_Name UNIQUE ([Name])
)
GO

-- Problem 18. Write a SQL statement to add a column GroupID to the table Users.
ALTER TABLE Users ADD GroupID INT
GO
--			   Fill some data in this new column and as well in the `Groups table.
INSERT INTO Groups ([Name]) VALUES ('Group1'), ('Group2') -- added respective values in Users table by hand 
--			   Write a SQL statement to add a foreign key constraint between tables Users and Groups tables.
ALTER TABLE Users
	ADD FOREIGN KEY(GroupId) REFERENCES Groups(Id)
GO

-- Problem 19. Write SQL statements to insert several records in the Users and Groups tables.
INSERT INTO Groups VALUES ('Group3'), ('Group4'), ('Group5')
INSERT INTO Users VALUES	('Gosho Ivanov', 'gogobogo', '******', GETDATE(), 3),
							('Penka Petrova', 'pepe123', '******', GETDATE(), 2),
							('John Doe', 'Jd1989', '123456', DATEADD(DD, -2, GETDATE()), 4),
							('Jane Doe', 'JD7656567', '******', CONVERT(datetime, '2005-10-10 12:12:12.123', 121), 2)

-- Problem 20. Write SQL statements to update some of the records in the Users and Groups tables.
UPDATE Groups SET [Name]='Group Three' WHERE [Name]='Group3'
UPDATE Groups SET [Name]='Group Five' WHERE [Name]='Group5'
UPDATE Users SET [Password]='******' WHERE [Password]='123456' 
UPDATE Users SET GroupId=1 WHERE FullName='Ivan Ivanov'

-- Problem 21. Write SQL statements to delete some of the records from the Users and Groups tables.
DELETE FROM Users WHERE FullName='Jane Doe'
DELETE FROM Users WHERE GroupId IS NULL
DELETE FROM Groups WHERE Id = 5

-- 22. Write SQL statements to insert in the Users table the names of all employees from the Employees table.
--		Combine the first and last names as a full name.
--		For username use the first letter of the first name + the last name (in lowercase).
--		Use the same for the password, and NULL for last login time.
INSERT INTO Users
SELECT	FirstName + ' ' + LastName,
		LOWER(LEFT(FirstName, 1) + LastName), 
		LOWER(LEFT(FirstName, 1) + LastName), 
		NULL, 
		NULL
FROM Employees 
WHERE len((SELECT LOWER(LEFT(FirstName, 1) + LastName))) > 5
--this adds 236 rows. Passwords are longer than 5 symbols

INSERT INTO Users
SELECT	FirstName + ' ' + LastName,
		LOWER(LEFT(FirstName, 1) + LastName) + CONVERT(NVARCHAR, EmployeeID),
		LOWER(LEFT(FirstName, 1) + LastName) + CONVERT(NVARCHAR, EmployeeID), 
		NULL, 
		NULL
FROM Employees 
WHERE len((SELECT LOWER(LEFT(FirstName, 1) + LastName))) <= 5
-- this adds the rest of the rows by making the password longer than 5.
-- Adding the EmployeeID (which is unique) also solves the uniqueness (a.k.a. the 'ahill') problem ;)

-- Problem 23. Write a SQL statement that changes the password to NULL for all users that have not been in the system since 10.03.2010.
	-- Insert a new user that has last log before 2010
INSERT INTO Users VALUES	('User TB Deleted', 'aaaaaa', 'bbbbbb', CONVERT(datetime, '2009-01-01 12:12:12.123', 121), 2)
	-- Make password nullable
ALTER TABLE Users ALTER COLUMN Password nvarchar(50) NULL

UPDATE Users SET [Password] = NULL
WHERE LastLogin < CONVERT(date, '10-03-2010')

-- Problem 24. Write a SQL statement that deletes all users without passwords (NULL password).
DELETE FROM Users WHERE [Password] IS NULL

-- Problem 25. Write a SQL query to display the average employee salary by department and job title.
SELECT d.[Name] AS 'Department', e.JobTitle AS 'Job title', AVG(Salary) AS 'Average Salary'
FROM Employees e 
	JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY e.JobTitle, d.[Name]

-- 26. Write a SQL query to display the minimal employee salary by department and job title 
-- along with the name of some of the employees that take it.
SELECT	d.[Name] AS 'Department', 
		e.JobTitle AS 'Job title', 
		MIN(Salary) AS 'Minimum Salary',
		MIN(e.FirstName + ' ' + e.LastName) AS 'Received by' -- MIN is just to have aggregate function
FROM Departments d 
	JOIN Employees e ON e.DepartmentID = d.DepartmentID
GROUP BY e.JobTitle, d.[Name]

-- Problem 27. Write a SQL query to display the town where maximal number of employees work.
SELECT TOP 1 t.[Name] AS 'Town with max employees', COUNT(*) AS 'Number of employees'
FROM Employees e 
	JOIN Addresses a ON e.AddressID = a.AddressID 
	JOIN Towns t ON a.TownID = t.TownID
GROUP BY t.[Name]
ORDER BY 'Number of employees' DESC

-- Problem 28. Write a SQL query to display the number of managers from each town.
SELECT t.[Name] AS 'Town', COUNT(DISTINCT e.ManagerID) AS 'Number of managers'
FROM Employees e, Employees m, Addresses a, Towns t
WHERE e.ManagerID = m.EmployeeID AND
	m.AddressID = a.AddressID AND
	a.TownID = t.TownID
GROUP BY t.Name
ORDER BY 'Number of managers' DESC

-- Problem 29. Write a SQL to create table WorkHours to store work reports for each employee 
-- (employee id, date, task, hours, comments).
-- Don't forget to define identity, primary key and appropriate foreign key.

CREATE TABLE WorkHours (
	Id int PRIMARY KEY IDENTITY,
	Task nvarchar(100) NOT NULL,
	StartDate datetime, 
	WorkHours int NOT NULL, 	 	
	Comments nvarchar(1000), 
	EmployeeID int NOT NULL
	CONSTRAINT FK_WorkHours_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
)

-- Issue few SQL statements to insert, update and delete of some data in the table.

INSERT INTO WorkHours 
VALUES
	('Some task', GETDATE(), 3, 'Some comments', 1),
	('Other task', NULL, 2, 'Other comments', 2),
	('Some other task', GETDATE(), 1, 'Some other comments', 3),
	('Final task', NULL, 4, NULL, 4)

UPDATE WorkHours
	SET EmployeeID = 100
	WHERE EmployeeID = 1

UPDATE WorkHours
	SET StartDate = GETDATE()
	WHERE StartDate IS NULL

DELETE FROM WorkHours
	WHERE Task = 'Other task'

-- Define a table WorkHoursLogs to track all changes in the WorkHours table with triggers.
-- For each change keep the old record data, the new record data and the command (insert / update / delete).

CREATE TABLE WorkHoursLogs (
	Id int PRIMARY KEY IDENTITY,
	OldEmployeeID int,
	OldTask nvarchar(100),
	OldReportDate datetime,	
	OldTaskHours int,
	OldComments nvarchar(1000),
	NewEmployeeID int,
	NewTask nvarchar(100),
	NewReportDate datetime,	
	NewTaskHours int,
	NewComments nvarchar(1000),
	Command nvarchar(20)
)
GO

CREATE TRIGGER tr_InsertTrigger ON WorkHours FOR INSERT
AS
	INSERT INTO WorkHoursLogs
	SELECT NULL, NULL, NULL, NULL, NULL, 
	i.EmployeeID, i.Task, i.StartDate, i.WorkHours, i.Comments, 'INSERT'
	FROM inserted i
GO

CREATE TRIGGER tr_UpdateTrigger ON WorkHours FOR UPDATE
AS
	INSERT INTO WorkHoursLogs
	SELECT d.EmployeeID, d.Task, d.StartDate, d.WorkHours, d.Comments, 
	i.EmployeeID, i.Task, i.StartDate, i.WorkHours, i.Comments, 'UPDATE'
	FROM inserted i, deleted d
GO

CREATE TRIGGER tr_DeleteTrigger ON WorkHours FOR DELETE
AS
	INSERT INTO WorkHoursLogs
	SELECT d.EmployeeID, d.Task, d.StartDate, d.WorkHours, d.Comments, 
	NULL, NULL, NULL, NULL, NULL, 'DELETE'
	FROM deleted d
GO

UPDATE WorkHours
	SET EmployeeID = 100
	WHERE EmployeeID = 3

UPDATE WorkHours
	SET StartDate = GETDATE()
	WHERE StartDate < GETDATE()

DELETE FROM WorkHours
	WHERE Id = 1

-- Problem 30. Start a database transaction, delete all employees from the 'Sales' department along with all dependent 
-- records from the other tables. At the end rollback the transaction.

BEGIN TRAN
	ALTER TABLE Departments
	DROP CONSTRAINT FK_Departments_Employees
	DELETE FROM Employees 
	WHERE DepartmentID = 3;
ROLLBACK TRAN

-- Problem 31. Start a database transaction and drop the table EmployeesProjects.
-- Now how you could restore back the lost table data? 

BEGIN TRAN 
	DROP TABLE EmployeesProjects
ROLLBACK TRAN 

-- Problem 32. Find how to use temporary tables in SQL Server.
-- Using temporary tables backup all records from EmployeesProjects 
-- and restore them back after dropping and re-creating the table.

SELECT * 
	INTO #Temp FROM EmployeesProjects
	DROP TABLE EmployeesProjects
		SELECT * INTO EmployeeProjects FROM #Temp
		DROP TABLE #Temp
