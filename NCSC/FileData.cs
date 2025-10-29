using System;

namespace NCSC
{
    public class FileData
    {
        public string batch_code { get; set; }
        public string municipality { get; set; }
        public string province { get; set; }
        public string province_municipality_date { get; set; }
        public int total_records { get; set; }
        public int unique_records { get; set; }
        public int duplicate_records { get; set; }
        public string upload_date { get; set; }
        public string file_name { get; set; }
        public string uploaded_by { get; set; }
        public int batch_number { get; set; }
        public int upload_year { get; set; }

        // Helper method to format the upload date for display
        public string GetFormattedUploadDate()
        {
            if (DateTime.TryParse(upload_date, out DateTime date))
            {
                return date.ToString("dd/MM/yyyy");
            }
            return upload_date ?? "N/A";
        }

        // Helper method to get the file name
        public string GetFileName()
        {
            return file_name ?? $"{batch_code}.csv";
        }
    }
}
