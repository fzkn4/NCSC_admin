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
        public Dashboard()
        {
            InitializeComponent();
            sidebarButtons = new List<Guna2Button> { dashboardButton, beneficiariesButton, messageButton, graphReportButton, aboutButton };
            SelectSidebarButton(dashboardButton);

            LoadRandomBeneficiariesData();
            UpdateBeneficiaryCounts();

            summaryGraph.MouseClick += SummaryGraph_MouseClick;

            randomizer();
        }


        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

        private void SelectSidebarButton(Guna.UI2.WinForms.Guna2Button selectedButton)
        {
            // List of buttons
            var buttons = new List<Guna.UI2.WinForms.Guna2Button>
        {
            dashboardButton,
            beneficiariesButton,
            messageButton,
            graphReportButton,
            aboutButton
        };

            // Corresponding panels — MUST be in the same order as buttons
            var panels = new List<Panel>
        {
            dashboardPanel,
            beneficiariesPanel,
            messagePanel,
            graphReportPanel,
            aboutPanel
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
        private void LoadRandomBeneficiariesData()
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

            // Sample data
            string[] regions = { "Zamboanga del Sur", "Zamboanga del Norte", "Zamboanga City", "Isabela City", "Zamboanga Sibugay" };
            string[] provinces = { "Province A", "Province B", "Province C" };
            string[] municipalities = { "Municipal 1", "Municipal 2", "Municipal 3" };
            string[] barangays = { "Barangay 1", "Barangay 2", "Barangay 3" };
            string[] sexes = { "Male", "Female" };

            Random rand = new Random();

            for (int i = 0; i < 50; i++) // 50 rows of fake data
            {
                int age = rand.Next(18, 90);
                DateTime birthDate = DateTime.Now.AddYears(-age).AddDays(rand.Next(0, 365));
                DateTime validatedDate = DateTime.Now.AddDays(-rand.Next(0, 365));

                string batchCode = $"BATCH-{rand.Next(1000, 9999)}";

                beneficiaries_table.Rows.Add(
                    batchCode,
                    age.ToString(),
                    birthDate.ToShortDateString(),
                    sexes[rand.Next(sexes.Length)],
                    regions[rand.Next(regions.Length)],
                    provinces[rand.Next(provinces.Length)],
                    municipalities[rand.Next(municipalities.Length)],
                    barangays[rand.Next(barangays.Length)],
                    validatedDate.ToShortDateString(),
                    rand.Next(0, 2) == 1 ? "Yes" : "No", // PWD
                    rand.Next(0, 2) == 1 ? "Yes" : "No"  // IP
                );
            }
        }



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
                string region = row.Cells["region_col"].Value?.ToString() ?? "";
                string isPwd = row.Cells["pwd_col"].Value?.ToString() ?? "";
                string isIp = row.Cells["ip_col"].Value?.ToString() ?? "";

                if (sex == "Male") male++;
                else if (sex == "Female") female++;

                if (isPwd == "Yes") pwd++;
                if (isIp == "Yes") ip++;

                switch (region)
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
            maleCount.Text = male.ToString();
            femaleCount.Text = female.ToString();
            pwdCount.Text = pwd.ToString();
            ipCount.Text = ip.ToString();
            zsCount.Text = zs.ToString();

            int total =
            zds + zc + zdn + isabela + zs +
            male + female + pwd + ip;


            totalCount.Text = total.ToString();

            // Optional: update total beneficiaries count
            beneficiaries_total_count.Text = beneficiaries_table.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Count()
                .ToString();
        }


        // SHOWING BENEFICIARIES BDAY


        private void SummaryGraph_MouseClick(object sender, MouseEventArgs e)
        {
        }

    }
}
