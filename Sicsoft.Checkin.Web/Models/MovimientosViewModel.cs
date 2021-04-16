using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class MovimientosViewModel
    {
        [Key]

        public string IdRendimiento { get; set; }

        public int TipoMovimiento { get; set; }

        public string IdInversion { get; set; }

        public int idInversionista { get; set; }

        public int idUsuario { get; set; }

        public DateTime FechaMovimiento { get; set; }

        [Column(TypeName = "numeric")]
        public decimal TasaInteres { get; set; }

        [Column(TypeName = "money")]
        public decimal Principal { get; set; }

        [Column(TypeName = "money")]
        public decimal Monto { get; set; }
    }
}
