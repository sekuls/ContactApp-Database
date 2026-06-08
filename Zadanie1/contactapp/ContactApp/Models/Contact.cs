namespace ContactApp.Models;

// kontakt
public class Contact
{
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
}
