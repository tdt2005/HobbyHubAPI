namespace Profile.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Thuộc tính Profile để liên kết với UserProfile
        public UserProfile Profile { get; set; }
    }
}