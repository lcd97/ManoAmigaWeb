using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlquilerDeLibros.Models;
using System.Linq.Dynamic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Helpers;

namespace AlquilerDeLibros.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private Cartera db = new Cartera();

        // GET: Clientes
        [Authorize(Roles = "Librarian, Admin")]
        public ActionResult Index()
        {
            return View(db.Clientes.ToList());
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create([Bind(Include = "Id,CodigoDeCliente,NombresDelCliente,ApellidosDelCliente,Email")] Cliente cliente)
        {
            //HttpPostedFileBase ArchivoImagen = Request.Files[0];

            //if (ArchivoImagen.ContentLength == 0)
            //{
            //    cliente.Foto = null;
            //}
            //else
            //{
            //    WebImage img = new WebImage(ArchivoImagen.InputStream);
            //    cliente.Foto = img.GetBytes();

            //}
            Cliente OCliente = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == cliente.CodigoDeCliente.Trim());
            if (OCliente != null)
            {
                ModelState.AddModelError("CodigoDeCliente", "El codigo especificado ya existe");
            }

            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit([Bind(Include = "Id,CodigoDeCliente,NombresDelCliente,ApellidosDelCliente,Email")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Error = "";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            AlquilerDeLibro OAlquiler = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(al => al.ClienteId == cliente.Id);
            ViewBag.Error = "";
            if (OAlquiler == null)
            {
                db.Clientes.Remove(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Hay Alquileres en este Cliente";
            return (View());
        }

        [Authorize(Roles = "Admin, Librarian, Reporter")]
        public PartialViewResult Busqueda(string Nombres = "", string Apellido1 = "")
        {
            string condicion = "";
            //Armar condicion según los datos que llenó
            if (Nombres.Trim().Length > 0)
                condicion += " && NombresDelCliente.Contains(\"" + Nombres.Trim() + "\")";
            if (!string.IsNullOrEmpty(Apellido1))
                condicion += " && ApellidosDelCliente.Contains(\"" + Apellido1.Trim() + "\")";


            if (string.IsNullOrEmpty(condicion))
            {
                condicion = "1==0";  //Impedir que cargue si no hay filtro.
            }
            else
            {
                condicion = condicion.Substring(4); //Dejar la condición unicamente
            }


            var clientes = db.Clientes.Where(condicion);
            //ViewBag.dat = clientes.Count();
            //Cambiar nombre al return de la vista parcial
            return PartialView("_BusquedaDeCliente", clientes.ToList());
        }

        public ActionResult getImage(int id)
        {
            Cliente fotosk = db.Clientes.Find(id);
            byte[] byteImg = fotosk.Foto;

            if (fotosk.Foto == null)
            {

                Cliente cliente = db.Clientes.Find(id);

                return View(cliente);
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream(byteImg);
                Image image = Image.FromStream(memoryStream);

                memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Position = 0;
                return File(memoryStream, "image/jpg");
            }

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
