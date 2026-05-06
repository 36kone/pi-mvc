using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;

namespace PizzaMvc.Controllers;

public class UsuarioController : Controller
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Usuarios.ToListAsync());
    }

    public IActionResult Create()
    {
        return View("CriarUsuario", new Usuario());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        if (!ModelState.IsValid) return View("CriarUsuario", usuario);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        return View("EditarUsuario", usuario);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(int id, Usuario usuario)
    {
        if (id != usuario.Id) return NotFound();
        if (!ModelState.IsValid) return View("EditarUsuario", usuario);

        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        return View(usuario);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
