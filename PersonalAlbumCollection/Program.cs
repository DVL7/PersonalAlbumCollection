// Program.cs

using PersonalAlbumCollection.Components;
using PersonalAlbumCollection.Data;
using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Services.Interfaces;
using PersonalAlbumCollection.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja komponentów Razor oraz włączenie interaktywnego renderowania po stronie serwera.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Konfiguracja uwierzytelniania opartego na plikach cookie.
// Po poprawnym zalogowaniu dane użytkownika są zapisywane w cookie,
// dzięki czemu aplikacja wie, który użytkownik jest aktualnie zalogowany.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ścieżka, na którą użytkownik zostanie przekierowany,
        // gdy będzie próbował wejść na stronę wymagającą logowania.
        options.LoginPath = "/login";
        // Ścieżka obsługująca wylogowanie użytkownika.
        options.LogoutPath = "/auth/logout";
        // Ścieżka używana w przypadku braku dostępu do zasobu.
        options.AccessDeniedPath = "/login";
        // Nazwa cookie przechowującego informacje o zalogowanym użytkowniku.
        options.Cookie.Name = "PersonalAlbumCollection.Auth";
    });

// Włączenie mechanizmu autoryzacji.
// Pozwala później ograniczać dostęp do stron lub funkcji tylko dla zalogowanych użytkowników.
builder.Services.AddAuthorization();
// Rejestracja dostępu do aktualnego HttpContext.
// Jest to potrzebne m.in. serwisom, które chcą odczytać dane aktualnie zalogowanego użytkownika.
builder.Services.AddHttpContextAccessor();

// ConnectionString
builder.Configuration.GetConnectionString("DefaultConnection");

// Rejestracja AppDbContext.
// Aplikacja korzysta z PostgreSQL, a connection string pobierany jest z konfiguracji,
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dodanie Serwisów
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();


// Konfiguracja HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Włączenie HSTS, czyli mechanizmu wymuszającego korzystanie z HTTPS w przeglądarce.
    app.UseHsts();
}


// Wymuszanie przekierowania z HTTP na HTTPS poza środowiskiem developerskim.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// Middleware odpowiedzialny za odczytanie cookie logowania
app.UseAuthentication();
// Middleware odpowiedzialny za sprawdzanie uprawnień użytkownika.
app.UseAuthorization();
// Obsługa kodów błędów HTTP, np. 404.
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// Włączenie ochrony przed atakami CSRF.
app.UseAntiforgery();

// Mapowanie plików statycznych aplikacji.
app.MapStaticAssets();

// Endpoint obsługujący logowanie użytkownika.
// Formularz logowania wysyła dane metodą POST na adres /auth/login.
app.MapPost("/auth/login", async (
    HttpContext httpContext,
    IAntiforgery antiforgery,
    [FromForm] LoginForm form,
    IAuthService authService) =>
{
    // Sprawdzenie tokenu anty-CSRF, aby upewnić się,
    // że żądanie pochodzi z prawidłowego formularza aplikacji
    await antiforgery.ValidateRequestAsync(httpContext);

    // Próba zalogowania użytkownika na podstawie loginu i hasła.
    var result = await authService.LoginAsync(form.Login, form.Password);

    // Jeśli logowanie się nie powiedzie, użytkownik wraca na stronę logowania
    // z parametrem informującym o błędzie.
    if (!result.Success || result.UserId is null || string.IsNullOrWhiteSpace(result.UserName))
    {
        return Results.LocalRedirect("/login?error=1");
    }

    // Claims to informacje opisujące zalogowanego użytkownika.
    // NameIdentifier przechowuje Id użytkownika,
    // a Name przechowuje jego nazwę/login.
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.UserId.Value.ToString()),
        new(ClaimTypes.Name, result.UserName)
    };

    // Utworzenie identity użytkownika na podstawie claims.
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    // Principal reprezentuje aktualnego użytkownika w systemie uwierzytelniania ASP.NET.
    var principal = new ClaimsPrincipal(identity);
    
    // Zalogowanie użytkownika poprzez zapisanie danych uwierzytelniających w cookie.
    // IsPersistent = true oznacza, że cookie może przetrwać zamknięcie przeglądarki.
    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });

    // Po poprawnym zalogowaniu użytkownik zostaje przekierowany do listy albumów.
    return Results.LocalRedirect("/albums");
});

// Endpoint obsługujący wylogowanie użytkownika.
app.MapPost("/auth/logout", async (HttpContext httpContext, IAntiforgery antiforgery) =>
{
    // Sprawdzenie tokenu anty-CSRF również przy wylogowaniu,
    // ponieważ jest to operacja zmieniająca stan sesji użytkownika.
    await antiforgery.ValidateRequestAsync(httpContext);
    // Usunięcie cookie uwierzytelniającego.
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    // Po wylogowaniu użytkownik wraca na stronę główną.
    return Results.LocalRedirect("/");
});

// Mapowanie głównej aplikacji Blazor.
// App jest komponentem startowym, a InteractiveServerRenderMode pozwala na interakcję po stronie serwera.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Uruchomienie aplikacji.
app.Run();

// Rekord reprezentujący dane przesyłane z formularza logowania.
// Nazwy pól muszą odpowiadać nazwom inputów w formularzu.
internal sealed record LoginForm(string Login, string Password);