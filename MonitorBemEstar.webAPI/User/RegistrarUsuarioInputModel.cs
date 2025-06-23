using System.ComponentModel.DataAnnotations;

namespace MonitorBemEstar.webAPI.User
{
    public class RegistrarUsuarioInputModel
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

        [Range(0, 120, ErrorMessage = "A idade deve estar entre 0 e 120.")]
        [Required]
        public int Idade { get; set; }

        [Required]
        public string Endereco { get; set; } = string.Empty;
    }
}
