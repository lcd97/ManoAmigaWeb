using System.Web;
using System.Web.Optimization;

namespace AlquilerDeLibros
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/Scripts/jquery-{version}.js",
                        //"~/Scripts/jquery-ui.min.js",
                        //"~/Scripts/FechaEspanol.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include(
                        "~/Scripts/DataTable/datatable.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/webgrid.css",
                      "~/Content/jquery-ui.css"
                      //"~/Content/site.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/datatablecss").Include(
                      "~/Content/Plugins/datatables.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatablejs").Include(
                      "~/Scripts/Plugins/datatables.js"));



            bundles.Add(new ScriptBundle("~/bundles/Busqueda").Include(
                      "~/Scripts/Busqueda.js"));

            bundles.Add(new ScriptBundle("~/bundles/prestamo").Include(
                      "~/Scripts/calendarmod.js",
                      "~/Scripts/devolucion.js",
                      "~/Scripts/prestamo.js"));
            
            /***********************************************************
             *                  PLANTILLA ADMIN LTE                    *
             **********************************************************/
            bundles.Add(new StyleBundle("~/Content/adminCSS").Include(
                        "~/plugins/fontawesome-free/css/all.min.css",
                        "~/plugins/icheck-bootstrap/icheck-bootstrap.min.css",
                        "~/Content/ionicons.min.css",
                        "~/dist/css/adminlte.min.css",
                        "~/plugins/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css",
                        "~/plugins/overlayScrollbars/css/OverlayScrollbars.min.css"
                       /* "~/plugins/daterangepicker/daterangepicker.css"*/));

            bundles.Add(new ScriptBundle("~/bundles/adminJS").Include(
                      "~/plugins/jquery/jquery.min.js",    
                      //"~/plugins/jquery-ui/jquery-ui.min.js",
                      "~/plugins/bootstrap/js/bootstrap.bundle.min.js",
                      "~/plugins/chart.js/Chart.min.js",
                      "~/plugins/jquery-knob/jquery.knob.min.js",
                      "~/plugins/moment/moment.min.js",
                      "~/plugins/daterangepicker/daterangepicker.js",
                      "~/plugins/tempusdominus-bootstrap-4/js/tempusdominus-bootstrap-4.min.js",
                      "~/plugins/overlayScrollbars/js/jquery.overlayScrollbars.min.js",
                      "~/dist/js/adminlte.js",
                      "~/dist/js/demo.js"));

            bundles.Add(new ScriptBundle("~/bundles/LoginCSS").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/AdminLTE.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/LoginJS").Include(
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/icheck.min.js"));
        }
    }
}
