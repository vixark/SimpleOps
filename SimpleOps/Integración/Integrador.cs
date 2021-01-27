using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using AutoMapper;
using SimpleOps.Modelo;
using static SimpleOps.Legal.Dian;
using SimpleOps.Legal;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.Integración {



    /// <summary>
    /// Clase para integrar mediante comunicación por archivos planos varias funcionalidades de SimpleOps con otros programas
    /// terceros incompatibles, principalmente la funcionalidad de facturación electrónica. Se deben tener abiertos ambos programas
    /// a la vez y se debe disponer del código del programa tercero para realizar los cambios necesarios para la creación de los archivos 
    /// de comunicación y el procesamiento de la respuesta de SimpleOps.
    /// </summary>
    class Integrador : IDisposable {


        #region Propiedades

        FileSystemWatcher SupervisorArchivos { get; set; }

        /// <summary>
        /// Si es verdadero cuando un archivo es creado registra su nombre en <see cref="ÚltimoArchivoCreado"/> y si el siguiente evento de cambio
        /// es sobre el mismo archivo se procesa el procedimiento <see cref="ProcesarNuevoArchivo(string, string)"/>. Esto es necesario porque
        /// en algunos casos el programa tercero genera el evento de creación de archivo y aún lo mantiene abierto porque no ha terminado
        /// de escribirlo, lo que genera una excepción en File.ReadAllText porque el archivo aún está en uso. Al hacer el procesamiento en el 
        /// evento de cambio se evita este error. Este modo puede tener problemas cuando se crean archivos muy frecuentemente por problemas 
        /// de concurrencia que harían que se pierdan. Si se usa para creaciones esporádicas cada algunos segundos no debería presentar problemas.<br/><br/>
        /// Si es falso se procesa el archivo directamente en el evento de creación.
        /// </summary>
        public bool ModoProcesarNuevoArchivoEnCambioPosterior { get; set; } = true;

        /// <summary>
        /// Debido a que en el evento Created sale error en el método ReadAllText porque es posible que aún no se ha cerrado.
        /// </summary>
        public string? ÚltimoArchivoCreado {get; set; }

        public bool Iniciado { get; set; } = false;

        #endregion Propiedades>


        #region Constructores


        public Integrador() {

            if (!Existe(TipoRuta.Carpeta, Equipo.RutaIntegración, "integración con programas terceros", out string? mensaje,
                "No funcionará la facturación electrónica ni la generación de catálogos desde programas terceros")) {

                MostrarError(mensaje);
                SupervisorArchivos = new FileSystemWatcher();
                Iniciado = false;

            } else {

                SupervisorArchivos = new FileSystemWatcher { Path = Equipo.RutaIntegración, Filter = "*.json", NotifyFilter = NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
                };
                SupervisorArchivos.Changed += EnCambioArchivo;
                SupervisorArchivos.Created += EnCreaciónArchivo;
                SupervisorArchivos.Deleted += EnEliminaciónArchivo;
                SupervisorArchivos.Renamed += EnRenombradoArchivo;
                SupervisorArchivos.EnableRaisingEvents = true;
                Iniciado = true;

            }

        } // Integrador>


        #endregion Constructores>


        #region Métodos y Funciones


        private void EnCambioArchivo(object source, FileSystemEventArgs e) {
            if (ModoProcesarNuevoArchivoEnCambioPosterior && ÚltimoArchivoCreado == e.Name) ProcesarNuevoArchivo(e.Name, e.FullPath);
        } // EnCambioArchivo>


        private void EnCreaciónArchivo(object source, FileSystemEventArgs e) {

            if (ModoProcesarNuevoArchivoEnCambioPosterior) {
                ÚltimoArchivoCreado = e.Name;
            } else {
                ProcesarNuevoArchivo(e.Name, e.FullPath);
            }

        } // EnCreaciónArchivo>


        private void EnEliminaciónArchivo(object source, FileSystemEventArgs e) { }

        private void EnRenombradoArchivo(object source, RenamedEventArgs e) { }

        public void Dispose() => SupervisorArchivos?.Dispose();


        public static void ProcesarNuevoArchivo(string nombre, string ruta) {

            OperacionesEspecialesDatos = true; // Necesario para evitar el mensaje de error al escribir Cliente.ContactoFacturas.Email en el mapeo inverso hacia el objeto venta.
            try {

                var documentoIntegración = ObtenerEnumeraciónDeTexto<DocumentoIntegración>(nombre[0..3]);
                if (documentoIntegración == DocumentoIntegración.Venta) {

                    var datosVenta = Deserializar<DatosVenta>(File.ReadAllText(ruta), Serialización.EnumeraciónEnTexto);
                    var mapeador = new Mapper(ConfiguraciónMapeadorVentaIntegraciónInverso);
                    var venta = mapeador.Map<Venta>(datosVenta);
                    venta.Líneas.ForEach(lv => lv.Venta = venta); // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                    if (venta.ConsecutivoDianAnual == null) venta.ConsecutivoDianAnual = venta.Número - Empresa.PrimerNúmeroFacturaAutorizada + 1;
                    ValidarCliente(venta.Cliente);
                    ProcesarDocumentoCliente(venta, ruta, "factura");

                } else if (documentoIntegración == DocumentoIntegración.NotaCrédito) {

                    var datosNotaCrédito = Deserializar<DatosVenta>(File.ReadAllText(ruta), Serialización.EnumeraciónEnTexto);
                    var mapeador = new Mapper(ConfiguraciónMapeadorNotaCréditoVentaIntegraciónInverso);
                    var notaCréditoVenta = mapeador.Map<NotaCréditoVenta>(datosNotaCrédito);
                    notaCréditoVenta.Líneas.ForEach(lv => lv.NotaCréditoVenta = notaCréditoVenta); // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                    ValidarCliente(notaCréditoVenta.Cliente);
                    ProcesarDocumentoCliente(notaCréditoVenta, ruta, "nota crédito");

                } else {
                    throw new Exception(CasoNoConsiderado(documentoIntegración));
                }

            #pragma warning disable CA1031 // No capture tipos de excepción generales. Se acepta porque el error se reporta mediante el archivo .error a la aplicación tercera.
            } catch (Exception ex) {
            #pragma warning restore CA1031

                try {
                    File.WriteAllText(ObtenerRutaCambiandoExtensión(ruta, "error"), ex.Message);
                } catch (Exception ex2) {
                    MostrarError($"Hubo un error en el procedimiento de facturación electrónica y no se pudo copiar el archivo de error. " +
                                 $"Reinice {NombreAplicación} y su aplicación de facturación.{DobleLínea}{ex.Message}{DobleLínea}{ex2.Message}");
                    throw;
                }

            } finally {
                OperacionesEspecialesDatos = false;
            }

        } // ProcesarNuevoArchivo>


        public static void ProcesarDocumentoCliente<M>(Factura<Cliente, M> documentoCliente, string ruta, string nombre) where M : MovimientoProducto {

            if (CrearYEnviarDocumentoElectrónico(documentoCliente, out string? mensaje,
                out DocumentoElectrónico<Factura<Cliente, M>, M>? documentoElectrónico, pruebaHabilitación: false)) {

                var rutaDocumento = documentoElectrónico?.Ruta;
                if (!File.Exists(rutaDocumento)) throw new Exception($"No se pudo encontrar el archivo XML de la {nombre} electrónica {rutaDocumento}.");

                if (documentoCliente != null && CrearPdfVenta(documentoCliente, documentoElectrónico, out string rutaPdf)) {

                    if (!File.Exists(rutaPdf)) 
                        throw new Exception($"No se pudo encontrar el PDF con la representación gráfica de la {nombre} electrónica {rutaPdf}.");

                    File.WriteAllText(ObtenerRutaCambiandoExtensión(ruta, "ok"), $"{rutaDocumento}{NuevaLínea}{rutaPdf}");

                } else {
                    throw new Exception($"No se pudo crear el PDF con la representación gráfica de la {nombre} electrónica {rutaDocumento}.");
                }

            } else {
                throw new Exception(mensaje);
            }

        } // ProcesarDocumentoCliente>


        public static void ValidarCliente(Cliente? cliente) {
            if (cliente?.Municipio?.CódigoDepartamento == CódigoDepartamentoNulo) throw new Exception("El departamento es incorrecto.");
        } // ValidarCliente>


        #endregion Métodos y Funciones>


    } // Integrador>



} // SimpleOps.Integración>
