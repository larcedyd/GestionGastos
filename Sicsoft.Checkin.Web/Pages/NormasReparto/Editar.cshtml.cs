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

namespace InversionGloblalWeb.Pages.NormasReparto
{
    public class EditarModel : PageModel
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

        public EditarModel(ICrudApi<NormasRepartoViewModel, int> service, ICrudApi<LoginUsuarioViewModel, int> usuario,  ICrudApi<DimensionesViewModel, int> dimensiones)
        {
            this.service = service;
            this.usuario = usuario;
            this.dimensiones = dimensiones;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                Usuarios = await usuario.ObtenerLista("");
                int ids = Convert.ToInt32(id);
                Dimensiones = await dimensiones.ObtenerLista("");
                Objeto = await service.ObtenerPorId(ids);

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
                Objeto.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                await service.Editar(Objeto);
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
