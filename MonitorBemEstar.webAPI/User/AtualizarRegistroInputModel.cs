using MonitorBemEstar.webAPI.Models;
using MonitorBemEstar.webAPI.Models.Enum;

namespace MonitorBemEstar.webAPI.User
{
    public class AtualizarRegistroInputModel
    {
        public double? HorasCelular { get; set; }
        public DateTime? DataRegistro { get; set; }
        public EnumHumor? Humor { get; set; }
        public EnumTipoAtividade? TipoAtividade { get; set; }
    }
}
