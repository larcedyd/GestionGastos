using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sicsoft.CostaRica.Checkin.Web.Models
{
    public class EjecutivosViewModel
    {
        [Key]
        public int idEjecutivo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = " Este campo Telefono es obligatorio")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "  Este campo Pais es obligatorio")]
        [StringLength(50)]
        public string Pais { get; set; }

        [Required(ErrorMessage = "  Este campo Email es obligatorio")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "  Este campo Password es obligatorio")]
        [StringLength(100)]
        public string Password { get; set; }

        [Required(ErrorMessage = " Este campo Wallet es obligatorio ")]
        
        [StringLength(100)]
        public string Wallet { get; set; }

        public int Estado { get; set; }
    }
}
