using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class DetCierreInsercionViewModel
    {
        public int idFactura { get; set; }
        public int idTipoGasto { get; set; }
        public string Comentario { get; set; }
    }
}
