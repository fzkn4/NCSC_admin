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

        // Store filter criteria
        private FilterCriteria currentFilterCriteria = FilterCriteria.GetDefault();

        // Track if status dropdown was changed by user
        private bool statusDropdownChanged = false;

        // Public property to get filter criteria
        public FilterCriteria FilterCriteria => currentFilterCriteria;

        public Beneficiaries_filter_window(FilterCriteria existingCriteria = null)
        {
            InitializeComponent();

            // If existing criteria provided, load it
            if (existingCriteria != null)
            {
                currentFilterCriteria = existingCriteria;
            }
        }

        private void Beneficiaries_filter_window_Load(object sender, EventArgs e)
        {
            // Populate province dropdown
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("All Provinces");
            foreach (string province in provinceMunicipalities.Keys)
            {
                guna2ComboBox1.Items.Add(province);
            }

            // Load existing filter criteria or use defaults
            if (currentFilterCriteria.MinAge.HasValue)
                start_age_trackbar.Value = currentFilterCriteria.MinAge.Value;
            else
                start_age_trackbar.Value = 60;

            if (currentFilterCriteria.MaxAge.HasValue)
                end_age_trackbar.Value = currentFilterCriteria.MaxAge.Value;
            else
                end_age_trackbar.Value = 100;

            // Update age labels
            start_age.Text = start_age_trackbar.Value.ToString();
            label7.Text = end_age_trackbar.Value.ToString();

            // Set province
            if (!string.IsNullOrEmpty(currentFilterCriteria.Province))
            {
                int provinceIndex = guna2ComboBox1.Items.IndexOf(currentFilterCriteria.Province);
                if (provinceIndex >= 0)
                    guna2ComboBox1.SelectedIndex = provinceIndex;
                else
                    guna2ComboBox1.SelectedIndex = 0;
            }
            else
            {
                guna2ComboBox1.SelectedIndex = 0;
            }

            // Municipality will be populated by the SelectedIndexChanged event
            // But we need to manually trigger it if province is already set
            if (guna2ComboBox1.SelectedIndex > 0)
            {
                UpdateMunicipalityDropdown();
            }

            // Set municipality
            if (!string.IsNullOrEmpty(currentFilterCriteria.Municipality))
            {
                int municipalityIndex = guna2ComboBox2.Items.IndexOf(currentFilterCriteria.Municipality);
                if (municipalityIndex >= 0)
                    guna2ComboBox2.SelectedIndex = municipalityIndex;
                else
                    guna2ComboBox2.SelectedIndex = 0;
            }
            else
            {
                guna2ComboBox2.Items.Clear();
                guna2ComboBox2.Items.Add("All Municipalities");
                guna2ComboBox2.SelectedIndex = 0;
            }

            // Set status - if status is null or empty, select index 0 but treat it as "no filter"
            // We'll check if status exists in the dropdown, otherwise set to 0
            if (!string.IsNullOrEmpty(currentFilterCriteria.Status))
            {
                int statusIndex = guna2ComboBox3.Items.IndexOf(currentFilterCriteria.Status);
                if (statusIndex >= 0)
                    guna2ComboBox3.SelectedIndex = statusIndex;
                else
                    guna2ComboBox3.SelectedIndex = 0;
            }
            else
            {
                // If no status filter is set, select index 0 (which will be treated as "no filter" when submitting)
                guna2ComboBox3.SelectedIndex = 0;
                statusDropdownChanged = false; // Reset flag since this is the default
            }

            // Track status dropdown changes
            guna2ComboBox3.SelectedIndexChanged += (s, e) => { statusDropdownChanged = true; };

            // Set checkboxes
            ip_checkbox.Checked = currentFilterCriteria.IsIP ?? false;
            pwd_checkbox.Checked = currentFilterCriteria.IsPWD ?? false;
        }

        private void UpdateMunicipalityDropdown()
        {
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
            // Also set DialogResult to OK so the cleared state is saved
            this.DialogResult = DialogResult.OK;
            this.Close();
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
            UpdateMunicipalityDropdown();

            // Reset status dropdown to first item (but we'll set Status to null in criteria)
            guna2ComboBox3.SelectedIndex = 0;
            statusDropdownChanged = false; // Reset flag since we're clearing

            // Reset checkboxes to unchecked state
            ip_checkbox.Checked = false;
            pwd_checkbox.Checked = false;

            // Update filter criteria to defaults
            currentFilterCriteria = FilterCriteria.GetDefault();
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
            UpdateMunicipalityDropdown();
        }

        private void submit_button_Click_1(object sender, EventArgs e)
        {
            // Update filter criteria from UI controls
            currentFilterCriteria.MinAge = start_age_trackbar.Value;
            currentFilterCriteria.MaxAge = end_age_trackbar.Value;

            string selectedProvince = guna2ComboBox1.SelectedItem?.ToString();
            currentFilterCriteria.Province = (selectedProvince == "All Provinces") ? null : selectedProvince;

            string selectedMunicipality = guna2ComboBox2.SelectedItem?.ToString();
            currentFilterCriteria.Municipality = (selectedMunicipality == "All Municipalities") ? null : selectedMunicipality;

            string selectedStatus = guna2ComboBox3.SelectedItem?.ToString();
            // If Status was null (default/cleared) and user hasn't changed the dropdown, keep Status as null.
            // Otherwise, save the selected status (even if it's index 0, as that means user explicitly selected it).
            if (currentFilterCriteria.Status == null && !statusDropdownChanged)
            {
                // Status was null and user didn't change dropdown, so keep it null (no status filter)
                currentFilterCriteria.Status = null;
            }
            else
            {
                // Status was set or user changed dropdown, so save the selected status
                currentFilterCriteria.Status = selectedStatus;
            }

            currentFilterCriteria.IsIP = ip_checkbox.Checked ? (bool?)true : null;
            currentFilterCriteria.IsPWD = pwd_checkbox.Checked ? (bool?)true : null;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
