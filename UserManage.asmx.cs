using AlquilerDeLibros.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace AlquilerDeLibros
{
    /// <summary>
    /// Descripción breve de UserManage
    /// </summary>
    [WebService(Namespace = "http://manoamiga.somee.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class UserManage : System.Web.Services.WebService
    {

        #region Configuracion
        private ApplicationDbContext context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserManage()
        {
            context = new ApplicationDbContext();
        }

        public UserManage(ApplicationUserManager userManager, ApplicationSignInManager singInManager)
        {

        }

        public ApplicationSignInManager SingInManager {
            get {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        [WebMethod]
        public WebLoginResult IniciarSesion(string username, string contrasenia)
        {
            var result = SingInManager.PasswordSignIn(username, contrasenia, true, false);
            bool sesion = result == SignInStatus.Success;
            ApplicationUser usuario = null;
            if (sesion)
            {
                usuario = context.Users.First(x => x.UserName == username);
            }
            if (!sesion)
            {
                return new WebLoginResult
                {
                    IsLogged = false,
                    Mensaje = "Error al iniciar sesion",
                    UserName = username
                };
            }
            else
            {
                var roles = UserManager.GetRolesAsync(usuario.Id).Result;
                return new WebLoginResult
                {
                    Mensaje = "Bienvenido " + usuario.UserName + "!",
                    IsLogged = sesion,
                    UserId = usuario.Id,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Rol = roles.FirstOrDefault()
                };
            }
        }

        public class WebLoginResult
        {
            public string UserId { get;set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Rol { get; set; }
            public byte[] Imagen { get; set; }
            public string Mensaje { get; set; }
            public bool IsLogged { get; set; }
        }
    }
}
