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
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<NormasRepartoViewModel, int> normas;
        private readonly ICrudApi<AsignacionViewModel, int> asignacion;
        [BindProperty]
        public ComprasViewModel Objeto { get; set; }

        [BindProperty]
        public NormasRepartoViewModel[] Normas { get; set; }
        public EditarModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<NormasRepartoViewModel, int> normas, ICrudApi<AsignacionViewModel, int> asignacion)
        {
            this.service = service;
            this.normas = normas;
            this.asignacion = asignacion;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "23").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Normas = await normas.ObtenerLista("");
                Objeto = await service.ObtenerPorId(id);


                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnGetAsignar(string id, int idNorma)
        {
            try
            {
                var ids = Convert.ToInt32(id);
                AsignacionViewModel asig = new AsignacionViewModel();
                asig.idFac = ids;
                asig.idNorma = idNorma;
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
