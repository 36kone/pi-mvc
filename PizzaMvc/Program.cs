using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "POST" && context.Request.HasFormContentType)
    {
        var form = await context.Request.ReadFormAsync();
        if (form.TryGetValue("_method", out var method))
        {
            var m = method.ToString().ToUpperInvariant();
            if (m == "PUT" || m == "DELETE" || m == "PATCH")
                context.Request.Method = m;
        }
    }

    await next();
});

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();