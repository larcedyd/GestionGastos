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
        private readonly ICrudApi<ComprasInsercionViewModel, int> insercionCompras;

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
        [BindProperty]
        public string Pais { get; set; }
        //Programacion para el modal de insercion
        [BindProperty(SupportsGet = true)]
        public ComprasInsercionViewModel Objeto1 { get; set; }
        //Termina

        public EditarModel(ICrudApi<ComprasViewModel, int> service, ICrudApi<LoginUsuarioViewModel, int> usuario, ICrudApi<GastosViewModel, int> gastos,
          ICrudApi<GastosR, int> liquidaciones, ICrudApi<ComprasViewModel, int> compras, ICrudApi<ComprasInsercionViewModel, int> insercionCompras)
        {
            this.gastos = gastos;
            this.service = service;
            this.usuario = usuario;
            this.liquidaciones = liquidaciones;
            this.compras = compras;
            this.insercionCompras = insercionCompras;
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

                Periodo = Liquidacion.EncCierre.Periodo;


                filtro.FechaInicio = Liquidacion.EncCierre.FechaInicial;
                filtro.FechaFinal = Liquidacion.EncCierre.FechaFinal; 


                var idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());

                // Objeto = await service.ObtenerLista(filtro);

                Usuarios = await usuario.ObtenerLista("");

                Usuario = Usuarios.Where(a => a.id == idLogin).FirstOrDefault();

                ParametrosFiltros filt = new ParametrosFiltros();

                var Pais = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Pais").Select(s1 => s1.Value).FirstOrDefault();

                this.Pais = Pais;

                /*  filt.FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
                  filt.FechaFinal = filt.FechaInicio.AddMonths(1);*/
                filt.FechaInicio = filtro.FechaInicio;
                filt.FechaFinal = filtro.FechaFinal;
                filt.NumCierre = Liquidacion.EncCierre.idCierre;
                //filt.Asignados = true;
                var objetos = await service.ObtenerListaCompras(filt);


                Objeto = new ComprasViewModel[Liquidacion.DetCierre.Length];
                for(int i = 0; i < Objeto.Length; i++)
                {
                    
                    Objeto[i] = objetos.Where(a => a.id == Liquidacion.DetCierre[i].idFactura).FirstOrDefault();
                    Objeto[i].PdfFac = new byte[0];
                }
                //Liquidacion.EncCierre.
                Objeto = Objeto.OrderBy(a => a.FecFactura).ToArray();
                await compras.RealizarLecturaEmails();
                await compras.LecturaBandejaEntrada();

                  Gastos = await gastos.ObtenerLista("");

                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
        //Este metodo busca las facturas para ponerlas en la liquidacion 
        public async Task<IActionResult> OnGetBuscar(string idB, int cierre)
        {
            try
            {


                //var ids = Convert.ToInt32(idB);
                var cierres = await liquidaciones.ObtenerPorId(cierre); 
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Texto = idB.ToString();
                
                filt.FechaInicio = cierres.EncCierre.FechaInicial;
                filt.FechaFinal = cierres.EncCierre.FechaFinal;

                filt.Codigo2 = 0; //int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                filt.Codigo3 = cierre;
               // filt.Asignados = true;
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


                
                await liquidaciones.Editar(Liquidacion);


                var obj = new
                {
                    success = true,
                    mensaje = ""
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
            //        mensaje = "Error en el api: -> "+errores.Message + " -> " + errores.StackTraceString
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
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace
                };
                return new JsonResult(obj);
            }
        }


        //Generar y Enviar a Revision

        public async Task<IActionResult> OnPostGeneraryEnviar(string recibidos)
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



                await liquidaciones.Editar(Liquidacion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);
                var obj = new
                {
                    success = false,
                    mensaje = errores.Message
                };
                return new JsonResult(obj);
                //return new JsonResult(false);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                var obj = new
                {
                    success = false,
                    mensaje = ex.Message
                };
                return new JsonResult(obj);
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
                Objeto1.EncCompras.CodMoneda = recibido.EncCompras.CodMoneda == "NULL" ? "CRC" : recibido.EncCompras.CodMoneda;
                Objeto1.EncCompras.CodCliente = recibido.EncCompras.CodCliente;
                Objeto1.EncCompras.NomCliente = recibido.EncCompras.NomCliente;
                Objeto1.EncCompras.TotalImpuesto = recibido.EncCompras.TotalImpuesto;
                Objeto1.EncCompras.TotalDescuentos = recibido.EncCompras.TotalDescuentos;
                Objeto1.EncCompras.Impuesto1 = recibido.EncCompras.Impuesto1;
                Objeto1.EncCompras.Impuesto2 = recibido.EncCompras.Impuesto2;
                Objeto1.EncCompras.Impuesto4 = recibido.EncCompras.Impuesto4;
                Objeto1.EncCompras.Impuesto8 = recibido.EncCompras.Impuesto8;
                Objeto1.EncCompras.Impuesto13 = recibido.EncCompras.Impuesto13;
                Objeto1.EncCompras.TotalComprobante = recibido.EncCompras.TotalComprobante;
                Objeto1.EncCompras.ImagenBase64 = recibido.EncCompras.ImagenBase64;
                Objeto1.EncCompras.TotalVenta = recibido.EncCompras.TotalVenta;
                Objeto1.EncCompras.RegimenSimplificado = recibido.EncCompras.RegimenSimplificado;
                Objeto1.EncCompras.GastosVarios = recibido.EncCompras.GastosVarios;
                Objeto1.EncCompras.FacturaExterior = recibido.EncCompras.FacturaExterior;
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



                var resp = await insercionCompras.Agregar(Objeto1);

                var obj = new
                {
                    success = true,
                    resp = resp.EncCompras.id
                };

                return new JsonResult(obj);
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);

                var obj = new
                {
                    success = false,
                    resp = errores.Message
                };
                return new JsonResult(obj);
                //return new JsonResult(false);
            }
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

        //Programacion para encontrar una factura manual

        public async Task<IActionResult> OnGetBuscarFM(string idB)
        {
            try
            {
                var FM = await service.ObtenerPorId(Convert.ToInt32(idB));

                return new JsonResult(FM);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());


                return new JsonResult(error);
            }
        }


        //Programacion para editar una factura Manual
        public async Task<IActionResult> OnPostEditarCompra(string recibidos)
        {
            string error = "";


            try
            {

                ComprasRecibidoViewModel recibido = JsonConvert.DeserializeObject<ComprasRecibidoViewModel>(recibidos);



                Objeto1 = new ComprasInsercionViewModel();
                Objeto1.EncCompras = new EncComprasViewModel();
                Objeto1.DetCompras = new DetComprasViewModel[recibido.DetCompras.Length];

                Objeto1.EncCompras.id = recibido.EncCompras.id;
                Objeto1.EncCompras.ConsecutivoHacienda = recibido.EncCompras.ConsecutivoHacienda;
                Objeto1.EncCompras.ClaveHacienda = recibido.EncCompras.ClaveHacienda;
                Objeto1.EncCompras.NumFactura = recibido.EncCompras.NumFactura;

                Objeto1.EncCompras.TipoIdentificacionCliente = recibido.EncCompras.TipoIdentificacionCliente;
                Objeto1.EncCompras.CodProveedor = recibido.EncCompras.CodProveedor;
                Objeto1.EncCompras.NomProveedor = recibido.EncCompras.NomProveedor;
                Objeto1.EncCompras.FecFactura = recibido.EncCompras.FecFactura;
                Objeto1.EncCompras.CodigoActividadEconomica = recibido.EncCompras.CodActividadEconomica;
                Objeto1.EncCompras.CodMoneda = recibido.EncCompras.CodMoneda == "NULL" ? "CRC" : recibido.EncCompras.CodMoneda;
                Objeto1.EncCompras.CodCliente = recibido.EncCompras.CodCliente;
                Objeto1.EncCompras.NomCliente = recibido.EncCompras.NomCliente;
                Objeto1.EncCompras.TotalImpuesto = recibido.EncCompras.TotalImpuesto;
                Objeto1.EncCompras.TotalDescuentos = recibido.EncCompras.TotalDescuentos;
                Objeto1.EncCompras.Impuesto1 = recibido.EncCompras.Impuesto1;
                Objeto1.EncCompras.Impuesto2 = recibido.EncCompras.Impuesto2;
                Objeto1.EncCompras.Impuesto4 = recibido.EncCompras.Impuesto4;
                Objeto1.EncCompras.Impuesto8 = recibido.EncCompras.Impuesto8;
                Objeto1.EncCompras.Impuesto13 = recibido.EncCompras.Impuesto13;
                Objeto1.EncCompras.TotalComprobante = recibido.EncCompras.TotalComprobante;
                Objeto1.EncCompras.ImagenBase64 = recibido.EncCompras.ImagenBase64;
                Objeto1.EncCompras.TotalVenta = recibido.EncCompras.TotalVenta;
                Objeto1.EncCompras.RegimenSimplificado = recibido.EncCompras.RegimenSimplificado;
                Objeto1.EncCompras.GastosVarios = recibido.EncCompras.GastosVarios;
                Objeto1.EncCompras.FacturaExterior = recibido.EncCompras.FacturaExterior;
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



                var resp = await insercionCompras.EditarFacturaManual(Objeto1);

                var obj = new
                {
                    success = true,
                    resp = resp.EncCompras.id
                };

                return new JsonResult(obj);
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);

                var obj = new
                {
                    success = true,
                    resp = errores.Message
                };
                return new JsonResult(obj);
                //return new JsonResult(false);
            }
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
