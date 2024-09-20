namespace CCDDemoExample.Models
{
    public class PatientRecord
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        // Add more properties as required
    }
}
