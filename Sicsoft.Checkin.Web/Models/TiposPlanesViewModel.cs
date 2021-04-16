using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class TiposPlanesViewModel
    {
        public int idTipoPlan { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaInteres { get; set; }
        public int TiempoEstancia { get; set; }
        public bool Estado { get; set; }
    }
}
