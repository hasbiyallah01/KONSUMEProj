
using DaticianProj.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DaticianProj.Models.UserModel
{
    public class UserRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Height must be greater than 0.")]
        public int Height { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public int Weight { get; set; }

        [Required]
        public string UserGoal { get; set; }
    }

    public class LoginRequestModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class ChangePasswordRequest
    {
        public string Password { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }


}
