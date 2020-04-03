using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("Libros", Schema = "prest")]
    public partial class Libro
    {
        //Constructor de la clase
        public Libro()
        {
            this.CopiasDeLibro = new HashSet<CopiaDeLibro>();
        }

        //Agregando los atributos de la clase y sus notaciones
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La longitud del campo debe ser de 3 dígitos")]
        [Display(Name = "Código")]
        public string CodigoDeLibro { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "La longitud máxima del campo debe ser menor o igual a 50 carácteres")]
        [Display(Name = "Título")]
        public string TituloDeLibro { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "La longitud del campo debe ser de 13 dígitos")]
        [Display(Name = "ISBN")]

        public string ISBN { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100, ErrorMessage = "La longitud del campo debe ser menor a 100 caracteres")]
        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Portada")]
        public byte[] Portada { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Fecha de Adquisición")]
        public DateTime Adquisicion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Resumen")]
        public string Descripcion { get; set; }

        //Foreign Key
        public int MateriaId { get; set; }

        //Hijos
        public virtual ICollection<CopiaDeLibro> CopiasDeLibro { get; set; }

        //Padres
        public virtual Materia Materia { get; set; }
    }
}