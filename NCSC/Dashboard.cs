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
using Guna.Charts.WinForms;

namespace NCSC
{
    public partial class Dashboard : Form
    {

        private List<Guna2Button> sidebarButtons;
        public Dashboard()
        {
            InitializeComponent();
            sidebarButtons = new List<Guna2Button> { dashboardButton, beneficiariesButton, messageButton, graphReportButton, aboutButton };
            SelectSidebarButton(dashboardButton);
            UpdateBeneficiaryCounts();
            guna2Panel13.MouseClick += SummaryGraph_MouseClick;
            randomizer();

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

            // Populate municipality filter with 'All' by default
            beneficiaries_municipality_filter.Items.Clear();
            beneficiaries_municipality_filter.Items.Add("All");
            beneficiaries_municipality_filter.SelectedIndex = 0;
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            await LoadBeneficiariesFromFirebase();
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
            MessageBox.Show("Panel click!", "Debug");
            if (summaryGraph.Datasets.Count == 0)
                return;

            var dataset = summaryGraph.Datasets[0] as GunaLineDataset;
            if (dataset == null || dataset.DataPoints.Count == 0)
            {
                MessageBox.Show("Dataset is not a GunaLineDataset or is empty.");
                return;
            }

            int pointCount = dataset.DataPoints.Count;

            // Estimate plotting area (THESE ARE THE VALUES TO TWEAK)
            int leftMargin = 50;
            int rightMargin = 20; // Y-axis labels might take up less space
            int plotWidth = summaryGraph.Width - leftMargin - rightMargin;

            if (plotWidth <= 0) return; // Avoid division by zero if chart is too small

            // For a single point, the spacing is not relevant, it's at the start.
            float pointSpacing = (pointCount > 1) ? (float)plotWidth / (pointCount - 1) : 0;

            int clickedIndex = -1;
            float minDist = float.MaxValue;

            // For debugging, let's build a string
            StringBuilder debugInfo = new StringBuilder();
            debugInfo.AppendLine($"Click X: {e.X}");
            debugInfo.AppendLine($"Plot Width: {plotWidth}, Point Spacing: {pointSpacing:F2}");

            for (int i = 0; i < pointCount; i++)
            {
                float pointX = leftMargin + (i * pointSpacing);
                float dist = Math.Abs(e.X - pointX);

                debugInfo.AppendLine($"Point {i} ({dataset.DataPoints[i].Label}): Calculated X = {pointX:F2}, Dist = {dist:F2}");

                if (dist < minDist)
                {
                    minDist = dist;
                    clickedIndex = i;
                }
            }

            debugInfo.AppendLine($"\nClosest point is index {clickedIndex} ({dataset.DataPoints[clickedIndex].Label}) with distance {minDist:F2}.");

            // You can comment this out once it's working
            //MessageBox.Show(debugInfo.ToString(), "Chart Click Debug");

            // Increase sensitivity: check if the click is within half the spacing distance, or a fixed threshold for a single point
            bool isClickValid = (pointCount > 1 && minDist < (pointSpacing / 2)) || (pointCount == 1 && minDist < 20);

            if (clickedIndex != -1 && isClickValid)
            {
                var month = dataset.DataPoints[clickedIndex].Label;

                // Filter and show popup as before
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
            Form popup = new Form();
            popup.Text = $"Birthdays in {month}";
            popup.Size = new Size(900, 400);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;

            // Copy columns from beneficiaries_table
            foreach (DataGridViewColumn col in beneficiaries_table.Columns)
            {
                dgv.Columns.Add((DataGridViewColumn)col.Clone());
            }

            // Add rows
            foreach (var row in rows)
            {
                int idx = dgv.Rows.Add();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dgv.Rows[idx].Cells[i].Value = row.Cells[i].Value;
                }
            }

            popup.Controls.Add(dgv);
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.ShowDialog();
        }

        private void graphReportPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void accounts_button_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(accounts_button);
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

            var filtered = allBeneficiaries.AsEnumerable();
            if (!string.IsNullOrEmpty(selectedProvince) && selectedProvince != "All" && provinceMunicipalities.ContainsKey(selectedProvince))
            {
                filtered = filtered.Where(b => b.province == selectedProvince);
            }
            if (!string.IsNullOrEmpty(selectedMunicipality) && selectedMunicipality != "All" && selectedMunicipality != "Municipality")
            {
                filtered = filtered.Where(b => b.municipality == selectedMunicipality);
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
    }
}
