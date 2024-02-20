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
        MySqlConnection con = new MySqlConnection("SERVER = 192.168.1.7; DATABASE = sys; UID = db; PASSWORD = Saks@2468;");
        private string device;
        private string topic;

        public Dashboard(string device, string topic)
        {
            InitializeComponent();
            this.device = device;
            this.FormClosed += Dashboard_FormClosed;
            this.topic = topic;
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
                    MessageBox.Show("Topics:" + topic);
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
                string query = "SELECT * FROM bmcmqtt WHERE Topic COLLATE utf8mb4_bin = @top";
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();

                if (topic.Contains(','))
                {
                    string[] topicArray = topic.Split(',');

                    foreach (string topicItem in topicArray)
                    {
                        string trimmedTopic = topicItem.Trim();
                        da.SelectCommand.Parameters.Clear();
                        da.SelectCommand.Parameters.AddWithValue("@top", trimmedTopic);
                        DataTable tempTable = new DataTable();
                        da.Fill(tempTable);
                        table.Merge(tempTable);
                    }
                }
                else
                {
                    da.SelectCommand.Parameters.AddWithValue("@top", topic);
                    da.Fill(table);
                }

                serverData.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open connection!");
            }
            finally
            {
                con.Close();
            }
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
                        string query = "SELECT * FROM bmcmqtt WHERE Topic COLLATE utf8mb4_bin = @top";
                        MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                        DataTable table = new DataTable();

                        if (topic.Contains(','))
                        {
                            string[] topicArray = topic.Split(',');

                            foreach (string topicItem in topicArray)
                            {
                                string trimmedTopic = topicItem.Trim();
                                da.SelectCommand.Parameters.Clear();
                                da.SelectCommand.Parameters.AddWithValue("@top", trimmedTopic);
                                DataTable tempTable = new DataTable();
                                da.Fill(tempTable);
                                table.Merge(tempTable);
                            }
                        }
                        else
                        {
                            da.SelectCommand.Parameters.AddWithValue("@top", topic);
                            da.Fill(table);
                        }

                        serverData.DataSource = table;
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
                if (topic.Contains(','))
                {
                    string[] topicArray = topic.Split(',');

                    foreach (string topic in topicArray)
                    {
                        string trimmedTopic = topic.Trim();
                        subscribedTopics.Add(trimmedTopic);
                        mqttClient.Subscribe(new string[] { trimmedTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    }
                }
                else
                {
                    subscribedTopics.Add(topic);
                    mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
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
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM bmcmqtt WHERE DeviceId COLLATE utf8mb4_bin = @deviceId", con);
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
                    con.Open();
                    string query = "SELECT * FROM bmcmqtt WHERE Topic COLLATE utf8mb4_bin = @top";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable table = new DataTable();

                    if (topic.Contains(','))
                    {
                        string[] topicArray = topic.Split(',');

                        foreach (string topicItem in topicArray)
                        {
                            string trimmedTopic = topicItem.Trim();
                            da.SelectCommand.Parameters.Clear();
                            da.SelectCommand.Parameters.AddWithValue("@top", trimmedTopic);
                            DataTable tempTable = new DataTable();
                            da.Fill(tempTable);
                            table.Merge(tempTable);
                        }
                    }
                    else
                    {
                        da.SelectCommand.Parameters.AddWithValue("@top", topic);
                        da.Fill(table);
                    }

                    serverData.DataSource = table;
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

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchText.Text))
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM bmcmqtt WHERE Topic COLLATE utf8mb4_bin = @Topic", con);
                    cmd.Parameters.AddWithValue("@Topic", searchText.Text);

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
                MessageBox.Show("Enter the value to search the row");
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
