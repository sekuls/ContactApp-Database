using Microsoft.EntityFrameworkCore;
using ContactApp.Data;
using ContactApp.DTOs;
using ContactApp.Models;

namespace ContactApp.Services;

// logika biznesowa
public class ContactService
{
    private readonly DbConfig _db;
    public ContactService(DbConfig db) { _db = db; } // połączenie z bazą

    //pobieramy niektóre dane z kontaków
    public async Task<List<ContactListItem>> GetListAsync()
    {
        return await _db.Contacts.Include(c => c.Category).Select(c => new ContactListItem{
                Id = c.Id,FirstName = c.FirstName,LastName= c.LastName, Email = c.Email,Phone= c.Phone,
                Category= c.Category.Name}).ToListAsync();
    }

    // pobranie szczegółów kontaku
    public async Task<ContactResponse?> GetByIdAsync(int id)
    {
        var contact = await _db.Contacts.Include(c => c.Category).Include(c => c.Subcategory).FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return null;
        return MapToResponse(contact);
    }

    // dodanie nowego kontaku
    public async Task<(int? Id, string? Error)> AddAsync(ContactRequest request)
    {
        // sprawdzamy czy nowy email
        if (await _db.Contacts.AnyAsync(c => c.Email == request.Email))
            return (null, "Email already exists!");

        // sprawdzany hasło
        if (!ValidatePassword(request.Password))
            return (null, "Password too weak!!!");

        var contact = new Contact{
            FirstName= request.FirstName,
            LastName= request.LastName,
            Email= request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            CategoryId= request.CategoryId,
            SubcategoryId= request.SubcategoryId,
            CustomSubcategory = request.CustomSubcategory
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();
        return (contact.Id, null);
    }

    // update contact
    public async Task<string?> UpdateAsync(int id, ContactRequest request)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact == null) return "Contact not found";

        // sprawdzanie czy ktoś już użył takiego emailu
        if (await _db.Contacts.AnyAsync(c => c.Email == request.Email && c.Id != id))
            return "This email is already taken";

        contact.FirstName= request.FirstName;
        contact.LastName= request.LastName;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.DateOfBirth = request.DateOfBirth;
        contact.CategoryId = request.CategoryId;
        contact.SubcategoryId= request.SubcategoryId;
        contact.CustomSubcategory = request.CustomSubcategory;

        // update hasła jak podano nowe
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (!ValidatePassword(request.Password)) return "Password too weak!";
            contact.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        await _db.SaveChangesAsync();
        return null;
    }

    // delete contact
    public async Task<bool> DeleteAsync(int id)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact == null) return false;

        _db.Contacts.Remove(contact);
        await _db.SaveChangesAsync();
        return true;
    }

    // mapowanie do wysłania
    private static ContactResponse MapToResponse(Contact c) => new()
    {
        Id = c.Id, FirstName= c.FirstName, LastName= c.LastName,Email= c.Email,Phone= c.Phone, DateOfBirth = c.DateOfBirth,
        CategoryId = c.CategoryId,Category = c.Category.Name, SubcategoryId = c.SubcategoryId, Subcategory = c.Subcategory?.Name ?? c.CustomSubcategory,
        CustomSubcategory= c.CustomSubcategory
    };

    // sprawdzanie poprawności hasła
    public static bool ValidatePassword(string password)
    {
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        return true;
    }
}
