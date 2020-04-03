using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("AlquileresDeLibro", Schema = "prest")]
    public partial class AlquilerDeLibro
    {
        //CONSTRUCTOR DE LA CLASE
        public AlquilerDeLibro()
        {
            this.DetallesAlquiler = new HashSet<DetalleAlquiler>();
        }

        //Agregando los atributos de la clase y sus notaciones
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La longitud debe ser de 3 dígitos")]
        [Display(Name = "Código alquiler")]
        public string CodigoAlquiler { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de alquiler")]
        public DateTime FechaAlquiler { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha devolución")]
        public DateTime FechaDevo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha real devolucion")]
        public DateTime FechaRealDevolucion { get; set; }

        //Foreign key
        public int ClienteId { get; set; }

        //Padres
        public virtual Cliente Cliente { get; set; }

        //HIJOS
        public ICollection<DetalleAlquiler> DetallesAlquiler { get; set; }
    }
}