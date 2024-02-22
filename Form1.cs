using MySql.Data.MySqlClient;
using System.Data;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Users
{
    public partial class Form1 : Form
    {
        MySqlConnection con = new MySqlConnection("SERVER = 192.168.56.1; DATABASE = sys; UID = db; PASSWORD = Saks@2468;");
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
                    username = userValue.Text.Trim();
                    password = passwordValue.Text.Trim();
                    string query = "SELECT * FROM users WHERE username COLLATE utf8mb4_bin = @user AND password COLLATE utf8mb4_bin = @pass";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Valid username and password
                            int Status = Convert.ToInt32(reader["Status"]);
                            String device = reader["DeviceID"].ToString();
                            String topic = reader["Topics"].ToString();

                            if (Status == 1)
                            {
                                this.Hide();
                                Form dashboardForm = new Dashboard(device, topic);
                                dashboardForm.Show();
                                dashboardForm.Closed += (s, args) => this.Close();
                            }
                            else
                            {
                                MessageBox.Show("You don't have access");
                            }
                        }
                        else
                        {
                            // No matching username and password found
                            MessageBox.Show("Invalid username and password");
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
