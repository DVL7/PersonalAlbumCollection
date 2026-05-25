using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.Models.Enums;

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

        // Define FK
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
        
        // Validation
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

        // Seeders
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Rock" },
            new Genre { Id = 2, Name = "Pop" },
            new Genre { Id = 3, Name = "Rap" },
            new Genre { Id = 4, Name = "Hip Hop" },
            new Genre { Id = 5, Name = "Jazz" },
            new Genre { Id = 6, Name = "Blues" },
            new Genre { Id = 7, Name = "Metal" }
        );

        modelBuilder.Entity<Artist>().HasData(
            new Artist
            {
                Id = 1,
                Name = "Michael Jackson",
                ArtistType = ArtistType.SoloArtist,
                Country = "USA",
                Description = "Krol popu, znany z albumu \"Thriller\" i widowiskowych wystepow.",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Artist
            {
                Id = 2,
                Name = "The Beatles",
                ArtistType = ArtistType.Band,
                Country = "Wielka Brytania",
                Description = "Legendarny zespol z Liverpoolu, ktory zrewolucjonizowal muzyke lat 60.",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Artist
            {
                Id = 3,
                Name = "Freddie Mercury",
                ArtistType = ArtistType.SoloArtist,
                Country = "Wielka Brytania",
                Description = "Charyzmatyczny wokalista Queen i autor wielu klasycznych utworow.",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Album>().HasData(
            new Album
            {
                Id = 1,
                Title = "Thriller",
                ReleaseYear = 1982,
                Description = "Ikoniczny album popowy, uznawany za jeden z najlepiej sprzedajacych sie w historii.",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Album
            {
                Id = 2,
                Title = "Abbey Road",
                ReleaseYear = 1969,
                Description = "Klasyczny album The Beatles, znany m.in. z utworu \"Come Together\".",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Album
            {
                Id = 3,
                Title = "Mr. Bad Guy",
                ReleaseYear = 1985,
                Description = "Solowy album Freddie'go Mercury'ego, prezentujacy jego indywidualny styl.",
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<AlbumArtist>().HasData(
            new AlbumArtist { AlbumId = 1, ArtistId = 1, DisplayOrder = 1 },
            new AlbumArtist { AlbumId = 2, ArtistId = 2, DisplayOrder = 1 },
            new AlbumArtist { AlbumId = 3, ArtistId = 3, DisplayOrder = 1 }
        );
    }
}