using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ContactApp.Data;
using ContactApp.Services;

//wyciągamy wartości z pliku konfuguracyjnego appsetting.json
var builder = WebApplication.CreateBuilder(args); //read appsetting.json



//db
builder.Services.AddDbContext<DbConfig>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

//services  - dependency injection
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ContactService>();

//controllers
builder.Services.AddControllers();


//jwt
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//baza ma istnieć przed uruchomieniem serwera
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbConfig>(); // create connection with db
    //db.Database.EnsureDeleted();
    DbInit.Init(db); // create contacts.db
}

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run("http://localhost:5000");
