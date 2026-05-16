using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using System.Text.Json;

namespace PizzaMvc.Controllers;

public class PedidoController : Controller
{
    private readonly AppDbContext _context;

    private sealed record CartItemPayload(string? Entity, int Id, int Quantity);

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

    [HttpGet]
    public IActionResult Checkout()
    {
        ViewBag.Checkout = true;
        ViewBag.FormAction = Url.Action("PlaceOrder", "Pedido") ?? "/Pedido/PlaceOrder";
        ViewBag.CancelUrl = Url.Action("Index", "Cart") ?? "/Cart";
        return View("~/Views/Cliente/CriarCliente.cshtml", new Cliente());
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Cliente cliente, string formaPagamento, string cartJson)
    {
        if (string.IsNullOrWhiteSpace(formaPagamento))
        {
            ViewBag.Checkout = true;
            ViewBag.FormAction = Url.Action("PlaceOrder", "Pedido") ?? "/Pedido/PlaceOrder";
            ViewBag.CancelUrl = Url.Action("Index", "Cart") ?? "/Cart";
            ViewBag.Error = "Selecione a forma de pagamento.";
            return View("~/Views/Cliente/CriarCliente.cshtml", cliente);
        }

        if (string.IsNullOrWhiteSpace(cliente?.Nome))
        {
            ViewBag.Checkout = true;
            ViewBag.FormAction = Url.Action("PlaceOrder", "Pedido") ?? "/Pedido/PlaceOrder";
            ViewBag.CancelUrl = Url.Action("Index", "Cart") ?? "/Cart";
            ViewBag.Error = "Informe o nome do cliente.";
            return View("~/Views/Cliente/CriarCliente.cshtml", cliente);
        }

        List<CartItemPayload> payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<CartItemPayload>>(cartJson ?? string.Empty, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CartItemPayload>();
        }
        catch
        {
            payload = new List<CartItemPayload>();
        }

        var normalized = payload
            .Where(i => i != null)
            .Select(i => new
            {
                Entity = (i.Entity ?? string.Empty).Trim().ToLowerInvariant(),
                i.Id,
                Quantity = i.Quantity < 1 ? 0 : i.Quantity
            })
            .Where(i => i.Quantity > 0 && i.Id > 0 && (i.Entity == "pizza" || i.Entity == "bebida"))
            .GroupBy(i => new { i.Entity, i.Id })
            .Select(g => new { g.Key.Entity, g.Key.Id, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        if (normalized.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                Total = 0
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal total = 0;
            foreach (var item in normalized)
            {
                if (item.Entity == "pizza")
                {
                    var pizza = await _context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (pizza == null) throw new InvalidOperationException("Pizza inválida no carrinho.");
                    var preco = pizza.Preco;
                    _context.ItensPedido.Add(new ItemPedido
                    {
                        PedidoId = pedido.Id,
                        PizzaId = pizza.Id,
                        BebidaId = null,
                        Quantidade = item.Quantity,
                        PrecoUnitario = preco
                    });
                    total += preco * item.Quantity;
                }
                else if (item.Entity == "bebida")
                {
                    var bebida = await _context.Bebidas.AsNoTracking().FirstOrDefaultAsync(b => b.Id == item.Id);
                    if (bebida == null) throw new InvalidOperationException("Bebida inválida no carrinho.");
                    var preco = bebida.Preco;
                    _context.ItensPedido.Add(new ItemPedido
                    {
                        PedidoId = pedido.Id,
                        PizzaId = null,
                        BebidaId = bebida.Id,
                        Quantidade = item.Quantity,
                        PrecoUnitario = preco
                    });
                    total += preco * item.Quantity;
                }
            }

            pedido.Total = total;
            _context.Pedidos.Update(pedido);

            _context.Pagamentos.Add(new Pagamento
            {
                PedidoId = pedido.Id,
                FormaPagamento = formaPagamento.Trim(),
                Valor = total,
                Status = "Pendente",
                Pedido = pedido
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction("Index", "Home", new { pedido = pedido.Id });
        }
        catch
        {
            await tx.RollbackAsync();
            ViewBag.Checkout = true;
            ViewBag.FormAction = Url.Action("PlaceOrder", "Pedido") ?? "/Pedido/PlaceOrder";
            ViewBag.CancelUrl = Url.Action("Index", "Cart") ?? "/Cart";
            ViewBag.Error = "Não foi possível finalizar o pedido. Tente novamente.";
            return View("~/Views/Cliente/CriarCliente.cshtml", cliente);
        }
    }
}
