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

namespace InversionGloblalWeb.Pages.Compras
{
    public class NuevoModel : PageModel
    {

        private readonly ICrudApi<ComprasInsercionViewModel, int> service;
        private readonly ICrudApi<GastosViewModel, int> gastos;


        [BindProperty(SupportsGet = true)]
        public ComprasInsercionViewModel Objeto { get; set; }

        [BindProperty(SupportsGet = true)]
        public GastosViewModel[] Gastos { get; set; }

        public NuevoModel(ICrudApi<ComprasInsercionViewModel, int> service, ICrudApi<GastosViewModel, int> gastos)
        {
            this.service = service;
            this.gastos = gastos;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                
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

        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            string error = "";


            try
            {

                ComprasRecibidoViewModel recibido = JsonConvert.DeserializeObject<ComprasRecibidoViewModel>(recibidos);



                Objeto = new ComprasInsercionViewModel();
                Objeto.EncCompras = new EncComprasViewModel();
                Objeto.DetCompras = new DetComprasViewModel[recibido.DetCompras.Length];


                Objeto.EncCompras.ConsecutivoHacienda = recibido.EncCompras.ConsecutivoHacienda;
                Objeto.EncCompras.ClaveHacienda = recibido.EncCompras.ClaveHacienda;
                Objeto.EncCompras.NumFactura = recibido.EncCompras.NumFactura;

                Objeto.EncCompras.TipoIdentificacionCliente = recibido.EncCompras.TipoIdentificacionCliente;
                Objeto.EncCompras.CodProveedor = recibido.EncCompras.CodProveedor;
                Objeto.EncCompras.NomProveedor = recibido.EncCompras.NomProveedor;
                Objeto.EncCompras.FecFactura = recibido.EncCompras.FecFactura;
                Objeto.EncCompras.CodigoActividadEconomica = recibido.EncCompras.CodActividadEconomica;
                Objeto.EncCompras.CodMoneda = recibido.EncCompras.CodMoneda;
                Objeto.EncCompras.CodCliente = recibido.EncCompras.CodCliente;
                Objeto.EncCompras.NomCliente = recibido.EncCompras.NomCliente;
                Objeto.EncCompras.TotalImpuesto = recibido.EncCompras.TotalImpuesto;
                Objeto.EncCompras.TotalDescuentos = recibido.EncCompras.Impuesto1;
                Objeto.EncCompras.Impuesto1 = recibido.EncCompras.Impuesto1;
                Objeto.EncCompras.Impuesto2 = recibido.EncCompras.Impuesto2;
                Objeto.EncCompras.Impuesto4 = recibido.EncCompras.Impuesto4;
                Objeto.EncCompras.Impuesto8 = recibido.EncCompras.Impuesto8;
                Objeto.EncCompras.Impuesto13 = recibido.EncCompras.Impuesto13;
                Objeto.EncCompras.TotalComprobante = recibido.EncCompras.TotalComprobante;
                Objeto.EncCompras.ImagenBase64 = recibido.EncCompras.ImagenBase64;
                Objeto.EncCompras.TotalVenta = recibido.EncCompras.TotalVenta;
                Objeto.EncCompras.RegimenSimplificado = recibido.EncCompras.RegimenSimplificado;
                Objeto.EncCompras.FacturaExterior = recibido.EncCompras.FacturaExterior;
                short cantidad = 1;

                foreach (var item in recibido.DetCompras)
                {
                    Objeto.DetCompras[cantidad - 1] = new DetComprasViewModel();
                    Objeto.DetCompras[cantidad - 1].CodPro = item.CodPro;
                    Objeto.DetCompras[cantidad - 1].NomPro = item.NomPro;
                    Objeto.DetCompras[cantidad - 1].Cantidad = item.Cantidad;
                    Objeto.DetCompras[cantidad - 1].PrecioUnitario = item.PrecioUnitario;
                    Objeto.DetCompras[cantidad - 1].MontoDescuento = item.MontoDescuento;
                    Objeto.DetCompras[cantidad - 1].ImpuestoTarifa = item.ImpuestoTarifa;
                    Objeto.DetCompras[cantidad - 1].ImpuestoMonto = item.ImpuestoMonto;
                    Objeto.DetCompras[cantidad - 1].MontoTotalLinea = item.MontoTotalLinea;
                    Objeto.DetCompras[cantidad - 1].MontoTotal = item.MontoTotalLinea;
                    Objeto.DetCompras[cantidad - 1].SubTotal = (item.Cantidad * item.PrecioUnitario) - item.MontoDescuento;
                    Objeto.DetCompras[cantidad - 1].idTipoGasto = item.idTipoGasto;

                    Objeto.DetCompras[cantidad - 1].UnidadMedida = "";

                    cantidad++;
                }


         
                await service.Agregar(Objeto);

                 
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
