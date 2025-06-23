using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonitorBemEstar.webAPI.Models;
using MonitorBemEstar.webAPI.Services;
using MonitorBemEstar.webAPI.User;

namespace MonitorBemEstar.webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly TokenService _tokenService;

        public AccountsController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrarUsuarioInputModel input)
        {
            var user = new Usuario
            {
                NomeCompleto = input.NomeCompleto,
                UserName = input.Email,
                Email = input.Email,
                Idade = input.Idade,
                Endereco = input.Endereco
            };

            var result = await _userManager.CreateAsync(user, input.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Usuario");

            return Ok(new { message = "Usuário criado com sucesso!" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user == null)
                return Unauthorized("Usuário não encontrado.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, input.Senha, false);

            if (!result.Succeeded)
                return Unauthorized("Email ou senha inválidos.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegistrarAdminInputModel input)
        {
            var user = new Usuario
            {
                NomeCompleto = input.NomeCompleto,
                UserName = input.Email,
                Email = input.Email,
                Idade = input.Idade,
                Endereco = input.Endereco
            };

            var result = await _userManager.CreateAsync(user, input.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { message = "Administrador criado com sucesso!" });
        }
    }
}
