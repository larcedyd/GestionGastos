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
    public class EditarModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<LoginUsuarioViewModel, int> usuario;
        private readonly ICrudApi<GastosViewModel, int> gastos;
        private readonly ICrudApi<GastosR, int> liquidaciones;
        private readonly ICrudApi<ComprasViewModel, int> compras;


        [BindProperty(SupportsGet = true)]
        public ComprasViewModel[] Objeto { get; set; }

        [BindProperty(SupportsGet = true)]
        public GastosR Liquidacion { get; set; }


        [BindProperty]
        public string[] Periodos { get; set; }


        [BindProperty]
        public string Periodo { get; set; }

        [BindProperty]
        public LoginUsuarioViewModel[] Usuarios { get; set; }

        [BindProperty]
        public GastosViewModel[] Gastos { get; set; }

        [BindProperty]
        public LoginUsuarioViewModel Usuario { get; set; }


        public EditarModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<LoginUsuarioViewModel, int> usuario, ICrudApi<GastosViewModel, int> gastos,
          ICrudApi<GastosR, int> liquidaciones, ICrudApi<ComprasViewModel, int> compras)
        {
            this.gastos = gastos;
            this.service = service;
            this.usuario = usuario;
            this.liquidaciones = liquidaciones;
            this.compras = compras;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var ids = Convert.ToInt32(id);
                Liquidacion = await liquidaciones.ObtenerPorId(ids);

                Periodo = Liquidacion.EncCierre.Periodo;


                filtro.FechaInicio = Liquidacion.EncCierre.FechaInicial;
                filtro.FechaFinal = Liquidacion.EncCierre.FechaFinal; 


                var idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());

                // Objeto = await service.ObtenerLista(filtro);

                Usuarios = await usuario.ObtenerLista("");

                Usuario = Usuarios.Where(a => a.id == idLogin).FirstOrDefault();
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
                filt.FechaFinal = filt.FechaInicio.AddMonths(1);
                //filt.Asignados = true;
                var objetos = await service.ObtenerLista(filt);


                Objeto = new ComprasViewModel[Liquidacion.DetCierre.Length];
                for(int i = 0; i < Objeto.Length; i++)
                {
                    
                    Objeto[i] = objetos.Where(a => a.id == Liquidacion.DetCierre[i].idFactura).FirstOrDefault();
                }


                await compras.RealizarLecturaEmails();
                await compras.LecturaBandejaEntrada();

                //  Gastos = await gastos.ObtenerLista("");

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnGetBuscar(int idB, int cierre)
        {
            try
            {


                var ids = Convert.ToInt32(idB);

                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Texto = idB.ToString();
                filt.FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 25);
                filt.FechaFinal = filt.FechaInicio.AddMonths(1);
                filt.Codigo2 = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                filt.Codigo3 = cierre;
                filt.Asignados = true;
                var objetos = await service.ObtenerLista(filt);

                var objeto = objetos.Where(a => a.ClaveHacienda.ToString().Contains(filt.Texto.ToUpper()) || a.ConsecutivoHacienda.ToString().Contains(filt.Texto.ToUpper()) 
                ).FirstOrDefault();



                return new JsonResult(objeto);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());


                return new JsonResult(error);
            }
        }


        public async Task<IActionResult> OnGetEstado(int idB)
        {
            try
            {
                await liquidaciones.CambiaEstado(idB, "E", "");

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                 


                return new JsonResult(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            string error = "";


            try
            {

                RecibidoC recibido = JsonConvert.DeserializeObject<RecibidoC>(recibidos);



                Liquidacion = new GastosR();
                Liquidacion.EncCierre = new EncCierreViewModel();
                Liquidacion.DetCierre = new DetCierreViewModel[recibido.DetCompras.Length];

                Liquidacion.EncCierre.idCierre = recibido.EncCompras.idCierre;
                Liquidacion.EncCierre.Periodo = recibido.EncCompras.Periodo;
                Liquidacion.EncCierre.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                Liquidacion.EncCierre.Descuento = recibido.EncCompras.Descuento;

                Liquidacion.EncCierre.FechaInicial = recibido.EncCompras.FechaInicial;
                Liquidacion.EncCierre.FechaFinal = recibido.EncCompras.FechaFinal;
                Liquidacion.EncCierre.SubTotal = recibido.EncCompras.SubTotal;
                Liquidacion.EncCierre.Impuestos = recibido.EncCompras.Impuestos;
                Liquidacion.EncCierre.Impuesto1 = recibido.EncCompras.Impuesto1;
                Liquidacion.EncCierre.Impuesto2 = recibido.EncCompras.Impuesto2;
                Liquidacion.EncCierre.Impuesto4 = recibido.EncCompras.Impuesto4;
                Liquidacion.EncCierre.Impuesto8 = recibido.EncCompras.Impuesto8;
                Liquidacion.EncCierre.Impuesto13 = recibido.EncCompras.Impuesto13;
                Liquidacion.EncCierre.Total = recibido.EncCompras.Total;


                short cantidad = 1;

                foreach (var item in recibido.DetCompras)
                {
                    Liquidacion.DetCierre[cantidad - 1] = new DetCierreViewModel();
                    Liquidacion.DetCierre[cantidad - 1].idFactura = item;

                    cantidad++;
                }


                
                await liquidaciones.Editar(Liquidacion);

                
                return new JsonResult(true);
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);
                return new JsonResult(error);
                //return new JsonResult(false);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                return new JsonResult(false);
            }
        }




    }
}
