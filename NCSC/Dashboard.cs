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
            summaryGraph.MouseClick += SummaryGraph_MouseClick;
            randomizer();
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
            bday100.DataPoints.Clear();

            // Prepare 12-month counters for each milestone
            int[] count80 = new int[12];
            int[] count85 = new int[12];
            int[] count90 = new int[12];
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
                bday100.DataPoints.Add(monthName, count100[i]);
            }

            // Add all datasets to the chart
            budgetChart.Datasets.Add(bday80);
            budgetChart.Datasets.Add(bday85);
            budgetChart.Datasets.Add(bday90);
            budgetChart.Datasets.Add(bday100);

            budgetChart.Update();
        }





        private void SummaryGraph_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void graphReportPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void accounts_button_Click(object sender, EventArgs e)
        {
            SelectSidebarButton(accounts_button);
        }

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

            if (beneficiaries != null)
            {
                foreach (var entry in beneficiaries.Values)
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

            UpdateBeneficiaryCounts();
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

    }
}
