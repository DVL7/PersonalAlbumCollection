using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Models.Entities;

namespace PersonalAlbumCollection.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumArtist> AlbumArtists => Set<AlbumArtist>();
    public DbSet<AlbumGenre> AlbumGenres => Set<AlbumGenre>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<UserAlbum> UserAlbums => Set<UserAlbum>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // DEFINIOWANIE FK 
        //  AlbumArtist 
        modelBuilder.Entity<AlbumArtist>()
            .HasKey(x => new { x.AlbumId, x.ArtistId });
        
        modelBuilder.Entity<AlbumArtist>()
            .HasOne(x => x.Album)
            .WithMany(x => x.AlbumArtists)
            .HasForeignKey(x => x.AlbumId);
        
        modelBuilder.Entity<AlbumArtist>()
            .HasOne(x => x.Artist)
            .WithMany(x => x.AlbumArtists)
            .HasForeignKey(x => x.ArtistId);
        
        // AlbumGenre  
        modelBuilder.Entity<AlbumGenre>()
            .HasKey(x => new { x.AlbumId, x.GenreId });
        
        modelBuilder.Entity<AlbumGenre>()
            .HasOne(x => x.Album)
            .WithMany(x => x.AlbumGenres)
            .HasForeignKey(x => x.AlbumId);
        
        modelBuilder.Entity<AlbumGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.AlbumGenres)
            .HasForeignKey(x => x.GenreId);
        
        // UserAlbum 
        modelBuilder.Entity<UserAlbum>()
            .HasKey(x => new { x.UserId, x.AlbumId });
        
        modelBuilder.Entity<UserAlbum>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserAlbums)
            .HasForeignKey(x => x.UserId);
        
        modelBuilder.Entity<UserAlbum>()
            .HasOne(x => x.Album)
            .WithMany(x => x.UserAlbums)
            .HasForeignKey(x => x.AlbumId);
        
        // WALIDACJA PÓL
        // Album 
        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);
            
            // ReleaseYear null lub w przedziale 1000-9999 
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Album_ReleaseYear_4Digits",
                "\"ReleaseYear\" IS NULL OR (\"ReleaseYear\" BETWEEN 1000 AND 9999)"));
            
            entity.Property(x => x.CoverUrl)
                .HasMaxLength(500);
            
            entity.Property(x => x.Description)
                .HasMaxLength(2000);
        });
        
        // ApplicationUser 
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(x => x.UserName)
                .IsUnique();

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(120);
            
            entity.HasIndex(x => x.Email)
                .IsUnique();
            
            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
        });
        
        // Artist 
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(x => x.Country)
                .HasMaxLength(50);
            
            entity.Property(x => x.Description)
                .HasMaxLength(2000);
        });
        
        // Genre
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
        
        // UserAlbum
        modelBuilder.Entity<UserAlbum>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_UserAlbum_Score_1_10",
                "\"Score\" IS NULL OR (\"Score\" BETWEEN 1 AND 10)"
            ));
            
            entity.Property(x => x.Review)
                .HasMaxLength(3000);
                
        });


    }
}