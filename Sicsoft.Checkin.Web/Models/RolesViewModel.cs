using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class RolesViewModel
    {
        [Key]
        public int idRol { get; set; }
        public string NomRol { get; set; }
    }
}
