namespace Users
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            passwordValue = new TextBox();
            userValue = new TextBox();
            label2 = new Label();
            label1 = new Label();
            Loginbutton = new Button();
            SuspendLayout();
            // 
            // passwordValue
            // 
            passwordValue.Location = new Point(213, 150);
            passwordValue.Name = "passwordValue";
            passwordValue.Size = new Size(125, 27);
            passwordValue.TabIndex = 7;
            passwordValue.UseSystemPasswordChar = true;
            // 
            // userValue
            // 
            userValue.Location = new Point(213, 102);
            userValue.Name = "userValue";
            userValue.Size = new Size(125, 27);
            userValue.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(126, 153);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 5;
            label2.Text = "Password";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(126, 106);
            label1.Name = "label1";
            label1.Size = new Size(75, 20);
            label1.TabIndex = 4;
            label1.Text = "Username";
            // 
            // Loginbutton
            // 
            Loginbutton.Location = new Point(177, 249);
            Loginbutton.Name = "Loginbutton";
            Loginbutton.Size = new Size(94, 29);
            Loginbutton.TabIndex = 8;
            Loginbutton.Text = "Login";
            Loginbutton.UseVisualStyleBackColor = true;
            Loginbutton.Click += Loginbutton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(426, 344);
            Controls.Add(Loginbutton);
            Controls.Add(passwordValue);
            Controls.Add(userValue);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox passwordValue;
        private TextBox userValue;
        private Label label2;
        private Label label1;
        private Button Loginbutton;
    }
}
