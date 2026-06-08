namespace ContactApp.DTOs;
//contact data sent when creating or editing
public class ContactRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";      
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int CategoryId  { get; set; }
    public int? SubcategoryId { get; set; }
    public string?  CustomSubcategory { get; set; }
}
