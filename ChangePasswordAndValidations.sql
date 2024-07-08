-- החלפת סיסמא

-- הצהרה על משתנים לצורך מעקב אחר פרטי העובד
DECLARE @code INT, -- משתנה לאחסון קוד העובד
        @id VARCHAR(10) = '111', -- משתנה לאחסון מספר זהות של העובד (מספר זהות לדוגמה)
        @old_password VARCHAR(12) = '1234', -- משתנה לאחסון הסיסמא הישנה (סיסמא לדוגמה)
        @new_password VARCHAR(12) = '5678', -- משתנה לאחסון הסיסמא החדשה (סיסמא לדוגמה)
        @answer VARCHAR(100) -- משתנה לאחסון תשובה להודעת המערכת

-- מציאת קוד עובד לפי מספר זהות
SELECT @code = code FROM Employees WHERE id = @id

-- אם אין עובד - הקפצת הודעת תשובה "שם משתמש או סיסמא לא נכונים"
IF @code IS NULL
BEGIN
    SELECT @answer = 'מספר זהות או סיסמא אינם נכונים' -- הקצאת הודעה למשתנה התשובה
    RETURN -- יציאה מהפרוצדורה
END

-- בדיקה האם הסיסמא הישנה קיימת ובתוקף
IF NOT EXISTS (SELECT * FROM Passwords WHERE employee_code = @code AND password = @old_password AND has_access=1)
BEGIN
    SELECT @answer = 'מספר זהות או סיסמא אינם נכונים' -- הקצאת הודעה למשתנה התשובה
    RETURN -- יציאה מהפרוצדורה
END

-- בדיקה האם הסיסמא החדשה כבר קיימת
IF EXISTS (SELECT code FROM Passwords WHERE employee_code = @code AND password = @new_password)
BEGIN
    SELECT @answer = 'נא בחר סיסמא חדשה' -- הקצאת הודעה למשתנה התשובה אם הסיסמא החדשה כבר קיימת
END
ELSE
BEGIN
    -- עדכון הסיסמא הישנה כלא בתוקף
    UPDATE Passwords SET has_access = 0 WHERE employee_code = @code

    -- הוספת הסיסמא החדשה עם תוקף של 180 ימים וגישה פעילה
    INSERT INTO Passwords VALUES (@code, @new_password, GETDATE() + 180, 1)

    -- הקצאת הודעה למשתנה התשובה שהסיסמא הוחלפה בהצלחה
    SELECT @answer = 'הסיסמא הוחלפה בהצלחה. תוקף הסיסמא 180 ימים'
END

-- הצגת ההודעה הסופית
SELECT @answer
