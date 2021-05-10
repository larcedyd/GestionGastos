using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Security.Claims;
using System.Threading.Tasks;
using InversionGloblalWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace InversionGloblalWeb.Pages.Compras
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<AsignacionViewModel, int> asignacion;

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public ComprasViewModel[] Objeto { get; set; }

        public IndexModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<AsignacionViewModel, int> asignacion)
        {
            this.service = service;
            this.asignacion = asignacion;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "5").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                if (string.IsNullOrEmpty(filtro.Texto))
                {

                await service.RealizarLecturaEmails();
                await service.LecturaBandejaEntrada();
                }

                DateTime time = new DateTime();

                if(time == filtro.FechaInicio)
                {
                    filtro.FechaFinal = DateTime.Now;
                    filtro.FechaInicio = filtro.FechaFinal.AddMonths(-1);
                }

                Objeto = await service.ObtenerListaCompras(filtro);


                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnGetAsignar(string id)
        {
            try
            {
                var ids = Convert.ToInt32(id);
                AsignacionViewModel asig = new AsignacionViewModel();
                asig.idFac = ids;
                asig.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                await asignacion.Editar(asig);

                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                return new JsonResult(false);
            }
        }


        public async Task<ActionResult> OnGetPDF(int id)
        {
            try
            {
                var factura = await service.ObtenerPorId(id);
                PdfReader reader = new PdfReader(new MemoryStream(factura.PdfFac));
                PdfStamper stamper = new PdfStamper(reader, new FileStream(factura.PdfFactura, FileMode.Create));


                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                byte[] all = null;
                return new JsonResult(all);
            }
        }
    }
}
