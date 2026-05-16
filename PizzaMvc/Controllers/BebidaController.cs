using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using PizzaMvc.Data;
using PizzaMvc.Models;
using System.IO;

namespace PizzaMvc.Controllers
{
    public class BebidaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BebidaController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Bebida
        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Bebidas.ToListAsync());
        }

        // GET: Bebida/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas.FirstOrDefaultAsync(m => m.Id == id);
            if (bebida == null)
            {
                return NotFound();
            }

            return View("CriarBebida", bebida);
        }

        // GET: Bebida/Create
        public IActionResult Create()
        {
            return View("CriarBebida", new Bebida());
        }

        // POST: Bebida/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sabor,Descricao,Preco,Categoria,Image")] Bebida bebida, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    var allowed = extension is ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif";
                    if (!allowed)
                    {
                        ModelState.AddModelError(string.Empty, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");
                        return View("CriarBebida", bebida);
                    }

                    try
                    {
                        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "bebidas");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadsPath, fileName);

                        await using (var stream = System.IO.File.Create(fullPath))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        bebida.Image = $"/uploads/bebidas/{fileName}";
                    }
                    catch
                    {
                        ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                        return View("CriarBebida", bebida);
                    }
                }

                _context.Add(bebida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("CriarBebida", bebida);
        }

        // GET: Bebida/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida == null)
            {
                return NotFound();
            }
            return View("CriarBebida", bebida);
        }

        // POST: Bebida/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sabor,Descricao,Preco,Categoria,Image")] Bebida bebida, IFormFile? imageFile)
        {
            if (id != bebida.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Bebidas.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existing == null) return NotFound();

                    var imagePath = existing.Image;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                        var allowed = extension is ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif";
                        if (!allowed)
                        {
                            ModelState.AddModelError(string.Empty, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");
                            bebida.Image = imagePath;
                            return View("CriarBebida", bebida);
                        }

                        try
                        {
                            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "bebidas");
                            Directory.CreateDirectory(uploadsPath);

                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var fullPath = Path.Combine(uploadsPath, fileName);

                            await using (var stream = System.IO.File.Create(fullPath))
                            {
                                await imageFile.CopyToAsync(stream);
                            }

                            imagePath = $"/uploads/bebidas/{fileName}";
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                            bebida.Image = imagePath;
                            return View("CriarBebida", bebida);
                        }
                    }

                    existing.Nome = bebida.Nome;
                    existing.Sabor = bebida.Sabor;
                    existing.Descricao = bebida.Descricao;
                    existing.Preco = bebida.Preco;
                    existing.Categoria = bebida.Categoria;
                    existing.Image = imagePath;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BebidaExists(bebida.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("CriarBebida", bebida);
        }

        // GET: Bebida/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bebida = await _context.Bebidas.FirstOrDefaultAsync(m => m.Id == id);
            if (bebida == null)
            {
                return NotFound();
            }

            return View(bebida);
        }

        // POST: Bebida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bebida = await _context.Bebidas.FindAsync(id);
            if (bebida != null)
            {
                _context.Bebidas.Remove(bebida);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BebidaExists(int id)
        {
            return _context.Bebidas.Any(e => e.Id == id);
        }
    }
}
