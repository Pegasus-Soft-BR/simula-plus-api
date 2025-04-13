namespace Domain.DTOs
{
    public class ChangeUserPasswordByHashCodeVM
    {
        public string HashCodePassword { get; set; }

        public string NewPassword { get; set; }
    }
}
