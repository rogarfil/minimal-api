using Microsoft.EntityFrameworkCore;
using rogarfil_api.Models;  // Alterar namespace

namespace rogarfil_api.Data;  // Alterar namespace

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
}