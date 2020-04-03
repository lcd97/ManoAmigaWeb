using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using AlquilerDeLibros.Models;

namespace AlquilerDeLibros.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private Cartera db = new Cartera();

        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Principal(string CodE = "0")
        {
            var alquileres = db.DetallesAlquiler.Include(al => al.Alquiler).Include(al => al.Alquiler.Cliente).Include(al => al.Copia).Include(al => al.Copia.Libro).Where(al => al.Alquiler.Cliente.CodigoDeCliente.Trim() == CodE.Trim() && al.Alquiler.FechaRealDevolucion.Year == 1900);
            var oCli = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == CodE.Trim());

            if (oCli != null)
            {
                ViewBag.Codigo = oCli.CodigoDeCliente.ToString();
                ViewBag.NombCompleto = oCli.NombreCompleto;
            }
            else
            {
                ViewBag.Codigo = "";
                ViewBag.NombCompleto = "";
            }
            return View(alquileres.ToList());
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpGet]
        public ActionResult Index(string pCodE)
        {
            AlquilerView alquilerview = new AlquilerView();
            alquilerview.Cliente = new AlquilerDeLibro();
            alquilerview.CopiasLibro = new List<CopiaAlquiler>();

            //VARIABLE DE SESION PARA NO PERDER LOS DATOS EN EL TRASLADO DE VISTAS
            Session["AlquilerView"] = alquilerview;
            //CARGAR LA LISTA DE CLIENTES
            var list = db.Clientes.ToList();
            ViewBag.Id = new SelectList(db.Clientes.Where(x => x.CodigoDeCliente == pCodE), "Id", "NombreCompleto");
            return View(alquilerview);
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost]
        public ActionResult Index(AlquilerView alquilerview)
        {
            alquilerview = Session["AlquilerView"] as AlquilerView;
            string codAlq = Request["Cliente.CodigoAlquiler"];


            AlquilerDeLibro oalq = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoAlquiler.Trim() == codAlq.Trim());

            if (oalq != null)
            {
                ModelState.AddModelError("Cliente.CodigoAlquiler", "El Codigo especificado ya existe");
            }
            else
            {
                int idcliente = int.Parse(Request["Id"]);
                DateTime fechaalquiler = Convert.ToDateTime(Request["Cliente.FechaAlquiler"]);
                DateTime fechadevo = Convert.ToDateTime(Request["Cliente.FechaDevo"]);

                //GUARDANDO EL ENCABEZADO
                AlquilerDeLibro nuevoalquiler = new AlquilerDeLibro
                {
                    CodigoAlquiler = codAlq,
                    ClienteId = idcliente,
                    FechaAlquiler = fechaalquiler,
                    FechaDevo = fechadevo,
                    FechaRealDevolucion = Convert.ToDateTime("01/01/1900")
                };
                db.AlquileresDeLibro.Add(nuevoalquiler);
                db.SaveChanges();

                int lastId = db.AlquileresDeLibro.ToList().Select(al => al.Id).Max();

                //GUARDANDO EL DETALLE
                foreach (CopiaAlquiler item in alquilerview.CopiasLibro)
                {
                    var detail = new DetalleAlquiler()
                    {
                        AlquilerId = lastId,
                        CopiaId = item.Id
                    };
                    db.DetallesAlquiler.Add(detail);
                }
                db.SaveChanges();

                //Limpiar datos de la vista
                ModelState.Clear();
                return RedirectToAction("Principal");
            }


            alquilerview = Session["AlquilerView"] as AlquilerView;
            var list = db.Clientes.ToList();
            ViewBag.Id = new SelectList(list, "Id", "NombreCompleto");
            return View(alquilerview);
        }

        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Librarian")]
        //Agregar libro a la tabla
        public ActionResult Add(string titulo, int numcopia, int id)
        {
            var alquilerview = Session["AlquilerView"] as AlquilerView;
            CopiaAlquiler d = new CopiaAlquiler();
            d.Id = id;
            d.TituloDeLibro = titulo;
            d.NumeroCopia = numcopia;
            if (alquilerview.CopiasLibro.Where(x => x.TituloDeLibro == d.TituloDeLibro && x.NumeroCopia == d.NumeroCopia).FirstOrDefault() == null)
                alquilerview.CopiasLibro.Add(d);
            else
                d.Id = -5;

            return Json(new { d }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Remove(string titulo, int numcopia)
        {
            var alquilerview = Session["AlquilerView"] as AlquilerView;

            var dato = alquilerview.CopiasLibro.Where(x => x.TituloDeLibro == titulo && x.NumeroCopia == numcopia).FirstOrDefault();
            var d = alquilerview.CopiasLibro.Remove(dato);

            return Json(new { d }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Librarian")]
        public PartialViewResult AgregarLibro()
        {
            ViewBag.CopiaId = new SelectList(db.CopiasDelibro, "Id", "NumeroCopia");
            ViewBag.LibroId = new SelectList(db.Libros, "Id", "TituloDeLibro");
            return PartialView("_AgregarLibro");
        }

        [Authorize(Roles = "Admin, Librarian")]
        //OBTENER LAS COPIAS DE LIBROS DISPONIBLES
        public ActionResult GetElementsCopias(int id = 0)
        {
            List<CopiaDeLibro> elements = (from c in db.CopiasDelibro
                                           where c.LibroId == id &&
                                           !(from a in db.AlquileresDeLibro
                                             join d in db.DetallesAlquiler on a.Id equals d.AlquilerId
                                             where a.FechaRealDevolucion.Year == 1900
                                             select d.CopiaId).Contains(c.Id)
                                           select c).ToList();

            if (elements == null)
                throw new ArgumentException("Categoría no es correcta");
            var data = new
            {
                rows = from c
                       in elements
                       select new
                       {
                           id = c.Id,
                           NumeroCopia = c.NumeroCopia.ToString()
                       }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /************************************************************************************************************************
         *                                  DEVOLUCIONES                                                                        *
         ***********************************************************************************************************************/
        //ENTRADA DE DEVOLUCION
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Devolucion(string CodE = "0")
        {
            var alquileres = db.AlquileresDeLibro.Include(al => al.DetallesAlquiler).Include(al => al.Cliente).Where(al => al.Cliente.CodigoDeCliente.Trim() == CodE.Trim() && al.FechaRealDevolucion.Year == 1900);
            var oCli = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == CodE.Trim());

            if (oCli != null)
            {
                ViewBag.Codigo = oCli.CodigoDeCliente.ToString();
                ViewBag.NombCompleto = oCli.NombreCompleto;
            }
            else
            {
                ViewBag.Codigo = "";
                ViewBag.NombCompleto = "";
            }
            return View(alquileres.ToList());
        }

        //GET DE LA VISTA PARCIAL DEVOLUCION
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult Detalle(/*int? id*/)
        {
            return View();
        }

        [Authorize(Roles = "Admin, Librarian")]
        //LLAMADO A VISTA PARCIAL
        public PartialViewResult DevolverAlquiler()
        {
            return PartialView("_DevolucionAlquiler");
        }

        //LLENADO DE ALQUILER
        [Authorize(Roles = "Admin, Librarian")]
        public ActionResult loadPrest(int id)
        {
            AlquilerDeLibro alq = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(a => a.Id == id);
            var data = new
            {
                Id = alq.Id,
                CodigoAlquiler = alq.CodigoAlquiler,
                FechaAlquiler = alq.FechaAlquiler.ToString("dd/MM/yyyy"),
                FechaDevo = alq.FechaDevo.ToString("dd/MM/yyyy"),
                FechaRealDevolucion = alq.FechaRealDevolucion,
                NombreCompleto = alq.Cliente.NombreCompleto,
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //LLENADO DE DETALLE ALQUILER
        [Authorize(Roles = "Admin, Librarian")]
        public JsonResult loadDetails(int id)
        {
            var obj = db.DetallesAlquiler.Where(d => d.AlquilerId == id).ToList();

            var list = (from a in obj
                        select new
                        {
                            TituloDeLibro = a.Copia.Libro.TituloDeLibro,
                            NumeroCopia = a.Copia.NumeroCopia,
                        });

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        //EDICION DE ALQUILERES (DEVOLUCION)
        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost]
        public ActionResult EditDev(string codAlq, DateTime fechaDev)
        {
            int d = -2;
            AlquilerDeLibro alquiler = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(a => a.CodigoAlquiler.Trim() == codAlq.Trim());

            //Modificar fecha de devolucion
            alquiler.FechaRealDevolucion = fechaDev;
            db.Entry(alquiler).State = EntityState.Modified;
            d = db.SaveChanges();


            return Json(new { d }, JsonRequestBehavior.AllowGet);
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