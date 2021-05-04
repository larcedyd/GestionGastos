using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class RecibidoC
    {
        public EncCierre EncCompras { get; set; }
        public DetCierreInsercionViewModel[] DetCompras { get; set; }
    }
}
