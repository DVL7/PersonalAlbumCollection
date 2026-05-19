# Personal Album Collection

Aplikacja webowa do zarządzania prywatną kolekcją albumów muzycznych. Pozwala dodawać, edytować, oceniać i przeglądać albumy oraz filtrować je po kluczowych parametrach.

## Spis treści
- [Stack](#stack)
- [Widoki](#widoki)
- [Funkcje](#funkcje)
- [Kroki realizacji projektu](#kroki-realizacji-projektu)

## Stack
| Warstwa | Technologia |
| --- | --- |
| Frontend | Blazor Web App |
| ORM | Entity Framework Core |
| Baza danych | PostgreSQL |
| Autoryzacja | ASP.NET Core Identity |
| UI | Bootstrap  |
| Migracje | EF Core Migrations |

## Widoki
- Strona główna z opisem aplikacji
- Lista albumów użytkownika
- Dodawanie albumu
- Edycja albumu
- Szczegóły albumu i ocena
- Logowanie
- Rejestracja

## Funkcje
- Rejestracja i logowanie
- Dodawanie albumów do własnej kolekcji
- Przeglądanie listy albumów
- Edycja i usuwanie albumów
- Ocenianie albumów w skali 1–10
- Dodawanie krótkiej recenzji/notatki
- Filtrowanie i sortowanie po: wykonawcy, gatunku, ocenie, roku wydania

## Kroki realizacji projektu
1. **Utworzenie projektu** — Utwórz projekt Blazor, uruchom aplikację lokalnie, sprawdź stronę główną, dodaj repozytorium Git, zrób pierwszy commit.
2. **Podłączenie PostgreSQL** — Utwórz bazę danych (np. MusicAlbumsDb), dodaj connection string w `appsettings.json`, zainstaluj pakiety EF Core dla PostgreSQL, skonfiguruj DbContext, sprawdź start aplikacji.
3. **Dodanie logowania** — Dodaj ASP.NET Core Identity, utwórz klasę ApplicationUser, zmień DbContext na `IdentityDbContext<ApplicationUser>`, dodaj migrację, wykonaj `database update`, sprawdź rejestrację i logowanie.
4. **Model Albumu** — Dodaj klasę Album, dodaj pola, dodaj `DbSet<Album>` w DbContext, dodaj migrację, zaktualizuj bazę.
5. **Lista albumów** — Dodaj stronę `/albums`, pobierz aktualnego użytkownika, pobierz z bazy tylko jego albumy, wyświetl albumy w tabeli albo kartach, dodaj link w menu.
6. **Dodawanie albumu** — Dodaj stronę `/albums/create`, utwórz formularz Blazor, dodaj walidację wymaganych pól, przypisz album do aktualnego użytkownika, po zapisie przekieruj na listę albumów.
7. **Szczegóły albumu** — Dodaj stronę `/albums/{id}`, pobierz album po Id, sprawdź właściciela, wyświetl szczegóły albumu.
8. **Edycja albumu** — Dodaj stronę `/albums/edit/{id}`, pobierz album z bazy, sprawdź właściciela, wyświetl formularz z aktualnymi danymi, zapisz zmiany, przekieruj do szczegółów albo listy.
9. **Usuwanie albumu** — Dodaj przycisk „Usuń” na liście albo w szczegółach, dodaj stronę potwierdzenia `/albums/delete/{id}`, sprawdź właściciela, usuń album z bazy, przekieruj na listę.
10. **Oceny i recenzje** — Dodaj Score i Review do Album, dodaj migrację, zaktualizuj bazę, dodaj pola oceny i recenzji do formularzy, wyświetl ocenę na liście i w szczegółach.
11. **Filtrowanie i sortowanie** — Dodaj wyszukiwarkę po tytule i wykonawcy, filtr po gatunku, sortowanie po tytule, wykonawcy, roku i ocenie.
12. **Uporządkowanie interfejsu** — Dodaj prosty layout, popraw menu, dodaj karty Bootstrapowe, dodaj komunikaty („Brak albumów”, „Album dodany”, „Album usunięty”), dodaj stronę główną z opisem.
13. **Dane testowe** — Załóż konto testowe, dodaj 8–12 albumów z ocenami, dodaj różne gatunki, dodaj kilka okładek z URL.
