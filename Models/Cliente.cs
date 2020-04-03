using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerDeLibros.Models
{
    [Table("Clientes", Schema = "prest")]
    public partial class Cliente
    {
        //Constructor de la clase
        public Cliente()
        {
            this.AlquileresDeLibro = new HashSet<AlquilerDeLibro>();
        }

        //Agregando los atributos de la clase y sus notaciones
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "La longitud del campo debe ser de 14 dígitos")]
        [Display(Name = "Cédula")]

        public string CodigoDeCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "La longitud del campo debe ser menor o igual a 50 caracteres")]
        [Display(Name = "Nombres")]
        public string NombresDelCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "La longitud del campo debe ser menor o igual a 50 caracteres")]
        [Display(Name = "Apellidos")]
        public string ApellidosDelCliente { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "El campo {0} es incorrecto")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Foto de perfil")]
        public byte[] Foto { get; set; }

        [NotMapped]
        public string NombreCompleto { get { return NombresDelCliente.Trim() + " " + ApellidosDelCliente.Trim(); } }

        //Hijos
        public virtual ICollection<AlquilerDeLibro> AlquileresDeLibro { get; set; }
    }
}