using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class InversionesViewModel
    {
        [Key]
        public string IdInversion { get; set; }

        public int idInversionista { get; set; }

        public string Inversionista { get; set; }

        public int idUsuario { get; set; }



        public DateTime FechaInversion { get; set; }

        [Required]

        public int TipoPlan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal TasaInteres { get; set; }

        [Column(TypeName = "money")]
        public decimal Monto { get; set; }

        [Column(TypeName = "money")]
        public decimal MontoGenerado { get; set; }

        public int Plazo { get; set; }

        public int Status { get; set; }

        [Required]
        [StringLength(50)]
        public string NumReferenciaCrypto { get; set; }
    }
}
