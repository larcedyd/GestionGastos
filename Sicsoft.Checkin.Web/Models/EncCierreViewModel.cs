using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class EncCierreViewModel
    {
 
        public int idCierre { get; set; }

        public string Periodo { get; set; }

        public DateTime FechaInicial { get; set; }

        public DateTime FechaFinal { get; set; }

        public int idLogin { get; set; }

        
        public decimal? SubTotal { get; set; }

        
        public decimal? Descuento { get; set; }

        
        public decimal? Impuestos { get; set; }


 
        public decimal? Total { get; set; }

        public string Estado { get; set; }
        public string Observacion { get; set; }
        public decimal Impuesto1 { get; set; }
        public decimal Impuesto2 { get; set; }
        public decimal Impuesto4 { get; set; }
        public decimal Impuesto8 { get; set; }
        public decimal Impuesto13 { get; set; }
        public int? CantidadRegistros { get; set; }

        public DateTime? FechaCierre { get; set; }
        public string CodMoneda { get; set; }
    }
}
