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
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<RolesViewModel, int> service;

        [BindProperty]
        public RolesViewModel Rol { get; set; }

        public EditarModel(ICrudApi<RolesViewModel, int> service)
        {
            this.service = service;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {

                Rol = await service.ObtenerPorId(id);
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
                await service.Editar(Rol);
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
