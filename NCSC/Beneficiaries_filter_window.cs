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
    public partial class Beneficiaries_filter_window : Form
    {
        // Province to Municipalities mapping
        private readonly Dictionary<string, List<string>> provinceMunicipalities = new Dictionary<string, List<string>>
        {
            { "Zamboanga del Sur", new List<string> { "Aurora", "Bayog", "Dimataling", "Dinas", "Dumalinao", "Dumingag", "Guipos", "Josefina", "Kumalarang", "Labangan", "Lakewood", "Lapuyan", "Mahayag", "Margosatubig", "Midsalip", "Molave", "Pagadian City", "Pitogo", "Ramon Magsaysay", "San Miguel", "San Pablo", "Sominot", "Tabina", "Tambulig", "Tigbao", "Tukuran", "Vincenzo A. Sagun" } },
            { "Zamboanga del Norte", new List<string> { "Baliguian", "Godod", "Gutalac", "Jose Dalman", "Kalawit", "Katipunan", "La Libertad", "Labason", "Leon B. Postigo", "Liloy", "Manukan", "Mutia", "Piñan", "Polanco", "President Manuel A. Roxas", "Rizal", "Salug", "Sergio Osmeña Sr.", "Siayan", "Sibuco", "Sibutad", "Sindangan", "Siocon", "Sirawai", "Tampilisan", "Dipolog City", "Dapitan City" } },
            { "Zamboanga Sibugay", new List<string> { "Alicia", "Buug", "Diplahan", "Imelda", "Ipil (capital)", "Kabasalan", "Mabuhay", "Malangas", "Naga", "Olutanga", "Payao", "Roseller Lim", "Siay", "Talusan", "Titay", "Tungawan" } },
            { "Zamboanga City", new List<string> { "Zamboanga City" } },
            { "Isabela City", new List<string> { "Isabela City" } }
        };

        public Beneficiaries_filter_window()
        {
            InitializeComponent();
        }

        private void Beneficiaries_filter_window_Load(object sender, EventArgs e)
        {
            // Initialize age labels to match trackbar values
            start_age.Text = start_age_trackbar.Value.ToString();
            label7.Text = end_age_trackbar.Value.ToString();
            
            // Populate province dropdown
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("All Provinces");
            foreach (string province in provinceMunicipalities.Keys)
            {
                guna2ComboBox1.Items.Add(province);
            }
            guna2ComboBox1.SelectedIndex = 0; // Select "All Provinces" by default

            // Clear municipality dropdown initially
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("All Municipalities");
            guna2ComboBox2.SelectedIndex = 0;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            ResetToDefaultValues();
        }

        private void ResetToDefaultValues()
        {
            // Reset age trackbars to default values
            start_age_trackbar.Value = 60;
            end_age_trackbar.Value = 100;
            
            // Update age labels to match trackbar values
            start_age.Text = "60";
            label7.Text = "100";
            
            // Reset province dropdown to "All Provinces"
            guna2ComboBox1.SelectedIndex = 0;
            
            // Reset municipality dropdown to "All Municipalities"
            guna2ComboBox2.SelectedIndex = 0;
            
            // Reset status dropdown to first item
            guna2ComboBox3.SelectedIndex = 0;
            
            // Reset checkboxes to unchecked state
            ip_checkbox.Checked = false;
            pwd_checkbox.Checked = false;
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            // Update start age label
            start_age.Text = start_age_trackbar.Value.ToString();
            
            // Ensure end age is not less than start age
            if (end_age_trackbar.Value < start_age_trackbar.Value)
            {
                end_age_trackbar.Value = start_age_trackbar.Value;
                label7.Text = end_age_trackbar.Value.ToString();
            }
        }

        private void end_age_trackbar_Scroll(object sender, ScrollEventArgs e)
        {
            // Ensure end age is not less than start age
            if (end_age_trackbar.Value < start_age_trackbar.Value)
            {
                end_age_trackbar.Value = start_age_trackbar.Value;
            }
            
            // Update end age label
            label7.Text = end_age_trackbar.Value.ToString();
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update municipality dropdown based on selected province
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("All Municipalities");

            if (guna2ComboBox1.SelectedItem != null && guna2ComboBox1.SelectedItem.ToString() != "All Provinces")
            {
                string selectedProvince = guna2ComboBox1.SelectedItem.ToString();
                if (provinceMunicipalities.ContainsKey(selectedProvince))
                {
                    foreach (string municipality in provinceMunicipalities[selectedProvince])
                    {
                        guna2ComboBox2.Items.Add(municipality);
                    }
                }
            }

            guna2ComboBox2.SelectedIndex = 0; // Select "All Municipalities" by default
        }
    }
}
