using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sicsoft.CostaRica.Checkin.Web.Models
{
    public class FiltroEjecutivo
    {
        public string Texto { get; set; }
        public int CodInversionista { get; set; }
        public int CodEjecutivo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public int TipoPlan { get; set; }
        public int TipoMovimiento { get; set; }
        public string Certificado { get; set; }
    }
}
