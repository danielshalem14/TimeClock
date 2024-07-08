--יצירת טבלת עובדים--
CREATE TABLE Employees(
	code int PRIMARY KEY IDENTITY, 
	id varchar(10), 
	first_name varchar(15), 
	last_name varchar(15)
)

--בדיקת הטבלה
SELECT * FROM Employees

--יצירת טבלת נוכחות
CREATE TABLE Shifts(
	code int PRIMARY KEY IDENTITY,
	employee_code int FOREIGN KEY REFERENCES Employees(code),
	entry_time DATETIME, exit_time DATETIME
)

--בדיקת הטבלה
SELECT * FROM Shifts

--יצירת טבלת סיסמאות
CREATE TABLE Passwords(
	code int PRIMARY KEY IDENTITY, 
	employee_code int FOREIGN KEY REFERENCES Employees(code), 
	password VARCHAR(12), 
	expiry_date DATE, 
	has_access BIT
)

-- בדיקת הטבלה
SELECT * FROM Passwords


-- הכנסת עובד חדש לחברה וסיסמא זמנית שיצטרך להחליפה בכניסה הראשונה

-- משתנים המכילים את פרטי העובד
DECLARE 
	@id varchar(10)='111', 
	@first_name varchar(15)='rami', 
	@last_name varchar(15)='shakshuka', 
	@code int, 
	@password varchar(12)='1234'

--בדיקה האם העובד כבר קיים במערכת
IF EXISTS (SELECT * FROM Employees WHERE id=@id)
	BEGIN -- פתיחת סוגריים
		SELECT @code=(SELECT code FROM Employees WHERE id=@id)
	END -- סגירת סוגריים
ELSE -- העובד לא קיים במערכת
	BEGIN
		--הכנסת העובד החדש
		INSERT INTO Employees VALUES(@id,@first_name,@last_name)
		SELECT @code = @@IDENTITY 
	END

--הכנסת סיסמא זמנית
INSERT INTO Passwords VALUES(@code,@password, GETDATE(), 1)

SELECT * FROM Passwords
SELECT * FROM Employees
SELECT * FROM Shifts

