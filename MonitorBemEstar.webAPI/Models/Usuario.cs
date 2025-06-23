using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MonitorBemEstar.webAPI.Models
{
    public class Usuario : IdentityUser
    {
        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(50)]
        public string? NomeCompleto { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "A idade deve estar entre 0 e 120.")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "O campo Endereço é obrigatório.")]
        public string? Endereco { get; set; } 
    }
}
