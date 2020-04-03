using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlquilerDeLibros.Models.Extension
{
    public class cLstAlquiler
    {
        public string Codigo { get; set; }
        public DateTime Fecha_Alquiler { get; set; }
        public DateTime Fecha_Devolucion { get; set; }
        public string Titulo { get; set; }
        public int Numero_Copia { get; set; }
        
    }
}