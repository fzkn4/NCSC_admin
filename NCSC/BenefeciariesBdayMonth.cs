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
        private string targetMonth;

        public BenefeciariesBdayMonth(string month, List<string> batchCodes)
        {
            InitializeComponent();
            targetMonth = month;
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
                    // First check if the batch code is in the list
                    if (batchCodes.Contains(entry.batch_code))
                    {
                        // Then check if the birth month matches the target month
                        if (entry.GetBirthMonth() == targetMonth)
                        {
                            beneficiaries_bday_month_table.Rows.Add(
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
                                entry.ip
                            );
                        }
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