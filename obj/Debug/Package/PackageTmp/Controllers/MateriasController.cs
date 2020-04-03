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
    public class MateriasController : Controller
    {
        private Cartera db = new Cartera();

        [Authorize(Roles = "Admin, Librarian")]
        // GET: Materias
        public ActionResult Index()
        {
            return View(db.Materias.ToList());
        }

        [Authorize(Roles = "Admin, Librarian")]
        // GET: Materias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materia materia = db.Materias.Find(id);
            if (materia == null)
            {
                return HttpNotFound();
            }
            return View(materia);
        }

        [Authorize(Roles = "Admin, Librarian")]
        // GET: Materias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Materias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create([Bind(Include = "Id,CodigoDeMateria,DescripcionDeMateria")] Materia materia)
        {
            Materia OMateria = db.Materias.DefaultIfEmpty(null).FirstOrDefault(m => m.CodigoDeMateria.Trim() == materia.CodigoDeMateria.Trim());
            if (OMateria != null)
            {
                ModelState.AddModelError("ErrAdd", "El codigo especificado ya existe");
            }
            if (ModelState.IsValid)
            {
                db.Materias.Add(materia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(materia);
        }

        // GET: Materias/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materia materia = db.Materias.Find(id);
            if (materia == null)
            {
                return HttpNotFound();
            }
            return View(materia);
        }

        // POST: Materias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit([Bind(Include = "Id,CodigoDeMateria,DescripcionDeMateria")] Materia materia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(materia);
        }

        [Authorize(Roles = "Admin")]
        // GET: Materias/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.Error = "";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materia materia = db.Materias.Find(id);
            if (materia == null)
            {
                return HttpNotFound();
            }
            return View(materia);
        }

        // POST: Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Materia materia = db.Materias.Find(id);
            Libro Olibro = db.Libros.DefaultIfEmpty(null).FirstOrDefault(l => l.MateriaId == materia.Id);
            ViewBag.Error = "";
            if (Olibro == null)
            {
                db.Materias.Remove(materia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Hay Libros en esta Materia";
            return (View());
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
