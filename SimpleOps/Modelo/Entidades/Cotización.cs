using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;
using SimpleOps.DocumentosGráficos;
using AutoMapper;
using System.IO;

namespace SimpleOps.Modelo {



    /// <summary>
    /// Ofrecimiento de precios de venta de varios productos a un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)] // Desde la lógica de la operación no hay problema en realizar dos cotizaciones al mismo tiempo al mismo cliente desde dos computadores diferentes, aunque no sería deseable que se hicieran con precios diferentes la concurrencia no es tanto problema.
    class Cotización : Registro {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public List<LíneaCotización> Líneas { get; set; } = new List<LíneaCotización>();

        #endregion Propiedades>


        #region Constructores

        private Cotización() { } // Solo para que EF Core no saque error.

        public Cotización(Cliente cliente) => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}";


        public DatosCotización ObtenerDatos(OpcionesDocumento opcionesDocumento, TipoCotización tipoCotización, int? tamañoImagenes, 
            int totalPáginasCatálogo = 1) {

            var mapeador = new Mapper(ConfiguraciónMapeadorCotización);
            var datos = mapeador.Map<DatosCotización>(this);
            datos.NombreDocumento = tipoCotización.ToString();

            var mapeadorEmpresa = new Mapper(ConfiguraciónMapeadorEmpresa);
            datos.Empresa = mapeadorEmpresa.Map<DatosEmpresa>(Empresa);
            datos.Columnas = ObtenerOpcionesColumnas(datos);

            datos.LogoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaImagenesPlantillas(),
                opcionesDocumento.ModoImpresión ? NombreArchivoLogoEmpresaImpresión : NombreArchivoLogoEmpresa), paraHtml: true);
            datos.Logo2Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImagenesPlantillas(), 
                opcionesDocumento.ModoImpresión ? AgregarSufijo(NombreArchivoLogoEmpresaImpresión, "2") : AgregarSufijo(NombreArchivoLogoEmpresa,"2")), 
                paraHtml: true);
            datos.Logo3Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImagenesPlantillas(),
                opcionesDocumento.ModoImpresión ? AgregarSufijo(NombreArchivoLogoEmpresaImpresión, "3") : AgregarSufijo(NombreArchivoLogoEmpresa, "3")), 
                paraHtml: true);
            datos.Logo4Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImagenesPlantillas(),
                opcionesDocumento.ModoImpresión ? AgregarSufijo(NombreArchivoLogoEmpresaImpresión, "4") : AgregarSufijo(NombreArchivoLogoEmpresa, "4")),
                paraHtml: true);

            (datos.ModoDocumento, datos.TotalPáginas) = tipoCotización switch {
                TipoCotización.Catálogo => (DatosDocumento.Modo.PáginasIndependientes, totalPáginasCatálogo),
                TipoCotización.Cotización => throw new NotImplementedException(), // Pendiente desarrollar para la cotización.
                _ => throw new NotImplementedException(),
            };

            var rutaImagenesProductos = ObtenerRutaImagenesProductos();
            foreach (var línea in Líneas) {

                var rutaImagenProducto = ObtenerRutaImagenProducto(línea.Producto?.Referencia);
                if (rutaImagenProducto != null) {

                    if (tamañoImagenes != null) {

                        var rutaImagenRedimensionada = Path.Combine(ObtenerRutaCarpeta(rutaImagenesProductos, ((int)tamañoImagenes).ATexto(),
                            crearSiNoExiste: true), Path.GetFileName(rutaImagenProducto));
                        if (RedimensionarImagen(rutaImagenProducto, rutaImagenRedimensionada, (int)tamañoImagenes, (int)tamañoImagenes)) {
                            rutaImagenProducto = rutaImagenRedimensionada;
                        } else {
                            MostrarError($"No se pudo redimensionar la imagen {rutaImagenProducto}.");
                        }

                    }

                    datos.ImágenesProductosBase64.Add(línea.Producto?.Referencia!, ObtenerBase64(rutaImagenProducto, paraHtml: true)); // Se asegura que Referencia no es nula porque ya encontró una imagen asociada a ella.

                }

            }

            datos.ModoImpresión = opcionesDocumento.ModoImpresión;
            datos.MostrarInformaciónAdicional = opcionesDocumento.MostrarInformaciónAdicional;
            datos.NombreArchivoPropio = "Catálogo - " + Cliente?.Nombre;

            return datos;

        } // ObtenerDatos>


        public Integración.DatosCotización ObtenerDatosIntegración() {

            var mapeador = new Mapper(ConfiguraciónMapeadorCotizaciónIntegración);
            var datos = mapeador.Map<Integración.DatosCotización>(this);
            return datos;

        } // ObtenerDatosIntegración>


        #endregion Métodos y Funciones>


    } // Cotización>



} // SimpleOps.Modelo>
