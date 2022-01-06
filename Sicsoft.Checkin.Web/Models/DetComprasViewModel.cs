using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class DetComprasViewModel
    {
        [Key]
        public int id { get; set; }
        [StringLength(20)]
        public string CodEmpresa { get; set; }

        [StringLength(20)]
        public string CodProveedor { get; set; }

        [StringLength(2)]
        public string TipoDocumento { get; set; }

        [StringLength(500)]
        public string NomProveedor { get; set; }


        public int NumFactura { get; set; }

        public int? NumLinea { get; set; }

        [StringLength(50)]
        public string CodPro { get; set; }

        [StringLength(50)]
        public string UnidadMedida { get; set; }

        [StringLength(500)]
        public string NomPro { get; set; }

        
        public decimal? PrecioUnitario { get; set; }

        [StringLength(20)]
        public string CodCliente { get; set; }

        [StringLength(500)]
        public string NomCliente { get; set; }

        public decimal? Cantidad { get; set; }

        
        public decimal? MontoTotal { get; set; }

        
        public decimal? MontoDescuento { get; set; }

        
        public decimal? SubTotal { get; set; }

        public decimal? ImpuestoTarifa { get; set; }

        
        public decimal? ImpuestoMonto { get; set; }

        
        public decimal? MontoTotalLinea { get; set; }

        [StringLength(1)]
        public string Estado { get; set; }

        [StringLength(500)]
        public string Observacion { get; set; }

        public int? idLoginAsignado { get; set; }

        public DateTime? FecAsignado { get; set; }

        public int? idLoginAceptacion { get; set; }

        public int? idNormaReparto { get; set; }

        public int? idTipoGasto { get; set; }

        public int? idCierre { get; set; }
    }
}
