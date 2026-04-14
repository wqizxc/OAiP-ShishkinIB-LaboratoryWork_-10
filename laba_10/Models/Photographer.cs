namespace laba_10.Models
{
    public class Photographer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PhotographyDirection { get; set; } = null!;
        public DateTime FirstPublicationDate { get; set; }
        public string Role { get; set; } = "User"; 
    }
}