using Microsoft.EntityFrameworkCore;
public class ApplicationContext : DbContext
{
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<JWT_A_T> JWT_A_T { get; set; } = null!;
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
}