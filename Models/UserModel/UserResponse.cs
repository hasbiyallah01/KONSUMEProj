using DaticianProj.Core.Domain.Entities;
using DaticianProj.Core.Domain.Enum;

namespace DaticianProj.Models.UserModel
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public int Height { get; set; } = default!;
        public int Weight { get; set; } = default!;
        public string UserGoal { get; set; } = default!;
        public Role Role { get; set; }
    }

    public class LoginResponseModel : BaseResponse
    {

        public string Token { get; set; }

        public UserResponse Data { get; set; }


    }


    public class GoogleUser
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
