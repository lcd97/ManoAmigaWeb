using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("DetallesAlquiler", Schema = "prest")]
    public class DetalleAlquiler
    {
        [Key]
        public int Id { get; set; }

        //FOREIGN KEY
        public int AlquilerId { get; set; }
        public int CopiaId { get; set; }

        //PADRES
        public virtual AlquilerDeLibro Alquiler { get; set; }
        public virtual CopiaDeLibro Copia { get; set; }
    }
}