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
        [BindProperty(SupportsGet = true)]
        public GastosR Liquidacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public ComprasViewModel[] Objeto { get; set; }
        public ObservarModel(ICrudApi<GastosR, int> liquidaciones, ICrudApi<ComprasViewModel, int> service)
        {
            this.liquidaciones = liquidaciones;
            this.service = service;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var ids = Convert.ToInt32(id);
                Liquidacion = await liquidaciones.ObtenerPorId(ids);


                

                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo3 = Liquidacion.EncCierre.idCierre;
                //filt.Asignados = true;
                var objetos = await service.ObtenerLista(filt);


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
