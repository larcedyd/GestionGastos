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

namespace InversionGloblalWeb.Pages.Liquidaciones
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ListadoCierresViewModel, int> service;
        private readonly ICrudApi<UsuariosViewModel, int> users;


        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public ListadoCierresViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        public IndexModel(ICrudApi<ListadoCierresViewModel, int> service, ICrudApi<UsuariosViewModel, int> users)
        {
            this.service = service;
            this.users = users;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                Usuarios = await users.ObtenerLista("");
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");

                if(string.IsNullOrEmpty(Roles.Where(a => a == "8").FirstOrDefault()))
                {
                  filtro.Codigo1 = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                     
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

        public async Task<IActionResult> OnGetEstado(int idB)
        {
            try
            {
                await service.CambiaEstado(idB, "P", "");

                return new JsonResult(true);
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message);
            }
        }
    }
}
