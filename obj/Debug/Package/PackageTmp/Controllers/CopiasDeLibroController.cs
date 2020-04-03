using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlquilerDeLibros.Models;

namespace AlquilerDeLibros.Controllers
{
    [Authorize]
    public class CopiasDeLibroController : Controller
    {
        private Cartera db = new Cartera();

        // GET: CopiasDeLibro
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Index()
        {
            var copiasDelibro = db.CopiasDelibro.Include(c => c.Libro);
            return View(copiasDelibro.ToList());
        }

        // GET: CopiasDeLibro/Details/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CopiaDeLibro copiaDeLibro = db.CopiasDelibro.Find(id);
            if (copiaDeLibro == null)
            {
                return HttpNotFound();
            }
            return View(copiaDeLibro);
        }

        // GET: CopiasDeLibro/Create
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create()
        {
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "TituloDeLibro"); //MOD
            return View();
        }

        // POST: CopiasDeLibro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create([Bind(Include = "Id,NumeroCopia,LibroId")] CopiaDeLibro copiaDeLibro)
        {
            CopiaDeLibro OCopiaDeLibro = db.CopiasDelibro.DefaultIfEmpty(null).FirstOrDefault(cl => cl.NumeroCopia == copiaDeLibro.NumeroCopia && cl.LibroId == copiaDeLibro.LibroId);
            if (OCopiaDeLibro != null)
            {
                ModelState.AddModelError("NumeroCopia", "La copia ya existe para ese libro");
            }
            if (ModelState.IsValid)
            {
                db.CopiasDelibro.Add(copiaDeLibro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LibroId = new SelectList(db.Libros, "Id", "TituloDeLibro", copiaDeLibro.LibroId);
            return View(copiaDeLibro);
        }

        // GET: CopiasDeLibro/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CopiaDeLibro copiaDeLibro = db.CopiasDelibro.Find(id);
            if (copiaDeLibro == null)
            {
                return HttpNotFound();
            }
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "TituloDeLibro", copiaDeLibro.LibroId);
            return View(copiaDeLibro);
        }

        // POST: CopiasDeLibro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit([Bind(Include = "Id,NumeroCopia,LibroId")] CopiaDeLibro copiaDeLibro)
        {
            CopiaDeLibro OCopiaDeLibro = db.CopiasDelibro.DefaultIfEmpty(null).FirstOrDefault(cl => cl.NumeroCopia == copiaDeLibro.NumeroCopia && cl.LibroId == copiaDeLibro.LibroId);
            if (OCopiaDeLibro != null)
            {
                ModelState.AddModelError("ErrAdd", "La copia ya existe para ese libro");
            }
            if (ModelState.IsValid)
            {
                db.Entry(copiaDeLibro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "TituloDeLibro", copiaDeLibro.LibroId);
            return View(copiaDeLibro);
        }

        // GET: CopiasDeLibro/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Error = "";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CopiaDeLibro copiaDeLibro = db.CopiasDelibro.Find(id);
            if (copiaDeLibro == null)
            {
                return HttpNotFound();
            }
            return View(copiaDeLibro);
        }

        // POST: CopiasDeLibro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CopiaDeLibro copiaDeLibro = db.CopiasDelibro.Find(id);
            db.CopiasDelibro.Remove(copiaDeLibro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
