public class LoginHistory
{
    public string username { get; set; }
    public string province { get; set; }
    public string municipality { get; set; }
    public string dateLogin { get; set; }
    public long timestamp { get; set; }

    // Method to format date for display
    public string GetFormattedDate()
    {
        if (string.IsNullOrEmpty(dateLogin)) return "";
        
        // Try to parse the ISO date string
        if (DateTime.TryParse(dateLogin, out DateTime parsedDate))
        {
            return parsedDate.ToString("MM-dd-yyyy HH:mm:ss");
        }
        
        return dateLogin; // Return original if parsing fails
    }
}

