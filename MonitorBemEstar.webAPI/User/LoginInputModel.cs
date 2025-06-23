using System.ComponentModel.DataAnnotations;

namespace MonitorBemEstar.webAPI.User
{
    public class LoginInputModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Senha { get; set; }
    }
}
