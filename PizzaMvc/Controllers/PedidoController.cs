using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;

namespace PizzaMvc.Controllers;

public class PedidoController : Controller
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Pizza)
            .ToListAsync();
        return View(pedidos);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Clientes = await _context.Clientes.ToListAsync();
        ViewBag.Pizzas = await _context.Pizzas.ToListAsync();
        return View("CriarPedido", new Pedido());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pedido pedido, int[] pizzaIds, int[] quantidades)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            ViewBag.Pizzas = await _context.Pizzas.ToListAsync();
            return View("CriarPedido", pedido);
        }

        pedido.DataPedido = DateTime.Now;
        pedido.Status = "Feito";
        pedido.Itens = new List<ItemPedido>();

        decimal total = 0;
        for (int i = 0; i < pizzaIds.Length; i++)
        {
            var pizza = await _context.Pizzas.FindAsync(pizzaIds[i]);
            if (pizza != null)
            {
                var item = new ItemPedido
                {
                    PizzaId = pizza.Id,
                    Quantidade = quantidades[i],
                    PrecoUnitario = pizza.Preco
                };
                pedido.Itens.Add(item);
                total += pizza.Preco * quantidades[i];
            }
        }

        pedido.Total = total;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null) return NotFound();

        ViewBag.Clientes = await _context.Clientes.ToListAsync();
        ViewBag.Pizzas = await _context.Pizzas.ToListAsync();
        return View("EditarPedido", pedido);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(int id, Pedido pedido)
    {
        if (id != pedido.Id) return NotFound();
        if (!ModelState.IsValid) return View("EditarPedido", pedido);

        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        return View(pedido);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AtualizarStatus(int id, string status)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
