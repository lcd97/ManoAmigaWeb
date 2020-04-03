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
    public class ValoracionesLibroController : Controller
    {
        private Cartera db = new Cartera();

        // GET: ValoracionesLibro
        public ActionResult Index()
        {
            var valoracionLibro = db.ValoracionLibro.Include(v => v.Cliente).Include(v => v.Libro);
            return View(valoracionLibro.ToList());
        }

        // GET: ValoracionesLibro/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ValoracionLibro valoracionLibro = db.ValoracionLibro.Find(id);
            if (valoracionLibro == null)
            {
                return HttpNotFound();
            }
            return View(valoracionLibro);
        }

        // GET: ValoracionesLibro/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente");
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "CodigoDeLibro");
            return View();
        }

        // POST: ValoracionesLibro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Puntaje,Comentario,Sugerencia,ClienteId,LibroId,FechaValoracion")] ValoracionLibro valoracionLibro)
        {
            if (ModelState.IsValid)
            {
                db.ValoracionLibro.Add(valoracionLibro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", valoracionLibro.ClienteId);
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "CodigoDeLibro", valoracionLibro.LibroId);
            return View(valoracionLibro);
        }

        // GET: ValoracionesLibro/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ValoracionLibro valoracionLibro = db.ValoracionLibro.Find(id);
            if (valoracionLibro == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", valoracionLibro.ClienteId);
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "CodigoDeLibro", valoracionLibro.LibroId);
            return View(valoracionLibro);
        }

        // POST: ValoracionesLibro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Puntaje,Comentario,Sugerencia,ClienteId,LibroId,FechaValoracion")] ValoracionLibro valoracionLibro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(valoracionLibro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", valoracionLibro.ClienteId);
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "CodigoDeLibro", valoracionLibro.LibroId);
            return View(valoracionLibro);
        }

        // GET: ValoracionesLibro/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ValoracionLibro valoracionLibro = db.ValoracionLibro.Find(id);
            if (valoracionLibro == null)
            {
                return HttpNotFound();
            }
            return View(valoracionLibro);
        }

        // POST: ValoracionesLibro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ValoracionLibro valoracionLibro = db.ValoracionLibro.Find(id);
            db.ValoracionLibro.Remove(valoracionLibro);
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
