using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlquilerDeLibros.Controllers
{
    //Toda llamada al controlador debe ser una peticion HTTPS -SOLO LA HACE "SEGURA"
    //[RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin, Reporter, Librarian")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}