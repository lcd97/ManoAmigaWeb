using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Services;
using AlquilerDeLibros.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace AlquilerDeLibros
{
    /// <summary>
    /// Descripción breve de LibrarySW
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class LibrarySW : System.Web.Services.WebService
    {
        //INSTANCIAS DE LAS BASES DE DATOS (MODELO Y SEGURIDAD)
        #region Configuracion

        Cartera db = new Cartera();
        ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LibrarySW()
        {
            context = new ApplicationDbContext();
        }

        public LibrarySW(ApplicationUserManager userManager, ApplicationSignInManager singInManager)
        {

        }
        public ApplicationSignInManager SingInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion


        /***************************************************************************************************************************
         *                                  INICIO DE SERVICIOS DE PARTE DEL CLIENTE                                               *
         ***************************************************************************************************************************/

        /// <summary>
        /// CREA LAS CREDENCIALES PARA LOS CLIENTES QUE SE REGISTREN
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Pass"></param>
        /// <param name="permiso"></param>
        /// <param name="Codigo"></param>
        /// <param name="Nombres"></param>
        /// <param name="Apellidos"></param>
        /// <returns>RETORNA UN OBJETO QUE CONTIENE UN MENSAJE DE REALIZACION O ERROR</returns>
        [WebMethod]
        public WebRegisterResult AccountSecurity(string Email, string Pass, string permiso, string Codigo, string Nombres, string Apellidos)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            WebRegisterResult Register = new WebRegisterResult();

            //PARA COMPROBAR MUCHAS COSAS, EMAIL EXISTENTE Y MODIFICACION
            Cliente cliente = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == Codigo.Trim());

            //SI EL CLIENTE EXISTE
            if (cliente != null)
            {
                //SI EL CLIENTE EXISTE Y TIENE UN EMAIL ASOCIADO
                if (cliente.Email != null)
                {
                    Register.Register = false;
                    Register.Mensaje = "La cuenta especificada esta asociado con otro correo";
                    Register.Codigo = "0";
                    Register.Email = "0";
                    Register.Action = "EmailModelo";
                }
                else
                {
                    //SE ALMACENA LAS CREDENCIALES EN EL OBJETO               
                    var user = new ApplicationUser();
                    user.UserName = Email;
                    user.Email = Email;
                    string userPWD = Pass;

                    //SE CREA LA CUENTA DE SEGURIDAD
                    var chkUser = UserManager.Create(user, userPWD);

                    //SI SE CREO LA CUENTA SATISFACTORIAMENTE
                    if (chkUser.Succeeded)
                    {
                        //AÑADIR EL ROL DEL USUARIO
                        var result1 = UserManager.AddToRole(user.Id, permiso);

                        //NO TIENE CUENTA ASOCIADA PERO EXISTE
                        cliente.Email = Email;

                        db.Entry(cliente).State = EntityState.Modified;
                        db.SaveChanges();

                        Register.Register = true;
                        Register.Mensaje = "Cuenta creada exitosamente";
                        Register.Codigo = Codigo;
                        Register.Email = Email;
                        Register.Action = "Modificacion";
                    }
                    else//Y SI NO SE CREO LA DE SEGURIDAD MANDAR FALSO TAMBIEN
                    {
                        //SI NO SE CREO LA CUENTA EN EL MODELO DE SEGURIDAD
                        Register.Register = false;
                        Register.Mensaje = "El correo especificado se encuentra asociado con otra cuenta";
                        Register.Codigo = "0";
                        Register.Email = "0";
                        Register.Action = "EmailSeguridad";
                    }
                }
            }//SI EL CLIENTE ES NUEVO
            else
            {
                //SE ALMACENA LAS CREDENCIALES EN EL OBJETO               
                var user = new ApplicationUser();
                user.UserName = Email;
                user.Email = Email;
                string userPWD = Pass;

                //SE CREA LA CUENTA DE SEGURIDAD
                var chkUser = UserManager.Create(user, userPWD);

                //SI SE CREO LA CUENTA SATISFACTORIAMENTE
                if (chkUser.Succeeded)
                {
                    //AÑADIR EL ROL DEL USUARIO
                    var result1 = UserManager.AddToRole(user.Id, permiso);
                    Cliente persona = new Cliente();
                    //AGREGAR TODOS LOS CAMPOS DEL MODELO
                    persona.CodigoDeCliente = Codigo.Trim();
                    persona.NombresDelCliente = Nombres.Trim();
                    persona.ApellidosDelCliente = Apellidos.Trim();
                    persona.Email = Email.Trim();

                    db.Clientes.Add(persona);
                    db.SaveChanges();

                    Register.Register = true;
                    Register.Mensaje = "Cuenta creada exitosamente completamente";
                    Register.Codigo = Codigo;
                    Register.Email = Email;
                    Register.Action = "NuevoRegistro";
                }
                else
                {
                    //SI NO SE CREO LA CUENTA EN EL MODELO DE SEGURIDAD
                    Register.Register = false;
                    Register.Mensaje = "El correo especificado se encuentra asociado con otra cuenta";
                    Register.Codigo = "0";
                    Register.Email = "0";
                    Register.Action = "EmailSeguridad";
                }

            }

            return Register;
        }

        /// <summary>
        /// REALIZA EL INICIO DE SESION DEL CLIENTE
        /// </summary>
        /// <param name="username"></param>
        /// <param name="contrasenia"></param>
        /// <returns>RETORNA UN OBJETO QUE CONTIENE UN MENSAJE DE INGRESO O ERROR</returns>
        [WebMethod]
        public WebLoginResult Login(string username, string contrasenia)
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

        /// <summary>
        /// OBTIENE LOS DATOS DE UN CLIENTE EN ESPECIFICO
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns>RETORNA UN OBJETO CLIENTE</returns>
        [WebMethod]
        public CustomerWS CustomerData(string email)
        {
            Cliente customer = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.Email.Trim() == email.Trim());
            if (customer == null)
                return null;
            else
                return db.Clientes.Where(c => c.Email.Trim() == email.Trim()).Select(c => new CustomerWS()
                {
                    Id = c.Id,
                    Codigo = c.CodigoDeCliente,
                    Nombres = c.NombresDelCliente,
                    Apellidos = c.ApellidosDelCliente,
                    Email = c.Email,
                    Foto = c.Foto
                }).FirstOrDefault();
        }

        /// <summary>
        /// OBTIENE LOS DATOS DEL CLIENTE PARA UN ALQUILER
        /// </summary>
        /// <param name="Code">CEDULA</param>
        /// <returns></returns>
        [WebMethod]
        public CustomerWS CustomerInfo(string Code)
        {
            var customer = db.Clientes.Where(c => c.CodigoDeCliente.Trim() == Code.Trim()).Select(c => new CustomerWS()
            {
                Id = c.Id,
                Codigo = c.CodigoDeCliente,
                 Nombres = c.NombresDelCliente,
                 Apellidos = c.ApellidosDelCliente,
                 Email = c.Email,
                 Foto = c.Foto
            }).FirstOrDefault();

            if (customer != null)
                return customer;
            else
                return null;
        }

        /// <summary>
        /// LISTA DE TODOS LIBROS
        /// </summary>
        /// <returns>RETORNA UNA LISTA DE TODOS LOS LIBROS</returns>
        [WebMethod]
        public List<BookWS> BookList()
        {
            return db.Libros.Select(l => new BookWS() {
                Id = l.Id,
                Codigo = l.CodigoDeLibro,
                Titulo = l.TituloDeLibro,
                ISBN = l.ISBN,
                Autor = l.Autor,
                Portada = l.Portada,
                Adquisicion = l.Adquisicion,
                MateriaId = l.MateriaId,
                Descripcion = l.Descripcion
            }).ToList();
        }

        /// <summary>
        /// LISTA DE TODAS LAS MATERIAS
        /// </summary>
        /// <returns>RETORNA UNA LISTA DE TODAS LAS CATEGORIAS</returns>
        [WebMethod]
        public List<CategoryWS> BookCategory()
        {
            return db.Materias.Select(c => new CategoryWS() {
                Id = c.Id,
                Codigo = c.CodigoDeMateria,
                Descripcion = c.DescripcionDeMateria
            }).ToList();
        }

        /// <summary>
        /// LISTA LOS LIBROS POR CATEGORIAS
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns>RETORNA UNA LISTA DE LIBROS FILTRADAS</returns>
        [WebMethod]
        public List<BookWS> BookListByCategory(int CategoryId)
        {
            return db.Libros.Where(l => l.MateriaId == CategoryId).Select(l => new BookWS()
            {
                Id = l.Id,
                Codigo = l.CodigoDeLibro,
                Titulo = l.TituloDeLibro,
                ISBN = l.ISBN,
                Autor = l.Autor,
                Portada = l.Portada,
                Adquisicion = l.Adquisicion,
                MateriaId = l.MateriaId,
                Descripcion = l.Descripcion
            }).ToList();
        }

        /// <summary>
        /// LISTA TODOS LOS LIBROS NUEVOS
        /// </summary>
        /// <param name="fecha">LA FECHA DEBE SER OBTENIDA DEL DIA ACTUAL</param>
        /// <returns>RETORNA UNA LISTA DE TODOS LOS LIBROS NUEVOS (DEL MES)</returns>
        [WebMethod]
        public List<BookWS> NewBooks(DateTime fecha)
        {

            return db.Libros.Where(l => l.Adquisicion.Month == fecha.Month).Select(l => new BookWS()
            {
                Id = l.Id,
                Codigo = l.CodigoDeLibro,
                Titulo = l.TituloDeLibro,
                ISBN = l.ISBN,
                Autor = l.Autor,
                Portada = l.Portada,
                Adquisicion = l.Adquisicion,
                MateriaId = l.MateriaId,
                Descripcion = l.Descripcion
            }).ToList();
        }

        /// <summary>
        /// LISTA TODOS LOS PRESTAMOS REALIZADOS POR EL CLIENTE
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns>RETORNA UNA LISTA DE TODOS LOS PRESTAMOS DEL CLIENTE</returns>
        [WebMethod]
        public List<RentalWS> RentalByCustomer(int CustomerId)
        {
            return db.AlquileresDeLibro.Where(a => a.ClienteId == CustomerId).Select(a => new RentalWS() {
                Id = a.Id,
                Codigo = a.CodigoAlquiler,
                FechaAlquiler = a.FechaAlquiler,
                FechaDevolucion = a.FechaDevo,
                FechaRealDevolucion = a.FechaRealDevolucion,
                ClienteId = a.ClienteId
            }).ToList();
        }

        /// <summary>
        /// LISTA DE TODOS LOS LIBROS RENTADOS POR CLIENTE
        /// </summary>
        /// <param name="RentalId">ID DEL ALQUILER</param>
        /// <returns>RETORNA UNA LISTA DE TODOS LOS LIBROS ALQUILADOS</returns>
        [WebMethod]
        public List<RentalDetailsWS> RentalDetails(int RentalId)
        {
            return db.DetallesAlquiler
                .Join(db.CopiasDelibro, d => d.CopiaId, c => c.Id, (d, c) => new { d, c })
                .Join(db.Libros, p => p.c.LibroId, l => l.Id, (p, l) => new { p, l })
                .Where(d => d.p.d.AlquilerId == RentalId).Select(d => new RentalDetailsWS() {
                    Id = d.p.d.Id,
                    CopiaId = d.p.d.CopiaId,
                    AlquilerId = d.p.d.AlquilerId,
                    TituloLibro = d.l.TituloDeLibro,
                    Portada = d.l.Portada,
                    NumeroCopia = d.p.c.NumeroCopia
                }).ToList();
        }

        /// <summary>
        /// DEVUELVE TODAS LAS VALORACIONES DE CADA CLIENTE DE UN DETERMINADO LIBRO
        /// </summary>
        /// <param name="BookId">EL ID DEL LIBRO</param>
        /// <returns>RETORNA LOS ELEMENTOS NECESARIOS PARA REALIZAR LA VALORACION DEL LIBRO</returns>
        [WebMethod]
        public List<FeedBackWS> FeedbackBook(int BookId)
        {
            ValoracionLibro val = db.ValoracionLibro.DefaultIfEmpty(null).FirstOrDefault(l => l.LibroId == BookId);
            List<FeedBackWS> fb = new List<FeedBackWS>();

            if (val != null)
            {
                fb = db.ValoracionLibro
                .Join(db.Clientes, v => v.ClienteId, c => c.Id, (v, c) => new { v, c })
                .Join(db.Libros, l => l.v.LibroId, o => o.Id, (l, o) => new { l, o }).Where(b => b.l.v.LibroId == BookId)
                .Select(b => new FeedBackWS()
                {
                    Id = b.l.v.Id,
                    Comentario = b.l.v.Comentario,
                    Puntaje = b.l.v.Puntaje,
                    Sugerencia = b.l.v.Sugerencia,
                    LibroId = b.l.v.LibroId,
                    ClienteId = b.l.v.ClienteId,
                    Resumen = b.o.Descripcion,
                    Titulo = b.o.TituloDeLibro,
                    NombresCliente = b.l.c.NombresDelCliente + " " + b.l.c.ApellidosDelCliente,
                    Portada = b.o.Portada,
                    FechaValoracion = b.l.v.FechaValoracion,
                    CustomerPhoto = b.l.c.Foto
                }).ToList();
            }
            else
            {
                //BUSCAR EL LIBRO
                fb = db.Libros.Where(w => w.Id == BookId)
               .Select(b => new FeedBackWS()
               {
                   LibroId = b.Id,
                   Resumen = b.Descripcion,
                   Titulo = b.TituloDeLibro,
                   Portada = b.Portada
               }).ToList();
            }

            return fb;
        }

        /// <summary>
        /// ALMACENA UNA VALORACION DEL CLIENTE PARA DETERMINADO LIBRO
        /// </summary>
        /// <param name="Score">PUNTAJE</param>
        /// <param name="Comment">COMENTARIO</param>
        /// <param name="Tip">SUGERENCIA</param>
        /// <param name="CustomerId">CLIENTE_ID</param>
        /// <param name="BookId">LIBRO_ID</param>
        /// <returns>RETORNA UN BOOL DE CAMBIOS EXITOSOS O ERROR</returns>
        [WebMethod]
        public bool FeedbackCustomer(float Score, string Comment, string Tip, int CustomerId, int BookId, DateTime FechaValoracion)
        {
            //VALIDAR QUE EL CLIENTE YA HAYA AGREGADO UN COMENTARIO
            ValoracionLibro fb = db.ValoracionLibro.DefaultIfEmpty(null).FirstOrDefault(v => v.ClienteId == CustomerId && v.LibroId == BookId);

            //EL CLIENTE NO A DEJADO UNA VALORACION AL LIBRO
            if (fb != null)
            {
                fb.Puntaje = Score;
                fb.Comentario = Comment;
                fb.Sugerencia = Tip;
                fb.FechaValoracion = FechaValoracion;

                db.Entry(fb).State = EntityState.Modified;
                int band = db.SaveChanges();

                if (band > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                ValoracionLibro valoracion = new ValoracionLibro();

                //ALMACENA UNA VALORACION
                valoracion.Puntaje = Score;
                valoracion.Comentario = Comment;
                valoracion.Sugerencia = Tip;
                valoracion.ClienteId = CustomerId;
                valoracion.LibroId = BookId;
                valoracion.FechaValoracion = FechaValoracion;

                db.ValoracionLibro.Add(valoracion);
                int band = db.SaveChanges();

                if (band > 0)
                    return true;
                else
                    return false;
            }
        }

        /***************************************************************************************************************************
         *                                     INICIO DE SERVICIOS DE PARTE DEL ADMIN                                              *
         ***************************************************************************************************************************/

        /// <summary>
        /// AGREGA UN NUEVO LIBRO
        /// </summary>
        /// <param name="Code">CODIGO</param>
        /// <param name="Title">TITULO</param>
        /// <param name="ISBN">ISBN</param>
        /// <param name="Autor">AUTOR</param>
        /// <param name="Portada">PORTADA</param>
        /// <param name="Adquisicion">ADQUISICION</param>
        /// <param name="Description">DESCRIPCION</param>
        /// <param name="Category">MATERIA ID</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS AddBook(string Code, string Title, string ISBN, string Autor, byte[] Portada, DateTime Adquisicion, string Description, int Category)
        {
            MessageWS ms = new MessageWS();
            Libro librito = db.Libros.DefaultIfEmpty(null).FirstOrDefault(x => x.ISBN == ISBN);

            if (librito == null)
            {
                Libro libro = new Libro();

                libro.CodigoDeLibro = Code;
                libro.TituloDeLibro = Title;
                libro.ISBN = ISBN;
                libro.Autor = Autor;
                libro.Portada = Portada;
                libro.Adquisicion = Adquisicion;
                libro.Descripcion = Description;
                libro.MateriaId = Category;

                db.Libros.Add(libro);
                int save = db.SaveChanges();

                if (save > 0)
                {
                    ms.band = true;
                    ms.message = "Almacenado";
                }
                else
                {
                    ms.band = false;
                    ms.message = "Error";
                }
            }
            else
            {
                ms.band = false;
                ms.message = "Ya existe un libro con el número ISBN especificado";
            }

            return ms;
        }

        /// <summary>
        /// ALMACENA UNA COPIA DE LIBRO
        /// </summary>
        /// <param name="NoCopy">NUMERO DE COPIA</param>
        /// <param name="NoBook">ID DEL LIBRO</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS AddCopy(int NoCopy, int NoBook)
        {
            MessageWS ms = new MessageWS();
            CopiaDeLibro copy = db.CopiasDelibro.DefaultIfEmpty(null).FirstOrDefault(x => x.LibroId == NoBook && x.NumeroCopia == NoCopy);

            if (copy != null)
            {
                ms.band = false;
                ms.message = "Ya existe un libro con ese número de copia";
            }
            else
            {
                CopiaDeLibro cl = new CopiaDeLibro();

                cl.NumeroCopia = NoCopy;
                cl.LibroId = NoBook;
                db.CopiasDelibro.Add(cl);
                int save = db.SaveChanges();

                if (save > 0)
                {
                    ms.band = true;
                    ms.message = "Almacenado";
                }
                else {
                    ms.band = false;
                    ms.message = "Error";
                }
            }

            return ms;
        }

        /// <summary>
        /// AGREGA UN CLIENTE
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="LastName"></param>
        /// <param name="Id">CEDULA</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS AddCustomer(string Name, string LastName, string Id)
        {
            MessageWS ms = new MessageWS();

            Cliente cliente = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.CodigoDeCliente.Trim() == Id.Trim());
            if (cliente != null)
            {
                ms.band = false;
                ms.message = "Ya existe este cliente";
            }
            else
            {
                Cliente persona = new Cliente();
                persona.CodigoDeCliente = Id;
                persona.NombresDelCliente = Name;
                persona.ApellidosDelCliente = LastName;
                db.Clientes.Add(persona);
                int save = db.SaveChanges();
                if (save > 0)
                {
                    ms.band = true;
                    ms.message = "Almacenado";
                }
                else
                {
                    ms.band = false;
                    ms.message = "Error";
                }
            }

            return ms;
        }

        /// <summary>
        /// LISTA TODOS LOS CLIENTES REGISTRADOS EN EL SISTEMA
        /// </summary>
        /// <returns>RETORNA LISTA DE TODOS LOS CLIENTES</returns>
        [WebMethod]
        public List<CustomerWS> CustomerList()
        {
            return db.Clientes.Select(c => new CustomerWS()
            {
                Id = c.Id,
                Codigo = c.CodigoDeCliente,
                Nombres = c.NombresDelCliente,
                Apellidos = c.ApellidosDelCliente,
                Email = c.Email,
                Foto = c.Foto
            }).ToList();
        }

        /// <summary>
        /// AGREGA UN ALQUILER POR CLIENTE
        /// </summary>
        /// <param name="Code">CODIGO DE ALQUILER</param>
        /// <param name="DateNow">FECHA DE ALQUILER</param>
        /// <param name="ReturnDate">FECHA DE DEVOLUCION</param>
        /// <param name="RealReturnDate">FECHA REAL DEVOLUCION</param>
        /// <param name="BooksDetails">TODOS LOS LIBROS ALQUILADOS</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS AddRental(int customerId, string Code, DateTime DateNow, DateTime ReturnDate, DateTime RealReturnDate, List<RentalDetailsWS> BooksDetails)
        {
            MessageWS ms = new MessageWS();
            AlquilerDeLibro alquiler = new AlquilerDeLibro();

            //VALIDAR SI EXISTE YA EL CODIGO
            AlquilerDeLibro alq = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(a => a.CodigoAlquiler.Trim() == Code.Trim());

            //SI EL CODIGO NO EXISTE AGREGAR
            if (alq == null)
            {
                //AGREGAR EL ALQUILER
                alquiler.CodigoAlquiler = Code;
                alquiler.FechaAlquiler = DateNow;
                alquiler.FechaDevo = ReturnDate;
                alquiler.FechaRealDevolucion = RealReturnDate;
                alquiler.ClienteId = customerId;
                db.AlquileresDeLibro.Add(alquiler);
                int saveRental = db.SaveChanges();

                //SI EL ALQUILER SE ALMACENO CORRECTAMENTE
                if (saveRental > 0)
                {
                    //AGREGAR EL DETALLE
                    foreach (var item in BooksDetails)
                    {
                        DetalleAlquiler detalle = new DetalleAlquiler();
                        //RECORRER LOS ITEM DEL OBJETO
                        detalle.AlquilerId = alquiler.Id;
                        detalle.CopiaId = item.CopiaId;
                        
                        //ALMACENAR EL DETALLLE
                        db.DetallesAlquiler.Add(detalle);
                    }                    
                    //GUARDAR CAMBIOS
                    int saveDetails = db.SaveChanges();

                    //SI SE ALMACENO CORRECTAMENTE EL DETALLE
                    if (saveDetails > 0)
                    {
                        ms.band = true;
                        ms.message = "Almacenado";
                    }
                    else
                    {
                        //SI EL DETALLLE NO SE ALMACENO
                        ms.band = false;
                        ms.message = "Error Detalle";
                    }
                }
                else
                {
                    //SI EL ALQUILER NO SE ALMACENO
                    ms.band = false;
                    ms.message = "Error Alquiler";
                }
            }
            else
            {
                ms.band = false;
                ms.message = "El código ya existe";
            }

            return ms;
        }

        /// <summary>
        /// DEVULVE UN PRESTAMO
        /// </summary>
        /// <param name="RentalCode">CODIGO DE ALQUILER</param>
        /// <param name="CustomerId">CEDULA DEL CLIENTE</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS RentalReturns(string RentalCode)
        {
            MessageWS ms = new MessageWS();

            //BUSCAR EL ID DEL CLIENTE POR EL CODIGO
            //Cliente cliente = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(a => a.CodigoDeCliente == CustomerId);
            //BUSCAR EL ALQUILER A DEVOLVER
            AlquilerDeLibro alquiler = db.AlquileresDeLibro.DefaultIfEmpty(null).FirstOrDefault(a => a.CodigoAlquiler == RentalCode/* || a.ClienteId == cliente.Id*/);

            if (alquiler != null)
            {
                //SE TOMA LA FECHA DEL SERVIDOR
                alquiler.FechaRealDevolucion = DateTime.Now;

                db.Entry(alquiler).State = EntityState.Modified;
                int save = db.SaveChanges();

                if (save > 0)
                {
                    ms.band = true;
                    ms.message = "Almacenado";
                }
                else
                {
                    ms.band = false;
                    ms.message = "Error";
                }
            }
            else
            {
                ms.band = false;
                ms.message = "No se ha encontrado una renta con ese código";
            }

            return ms;
        }

        /// <summary>
        /// BUSCA UN LIBRO EN ESPECIFICO
        /// </summary>
        /// <param name="ISBN">CODIGO ISBN</param>
        /// <returns>RETORNA UN OBJETO LIBRO</returns>
        [WebMethod]
        public BookWS SearchBook(string ISBN)
        {
            return db.Libros.Where(l => l.ISBN == ISBN).Select(l => new BookWS() {
                Id = l.Id,
                ISBN = l.ISBN,
                Adquisicion = l.Adquisicion,
                Autor = l.Autor,
                Codigo = l.CodigoDeLibro,
                Descripcion = l.Descripcion,
                MateriaId = l.MateriaId,
                Portada = l.Portada,
                Titulo =l.TituloDeLibro
            }).FirstOrDefault();
        }

        /// <summary>
        /// LISTA TODOS LOS LIBROS DISPONIBLES EN LA BIBLIOTECA
        /// </summary>
        /// <returns>LISTA DE LIBROS DISPONIBLES</returns>
        [WebMethod]
        public List<BookListWS> AvailableBooks()
        {
            List<BookListWS> book = new List<BookListWS>();

            book = (from c in db.CopiasDelibro
                    where !(from a in db.AlquileresDeLibro
                            join d in db.DetallesAlquiler on a.Id equals d.AlquilerId
                            where a.FechaRealDevolucion.Year == 1900
                            select d.CopiaId).Contains(c.Id)
                    select new BookListWS()
                    {
                        Id = c.LibroId,
                        Codigo = c.Libro.CodigoDeLibro,
                        Titulo = c.Libro.TituloDeLibro,
                        ISBN = c.Libro.ISBN,
                        Autor = c.Libro.Autor,
                        Portada = c.Libro.Portada,
                        Adquisicion = c.Libro.Adquisicion,
                        Descripcion = c.Libro.Descripcion,
                        NumeroCopia = c.NumeroCopia
                    }).ToList();

            return book;
        }

        /// <summary>
        /// LISTA TODOS LOS CLIENTES QUE TIENEN PRESTAMOS PENDIENTES
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<Stattics> Pendientes()
        {
            var linq = db.Database.SqlQuery<Stattics>("Pendientes");

            return linq.ToList();
        }

        /// <summary>
        /// LISTA TODOS LOS PRESTAMOS PENDIENTES DE UN CLIENTE
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Stattics> PendientesByCustomer(int cliente)
        {
            var linq = db.Database.SqlQuery<Stattics>("PendientesByCustomer @cliente", new SqlParameter("@cliente", cliente));

            return linq.ToList();
        }

        /// <summary>
        /// REPORTE DE LOS LIBROS MAS SOLICITADOS POR MES
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<LibroSolicitado> LibrosSolicitados(DateTime fecha)
        {
            return db.Database.SqlQuery<LibroSolicitado>("LibroMasSolicitado @fecha", new SqlParameter("@fecha", fecha)).ToList();
        }

        /// <summary>
        /// ENVIA EL NUMERO MAXIMO DE CODIGO DE LA RENTA
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string RentalCode()
        {
            int code = int.Parse(db.AlquileresDeLibro.Max(x => x.CodigoAlquiler));

            string num;

            if (code < 10)
                num = "00" + (code + 1);
            else
                if (code >= 10 || code < 100)
                num = "0" + (code + 1);
            else
                num = (code + 1).ToString();

            return num;
        }

        /// <summary>
        /// ENVIA EL NUMERO MAXIMO DE CODIGO DE UNA RENTA
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string BookCode()
        {
            int code = int.Parse(db.Libros.Max(x => x.CodigoDeLibro));
            string num;

            if (code < 10)
                num = "00" + (code + 1);
            else
                if (code >= 10 || code < 100)
                num = "0" + (code + 1);
            else
                num = (code + 1).ToString();

            return num;
        }

        /***************************************************************************************************************************
        *                                                PERSONALIZACION                                                          *
        ***************************************************************************************************************************/
        /// <summary>
        /// CAMBIO DE IMAGEN DE PERFIL
        /// </summary>
        /// <param name="CustomerId">CLIENTE ID</param>
        /// <param name="ImageProfile">IMAGEN A GUARDAR</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS ImageProfileChange(int CustomerId, byte[] ImageProfile)
        {
            MessageWS ms = new MessageWS();

            //BUSCAR AL CLIENTE
            Cliente cliente = db.Clientes.DefaultIfEmpty(null).FirstOrDefault(c => c.Id == CustomerId);
            if (cliente != null)
            {
                cliente.Foto = ImageProfile;

                //GUARDAR MODIFICACIONES
                db.Entry(cliente).State = EntityState.Modified;
                int save = db.SaveChanges();

                if (save > 0)
                {
                    ms.band = true;
                    ms.message = "Cambios almacenado correctamente";
                }
                else
                {
                    ms.band = true;
                    ms.message = "Error. Intentelo de nuevo";
                }
            }

            return ms;
        }

        [WebMethod]
        public List<CopysBookWS> CopiesBook(string ISBN)
        {

            var copies = (from c in db.CopiasDelibro
                        join l in db.Libros on c.LibroId equals l.Id
                         where l.ISBN == ISBN && !(from a in db.AlquileresDeLibro
                         join d in db.DetallesAlquiler on a.Id equals d.AlquilerId
                         where a.FechaRealDevolucion.Year == 1900
                         select d.CopiaId).Contains(c.Id)
                    select new CopysBookWS()
                    {
                        LibroId = c.LibroId,
                        CopyId = c.Id,
                        ISBN = l.ISBN,
                        NumeroCopia = c.NumeroCopia,
                        Portada = l.Portada,
                        Titulo = l.TituloDeLibro
                    }).ToList();

            return copies;
        }

        /// <summary>
        /// CAMBIA LA CONTRASEÑA DE LA CUENTA
        /// </summary>
        /// <param name="Email">CORREO CLIENTE</param>
        /// <param name="OldPassword">VIEJA CONTRASEÑA</param>
        /// <param name="NewPassword">NUEVA CONTRASEÑA</param>
        /// <returns></returns>
        [WebMethod]
        public MessageWS PasswordChange(string Email, string OldPassword, string NewPassword)
        {
            MessageWS ms = new MessageWS();
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //BUSCAR EN LA BD DE SEGURIDAD EL EMAIL Y OBTENER SU ID
            var user = UserManager.FindByEmail(Email);

            //CAMBIAMOS LA CONTRASEÑA
            var result = UserManager.ChangePassword(user.Id, OldPassword, NewPassword);

            if (result.Succeeded)
            {
                ms.band = true;
                ms.message = "Cambio exitoso";
            }
            else {
                ms.band = false;
                ms.message = "Error en cambio de contraseña";
            }

            return ms;
        }

        /***************************************************************************************************************************
         *                                                  SERIALIZACION                                                          *
        ***************************************************************************************************************************/
        [Serializable]
        public class WebRegisterResult
        {
            public string Codigo { get; set; }
            public string Email { get; set; }
            public string Mensaje { get; set; }
            public bool Register { get; set; }
            public string Action { get; set; }

        }

        [Serializable]
        public class WebLoginResult
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Rol { get; set; }
            public byte[] Imagen { get; set; }
            public string Mensaje { get; set; }
            public bool IsLogged { get; set; }
        }

        [Serializable]
        public partial class CategoryWS
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
        }

        [Serializable]
        public partial class CustomerWS
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public byte[] Foto { get; set; }
        }

        [Serializable]
        public partial class BookWS
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Titulo { get; set; }
            public string ISBN { get; set; }
            public string Autor { get; set; }
            public byte[] Portada { get; set; }
            public DateTime Adquisicion { get; set; }
            public string Descripcion { get; set; }
            public int MateriaId { get; set; }
        }

        public partial class BookListWS
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Titulo { get; set; }
            public string ISBN { get; set; }
            public string Autor { get; set; }
            public byte[] Portada { get; set; }
            public DateTime Adquisicion { get; set; }
            public string Descripcion { get; set; }
            public int NumeroCopia { get; set; }
        }

        [Serializable]
        public partial class CopysBookWS
        {
            public int CopyId { get; set; }
            public int NumeroCopia { get; set; }
            public int LibroId { get; set; }
            public string ISBN { get; set; }
            public string Titulo { get; set; }
            public byte[] Portada { get; set; }
        }

        [Serializable]
        public partial class RentalWS
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public DateTime FechaAlquiler { get; set; }
            public DateTime FechaDevolucion { get; set; }
            public DateTime FechaRealDevolucion { get; set; }
            public int ClienteId { get; set; }
        }

        [Serializable]
        public partial class RentalDetailsWS
        {
            public int Id { get; set; }
            public int CopiaId { get; set; }
            public int AlquilerId { get; set; }
            public string TituloLibro { get; set; }
            public byte[] Portada { get; set; }
            public int NumeroCopia { get; set; }
        }

        [Serializable]
        public partial class FeedBackWS
        {
            public int Id { get; set; }
            public float Puntaje { get; set; }
            public string Comentario { get; set; }
            public string Sugerencia { get; set; }
            public int ClienteId { get; set; }
            public int LibroId { get; set; }
            public string NombresCliente { get; set; }
            public string Titulo { get; set; }
            public byte[] Portada { get; set; }
            public byte[] CustomerPhoto { get; set; }
            public string Resumen { get; set; }
            public DateTime FechaValoracion { get; set; }
        }

        [Serializable]
        public partial class FeedBackCustomerWS
        {
            public int Id { get; set; }
            public float Puntaje { get; set; }
            public string Comentario { get; set; }
            public string Sugerencia { get; set; }
            public int ClienteId { get; set; }
            public int LibroId { get; set; }
            public DateTime FechaValoracion { get; set; }
        }

        [Serializable]
        public partial class MessageWS
        {
            public string message { get; set; }
            public bool band { get; set; }
        }

        public partial class Stattics
        {
            public int RentaId { get; set; }
            public string CodigoAlquiler { get; set; }
            public string Cedula { get; set; }
            public string Cliente { get; set; }
            public int CantidadLibros { get; set; }
            public int ClienteId { get; set; }
            public DateTime FechaAlq { get; set; }
            public DateTime FechaDevo { get; set; }
        }

        public partial class LibroSolicitado
        {
            public string Titulo { get; set; }
            public int Cantidad { get; set; }
        }
    }
}