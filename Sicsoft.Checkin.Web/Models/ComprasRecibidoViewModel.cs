using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class ComprasRecibidoViewModel
    {
        public EncComprasTemp EncCompras { get; set; }
        public DetComprasViewModel[] DetCompras { get; set; }
    }
}
