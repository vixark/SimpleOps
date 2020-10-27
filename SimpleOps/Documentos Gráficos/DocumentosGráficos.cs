using iText.Html2pdf;
using iText.Kernel;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RazorEngineCore;
using SimpleOps.Legal;
using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;
using AutoMapper;

namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// Métodos y funciones estáticas generales relacionadas con los documentos gráficos.
    /// </summary>
    static class DocumentosGráficos { // No debe ser public porque referencia otras clases que no deben ser públicas.


        #region Métodos y Funciones


        /// <summary>
        /// Compila un objeto plantilla complejo con el que se puede generar HTML usando código Razor (CSHTML) con @Model tipado y 
        /// tiene soporte para incluir partes y marcos de HTML (similar a _Partial y _Layout en MVC). Ver más en <see cref="PlantillaBase{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="motorRazor"></param>
        /// <param name="cuerpo"></param>
        /// <param name="partes"></param>
        /// <returns></returns>
        public static PlantillaCompilada<T> CompilarPlantilla<T>(RazorEngine motorRazor, string cuerpo, IDictionary<string, string> partes) where T : class {

            static void configurarOpciones(IRazorEngineCompilationOptionsBuilder opciones) => opciones.AddAssemblyReferenceByName("System.Collections"); // Se intentó agregar para habilitar System.Math opciones.AddAssemblyReferenceByName("System.Runtime.Extensions"); , pero parece que es necesario hacer otros pasos https://stackoverflow.com/questions/19736600/the-name-math-does-not-exist-in-the-current-context. Por el momento mejor se prefiere evitar el uso de System.Math para no agregar ensamblados solo para algo que se puede hacer desde el la clase Datos[]. Esta línea se usó opciones.AddUsing("SimpleOps.DocumentosGráficos"); para intentar agregar este espacio de nombres para evitar el error en compilación de la plantilla si no se usaba, pero resultó que al añadirlo sacaba error de compilación si en la plantilla se usaba también @using SimpleOps.DocumentosGráficos, así que se prefirió mantener el espacio de nombres, pero en la plantilla siempre se deben llamar las clases y métodos de este espacio de nombres con su nombre completo SimpleOps.DocumentosGráficos.[].
            return new PlantillaCompilada<T>(motorRazor.Compile<PlantillaBase<T>>(cuerpo, o => configurarOpciones(o)),
                   partes.ToDictionary(k => k.Key, v => motorRazor.Compile<PlantillaBase<T>>(v.Value, o => configurarOpciones(o))));

        } // Compilar>


        public static bool CrearPdf<D, M>(D documento, DocumentoElectrónico<Factura<Cliente, M>, M>? documentoElectrónico, out string rutaPdf) 
            where D : Factura<Cliente, M> where M : MovimientoProducto { // Una vez compilada la plantilla la primera vez este procedimiento es relativamente rápido en alrededor de 200ms, 150ms de estos son en la generación del HTML que se podrían reducir si se almacenaran los archivos CSHTML en memoria o directamente la plantilla compilada según https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout, pero por el momento se prefiere evitar esto para no complicar más el procedimiento y facilitar hacer cambios a los archivos CSHTML al permitir que estos cambios se hagan efectivos sin necesidad de reiniciar la aplicación.

            otraVez:
            var creado = false;
            rutaPdf = "";
            if (documentoElectrónico == null) return false;
            var motorRazor = new RazorEngine();    
            PlantillaCompilada<DatosVenta>? plantillaCompilada = null;

            try {

                var cuerpo = File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.VentaPdf));
                var partes = new Dictionary<string, string>() {
                    {"Marco", File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.MarcoPdf))},
                    {"Lista", File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.ListaProductosPdf))}, // Si se quiere usar el modo de páginas independientes (cada una con su encabezado y pie de página) se deben agregar las páginas extra como partes en el diccionario, así: {"Página2", "<h1>Título Página 2</h1><p>Contenido de la página 2.</p>"},
                };
                plantillaCompilada = CompilarPlantilla<DatosVenta>(motorRazor, cuerpo, partes);

            } catch (RazorEngineCompilationException ex) {

                MostrarError($"No se pudo cargar la plantilla HTML necesaria para construir el PDF de la factura electrónica.{DobleLínea}{ex.Message}");
                creado = false;

            }

            if (plantillaCompilada != null) {

                creado = CrearPdf(documento, documentoElectrónico, plantillaCompilada, 
                    opcionesDocumento: new OpcionesDocumento() { ModoImpresión = false }, out rutaPdf);
                if (Empresa.GenerarPDFsAdicionalesImpresión) CrearPdf(documento, documentoElectrónico, plantillaCompilada, 
                    opcionesDocumento: new OpcionesDocumento() { ModoImpresión = true }, out _);

            }

            if (ModoDesarrolloPlantillasDocumentos) {

                if (creado) AbrirArchivo(rutaPdf);
                SuspenderEjecuciónEnModoDesarrollo(); // Al estar detenida la ejecución en este punto se pueden editar los archivos de plantillas CSHTML, guardar el archivo de la plantilla, cerrar el último PDF creado si está abierto y reanudar ejecución para generar un nuevo PDF con los cambios realizados.
                goto otraVez;

            }

            return creado;

        } // CrearPdf>


        public static bool CrearPdf<D, M>(D documento, DocumentoElectrónico<Factura<Cliente, M>, M> documentoElectrónico, 
            PlantillaCompilada<DatosVenta> plantillaCompilada, OpcionesDocumento opcionesDocumento, out string rutaPdf) 
            where D : Factura<Cliente, M> where M : MovimientoProducto {

            rutaPdf = "";
            opcionesDocumento.MostrarInformaciónAdicional = documento.MostrarInformaciónAdicional;
            var datos = documento switch { 
                Venta v => v.ObtenerDatos(opcionesDocumento), 
                NotaCréditoVenta ncv => ncv.ObtenerDatos(opcionesDocumento),
                _ => throw new Exception(CasoNoConsiderado(typeof(D).Name))
            };
            var html = plantillaCompilada.ObtenerHtml(datos);
            if (ModoDesarrolloPlantillasDocumentos)
                File.WriteAllText(Path.Combine(documentoElectrónico.RutaDocumentosElectrónicosHoy, $"{documento.Código}.html"), html);

            try {

                rutaPdf = Path.Combine(documentoElectrónico.RutaDocumentosElectrónicosHoy, 
                    $"{datos.PrefijoNombreArchivo}{documento.Código}{(opcionesDocumento.ModoImpresión ? "-I" : "")}.pdf");
                using var escritorPdf = new PdfWriter(rutaPdf);
                using var pdf = new PdfDocument(escritorPdf);
                pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LETTER);
                HtmlConverter.ConvertToPdf(html, pdf, OpcionesConversiónPdf);
                return true;

            } catch (Exception ex) when (ex is PdfException || ex is IOException) {

                MostrarError($"No se pudo crear el PDF de la factura electrónica.{DobleLínea}{ex.Message}");
                return false;

            }

        } // CrearPdf>


        /// <summary>
        /// Procedimiento común para las ventas y las notas crédito de ventas que termina de completar el objeto de datos después de ser mapeado.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="opcionesDocumento"></param>
        /// <param name="datosVenta"></param>
        /// <param name="líneas"></param>
        public static void CompletarDatosVenta<M>(OpcionesDocumento opcionesDocumento, DatosVenta datosVenta, List<M> líneas) 
            where M : MovimientoProducto {

            var mapeadorEmpresa = new Mapper(ConfiguraciónMapeadorEmpresa);
            datosVenta.Empresa = mapeadorEmpresa.Map<DatosEmpresa>(Empresa);
            datosVenta.Columnas = ObtenerOpcionesColumnas(datosVenta, líneas);
            datosVenta.LogoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaCarpetaImagenesPlantillas(),
                opcionesDocumento.ModoImpresión ? NombreArchivoLogoEmpresaImpresión : NombreArchivoLogoEmpresa), paraHtml: true);
            datosVenta.CertificadoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaCarpetaImagenesPlantillas(),
                opcionesDocumento.ModoImpresión ? NombreArchivoCertificadoEmpresaImpresión : NombreArchivoCertificadoEmpresa), paraHtml: true);
            datosVenta.TotalPáginas = ObtenerTotalPáginas(datosVenta, líneas);
            datosVenta.ModoImpresión = opcionesDocumento.ModoImpresión;
            datosVenta.MostrarInformaciónAdicional = opcionesDocumento.MostrarInformaciónAdicional;

        } // CompletarDatosVenta>


        public static OpcionesColumnas ObtenerOpcionesColumnas<T, M>(T datosDocumento, List<M> Líneas) 
            where T : DatosDocumento, IConLíneasProductos where M : MovimientoProducto {

            var columnas = new OpcionesColumnas();
            if (datosDocumento.Empresa == null) return columnas;
            columnas.MostrarUnidad = datosDocumento.Empresa.MostrarUnidad;
            columnas.EnlaceWebADetalleProducto = datosDocumento.Empresa.EnlaceWebADetalleProducto;
            columnas.AnchoLista = datosDocumento.AnchoContenido;
            if (datosDocumento.Empresa.DetallarImpuestoSiPorcentajesDiferentes) {
                columnas.MostrarIVAYTotal = Líneas.Select(l => l.PorcentajeEfectivoIVA).Distinct().Count() > 1;
                columnas.MostrarImpuestoConsumoYTotal = Líneas.Select(l => l.PorcentajeEfectivoImpuestoConsumo).Distinct().Count() > 1;
            }
            return columnas;

        } // ObtenerOpcionesColumnas>


        /// <summary>
        /// Obtiene el total de páginas para los documentos en modo <see cref="DatosDocumento.Modo.CuerpoContinuo"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="datosDocumento"></param>
        /// <param name="Líneas"></param>
        /// <returns></returns>
        public static int ObtenerTotalPáginas<T, M>(T datosDocumento, List<M> Líneas) 
            where T : DatosDocumento, IConLíneasProductos where M : MovimientoProducto {

            var altoCabezaYLínea = datosDocumento.AltoCabeza + datosDocumento.MargenVertical
                + (datosDocumento.LíneaBajoCabeza ? datosDocumento.AnchoLíneaBajoCabeza + datosDocumento.SeparaciónLíneaBajoCabeza : 0);

            var factorTamañoLetra = datosDocumento.TamañoLetra / 16; // Algunos valores experimentales se ajustan con este factor para dar aproximadamente los altos con otro tamaño de letra.
            var altoSuperior = datosDocumento switch {
                DatosVenta _ => 133.6 * factorTamañoLetra + datosDocumento.MargenContenidos, // 133.6 es un valor experimental cuando el cliente tiene todos las 5 líneas de datos.
                DatosCotización _ => throw new NotImplementedException(),
                _ => throw new Exception(CasoNoConsiderado(typeof(T).Name))
            };

            double altoLista = 0;
            var anchoDescripción = datosDocumento.Columnas.AnchoDescripción; // Se guarda primero en una variable porque aunque es una propiedad autocalculada muy simple se prefiere no calcularla varias veces en el ciclo.
            var anchoReferencia = datosDocumento.Columnas.AnchoReferencia;
            var acolchadoLínea = datosDocumento.Columnas.RellenoVerticalLíneas * 2;
            using var fuente = new Font(datosDocumento.NombreFuente, datosDocumento.TamañoLetra * Equipo.RelaciónFuentesPdfPantalla); 
            foreach (var línea in Líneas) {

                var altoPorDescripción = ObtenerTamañoTexto(línea.Producto?.Descripción, fuente, anchoDescripción).Height;
                var altoPorReferencia = ObtenerTamañoTexto(línea.Producto?.Referencia, fuente, anchoReferencia).Height;
                altoLista += Math.Max(altoPorDescripción, altoPorReferencia) + acolchadoLínea;

            }
            altoLista += datosDocumento.Columnas.MargenInferiorNombres + ObtenerTamañoTexto("Referencia", fuente, int.MaxValue).Height
                + acolchadoLínea + 12 + datosDocumento.MargenContenidos; // Suma el alto de la fila de nombres de columnas y 12 puntos extra que tiene el elemento lista-tb comparado con al suma directa del alto de sus filas tr.

            var altoObservación = ObtenerTamañoTexto(datosDocumento.Observación, fuente, datosDocumento.AnchoObservación).Height;
            var extraAltoPorObservación = altoObservación < 58 * factorTamañoLetra ? 0 : altoObservación - 58 * factorTamañoLetra; // Si el alto de la observación es 57 (3 líneas), cabe sin problemas. Si es mayor empieza a aumentar el alto la sección inferior. 

            var altoInferior = datosDocumento switch {
                DatosVenta _ => 222 * factorTamañoLetra + datosDocumento.MargenVertical + extraAltoPorObservación, // 222 = alto de infoinf + margen superior de infoinf-línea. Además, suma el margen vertical que le corresponde al div cuerpo.
                DatosCotización _ => throw new NotImplementedException(),
                _ => throw new Exception(CasoNoConsiderado(typeof(T).Name))
            };

            var altoCuerpo = altoSuperior + altoLista + altoInferior;
            var altoPie = datosDocumento.AltoPie + datosDocumento.RellenoSuperiorPie; // Se suma nuevamente el relleno superior porque en el Alto pie no se tiene en cuenta el relleno inferior que es del mismo alto que el superior.
            return (int)Math.Ceiling((altoCabezaYLínea + altoCuerpo + altoPie) / datosDocumento.AltoHoja);

        } // ObtenerTotalPáginas>


        #endregion Métodos y Funciones>


    } // DocumentosGráficos>



} // SimpleOps.DocumentosGráficos>
