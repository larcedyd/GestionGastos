using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sicsoft.Checkin.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICrudApi<HeaderReportViewModel, int> service;
        private readonly ICrudApi<EncComprasReportes, int> compras;
        private readonly ICrudApi<UsuariosViewModel, int> users;

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }
        [BindProperty]
        public HeaderReportViewModel MesActual { get; set; }

        [BindProperty]
        public HeaderReportViewModel MesAnterior { get; set; }

        [BindProperty]
        public HeaderReportViewModel Año { get; set; }

        [BindProperty]
        public EncComprasReportes[] Compra { get; set; }

        [BindProperty]
        public List<IGrouping<string,EncComprasReportes>> Compras { get; set; }

        [BindProperty]
        public EncComprasReportes[] CompraAnual { get; set; }

        [BindProperty]
        public List<IGrouping<string, EncComprasReportes>> ComprasAnuales { get; set; }


        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ICrudApi<HeaderReportViewModel, int> service, ICrudApi<EncComprasReportes, int> compras, ICrudApi<UsuariosViewModel, int> users)
        {
            _logger = logger;
            this.service = service;
            this.compras = compras;
            this.users = users;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Usuarios = await users.ObtenerLista("");
               
                if(filtro.Codigo1 == 0)
                {
                    filtro.Codigo1 = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                    
                }
                if(filtro.CodMoneda == null)
                {
                    filtro.CodMoneda = "CRC";
                }
                DateTime time = DateTime.Now;
           

                filtro.FechaInicio = DateTime.Now;

                filtro.FechaInicio = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 1);
                

                DateTime primerDia = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 1);


                DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                filtro.FechaFinal = ultimoDia;



                Compra = await compras.ObtenerLista(filtro);
                Compras = Compra.GroupBy(a => a.TipoGasto).ToList();
                MesActual = await service.ObtenerHeader(filtro);

                filtro.FechaInicio = filtro.FechaInicio.AddMonths(-1);
                filtro.FechaFinal = filtro.FechaFinal.AddMonths(-1);

                MesAnterior = await service.ObtenerHeader(filtro);


                filtro.FechaInicio = new DateTime(DateTime.Now.Year, 1, 1);
                filtro.FechaFinal = new DateTime(DateTime.Now.Year, 12, 31);

                CompraAnual = await compras.ObtenerLista(filtro);
                ComprasAnuales = CompraAnual.GroupBy(a => a.TipoGasto).ToList();
                Año = await service.ObtenerHeader(filtro);

                return Page();


            }
            catch (ApiException ex)
            {

                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
