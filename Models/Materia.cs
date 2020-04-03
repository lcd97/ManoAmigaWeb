using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("Materias", Schema = "prest")]
    public partial class Materia
    {
        //Constructor de la clase
        public Materia()
        {
            this.Libros = new HashSet<Libro>();
        }

        //Agregando los atributos de la clase y sus notaciones
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La longitud del campo debe ser de 3 dígitos")]
        [Display(Name = "Código")]
        public string CodigoDeMateria { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "La longitud máxima del campo debe ser menor o igual a 50 carácteres")]
        [Display(Name = "Categoría")]
        public string DescripcionDeMateria { get; set; }

        //Hijos
        public virtual ICollection<Libro> Libros { get; set; }
    }
}