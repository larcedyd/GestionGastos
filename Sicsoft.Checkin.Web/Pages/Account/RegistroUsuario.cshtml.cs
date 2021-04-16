using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Sicsoft.CostaRica.Checkin.Web.Pages.Account
{
    public class RegistroUsuarioModel : PageModel
    {
        private readonly ICrudApi<LoginUsuario, int> checkInService;
        private readonly ICrudApi<RolesViewModel, int> rolesService;

        [BindProperty]
        public LoginUsuario Input { get; set; }

        [BindProperty]
        public RolesViewModel[] Roles { get; set; }

        public RegistroUsuarioModel(ICrudApi<LoginUsuario, int> checkInService, ICrudApi<RolesViewModel, int> rolesService)
        {
            this.checkInService = checkInService;
            this.rolesService = rolesService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await rolesService.ObtenerLista("");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await checkInService.Agregar(Input);
                return Redirect("../Index");
            }
            catch(ApiException ex)
            {
               return Redirect("/Error");
             
            }
        }
    }
}
