namespace Users
{
    partial class Dashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            statLbl = new Label();
            Connbtn = new Button();
            serverData = new DataGridView();
            filterMyData = new CheckBox();
            searchBtn = new Button();
            searchText = new TextBox();
            ((System.ComponentModel.ISupportInitialize)serverData).BeginInit();
            SuspendLayout();
            // 
            // statLbl
            // 
            statLbl.AutoSize = true;
            statLbl.Location = new Point(112, 16);
            statLbl.Name = "statLbl";
            statLbl.Size = new Size(172, 20);
            statLbl.TabIndex = 3;
            statLbl.Text = "Not connected to broker";
            // 
            // Connbtn
            // 
            Connbtn.Location = new Point(12, 12);
            Connbtn.Name = "Connbtn";
            Connbtn.Size = new Size(94, 29);
            Connbtn.TabIndex = 2;
            Connbtn.Text = "Connect";
            Connbtn.UseVisualStyleBackColor = true;
            Connbtn.Click += Connbtn_Click;
            // 
            // serverData
            // 
            serverData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            serverData.BackgroundColor = SystemColors.InactiveCaption;
            serverData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            serverData.Location = new Point(0, 125);
            serverData.Name = "serverData";
            serverData.RowHeadersWidth = 51;
            serverData.Size = new Size(1837, 377);
            serverData.TabIndex = 5;
            // 
            // filterMyData
            // 
            filterMyData.AutoSize = true;
            filterMyData.Location = new Point(12, 95);
            filterMyData.Name = "filterMyData";
            filterMyData.Size = new Size(157, 24);
            filterMyData.TabIndex = 10;
            filterMyData.Text = "Only show my data";
            filterMyData.UseVisualStyleBackColor = true;
            filterMyData.CheckedChanged += filterMyData_CheckedChanged;
            // 
            // searchBtn
            // 
            searchBtn.Location = new Point(408, 93);
            searchBtn.Name = "searchBtn";
            searchBtn.Size = new Size(94, 29);
            searchBtn.TabIndex = 13;
            searchBtn.Text = "Search";
            searchBtn.UseVisualStyleBackColor = true;
            // 
            // searchText
            // 
            searchText.Location = new Point(236, 95);
            searchText.Name = "searchText";
            searchText.PlaceholderText = "topic/#";
            searchText.Size = new Size(166, 27);
            searchText.TabIndex = 12;
            searchText.TextChanged += searchText_TextChanged;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1849, 502);
            Controls.Add(searchBtn);
            Controls.Add(searchText);
            Controls.Add(filterMyData);
            Controls.Add(serverData);
            Controls.Add(statLbl);
            Controls.Add(Connbtn);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            ((System.ComponentModel.ISupportInitialize)serverData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label statLbl;
        private Button Connbtn;
        private DataGridView serverData;
        private CheckBox filterMyData;
        private Button searchBtn;
        private TextBox searchText;
    }
}