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
    
    // New boolean fields
    public bool TotalEndorseFromLGUs { get; set; } = true; // Default to true
    public bool Assessed { get; set; } = false;
    public bool ScheduleValidation { get; set; } = false;
    public bool TotalValidated { get; set; } = false;
    public bool TotalEndorsedToNCSCO { get; set; } = false;
    public bool TotalCleanedListFromNCSCO { get; set; } = false;
    public bool ScheduledPayout { get; set; } = false;
    public bool NumberOfApplicantsReceivedCashGift { get; set; } = false;
}
