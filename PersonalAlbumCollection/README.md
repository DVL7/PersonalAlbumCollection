# Personal Album Collection

Aplikacja webowa do prowadzenia prywatnej kolekcji albumów muzycznych. Każdy
zarejestrowany użytkownik buduje własną, odizolowaną bibliotekę albumów wraz z
powiązanymi artystami i gatunkami — z wyszukiwaniem, filtrowaniem i pełną obsługą
operacji CRUD.

## Stos technologiczny

- **.NET 10** + **Blazor** (interaktywne renderowanie po stronie serwera)
- **Entity Framework Core 10** + **PostgreSQL** (dostawca Npgsql)
- Uwierzytelnianie **cookie**, hashowanie haseł **PasswordHasher (PBKDF2)**,
  ochrona **Antiforgery (CSRF)**
- **Bootstrap** + autorski motyw `pac-theme.css`

## Funkcjonalności

- Rejestracja i logowanie (nazwą użytkownika lub adresem e-mail)
- CRUD albumów (tytuł, rok, okładka, opis, wielu artystów i gatunków)
- CRUD artystów (artysta solowy / zespół) oraz gatunków, z edycją w tabeli
- Wyszukiwanie po tytule lub wykonawcy i filtrowanie po gatunku
- Statystyki kolekcji oraz panel ustawień konta
- Pełna izolacja danych — każdy użytkownik widzi wyłącznie własne zasoby

## Wymagania

- .NET SDK 10.0 lub nowszy
- Serwer PostgreSQL (lokalny lub zdalny)
- Narzędzie `dotnet-ef` do migracji

## Uruchomienie

1. Sklonuj repozytorium.
2. Skopiuj plik `appsettings.json` i nazwij go `appsettings.Development.json`/
1. Skonfiguruj połączenie z bazą w pliku `appsettings.Development.json`
   (klucz `ConnectionStrings:DefaultConnection`):

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=PersonalAlbumCollection;Username=postgres;Password=***"
   }
   ```

2. Utwórz schemat bazy (zastosuj migracje):

   ```bash
   dotnet ef database update
   ```

3. Zbuduj i uruchom aplikację:

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

4. Otwórz w przeglądarce: <http://localhost:5222>

> Aplikacja nie zawiera kont testowych — załóż własne konto na ekranie rejestracji.

## Struktura projektu

```
PersonalAlbumCollection/
├─ Components/    # komponenty Razor: strony, layouty, elementy wspólne (UI)
├─ Services/      # logika aplikacji (interfejsy + implementacje)
├─ DTOs/          # obiekty transferu danych między warstwami
├─ Models/        # encje domenowe (Entities) i typy wyliczeniowe (Enums)
├─ Data/          # AppDbContext (EF Core) + migracje
├─ wwwroot/       # zasoby statyczne, motyw pac-theme.css
└─ Program.cs     # konfiguracja DI, uwierzytelniania i endpointów
```

## Autor

Dawid Deryło — projekt na przedmiot Programowanie Obiektowe 2,
Uniwersytet Rzeszowski.