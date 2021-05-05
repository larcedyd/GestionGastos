using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class DimensionesViewModel
    {
        public int id { get; set; }
        [StringLength(50)]
        public string codigoSAP { get; set; }
        [StringLength(100)]
        public string Nombre { get; set; }
    }
}
