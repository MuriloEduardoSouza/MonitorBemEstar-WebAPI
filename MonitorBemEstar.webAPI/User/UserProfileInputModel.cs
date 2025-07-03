using System.ComponentModel.DataAnnotations;

namespace MonitorBemEstar.webAPI.User;

public class UserProfileInputModel
{
    [Required(ErrorMessage="Nome completo é obrigatório.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Idade é obrigatória.")]
    public int Idade { get; set; }

    [Required(ErrorMessage = "Endereço é obrigatório.")]
    public string Endereco { get; set; } = string.Empty;
}

