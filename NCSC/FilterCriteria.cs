using System;

namespace NCSC
{
    public class FilterCriteria
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string Province { get; set; }
        public string Municipality { get; set; }
        public string Status { get; set; }
        public bool? IsIP { get; set; }
        public bool? IsPWD { get; set; }

        // Check if filters are active (not default values)
        public bool IsActive()
        {
            // Default values:
            // MinAge: 60, MaxAge: 100
            // Province: "All Provinces" or null
            // Municipality: "All Municipalities" or null
            // Status: null or empty (first item in dropdown, but we treat it as "All")
            // IsIP: false or null
            // IsPWD: false or null

            bool hasAgeFilter = (MinAge.HasValue && MinAge.Value != 60) || (MaxAge.HasValue && MaxAge.Value != 100);
            bool hasProvinceFilter = !string.IsNullOrEmpty(Province) && Province != "All Provinces";
            bool hasMunicipalityFilter = !string.IsNullOrEmpty(Municipality) && Municipality != "All Municipalities";
            // Status is considered active only if it's explicitly set (not null/empty)
            // Note: The filter window dropdown doesn't have an "All" option, so if Status is set to the first item,
            // it's still a valid filter. We'll only consider it inactive if Status is null/empty.
            bool hasStatusFilter = !string.IsNullOrEmpty(Status);
            bool hasIPFilter = IsIP.HasValue && IsIP.Value == true;
            bool hasPWDFilter = IsPWD.HasValue && IsPWD.Value == true;

            return hasAgeFilter || hasProvinceFilter || hasMunicipalityFilter || hasStatusFilter || hasIPFilter || hasPWDFilter;
        }

        // Get default filter criteria
        public static FilterCriteria GetDefault()
        {
            return new FilterCriteria
            {
                MinAge = 60,
                MaxAge = 100,
                Province = "All Provinces",
                Municipality = "All Municipalities",
                Status = null,
                IsIP = false,
                IsPWD = false
            };
        }
    }
}

