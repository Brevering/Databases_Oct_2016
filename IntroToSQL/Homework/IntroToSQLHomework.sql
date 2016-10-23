-- 4. Write a SQL query to find all information about all departments
SELECT * FROM Departments

-- 5. Write a SQL query to find all department names.
SELECT Name FROM Departments

-- 6. Write a SQL query to find the salary of each employee.
SELECT FirstName, LastName, Salary FROM Employees

-- 7. Write a SQL to find the full name of each employee.
SELECT FirstName + ' ' + LastName AS 'Full Name' FROM Employees

-- 8. Write a SQL query to find the email addresses of each employee (by his first and last name). 
-- Consider that the mail domain is telerik.com. Emails should look like “John.Doe@telerik.com". 
-- The produced column should be named "Full Email Addresses".

SELECT FirstName + '.' + LastName + '@telerik.com' AS 'Full Email Address' FROM Employees

-- 9. 
