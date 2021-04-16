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

namespace InversionGloblalWeb.Pages.Account
{
    public class CambioContrasenaModel : PageModel
    {
        private readonly ICrudApi<LoginCambioClave, int> checkInService;

        [BindProperty]
        public LoginCambioClave Input { get; set; }

        public CambioContrasenaModel(ICrudApi<LoginCambioClave, int> checkInService)
        {
            this.checkInService = checkInService;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.Email = User.Identity.Name;
                await checkInService.CambiarClave(Input);
                return Redirect("../Index");
            }
            catch (ApiException ex)
            {
                
                return Redirect("/Error");

            }
        }
    }
}
