using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Hubs;
using SalesManagementSystem.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
//app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<SalesHub>("/hubs/sales");

app.Run();