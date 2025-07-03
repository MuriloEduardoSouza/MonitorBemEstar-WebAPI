using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MonitorBemEstar.webAPI.Models.Enum;

namespace MonitorBemEstar.webAPI.Models
{
    public class RegistroDiario
    {
        public int Id { get; set; }

        [Required]
        public double HorasCelular { get; set; }

        [Required]
        public DateTime DataRegistro { get; set; }

        [Required]
        public EnumHumor Humor { get; set; }

        [Required]
        public EnumTipoAtividade TipoAtividade { get; set; }

        //Relacionando usuario com Registro Diário
        [Required]
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public Usuario? Usuario { get; set; }
        
    }
}
