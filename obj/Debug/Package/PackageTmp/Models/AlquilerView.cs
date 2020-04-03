using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlquilerDeLibros.Models
{
    public class AlquilerView
    {
        public AlquilerDeLibro Cliente { get; set; }
        //SOLAMENTE PARA MOSTRAR LOS ENCABEZADOS EN EL INDEX DE PRESTAMOS
        public DetalleAlquiler Titles { get; set; }
        public List<CopiaAlquiler> CopiasLibro { get; set; }
    }
}