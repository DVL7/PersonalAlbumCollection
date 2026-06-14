// AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Models.Entities;

namespace PersonalAlbumCollection.Data;

/// <summary>
/// Kontekst bazy danych aplikacji dla EF Core.
/// Dziedziczy po DbContext.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Konstruktor przyjmujący opcje konfiguracyjne.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Definicje tabel w bazie danych
    
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumArtist> AlbumArtists => Set<AlbumArtist>();
    public DbSet<AlbumGenre> AlbumGenres => Set<AlbumGenre>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Genre> Genres => Set<Genre>();

    /// <summary>
    /// Konfiguracje modeli EF Core.
    /// Wywoływana po utworzeniu kontekstu bazy danych.
    /// Definiuje klucze złożone, relacje, ograniczenia i indeksy dla encji.
    /// </summary>
    /// <param name="modelBuilder">Obiekt dostarczany przez EF Core służący do konfiguracji modelu danych.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // inicjalizacja
        base.OnModelCreating(modelBuilder);

        // AlbumArtist
        // Klucz złożony z AlbumId i ArtistId.
        modelBuilder.Entity<AlbumArtist>()
            .HasKey(x => new { x.AlbumId, x.ArtistId });

        // Album może mieć wielu artystów.
        modelBuilder.Entity<AlbumArtist>()
            .HasOne(x => x.Album)
            .WithMany(x => x.AlbumArtists)
            .HasForeignKey(x => x.AlbumId);

        // Artysta może być przypisany do wielu albumów. 
        modelBuilder.Entity<AlbumArtist>()
            .HasOne(x => x.Artist)
            .WithMany(x => x.AlbumArtists)
            .HasForeignKey(x => x.ArtistId);

        // AlbumGenre 
        // Klucz złożony z AlbumId i GenreId.
        modelBuilder.Entity<AlbumGenre>()
            .HasKey(x => new { x.AlbumId, x.GenreId });

        // Album może mieć wiele gatunków muzycznych.
        modelBuilder.Entity<AlbumGenre>()
            .HasOne(x => x.Album)
            .WithMany(x => x.AlbumGenres)
            .HasForeignKey(x => x.AlbumId);

        // Gatunek muzyczny może być przypisany do wielu albumów.
        modelBuilder.Entity<AlbumGenre>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.AlbumGenres)
            .HasForeignKey(x => x.GenreId);

        // Album
        modelBuilder.Entity<Album>(entity =>
        {
            // Tytuł jest wymagany, max 150 znaków.
            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            // Rok wydania musi mieć 4 cyfry lub być null.
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Album_ReleaseYear_4Digits",
                "\"ReleaseYear\" IS NULL OR (\"ReleaseYear\" BETWEEN 1000 AND 9999)"));

            // URL do obrazka okładki albumu max 500 znaków.
            entity.Property(x => x.CoverUrl)
                .HasMaxLength(500);

            // Opis max 2000 znaków.
            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            // Relacja do właściciela — usunięcie usera kasuje jego albumy
            entity.HasOne<ApplicationUser>(x => x.User)
                .WithMany(x => x.Albums)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Artist
        modelBuilder.Entity<Artist>(entity =>
        {
            // Nazwa wymagana, max 50 znaków.
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Kraj opcjonalny, max 50 znaków.
            entity.Property(x => x.Country)
                .HasMaxLength(50);

            // Opis, max 2000 znaków.
            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            // Unikalna nazwa w obrębie jednego użytkownika
            entity.HasIndex(x => new { x.UserId, x.Name }).IsUnique();

            // Klucz obcy UserId, usuwanie kaskadowe.
            entity.HasOne(x => x.User)
                .WithMany(x => x.Artists)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Genre
        modelBuilder.Entity<Genre>(entity =>
        {
            // Nazwa wymagana, max 50 znaków.
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Unikalna nazwa w obrębie jednego użytkownika.
            entity.HasIndex(x => new { x.UserId, x.Name }).IsUnique();

            // Klucz obcy UserId, usuwanie kaskadowe.
            entity.HasOne(x => x.User)
                .WithMany(x => x.Genres)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            // Nazwa wymagana, max 50 znaków.
            entity.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);

            // UserName unikalny globalnie.
            entity.HasIndex(x => x.UserName).IsUnique();

            // Email wymagany, max 120 znaków.
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(120);

            // Email unikalny globalnie.
            entity.HasIndex(x => x.Email).IsUnique();

            // Hasło wymagane, max 255 znaków.
            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
        });
    }
}
