using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.Charts.Interfaces;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace NCSC
{
    public partial class Dashboard : Form
    {

        private List<Guna2Button> sidebarButtons;
        private string _userRole;
        // Constructor for runtime, accepts user role
        public Dashboard(string userRole)
        {
            _userRole = userRole;
            InitializeComponent();
            InitializeBeneficiariesContextMenu();
            beneficiaries_table.MouseDown += beneficiaries_table_MouseDown;
            sidebarButtons = new List<Guna2Button> { dashboardButton, beneficiariesButton, messageButton, graphReportButton, aboutButton };
            SelectSidebarButton(dashboardButton);
            UpdateBeneficiaryCounts();
            guna2Panel13.MouseClick += SummaryGraph_MouseClick;
            randomizer();

            // Make the form fullscreen by default (only at runtime, not in designer)
            if (!this.DesignMode)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            SetAccountsVisibilityByRole(_userRole);

            // Populate province filter with 'All' + provinces
            beneficiaries_province_filter.Items.Clear();
            beneficiaries_province_filter.Items.Add("All");
            foreach (var province in provinceMunicipalities.Keys)
            {
                beneficiaries_province_filter.Items.Add(province);
            }
            beneficiaries_province_filter.SelectedIndex = 0;

            // Set up event handlers
            beneficiaries_province_filter.SelectedIndexChanged += beneficiaries_province_filter_SelectedIndexChanged;
            beneficiaries_municipality_filter.SelectedIndexChanged += beneficiaries_municipality_filter_SelectedIndexChanged;
            beneficiaries_table_filter.SelectedIndexChanged += beneficiaries_table_filter_SelectedIndexChanged;
            
            // Set up message button event handlers
            msg_mailing_list_button.Click += msg_mailing_list_button_Click;
            msg_message_button.Click += msg_message_button_Click;

            // Initialize historical report filters
            InitializeHistoricalReportFilters();
            
            // Initialize batch graph filters
            InitializeBatchGraphFilters();

            // Set initial state for message buttons (default to mailing list)
            ToggleMessageButtons(msg_mailing_list_button);

            // Populate municipality filter with 'All' by default
            beneficiaries_municipality_filter.Items.Clear();
            beneficiaries_municipality_filter.Items.Add("All");
            beneficiaries_municipality_filter.SelectedIndex = 0;

            // Populate status filter without 'All' option
            beneficiaries_table_filter.Items.Clear();
            beneficiaries_table_filter.Items.Add("Total Endorse from LGUs");
            beneficiaries_table_filter.Items.Add("Assessed");
            beneficiaries_table_filter.Items.Add("Schedule Validation");
            beneficiaries_table_filter.Items.Add("Total Validated");
            beneficiaries_table_filter.Items.Add("Total Endorsed to NCSC CO");
            beneficiaries_table_filter.Items.Add("Total Cleaned list from NCSC CO");
            beneficiaries_table_filter.Items.Add("Scheduled payout");
            beneficiaries_table_filter.Items.Add("No. of applicants received the Cash Gift");
            beneficiaries_table_filter.SelectedIndex = 0;
        }

        // Default constructor for designer compatibility
        public Dashboard() : this("user") { }

        // Allow toggling fullscreen mode with F11
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F11)
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
                    this.TopMost = false;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.TopMost = true;
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            await LoadBeneficiariesFromFirebase();
            
            // Add sample beneficiary data on dashboard load
            try
            {
                await FirebaseHelper.PushSampleBeneficiaryAsync();
                // Reload beneficiaries to include the new sample data
                await LoadBeneficiariesFromFirebase();
            }
            catch (Exception ex)
            {
                // Handle any errors silently to avoid disrupting the dashboard load
                Console.WriteLine($"Error adding sample beneficiary: {ex.Message}");
            }
            
            UpdateBeneficiaryCounts();
            UpdateBirthdaySummaryGraph();
            UpdateMilestoneBirthdayChartByMonth();
            await LoadProvincialAccountsAsync();

            file_history_table.Rows.Clear(); // Clear existing rows if needed

            file_history_table.Rows.Add("batch_zds_01.csv", "Zamboanga del Sur", "Pagadian City", "01/07/2025");
            file_history_table.Rows.Add("batch_zc_02.csv", "Zamboanga City", "District 1", "03/07/2025");
            file_history_table.Rows.Add("batch_zdn_03.csv", "Zamboanga del Norte", "Dipolog", "05/07/2025");
            file_history_table.Rows.Add("batch_isabela_04.csv", "Isabela City", "Binuangan", "06/07/2025");
            file_history_table.Rows.Add("batch_zs_05.csv", "Zamboanga Sibugay", "Imelda", "07/07/2025");
            file_history_table.Rows.Add("batch_zc_06.csv", "Zamboanga City", "District 2", "08/07/2025");
            file_history_table.Rows.Add("batch_zds_07.csv", "Zamboanga del Sur", "Dumingag", "09/07/2025");
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

        private void SelectSidebarButton(Guna.UI2.WinForms.Guna2Button selectedButton)
        {
            // List of buttons — add accountsButton at the end (or wherever it fits logically)
            var buttons = new List<Guna.UI2.WinForms.Guna2Button>
        {
            dashboardButton,
            beneficiariesButton,
            messageButton,
            graphReportButton,
            aboutButton,
            accounts_button
        };

            // Corresponding panels — must match the order above
            var panels = new List<Panel>
        {
            dashboardPanel,
            beneficiariesPanel,
            messagePanel,
            graphReportPanel,
            aboutPanel,
            manage_accounts_panel
        };

            for (int i = 0; i < buttons.Count; i++)
            {
                bool isSelected = buttons[i] == selectedButton;

                // Update button visual state
                buttons[i].FillColor = isSelected ? Color.White : Color.Transparent;
                buttons[i].ForeColor = isSelected ? Color.Black : Color.White;

                // Show/hide panel
                panels[i].Visible = isSelected;
            }
        }



        private void dashboardButton_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(dashboardButton);
        }

        private void beneficiariesButton_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(beneficiariesButton);
        }

        private void messageButton_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(messageButton);
        }

        private void graphReportButton_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(graphReportButton);
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(aboutButton);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dashboardPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void randomizer()
        {
            // Only one dataset is used for birthday distribution
            GunaLineDataset birthdayDataset = new GunaLineDataset
            {
                Label = "Birthdays per Month",
                BorderWidth = 3,
                PointRadius = 4
            };

            // Initialize month count dictionary
            Dictionary<string, int> monthCounts = new Dictionary<string, int>();
            string[] months = {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

            foreach (string month in months)
            {
                monthCounts[month] = 0;
            }

            // Count how many birthdays per month
            foreach (DataGridViewRow row in beneficiaries_table.Rows)
            {
                if (row.IsNewRow) continue;

                string birthDateStr = row.Cells["birth_date_col"].Value?.ToString();
                if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                {
                    string monthName = birthDate.ToString("MMMM");
                    if (monthCounts.ContainsKey(monthName))
                    {
                        monthCounts[monthName]++;
                    }
                }
            }

            // Assign counts to dataset
            foreach (var month in months)
            {
                birthdayDataset.DataPoints.Add(month, monthCounts[month]);
            }

            // Optionally clear and add to chart (depends on your chart setup)
            summaryGraph.Datasets.Clear(); // replace `gunaChart1` with your chart instance name
            summaryGraph.Datasets.Add(birthdayDataset);
            summaryGraph.Update();
        }



        // beneficiaries table

        // MESSAGE
        private void msg_immediately_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (msg_immediately_checkbox.Checked)
            {
                msg_start_at_checkbox.Checked = false;
            }
        }

        private void msg_start_at_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (msg_start_at_checkbox.Checked)
            {
                msg_immediately_checkbox.Checked = false;
            }
        }

        private void msg_mailing_list_button_Click(object sender, EventArgs e)
        {
            ToggleMessageButtons(msg_mailing_list_button);
        }

        private void msg_message_button_Click(object sender, EventArgs e)
        {
            ToggleMessageButtons(msg_message_button);
        }

        private void ToggleMessageButtons(Guna.UI2.WinForms.Guna2Button selectedButton)
        {
            // Reset both buttons to default state
            msg_mailing_list_button.FillColor = Color.White;
            msg_mailing_list_button.ForeColor = SystemColors.ControlText;
            msg_mailing_list_button.BorderColor = Color.Transparent;
            msg_message_button.FillColor = Color.White;
            msg_message_button.ForeColor = SystemColors.ControlText;
            msg_message_button.BorderColor = Color.Transparent;

            // Set selected button to highlighted state with custom colors
            selectedButton.FillColor = Color.FromArgb(158, 188, 138); // Light green fill
            selectedButton.ForeColor = Color.FromArgb(22, 97, 14); // Dark green text
            selectedButton.BorderColor = Color.FromArgb(22, 97, 14); // Dark green border

            // Show/hide panels based on selection
            if (selectedButton == msg_mailing_list_button)
            {
                msg_mailing_list_panel.Visible = true;
                msg_message_panel.Visible = false;
            }
            else if (selectedButton == msg_message_button)
            {
                msg_mailing_list_panel.Visible = false;
                msg_message_panel.Visible = true;
            }
        }


        private void UpdateBeneficiaryCounts()
        {
            int male = 0, female = 0, pwd = 0, ip = 0;
            int zds = 0, zc = 0, zdn = 0, isabela = 0, zs = 0;

            foreach (DataGridViewRow row in beneficiaries_table.Rows)
            {
                if (row.IsNewRow) continue;

                string sex = row.Cells["sex_col"].Value?.ToString() ?? "";
                string province = row.Cells["province_col"].Value?.ToString() ?? "";
                string isPwd = row.Cells["pwd_col"].Value?.ToString() ?? "";
                string isIp = row.Cells["ip_col"].Value?.ToString() ?? "";

                if (sex == "Male") male++;
                else if (sex == "Female") female++;

                if (isPwd == "Yes") pwd++;
                if (isIp == "Yes") ip++;

                switch (province)
                {
                    case "Zamboanga del Sur":
                        zds++; break;
                    case "Zamboanga City":
                        zc++; break;
                    case "Zamboanga del Norte":
                        zdn++; break;
                    case "Isabela City":
                        isabela++; break;
                    case "Zamboanga Sibugay":
                        zs++; break;
                }
            }

            // Assign counts to UI labels
            zdsCount.Text = zds.ToString();
            zcCount.Text = zc.ToString();
            zdnCount.Text = zdn.ToString();
            isabelaCount.Text = isabela.ToString();
            zsCount.Text = zs.ToString();

            maleCount.Text = male.ToString();
            femaleCount.Text = female.ToString();
            pwdCount.Text = pwd.ToString();
            ipCount.Text = ip.ToString();

            int total = zds + zc + zdn + isabela + zs;
            totalCount.Text = total.ToString();

            beneficiaries_total_count.Text = beneficiaries_table.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Count()
                .ToString();
        }


        // SHOWING BENEFICIARIES BDAY
        private void UpdateBirthdaySummaryGraph()
        {
            // Prepare dictionary for counting birthdays by month
            Dictionary<string, int> birthdayCounts = new Dictionary<string, int>();
            for (int month = 1; month <= 12; month++)
            {
                string monthName = new DateTime(2000, month, 1).ToString("MMMM");
                birthdayCounts[monthName] = 0;
            }

            // Count birthdays per month from table
            foreach (DataGridViewRow row in beneficiaries_table.Rows)
            {
                if (row.IsNewRow) continue;

                string birthDateStr = row.Cells["birth_date_col"].Value?.ToString();
                if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                {
                    string monthName = birthDate.ToString("MMMM");
                    if (birthdayCounts.ContainsKey(monthName))
                        birthdayCounts[monthName]++;
                }
            }

            // Sort dictionary by month number (Jan to Dec)
            var sortedMonthCounts = birthdayCounts.OrderBy(m => DateTime.ParseExact(m.Key, "MMMM", null).Month);

            // Clear old chart data
            summary_graph_bday.DataPoints.Clear();
            summaryGraph.Datasets.Clear();

            // Add values to the chart (all integers)
            foreach (var entry in sortedMonthCounts)
            {
                summary_graph_bday.DataPoints.Add(new Guna.Charts.WinForms.LPoint(entry.Key, entry.Value));
            }

            summaryGraph.Datasets.Add(summary_graph_bday);
            summaryGraph.Update();
        }


        private void UpdateMilestoneBirthdayChartByMonth()
        {
            // Clear datasets
            budgetChart.Datasets.Clear();
            bday80.DataPoints.Clear();
            bday85.DataPoints.Clear();
            bday90.DataPoints.Clear();
            bday95.DataPoints.Clear();
            bday100.DataPoints.Clear();

            // Prepare 12-month counters for each milestone
            int[] count80 = new int[12];
            int[] count85 = new int[12];
            int[] count90 = new int[12];
            int[] count95 = new int[12];
            int[] count100 = new int[12];

            int currentYear = DateTime.Now.Year;

            foreach (DataGridViewRow row in beneficiaries_table.Rows)
            {
                if (row.IsNewRow) continue;

                string birthDateStr = row.Cells["birth_date_col"].Value?.ToString();

                if (DateTime.TryParseExact(birthDateStr, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime birthDate))
                {
                    int birthYear = birthDate.Year;
                    int birthMonth = birthDate.Month;

                    // Determine the age the person will turn THIS YEAR
                    int turningThisYear = currentYear - birthYear;

                    switch (turningThisYear)
                    {
                        case 80:
                            count80[birthMonth - 1]++;
                            break;
                        case 85:
                            count85[birthMonth - 1]++;
                            break;
                        case 90:
                            count90[birthMonth - 1]++;
                            break;
                        case 95:
                            count95[birthMonth - 1]++;
                            break;
                        case 100:
                            count100[birthMonth - 1]++;
                            break;
                    }
                }
            }

            // Add month-wise data points
            for (int i = 0; i < 12; i++)
            {
                string monthName = new DateTime(currentYear, i + 1, 1).ToString("MMMM");

                bday80.DataPoints.Add(monthName, count80[i]);
                bday85.DataPoints.Add(monthName, count85[i]);
                bday90.DataPoints.Add(monthName, count90[i]);
                bday95.DataPoints.Add(monthName, count95[i]);
                bday100.DataPoints.Add(monthName, count100[i]);
            }

            // Add all datasets to the chart
            budgetChart.Datasets.Add(bday80);
            budgetChart.Datasets.Add(bday85);
            budgetChart.Datasets.Add(bday90);
            budgetChart.Datasets.Add(bday95);
            budgetChart.Datasets.Add(bday100);

            budgetChart.Update();
        }





        private void SummaryGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (summaryGraph.Datasets.Count == 0)
                return;

            var dataset = summaryGraph.Datasets[0] as GunaLineDataset;
            if (dataset == null || dataset.DataPoints.Count == 0)
            {
                MessageBox.Show("Dataset is not a GunaLineDataset or is empty.");
                return;
            }

            // Try to use GetPointIndexAt if available
            var method = summaryGraph.GetType().GetMethod("GetPointIndexAt");
            if (method != null)
            {
                var hitInfo = method.Invoke(summaryGraph, new object[] { e.X, e.Y });
                if (hitInfo != null)
                {
                    var datasetIndexProp = hitInfo.GetType().GetProperty("DatasetIndex");
                    var pointIndexProp = hitInfo.GetType().GetProperty("PointIndex");
                    int datasetIndex = (int)datasetIndexProp.GetValue(hitInfo);
                    int pointIndex = (int)pointIndexProp.GetValue(hitInfo);
                    if (datasetIndex >= 0 && pointIndex >= 0)
                    {
                        var ds = summaryGraph.Datasets[datasetIndex] as GunaLineDataset;
                        if (ds != null && pointIndex < ds.DataPoints.Count)
                        {
                            var month = ds.DataPoints[pointIndex].Label;
                            var rows = beneficiaries_table.Rows
                                .Cast<DataGridViewRow>()
                                .Where(r => !r.IsNewRow)
                                .Where(r =>
                                {
                                    var birthDateStr = r.Cells["birth_date_col"].Value?.ToString();
                                    if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                                    {
                                        return birthDate.ToString("MMMM") == month;
                                    }
                                    return false;
                                })
                                .ToList();
                            ShowBeneficiariesForMonth(month, rows);
                        }
                        return;
                    }
                }
            }

            // Improved manual hit test: map Y value to pixel
            int pointCount = dataset.DataPoints.Count;
            int leftMargin = 50;
            int rightMargin = 20;
            int plotWidth = summaryGraph.Width - leftMargin - rightMargin;
            if (plotWidth <= 0) return;
            float pointSpacing = (pointCount > 1) ? (float)plotWidth / (pointCount - 1) : 0;

            float chartTopMargin = 40;
            float chartBottomMargin = 40;
            float chartHeight = summaryGraph.Height - chartTopMargin - chartBottomMargin;
            float minY = 0;
            float maxY = dataset.DataPoints.Cast<Guna.Charts.WinForms.LPoint>().Max(p => (float)p.Y);
            if (Math.Abs(maxY - minY) < 0.0001f) maxY = minY + 1; // avoid div by zero

            int clickedIndex = -1;
            float minDist = float.MaxValue;
            StringBuilder debugInfo = new StringBuilder();
            debugInfo.AppendLine($"Click X: {e.X}, Y: {e.Y}");
            for (int i = 0; i < pointCount; i++)
            {
                float pointX = leftMargin + (i * pointSpacing);
                float value = (float)dataset.DataPoints[i].Y;
                float pointY = chartTopMargin + chartHeight * (1 - (value - minY) / (maxY - minY + 0.0001f));
                float dist = (float)Math.Sqrt(Math.Pow(e.X - pointX, 2) + Math.Pow(e.Y - pointY, 2));
                debugInfo.AppendLine($"Point {i}: X={pointX:F2}, Y={pointY:F2}, Value={value}, Dist={dist:F2}");
                if (dist < minDist)
                {
                    minDist = dist;
                    clickedIndex = i;
                }
            }
            debugInfo.AppendLine($"Closest index: {clickedIndex}, minDist: {minDist:F2}");
            MessageBox.Show(debugInfo.ToString(), "Chart Click Debug");
            if (clickedIndex != -1 && minDist < 12)
            {
                var month = dataset.DataPoints[clickedIndex].Label;
                var rows = beneficiaries_table.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => !r.IsNewRow)
                    .Where(r =>
                    {
                        var birthDateStr = r.Cells["birth_date_col"].Value?.ToString();
                        if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                        {
                            return birthDate.ToString("MMMM") == month;
                        }
                        return false;
                    })
                    .ToList();
                ShowBeneficiariesForMonth(month, rows);
            }
        }

        private void ShowBeneficiariesForMonth(string month, List<DataGridViewRow> rows)
        {
            // Only pass batch codes to the BenefeciariesBdayMonth form, do not display a table here
            var batchCodes = rows
                .Select(r => r.Cells["batch_code_col"].Value?.ToString())
                .Where(code => !string.IsNullOrEmpty(code))
                .ToList();
            if (batchCodes.Any())
            {
                var popup = new BenefeciariesBdayMonth(month, batchCodes);
                popup.ShowDialog();
            }
            else
            {
                MessageBox.Show($"No beneficiaries found for {month}.", "Info");
            }
        }

        private void graphReportPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void accounts_button_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(accounts_button);
        }

        // Show or hide accounts button and panel based on user role
        private void SetAccountsVisibilityByRole(string userRole)
        {
            // MessageBox.Show($"SetAccountsVisibilityByRole called with: '{userRole}'");
            bool isSuperAdmin = userRole != null && userRole.Trim().ToLower() == "super_admin";
            accounts_button.Visible = isSuperAdmin;
            // manage_accounts_panel.Visible = isSuperAdmin; // Removed to fix panel visibility logic
        }

        // Store all beneficiaries for filtering
        private List<Beneficiary> allBeneficiaries = new List<Beneficiary>();

        private async Task LoadBeneficiariesFromFirebase()
        {
            beneficiaries_table.Rows.Clear();
            beneficiaries_table.Columns.Clear();

            // Define columns
            beneficiaries_table.Columns.Add("batch_code_col", "Batch Code");
            beneficiaries_table.Columns.Add("age_col", "Age");
            beneficiaries_table.Columns.Add("birth_date_col", "Birth Date");
            beneficiaries_table.Columns.Add("sex_col", "Sex");
            beneficiaries_table.Columns.Add("region_col", "Region");
            beneficiaries_table.Columns.Add("province_col", "Province");
            beneficiaries_table.Columns.Add("municipalities_col", "Municipality");
            beneficiaries_table.Columns.Add("barangay_col", "Barangay");
            beneficiaries_table.Columns.Add("date_validated_col", "Date Validated");
            beneficiaries_table.Columns.Add("pwd_col", "PWD");
            beneficiaries_table.Columns.Add("ip_col", "IP");

            var beneficiaries = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");

            allBeneficiaries.Clear();
            if (beneficiaries != null)
            {
                foreach (var entry in beneficiaries.Values)
                {
                    allBeneficiaries.Add(entry);
                }
            }

            ApplyBeneficiaryFilters();
            UpdateBeneficiaryCounts();
        }

        private void ApplyBeneficiaryFilters()
        {
            beneficiaries_table.Rows.Clear();
            string selectedProvince = beneficiaries_province_filter.SelectedItem?.ToString();
            string selectedMunicipality = beneficiaries_municipality_filter.SelectedItem?.ToString();
            string selectedStatus = beneficiaries_table_filter.SelectedItem?.ToString();

            var filtered = allBeneficiaries.AsEnumerable();
            
            // Filter by province
            if (!string.IsNullOrEmpty(selectedProvince) && selectedProvince != "All" && provinceMunicipalities.ContainsKey(selectedProvince))
            {
                filtered = filtered.Where(b => b.province == selectedProvince);
            }
            
            // Filter by municipality
            if (!string.IsNullOrEmpty(selectedMunicipality) && selectedMunicipality != "All" && selectedMunicipality != "Municipality")
            {
                filtered = filtered.Where(b => b.municipality == selectedMunicipality);
            }
            
            // Filter by status
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                switch (selectedStatus)
                {
                    case "Total Endorse from LGUs":
                        filtered = filtered.Where(b => b.TotalEndorseFromLGUs);
                        break;
                    case "Assessed":
                        filtered = filtered.Where(b => b.Assessed);
                        break;
                    case "Schedule Validation":
                        filtered = filtered.Where(b => b.ScheduleValidation);
                        break;
                    case "Total Validated":
                        filtered = filtered.Where(b => b.TotalValidated);
                        break;
                    case "Total Endorsed to NCSC CO":
                        filtered = filtered.Where(b => b.TotalEndorsedToNCSCO);
                        break;
                    case "Total Cleaned list from NCSC CO":
                        filtered = filtered.Where(b => b.TotalCleanedListFromNCSCO);
                        break;
                    case "Scheduled payout":
                        filtered = filtered.Where(b => b.ScheduledPayout);
                        break;
                    case "No. of applicants received the Cash Gift":
                        filtered = filtered.Where(b => b.NumberOfApplicantsReceivedCashGift);
                        break;
                }
            }

            foreach (var entry in filtered)
            {
                beneficiaries_table.Rows.Add(
                    entry.batch_code,
                    entry.age,
                    entry.birth_date,
                    entry.sex,
                    entry.region,
                    entry.province,
                    entry.municipality,
                    entry.barangay,
                    entry.date_validated,
                    entry.pwd,
                    entry.ip
                );
            }
        }

        private void upload_file_button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file to upload";
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|CSV Files|*.csv|All Files|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(selectedFilePath);

                    // Example: display file details in your guna DataGridView (file_history_table)
                    string province = "Zamboanga del Sur"; // Replace with actual logic if needed
                    string municipality = "Municipal 1";   // Replace with actual logic if needed
                    string dateSubmission = DateTime.Now.ToString("dd/MM/yyyy");

                    file_history_table.Rows.Add(fileName, province, municipality, dateSubmission);

                    MessageBox.Show("File uploaded successfully:\n" + fileName, "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // You can optionally store it or process the file here
                    // e.g., read Excel/CSV content, upload to database, etc.
                }
            }
        }

        private void manage_accounts_panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private async Task LoadProvincialAccountsAsync()
        {
            try
            {
                var accounts = await FirebaseHelper.GetDataAsync<Dictionary<string, ProvincialAccounts>>("provincial_accounts");

                if (accounts == null)
                {
                    MessageBox.Show("No accounts found.");
                    return;
                }

                accounts_table.Rows.Clear();

                foreach (var account in accounts)
                {
                    string id = account.Key;
                    var acc = account.Value;

                    if (acc != null)
                    {
                        accounts_table.Rows.Add(
                            id,
                            acc.username ?? "N/A",
                            acc.province ?? "N/A",
                            acc.municipality ?? "N/A",
                            acc.status ?? "N/A"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message);
            }
        }

        private async void approve_button_Click(object sender, EventArgs e)
        {
            if (accounts_table.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an account to approve.");
                return;
            }

            // Get selected row
            var selectedRow = accounts_table.SelectedRows[0];
            string accountId = selectedRow.Cells["account_id"].Value?.ToString();

            if (string.IsNullOrEmpty(accountId))
            {
                MessageBox.Show("Invalid account ID.");
                return;
            }

            try
            {
                // Fetch the existing account data
                var account = await FirebaseHelper.GetDataAsync<ProvincialAccounts>($"provincial_accounts/{accountId}");

                if (account == null)
                {
                    MessageBox.Show("Account not found.");
                    return;
                }

                // Update the status
                account.status = "approved";

                // Push updated data back to Firebase
                await FirebaseHelper.SetDataAsync($"provincial_accounts/{accountId}", account);

                MessageBox.Show("Account approved successfully!");

                // Refresh the table
                await LoadProvincialAccountsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving account: " + ex.Message);
            }
        }

        // Province to Municipalities mapping
        private readonly Dictionary<string, List<string>> provinceMunicipalities = new Dictionary<string, List<string>>
        {
            { "Zamboanga del Sur", new List<string> { "Aurora", "Bayog", "Dimataling", "Dinas", "Dumalinao", "Dumingag", "Guipos", "Josefina", "Kumalarang", "Labangan", "Lakewood", "Lapuyan", "Mahayag", "Margosatubig", "Midsalip", "Molave", "Pagadian City", "Pitogo", "Ramon Magsaysay", "San Miguel", "San Pablo", "Sominot", "Tabina", "Tambulig", "Tigbao", "Tukuran", "Vincenzo A. Sagun" } },
            { "Zamboanga del Norte", new List<string> { "Baliguian", "Godod", "Gutalac", "Jose Dalman", "Kalawit", "Katipunan", "La Libertad", "Labason", "Leon B. Postigo", "Liloy", "Manukan", "Mutia", "Piñan", "Polanco", "President Manuel A. Roxas", "Rizal", "Salug", "Sergio Osmeña Sr.", "Siayan", "Sibuco", "Sibutad", "Sindangan", "Siocon", "Sirawai", "Tampilisan", "Dipolog City", "Dapitan City" } },
            { "Zamboanga Sibugay", new List<string> { "Alicia", "Buug", "Diplahan", "Imelda", "Ipil (capital)", "Kabasalan", "Mabuhay", "Malangas", "Naga", "Olutanga", "Payao", "Roseller Lim", "Siay", "Talusan", "Titay", "Tungawan" } },
            { "Zamboanga City", new List<string> { "Zamboanga City" } },
            { "Isabela City", new List<string> { "Isabela City" } }
        };

        // Update filter when province or municipality changes
        private void beneficiaries_province_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            beneficiaries_municipality_filter.Items.Clear();
            string selectedProvince = beneficiaries_province_filter.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProvince) && provinceMunicipalities.ContainsKey(selectedProvince))
            {
                // For Zamboanga City and Isabela City, only show the city
                if (selectedProvince == "Zamboanga City" || selectedProvince == "Isabela City")
                {
                    beneficiaries_municipality_filter.Items.Add(provinceMunicipalities[selectedProvince][0]);
                    beneficiaries_municipality_filter.SelectedIndex = 0;
                }
                else
                {
                    beneficiaries_municipality_filter.Items.Add("All");
                    beneficiaries_municipality_filter.Items.AddRange(provinceMunicipalities[selectedProvince].ToArray());
                    beneficiaries_municipality_filter.SelectedIndex = 0;
                }
            }
            else
            {
                // If 'All' or invalid, show 'All' only
                beneficiaries_municipality_filter.Items.Add("All");
                beneficiaries_municipality_filter.SelectedIndex = 0;
            }
            ApplyBeneficiaryFilters();
        }

        private void beneficiaries_municipality_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyBeneficiaryFilters();
        }

        private void beneficiaries_table_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyBeneficiaryFilters();
        }

        // Historical Report Filters
        private void InitializeHistoricalReportFilters()
        {
            // Populate historical province filter with 'All' + provinces (reusing existing mapping)
            graph_report_historical_province_filter.Items.Clear();
            graph_report_historical_province_filter.Items.Add("All");
            foreach (var province in provinceMunicipalities.Keys)
            {
                graph_report_historical_province_filter.Items.Add(province);
            }
            graph_report_historical_province_filter.SelectedIndex = 0;

            // Populate historical municipality filter with 'All' by default
            graph_report_historical_municipality_filter.Items.Clear();
            graph_report_historical_municipality_filter.Items.Add("All");
            graph_report_historical_municipality_filter.SelectedIndex = 0;

            // Set up event handlers for historical report filters
            graph_report_historical_province_filter.SelectedIndexChanged += graph_report_historical_province_filter_SelectedIndexChanged;
            graph_report_historical_municipality_filter.SelectedIndexChanged += graph_report_historical_municipality_filter_SelectedIndexChanged;
        }

        private void graph_report_historical_province_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            graph_report_historical_municipality_filter.Items.Clear();
            string selectedProvince = graph_report_historical_province_filter.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProvince) && provinceMunicipalities.ContainsKey(selectedProvince))
            {
                // For Zamboanga City and Isabela City, only show the city
                if (selectedProvince == "Zamboanga City" || selectedProvince == "Isabela City")
                {
                    graph_report_historical_municipality_filter.Items.Add(provinceMunicipalities[selectedProvince][0]);
                    graph_report_historical_municipality_filter.SelectedIndex = 0;
                }
                else
                {
                    graph_report_historical_municipality_filter.Items.Add("All");
                    graph_report_historical_municipality_filter.Items.AddRange(provinceMunicipalities[selectedProvince].ToArray());
                    graph_report_historical_municipality_filter.SelectedIndex = 0;
                }
            }
            else
            {
                // If 'All' or invalid, show 'All' only
                graph_report_historical_municipality_filter.Items.Add("All");
                graph_report_historical_municipality_filter.SelectedIndex = 0;
            }
            // TODO: Add method to update historical report charts based on filter selection
            UpdateHistoricalReportCharts();
        }

        private void graph_report_historical_municipality_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: Add method to update historical report charts based on filter selection
            UpdateHistoricalReportCharts();
        }

        private void UpdateHistoricalReportCharts()
        {
            // This method will be implemented to update the historical report charts
            // based on the selected province and municipality filters
            // For now, it's a placeholder that can be expanded later
        }

        // Batch Graph Filters
        private void InitializeBatchGraphFilters()
        {
            // Populate batch graph province filter with 'All' + provinces (reusing existing mapping)
            graph_report_batch_graph_province.Items.Clear();
            graph_report_batch_graph_province.Items.Add("All");
            foreach (var province in provinceMunicipalities.Keys)
            {
                graph_report_batch_graph_province.Items.Add(province);
            }
            graph_report_batch_graph_province.SelectedIndex = 0;

            // Populate batch graph municipality filter with 'All' by default
            graph_report_batch_graph_municipality.Items.Clear();
            graph_report_batch_graph_municipality.Items.Add("All");
            graph_report_batch_graph_municipality.SelectedIndex = 0;

            // Set up event handlers for batch graph filters
            graph_report_batch_graph_province.SelectedIndexChanged += graph_report_batch_graph_province_SelectedIndexChanged;
            graph_report_batch_graph_municipality.SelectedIndexChanged += graph_report_batch_graph_municipality_SelectedIndexChanged;
        }

        private void graph_report_batch_graph_province_SelectedIndexChanged(object sender, EventArgs e)
        {
            graph_report_batch_graph_municipality.Items.Clear();
            string selectedProvince = graph_report_batch_graph_province.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProvince) && provinceMunicipalities.ContainsKey(selectedProvince))
            {
                // For Zamboanga City and Isabela City, only show the city
                if (selectedProvince == "Zamboanga City" || selectedProvince == "Isabela City")
                {
                    graph_report_batch_graph_municipality.Items.Add(provinceMunicipalities[selectedProvince][0]);
                    graph_report_batch_graph_municipality.SelectedIndex = 0;
                }
                else
                {
                    graph_report_batch_graph_municipality.Items.Add("All");
                    graph_report_batch_graph_municipality.Items.AddRange(provinceMunicipalities[selectedProvince].ToArray());
                    graph_report_batch_graph_municipality.SelectedIndex = 0;
                }
            }
            else
            {
                // If 'All' or invalid, show 'All' only
                graph_report_batch_graph_municipality.Items.Add("All");
                graph_report_batch_graph_municipality.SelectedIndex = 0;
            }
            // TODO: Add method to update batch graph charts based on filter selection
            UpdateBatchGraphCharts();
        }

        private void graph_report_batch_graph_municipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: Add method to update batch graph charts based on filter selection
            UpdateBatchGraphCharts();
        }

        private void UpdateBatchGraphCharts()
        {
            // This method will be implemented to update the batch graph charts
            // based on the selected province and municipality filters
            // For now, it's a placeholder that can be expanded later
        }

        private void beneficiaries_table_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell content click if needed
            // This method is required by the designer but can be empty if not needed
        }

        private void beneficiaries_table_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = beneficiaries_table.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            var popup = new Benefeciaries_popup();

            // Set text fields
            popup.Controls["benefeciaries_batch_code"].Text = row.Cells["batch_code_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_region"].Text = row.Cells["region_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_age"].Text = row.Cells["age_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_province"].Text = row.Cells["province_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_municipality"].Text = row.Cells["municipalities_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_sex"].Text = row.Cells["sex_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_barangay"].Text = row.Cells["barangay_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_pwd"].Text = row.Cells["pwd_col"].Value?.ToString() ?? "";
            popup.Controls["benefeciaries_ip"].Text = row.Cells["ip_col"].Value?.ToString() ?? "";

            // Set birthdate
            var birthDateStr = row.Cells["birth_date_col"].Value?.ToString();
            if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                ((Guna.UI2.WinForms.Guna2DateTimePicker)popup.Controls["benefeciaries_birthdate"]).Value = birthDate;

            // Set date validated
            var dateValidatedStr = row.Cells["date_validated_col"].Value?.ToString();
            if (DateTime.TryParse(dateValidatedStr, out DateTime dateValidated))
                ((Guna.UI2.WinForms.Guna2DateTimePicker)popup.Controls["benefeciaries_date_validated"]).Value = dateValidated;

            // Date registered is not in the table, so leave as default

            popup.ShowDialog();
        }

        private void jan_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("January");
        }
        private void feb_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("February");
        }
        private void mar_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("March");
        }
        private void apr_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("April");
        }
        private void may_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("May");
        }
        private void jun_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("June");
        }
        private void jul_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("July");
        }
        private void aug_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("August");
        }
        private void sep_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("September");
        }
        private void oct_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("October");
        }
        private void nov_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("November");
        }
        private void dec_bday_button_Click(object sender, EventArgs e)
        {
            ShowBeneficiariesForMonthByButton("December");
        }

        private void ShowBeneficiariesForMonthByButton(string month)
        {
            var rows = beneficiaries_table.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r =>
                {
                    var birthDateStr = r.Cells["birth_date_col"].Value?.ToString();
                    if (DateTime.TryParse(birthDateStr, out DateTime birthDate))
                    {
                        return birthDate.ToString("MMMM") == month;
                    }
                    return false;
                })
                .ToList();

            var batchCodes = rows
                .Select(r => r.Cells["batch_code_col"].Value?.ToString())
                .Where(code => !string.IsNullOrEmpty(code))
                .ToList();

            if (batchCodes.Any())
            {
                var popup = new BenefeciariesBdayMonth(month, batchCodes);
                popup.ShowDialog();
            }
            else
            {
                MessageBox.Show($"No beneficiaries found for {month}.", "Info");
            }
        }

        private void budget_filter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Context menu for beneficiaries_table
        private ContextMenuStrip beneficiariesContextMenu;
        private ToolTip beneficiariesToolTip;

        private void InitializeBeneficiariesContextMenu()
        {
            beneficiariesContextMenu = new ContextMenuStrip();
            
            // Add menu items with event handlers
            var assessedItem = beneficiariesContextMenu.Items.Add("Add to: Assessed");
            assessedItem.Click += (s, e) => UpdateBeneficiaryStatus("Assessed");
            
            var scheduleValidationItem = beneficiariesContextMenu.Items.Add("Add to: Schedule Validation");
            scheduleValidationItem.Click += (s, e) => UpdateBeneficiaryStatus("ScheduleValidation");
            
            var totalValidatedItem = beneficiariesContextMenu.Items.Add("Add to: Total Validated");
            totalValidatedItem.Click += (s, e) => UpdateBeneficiaryStatus("TotalValidated");
            
            var totalEndorsedItem = beneficiariesContextMenu.Items.Add("Add to: Total Endorsed to NCSC CO");
            totalEndorsedItem.Click += (s, e) => UpdateBeneficiaryStatus("TotalEndorsedToNCSCO");
            
            var totalCleanedItem = beneficiariesContextMenu.Items.Add("Add to: Total Cleaned list from NCSC CO");
            totalCleanedItem.Click += (s, e) => UpdateBeneficiaryStatus("TotalCleanedListFromNCSCO");
            
            var scheduledPayoutItem = beneficiariesContextMenu.Items.Add("Add to: Scheduled payout");
            scheduledPayoutItem.Click += (s, e) => UpdateBeneficiaryStatus("ScheduledPayout");
            
            var receivedCashGiftItem = beneficiariesContextMenu.Items.Add("Add to: No. of applicants received the Cash Gift");
            receivedCashGiftItem.Click += (s, e) => UpdateBeneficiaryStatus("NumberOfApplicantsReceivedCashGift");

            beneficiariesContextMenu.Opening += (s, e) =>
            {
                // No-op for now
            };

            beneficiariesToolTip = new ToolTip();
        }

        private async void UpdateBeneficiaryStatus(string statusField)
        {
            if (beneficiaries_table.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a beneficiary to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the selected row
                var selectedRow = beneficiaries_table.SelectedRows[0];
                string batchCode = selectedRow.Cells["batch_code_col"].Value?.ToString();

                if (string.IsNullOrEmpty(batchCode))
                {
                    MessageBox.Show("Invalid batch code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Find the beneficiary in the allBeneficiaries list
                var beneficiary = allBeneficiaries.FirstOrDefault(b => b.batch_code == batchCode);
                if (beneficiary == null)
                {
                    MessageBox.Show("Beneficiary not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the appropriate status field
                switch (statusField)
                {
                    case "Assessed":
                        beneficiary.Assessed = true;
                        break;
                    case "ScheduleValidation":
                        beneficiary.ScheduleValidation = true;
                        break;
                    case "TotalValidated":
                        beneficiary.TotalValidated = true;
                        break;
                    case "TotalEndorsedToNCSCO":
                        beneficiary.TotalEndorsedToNCSCO = true;
                        break;
                    case "TotalCleanedListFromNCSCO":
                        beneficiary.TotalCleanedListFromNCSCO = true;
                        break;
                    case "ScheduledPayout":
                        beneficiary.ScheduledPayout = true;
                        break;
                    case "NumberOfApplicantsReceivedCashGift":
                        beneficiary.NumberOfApplicantsReceivedCashGift = true;
                        break;
                    default:
                        MessageBox.Show("Invalid status field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                // Find the Firebase key for this beneficiary
                var allBeneficiariesFromFirebase = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");
                string beneficiaryKey = null;
                
                if (allBeneficiariesFromFirebase != null)
                {
                    foreach (var entry in allBeneficiariesFromFirebase)
                    {
                        if (entry.Value.batch_code == batchCode)
                        {
                            beneficiaryKey = entry.Key;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(beneficiaryKey))
                {
                    MessageBox.Show("Beneficiary key not found in Firebase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the beneficiary in Firebase
                await FirebaseHelper.SetDataAsync($"beneficiaries/{beneficiaryKey}", beneficiary);

                // Update the local list
                var localBeneficiary = allBeneficiaries.FirstOrDefault(b => b.batch_code == batchCode);
                if (localBeneficiary != null)
                {
                    switch (statusField)
                    {
                        case "Assessed":
                            localBeneficiary.Assessed = true;
                            break;
                        case "ScheduleValidation":
                            localBeneficiary.ScheduleValidation = true;
                            break;
                        case "TotalValidated":
                            localBeneficiary.TotalValidated = true;
                            break;
                        case "TotalEndorsedToNCSCO":
                            localBeneficiary.TotalEndorsedToNCSCO = true;
                            break;
                        case "TotalCleanedListFromNCSCO":
                            localBeneficiary.TotalCleanedListFromNCSCO = true;
                            break;
                        case "ScheduledPayout":
                            localBeneficiary.ScheduledPayout = true;
                            break;
                        case "NumberOfApplicantsReceivedCashGift":
                            localBeneficiary.NumberOfApplicantsReceivedCashGift = true;
                            break;
                    }
                }

                // Refresh the table
                ApplyBeneficiaryFilters();
                UpdateBeneficiaryCounts();

                MessageBox.Show($"Beneficiary {batchCode} has been successfully updated to {statusField}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating beneficiary status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void beneficiaries_table_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = beneficiaries_table.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    // If the right-clicked row is not already selected, select only it
                    if (!beneficiaries_table.Rows[hit.RowIndex].Selected)
                    {
                        beneficiaries_table.ClearSelection();
                        beneficiaries_table.Rows[hit.RowIndex].Selected = true;
                    }
                    // Otherwise, keep the current selection (for multi-select)
                    beneficiariesContextMenu.Show(beneficiaries_table, e.Location);
                }
            }
        }
    }
}
