using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, TokenService tokenService, IConfiguration configuration  )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
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
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Senha))
                return Unauthorized("Usuário ou senha inválidos.");

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minhaChaveSuperSecreta1234567890ABCDEF"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration ["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiracao = token.ValidTo
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/register-admin")]
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

            var token = _tokenService.GenerateToken(user);

            return Ok(new { message = "Administrador criado com sucesso!",
            token = token});
        }
    }
}
