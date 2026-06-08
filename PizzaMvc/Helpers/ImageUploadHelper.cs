using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PizzaMvc.Helpers;

public readonly record struct ImageUploadResult(string? Path, string? Error);

public static class ImageUploadHelper
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".webp", ".gif"];

    public static async Task<ImageUploadResult> SaveAsync(IFormFile? file, IWebHostEnvironment env, string subfolder)
    {
        if (file == null || file.Length == 0)
            return new ImageUploadResult(null, null);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return new ImageUploadResult(null, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");

        try
        {
            var uploadsPath = Path.Combine(env.ContentRootPath, "files", subfolder);
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            await using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream);

            return new ImageUploadResult($"/files/{subfolder}/{fileName}", null);
        }
        catch
        {
            return new ImageUploadResult(null, "Não foi possível salvar a imagem. Tente novamente.");
        }
    }
}
