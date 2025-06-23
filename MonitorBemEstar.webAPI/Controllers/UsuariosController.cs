using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorBemEstar.webAPI.Context;
using MonitorBemEstar.webAPI.Models;
using MonitorBemEstar.webAPI.User;

namespace MonitorBemEstar.webAPI.Controllers
{
    [Route("api/usuarios")]
    public class UsuariosController : Controller
    {
        private readonly MeuDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UsuariosController(MeuDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            return Ok(usuarios);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            Usuario? usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return NotFound();

            return usuario;
        }


        [HttpPost]
        public async Task<ActionResult> CriarUsuario([FromBody] RegistrarUsuarioInputModel input)
        {
            Usuario usuario = new ()
            {
                UserName = input.UserName,
                Email = input.Email,
                NomeCompleto = input.NomeCompleto,
                Idade = input.Idade,
                Endereco = input.Endereco
            };


            IdentityResult result = await _userManager.CreateAsync(usuario, input.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return NotFound();

            IdentityResult result = await _userManager.DeleteAsync(usuario);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}


