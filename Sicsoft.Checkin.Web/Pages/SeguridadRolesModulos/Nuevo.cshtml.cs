using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace InversionGloblalWeb.Pages.SeguridadRolesModulos
{
    public class NuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<RolesViewModel, int> service;

        [BindProperty]
        public RolesViewModel Rol { get; set; }

        public NuevoModel(ICrudApi<RolesViewModel, int> service)
        {
            this.service = service;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await service.Agregar(Rol);
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
