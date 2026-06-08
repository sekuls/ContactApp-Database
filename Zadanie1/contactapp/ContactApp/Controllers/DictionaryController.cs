using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactApp.Data;

namespace ContactApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly DbConfig _db;

    public DictionaryController(DbConfig db)
    {
        _db = db;
    }

    // GET /api/dictionary/categories - list of categories from db
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _db.Categories
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();
        return Ok(categories);
    }

    // GET /api/dictionary/subcategories/{categoryId - returns subcategories for given category
    [HttpGet("subcategories/{categoryId}")]
    public async Task<IActionResult> GetSubcategories(int categoryId)
    {
        var subcategories = await _db.Subcategories
            .Where(s => s.CategoryId == categoryId)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return Ok(subcategories);
    }
}
