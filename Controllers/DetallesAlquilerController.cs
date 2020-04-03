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
    public class DetallesAlquilerController : Controller
    {
        private Cartera db = new Cartera();

        // GET: DetallesAlquiler
        public ActionResult Index()
        {
            var detallesAlquiler = db.DetallesAlquiler.Include(d => d.Alquiler).Include(d => d.Copia);
            return View(detallesAlquiler.ToList());
        }

        // GET: DetallesAlquiler/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleAlquiler detalleAlquiler = db.DetallesAlquiler.Find(id);
            if (detalleAlquiler == null)
            {
                return HttpNotFound();
            }
            return View(detalleAlquiler);
        }

        // GET: DetallesAlquiler/Create
        public ActionResult Create()
        {
            ViewBag.AlquilerId = new SelectList(db.AlquileresDeLibro, "Id", "CodigoAlquiler");
            ViewBag.CopiaId = new SelectList(db.CopiasDelibro, "Id", "Id");
            return View();
        }

        // POST: DetallesAlquiler/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AlquilerId,CopiaId")] DetalleAlquiler detalleAlquiler)
        {
            if (ModelState.IsValid)
            {
                db.DetallesAlquiler.Add(detalleAlquiler);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AlquilerId = new SelectList(db.AlquileresDeLibro, "Id", "CodigoAlquiler", detalleAlquiler.AlquilerId);
            ViewBag.CopiaId = new SelectList(db.CopiasDelibro, "Id", "Id", detalleAlquiler.CopiaId);
            return View(detalleAlquiler);
        }

        // GET: DetallesAlquiler/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleAlquiler detalleAlquiler = db.DetallesAlquiler.Find(id);
            if (detalleAlquiler == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlquilerId = new SelectList(db.AlquileresDeLibro, "Id", "CodigoAlquiler", detalleAlquiler.AlquilerId);
            ViewBag.CopiaId = new SelectList(db.CopiasDelibro, "Id", "Id", detalleAlquiler.CopiaId);
            return View(detalleAlquiler);
        }

        // POST: DetallesAlquiler/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AlquilerId,CopiaId")] DetalleAlquiler detalleAlquiler)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalleAlquiler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AlquilerId = new SelectList(db.AlquileresDeLibro, "Id", "CodigoAlquiler", detalleAlquiler.AlquilerId);
            ViewBag.CopiaId = new SelectList(db.CopiasDelibro, "Id", "Id", detalleAlquiler.CopiaId);
            return View(detalleAlquiler);
        }

        // GET: DetallesAlquiler/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleAlquiler detalleAlquiler = db.DetallesAlquiler.Find(id);
            if (detalleAlquiler == null)
            {
                return HttpNotFound();
            }
            return View(detalleAlquiler);
        }

        // POST: DetallesAlquiler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetalleAlquiler detalleAlquiler = db.DetallesAlquiler.Find(id);
            db.DetallesAlquiler.Remove(detalleAlquiler);
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
