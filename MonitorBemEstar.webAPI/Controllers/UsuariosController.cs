using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorBemEstar.webAPI.Context;
using MonitorBemEstar.webAPI.Models;
using MonitorBemEstar.webAPI.User;
using System.Security.Claims;

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

            var usuarios =  _userManager.Users.ToList();
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

        [HttpGet("me")] 
        [Authorize] 
        public async Task<ActionResult<Usuario>> GetCurrentUser()
        {
        
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userEmail))
        {
                return Unauthorized(new { message = "Email do usuário não encontrado no token. Certifique-se de que o token é válido e possui a claim de Email." });
            }

        Usuario? usuario = await _userManager.FindByEmailAsync(userEmail);

        if (usuario == null)
        {
                return NotFound(new { message = "Usuário não encontrado." });
            }

        return Ok(usuario); 
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

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserProfileInputModel input)
        {
            var userEmailFromToken = User.FindFirstValue(ClaimTypes.Email);

            if(string.IsNullOrEmpty(userEmailFromToken))
            {
                return Unauthorized(new { message = "ID de usuário não encontrado no token." });
            }

            var user = await _userManager.FindByEmailAsync(userEmailFromToken);

            if (user == null)
            {
                return NotFound(new {message = "Usuário não encontrado no sistema."});
            }

            user.NomeCompleto = input.NomeCompleto;
            user.Idade = input.Idade;
            user.Endereco = input.Endereco;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Perfil atualizado com sucesso." });
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


