using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAppMVC.DAL;
using WebAppMVC.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WebAppMVCContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppMVCContext") ?? throw new InvalidOperationException("Connection string 'WebAppMVCContext' not found.")));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Users/Login";
        options.AccessDeniedPath = "/Forbidden/";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
