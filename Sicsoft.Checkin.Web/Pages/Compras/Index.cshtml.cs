using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace InversionGloblalWeb.Pages.Compras
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<AsignacionViewModel, int> asignacion;

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public ComprasViewModel[] Objeto { get; set; }

        public IndexModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<AsignacionViewModel, int> asignacion)
        {
            this.service = service;
            this.asignacion = asignacion;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if(string.IsNullOrEmpty(filtro.Texto))
                {

                await service.RealizarLecturaEmails();
                await service.LecturaBandejaEntrada();
                }
                Objeto = await service.ObtenerLista(filtro);


                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnGetAsignar(string id)
        {
            try
            {
                var ids = Convert.ToInt32(id);
                AsignacionViewModel asig = new AsignacionViewModel();
                asig.idFac = ids;
                asig.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                await asignacion.Editar(asig);

                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                return new JsonResult(false);
            }
        }
    }
}
