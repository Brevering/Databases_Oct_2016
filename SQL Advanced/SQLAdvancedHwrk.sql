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
SELECT e.FirstName + ' ' + e.LastName AS "Full name", e.Salary, d.Name AS Department 
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
-- You can also say SELECT AVG(Salary) AS 'Average Salary' FROM Employees WHERE DepartmentID = 1

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
SELECT d.Name AS 'Department', t.Name AS 'Town', COUNT(*) AS 'Number of employees'
FROM Employees e 
	JOIN Addresses a ON e.AddressID = a.AddressID 
	JOIN Towns t ON a.TownID = t.TownID 
	JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY t.Name, d.Name
ORDER BY Department

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
Username NVARCHAR(50) NOT NULL,
[Password] NVARCHAR(50) NOT NULL,
FullName NVARCHAR(100) NOT NULL,
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

-- Problem 18. Write a SQL statement to add a column GroupID to the table Users.
ALTER TABLE Users ADD GroupID INT
--			   Fill some data in this new column and as well in the `Groups table.
INSERT INTO Groups ([Name]) VALUES ('Males'), ('Females') -- added respective values in Users table by hand 
--			   Write a SQL statement to add a foreign key constraint between tables Users and Groups tables.
ALTER TABLE Users
	ADD FOREIGN KEY(GroupId) REFERENCES Groups(Id)



