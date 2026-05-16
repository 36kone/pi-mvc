using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaMvc.Data;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await EnsureColumnExistsAsync(db, tableName: "pizzas", columnName: "Image", columnDefinition: "VARCHAR(255) NULL");
    await EnsureColumnExistsAsync(db, tableName: "bebidas", columnName: "Image", columnDefinition: "VARCHAR(255) NULL");
    await EnsureColumnExistsAsync(db, tableName: "eventos", columnName: "Image", columnDefinition: "VARCHAR(255) NULL");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "POST" && context.Request.HasFormContentType)
    {
        var contentType = context.Request.ContentType ?? string.Empty;
        var isMultipart = contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase);

        if (!isMultipart)
        {
            try
            {
                var form = await context.Request.ReadFormAsync();
                if (form.TryGetValue("_method", out var method))
                {
                    var m = method.ToString().ToUpperInvariant();
                    if (m == "PUT" || m == "DELETE" || m == "PATCH")
                        context.Request.Method = m;
                }
            }
            catch
            {
            }
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

static async Task EnsureColumnExistsAsync(AppDbContext db, string tableName, string columnName, string columnDefinition)
{
    var connection = db.Database.GetDbConnection();
    var shouldClose = connection.State != ConnectionState.Open;

    if (shouldClose)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText =
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tableName AND COLUMN_NAME = @columnName;";

        var tableParam = checkCommand.CreateParameter();
        tableParam.ParameterName = "@tableName";
        tableParam.Value = tableName;
        checkCommand.Parameters.Add(tableParam);

        var columnParam = checkCommand.CreateParameter();
        columnParam.ParameterName = "@columnName";
        columnParam.Value = columnName;
        checkCommand.Parameters.Add(columnParam);

        var countObj = await checkCommand.ExecuteScalarAsync();
        var count = Convert.ToInt32(countObj);

        if (count == 0)
        {
            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = $"ALTER TABLE `{tableName}` ADD COLUMN `{columnName}` {columnDefinition};";
            await alterCommand.ExecuteNonQueryAsync();
        }
    }
    finally
    {
        if (shouldClose)
        {
            await connection.CloseAsync();
        }
    }
}
