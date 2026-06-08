using Microsoft.EntityFrameworkCore;
using ContactApp.Models;

namespace ContactApp.Data;
// konfuguracja naszej bazy ( relacje , tanele itp)
public class DbConfig : DbContext // dziedziczmy po Dbcontext 
{
    public DbConfig(DbContextOptions<DbConfig> options) : base(options) { } // pobieramy options z Program.cs

    // tabele
    public DbSet<Contact> Contacts { get; set;}
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get;set; }
    public DbSet<User> Users { get;set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) // OnModelCreating jest bardzo estetyczny i czytelny więc wybrałam go zamiast adnotacji
    {
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email) // unikalny by nikt nie założył konta 
            .IsUnique();

        // relacja contact -> category  1:n
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Category)
            .WithMany(c => c.Contacts)
            .HasForeignKey(c => c.CategoryId);


        //contact -> subcategory (opcjonalne bo może być nullem) 1:n
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Subcategory)
            .WithMany()
            .HasForeignKey(c => c.SubcategoryId)
            .IsRequired(false);

        //subcategory -> category
        modelBuilder.Entity<Subcategory>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId);
    }
}
