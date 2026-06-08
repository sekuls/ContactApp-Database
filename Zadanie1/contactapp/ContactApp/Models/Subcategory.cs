namespace ContactApp.Models;

// dictionary of subcategories assigned to a specific category
public class Subcategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }   //which category it belongs to
    public Category Category { get; set; } = null!;
}
