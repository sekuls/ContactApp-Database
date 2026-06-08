# Contact App

autor: Natalia Sekula

## Opis poszczególnych klas i metod

`Program.cs`

Plik startowy aplikacji. Odpowiada za:
- konfigurację bazy danych SQLite
- rejestrację serwisów
- konfigurację kontrolerów, JWT,
- serwowanie plików frontendu z `wwwroot`


### Modele

`Contact.cs` - kontakt

``` csharp
 public int Id { get; set; }
 public string FirstName { get; set; } = "";
 public string LastName { get; set; } = "";
 public string Email { get; set; } = "";   
 public string PasswordHash { get; set; } = "";   
 public string? Phone { get; set; }
 public DateTime? DateOfBirth{ get; set; }

 //relation to category (work/personal/other)
 public int       CategoryId          { get; set; }
 public Category  Category            { get; set; } = null!;

 // subcategory from dictionary - only for "work"
 public int?  SubcategoryId { get; set; }
 public Subcategory? Subcategory { get; set; }

 //custom subcategory typed manually- other
 public string?   CustomSubcategory  { get; set; }
```


 `Category.cs` -  kategorie kontaktu
- Kategorie są przetrzymywane w bazie. Przykładowe kategorie: work, personal, other

``` csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Contact> Contacts { get; set; }
    public ICollection<Subcategory> Subcategories { get; set; }
}
```


`Subcategory.cs` - model podkategorii
- Są używane głównie dla kategorii work.

``` csharp
public class Subcategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
```


 `User.cs` - użytkownik. Pomoce przy logowaniu. Hasło jest zapisane w bazie jako hash BCrypt


``` csharp
public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
}
```

Domyślny admin tworzy `DbInit.cs`:

``` csharp
 db.Users.Add(new User
 {
     Login = "admin",
     PasswordHash = BCrypt.Net.BCrypt.HashPassword("maslo123.")
 });
 db.SaveChanges();
```


### DTO
- używamy do przesyłania danych między frontendem a backendem. Potrzebne jak  nie chcemy wysyłać informacji wrażliwych.


`LoginRequest.cs` - klasa używana przy logowaniu

``` csharp
public class LoginRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
}
```

przykładowa serializacja na json przy  `POST /api/auth/login` - endpoint loginu

``` JSON
{
  "login": "admin",
  "password": "Admin123!"
}
```


 `ContactRequest.cs` - dodawanie i edytowanie kontaktu

``` csharp
public class ContactRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public string? CustomSubcategory { get; set; }
}
```



 `ContactResponse.cs` - zwracanie szczegółów kontaktu do frontendu. Nie zwraca hasła ani hasha hasła!


``` csharp
public class ContactResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public int? SubcategoryId { get; set; }
    public string? Subcategory { get; set; }
    public string? CustomSubcategory { get; set; }
}
```


`ContactResponse.cs` zawiera `ContactListItem` - czyli wyświetlenie listy kontaktów.


```csharp
public class ContactListItem
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string Category { get; set; }
}
```


### Dane

`DbConfig.cs` - konfiguracja bazy danych Entity Framework Core

``` csharp
public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<User> Users { get; set; }
}
```

Zawiera metodę z użyciem `OnModelCreating` , ponieważ jest estetyczny i czytelniejszy od adnotacji

``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
```

konfiguruje bazy danych:
- ustawia unikalny email
- konfiguruje relację: 
	- Contact -> Category
	- Contact -> Subcategory
	- Subcategory -> Category




 `DbInit.cs` - dodanie danych startowych do bazy

metody:

``` csharp
public static void Init(DbConfig db)
```

Odpowiada za:
- utworzenie bazy danych ( jeśli nie istnieje )
- dodanie:
	- kategorii
	- podkategorii
	- użytkownika ( admina )


``` csharp
private static Category GetOrCreateCategory(DbConfig db, string name)
```

- Sprawdza, czy dana kategoria już istnieje  ( jak nie, to ją tworzy )


``` csharp
private static Subcategory GetOrCreateSubcategory(DbConfig db, string name, int categoryId)
```

- Sprawdza, czy dana podkategoria już istnieje ( jak nie, to ją tworzy )



## Serwisy

`AuthService.cs` - odpowiada za logowanie i tworzenie tokena JWT


Metoda `LoginAsync`:

``` csharp 
public async Task<string?> LoginAsync(LoginRequest request)
```

1. Szuka użytkownika po loginie
2. Sprawdza hasło za pomocą BCrypt
3. Jeśli dane są poprawne, generuje JWT
4. Jeśli dane są błędne, zwraca `null`


Metoda `GenerateToken`:

``` csharp
private string GenerateToken(string login, int userId)
```

Tworzy token JWT z:
- loginem usera
- id usera
- deadline tokenu

Token jest potem używany przy dodawaniu, edycji i usuwaniu kontaktów.



> `ContactService.cs` - logika kontaktów


Metoda `GetListAsync` :

``` csharp
public async Task<List<ContactListItem>> GetListAsync()
```

Pobiera listę kontaktów z podstawowymi danymi - imię, nazwisko, email, telefon, kategorię.


Metoda `GetByIdAsync` : 

``` csharp
public async Task<ContactResponse?> GetByIdAsync(int id)
```

Pobiera szczegóły pojedynczego kontaktu po jego id ( jak nie ma kontaktu zwraca `null`)


Metoda `AddAsync` : 

``` csharp
public async Task<(int? Id, string? Error)> AddAsync(ContactRequest request)
```

Dodaje nowy kontakt:
1. Sprawdza, czy email jest unikalny
2. Sprawdza siłę hasła
3. Hashuje hasło - BCrypt
4. Tworzy nowy obiekt Contact
5. Zapisuje go do bazy

Zwraca id nowego kontaktu 


Metoda `UpdateAsync` :

``` csharp
public async Task<string?> UpdateAsync(int id, ContactRequest request)
```

Edytuje istniejący kontakt: 
1. Szuka kontaktu po id
2. Sprawdza, czy nowy email nie jest zajęty przez inny kontakt
3. Aktualizuje dane
4. Jeśli podano nowe hasło, waliduje je i zapisuje nowy hash
5. Zapisuje zmiany w bazie

 Zwraca `null` jak się nie powiedzie edycja.


Metoda `DeleteAsync` :

``` csharp
public async Task<bool> DeleteAsync(int id)
```

Usuwa kontakt z bazy danych.
Zwraca:
- 1 jeśli usunięto kontakt
- 0 jeśli kontakt nie istnieje



Metoda `ValidatePassword`: 

``` csharp
public static bool ValidatePassword(string password)
```

Sprawdza, czy hasło spełnia podstawowe wymagania: minimum 8 znaków, mała litera, wielka litera, cyfra,


### Controllers REST API

`AuthController.cs` - endpoint logowania

Login : 

``` csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
```

``` http
POST /api/auth/login
```

- Przyjmuje login i hasło  
- Zwraca token JWT, jeśli dane są poprawne


`ContactsController.cs` - Kontroler obsługujący kontakty

GetList:

``` csharp
[HttpGet]
public async Task<IActionResult> GetList()
```

``` http
GET /api/contacts
```

- Zwraca listę kontaktów
- Dostępne bez logowania



GetById :

``` csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
```

``` http
GET /api/contacts/{id}
```

- Zwraca szczegóły kontaktu  
- Dostępne bez logowania


Add :

``` csharp
[HttpPost]
[Authorize]
public async Task<IActionResult> Add([FromBody] ContactRequest request)
```

``` http
POST /api/contacts
```

- Dodaje nowy kontakt  
- Wymaga zalogowania


Update:

``` csharp
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> Update(int id, [FromBody] ContactRequest request)
```


``` http
PUT /api/contacts/{id}
```

- Edytuje kontakt  
- Wymaga zalogowania


Delete:

``` csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> Delete(int id)
```

``` http
DELETE /api/contacts/{id}
```

- Usuwa kontakt  
- Wymaga zalogowania



`DictionaryController.cs` - obsługuje dane słownikowe


GetCategories:

``` csharp
[HttpGet("categories")]
public async Task<IActionResult> GetCategories()
```

``` http
GET /api/dictionary/categories
```

- Zwraca listę kategorii z bazy danych.


 GetSubcategories:

``` csharp
[HttpGet("subcategories/{categoryId}")]
public async Task<IActionResult> GetSubcategories(int categoryId)
```

``` http
GET /api/dictionary/subcategories/{categoryId}
```

- Zwraca podkategorie dla wybranej kategorii


### Frontend

`index.html` -zawiera widoki:
- lista kontaktów
- szczegóły kontaktu
- logowanie
- formularz dodawania/edycji kontaktu


`app.js` - logika frontendu. Najważniejsze funkcje:

- `showList()` - pokazuje listę kontaktów
- `showLogin()` - pokazuje formularz logowania
- `showForm(id)` - pokazuje formularz dodawania/edycji kontaktu
- `hideAll()` - ukrywa wszystkie widoki
- `login()` - wysyła dane logowania do: `POST /api/auth/login`. Po zalogowaniu zapisuje JWT w localStorage
- `logout()` - Usuwa token z localStorage i wylogowuje uzytkownika
- `fetchContacts()` - obiera listę kontaktow
- `showDetails(id)` - pokazuje szczegóły wybranego kontaktu
- `fetchContactDetails(id, toForm)` - pobiera szczegóły kontaktu
- `saveContact()` - dodaje albo edytuje kontakt
- `deleteContact()` - usuwa kontakt
 - `fetchCategories()` - pobiera kategorie z bd


`style.css` - styl aplikacji. No wiadomo odpowiada za wygląd 


## Biblioteki

- ASP.NET Core 
- Entity Framework Core
- Baza -  SQLite `contacts.db`
- JWT Bearer Authentication
- Frontend - Bootstrap


```
 <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite"          Version="8.0.0" />
 <PackageReference Include="Microsoft.EntityFrameworkCore.Design"          Version="8.0.0" />
 <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
 <PackageReference Include="BCrypt.Net-Next"                               Version="4.0.3" />
```




## Kompilacja

koniecznie: `.NET 8 SDK`

w cmd należy wpisać:
```
cd ContactApp
dotnet restore
dotnet build
dotnet run
```

W celu zobaczenia pięknej aplikacji zapraszam na adres:

```
http://localhost:5000
```
