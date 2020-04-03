using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlquilerDeLibros.Models;
using AlquilerDeLibros.Models.Extension;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace AlquilerDeLibros.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {
        private Cartera db = new Cartera();
        // GET: Consultas
        [Authorize(Roles = "Admin, Librarian, Reporter")]
        public ActionResult BuscarClientes()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Librarian, Reporter")]
        public ActionResult BuscarLibro()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Librarian, Reporter")]
        public ActionResult LibrosPrestados(string CodE = "0", string Desde = "1900/01/01", string Hasta = "1900/01/01")
        {
            Cliente oCli = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == CodE.Trim());
            int idCli = oCli == null ? -1 : oCli.Id;
            Desde = string.IsNullOrEmpty(Desde) ? "1900/01/01" : Desde;
            Hasta = string.IsNullOrEmpty(Hasta) ? "1900/01/01" : Hasta;

            ViewBag.idCli = idCli;
            ViewBag.desde = Desde;
            ViewBag.hasta = Hasta;

            if (oCli != null)
            {
                ViewBag.Nombre = oCli.NombreCompleto;
            }


            var q = db.Database.SqlQuery<cLstAlquiler>("dbo.LibrosPrestados @pId,@Desde,@Hasta",
                new SqlParameter("@pId", idCli),
                new SqlParameter("@Desde", DateTime.Parse(Desde)),
                new SqlParameter("@Hasta", DateTime.Parse(Hasta)));
            return View(q.ToList());
        }// fin de creditos otorgados

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }//dispose
    }
}