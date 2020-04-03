using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AlquilerDeLibros.Models;

namespace AlquilerDeLibros.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private Cartera db = new Cartera();

        [Authorize(Roles = "Admin, Librarian")]
        // GET: Libros
        public ActionResult Index()
        {
            var libros = db.Libros.Include(l => l.Materia);
            return View(libros.ToList());
        }

        // GET: Libros/Details/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(libro);
        }

        // GET: Libros/Create
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create()
        {
            ViewBag.MateriaId = new SelectList(db.Materias, "Id", "DescripcionDeMateria"); /*Aqui Mostrar Campo en create */
            return View();
        }

        // POST: Libros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create([Bind(Include = "Id,CodigoDeLibro,TituloDeLibro,ISBN,Autor,Portada,Adquisicion,Descripcion,MateriaId")] Libro libro)
        {
            //Esta es para acceder a los archivos
            HttpPostedFileBase ArchivoImagen = Request.Files[0];

            //Aqui se accede a los archivos para el usuario pueda subir una imagen
            if (ArchivoImagen.ContentLength == 0)
            {
                //Esto es para el caso de que el usuario no ha agregado imagen
                ModelState.AddModelError("Portada","Es necesario seleccionar una imagen");
            }
            else
            {
                //En esta parte, es para que se cargue al imagen en el sistenma y poder guardarlo
                WebImage img = new WebImage(ArchivoImagen.InputStream);
                libro.Portada = img.GetBytes();
            }

            Libro OLibro = db.Libros.DefaultIfEmpty(null).FirstOrDefault(l => l.CodigoDeLibro.Trim() == libro.CodigoDeLibro.Trim());
            if (OLibro != null)
            {
                ModelState.AddModelError("ErrAdd", "El codigo especificado ya existe");
            }

            if (ModelState.IsValid)
            {
                db.Libros.Add(libro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MateriaId = new SelectList(db.Materias, "Id", "DescripcionDeMateria", libro.MateriaId); /*Aqui Mostrar Campo Create */
            return View(libro);
        }

        // GET: Libros/Edit/5
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            ViewBag.MateriaId = new SelectList(db.Materias, "Id", "DescripcionDeMateria", libro.MateriaId); /*Aqui Mostrar Campo Edicion*/
            return View(libro);
        }

        // POST: Libros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Edit([Bind(Include = "Id,CodigoDeLibro,TituloDeLibro,ISBN,Autor,Portada,Adquisicion,Descripcion,MateriaId")] Libro libro)
        {
            // en esta parte unicamnete se utiliza para que el usuaruario sellecione otra imagen y pueda asi poder realizar un cambio
            Libro _libros = new Libro();

            HttpPostedFileBase ArchivoImagen = Request.Files[0];

            if (ArchivoImagen.ContentLength == 0)
            {
                _libros = db.Libros.Find(libro.Id);
                libro.Portada = _libros.Portada;
            }
            else
            {
                WebImage img = new WebImage(ArchivoImagen.InputStream);
                libro.Portada = img.GetBytes();
            }

            if (ModelState.IsValid)
            {
                db.Entry(_libros).State = EntityState.Detached;
                db.Entry(libro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MateriaId = new SelectList(db.Materias, "Id", "DescripcionDeMateria", libro.MateriaId); /*Aqui Mostrar Campo Edicion*/
            return View(libro);
        }

        // GET: Libros/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Error = "";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Libro libro = db.Libros.Find(id);
            CopiaDeLibro Ocopia = db.CopiasDelibro.DefaultIfEmpty(null).FirstOrDefault(c => c.LibroId == libro.Id);
            ViewBag.Error = "";
            if (Ocopia == null)
            {
                db.Libros.Remove(libro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Hay copias en este Libro";
            return (View());
        }

        [Authorize(Roles = "Admin, Librarian, Reporter")]
        public PartialViewResult Busqueda(string Titulo = "", string Autor = "")
        {
            string condicion = "";
            //Armar condicion según los datos que llenó
            if (Titulo.Trim().Length > 0)
                condicion += " && TituloDeLibro.Contains(\"" + Titulo.Trim() + "\")";
            if (!string.IsNullOrEmpty(Autor))
                condicion += " && Autor.Contains(\"" + Autor.Trim() + "\")";

            if (string.IsNullOrEmpty(condicion))
            {
                condicion = "1==0";  //Impedir que cargue si no hay filtro.
            }
            else
            {
                condicion = condicion.Substring(4); //Dejar la condición unicamente
            }


            var libros = db.Libros.Include("Materia")
                .Where(condicion);

            //Cambiar nombre al return de la vista parcial
            return PartialView("_BusquedaDeLibro", libros.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Se agrega una nueva metodo para q se pueda realizar que se pueda realizar un cambio en la imagen
        public ActionResult getImage(int id)
        {
            Libro librosk = db.Libros.Find(id);
            byte[] byteImg = librosk.Portada;

            MemoryStream memoryStream = new MemoryStream(byteImg);
            Image image = Image.FromStream(memoryStream);

            memoryStream = new MemoryStream();
            image.Save(memoryStream,ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return File(memoryStream,"image/jpg");
        }
    }
}
