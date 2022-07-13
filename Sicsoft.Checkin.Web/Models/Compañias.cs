using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class Compañias
    {
        public string CedulaJuridica { get; set; }
        public string NombreEmpresa { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Activo { get; set; }
        public string CadenaConexionBD { get; set; }
        public string Pais { get; set; }
    }
}
