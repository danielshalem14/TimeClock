-- ����� �����

-- ����� �� ������ ����� ���� ��� ���� �����
DECLARE @code INT, -- ����� ������ ��� �����
        @id VARCHAR(10) = '111', -- ����� ������ ���� ���� �� ����� (���� ���� ������)
        @old_password VARCHAR(12) = '1234', -- ����� ������ ������ ����� (����� ������)
        @new_password VARCHAR(12) = '5678', -- ����� ������ ������ ����� (����� ������)
        @answer VARCHAR(100) -- ����� ������ ����� ������ ������

-- ����� ��� ���� ��� ���� ����
SELECT @code = code FROM Employees WHERE id = @id

-- �� ��� ���� - ����� ����� ����� "�� ����� �� ����� �� ������"
IF @code IS NULL
BEGIN
    SELECT @answer = '���� ���� �� ����� ���� ������' -- ����� ����� ������ ������
    RETURN -- ����� ����������
END

-- ����� ��� ������ ����� ����� ������
IF NOT EXISTS (SELECT * FROM Passwords WHERE employee_code = @code AND password = @old_password AND has_access=1)
BEGIN
    SELECT @answer = '���� ���� �� ����� ���� ������' -- ����� ����� ������ ������
    RETURN -- ����� ����������
END

-- ����� ��� ������ ����� ��� �����
IF EXISTS (SELECT code FROM Passwords WHERE employee_code = @code AND password = @new_password)
BEGIN
    SELECT @answer = '�� ��� ����� ����' -- ����� ����� ������ ������ �� ������ ����� ��� �����
END
ELSE
BEGIN
    -- ����� ������ ����� ��� �����
    UPDATE Passwords SET has_access = 0 WHERE employee_code = @code

    -- ����� ������ ����� �� ���� �� 180 ���� ����� �����
    INSERT INTO Passwords VALUES (@code, @new_password, GETDATE() + 180, 1)

    -- ����� ����� ������ ������ ������� ������ ������
    SELECT @answer = '������ ������ ������. ���� ������ 180 ����'
END

-- ���� ������ ������
SELECT @answer
