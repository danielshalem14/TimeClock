using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeClock
{
    public partial class ChangePasswordForm : Form
    {

        SqlConnection Connection;

        private readonly string ConnectionString = "Your_Connection_String";

        private string userId;
        public ChangePasswordForm(string _UserId)
        {
            InitializeComponent();
            userId = _UserId;
        }

        private bool Connect()
        {
            try
            {
                Connection = new SqlConnection(ConnectionString);
                Connection.Open();
                return true;
            }
            catch (SqlException ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Disconnect()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string newPasswordValidate = txtNewPasswordValidation.Text;

            if (newPasswordValidate != newPassword)
            {
                MessageBox.Show("סיסמאות לא תואמות");
                return;
            }

            // בדיקה האם הסיסמא החדשה הוכנסה בעבר
            string query = @"
                DECLARE @code INT,
                        @answer VARCHAR(100)

                -- מציאת קוד עובד לפי מספר זהות
                SELECT @code = code FROM Employees WHERE id = @id

                -- אם אין עובד - הקפצת הודעת תשובה
                IF @code IS NULL
                BEGIN
                    SELECT @answer = 'מספר זהות או סיסמא אינם נכונים'
                    RETURN
                END

                -- בדיקה האם הסיסמא הישנה קיימת ובתוקף
                IF NOT EXISTS (SELECT * FROM Passwords WHERE employee_code = @code AND password = @old_password AND has_access=1)
                BEGIN
                    SELECT @answer = 'מספר זהות או סיסמא אינם נכונים'
                    RETURN
                END

                -- בדיקה האם הסיסמא החדשה כבר קיימת
                IF EXISTS (SELECT code FROM Passwords WHERE employee_code = @code AND password = @new_password)
                BEGIN
                    SELECT @answer = 'נא בחר סיסמא חדשה'
                END
                ELSE
                BEGIN
                    -- עדכון הסיסמא הישנה כלא בתוקף
                    UPDATE Passwords SET has_access = 0 WHERE employee_code = @code

                    -- הוספת הסיסמא החדשה עם תוקף של 180 ימים וגישה פעילה
                    INSERT INTO Passwords (employee_code, password, expiry_date, has_access) VALUES (@code, @new_password, GETDATE() + 180, 1)

                    -- הקצאת הודעה למשתנה התשובה שהסיסמא הוחלפה בהצלחה
                    SELECT @answer = 'הסיסמא הוחלפה בהצלחה. תוקף הסיסמא 180 ימים'
                END

                -- הצגת ההודעה הסופית
                SELECT @answer";


            if (Connect())
            {
                SqlCommand cmd = new SqlCommand(query, Connection);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@old_password", oldPassword);
                cmd.Parameters.AddWithValue("@new_password", newPassword);

                object result = cmd.ExecuteScalar();
                MessageBox.Show(result.ToString());
                this.Close();

            }
            Disconnect();

        }
    }
}
