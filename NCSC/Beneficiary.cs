public class Beneficiary
{
    public string batch_code { get; set; }
    public string age { get; set; }
    public string birth_date { get; set; }
    public string sex { get; set; }
    public string region { get; set; }
    public string province { get; set; }
    public string municipality { get; set; }
    public string barangay { get; set; }
    public string date_validated { get; set; }
    public string pwd { get; set; }
    public string ip { get; set; }
    
    // New fields from updated data structure
    public string name { get; set; }
    public string first_name { get; set; }
    public string middle_name { get; set; }
    public string last_name { get; set; }
    public string province_municipality_date { get; set; }
    public int batch_number { get; set; }
    
    // New boolean fields
    public bool TotalEndorseFromLGUs { get; set; } = true; // Default to true
    public bool Assessed { get; set; } = false;
    public bool TotalValidated { get; set; } = false;
    public bool TotalEndorsedToNCSCO { get; set; } = false;
    public bool TotalCleanedListFromNCSCO { get; set; } = false;
    public bool ScheduledPayout { get; set; } = false;
    public bool NumberOfApplicantsReceivedCashGift { get; set; } = false;
    public bool Deceased { get; set; } = false;
    public bool Unpaid { get; set; } = false;
    public bool Paid { get; set; } = false;

    // Constructor to ensure proper initialization
    public Beneficiary()
    {
        // Ensure boolean fields have proper default values
        TotalEndorseFromLGUs = true; // Default to true for new beneficiaries
        Assessed = false;
        TotalValidated = false;
        TotalEndorsedToNCSCO = false;
        TotalCleanedListFromNCSCO = false;
        ScheduledPayout = false;
        NumberOfApplicantsReceivedCashGift = false;
        Deceased = false;
        Unpaid = false;
        Paid = false;
    }

    // Method to normalize sex field format for display
    public string GetNormalizedSex()
    {
        if (string.IsNullOrEmpty(sex)) return "";
        
        switch (sex.ToUpper().Trim())
        {
            case "M":
            case "MALE":
                return "Male";
            case "F":
            case "FEMALE":
                return "Female";
            default:
                return sex; // Return original if not recognized
        }
    }

    // Method to normalize date format for display
    public string GetNormalizedBirthDate()
    {
        if (string.IsNullOrEmpty(birth_date)) return "";
        
        // Try to parse the date and format it consistently
        if (DateTime.TryParse(birth_date, out DateTime parsedDate))
        {
            return parsedDate.ToString("MM-dd-yyyy");
        }
        
        return birth_date; // Return original if parsing fails
    }

    // Method to normalize date validated format for display
    public string GetNormalizedDateValidated()
    {
        if (string.IsNullOrEmpty(date_validated)) return "";
        
        // Try to parse the date and format it consistently
        if (DateTime.TryParse(date_validated, out DateTime parsedDate))
        {
            return parsedDate.ToString("MM-dd-yyyy");
        }
        
        return date_validated; // Return original if parsing fails
    }

    // Method to get the birth month name for filtering
    public string GetBirthMonth()
    {
        if (string.IsNullOrEmpty(birth_date)) return "";
        
        if (DateTime.TryParse(birth_date, out DateTime parsedDate))
        {
            return parsedDate.ToString("MMMM");
        }
        
        return ""; // Return empty if parsing fails
    }
}
