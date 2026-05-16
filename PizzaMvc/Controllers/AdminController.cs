using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;

namespace PizzaMvc.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalPedidos = await _context.Pedidos.CountAsync();
        ViewBag.PedidosPendentes = await _context.Pedidos.CountAsync(p => p.Status == "Pendente");
        ViewBag.PedidosEmAndamento = await _context.Pedidos.CountAsync(p => p.Status == "Em Andamento");
        ViewBag.PedidosConcluidos = await _context.Pedidos.CountAsync(p => p.Status == "Concluido");
        return View();
    }

    public IActionResult CreatePizza()
    {
        return RedirectToAction(actionName: "Create", controllerName: "Pizza");
    }

    public IActionResult CreateBebida()
    {
        return RedirectToAction(actionName: "Create", controllerName: "Bebida");
    }

    public IActionResult CreateUsuario()
    {
        return RedirectToAction(actionName: "Create", controllerName: "Usuario");
    }
}
