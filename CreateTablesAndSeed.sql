--����� ���� ������--
CREATE TABLE Employees(
	code int PRIMARY KEY IDENTITY, 
	id varchar(10), 
	first_name varchar(15), 
	last_name varchar(15)
)

--����� �����
SELECT * FROM Employees

--����� ���� ������
CREATE TABLE Shifts(
	code int PRIMARY KEY IDENTITY,
	employee_code int FOREIGN KEY REFERENCES Employees(code),
	entry_time DATETIME, exit_time DATETIME
)

--����� �����
SELECT * FROM Shifts

--����� ���� �������
CREATE TABLE Passwords(
	code int PRIMARY KEY IDENTITY, 
	employee_code int FOREIGN KEY REFERENCES Employees(code), 
	password VARCHAR(12), 
	expiry_date DATE, 
	has_access BIT
)

-- ����� �����
SELECT * FROM Passwords


-- ����� ���� ��� ����� ������ ����� ������ ������� ������ �������

-- ������ ������� �� ���� �����
DECLARE 
	@id varchar(10)='111', 
	@first_name varchar(15)='rami', 
	@last_name varchar(15)='shakshuka', 
	@code int, 
	@password varchar(12)='1234'

--����� ��� ����� ��� ���� ������
IF EXISTS (SELECT * FROM Employees WHERE id=@id)
	BEGIN -- ����� �������
		SELECT @code=(SELECT code FROM Employees WHERE id=@id)
	END -- ����� �������
ELSE -- ����� �� ���� ������
	BEGIN
		--����� ����� ����
		INSERT INTO Employees VALUES(@id,@first_name,@last_name)
		SELECT @code = @@IDENTITY 
	END

--����� ����� �����
INSERT INTO Passwords VALUES(@code,@password, GETDATE(), 1)

SELECT * FROM Passwords
SELECT * FROM Employees
SELECT * FROM Shifts

