using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Book _book { get; set; }
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            _book = new Book();
            if (! id.HasValue)
            {
                return View(_book);
            }
            _book = await _db.Books.FirstOrDefaultAsync(b=>b.Id == id);
            if (_book == null)
            {
                return NotFound();
            }
            return View(_book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert()
        {
            if(ModelState.IsValid)
            {
                if(_book.Id == 0)
                {
                    await _db.Books.AddAsync(_book);
                } 
                else
                {
                    _db.Books.Update(_book);
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(_book);
        }

        #region Api Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
                return Json(new { success = false, message = "Error while Deleting" });
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
