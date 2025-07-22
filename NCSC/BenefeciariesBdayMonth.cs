using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCSC
{
    public partial class BenefeciariesBdayMonth : Form
    {

        public BenefeciariesBdayMonth(string month, List<string> batchCodes)
        {
            InitializeComponent();
            LoadBeneficiariesForMonthAsync(batchCodes);
            header.Text = $"Month of {month}";
        }

        private async void LoadBeneficiariesForMonthAsync(List<string> batchCodes)
        {
            beneficiaries_bday_month_table.Rows.Clear();
            // Columns are already defined in Designer, just clear rows
            var allBeneficiaries = await FirebaseHelper.GetDataAsync<Dictionary<string, Beneficiary>>("beneficiaries");
            if (allBeneficiaries != null)
            {
                foreach (var entry in allBeneficiaries.Values)
                {
                    if (batchCodes.Contains(entry.batch_code))
                    {
                        beneficiaries_bday_month_table.Rows.Add(
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
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}