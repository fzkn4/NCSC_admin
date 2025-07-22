namespace NCSC
{
    partial class BenefeciariesBdayMonth
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BenefeciariesBdayMonth));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            header = new Label();
            beneficiaries_bday_month_table = new Guna.UI2.WinForms.Guna2DataGridView();
            batch_code_col = new DataGridViewTextBoxColumn();
            age_col = new DataGridViewTextBoxColumn();
            birth_date_col = new DataGridViewTextBoxColumn();
            sex_col = new DataGridViewTextBoxColumn();
            region_col = new DataGridViewTextBoxColumn();
            province_col = new DataGridViewTextBoxColumn();
            minicipalities_col = new DataGridViewTextBoxColumn();
            barangay_col = new DataGridViewTextBoxColumn();
            date_validated_col = new DataGridViewTextBoxColumn();
            pwd_col = new DataGridViewTextBoxColumn();
            ip_col = new DataGridViewTextBoxColumn();
            closeButton = new Guna.UI2.WinForms.Guna2Button();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(components);
            ((System.ComponentModel.ISupportInitialize)beneficiaries_bday_month_table).BeginInit();
            SuspendLayout();
            // 
            // header
            // 
            header.BackColor = Color.Transparent;
            header.Font = new Font("Poppins", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            header.Location = new Point(12, 9);
            header.Name = "header";
            header.Size = new Size(1147, 48);
            header.TabIndex = 7;
            header.Text = "Month of January";
            header.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // beneficiaries_bday_month_table
            // 
            beneficiaries_bday_month_table.AllowUserToAddRows = false;
            beneficiaries_bday_month_table.AllowUserToDeleteRows = false;
            beneficiaries_bday_month_table.AllowUserToResizeColumns = false;
            beneficiaries_bday_month_table.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            beneficiaries_bday_month_table.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(134, 115, 243);
            dataGridViewCellStyle2.Font = new Font("Poppins SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(134, 115, 243);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            beneficiaries_bday_month_table.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            beneficiaries_bday_month_table.ColumnHeadersHeight = 32;
            beneficiaries_bday_month_table.Columns.AddRange(new DataGridViewColumn[] { batch_code_col, age_col, birth_date_col, sex_col, region_col, province_col, minicipalities_col, barangay_col, date_validated_col, pwd_col, ip_col });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            beneficiaries_bday_month_table.DefaultCellStyle = dataGridViewCellStyle3;
            beneficiaries_bday_month_table.GridColor = Color.FromArgb(231, 229, 255);
            beneficiaries_bday_month_table.Location = new Point(12, 60);
            beneficiaries_bday_month_table.Name = "beneficiaries_bday_month_table";
            beneficiaries_bday_month_table.ReadOnly = true;
            beneficiaries_bday_month_table.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = Color.White;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            beneficiaries_bday_month_table.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            beneficiaries_bday_month_table.RowHeadersVisible = false;
            beneficiaries_bday_month_table.Size = new Size(1173, 378);
            beneficiaries_bday_month_table.TabIndex = 8;
            beneficiaries_bday_month_table.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            beneficiaries_bday_month_table.ThemeStyle.AlternatingRowsStyle.Font = new Font("Segoe UI", 9F);
            beneficiaries_bday_month_table.ThemeStyle.AlternatingRowsStyle.ForeColor = SystemColors.ControlText;
            beneficiaries_bday_month_table.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            beneficiaries_bday_month_table.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            beneficiaries_bday_month_table.ThemeStyle.BackColor = Color.White;
            beneficiaries_bday_month_table.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            beneficiaries_bday_month_table.ThemeStyle.HeaderStyle.Height = 32;
            beneficiaries_bday_month_table.ThemeStyle.ReadOnly = true;
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.BackColor = Color.White;
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.Height = 25;
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            beneficiaries_bday_month_table.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // batch_code_col
            // 
            batch_code_col.HeaderText = "Batch Code";
            batch_code_col.Name = "batch_code_col";
            batch_code_col.ReadOnly = true;
            // 
            // age_col
            // 
            age_col.HeaderText = "Age";
            age_col.Name = "age_col";
            age_col.ReadOnly = true;
            // 
            // birth_date_col
            // 
            birth_date_col.HeaderText = "Birthdate";
            birth_date_col.Name = "birth_date_col";
            birth_date_col.ReadOnly = true;
            // 
            // sex_col
            // 
            sex_col.HeaderText = "Sex";
            sex_col.Name = "sex_col";
            sex_col.ReadOnly = true;
            // 
            // region_col
            // 
            region_col.HeaderText = "Region";
            region_col.Name = "region_col";
            region_col.ReadOnly = true;
            // 
            // province_col
            // 
            province_col.HeaderText = "Province";
            province_col.Name = "province_col";
            province_col.ReadOnly = true;
            // 
            // minicipalities_col
            // 
            minicipalities_col.HeaderText = "Municipalities";
            minicipalities_col.Name = "minicipalities_col";
            minicipalities_col.ReadOnly = true;
            // 
            // barangay_col
            // 
            barangay_col.HeaderText = "Barangay";
            barangay_col.Name = "barangay_col";
            barangay_col.ReadOnly = true;
            // 
            // date_validated_col
            // 
            date_validated_col.HeaderText = "Date Validated";
            date_validated_col.Name = "date_validated_col";
            date_validated_col.ReadOnly = true;
            // 
            // pwd_col
            // 
            pwd_col.HeaderText = "PWD";
            pwd_col.Name = "pwd_col";
            pwd_col.ReadOnly = true;
            // 
            // ip_col
            // 
            ip_col.HeaderText = "IP";
            ip_col.Name = "ip_col";
            ip_col.ReadOnly = true;
            // 
            // closeButton
            // 
            closeButton.Animated = true;
            closeButton.BackColor = Color.Transparent;
            closeButton.BorderColor = Color.Transparent;
            closeButton.CustomizableEdges = customizableEdges1;
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
            closeButton.Location = new Point(1161, 7);
            closeButton.Name = "closeButton";
            closeButton.PressedColor = Color.Transparent;
            closeButton.PressedDepth = 0;
            closeButton.ShadowDecoration.Color = Color.Transparent;
            closeButton.ShadowDecoration.CustomizableEdges = customizableEdges2;
            closeButton.Size = new Size(29, 26);
            closeButton.TabIndex = 9;
            closeButton.Click += closeButton_Click;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 12;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2AnimateWindow1
            // 
            guna2AnimateWindow1.AnimationType = Guna.UI2.WinForms.Guna2AnimateWindow.AnimateWindowType.AW_BLEND;
            guna2AnimateWindow1.Interval = 200;
            guna2AnimateWindow1.TargetForm = this;
            // 
            // BenefeciariesBdayMonth
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1196, 450);
            Controls.Add(closeButton);
            Controls.Add(beneficiaries_bday_month_table);
            Controls.Add(header);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BenefeciariesBdayMonth";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BenefeciariesBdayMonth";
            ((System.ComponentModel.ISupportInitialize)beneficiaries_bday_month_table).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Label header;
        private Guna.UI2.WinForms.Guna2DataGridView beneficiaries_bday_month_table;
        private DataGridViewTextBoxColumn batch_code_col;
        private DataGridViewTextBoxColumn age_col;
        private DataGridViewTextBoxColumn birth_date_col;
        private DataGridViewTextBoxColumn sex_col;
        private DataGridViewTextBoxColumn region_col;
        private DataGridViewTextBoxColumn province_col;
        private DataGridViewTextBoxColumn minicipalities_col;
        private DataGridViewTextBoxColumn barangay_col;
        private DataGridViewTextBoxColumn date_validated_col;
        private DataGridViewTextBoxColumn pwd_col;
        private DataGridViewTextBoxColumn ip_col;
        private Guna.UI2.WinForms.Guna2Button closeButton;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
    }
}