using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TimeClock
{
    public partial class LoginForm : Form
    {
        SqlConnection Connection;

        // מחרוזת התחברות למסד הנתונים
        private readonly string ConnectionString = "Your Connection String";

        public LoginForm()
        {
            InitializeComponent();
        }

        // פונקציה לחיבור למסד נתונים
        private bool Connect()
        {
            try
            {
                // יצירת חיבור עם מסד הנתונים
                Connection = new SqlConnection(ConnectionString);
                // פתיחת החיבור
                Connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                // הצגת הודעת שגיאה במקרה של כישלון
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // פונקציה לניתוק ממסד נתונים
        private void Disconnect()
        {
            // בדיקה אם החיבור פתוח
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                // סגירת החיבור
                Connection.Close();
            }
        }

        // פונקציה לבדוק התחברות המשתמש
        private DataTable CheckLogin(string id, string password)
        {
            // יצירת טבלה ריקה לתוצאות
            DataTable result = new DataTable();
            // שאילתא לבדיקת התחברות
            string query = @"SELECT Employees.*, Passwords.expiry_date FROM Employees 
                             INNER JOIN Passwords 
                             ON Employees.code = Passwords.employee_code 
                             WHERE Employees.id = @id 
                             AND Passwords.password = @password
                             AND Passwords.has_access = 1";

            if (Connect())
            {
                using (SqlCommand cmd = new SqlCommand(query, Connection))
                {
                    // הוספת פרמטרים לשאילתא
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@password", password);
                    // מילוי התוצאות בטבלה
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                // ניתוק ממסד הנתונים
                Disconnect();
            }

            return result;
        }

        // פונקציה לבדוק תוקף סיסמא
        private bool IsPasswordExpired(DataRow userRow)
        {
            // קבלת תאריך התפוגה מהשורה
            DateTime expirationDate = (DateTime)userRow["expiry_date"];
            // בדיקה אם התאריך עבר
            return expirationDate < DateTime.Now;
        }

        // פונקציה לבדוק אם יש משמרת פעילה
        private DataTable CheckActiveShift(string employeeCode)
        {
            DataTable shiftTable = new DataTable();
            // שאילתא לבדוק משמרת פעילה
            string queryCheckShift = @"SELECT TOP 1 * FROM Shifts 
                                       WHERE employee_code = @code 
                                       AND exit_time IS NULL 
                                       ORDER BY entry_time DESC";

            using (SqlCommand cmd = new SqlCommand(queryCheckShift, Connection))
            {
                cmd.Parameters.AddWithValue("@code", employeeCode);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(shiftTable);
            }

            return shiftTable;
        }

        // פונקציה לעדכן שעת יציאה
        private void UpdateExitTime(string shiftCode)
        {
            // שאילתא לעדכן שעת יציאה
            string updateExitTimeQuery = @"UPDATE Shifts 
                                           SET exit_time = @exit_time 
                                           WHERE code = @shift_code";

            using (SqlCommand cmd = new SqlCommand(updateExitTimeQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@exit_time", DateTime.Now);
                cmd.Parameters.AddWithValue("@shift_code", shiftCode);
                cmd.ExecuteNonQuery();
            }
        }

        // פונקציה להוספת שעת כניסה
        private void InsertEntryTime(string employeeCode)
        {
            // שאילתא להוסיף שעת כניסה
            string insertEntryTimeQuery = @"INSERT INTO Shifts (employee_code, entry_time) 
                                            VALUES (@code, @entry_time)";

            using (SqlCommand cmd = new SqlCommand(insertEntryTimeQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@code", employeeCode);
                cmd.Parameters.AddWithValue("@entry_time", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            string password = txtPassword.Text;

            // בדיקה אם כל השדות מלאים
            if (id == "" || password == "")
            {
                MessageBox.Show("נא למלא את כל השדות");
                return;
            }

            // בדיקה אם התחברות תקינה
            DataTable userDataTable = CheckLogin(id, password);

            if (userDataTable.Rows.Count == 0)
            {
                MessageBox.Show("תעודת זהות או סיסמא לא תקינים");
                return;
            }

            if (Connect())
            {
                DataRow userRow = userDataTable.Rows[0];

                // בדיקה אם תוקף הסיסמא פג
                if (IsPasswordExpired(userRow))
                {
                    MessageBox.Show("תאריך תוקף הסיסמא שלך פג, עליך להחליף סיסמא");
                    return;
                }

                // בדיקה אם יש משמרת פעילה
                DataTable shiftTable = CheckActiveShift(userRow["code"].ToString());

                if (shiftTable.Rows.Count > 0)
                {
                    // עדכון שעת יציאה
                    UpdateExitTime(shiftTable.Rows[0]["code"].ToString());
                    MessageBox.Show("יציאה בוצעה בהצלחה, תודה");
                }
                else
                {
                    // הוספת שעת כניסה חדשה
                    InsertEntryTime(userRow["code"].ToString());
                    MessageBox.Show("כניסה בוצעה בהצלחה");
                }

                Disconnect();
            }
        }

        private void btnChangePassword_Click_1(object sender, EventArgs e)
        {
            // פתיחת טופס לשינוי סיסמא
            ChangePasswordForm changePasswordForm = new ChangePasswordForm(txtId.Text);
            changePasswordForm.ShowDialog();
        }
    }
}


//SELECT* FROM Passwords
//SELECT * FROM Employees
//SELECT * FROM Shifts


//-- CheckLogin
//SELECT Employees.*, Passwords.expiry_date FROM Employees 
//                    INNER JOIN Passwords 
//                    ON Employees.code = Passwords.employee_code 
//                    WHERE Employees.id = 111 
//                    AND Passwords.password = 54321
//                    AND Passwords.has_access = 1


//-- CheckActiveShift
//SELECT TOP 1 * FROM Shifts 
//       WHERE employee_code = 3
//       AND exit_time IS NULL 
//       ORDER BY entry_time DESC