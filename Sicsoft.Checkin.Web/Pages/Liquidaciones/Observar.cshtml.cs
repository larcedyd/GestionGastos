using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;


namespace InversionGloblalWeb.Pages.Liquidaciones
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<GastosR, int> liquidaciones;
        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<UsuariosViewModel, int> users;
        private readonly ICrudApi<RolesViewModel, int> roles;
        private readonly ICrudApi<InfoLiquid, int> info;

        [BindProperty(SupportsGet = true)]
        public GastosR Liquidacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public ComprasViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }

        [BindProperty]
        public RolesViewModel[] Rols { get; set; }
        public ObservarModel(ICrudApi<GastosR, int> liquidaciones, ICrudApi<ComprasViewModel, int> service, ICrudApi<UsuariosViewModel, int> users, ICrudApi<RolesViewModel, int> roles, ICrudApi<InfoLiquid, int> info)
        {
            this.liquidaciones = liquidaciones;
            this.service = service;
            this.users = users;
            this.roles = roles;
            this.info = info;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "4").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                var ids = Convert.ToInt32(id);
                Liquidacion = await liquidaciones.ObtenerPorId(ids);
                Usuarios = await users.ObtenerLista("");
                Rols = await roles.ObtenerLista("");


                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo3 = Liquidacion.EncCierre.idCierre;
                filt.NumCierre = Liquidacion.EncCierre.idCierre;
                //filt.Asignados = true;
                var objetos = await service.ObtenerListaCompras(filt);


                Objeto = new ComprasViewModel[Liquidacion.DetCierre.Length];
                for (int i = 0; i < Objeto.Length; i++)
                {

                    Objeto[i] = objetos.Where(a => a.id == Liquidacion.DetCierre[i].idFactura).FirstOrDefault();
                }


                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnGetAceptar(int idB, string comentario)
        {
            try
            {
                await liquidaciones.CambiaEstado(idB, "A", comentario, int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault()));

                return new JsonResult(true);
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message);
            }
        }


        public async Task<IActionResult> OnPostEnviarCorreo(string recibido)
        {
            try
            {


                InfoLiquid liq = JsonConvert.DeserializeObject<InfoLiquid>(recibido);
                //liq.idCierre = idCierre;
                //liq.emailDest = emailDest;
                //liq.emailCC = emailCC;
                //liq.body = body;

                await info.EnviarCorreo(liq);
                
                return new JsonResult(true);
            }
            catch (Exception ex)
            {



                return new JsonResult(false);
            }
        }

        public async Task<IActionResult> OnGetRechazar(int idB, string comentario)
        {
            try
            {
                await liquidaciones.CambiaEstado(idB, "R", comentario);

                return new JsonResult(true);
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message);
            }
        }

    }
}
