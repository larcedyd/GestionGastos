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
    public class NuevoModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        private readonly ICrudApi<ComprasViewModel, int> service;
        private readonly ICrudApi<LoginUsuarioViewModel, int> usuario;
        private readonly ICrudApi<GastosViewModel, int> gastos;
        private readonly ICrudApi<GastosR, int> liquidaciones;
        private readonly ICrudApi<ComprasViewModel, int> compras;
        private readonly ICrudApi<ComprasInsercionViewModel, int> insercionCompras;
        private readonly ICrudApi<MonedasViewModel, bool> mon;

        [BindProperty(SupportsGet = true)]
        public ComprasViewModel[] Objeto { get; set; }

        [BindProperty(SupportsGet = true)]
        public GastosR Liquidacion { get; set; }


        [BindProperty]
        public string[] Periodos { get; set; }


        [BindProperty]
        public List<MonedasViewModel> Monedas { get; set; }


        [BindProperty]
        public string Periodo { get; set; }

        [BindProperty]
        public LoginUsuarioViewModel[] Usuarios { get; set; }

        [BindProperty]
        public GastosViewModel[] Gastos { get; set; }

        [BindProperty]
        public LoginUsuarioViewModel Usuario { get; set; }

        //Programacion para el modal de insercion
        [BindProperty(SupportsGet = true)]
        public ComprasInsercionViewModel Objeto1 { get; set; }
        //Termina

        public NuevoModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<LoginUsuarioViewModel, int> usuario, ICrudApi<GastosViewModel, int> gastos,
            ICrudApi<GastosR, int> liquidaciones, ICrudApi<ComprasViewModel, int> compras, ICrudApi<ComprasInsercionViewModel, int> insercionCompras,
            ICrudApi<MonedasViewModel, bool> mon)
        {
            this.gastos = gastos;
            this.service = service;
            this.usuario = usuario;
            this.liquidaciones = liquidaciones;
            this.compras = compras;
            this.insercionCompras = insercionCompras;
            this.mon = mon;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "4").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                var idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                DateTime time = DateTime.Now;

                Periodos = new string[12] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Setiembre", "Octubre", "Noviembre", "Diciembre" };

                Monedas = new List<MonedasViewModel>();


                if (time.Day < 30)
                {
                    filtro.FechaInicio = DateTime.Now;
                    filtro.FechaInicio = filtro.FechaInicio.AddMonths(-1);
                    filtro.FechaInicio = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 26);

                    filtro.FechaFinal = filtro.FechaInicio.AddMonths(1);//.AddDays(-1); Agregar al final 
                    DateTime primerDia = new DateTime(filtro.FechaFinal.Year, filtro.FechaFinal.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                }
                else
                {

                    filtro.FechaInicio = DateTime.Now;
 
                    filtro.FechaInicio = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 26);
                    filtro.FechaFinal = filtro.FechaInicio.AddMonths(1).AddDays(-1);

                    DateTime primerDia = new DateTime(filtro.FechaFinal.Year, filtro.FechaFinal.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;
                }

                

               
                


                Periodo = Periodos[filtro.FechaFinal.Month - 1];


                MonedasViewModel moneda = new MonedasViewModel();
                
                moneda.identificador = "CRC";
                moneda.Moneda = "Colones";

                var resp = await mon.VCierre(idLogin,Periodo,DateTime.Now,moneda.identificador);
                if(resp)
                {
                    Monedas.Add(moneda);

                }




                MonedasViewModel moneda2 = new MonedasViewModel();
                moneda2.identificador = "USD";
                moneda2.Moneda = "Dolares";
                 resp = await mon.VCierre(idLogin, Periodo, DateTime.Now, moneda2.identificador);
                if (resp)
                {
                    Monedas.Add(moneda2);

                }

            

                MonedasViewModel moneda3 = new MonedasViewModel();
                moneda3.identificador = "EUR";
                moneda3.Moneda = "Euros";
                resp = await mon.VCierre(idLogin, Periodo, DateTime.Now, moneda3.identificador);
                if (resp)
                {
                    Monedas.Add(moneda3);

                }
                

                Usuarios = await usuario.ObtenerLista("");

                Usuario = Usuarios.Where(a => a.id == idLogin).FirstOrDefault();

                Gastos = await gastos.ObtenerLista("");

                await compras.RealizarLecturaEmails();
                await compras.LecturaBandejaEntrada();

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        //Revisar Liquidacion

        //public async Task<IActionResult> OnGetRevisar(string id)
        ///


        public async Task<IActionResult> OnGetBuscar(string id)
        {
            try
            {


                //var ids = Convert.ToInt32(id);

                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Texto = id;
                filt.FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month -1, 26);
                filt.FechaFinal = filt.FechaInicio.AddMonths(1).AddDays(-1);
                DateTime time = DateTime.Now;
                if (time.Day < 30)
                {
                    filt.FechaInicio = DateTime.Now;
                    filt.FechaInicio = filt.FechaInicio.AddMonths(-1);
                    filt.FechaInicio = new DateTime(filt.FechaInicio.Year, filt.FechaInicio.Month, 26);
                    filt.FechaFinal = filt.FechaInicio.AddMonths(1);//.AddDays(-1);
                    DateTime primerDia = new DateTime(filtro.FechaFinal.Year, filtro.FechaFinal.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                }
                else
                {

                    filt.FechaInicio = DateTime.Now;

                    filt.FechaInicio = new DateTime(filt.FechaInicio.Year, filt.FechaInicio.Month, 26);
                    filt.FechaFinal = filt.FechaInicio.AddMonths(1).AddDays(-1);
                    DateTime primerDia = new DateTime(filtro.FechaFinal.Year, filtro.FechaFinal.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                }


                filt.Asignados = true;
                filt.Codigo2 = 0;
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


        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            string error = "";
            

            try
            {
             
                RecibidoC recibido = JsonConvert.DeserializeObject<RecibidoC>(recibidos);

               

                Liquidacion = new GastosR();
                Liquidacion.EncCierre = new EncCierreViewModel();
                Liquidacion.DetCierre = new DetCierreViewModel[recibido.DetCompras.Length];


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
                Liquidacion.EncCierre.Estado = "P";
                Liquidacion.EncCierre.CodMoneda = recibido.EncCompras.CodMoneda;
                Liquidacion.EncCierre.TotalOtrosCargos = recibido.EncCompras.TotalOtrosCargos;
                Liquidacion.EncCierre.Observacion = recibido.EncCompras.Observacion;
                short cantidad = 1;

                foreach (var item in recibido.DetCompras)
                {
                    Liquidacion.DetCierre[cantidad - 1] = new DetCierreViewModel();
                    Liquidacion.DetCierre[cantidad - 1].idFactura = item.idFactura;
                    Liquidacion.DetCierre[cantidad - 1].idTipoGasto = item.idTipoGasto;
                    Liquidacion.DetCierre[cantidad - 1].Comentario = item.Comentario;
                    cantidad++;
                }


                error += " Antes de Agregar ";
                await liquidaciones.Agregar(Liquidacion);

                error += " DEspues de agregar";
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

        public async Task<IActionResult> OnPostGeneraryEnviar(string recibidos)
        {
            string error = "";


            try
            {

                RecibidoC recibido = JsonConvert.DeserializeObject<RecibidoC>(recibidos);



                Liquidacion = new GastosR();
                Liquidacion.EncCierre = new EncCierreViewModel();
                Liquidacion.DetCierre = new DetCierreViewModel[recibido.DetCompras.Length];


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
                Liquidacion.EncCierre.Estado = "E";
                Liquidacion.EncCierre.CodMoneda = recibido.EncCompras.CodMoneda;
                Liquidacion.EncCierre.TotalOtrosCargos = recibido.EncCompras.TotalOtrosCargos;
                Liquidacion.EncCierre.Observacion = recibido.EncCompras.Observacion;
                short cantidad = 1;

                foreach (var item in recibido.DetCompras)
                {
                    Liquidacion.DetCierre[cantidad - 1] = new DetCierreViewModel();
                    Liquidacion.DetCierre[cantidad - 1].idFactura = item.idFactura;
                    Liquidacion.DetCierre[cantidad - 1].idTipoGasto = item.idTipoGasto;
                    Liquidacion.DetCierre[cantidad - 1].Comentario = item.Comentario;

                    cantidad++;
                }


                error += " Antes de Agregar ";
                var c = await liquidaciones.Agregar(Liquidacion);
               
                error += " DEspues de agregar";




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

        //Programacion para inserta factura Manual
        public async Task<IActionResult> OnPostInsertar(string recibidos)
        {
            string error = "";


            try
            {

                ComprasRecibidoViewModel recibido = JsonConvert.DeserializeObject<ComprasRecibidoViewModel>(recibidos);



                Objeto1 = new ComprasInsercionViewModel();
                Objeto1.EncCompras = new EncComprasViewModel();
                Objeto1.DetCompras = new DetComprasViewModel[recibido.DetCompras.Length];


                Objeto1.EncCompras.ConsecutivoHacienda = recibido.EncCompras.ConsecutivoHacienda;
                Objeto1.EncCompras.ClaveHacienda = recibido.EncCompras.ClaveHacienda;
                Objeto1.EncCompras.NumFactura = recibido.EncCompras.NumFactura;

                Objeto1.EncCompras.TipoIdentificacionCliente = recibido.EncCompras.TipoIdentificacionCliente;
                Objeto1.EncCompras.CodProveedor = recibido.EncCompras.CodProveedor;
                Objeto1.EncCompras.NomProveedor = recibido.EncCompras.NomProveedor;
                Objeto1.EncCompras.FecFactura = recibido.EncCompras.FecFactura;
                Objeto1.EncCompras.CodigoActividadEconomica = recibido.EncCompras.CodActividadEconomica;
                Objeto1.EncCompras.CodMoneda = recibido.EncCompras.CodMoneda == "NULL" ? "CRC": recibido.EncCompras.CodMoneda;
                Objeto1.EncCompras.CodCliente = recibido.EncCompras.CodCliente;
                Objeto1.EncCompras.NomCliente = recibido.EncCompras.NomCliente;
                Objeto1.EncCompras.TotalImpuesto = recibido.EncCompras.TotalImpuesto;
                Objeto1.EncCompras.TotalDescuentos = recibido.EncCompras.Impuesto1;
                Objeto1.EncCompras.Impuesto1 = recibido.EncCompras.Impuesto1;
                Objeto1.EncCompras.Impuesto2 = recibido.EncCompras.Impuesto2;
                Objeto1.EncCompras.Impuesto4 = recibido.EncCompras.Impuesto4;
                Objeto1.EncCompras.Impuesto8 = recibido.EncCompras.Impuesto8;
                Objeto1.EncCompras.Impuesto13 = recibido.EncCompras.Impuesto13;
                Objeto1.EncCompras.TotalComprobante = recibido.EncCompras.TotalComprobante;
                Objeto1.EncCompras.ImagenBase64 = recibido.EncCompras.ImagenBase64;
                Objeto1.EncCompras.TotalVenta = recibido.EncCompras.TotalVenta;
                Objeto1.EncCompras.RegimenSimplificado = recibido.EncCompras.RegimenSimplificado;
                Objeto1.EncCompras.FacturaExterior = recibido.EncCompras.FacturaExterior;
                Objeto1.EncCompras.GastosVarios = recibido.EncCompras.GastosVarios;
                Objeto1.EncCompras.FacturaNoRecibida = recibido.EncCompras.FacturaNoRecibida;


                short cantidad = 1;

                foreach (var item in recibido.DetCompras)
                {
                    Objeto1.DetCompras[cantidad - 1] = new DetComprasViewModel();
                    Objeto1.DetCompras[cantidad - 1].CodPro = item.CodPro;
                    Objeto1.DetCompras[cantidad - 1].NomPro = item.NomPro;
                    Objeto1.DetCompras[cantidad - 1].Cantidad = item.Cantidad;
                    Objeto1.DetCompras[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    Objeto1.DetCompras[cantidad - 1].MontoDescuento = item.MontoDescuento;
                    Objeto1.DetCompras[cantidad - 1].ImpuestoTarifa = item.ImpuestoTarifa;
                    Objeto1.DetCompras[cantidad - 1].ImpuestoMonto = item.ImpuestoMonto;
                    Objeto1.DetCompras[cantidad - 1].MontoTotalLinea = item.MontoTotalLinea;
                    Objeto1.DetCompras[cantidad - 1].MontoTotal = item.MontoTotalLinea;
                    Objeto1.DetCompras[cantidad - 1].SubTotal = item.PrecioUnitario - item.MontoDescuento;
                    Objeto1.DetCompras[cantidad - 1].idTipoGasto = item.idTipoGasto;

                    Objeto1.DetCompras[cantidad - 1].UnidadMedida = "";

                    cantidad++;
                }



               var resp =  await insercionCompras.Agregar(Objeto1);

                var obj = new
                {
                    success = true,
                    resp = resp.EncCompras.id
                };

                return new JsonResult(obj);
            }
            //catch (ApiException ex)
            //{
            //    Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
            //    ModelState.AddModelError(string.Empty, errores.Message);

            //    var obj = new
            //    {
            //        success = false,
            //        resp = errores.Message
            //    };
            //    return new JsonResult(obj);
            //    //return new JsonResult(false);
            //}
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                var obj = new
                {
                    success = false,
                    resp = ex.Message
                };
                return new JsonResult(obj);
            }
        }



    }
}
