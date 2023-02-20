using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROIECT.Data;
using PROIECT.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//In acest caz, stringul de conexiune o sa fie in Solution Explorer appsetting.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// PASUL 2
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

//PASUL 5 USERI si ROLURI
//CreateScope ofera acces la instanta curenta a aplicatiei
//variabila scope are un service provider - folsoit pentru a injecta dependeneele in aplicatie
//dependente -> db, cookie, sesiune, autentificare, etc
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}"); // ruta la pornirea aplicatiei
app.MapRazorPages();

app.Run();
