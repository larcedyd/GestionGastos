using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace InversionGloblalWeb.Pages.Usuarios
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<UsuariosViewModel, int> service;
        private readonly ICrudApi<RolesViewModel, int> roles;

        [BindProperty]
        public UsuariosViewModel Input { get; set; }

        [BindProperty]
        public RolesViewModel[] Roles { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }
        public EditarModel(ICrudApi<UsuariosViewModel, int> service, ICrudApi<RolesViewModel, int> roles)
        {
            this.service = service;
            this.roles = roles;

        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Roles = await roles.ObtenerLista("");
                Usuarios = await service.ObtenerLista("");
                Input = await service.ObtenerPorId(id);
                var Rol = Roles.Where(a => a.NombreRol.ToUpper().Contains("Aprobador".ToUpper())).FirstOrDefault();
                var RolA = Roles.Where(a => a.NombreRol.ToUpper().Contains("Administrador".ToUpper())).FirstOrDefault();
                var RolCA = Roles.Where(a => a.NombreRol.ToUpper().Contains("Contador Aprobador".ToUpper())).FirstOrDefault();
                Usuarios = Usuarios.Where(a => a.idRol == Rol.idRol || a.idRol == RolA.idRol || a.idRol == RolCA.idRol).ToArray();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var ced = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.NameIdentifier).Select(s1 => s1.Value).FirstOrDefault();
                Input.CedulaJuridica = ced;
               if(Input.idLoginAceptacion == 0)
                {
                    throw new Exception("Debe seleccionar un usuario aprobador");
                }
                await service.Editar(Input);
                return Redirect("../Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                //return Redirect("/Error");
                return Page();
            }
        }
    }
}
