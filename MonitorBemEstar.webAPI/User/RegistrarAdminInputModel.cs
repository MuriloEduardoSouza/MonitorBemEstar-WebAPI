using System.ComponentModel.DataAnnotations;

namespace MonitorBemEstar.webAPI.User
{
    public class RegistrarAdminInputModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        public int Idade { get; set; }

        [Required]
        public string Endereco { get; set; } = string.Empty;
    }
}
