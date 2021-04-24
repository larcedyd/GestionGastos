using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class CuentasContablesViewModel
    {
        [Key]
        public int idCuentaContable { get; set; }


        [StringLength(50)]
        public string CodSAP { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }
    }
}
