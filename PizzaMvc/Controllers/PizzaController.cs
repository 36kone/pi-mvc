using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using PizzaMvc.Data;
using PizzaMvc.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PizzaMvc.Controllers;

public class PizzaController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PizzaController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Pizzas.ToListAsync());
    }

    public IActionResult Create()
    {
        return View("CriarPizza", new Pizza());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pizza pizza, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View("CriarPizza", pizza);

        if (imageFile != null && imageFile.Length > 0)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowed = extension is ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif";
            if (!allowed)
            {
                ModelState.AddModelError(string.Empty, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");
                return View("CriarPizza", pizza);
            }

            try
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "pizzas");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await imageFile.CopyToAsync(stream);
                }

                pizza.Image = $"/uploads/pizzas/{fileName}";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                return View("CriarPizza", pizza);
            }
        }

        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza == null) return NotFound();

        return View("EditarPizza", pizza);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Pizza pizza, IFormFile? imageFile)
    {
        if (id != pizza.Id) return NotFound();
        if (!ModelState.IsValid) return View("EditarPizza", pizza);

        var existing = await _context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (existing == null) return NotFound();

        var imagePath = existing.Image;

        if (imageFile != null && imageFile.Length > 0)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowed = extension is ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif";
            if (!allowed)
            {
                ModelState.AddModelError(string.Empty, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");
                pizza.Image = imagePath;
                return View("EditarPizza", pizza);
            }

            try
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "pizzas");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imagePath = $"/uploads/pizzas/{fileName}";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                pizza.Image = imagePath;
                return View("EditarPizza", pizza);
            }
        }

        existing.Nome = pizza.Nome;
        existing.Sabor = pizza.Sabor;
        existing.Descricao = pizza.Descricao;
        existing.Preco = pizza.Preco;
        existing.Categoria = pizza.Categoria;
        existing.Image = imagePath;

        _context.Update(existing);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza == null) return NotFound();

        return View(pizza);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza == null) return NotFound();

        _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
