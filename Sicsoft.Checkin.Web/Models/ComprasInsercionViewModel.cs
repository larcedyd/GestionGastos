using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class ComprasInsercionViewModel
    {
        public EncComprasViewModel EncCompras { get; set; }
        public DetComprasViewModel[] DetCompras { get; set; }
    }
}
