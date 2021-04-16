using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class TiposPlanes
    {
        [Key]
        public int idTipoPlan { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaInteres { get; set; }
        public int TiempoEstancia { get; set; }
    }
}
