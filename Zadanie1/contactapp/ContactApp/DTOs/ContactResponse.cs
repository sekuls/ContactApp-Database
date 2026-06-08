namespace ContactApp.DTOs;
// contact data returned to frontend ( without password hash))
public class ContactResponse
{
    public int Id { get; set; }
    public string FirstName  { get; set; } = "";
    public string LastName {get; set; } = "";
    public string Email {get; set; } = "";
    public string?  Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = "";
    public int? SubcategoryId { get; set; }
    public string? Subcategory  { get; set; }   
    public string? CustomSubcategory { get; set; }
}

// easier version for list view
public class ContactListItem
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone     { get; set; }
    public string Category  { get; set; } = "";
}
