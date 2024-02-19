using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Drawing;

namespace Users
{
    public partial class Dashboard : Form
    {
        bool connect = false;
        private MqttClient mqttClient;
        private List<string> subscribedTopics = new List<string>();
        private List<MessageData> messages = new List<MessageData>();
        MySqlConnection con = new MySqlConnection("SERVER = 192.168.1.9; DATABASE = sys; UID = db; PASSWORD = Saks@2468;");

        private string username;
        private string password;
        private string device;

        public Dashboard(string username, string password, string device)
        {
            InitializeComponent();
            this.username = username;
            this.password = password;
            this.device = device;
            this.FormClosed += Dashboard_FormClosed;
        }

        private void Dashboard_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (connect)
            {
                mqttClient.Disconnect();
                Connbtn.Text = "Connect";
                statLbl.Text = "Not Connected to broker";
                connect = false;
                MessageBox.Show("Disconnected from MQTT broker!");
            }
        }

        private void Connbtn_Click(object sender, EventArgs e)
        {
            if (connect == false)
            {
                try
                {
                    mqttClient = new MqttClient("www.mqtt-dashboard.com", 1883, false, null, null, MqttSslProtocols.None);
                    mqttClient.Connect(Guid.NewGuid().ToString());
                    MessageBox.Show("Connected to MQTT broker!");
                    Connbtn.Text = "Disconnect";
                    statLbl.Text = "Connected to broker";
                    connect = true;

                    try
                    {
                        con.Open();
                        LoadSubscribedTopics();
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot open connection!");
                    }
                    mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                    viewDataTable();
                }
                catch (MqttConnectionException ex)
                {
                    MessageBox.Show("Error connecting to MQTT broker: " + ex.Message);
                }
            }
            else
            {
                mqttClient.Disconnect();
                Connbtn.Text = "Connect";
                statLbl.Text = "Not Connected to broker";
                connect = false;
            }
        }
        void viewDataTable()
        {
            try
            {
                con.Open();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open connection!");
            }
            MySqlDataAdapter da = new MySqlDataAdapter("select * from bmcmqtt", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            serverData.DataSource = ds.Tables[0];
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (connect)
            {
                string messageText = System.Text.Encoding.UTF8.GetString(e.Message);
                string topic = e.Topic;
                List<string> parts = new List<string>(messageText.Split(','));
                BeginInvoke((Action)(() =>
                {
                    try
                    {
                        con.Open();
                        string insertQuery = "INSERT INTO bmcmqtt (DeviceId,BMCODE,Temperature,Pressure,Volume,Level,Generator,Grid,Aggregate,Compressor1,compressor2,VoltageU,VoltageV,VoltageW,CurrentU,CurrentV,CurrentW,Frequency,PwrF,TPwr,Time,Date,Topic) VALUES (@DeviceId,@BMCODE,@Temp,@Press,@Vol,@Lvl,@Gen,@Grid,@Agr,@Comp1,@Comp2,@VoltU,@VoltV,@VoltW,@CurrU,@CurrV,@CurrW,@Freq,@PwrF,@TPwr,@Time,@Date,@Topic)";
                        MySqlCommand cmd = new MySqlCommand(insertQuery, con);
                        cmd.Parameters.AddWithValue("@DeviceId", device);
                        cmd.Parameters.AddWithValue("@BMCODE", parts[0]);
                        cmd.Parameters.AddWithValue("@Temp", parts[1]);
                        cmd.Parameters.AddWithValue("@Press", parts[2]);
                        cmd.Parameters.AddWithValue("@Vol", parts[3]);
                        cmd.Parameters.AddWithValue("@Lvl", parts[4]);
                        cmd.Parameters.AddWithValue("@Gen", parts[5]);
                        cmd.Parameters.AddWithValue("@Grid", parts[6]);
                        cmd.Parameters.AddWithValue("@Agr", parts[7]);
                        cmd.Parameters.AddWithValue("@Comp1", parts[8]);
                        cmd.Parameters.AddWithValue("@Comp2", parts[9]);
                        cmd.Parameters.AddWithValue("@VoltU", parts[10]);
                        cmd.Parameters.AddWithValue("@VoltV", parts[11]);
                        cmd.Parameters.AddWithValue("@VoltW", parts[12]);
                        cmd.Parameters.AddWithValue("@CurrU", parts[13]);
                        cmd.Parameters.AddWithValue("@CurrV", parts[14]);
                        cmd.Parameters.AddWithValue("@CurrW", parts[15]);
                        cmd.Parameters.AddWithValue("@Freq", parts[16]);
                        cmd.Parameters.AddWithValue("@Pwrf", parts[17]);
                        cmd.Parameters.AddWithValue("@TPwr", parts[18]);
                        cmd.Parameters.AddWithValue("@Time", parts[19]);
                        cmd.Parameters.AddWithValue("@Date", parts[20].Substring(0, 12));
                        cmd.Parameters.AddWithValue("@Topic", topic);
                        cmd.ExecuteNonQuery();

                        // Retrieve data from the database and update the DataGridView
                        MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM bmcmqtt", con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        serverData.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error inserting into bmcmqtt table: {ex.Message}");
                    }
                    finally
                    {
                        con.Close();
                    }
                }));
            }
        }
        private void LoadSubscribedTopics()
        {
            try
            {
                subscribedTopics.Clear();

                string query = "SELECT * FROM users WHERE username = @user AND password = @pass";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string topics = reader["Topic"].ToString();
                        if (topics.Contains(','))
                        {
                            string[] topicArray = topics.Split(',');

                            foreach (string topic in topicArray)
                            {
                                string trimmedTopic = topic.Trim();
                                subscribedTopics.Add(trimmedTopic);
                                mqttClient.Subscribe(new string[] { trimmedTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                            }
                        }
                        else
                        {
                            subscribedTopics.Add(topics);
                            mqttClient.Subscribe(new string[] { topics }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subscribed topics: " + ex.Message);
            }
        }

        private void filterMyData_CheckedChanged(object sender, EventArgs e)
        {
            if (filterMyData.Checked)
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM bmcmqtt WHERE DeviceId = @deviceId", con);
                    cmd.Parameters.AddWithValue("@deviceId", device);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    serverData.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error selecting from bmcmqtt table: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                try
                {

                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM bmcmqtt", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    serverData.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error selecting from bmcmqtt table: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
    public class MessageData
    {
        public string Message { get; set; }
        public string Topic { get; set; }

        public MessageData(string message, string topic)
        {
            Message = message;
            Topic = topic;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otherMessage = (MessageData)obj;
            return Message == otherMessage.Message;
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}
