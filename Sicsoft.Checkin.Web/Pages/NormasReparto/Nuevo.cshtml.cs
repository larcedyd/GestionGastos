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
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace InversionGloblalWeb.Pages.NormasReparto
{
    public class NuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<NormasRepartoViewModel, int> service;
        private readonly ICrudApi<LoginUsuarioViewModel, int> usuario;
        private readonly ICrudApi<DimensionesViewModel, int> dimensiones;
        [BindProperty]
        public NormasRepartoViewModel Objeto { get; set; }

        [BindProperty]
        public LoginUsuarioViewModel[] Usuarios { get; set; }
        [BindProperty]
        public DimensionesViewModel[] Dimensiones { get; set; }

        public NuevoModel(ICrudApi<NormasRepartoViewModel, int> service, ICrudApi<LoginUsuarioViewModel, int> usuario, ICrudApi<DimensionesViewModel, int> dimensiones)
        {
            this.service = service;
            this.usuario = usuario;
            this.dimensiones = dimensiones;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "12").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                int ids = Convert.ToInt32(id);
                Dimensiones = await dimensiones.ObtenerLista("");
                Usuarios = await usuario.ObtenerLista("");
                return Page();
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
               // Objeto.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                await service.Agregar(Objeto);
                return RedirectToPage("./Index");
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
