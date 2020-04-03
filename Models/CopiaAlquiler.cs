using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlquilerDeLibros.Models
{
    public class CopiaAlquiler : CopiaDeLibro
    {
        [StringLength(50, ErrorMessage = "La longitud máxima del campo debe ser menor o igual a 50 carácteres")]
        [Display(Name = "Título")]
        public string TituloDeLibro { get; set; }

    }
}