using System.ComponentModel.DataAnnotations;

namespace Profile.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        public List<string> Hobbies { get; set; }

        public Guid UserID { get; set; }
        public User User { get; set; }
    }
}