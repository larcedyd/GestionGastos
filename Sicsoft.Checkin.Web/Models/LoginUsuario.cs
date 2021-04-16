using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sicsoft.CostaRica.Checkin.Web.Models
{
    public class LoginUsuario
    {
        [Key]
        public int idUsuario { get; set; }
        public int Rol { get; set; }
        [Required(ErrorMessage = "Este campo es obligorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es obligorio")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Este campo es obligorio")]
        [StringLength(100)]
        public string Password { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime UltIngreso { get; set; }
    }
}
