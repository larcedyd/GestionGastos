using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class DevolucionSAP
    {
        public int DocEntry { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
    }
}
