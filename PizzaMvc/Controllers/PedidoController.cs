using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using System.Text.Json;

namespace PizzaMvc.Controllers;

public class PedidoController : Controller
{
    private readonly AppDbContext _context;
    private const string ClientIdCookieName = "pi_client_id";

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
            .Include(p => p.Itens)
            .ThenInclude(i => i.Bebida)
            .Include(p => p.Pagamento)
            .ToListAsync();
        return View(pedidos);
    }

    [HttpGet]
    public IActionResult MeusPedidos()
    {
        ViewBag.NeedsDocumento = true;
        return View("MeusPedidos", Enumerable.Empty<Pedido>());
    }

    [HttpGet]
    public async Task<IActionResult> GetByClientId(int clientId)
    {
        if (clientId <= 0) return NotFound();

        var pedidos = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.ClienteId == clientId)
            .Include(p => p.Cliente)
            .Include(p => p.Pagamento)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Pizza)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Bebida)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        Response.Cookies.Append(ClientIdCookieName, clientId.ToString(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = false
        });

        return View("MeusPedidos", pedidos);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCpfCnpj(string cpfCnpj)
    {
        var normalized = NormalizeCpfCnpj(cpfCnpj);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            ViewBag.NeedsDocumento = true;
            ViewBag.InvalidDocumento = true;
            ViewBag.Documento = cpfCnpj;
            return View("MeusPedidos", Enumerable.Empty<Pedido>());
        }

        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.CpfCnpj != null
                && c.CpfCnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty).Replace(" ", string.Empty) == normalized);

        if (cliente == null)
        {
            ViewBag.NeedsDocumento = true;
            ViewBag.ClientNotFound = true;
            ViewBag.Documento = cpfCnpj;
            return View("MeusPedidos", Enumerable.Empty<Pedido>());
        }

        var pedidos = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.ClienteId == cliente.Id)
            .Include(p => p.Cliente)
            .Include(p => p.Pagamento)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Pizza)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Bebida)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        ViewBag.Documento = cliente.CpfCnpj;
        return View("MeusPedidos", pedidos);
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

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        var next = (status ?? string.Empty).Trim();
        var current = (pedido.Status ?? string.Empty).Trim();

        var allowed = (current == "Pendente" && next == "Em Andamento")
            || (current == "Em Andamento" && next == "Concluido");

        if (!allowed) return BadRequest();

        pedido.Status = next;
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

        cliente.CpfCnpj = NormalizeCpfCnpj(cliente.CpfCnpj);
        if (string.IsNullOrWhiteSpace(cliente.CpfCnpj))
        {
            ViewBag.Checkout = true;
            ViewBag.FormAction = Url.Action("PlaceOrder", "Pedido") ?? "/Pedido/PlaceOrder";
            ViewBag.CancelUrl = Url.Action("Index", "Cart") ?? "/Cart";
            ViewBag.Error = "Informe o CPF/CNPJ.";
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
            var existingClient = await TryResolveExistingClientAsync(cliente);
            if (existingClient == null)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            else
            {
                existingClient.Nome = cliente.Nome;
                existingClient.Telefone = cliente.Telefone;
                existingClient.Email = cliente.Email;
                existingClient.CpfCnpj = cliente.CpfCnpj;
                _context.Clientes.Update(existingClient);
                await _context.SaveChangesAsync();
                cliente = existingClient;
            }

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

            Response.Cookies.Append(ClientIdCookieName, cliente.Id.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = false
            });

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

    private int? TryGetClientIdFromCookie()
    {
        if (!Request.Cookies.TryGetValue(ClientIdCookieName, out var raw)) return null;
        if (!int.TryParse(raw, out var id)) return null;
        return id > 0 ? id : null;
    }

    private async Task<Cliente?> TryResolveExistingClientAsync(Cliente cliente)
    {
        var cookieClientId = TryGetClientIdFromCookie();
        if (cookieClientId.HasValue)
        {
            var fromCookie = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cookieClientId.Value);
            if (fromCookie != null) return fromCookie;
        }

        var cpf = NormalizeCpfCnpj(cliente.CpfCnpj);
        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var byCpf = await _context.Clientes.FirstOrDefaultAsync(c =>
                c.CpfCnpj != null
                && c.CpfCnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty).Replace(" ", string.Empty) == cpf);
            if (byCpf != null) return byCpf;
        }

        var email = (cliente.Email ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(email))
        {
            var byEmail = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
            if (byEmail != null) return byEmail;
        }

        return null;
    }

    private static string NormalizeCpfCnpj(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
