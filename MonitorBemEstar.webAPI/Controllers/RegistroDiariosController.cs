using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorBemEstar.webAPI.Context;
using MonitorBemEstar.webAPI.Models;
using MonitorBemEstar.webAPI.User;



namespace MonitorBemEstar.webAPI.Controllers
{
    [Route("api/registro-diarios")]
    public class RegistroDiariosController : Controller
    {
        private readonly MeuDbContext _context;

        public RegistroDiariosController(MeuDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroDiario>>> GetRegistroDiarios()
        {
            return await _context.RegistroDiarios.ToListAsync();
        }

        [Authorize]
        [HttpGet("meus-registros")]
        public async Task<ActionResult<IEnumerable<RegistroDiario>>> GetMeusRegistros()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var registros = await _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId)
                .ToListAsync();

            return registros;
        }

        [Authorize]
        [HttpGet("relatorio/evolucao-semanal")]
        public async Task<IActionResult> GetEvolucaoSemanal(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim
            )
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var registrosQuery =  _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId);

            if (dataInicio.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date <= dataFim.Value.Date);

            var registros = await registrosQuery.ToListAsync();

            if (!registros.Any())
                return Ok("Nenhum registro encontrado no período selecionado.");

            var resultado = registros
                .GroupBy(itemLista =>
                {
                    var data = itemLista.DataRegistro.Date;

                    int diferenca = data.DayOfWeek - DayOfWeek.Monday;
                    if (diferenca < 0) diferenca += 7;

                    var inicioSemana = data.AddDays(-diferenca);
                    var fimSemana = inicioSemana.AddDays(6);

                    return new { Inicio = inicioSemana, Fim = fimSemana };
                })

                .Select(grupoFormado => new
                {
                    Semana = $"{grupoFormado.Key.Inicio:dd/MM/yyyy} até {grupoFormado.Key.Fim:dd/MM/yyyy}",
                    Horas = grupoFormado.Sum(itemLista => itemLista.HorasCelular)
                })
                .OrderBy(itemLista => itemLista.Semana)
                .ToList();

            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("relatorio/evolucao-diaria")]
        public async Task<IActionResult> GetEvolucaoDiaria(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var registrosQuery =  _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId);


            if (dataInicio.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date <= dataFim.Value.Date);

            var registros = await registrosQuery.ToListAsync();

            if (!registros.Any())
                return Ok("Nenhum registro encontrado no período selecionado.");

            var resultado = registros
                .GroupBy(itemLista => itemLista.DataRegistro.Date)

                //Agora vamos pegar os grupos formados
                .Select(grupoFormado => new
                {
                    Data = grupoFormado.Key.ToString("dd/MM/yyyy"),
                    Horas = grupoFormado.Sum(itemLista => itemLista.HorasCelular)
                })
                .OrderBy(itemLista => itemLista.Data)
                .ToList();

            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("relatorio/horas-celular")]
        public async Task<IActionResult> GetRelatorioHorasCelular()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            DateTime hoje = DateTime.Today;
            DateTime dataSemana = hoje.AddDays(-7);
            DateTime dataMes = new DateTime(hoje.Year, hoje.Month, 1);
      
            var horasSemana = await _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId && itemLista.DataRegistro >= dataSemana)
                .SumAsync(itemLista => itemLista.HorasCelular);
            
            var horasMes = await _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId && itemLista.DataRegistro >= dataMes)
                .SumAsync(itemLista => itemLista.HorasCelular);

            return Ok(new
            {
                HorasUltimaSemana = horasSemana,
                HorasMesAtual = horasMes
            });
        }

        [Authorize]
        [HttpGet("relatorio/humor")]
        public async Task<IActionResult> GetRelatorioHumor(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            //var dataMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var registrosQuery = _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId);

            if (dataInicio.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date <= dataFim.Value.Date);


            var registros = await registrosQuery.ToListAsync();

            if (!registros.Any())
                return Ok("Nenhum registro encontrado no período selecionado.");


            var resultado = registros
                .GroupBy(itemLista => itemLista.Humor)
                .Select(grupoFormado => new
                {
                    Humor = grupoFormado.Key.ToString(),
                    Quantidade = grupoFormado.Count()
                })
                .ToList();

            return Ok(resultado);
        }
        
        [Authorize]
        [HttpGet("relatorio/atividades")]
        public async Task<IActionResult> GetRelatorioAtividades(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            //DateTime dataMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var registrosQuery = _context.RegistroDiarios
                .Where(itemLista => itemLista.UserId == userId);

            if (dataInicio.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                registrosQuery = registrosQuery.Where(itemLista => itemLista.DataRegistro.Date <= dataFim.Value.Date);

            var registros = await registrosQuery.ToListAsync();

            if (!registros.Any())
                return Ok("Nenhum registro encontrado no filtro selecionado.");

            var resultado = registros
                .GroupBy(itemLista => itemLista.TipoAtividade)
                .Select(grupoFormado => new
                {
                    Atividade = grupoFormado.Key.ToString(),
                    Quantidade = grupoFormado.Count()
                })
                .ToList();

            return Ok(resultado);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RegistroDiario>> PostRegistroDiario([FromBody] RegistroDiario input)
        {
            var userId = User.FindFirst("sub")?.Value;

            RegistroDiario registro = new()
            {
                HorasCelular = input.HorasCelular,
                DataRegistro = input.DataRegistro,
                Humor = input.Humor,
                TipoAtividade = input.TipoAtividade,
                UserId = userId
            };

            _context.RegistroDiarios.Add(registro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegistroDiarios), new { id = registro.Id }, registro);
        }

        [Authorize]
        [HttpPut("meu/{id}")]
        public async Task<IActionResult> AtualizarRegistro(int id, [FromBody] RegistroDiario input)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            RegistroDiario? registro = await _context.RegistroDiarios.FirstOrDefaultAsync(register => register.Id == id);

            if (registro == null)
                return NotFound("Registro não encontrado.");

            if (registro.UserId != userId)
                return Forbid("Você não tem permissão para editar este registro.");
     
            registro.HorasCelular = input.HorasCelular;
            registro.DataRegistro = input.DataRegistro;
            registro.Humor = input.Humor;
            registro.TipoAtividade = input.TipoAtividade;

            await _context.SaveChangesAsync();

            return Ok(registro);
        }

        [Authorize]
        [HttpPatch("meu/{id}")]
        public async Task<IActionResult> PatchMeuRegistro(int id, [FromBody] AtualizarRegistroInputModel input)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            RegistroDiario? registro = await _context.RegistroDiarios.FirstOrDefaultAsync(register => register.Id == id);

            if (registro == null)
                return NotFound("Registro não encontrado.");

            if (registro.UserId != userId)
                return Forbid("Você não tem permissão para alterar este registro.");

            if (input.HorasCelular.HasValue)
                registro.HorasCelular = input.HorasCelular.Value;

            if (input.DataRegistro.HasValue)
                registro.DataRegistro = input.DataRegistro.Value;

            if (input.Humor.HasValue)
                registro.Humor = input.Humor.Value;

            if (input.TipoAtividade.HasValue)
                registro.TipoAtividade = input.TipoAtividade.Value;

            await _context.SaveChangesAsync();

            return Ok(registro);
        }


        [Authorize]
        [HttpDelete("meu/{id}")]
        public async Task<IActionResult> DeleteMeuRegistro(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            RegistroDiario? registro = await _context.RegistroDiarios.FirstOrDefaultAsync(register => register.Id == id);

            if (registro == null)
                return NotFound("Registro não encontrado");

            if (registro.UserId != userId)
                return Forbid("Voce não tem permissão para excluir este registro.");

            _context.RegistroDiarios.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistro(int id)
        {
            RegistroDiario? registro = await _context.RegistroDiarios.FindAsync(id);

            if (registro == null)
                return NotFound("Registro não encontrado.");

            _context.RegistroDiarios.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}


