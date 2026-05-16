using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PizzaMvc.Controllers;

public class PizzaController : Controller
{
    private readonly AppDbContext _context;

    public PizzaController(AppDbContext context)
    {
        _context = context;
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
    public async Task<IActionResult> Create(Pizza pizza)
    {
        if (!ModelState.IsValid) return View("CriarPizza", pizza);

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

    [HttpPut]
    public async Task<IActionResult> Edit(int id, Pizza pizza)
    {
        if (id != pizza.Id) return NotFound();
        if (!ModelState.IsValid) return View("EditarPizza", pizza);

        _context.Entry(pizza).State = EntityState.Modified;
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
