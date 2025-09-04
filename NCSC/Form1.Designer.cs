namespace NCSC
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            register_label = new Label();
            loginButton = new Guna.UI2.WinForms.Guna2Button();
            label5 = new Label();
            show_password = new Guna.UI2.WinForms.Guna2CheckBox();
            label4 = new Label();
            passwordField = new Guna.UI2.WinForms.Guna2TextBox();
            label3 = new Label();
            usernameField = new Guna.UI2.WinForms.Guna2TextBox();
            label2 = new Label();
            closeButton = new Guna.UI2.WinForms.Guna2Button();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(components);
            guna2GradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 12;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2GradientPanel1
            // 
            guna2GradientPanel1.Controls.Add(register_label);
            guna2GradientPanel1.Controls.Add(loginButton);
            guna2GradientPanel1.Controls.Add(label5);
            guna2GradientPanel1.Controls.Add(show_password);
            guna2GradientPanel1.Controls.Add(label4);
            guna2GradientPanel1.Controls.Add(passwordField);
            guna2GradientPanel1.Controls.Add(label3);
            guna2GradientPanel1.Controls.Add(usernameField);
            guna2GradientPanel1.Controls.Add(label2);
            guna2GradientPanel1.Controls.Add(closeButton);
            guna2GradientPanel1.CustomizableEdges = customizableEdges9;
            guna2GradientPanel1.FillColor = Color.FromArgb(166, 172, 214);
            guna2GradientPanel1.FillColor2 = Color.FromArgb(45, 48, 114);
            guna2GradientPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            guna2GradientPanel1.Location = new Point(350, -3);
            guna2GradientPanel1.Name = "guna2GradientPanel1";
            guna2GradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges10;
            guna2GradientPanel1.Size = new Size(361, 455);
            guna2GradientPanel1.TabIndex = 0;
            // 
            // register_label
            // 
            register_label.AutoSize = true;
            register_label.BackColor = Color.Transparent;
            register_label.Cursor = Cursors.Hand;
            register_label.Font = new Font("Poppins SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            register_label.ForeColor = Color.FromArgb(116, 144, 254);
            register_label.Location = new Point(123, 383);
            register_label.Name = "register_label";
            register_label.Size = new Size(115, 22);
            register_label.TabIndex = 11;
            register_label.Text = "Register Account";
            register_label.TextAlign = ContentAlignment.BottomCenter;
            register_label.Click += label6_Click;
            // 
            // loginButton
            // 
            loginButton.Animated = true;
            loginButton.BackColor = Color.Transparent;
            loginButton.BorderRadius = 8;
            loginButton.CustomizableEdges = customizableEdges1;
            loginButton.DisabledState.BorderColor = Color.DarkGray;
            loginButton.DisabledState.CustomBorderColor = Color.DarkGray;
            loginButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            loginButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            loginButton.FillColor = Color.FromArgb(134, 115, 243);
            loginButton.Font = new Font("Poppins SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            loginButton.ForeColor = Color.White;
            loginButton.Location = new Point(42, 330);
            loginButton.Name = "loginButton";
            loginButton.ShadowDecoration.CustomizableEdges = customizableEdges2;
            loginButton.Size = new Size(277, 39);
            loginButton.TabIndex = 10;
            loginButton.Text = "Login";
            loginButton.Click += loginButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Cursor = Cursors.Hand;
            label5.Font = new Font("Poppins SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(116, 144, 254);
            label5.Location = new Point(199, 274);
            label5.Name = "label5";
            label5.Size = new Size(120, 22);
            label5.TabIndex = 9;
            label5.Text = "Forgot password?";
            label5.TextAlign = ContentAlignment.BottomCenter;
            // 
            // show_password
            // 
            show_password.Animated = true;
            show_password.AutoSize = true;
            show_password.BackColor = Color.Transparent;
            show_password.CheckedState.BorderColor = Color.FromArgb(134, 115, 243);
            show_password.CheckedState.BorderRadius = 4;
            show_password.CheckedState.BorderThickness = 0;
            show_password.CheckedState.FillColor = Color.FromArgb(134, 115, 243);
            show_password.Font = new Font("Poppins SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            show_password.Location = new Point(42, 274);
            show_password.Name = "show_password";
            show_password.Size = new Size(125, 26);
            show_password.TabIndex = 8;
            show_password.Text = "Show Password";
            show_password.TextAlign = ContentAlignment.MiddleCenter;
            show_password.UncheckedState.BorderColor = Color.White;
            show_password.UncheckedState.BorderRadius = 4;
            show_password.UncheckedState.BorderThickness = 0;
            show_password.UncheckedState.FillColor = Color.White;
            show_password.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Poppins SemiBold", 11.25F, FontStyle.Bold);
            label4.Location = new Point(42, 200);
            label4.Name = "label4";
            label4.Size = new Size(86, 26);
            label4.TabIndex = 7;
            label4.Text = "Password";
            label4.TextAlign = ContentAlignment.BottomCenter;
            // 
            // passwordField
            // 
            passwordField.Animated = true;
            passwordField.BackColor = Color.Transparent;
            passwordField.BorderColor = Color.White;
            passwordField.BorderRadius = 8;
            passwordField.CustomizableEdges = customizableEdges3;
            passwordField.DefaultText = "";
            passwordField.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            passwordField.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            passwordField.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            passwordField.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            passwordField.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            passwordField.Font = new Font("Poppins", 9F);
            passwordField.ForeColor = SystemColors.ControlText;
            passwordField.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            passwordField.Location = new Point(42, 229);
            passwordField.Margin = new Padding(3, 6, 3, 6);
            passwordField.Name = "passwordField";
            passwordField.PasswordChar = '*';
            passwordField.PlaceholderText = "";
            passwordField.SelectedText = "";
            passwordField.ShadowDecoration.CustomizableEdges = customizableEdges4;
            passwordField.Size = new Size(277, 39);
            passwordField.TabIndex = 6;
            passwordField.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Poppins SemiBold", 11.25F, FontStyle.Bold);
            label3.Location = new Point(42, 120);
            label3.Name = "label3";
            label3.Size = new Size(90, 26);
            label3.TabIndex = 5;
            label3.Text = "Username";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.Click += label3_Click;
            // 
            // usernameField
            // 
            usernameField.Animated = true;
            usernameField.BackColor = Color.Transparent;
            usernameField.BorderColor = Color.White;
            usernameField.BorderRadius = 8;
            usernameField.CustomizableEdges = customizableEdges5;
            usernameField.DefaultText = "";
            usernameField.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            usernameField.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            usernameField.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            usernameField.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            usernameField.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            usernameField.Font = new Font("Poppins", 9F);
            usernameField.ForeColor = SystemColors.ControlText;
            usernameField.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            usernameField.Location = new Point(42, 149);
            usernameField.Margin = new Padding(3, 6, 3, 6);
            usernameField.Name = "usernameField";
            usernameField.PlaceholderText = "";
            usernameField.SelectedText = "";
            usernameField.ShadowDecoration.CustomizableEdges = customizableEdges6;
            usernameField.Size = new Size(277, 39);
            usernameField.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Poppins SemiBold", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(42, 44);
            label2.Name = "label2";
            label2.Size = new Size(101, 51);
            label2.TabIndex = 3;
            label2.Text = "Login";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            label2.Click += label2_Click;
            // 
            // closeButton
            // 
            closeButton.Animated = true;
            closeButton.BackColor = Color.Transparent;
            closeButton.BorderColor = Color.Transparent;
            closeButton.CustomizableEdges = customizableEdges7;
            closeButton.DisabledState.BorderColor = Color.DarkGray;
            closeButton.DisabledState.CustomBorderColor = Color.DarkGray;
            closeButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            closeButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            closeButton.FillColor = Color.Transparent;
            closeButton.Font = new Font("Segoe UI", 9F);
            closeButton.ForeColor = Color.White;
            closeButton.HoverState.BorderColor = Color.Transparent;
            closeButton.HoverState.FillColor = Color.Transparent;
            closeButton.HoverState.Image = (Image)resources.GetObject("resource.Image");
            closeButton.Image = (Image)resources.GetObject("closeButton.Image");
            closeButton.ImageSize = new Size(15, 15);
            closeButton.Location = new Point(331, 5);
            closeButton.Name = "closeButton";
            closeButton.PressedColor = Color.Transparent;
            closeButton.PressedDepth = 0;
            closeButton.ShadowDecoration.Color = Color.Transparent;
            closeButton.ShadowDecoration.CustomizableEdges = customizableEdges8;
            closeButton.Size = new Size(29, 26);
            closeButton.TabIndex = 0;
            closeButton.Click += closeButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 75);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(332, 249);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Poppins SemiBold", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(97, 327);
            label1.Name = "label1";
            label1.Size = new Size(160, 51);
            label1.TabIndex = 2;
            label1.Text = "Region IX";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // guna2AnimateWindow1
            // 
            guna2AnimateWindow1.AnimationType = Guna.UI2.WinForms.Guna2AnimateWindow.AnimateWindowType.AW_BLEND;
            guna2AnimateWindow1.Interval = 200;
            guna2AnimateWindow1.TargetForm = this;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(711, 449);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(guna2GradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            guna2GradientPanel1.ResumeLayout(false);
            guna2GradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Guna.UI2.WinForms.Guna2Button closeButton;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Guna.UI2.WinForms.Guna2TextBox usernameField;
        private Label label4;
        private Guna.UI2.WinForms.Guna2TextBox passwordField;
        private Guna.UI2.WinForms.Guna2CheckBox show_password;
        private Guna.UI2.WinForms.Guna2Button loginButton;
        private Label label5;
        private Label register_label;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
    }
}
