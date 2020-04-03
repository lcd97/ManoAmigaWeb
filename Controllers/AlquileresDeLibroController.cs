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
    public class AlquileresDeLibroController : Controller
    {
        private Cartera db = new Cartera();

        // GET: AlquileresDeLibro
        public ActionResult Index()
        {
            var alquileresDeLibro = db.AlquileresDeLibro.Include(a => a.Cliente);
            return View(alquileresDeLibro.ToList());
        }

        // GET: AlquileresDeLibro/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlquilerDeLibro alquilerDeLibro = db.AlquileresDeLibro.Find(id);
            if (alquilerDeLibro == null)
            {
                return HttpNotFound();
            }
            return View(alquilerDeLibro);
        }

        // GET: AlquileresDeLibro/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente");
            return View();
        }

        // POST: AlquileresDeLibro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CodigoAlquiler,FechaAlquiler,FechaRealDevolucion,ClienteId")] AlquilerDeLibro alquilerDeLibro)
        {
            if (ModelState.IsValid)
            {
                db.AlquileresDeLibro.Add(alquilerDeLibro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", alquilerDeLibro.ClienteId);
            return View(alquilerDeLibro);
        }

        // GET: AlquileresDeLibro/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlquilerDeLibro alquilerDeLibro = db.AlquileresDeLibro.Find(id);
            if (alquilerDeLibro == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", alquilerDeLibro.ClienteId);
            return View(alquilerDeLibro);
        }

        // POST: AlquileresDeLibro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CodigoAlquiler,FechaAlquiler,FechaRealDevolucion,ClienteId")] AlquilerDeLibro alquilerDeLibro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alquilerDeLibro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "CodigoDeCliente", alquilerDeLibro.ClienteId);
            return View(alquilerDeLibro);
        }

        // GET: AlquileresDeLibro/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlquilerDeLibro alquilerDeLibro = db.AlquileresDeLibro.Find(id);
            if (alquilerDeLibro == null)
            {
                return HttpNotFound();
            }
            return View(alquilerDeLibro);
        }

        // POST: AlquileresDeLibro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AlquilerDeLibro alquilerDeLibro = db.AlquileresDeLibro.Find(id);
            db.AlquileresDeLibro.Remove(alquilerDeLibro);
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
