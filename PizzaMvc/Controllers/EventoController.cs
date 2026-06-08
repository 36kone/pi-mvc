using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using PizzaMvc.Helpers;

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

        public IActionResult Create()
        {
            return View("CriarEvento", new Evento { DataEvento = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,DataEvento,Local,Image")] Evento evento, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "eventos");
                if (upload.Error != null)
                {
                    ModelState.AddModelError(string.Empty, upload.Error);
                    return View("CriarEvento", evento);
                }
                if (upload.Path != null) evento.Image = upload.Path;

                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("CriarEvento", evento);
        }

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

                    var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "eventos");
                    if (upload.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, upload.Error);
                        evento.Image = imagePath;
                        return View("CriarEvento", evento);
                    }
                    if (upload.Path != null) imagePath = upload.Path;

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
