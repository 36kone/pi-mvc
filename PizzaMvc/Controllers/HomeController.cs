using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;

namespace PizzaMvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var pizzas = await _context.Pizzas
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        var bebidas = await _context.Bebidas
            .AsNoTracking()
            .OrderByDescending(b => b.Id)
            .ToListAsync();

        var eventos = await _context.Eventos
            .AsNoTracking()
            .OrderByDescending(e => e.DataEvento)
            .ThenByDescending(e => e.Id)
            .ToListAsync();

        return View(new HomeIndexViewModel
        {
            Pizzas = pizzas,
            Bebidas = bebidas,
            Eventos = eventos
        });
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? q)
    {
        var keyword = (q ?? string.Empty).Trim();
        if (keyword.Length == 0)
        {
            return View(new HomeSearchViewModel
            {
                Query = keyword
            });
        }

        var keywordLower = keyword.ToLower();

        var pizzas = await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.Nome != null && p.Nome.ToLower().Contains(keywordLower))
            .OrderByDescending(p => p.Id)
            .Take(50)
            .ToListAsync();

        var bebidas = await _context.Bebidas
            .AsNoTracking()
            .Where(b => b.Nome != null && b.Nome.ToLower().Contains(keywordLower))
            .OrderByDescending(b => b.Id)
            .Take(50)
            .ToListAsync();

        var eventos = await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Nome != null && e.Nome.ToLower().Contains(keywordLower))
            .OrderByDescending(e => e.DataEvento)
            .ThenByDescending(e => e.Id)
            .Take(50)
            .ToListAsync();

        return View(new HomeSearchViewModel
        {
            Query = keyword,
            Pizzas = pizzas,
            Bebidas = bebidas,
            Eventos = eventos
        });
    }
}

public sealed class HomeIndexViewModel
{
    public List<Pizza> Pizzas { get; set; } = [];
    public List<Bebida> Bebidas { get; set; } = [];
    public List<Evento> Eventos { get; set; } = [];
}

public sealed class HomeSearchViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<Pizza> Pizzas { get; set; } = [];
    public List<Bebida> Bebidas { get; set; } = [];
    public List<Evento> Eventos { get; set; } = [];
}
