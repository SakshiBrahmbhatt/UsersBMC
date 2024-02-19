using MySql.Data.MySqlClient;
using System.Data;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Users
{
    public partial class Form1 : Form
    {
        MySqlConnection con = new MySqlConnection("SERVER = 192.168.1.9; DATABASE = sys; UID = db; PASSWORD = Saks@2468;");
        String username, password;
        public Form1()
        {
            InitializeComponent();
        }

        private void Loginbutton_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                if (!string.IsNullOrEmpty(userValue.Text) && !string.IsNullOrEmpty(passwordValue.Text))
                {
                    username = userValue.Text;
                    password = passwordValue.Text;
                    string query = "SELECT * FROM users WHERE username = @user AND password = @pass";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            int Status = Convert.ToInt32(reader["Status"]);
                            string username = reader["username"].ToString();
                            String password = reader["password"].ToString();
                            String device = reader["Device ID"].ToString();
                            if( Status == 1)
                            {
                                this.Hide();
                                Form dashboardForm = new Dashboard(username, password, device);
                                dashboardForm.Show();
                                dashboardForm.Closed += (s, args) => this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username and password or you don't have access");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Username and password required!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ ex);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
