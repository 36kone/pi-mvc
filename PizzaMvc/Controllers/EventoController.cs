using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PizzaMvc.Data;
using PizzaMvc.Models;
using System.IO;

namespace PizzaMvc.Controllers
{
    public class EventoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventoController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Evento
        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Eventos.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var evento = await _context.Eventos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null) return NotFound();

            return Json(new
            {
                id = evento.Id,
                nome = evento.Nome,
                descricao = evento.Descricao,
                dataEvento = evento.DataEvento,
                local = evento.Local,
                image = evento.Image
            });
        }

        // GET: Evento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View("CriarEvento", evento);
        }

        // GET: Evento/Create
        public IActionResult Create()
        {
            return View("CriarEvento", new Evento { DataEvento = DateTime.Now });
        }

        // POST: Evento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,DataEvento,Local,Image")] Evento evento, IFormFile? imageFile)
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
                        return View("CriarEvento", evento);
                    }

                    try
                    {
                        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "eventos");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadsPath, fileName);

                        await using (var stream = System.IO.File.Create(fullPath))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        evento.Image = $"/uploads/eventos/{fileName}";
                    }
                    catch
                    {
                        ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                        return View("CriarEvento", evento);
                    }
                }

                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("CriarEvento", evento);
        }

        // GET: Evento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return View("CriarEvento", evento);
        }

        // POST: Evento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,DataEvento,Local,Image")] Evento evento, IFormFile? imageFile)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existing == null) return NotFound();

                    var imagePath = existing.Image;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                        var allowed = extension is ".png" or ".jpg" or ".jpeg" or ".webp" or ".gif";
                        if (!allowed)
                        {
                            ModelState.AddModelError(string.Empty, "Formato de imagem inválido. Use PNG, JPG, JPEG, WEBP ou GIF.");
                            evento.Image = imagePath;
                            return View("CriarEvento", evento);
                        }

                        try
                        {
                            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "eventos");
                            Directory.CreateDirectory(uploadsPath);

                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var fullPath = Path.Combine(uploadsPath, fileName);

                            await using (var stream = System.IO.File.Create(fullPath))
                            {
                                await imageFile.CopyToAsync(stream);
                            }

                            imagePath = $"/uploads/eventos/{fileName}";
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "Não foi possível salvar a imagem. Tente novamente.");
                            evento.Image = imagePath;
                            return View("CriarEvento", evento);
                        }
                    }

                    existing.Nome = evento.Nome;
                    existing.Descricao = evento.Descricao;
                    existing.DataEvento = evento.DataEvento;
                    existing.Local = evento.Local;
                    existing.Image = imagePath;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            return View("CriarEvento", evento);
        }

        // GET: Evento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View("Delete", evento);
        }

        // POST: Evento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
