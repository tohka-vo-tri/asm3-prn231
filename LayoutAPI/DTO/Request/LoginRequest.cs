using System.ComponentModel;

namespace LayoutAPI.DTO.Request
{
    public class LoginRequest
    {
        [DefaultValue("example@example.com")] // Set default value for Email
        public string Email { get; set; } = string.Empty;

        [DefaultValue("Password123")] // Set default value for Password
        public string Password { get; set; } = string.Empty;
    }
}
