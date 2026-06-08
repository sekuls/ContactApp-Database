namespace ContactApp.Models;

// dictionary of categories
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<Contact>     Contacts      { get; set; } = new List<Contact>();
    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
