using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AlquilerDeLibros.Models;

[assembly: OwinStartupAttribute(typeof(AlquilerDeLibros.Startup))]
namespace AlquilerDeLibros
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CrearPermisos();
        }

        private void CrearPermisos()
        {
            ApplicationDbContext contex = new ApplicationDbContext();
            var AdmPermisos = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(contex));
            var AdmUsuario = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(contex));
            IdentityRole permiso = new IdentityRole();

            if (!AdmPermisos.RoleExists("Admin"))
            {
                permiso = new IdentityRole();
                permiso.Name = "Admin";
                AdmPermisos.Create(permiso);

                var usuario = new ApplicationUser();

                usuario.UserName = "josielcastillov@gmail.com";
                usuario.Email = "josielcastillov@gmail.com";

                var resultado = AdmUsuario.Create(usuario, "Jcv100798..");

                if (resultado.Succeeded)
                {
                    AdmUsuario.AddToRole(usuario.Id, "Admin");
                }
            }

            if (!AdmPermisos.RoleExists("User"))
            {
                permiso = new IdentityRole();
                permiso.Name = "User";
                AdmPermisos.Create(permiso);

            }

            if (!AdmPermisos.RoleExists("Librarian"))
            {
                permiso = new IdentityRole();
                permiso.Name = "Librarian";
                AdmPermisos.Create(permiso);

            }

            if (!AdmPermisos.RoleExists("Reporter"))
            {
                permiso = new IdentityRole();
                permiso.Name = "Reporter";
                AdmPermisos.Create(permiso);

            }
        }
    }
}
