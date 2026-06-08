using ContactApp.Models;
using Microsoft.EntityFrameworkCore;
namespace ContactApp.Data;

// wypełnia bazę początkowymi wartościami
public static class DbInit
{
    public static void Init(DbConfig db)
    {
        db.Database.EnsureCreated(); // towrzymy contacts.db

        // jak nic nie ma w tabeli categories
        if (!db.Categories.Any())
        {
            //dodawanie kategorii
            var work = new Category {Name = "work"};
            var personal = new Category {Name = "personal"};
            var other = new Category {Name = "other"};

            db.Categories.AddRange(work, personal, other);
            db.SaveChanges(); //wypełnienie id

            //dodajemy i przypisujemy podkategorie do rodzica
            db.Subcategories.AddRange(
                new Subcategory { Name = "boss", CategoryId = work.Id },
                new Subcategory { Name = "client", CategoryId = work.Id },
                new Subcategory { Name = "colleague", CategoryId = work.Id },
                new Subcategory { Name = "employee", CategoryId = work.Id }
            );
            db.SaveChanges();

            Console.WriteLine($"dodano kategorie / {work.Id}");
        }
        else
        {
            Console.WriteLine("kategorie już są");
        }

        // create admin acc
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            });
            db.SaveChanges();

            Console.WriteLine("admin created :)");
        }
        else
        {
            Console.WriteLine("admin już istnieje :((");
        }
    }
}