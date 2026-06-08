using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMvc.Data;
using PizzaMvc.Models;
using PizzaMvc.Helpers;

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

        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Bebidas.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var bebida = await _context.Bebidas
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bebida == null) return NotFound();

            return Json(new
            {
                id = bebida.Id,
                nome = bebida.Nome,
                sabor = bebida.Sabor,
                descricao = bebida.Descricao,
                preco = bebida.Preco,
                categoria = bebida.Categoria,
                image = bebida.Image
            });
        }

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

        public IActionResult Create()
        {
            return View("CriarBebida", new Bebida());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Sabor,Descricao,Preco,Categoria,Image")] Bebida bebida, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "bebidas");
                if (upload.Error != null)
                {
                    ModelState.AddModelError(string.Empty, upload.Error);
                    return View("CriarBebida", bebida);
                }
                if (upload.Path != null) bebida.Image = upload.Path;

                _context.Add(bebida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("CriarBebida", bebida);
        }

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

                    var upload = await ImageUploadHelper.SaveAsync(imageFile, _env, "bebidas");
                    if (upload.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, upload.Error);
                        bebida.Image = imagePath;
                        return View("CriarBebida", bebida);
                    }
                    if (upload.Path != null) imagePath = upload.Path;

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
