using System.ComponentModel.DataAnnotations;

namespace CCDDemoExample.Models
{
    public class Patient
    {
        [Key]
        public string Id { get; set; }

        // Name fields
        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        // Birth and Gender
        public string BirthTime { get; set; }
        public string Gender { get; set; } // "F" for Female, "M" for Male

        // Address details
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Additional fields
        public string MaritalStatus { get; set; } // Married, Single, etc.
        public string ReligiousAffiliation { get; set; }
        public string Race { get; set; } // Race code (e.g., White, Italian)
        public string EthnicGroup { get; set; } // Ethnic Group code (e.g., Not Hispanic or Latino)

        // Guardian details
        public string GuardianCode { get; set; } // Guardian relationship code
        public string GuardianName { get; set; } // Guardian's name
        public string GuardianAddress { get; set; }
        public string GuardianCity { get; set; }
        public string GuardianState { get; set; }
        public string GuardianPostalCode { get; set; }
        public string GuardianCountry { get; set; }
        public string GuardianTelecom { get; set; } // Phone number of the guardian
    }
}
