using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("ValoracionesLibro", Schema = "prest")]
    public class ValoracionLibro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese un {0} para este libro")]
        [Range(1, 5, ErrorMessage = "{0} excedido del límite")]
        [Display(Name = "Puntaje")]
        public float Puntaje { get; set; }

        [Required(ErrorMessage = "Ingrese un {0}")]
        [StringLength(100)]
        [Display(Name = "Comentario")]
        public string Comentario { get; set; }

        [StringLength(60)]
        [Display(Name = "Sugerencia")]
        public string Sugerencia { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime FechaValoracion { get; set; }

        public int ClienteId { get; set; }
        public int LibroId { get; set; }

        //PADRES
        public virtual Cliente Cliente { get; set; }
        public virtual Libro Libro { get; set; }
    }
}