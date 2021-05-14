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
        public static PlantillaCompilada<T> CompilarPlantilla<T>(RazorEngine motorRazor, string cuerpo, IDictionary<string, string> partes) 
            where T : class {

            static void configurarOpciones(IRazorEngineCompilationOptionsBuilder opciones) 
                => opciones.AddAssemblyReferenceByName("System.Collections"); // Se intentó agregar para habilitar System.Math opciones.AddAssemblyReferenceByName("System.Runtime.Extensions"); , pero parece que es necesario hacer otros pasos https://stackoverflow.com/questions/19736600/the-name-math-does-not-exist-in-the-current-context. Por el momento mejor se prefiere evitar el uso de System.Math para no agregar ensamblados solo para algo que se puede hacer desde la clase Datos[]. Si algun objeto referenciado en estos ensamblados no compila, puede ser necesario agregar la directiva @using así el compilador no la exija. Por ejemplo, se usa @using System.Collections.Generic para poder usar List<string> en el código sin que saque error. O se usa @using SimpleOps.DocumentosGráficos para poder llamar a funciones en la clase DatosDocumento.
            return new PlantillaCompilada<T>(motorRazor.Compile<PlantillaBase<T>>(cuerpo, o => configurarOpciones(o)),
                   partes.ToDictionary(k => k.Key, v => motorRazor.Compile<PlantillaBase<T>>(v.Value, o => configurarOpciones(o))));

        } // Compilar>


        private static bool CrearPdf<T>(out string rutaPdf, Func<RazorEngine, PlantillaCompilada<T>> crearPlantillaCompilada, 
            Func<PlantillaCompilada<T>, (bool, string)> crearPdf) where T : class { // Una vez compilada la plantilla la primera vez este procedimiento es relativamente rápido en alrededor de 200ms, 150ms de estos son en la generación del HTML que se podrían reducir si se almacenaran los archivos CSHTML en memoria o directamente la plantilla compilada según https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout, pero por el momento se prefiere evitar esto para no complicar más el procedimiento y facilitar hacer cambios a los archivos CSHTML al permitir que estos cambios se hagan efectivos sin necesidad de reiniciar la aplicación.

            otraVez:
            var creado = false;
            rutaPdf = "";
            
            var motorRazor = new RazorEngine();
            PlantillaCompilada<T>? plantillaCompilada = null;

            try {

                plantillaCompilada = crearPlantillaCompilada(motorRazor);

            } catch (RazorEngineCompilationException ex) {

                MostrarError($"No se pudo cargar la plantilla HTML necesaria para construir el PDF del documento gráfico.{DobleLínea}{ex.Message}");
                creado = false;

            }

            if (plantillaCompilada != null) (creado, rutaPdf) = crearPdf(plantillaCompilada);

            if (ModoDesarrolloPlantillas) {

                if (creado) AbrirArchivo(rutaPdf);
                CopiarPlantillasARutaAplicación(sobreescribir: true); // En el modo desarrollo se intentará mantener sincronizadas las carpetas de plantillas en producción con la última versión de las plantillas en las carpetas de desarrollo. Justo antes del punto de interrupción a continuación es un buen lugar para hacerlo porque después de este, el usuario del código puede suspender la ejecución. Esto reduce el rendimiento, pero es algo que solo se presenta en el modo desarrollo. No es completamente necesario actualizar las plantillas para que funcione en modo desarrollo porque estas son leídas directamente de las carpetas de plantillas de desarrollo, pero si es conveniente para mantener solo una versión de las plantillas en el computador y evitar confusiones.
                SuspenderEjecuciónEnModoDesarrollo(); // Al estar detenida la ejecución en este punto se pueden editar los archivos de plantillas CSHTML, guardar el archivo de la plantilla, cerrar el último PDF creado si está abierto (sin cerrar el Foxit Reader, dejando otro PDF cualquier abierto, para evitar error de Foxit Reader cuando se abre desde el código) y reanudar ejecución para generar un nuevo PDF con los cambios realizados.
                goto otraVez;

            }

            return creado;

        } // CrearPdf>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cotización"></param>
        /// <param name="rutaPdf"></param>
        /// <returns></returns>
        public static bool CrearPdfCatálogo(Cotización cotización, out string rutaPdf) {

            if (cotización.Tipo != TipoCotización.Catálogo) 
                throw new Exception("No se esperaba que la cotización no fuera de tipo Catálogo en CrearPdfCatálogo()");

            var cantidadPáginasExtra = (int)Math.Ceiling((double)cotización.ReferenciasProductosPáginasExtra.Count / 
                ((cotización.CantidadFilasProductos ?? 1) * (cotización.CantidadColumnasProductos ?? 1))); 
            var rutasPáginasPlantilla = ObtenerRutasPáginasPlantilla(PlantillaDocumento.CatálogoPdf, omitirPrimera: true, 
                cantidadPáginasExtra: cantidadPáginasExtra);
            var cantidadPáginasConPlantillaPropia = rutasPáginasPlantilla.Count - cantidadPáginasExtra;
            var índiceInversoInserciónPáginasExtra = cotización.ÍndiceInversoInserciónPáginasExtra ?? 0;
            var númeroInserciónPáginasExtra = cantidadPáginasConPlantillaPropia - índiceInversoInserciónPáginasExtra + 1;
            var desfaceNúmeroPáginaExtra = cantidadPáginasConPlantillaPropia == 0 ? 1 : 0; // Las páginas del catálogo inician en la página 2, la 1 siempre es omitida porque se considera que es el marco. Por esto, cuando no hay páginas con plantilla propia se debe establecer un desface a las páginas extra de 1 para que inicien en 2.

            return CrearPdf(out rutaPdf, motorRazor => {

                var cuerpo = File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.CatálogoPdf));
                var partes = new Dictionary<string, string> { {"Marco", File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.MarcoPdf))} };
              
                foreach (var kv in rutasPáginasPlantilla) {

                    if (EsPáginaExtra(kv.Value)) {
                        partes.Add($"Página{kv.Key - índiceInversoInserciónPáginasExtra + desfaceNúmeroPáginaExtra}", // El número de la página extra insertada se reduce por el índice inverso de inserción porque entre más alto este número más hacia el inicio del documento quedarán las páginas extra.
                            File.ReadAllText(kv.Value).Reemplazar("36923463", $"{kv.Key - cantidadPáginasConPlantillaPropia - 1}")); // Reemplazar directamente en el texto leído el número de la página en la plantilla CatálogoPdfExtra.cshtml (36923463) por el número correcto, no es la solución más elegante, pero funciona razonablemente bien para poder lograr diferenciar en que número de página de CatálogoPdfExtra se encuentra y poder mostrar los productos extra apropiados.
                    } else {
                        if (kv.Key <= númeroInserciónPáginasExtra) partes.Add($"Página{kv.Key}", File.ReadAllText(kv.Value));
                    } 

                }

                foreach (var kv in rutasPáginasPlantilla) {
                    if (kv.Key > númeroInserciónPáginasExtra && !EsPáginaExtra(kv.Value)) // Inserta las páginas posteriores a la página extra. Normalmente solo es la contraportada cuando índiceInversoInserciónPáginasExtra = 1.
                        partes.Add($"Página{kv.Key + cantidadPáginasExtra}", File.ReadAllText(kv.Value)); // El número de las páginas posterior a la inserción de las páginas extra se incrementa en la cantidad de páginas extra insertadas.
                }

                return CompilarPlantilla<DatosCotización>(motorRazor, cuerpo, partes);

            }, plantillaCompilada => {

                var datos = cotización.ObtenerDatos(new OpcionesDocumento(), PlantillaDocumento.CatálogoPdf, rutasPáginasPlantilla.Count + 1); // Se suma uno para obtener las páginas porque en el diccionario rutasPáginasPlantilla se está omitiendo la primera página.
                datos.TítuloPáginasExtra = cantidadPáginasConPlantillaPropia == 0 ? "Productos" : "Otros Productos";
                var creado = CrearPdf(datos, plantillaCompilada, ObtenerRutaCotizacionesDeHoy(), out string rutaPdfAux); 
                return (creado, rutaPdfAux);

            });

        } // CrearPdfCatálogo>


        public static bool CrearPdfVenta<D, M>(D documento, DocumentoElectrónico<Factura<Cliente, M>, M>? documentoElectrónico, out string rutaPdf)
            where D : Factura<Cliente, M> where M : MovimientoProducto {

            rutaPdf = "";
            if (documentoElectrónico == null) return false;

            return CrearPdf(out rutaPdf, motorRazor => {

                var cuerpo = File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.VentaPdf));
                var partes = new Dictionary<string, string> {
                    {"Marco", File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.MarcoPdf))},
                    {"Lista", File.ReadAllText(ObtenerRutaPlantilla(PlantillaDocumento.ListaProductosPdf))},
                };
                return CompilarPlantilla<DatosVenta>(motorRazor, cuerpo, partes);

            }, plantillaCompilada => {

                bool crearPdf(bool modoImpresión, out string rutaPdfAux) {

                    var opciones = new OpcionesDocumento {
                        ModoImpresión = modoImpresión, MostrarInformaciónAdicional = documento.MostrarInformaciónAdicional
                    };
                    var datos = documento switch {
                        Venta v => v.ObtenerDatos(opciones),
                        NotaCréditoVenta ncv => ncv.ObtenerDatos(opciones),
                        _ => throw new Exception(CasoNoConsiderado(typeof(D).Name))
                    };
                    return CrearPdf(datos, plantillaCompilada, documentoElectrónico.RutaDocumentosElectrónicosHoy, out rutaPdfAux,
                        modoImpresión: modoImpresión);

                }

                var creado = crearPdf(modoImpresión: false, out string rutaPdfAux);
                if (creado && Empresa.GenerarPDFsAdicionalesImpresión) crearPdf(modoImpresión: true, out string _);
                return (creado, rutaPdfAux);

            });

        } // CrearPdfVenta>


        private static bool CrearPdf<D>(D datos, PlantillaCompilada<D> plantillaCompilada, string rutaCarpetaPdf, out string rutaPdf, 
            bool modoImpresión = false) where D : DatosDocumento {

            rutaPdf = "";
            var html = plantillaCompilada.ObtenerHtml(datos);
            if (ModoDesarrolloPlantillas) File.WriteAllText(Path.Combine(rutaCarpetaPdf, $"{datos.NombreArchivo}.html"), html); // Se escribe en HTML para facilitar su revisión desde un explorador aunque pueda diferir un poco del diseño final en PDF.

            try {

                rutaPdf = Path.Combine(rutaCarpetaPdf, $"{datos.NombreArchivo}{(modoImpresión ? "-I" : "")}.pdf");
                using var escritorPdf = new PdfWriter(rutaPdf);
                using var pdf = new PdfDocument(escritorPdf);
                pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.LETTER);
                HtmlConverter.ConvertToPdf(html, pdf, OpcionesConversiónPdf);
                return true;

            } catch (Exception ex) when (ex is PdfException || ex is IOException) {

                MostrarError($"No se pudo crear el archivo PDF.{DobleLínea}{ex.Message}");
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
            datosVenta.LogoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(),
                opcionesDocumento.ModoImpresión ? ArchivoLogoEmpresaImpresión : ArchivoLogoEmpresa), paraHtml: true);
            datosVenta.CertificadoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(),
                opcionesDocumento.ModoImpresión ? ArchivoCertificadoEmpresaImpresión : ArchivoCertificadoEmpresa), paraHtml: true);
            datosVenta.TotalPáginas = ObtenerTotalPáginas(datosVenta, líneas);
            datosVenta.ModoImpresión = opcionesDocumento.ModoImpresión;
            datosVenta.MostrarInformaciónAdicional = opcionesDocumento.MostrarInformaciónAdicional;

        } // CompletarDatosVenta>


        private static OpcionesColumnas ObtenerOpcionesColumnas<T, M>(T datosDocumento, List<M> Líneas) 
            where T : DatosDocumento, IConLíneasProductos where M : MovimientoProducto {

            var columnas = ObtenerOpcionesColumnas(datosDocumento);
            if (datosDocumento.Empresa == null) return columnas;
            if (datosDocumento.Empresa.DetallarImpuestoSiPorcentajesDiferentes) {
                columnas.MostrarIVAYTotal = Líneas.Select(l => l.PorcentajeEfectivoIVA).Distinct().Count() > 1;
                columnas.MostrarImpuestoConsumoYTotal = Líneas.Select(l => l.PorcentajeEfectivoImpuestoConsumo).Distinct().Count() > 1;
            }
            return columnas;

        } // ObtenerOpcionesColumnas>


        public static OpcionesColumnas ObtenerOpcionesColumnas<T>(T datosDocumento) where T : DatosDocumento {

            var columnas = new OpcionesColumnas();
            if (datosDocumento.Empresa == null) return columnas;
            columnas.MostrarUnidad = datosDocumento.Empresa.MostrarUnidad;
            columnas.EnlaceWebADetalleProducto = datosDocumento.Empresa.EnlaceWebADetalleProducto;
            columnas.AnchoLista = datosDocumento.AnchoContenido;
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
        public static int ObtenerTotalPáginas<T, M>(T datosDocumento, List<M> Líneas) where T : DatosDocumento, IConLíneasProductos 
            where M : MovimientoProducto {

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


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
        public static int ObtenerTamañoPredeterminadoImágenesProductos(PlantillaDocumento plantillaDocumento) =>
            plantillaDocumento switch {
                PlantillaDocumento.VentaPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ProformaPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaCréditoPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaDébitoPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CotizaciónPdf => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.PedidoPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ComprobanteEgresoPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CobroPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.RemisiónPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CatálogoPdf => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.MarcoPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.FichaInformativaPdf => Empresa.TamañoImágenesProductosFichas,
                PlantillaDocumento.VentaEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ProformaEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaCréditoEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaDébitoEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CotizaciónEmail => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.PedidoEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ComprobanteEgresoEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CobroEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.RemisiónEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CatálogoEmail => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.MarcoEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.FichaInformativaEmail => Empresa.TamañoImágenesProductosFichas,
                PlantillaDocumento.VentaWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ProformaWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaCréditoWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.NotaDébitoWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CotizaciónWeb => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.PedidoWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ComprobanteEgresoWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CobroWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.RemisiónWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.CatálogoWeb => Empresa.TamañoImágenesProductosCotizaciones,
                PlantillaDocumento.MarcoWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.FichaInformativaWeb => Empresa.TamañoImágenesProductosFichas,
                PlantillaDocumento.ListaProductosPdf => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ListaProductosWeb => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
                PlantillaDocumento.ListaProductosEmail => throw new Exception($"No se ha especificado un tamaño de imagen para {plantillaDocumento.ATexto()}"),
            };
        #pragma warning restore CS8524


        #endregion Métodos y Funciones>


    } // DocumentosGráficos>



} // SimpleOps.DocumentosGráficos>
