using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using PizzaMvc.Helpers;

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

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var pizza = await _context.Pizzas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pizza == null) return NotFound();

        return Json(new
        {
            id = pizza.Id,
            nome = pizza.Nome,
            sabor = pizza.Sabor,
            descricao = pizza.Descricao,
            preco = pizza.Preco,
            categoria = pizza.Categoria,
            image = pizza.Image
        });
    }

    public IActionResult Create()
    {
        return View("CriarPizza", new Pizza());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pizza pizza, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View("CriarPizza", pizza);

        var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "pizzas");
        if (upload.Error != null)
        {
            ModelState.AddModelError(string.Empty, upload.Error);
            return View("CriarPizza", pizza);
        }
        if (upload.Path != null) pizza.Image = upload.Path;

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

        var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "pizzas");
        if (upload.Error != null)
        {
            ModelState.AddModelError(string.Empty, upload.Error);
            pizza.Image = imagePath;
            return View("EditarPizza", pizza);
        }
        if (upload.Path != null) imagePath = upload.Path;

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
