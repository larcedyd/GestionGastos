using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class EncComprasTemp
    {
        public string ClaveHacienda { get; set; }
        public string ConsecutivoHacienda { get; set; }
        public string TipoIdentificacionCliente { get; set; }
        public int NumFactura { get; set; }
        public string CodProveedor { get; set; }
        public string NomProveedor { get; set; }
        public DateTime FecFactura { get; set; }
        public string CodActividadEconomica { get; set; }
        public string CodMoneda { get; set; }
        public string CodCliente { get; set; }
        public string NomCliente { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal Impuesto1 { get; set; }
        public decimal Impuesto2 { get; set; }
        public decimal Impuesto4 { get; set; }
        public decimal Impuesto8 { get; set; }
        public decimal Impuesto13 { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal TotalComprobante { get; set; }
        public string ImagenBase64 { get; set; }
        public bool RegimenSimplificado { get; set; }
        public bool FacturaExterior { get; set; }
        public bool GastosVarios { get; set; }
    }
}
