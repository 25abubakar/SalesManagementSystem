using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Repository;

var builder = WebApplication.CreateBuilder(args); // Pehle builder create karein

// 1. Connection String hasil karein
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Sirf ApplicationDbContext register karein (AppDbContext ko nikal dein agar wo use nahi ho raha)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Other Services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

var app = builder.Build();

// 4. Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // MapStaticAssets ki jagah standard UseStaticFiles behtar hai basic project ke liye

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();