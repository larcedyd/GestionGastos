﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class EncComprasReportes
    {
  
        public int id { get; set; }
        [Required]
        [StringLength(20)]
        public string CodEmpresa { get; set; }

        [Required]
        [StringLength(20)]
        public string CodProveedor { get; set; }

        public string NomProveedor { get; set; }

        [Required]
        [StringLength(10)]
        public string TipoDocumento { get; set; }


        
        public int NumFactura { get; set; }

        public DateTime? FecFactura { get; set; }

        [StringLength(10)]
        public string TipoIdentificacionCliente { get; set; }

        [StringLength(20)]
        public string CodCliente { get; set; }

        [StringLength(500)]
        public string NomCliente { get; set; }

        [StringLength(500)]
        public string EmailCliente { get; set; }

        public int? DiasCredito { get; set; }

        [StringLength(2)]
        public string CondicionVenta { get; set; }

        [StringLength(50)]
        public string ClaveHacienda { get; set; }

        [StringLength(20)]
        public string ConsecutivoHacienda { get; set; }

        [StringLength(2)]
        public string MedioPago { get; set; }

        public byte? Situacion { get; set; }

        [StringLength(3)]
        public string CodMoneda { get; set; }

        
        public decimal? TotalServGravados { get; set; }

        
        public decimal? TotalServExentos { get; set; }

        
        public decimal? TotalMercanciasGravadas { get; set; }

        
        public decimal? TotalMercanciasExentas { get; set; }

        
        public decimal? TotalExento { get; set; }

        
        public decimal? TotalVenta { get; set; }

        
        public decimal? TotalDescuentos { get; set; }

        
        public decimal? TotalVentaNeta { get; set; }

        
        public decimal? TotalImpuesto { get; set; }

        
        public decimal? TotalComprobante { get; set; }

 



        public DateTime? FechaGravado { get; set; }

        
        public decimal? TotalServExonerado { get; set; }

        
        public decimal? TotalMercExonerada { get; set; }

        
        public decimal? TotalExonerado { get; set; }

        
        public decimal? TotalIVADevuelto { get; set; }

        
        public decimal? TotalOtrosCargos { get; set; }

        [StringLength(6)]
        public string CodigoActividadEconomica { get; set; }

        public int? idLoginAsignado { get; set; }

        public DateTime? FecAsignado { get; set; }



        

        public int? idNormaReparto { get; set; }

        public int? idTipoGasto { get; set; }

        public string TipoGasto { get; set; }

        public int? idCierre { get; set; }
       
    }
}
