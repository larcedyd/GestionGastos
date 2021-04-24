using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class NormasRepartoViewModel
    {
        public int id { get; set; }

        public int idLogin { get; set; }

        [StringLength(4)]
        public string CodSAP { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }
    }
}
