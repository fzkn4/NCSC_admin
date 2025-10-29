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
using OfficeOpenXml;

namespace NCSC
{
    public partial class Dashboard : Form
    {

        private List<Guna2Button> sidebarButtons;
        private string _userRole;
        private string _username;
        // Constructor for runtime, accepts user role and username
        public Dashboard(string userRole, string username = "")
        {
            _userRole = userRole;
            _username = username;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            
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

            // Populate status filter with 'All' option first
            beneficiaries_table_filter.Items.Clear();
            beneficiaries_table_filter.Items.Add("All");
            beneficiaries_table_filter.Items.Add("Total Endorse from LGUs");
            beneficiaries_table_filter.Items.Add("Assessed");
            beneficiaries_table_filter.Items.Add("Total Validated");
            beneficiaries_table_filter.Items.Add("Total Endorsed to NCSC CO");
            beneficiaries_table_filter.Items.Add("Total Cleaned list from NCSC CO");
            beneficiaries_table_filter.Items.Add("Scheduled payout");
            beneficiaries_table_filter.Items.Add("No. of applicants received the Cash Gift");
            beneficiaries_table_filter.Items.Add("Deceased");
            beneficiaries_table_filter.Items.Add("Unpaid");
            beneficiaries_table_filter.Items.Add("Paid");
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
            await LoadFilesFromFirebase();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

        private async void SelectSidebarButton(Guna.UI2.WinForms.Guna2Button selectedButton)
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

            // Refresh data based on which panel is selected
            if (selectedButton == dashboardButton)
            {
                await RefreshDashboardData();
            }
            else if (selectedButton == beneficiariesButton)
            {
                await RefreshBeneficiariesData();
            }
            else if (selectedButton == graphReportButton)
            {
                await RefreshGraphReportData();
            }
            else if (selectedButton == accounts_button)
            {
                await RefreshAccountsData();
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
                PointRadius = 4,
                BorderColor = Color.FromArgb(134, 115, 243),
                FillColor = Color.FromArgb(134, 115, 243)
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

            // Count how many birthdays per month from all beneficiaries (not just filtered table)
            foreach (var beneficiary in allBeneficiaries)
            {
                if (DateTime.TryParse(beneficiary.birth_date, out DateTime birthDate))
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

        private void beneficiaries_ultimate_filter_button_Click(object sender, EventArgs e)
        {
            var filterWindow = new Beneficiaries_filter_window();
            filterWindow.ShowDialog();
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

            // Count birthdays per month from all beneficiaries (not just filtered table)
            foreach (var beneficiary in allBeneficiaries)
            {
                if (DateTime.TryParse(beneficiary.birth_date, out DateTime birthDate))
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

            // Ensure color styling is maintained
            summary_graph_bday.BorderColor = Color.FromArgb(134, 115, 243);
            summary_graph_bday.FillColor = Color.FromArgb(134, 115, 243);

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

            // Count milestone birthdays from all beneficiaries (not just filtered table)
            foreach (var beneficiary in allBeneficiaries)
            {
                if (DateTime.TryParse(beneficiary.birth_date, out DateTime birthDate))
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
                            ShowBeneficiariesForMonth(month);
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
                ShowBeneficiariesForMonth(month);
            }
        }

        private void ShowBeneficiariesForMonth(string month, List<DataGridViewRow> rows = null)
        {
            // Get all beneficiaries from the allBeneficiaries list instead of the filtered table rows
            var beneficiariesForMonth = allBeneficiaries
                .Where(b => b.GetBirthMonth() == month)
                .ToList();

            var batchCodes = beneficiariesForMonth
                .Select(b => b.batch_code)
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
            beneficiaries_table.Columns.Add("paid_status", "Paid Status");
            beneficiaries_table.Columns.Add("deceased_status", "Deceased Status");

            try
            {
                // First, get the raw response to see the structure
                var rawResponse = await FirebaseHelper.GetDataAsync("beneficiaries");
                Console.WriteLine($"Raw Firebase response: {rawResponse}");

                var beneficiaries = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");

                allBeneficiaries.Clear();
                if (beneficiaries != null)
                {
                    Console.WriteLine($"Loaded {beneficiaries.Count} beneficiaries from Firebase");
                    foreach (var entry in beneficiaries.Values)
                    {
                        if (entry != null)
                        {
                            // Ensure boolean fields are properly initialized
                            if (entry.TotalEndorseFromLGUs == false && string.IsNullOrEmpty(entry.batch_code))
                            {
                                // This might be a deserialization issue, skip it
                                Console.WriteLine($"Skipping invalid beneficiary entry");
                                continue;
                            }

                            allBeneficiaries.Add(entry);
                            Console.WriteLine($"Added beneficiary: {entry.batch_code} - {entry.name ?? "No name"} - TotalEndorseFromLGUs: {entry.TotalEndorseFromLGUs}");
                            Console.WriteLine($"  Sex: '{entry.sex}' -> Normalized: '{entry.GetNormalizedSex()}'");
                            Console.WriteLine($"  Birth Date: '{entry.birth_date}' -> Normalized: '{entry.GetNormalizedBirthDate()}'");
                            Console.WriteLine($"  Date Validated: '{entry.date_validated}' -> Normalized: '{entry.GetNormalizedDateValidated()}'");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No beneficiaries data received from Firebase");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading beneficiaries from Firebase: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

            Console.WriteLine($"Applying filters - Total beneficiaries: {allBeneficiaries.Count}");
            Console.WriteLine($"Selected province: {selectedProvince}, municipality: {selectedMunicipality}, status: {selectedStatus}");

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
            if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All")
            {
                Console.WriteLine($"Filtering by status: {selectedStatus}");
                switch (selectedStatus)
                {
                    case "Total Endorse from LGUs":
                        filtered = filtered.Where(b => b.TotalEndorseFromLGUs);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for TotalEndorseFromLGUs");
                        break;
                    case "Assessed":
                        filtered = filtered.Where(b => b.Assessed);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for Assessed");
                        break;
                    case "Total Validated":
                        filtered = filtered.Where(b => b.TotalValidated);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for TotalValidated");
                        break;
                    case "Total Endorsed to NCSC CO":
                        filtered = filtered.Where(b => b.TotalEndorsedToNCSCO);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for TotalEndorsedToNCSCO");
                        break;
                    case "Total Cleaned list from NCSC CO":
                        filtered = filtered.Where(b => b.TotalCleanedListFromNCSCO);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for TotalCleanedListFromNCSCO");
                        break;
                    case "Scheduled payout":
                        filtered = filtered.Where(b => b.ScheduledPayout);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for ScheduledPayout");
                        break;
                    case "No. of applicants received the Cash Gift":
                        filtered = filtered.Where(b => b.NumberOfApplicantsReceivedCashGift);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for NumberOfApplicantsReceivedCashGift");
                        break;
                    case "Deceased":
                        filtered = filtered.Where(b => b.Deceased);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for Deceased");
                        break;
                    case "Unpaid":
                        filtered = filtered.Where(b => b.Unpaid);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for Unpaid");
                        break;
                    case "Paid":
                        filtered = filtered.Where(b => b.Paid);
                        Console.WriteLine($"Filtered to {filtered.Count()} records for Paid");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No status filter applied (showing all records)");
            }

            int addedCount = 0;
            foreach (var entry in filtered)
            {
                // Determine paid status based on Paid and Unpaid boolean attributes
                string paidStatus = "Not Paid";
                if (entry.Paid)
                    paidStatus = "Paid";
                else if (entry.Unpaid)
                    paidStatus = "Unpaid";

                // Determine deceased status based on Deceased boolean attribute
                string deceasedStatus = entry.Deceased ? "Deceased" : "Alive";

                beneficiaries_table.Rows.Add(
                    entry.batch_code,
                    entry.age,
                    entry.GetNormalizedBirthDate(),
                    entry.GetNormalizedSex(),
                    entry.region,
                    entry.province,
                    entry.municipality,
                    entry.barangay,
                    entry.GetNormalizedDateValidated(),
                    entry.pwd,
                    entry.ip,
                    paidStatus,
                    deceasedStatus
                );
                addedCount++;
            }
            // Apply deceased highlighting after populating rows
            ApplyDeceasedRowHighlighting();
            Console.WriteLine($"Added {addedCount} records to the table");
        }

        // Highlight rows for deceased beneficiaries
        private void ApplyDeceasedRowHighlighting()
        {
            try
            {
                var deceasedColor = ColorTranslator.FromHtml("#E06B80");
                foreach (DataGridViewRow row in beneficiaries_table.Rows)
                {
                    if (row.IsNewRow) continue;
                    var status = row.Cells["deceased_status"]?.Value?.ToString() ?? string.Empty;
                    if (status.Equals("Deceased", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = deceasedColor;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying deceased highlighting: {ex.Message}");
            }
        }

        private async void upload_file_button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a file to upload";
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|All Files|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(selectedFilePath);

                    try
                    {
                        // Show loading message
                        MessageBox.Show("Processing file... Please wait.", "Processing", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Parse Excel file
                        var parsedData = await ParseExcelFile(selectedFilePath);
                        
                        if (parsedData == null || parsedData.Count == 0)
                        {
                            MessageBox.Show("No data found in the Excel file or file could not be parsed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Check for duplicates
                        var duplicateCheckResult = await CheckForDuplicates(parsedData);
                        var uniqueRecords = duplicateCheckResult.uniqueRecords;
                        var duplicateRecords = duplicateCheckResult.duplicateRecords;

                        // Show confirmation dialog
                        string message = $"File: {fileName}\n";
                        message += $"Total Records: {parsedData.Count}\n";
                        message += $"Unique Records: {uniqueRecords.Count}\n";
                        message += $"Duplicate Records: {duplicateRecords.Count}\n\n";
                        message += "Do you want to proceed with uploading the unique records?";

                        if (duplicateRecords.Count > 0)
                        {
                            message += $"\n\nNote: {duplicateRecords.Count} duplicate records will be skipped.";
                        }

                        var result = MessageBox.Show(message, "Confirm Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Upload unique records to Firebase
                            await UploadBeneficiariesToFirebase(uniqueRecords, duplicateRecords, fileName);
                            
                            // Show success message
                            string successMessage = $"Upload completed!\n\n";
                            successMessage += $"✅ Unique records uploaded: {uniqueRecords.Count}\n";
                            successMessage += $"❌ Duplicate records skipped: {duplicateRecords.Count}\n";
                            successMessage += $"📋 File: {fileName}";

                            if (duplicateRecords.Count > 0)
                            {
                                successMessage += $"\n\n⚠️ Note: {duplicateRecords.Count} duplicate records were found and skipped.";
                            }

                            MessageBox.Show(successMessage, "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh the file history table and beneficiaries data
                            await LoadFilesFromFirebase();
                            await LoadBeneficiariesFromFirebase();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private async Task LoadFilesFromFirebase()
        {
            try
            {
                var files = await FirebaseHelper.GetDataAsync<Dictionary<string, FileData>>("files");

                if (files == null)
                {
                    Console.WriteLine("No files data received from Firebase");
                    return;
                }

                file_history_table.Rows.Clear();

                foreach (var file in files)
                {
                    var fileData = file.Value;
                    if (fileData != null)
                    {
                        file_history_table.Rows.Add(
                            fileData.GetFileName(),
                            fileData.province ?? "N/A",
                            fileData.municipality ?? "N/A",
                            fileData.GetFormattedUploadDate(),
                            fileData.total_records.ToString()
                        );
                    }
                }

                Console.WriteLine($"Loaded {files.Count} files from Firebase");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading files from Firebase: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
            // TODO: Add method to update historical report charts based on filter selectionq
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

            // Initialize status filter dropdown - add "All" at the beginning
            graph_report_batch_graph_filter.Items.Clear();
            graph_report_batch_graph_filter.Items.Add("All");
            graph_report_batch_graph_filter.Items.Add("Total Endorse from LGUs");
            graph_report_batch_graph_filter.Items.Add("Assessed");
            graph_report_batch_graph_filter.Items.Add("Total Validated");
            graph_report_batch_graph_filter.Items.Add("Total Endorsed to NCSC CO");
            graph_report_batch_graph_filter.Items.Add("Total Cleaned list from NCSC CO");
            graph_report_batch_graph_filter.Items.Add("Scheduled payout");
            graph_report_batch_graph_filter.Items.Add("No. of applicants received the Cash Gift");
            graph_report_batch_graph_filter.Items.Add("Deceased");
            graph_report_batch_graph_filter.Items.Add("Unpaid");
            graph_report_batch_graph_filter.Items.Add("Paid");
            graph_report_batch_graph_filter.SelectedIndex = 0;

            // Set up event handlers for batch graph filters
            graph_report_batch_graph_province.SelectedIndexChanged += graph_report_batch_graph_province_SelectedIndexChanged;
            graph_report_batch_graph_municipality.SelectedIndexChanged += graph_report_batch_graph_municipality_SelectedIndexChanged;
            graph_report_batch_graph_filter.SelectedIndexChanged += graph_report_batch_graph_filter_SelectedIndexChanged;
        }

        private async void graph_report_batch_graph_province_SelectedIndexChanged(object sender, EventArgs e)
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
            // Update batch graph charts based on filter selection
            await UpdateBatchGraphChartsAsync();
        }

        private async void graph_report_batch_graph_municipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update batch graph charts based on filter selection
            await UpdateBatchGraphChartsAsync();
        }

        private async void graph_report_batch_graph_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update batch graph charts based on filter selection
            await UpdateBatchGraphChartsAsync();
        }

        // Synchronous wrapper for backward compatibility - redirects to async version
        private void UpdateBatchGraphCharts()
        {
            // Call the async version - use GetAwaiter().GetResult() for synchronous call
            UpdateBatchGraphChartsAsync().GetAwaiter().GetResult();
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
            // Get all beneficiaries from the allBeneficiaries list instead of the filtered table
            var beneficiariesForMonth = allBeneficiaries
                .Where(b => b.GetBirthMonth() == month)
                .ToList();

            var batchCodes = beneficiariesForMonth
                .Select(b => b.batch_code)
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
            var totalEndorseFromLGUsItem = beneficiariesContextMenu.Items.Add("Add to: Total Endorse from LGUs");
            totalEndorseFromLGUsItem.Click += (s, e) => UpdateBeneficiaryStatus("TotalEndorseFromLGUs");

            var assessedItem = beneficiariesContextMenu.Items.Add("Add to: Assessed");
            assessedItem.Click += (s, e) => UpdateBeneficiaryStatus("Assessed");

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

            var deceasedItem = beneficiariesContextMenu.Items.Add("Add to: Deceased");
            deceasedItem.Click += (s, e) => UpdateBeneficiaryStatus("Deceased");

            var unpaidItem = beneficiariesContextMenu.Items.Add("Add to: Unpaid");
            unpaidItem.Click += (s, e) => UpdateBeneficiaryStatus("Unpaid");

            var paidItem = beneficiariesContextMenu.Items.Add("Add to: Paid");
            paidItem.Click += (s, e) => UpdateBeneficiaryStatus("Paid");

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
                // Build a set of selected batch codes (trimmed to handle whitespace issues)
                var selectedBatchCodes = new List<string>();
                foreach (DataGridViewRow row in beneficiaries_table.SelectedRows)
                {
                    if (row.IsNewRow) continue;
                    var code = row.Cells["batch_code_col"].Value?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(code)) selectedBatchCodes.Add(code);
                }

                if (selectedBatchCodes.Count == 0)
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Fetch all beneficiaries once to map batch_code -> key
                var allBeneficiariesFromFirebase = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");
                if (allBeneficiariesFromFirebase == null)
                {
                    MessageBox.Show("No beneficiaries found in storage.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create mapping with trimmed batch codes to handle whitespace issues
                var batchCodeToKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var batchCodeToBeneficiary = new Dictionary<string, Beneficiary>(StringComparer.OrdinalIgnoreCase);
                
                foreach (var entry in allBeneficiariesFromFirebase)
                {
                    var val = entry.Value;
                    if (val == null) continue;
                    
                    var trimmedBatchCode = val.batch_code?.Trim();
                    if (!string.IsNullOrEmpty(trimmedBatchCode))
                    {
                        // If duplicate batch code exists, log it but use the first one found
                        if (!batchCodeToKey.ContainsKey(trimmedBatchCode))
                        {
                            batchCodeToKey[trimmedBatchCode] = entry.Key;
                            batchCodeToBeneficiary[trimmedBatchCode] = val;
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Duplicate batch code found: {trimmedBatchCode}");
                        }
                    }
                }

                int updatedCount = 0;
                var failedBatchCodes = new List<string>();
                var failureReasons = new List<string>();

                foreach (var code in selectedBatchCodes)
                {
                    // Try to find the batch code in Firebase (case-insensitive, trimmed)
                    if (!batchCodeToKey.TryGetValue(code, out var key))
                    {
                        failedBatchCodes.Add(code);
                        failureReasons.Add("Batch code not found in Firebase");
                        Console.WriteLine($"Failed to find batch code '{code}' in Firebase.");
                        continue;
                    }

                    // Get beneficiary directly from Firebase data instead of local filtered list
                    if (!batchCodeToBeneficiary.TryGetValue(code, out var beneficiary))
                    {
                        failedBatchCodes.Add(code);
                        failureReasons.Add("Beneficiary data not found");
                        Console.WriteLine($"Failed to find beneficiary data for batch code '{code}'.");
                        continue;
                    }

                    // Update field on model
                    switch (statusField)
                    {
                        case "TotalEndorseFromLGUs":
                            beneficiary.TotalEndorseFromLGUs = true;
                            break;
                        case "Assessed":
                            beneficiary.Assessed = true;
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
                        case "Deceased":
                            beneficiary.Deceased = true;
                            break;
                        case "Unpaid":
                            beneficiary.Unpaid = true;
                            break;
                        case "Paid":
                            beneficiary.Paid = true;
                            break;
                        default:
                            MessageBox.Show("Invalid status field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    try
                    {
                        // Persist to storage
                        await FirebaseHelper.SetDataAsync($"beneficiaries/{key}", beneficiary);
                        
                        // Update local list if it exists there
                        var localBeneficiary = allBeneficiaries.FirstOrDefault(b => b.batch_code?.Trim() == code);
                        if (localBeneficiary != null)
                        {
                            // Sync the updated field to local list
                            switch (statusField)
                            {
                                case "TotalEndorseFromLGUs":
                                    localBeneficiary.TotalEndorseFromLGUs = true;
                                    break;
                                case "Assessed":
                                    localBeneficiary.Assessed = true;
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
                                case "Deceased":
                                    localBeneficiary.Deceased = true;
                                    break;
                                case "Unpaid":
                                    localBeneficiary.Unpaid = true;
                                    break;
                                case "Paid":
                                    localBeneficiary.Paid = true;
                                    break;
                            }
                        }
                        
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        failedBatchCodes.Add(code);
                        failureReasons.Add($"Firebase update error: {ex.Message}");
                        Console.WriteLine($"Error updating batch code '{code}' to Firebase: {ex.Message}");
                    }
                }

                // Refresh UI
                ApplyBeneficiaryFilters();
                UpdateBeneficiaryCounts();

                // Report results with details about failures
                string message = $"Updated {updatedCount} beneficiary(ies) to {statusField}.";
                
                if (failedBatchCodes.Count > 0)
                {
                    message += $"\n\nFailed to update {failedBatchCodes.Count} beneficiary(ies):\n";
                    for (int i = 0; i < failedBatchCodes.Count && i < 10; i++) // Show up to 10 failures
                    {
                        message += $"- {failedBatchCodes[i]}: {failureReasons[i]}\n";
                    }
                    if (failedBatchCodes.Count > 10)
                    {
                        message += $"... and {failedBatchCodes.Count - 10} more.";
                    }
                }

                MessageBox.Show(message, updatedCount > 0 ? "Partial Success" : "Update Failed", 
                    MessageBoxButtons.OK, 
                    failedBatchCodes.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating beneficiary status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Exception in UpdateBeneficiaryStatus: {ex.StackTrace}");
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

        // Refresh methods for each panel
        private async Task RefreshDashboardData()
        {
            try
            {
                // Reload beneficiaries from Firebase
                await LoadBeneficiariesFromFirebase();

                // Update all counters
                UpdateBeneficiaryCounts();

                // Update all charts
                UpdateBirthdaySummaryGraph();
                UpdateMilestoneBirthdayChartByMonth();

                // Update the randomizer chart
                randomizer();

                Console.WriteLine("Dashboard data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing dashboard data: {ex.Message}");
            }
        }

        private async Task RefreshBeneficiariesData()
        {
            try
            {
                // Reload beneficiaries from Firebase
                await LoadBeneficiariesFromFirebase();

                // Update counters
                UpdateBeneficiaryCounts();

                Console.WriteLine("Beneficiaries data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing beneficiaries data: {ex.Message}");
            }
        }

        private async Task RefreshGraphReportData()
        {
            try
            {
                // Reload beneficiaries from Firebase for chart data
                await LoadBeneficiariesFromFirebase();

                // Reload files from Firebase for file history table
                await LoadFilesFromFirebase();

                // Update all charts in the graph report panel
                UpdateHistoricalReportCharts();
                await UpdateBatchGraphChartsAsync();

                Console.WriteLine("Graph report data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing graph report data: {ex.Message}");
            }
        }

        // Async version for RefreshGraphReportData
        private async Task UpdateBatchGraphChartsAsync()
        {
            try
            {
                // Get filter selections
                string selectedProvince = graph_report_batch_graph_province.SelectedItem?.ToString();
                string selectedMunicipality = graph_report_batch_graph_municipality.SelectedItem?.ToString();
                string selectedStatus = graph_report_batch_graph_filter.SelectedItem?.ToString();
                int currentYear = DateTime.Now.Year;

                // If "All" is selected or filters are empty, show data for all
                bool filterByProvince = !string.IsNullOrEmpty(selectedProvince) && selectedProvince != "All";
                bool filterByMunicipality = !string.IsNullOrEmpty(selectedMunicipality) && selectedMunicipality != "All" && selectedMunicipality != "Municipality";
                bool filterByStatus = !string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All";

                // Load files from Firebase
                var allFiles = await FirebaseHelper.GetDataAsync<Dictionary<string, FileData>>("files");
                
                if (allFiles == null)
                {
                    // Clear chart if no data
                    graph_report_bar_chart_dataset.DataPoints.Clear();
                    graph_report_chart.Datasets.Clear();
                    graph_report_chart.Update();
                    return;
                }

                // Filter files based on selections
                var filteredFiles = allFiles.Values.Where(f => f != null).AsEnumerable();
                
                if (filterByProvince)
                {
                    filteredFiles = filteredFiles.Where(f => f.province == selectedProvince);
                }
                
                if (filterByMunicipality)
                {
                    filteredFiles = filteredFiles.Where(f => f.municipality == selectedMunicipality);
                }

                // Filter by current year
                filteredFiles = filteredFiles.Where(f => f.upload_year == currentYear);

                // If status filter is active, we need to count beneficiaries with that status per batch
                if (filterByStatus)
                {
                    // Load beneficiaries to check their status
                    var allBeneficiaries = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");
                    
                    if (allBeneficiaries == null)
                    {
                        graph_report_bar_chart_dataset.DataPoints.Clear();
                        graph_report_chart.Datasets.Clear();
                        graph_report_chart.Update();
                        return;
                    }

                    // Group by batch number and count beneficiaries with the selected status
                    var batchData = new Dictionary<int, int>();
                    
                    foreach (var file in filteredFiles)
                    {
                        if (file.batch_number > 0)
                        {
                            if (!batchData.ContainsKey(file.batch_number))
                            {
                                batchData[file.batch_number] = 0;
                            }
                            
                            // Count beneficiaries in this batch with the selected status
                            int countWithStatus = 0;
                            foreach (var beneficiary in allBeneficiaries.Values)
                            {
                                if (beneficiary != null && beneficiary.batch_number == file.batch_number)
                                {
                                    // Apply province/municipality filter if needed
                                    bool matchesLocation = true;
                                    if (filterByProvince && !string.Equals(beneficiary.province?.Trim(), selectedProvince, StringComparison.OrdinalIgnoreCase))
                                    {
                                        matchesLocation = false;
                                    }
                                    if (filterByMunicipality && !string.Equals(beneficiary.municipality?.Trim(), selectedMunicipality, StringComparison.OrdinalIgnoreCase))
                                    {
                                        matchesLocation = false;
                                    }
                                    
                                    if (matchesLocation && CheckBeneficiaryStatus(beneficiary, selectedStatus))
                                    {
                                        countWithStatus++;
                                    }
                                }
                            }
                            
                            batchData[file.batch_number] += countWithStatus;
                        }
                    }

                    // Sort by batch number
                    var sortedBatches = batchData.OrderBy(b => b.Key).ToList();

                    // Clear existing chart data
                    graph_report_bar_chart_dataset.DataPoints.Clear();
                    graph_report_chart.Datasets.Clear();

                    // If no batches found, show empty chart
                    if (sortedBatches.Count == 0)
                    {
                        graph_report_chart.Update();
                        return;
                    }

                    // Calculate total of all batches
                    int totalBeneficiaries = sortedBatches.Sum(b => b.Value);

                    // Add data points to chart - format batch labels as "Batch 1", "Batch 2", etc.
                    foreach (var batch in sortedBatches)
                    {
                        string batchLabel = $"Batch {batch.Key}";
                        graph_report_bar_chart_dataset.DataPoints.Add(batchLabel, batch.Value);
                    }

                    // Add total bar at the end
                    graph_report_bar_chart_dataset.DataPoints.Add("Total", totalBeneficiaries);

                    // Configure the dataset with status label
                    graph_report_bar_chart_dataset.Label = $"{selectedStatus} Beneficiaries per Batch";
                }
                else
                {
                    // No status filter - use file counts (original behavior)
                    var batchData = new Dictionary<int, int>();
                    
                    foreach (var file in filteredFiles)
                    {
                        if (file.batch_number > 0)
                        {
                            if (!batchData.ContainsKey(file.batch_number))
                            {
                                batchData[file.batch_number] = 0;
                            }
                            batchData[file.batch_number] += file.unique_records; // Count unique beneficiaries per batch
                        }
                    }

                    // Sort by batch number
                    var sortedBatches = batchData.OrderBy(b => b.Key).ToList();

                    // Clear existing chart data
                    graph_report_bar_chart_dataset.DataPoints.Clear();
                    graph_report_chart.Datasets.Clear();

                    // If no batches found, show empty chart
                    if (sortedBatches.Count == 0)
                    {
                        graph_report_chart.Update();
                        return;
                    }

                    // Calculate total of all batches
                    int totalBeneficiaries = sortedBatches.Sum(b => b.Value);

                    // Add data points to chart - format batch labels as "Batch 1", "Batch 2", etc.
                    foreach (var batch in sortedBatches)
                    {
                        string batchLabel = $"Batch {batch.Key}";
                        graph_report_bar_chart_dataset.DataPoints.Add(batchLabel, batch.Value);
                    }

                    // Add total bar at the end
                    graph_report_bar_chart_dataset.DataPoints.Add("Total", totalBeneficiaries);

                    // Configure the dataset
                    graph_report_bar_chart_dataset.Label = "Beneficiaries per Batch";
                }
                
                // Add dataset to chart
                graph_report_chart.Datasets.Add(graph_report_bar_chart_dataset);
                
                // Update the chart
                graph_report_chart.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating batch graph charts: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Helper method to check if a beneficiary has a specific status
        private bool CheckBeneficiaryStatus(Beneficiary beneficiary, string status)
        {
            if (beneficiary == null || string.IsNullOrEmpty(status))
                return false;

            switch (status)
            {
                case "Total Endorse from LGUs":
                    return beneficiary.TotalEndorseFromLGUs;
                case "Assessed":
                    return beneficiary.Assessed;
                case "Total Validated":
                    return beneficiary.TotalValidated;
                case "Total Endorsed to NCSC CO":
                    return beneficiary.TotalEndorsedToNCSCO;
                case "Total Cleaned list from NCSC CO":
                    return beneficiary.TotalCleanedListFromNCSCO;
                case "Scheduled payout":
                    return beneficiary.ScheduledPayout;
                case "No. of applicants received the Cash Gift":
                    return beneficiary.NumberOfApplicantsReceivedCashGift;
                case "Deceased":
                    return beneficiary.Deceased;
                case "Unpaid":
                    return beneficiary.Unpaid;
                case "Paid":
                    return beneficiary.Paid;
                default:
                    return false;
            }
        }

        private async Task RefreshAccountsData()
        {
            try
            {
                // Reload provincial accounts
                await LoadProvincialAccountsAsync();

                Console.WriteLine("Accounts data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing accounts data: {ex.Message}");
            }
        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        // Excel parsing functionality
        private async Task<List<Beneficiary>> ParseExcelFile(string filePath)
        {
            try
            {
                using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var beneficiaries = new List<Beneficiary>();

                    // Get the number of rows and columns
                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;

                    if (rowCount <= 1) return beneficiaries; // No data rows

                    // Get headers from first row
                    var headers = new List<string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        headers.Add(worksheet.Cells[1, col].Value?.ToString() ?? "");
                    }

                    // Debug: Log the headers found
                    Console.WriteLine($"Excel Headers found: {string.Join(", ", headers)}");

                    // Process data rows
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var beneficiary = new Beneficiary();
                        bool hasData = false;

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? "";
                            var header = headers[col - 1].ToLower().Trim();

                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                hasData = true;
                            }

                            // Debug: Log the mapping attempt
                            Console.WriteLine($"Mapping column '{header}' with value '{cellValue}'");

                            // Map Excel columns to Beneficiary properties
                            switch (header)
                            {
                                case "first_name":
                                case "first name":
                                case "firstname":
                                case "given name":
                                    beneficiary.first_name = cellValue;
                                    break;
                                case "middle_name":
                                case "middle name":
                                case "middlename":
                                    beneficiary.middle_name = cellValue;
                                    break;
                                case "last_name":
                                case "last name":
                                case "lastname":
                                case "surname":
                                    beneficiary.last_name = cellValue;
                                    break;
                                case "name":
                                case "full name":
                                case "fullname":
                                    beneficiary.name = cellValue;
                                    break;
                                case "age":
                                    if (int.TryParse(cellValue, out int age))
                                        beneficiary.age = age.ToString();
                                    break;
                                case "sex":
                                case "gender":
                                    beneficiary.sex = cellValue;
                                    break;
                                case "birth_date":
                                case "birth date":
                                case "birthdate":
                                case "date of birth":
                                case "dob":
                                    beneficiary.birth_date = cellValue;
                                    break;
                                case "barangay":
                                case "address":
                                case "location":
                                    beneficiary.barangay = cellValue;
                                    break;
                                case "region":
                                    beneficiary.region = cellValue;
                                    break;
                                case "province":
                                    beneficiary.province = cellValue;
                                    break;
                                case "municipality":
                                    beneficiary.municipality = cellValue;
                                    break;
                                case "ip":
                                case "indigenous people":
                                case "ip status":
                                    beneficiary.ip = cellValue;
                                    break;
                                case "pwd":
                                case "person with disability":
                                case "pwd status":
                                case "disability":
                                case "disabled":
                                    beneficiary.pwd = cellValue;
                                    break;
                            }
                        }

                        if (hasData)
                        {
                            // Set default values - all false except Total Endorse from LGUs
                            beneficiary.Assessed = false;
                            beneficiary.NumberOfApplicantsReceivedCashGift = false;
                            beneficiary.ScheduledPayout = false;
                            beneficiary.TotalCleanedListFromNCSCO = false;
                            beneficiary.TotalEndorseFromLGUs = true; // Default to true for new beneficiaries
                            beneficiary.TotalEndorsedToNCSCO = false;
                            beneficiary.TotalValidated = false;
                            beneficiary.Deceased = false;
                            beneficiary.Unpaid = false;
                            beneficiary.Paid = false;

                            // Generate batch code
                            beneficiary.batch_code = GenerateBatchCode();

                            // Set validation date
                            beneficiary.date_validated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                            // Set province_municipality_date
                            if (!string.IsNullOrEmpty(beneficiary.province) && !string.IsNullOrEmpty(beneficiary.municipality))
                            {
                                beneficiary.province_municipality_date = $"{beneficiary.province}_{beneficiary.municipality}_{DateTime.Now:yyyy-MM-dd}";
                            }

                            // Calculate age from birth_date if age is not provided
                            if (string.IsNullOrEmpty(beneficiary.age) && !string.IsNullOrEmpty(beneficiary.birth_date))
                            {
                                beneficiary.age = CalculateAgeFromBirthDate(beneficiary.birth_date);
                            }

                            // Ensure name is constructed if not provided
                            if (string.IsNullOrEmpty(beneficiary.name) && 
                                (!string.IsNullOrEmpty(beneficiary.first_name) || 
                                 !string.IsNullOrEmpty(beneficiary.middle_name) || 
                                 !string.IsNullOrEmpty(beneficiary.last_name)))
                            {
                                var nameParts = new List<string>();
                                if (!string.IsNullOrEmpty(beneficiary.first_name)) nameParts.Add(beneficiary.first_name);
                                if (!string.IsNullOrEmpty(beneficiary.middle_name)) nameParts.Add(beneficiary.middle_name);
                                if (!string.IsNullOrEmpty(beneficiary.last_name)) nameParts.Add(beneficiary.last_name);
                                beneficiary.name = string.Join(" ", nameParts);
                            }

                            beneficiaries.Add(beneficiary);
                        }
                    }

                    return beneficiaries;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing Excel file: {ex.Message}");
            }
        }

        // Generate batch code for WinForms
        private string GenerateBatchCode()
        {
            // Generate a simple batch code with timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ADMIN-{timestamp}-{random}";
        }

        // Calculate age from birth date
        private string CalculateAgeFromBirthDate(string birthDate)
        {
            try
            {
                if (DateTime.TryParse(birthDate, out DateTime birth))
                {
                    var today = DateTime.Today;
                    var age = today.Year - birth.Year;
                    
                    // Adjust if birthday hasn't occurred this year
                    if (birth.Date > today.AddYears(-age))
                    {
                        age--;
                    }
                    
                    return age.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating age from birth date '{birthDate}': {ex.Message}");
            }
            
            return "";
        }

        // Check for duplicates
        private async Task<(List<Beneficiary> uniqueRecords, List<Beneficiary> duplicateRecords)> CheckForDuplicates(List<Beneficiary> newRecords)
        {
            var uniqueRecords = new List<Beneficiary>();
            var duplicateRecords = new List<Beneficiary>();

            try
            {
                // Get existing beneficiaries from Firebase
                var existingBeneficiaries = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");
                
                if (existingBeneficiaries == null)
                {
                    return (newRecords, new List<Beneficiary>());
                }

                foreach (var newRecord in newRecords)
                {
                    bool isDuplicate = false;

                    foreach (var existing in existingBeneficiaries.Values)
                    {
                        if (existing != null && IsDuplicateRecord(newRecord, existing))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (isDuplicate)
                    {
                        duplicateRecords.Add(newRecord);
                    }
                    else
                    {
                        uniqueRecords.Add(newRecord);
                    }
                }

                return (uniqueRecords, duplicateRecords);
            }
            catch (Exception ex)
            {
                // If error occurs, treat all records as unique to avoid data loss
                return (newRecords, new List<Beneficiary>());
            }
        }

        // Check if two records are duplicates
        private bool IsDuplicateRecord(Beneficiary record1, Beneficiary record2)
        {
            return NormalizeString(record1.birth_date) == NormalizeString(record2.birth_date) &&
                   NormalizeString(record1.first_name) == NormalizeString(record2.first_name) &&
                   NormalizeString(record1.middle_name) == NormalizeString(record2.middle_name) &&
                   NormalizeString(record1.last_name) == NormalizeString(record2.last_name);
        }

        // Normalize string for comparison
        private string NormalizeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Trim().ToLower();
        }

        // Get the next batch number for a province/municipality/year combination
        private async Task<int> GetNextBatchNumber(string province, string municipality, int year)
        {
            try
            {
                // Normalize province and municipality for comparison (trim and ignore case)
                string normalizedProvince = (province ?? "").Trim();
                string normalizedMunicipality = (municipality ?? "").Trim();

                // Get all files from Firebase
                var allFiles = await FirebaseHelper.GetDataAsync<Dictionary<string, FileData>>("files");
                
                if (allFiles == null || allFiles.Count == 0)
                {
                    Console.WriteLine($"GetNextBatchNumber: No files found, returning batch 1 for {normalizedProvince}/{normalizedMunicipality}/{year}");
                    return 1; // First batch if no files exist
                }

                // Find the highest batch number for this province/municipality/year
                int maxBatchNumber = 0;
                int matchingFilesCount = 0;
                bool hasFilesWithZeroBatch = false;
                
                foreach (var file in allFiles.Values)
                {
                    if (file != null)
                    {
                        // Normalize file's province and municipality for comparison
                        string fileProvince = (file.province ?? "").Trim();
                        string fileMunicipality = (file.municipality ?? "").Trim();
                        
                        // Case-insensitive comparison
                        if (string.Equals(fileProvince, normalizedProvince, StringComparison.OrdinalIgnoreCase) && 
                            string.Equals(fileMunicipality, normalizedMunicipality, StringComparison.OrdinalIgnoreCase) &&
                            file.upload_year == year)
                        {
                            matchingFilesCount++;
                            
                            // Count files with batch_number = 0 (files uploaded before batch tracking was added)
                            if (file.batch_number == 0)
                            {
                                hasFilesWithZeroBatch = true;
                            }
                            else if (file.batch_number > maxBatchNumber)
                            {
                                maxBatchNumber = file.batch_number;
                            }
                        }
                    }
                }
                
                // If we found files with batch_number = 0, treat them as batch 1
                // So if we have files with batch_number = 0, the next batch should be 2
                if (hasFilesWithZeroBatch && maxBatchNumber == 0)
                {
                    maxBatchNumber = 1; // Treat existing files with 0 as batch 1
                }

                int nextBatchNumber = maxBatchNumber + 1;
                Console.WriteLine($"GetNextBatchNumber: Found {matchingFilesCount} matching files for {normalizedProvince}/{normalizedMunicipality}/{year}. Max batch: {maxBatchNumber}, Next batch: {nextBatchNumber}");

                return nextBatchNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating batch number: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1; // Default to 1 on error
            }
        }

        // Extract province and municipality from beneficiaries (using most common or first valid value)
        private (string province, string municipality) ExtractProvinceMunicipality(List<Beneficiary> beneficiaries)
        {
            if (beneficiaries == null || beneficiaries.Count == 0)
            {
                return ("Admin Upload", "Admin Upload");
            }

            // Get province - use the most common province, or first valid one (normalized)
            var validProvinces = beneficiaries
                .Where(b => !string.IsNullOrWhiteSpace(b.province))
                .Select(b => b.province.Trim())
                .ToList();
            
            string province = validProvinces.Any() 
                ? validProvinces.GroupBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .First().Key
                : "Admin Upload";

            // Get municipality - use the most common municipality for the selected province, or first valid one (normalized)
            var validMunicipalities = beneficiaries
                .Where(b => !string.IsNullOrWhiteSpace(b.municipality) && 
                           string.Equals((b.province ?? "").Trim(), province, StringComparison.OrdinalIgnoreCase))
                .Select(b => b.municipality.Trim())
                .ToList();
            
            string municipality = validMunicipalities.Any()
                ? validMunicipalities.GroupBy(m => m, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .First().Key
                : (beneficiaries.FirstOrDefault(b => !string.IsNullOrWhiteSpace(b.municipality))?.municipality?.Trim() ?? "Admin Upload");

            Console.WriteLine($"ExtractProvinceMunicipality: Extracted province='{province}', municipality='{municipality}' from {beneficiaries.Count} beneficiaries");

            return (province, municipality);
        }

        // Upload beneficiaries to Firebase
        private async Task UploadBeneficiariesToFirebase(List<Beneficiary> uniqueRecords, List<Beneficiary> duplicateRecords, string fileName)
        {
            try
            {
                // Extract province and municipality from the records
                var (province, municipality) = ExtractProvinceMunicipality(uniqueRecords);
                int currentYear = DateTime.Now.Year;

                Console.WriteLine($"UploadBeneficiariesToFirebase: Starting upload for {fileName}");
                Console.WriteLine($"UploadBeneficiariesToFirebase: Extracted province='{province}', municipality='{municipality}', year={currentYear}");

                // Get the next batch number for this province/municipality/year
                int batchNumber = await GetNextBatchNumber(province, municipality, currentYear);
                
                Console.WriteLine($"UploadBeneficiariesToFirebase: Assigned batch number {batchNumber} for {uniqueRecords.Count} beneficiaries");

                // Upload each unique beneficiary with auto-generated key and assign batch number
                foreach (var beneficiary in uniqueRecords)
                {
                    // Assign batch number to beneficiary
                    beneficiary.batch_number = batchNumber;
                    
                    // Generate unique key for each beneficiary
                    await FirebaseHelper.PushDataAsync("beneficiaries", beneficiary);
                }

                // Create file tracking record with proper counts
                var fileRecord = new FileData
                {
                    batch_code = GenerateBatchCode(), // Generate a batch code for the file
                    file_name = fileName,
                    province = province,
                    municipality = municipality,
                    province_municipality_date = $"{province}_{municipality}_{DateTime.Now:yyyy-MM-dd}",
                    total_records = uniqueRecords.Count + duplicateRecords.Count,
                    unique_records = uniqueRecords.Count,
                    duplicate_records = duplicateRecords.Count,
                    upload_date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    uploaded_by = _username ?? "Unknown Admin",
                    batch_number = batchNumber,
                    upload_year = currentYear
                };

                // Store file record with auto-generated key
                await FirebaseHelper.PushDataAsync("files", fileRecord);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading to Firebase: {ex.Message}");
            }
        }
    }
}
