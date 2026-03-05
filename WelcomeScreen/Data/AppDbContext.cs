using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Models;

namespace WelcomeScreen.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<WelcomeScreen.API.Models.WelcomeScreen> WelcomeScreens => Set<WelcomeScreen.API.Models.WelcomeScreen>();
    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<EventField> EventFields => Set<EventField>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<ScreenConfig> ScreenConfigs => Set<ScreenConfig>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // WelcomeScreen → User
        modelBuilder.Entity<WelcomeScreen.API.Models.WelcomeScreen>()
            .HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId);

        // DataSource → WelcomeScreen (1-1)
        modelBuilder.Entity<DataSource>()
            .HasOne(d => d.WelcomeScreen)
            .WithOne(w => w.DataSource)
            .HasForeignKey<DataSource>(d => d.WelcomeScreenId);

        // EventField → WelcomeScreen (1-nhiều)
        modelBuilder.Entity<EventField>()
            .HasOne(f => f.WelcomeScreen)
            .WithMany(w => w.Fields)
            .HasForeignKey(f => f.WelcomeScreenId);

        // MediaFile → WelcomeScreen (optional)
        modelBuilder.Entity<MediaFile>()
            .HasOne(m => m.WelcomeScreen)
            .WithMany()
            .HasForeignKey(m => m.WelcomeScreenId)
            .IsRequired(false);

        // ScreenConfig → WelcomeScreen (1-1)
        modelBuilder.Entity<ScreenConfig>()
            .HasOne(s => s.WelcomeScreen)
            .WithOne(w => w.ScreenConfig)
            .HasForeignKey<ScreenConfig>(s => s.WelcomeScreenId);

        // ScreenConfig → BackgroundMedia (optional)
        modelBuilder.Entity<ScreenConfig>()
            .HasOne(s => s.BackgroundMedia)
            .WithMany()
            .HasForeignKey(s => s.BackgroundMediaId)
            .IsRequired(false);

        // Encrypt connection string - đừng log ra
        modelBuilder.Entity<DataSource>()
            .Property(d => d.ConnectionString)
            .HasColumnName("ConnectionStringEncrypted");
    }
}