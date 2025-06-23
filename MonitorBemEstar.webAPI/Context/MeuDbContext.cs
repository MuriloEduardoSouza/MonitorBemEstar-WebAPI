using Microsoft.EntityFrameworkCore;
using MonitorBemEstar.webAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MonitorBemEstar.webAPI.Context
{
    public class MeuDbContext : IdentityDbContext<Usuario>
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options) { }

       
        public DbSet<RegistroDiario> RegistroDiarios { get; set; }
    }
}
