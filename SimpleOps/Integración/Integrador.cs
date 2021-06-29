// Copyright Notice:
//
// SimpleOps® is a free ERP software for small businesses and independents.
// Copyright© 2021 Vixark (vixark@outlook.com).
// For more information about SimpleOps®, see https://simpleops.net.
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero
// General Public License as published by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public
// License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not,
// see https://www.gnu.org/licenses.
//
// This License does not grant permission to use the trade names, trademarks, service marks, or product names
// of the Licensor, except as required for reasonable and customary use in describing the origin of the Work
// and reproducing the content of the NOTICE file.
//
// Removing or changing the above text is not allowed.
//

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


        #region Variables Estáticas

        public static HashSet<string> ArchivosProcesados = new HashSet<string>();

        #endregion Variables Estáticas>


        #region Propiedades

        FileSystemWatcher SupervisorArchivos { get; set; }

        /// <summary>
        /// Si es verdadero, cuando un archivo es creado registra su nombre en <see cref="ÚltimoArchivoCreado"/> y si el siguiente evento de cambio
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

            if (!ExisteRuta(TipoElementoRuta.Carpeta, Equipo.RutaIntegración, "integración con programas terceros", out string? mensaje,
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

            if (ArchivosProcesados.Contains(nombre)) return; // Cuando se hace copiar y pegar de un archivo a procesar se genera el evento EnCambioArchivo doble. Es un problema conocido de .Net https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice. Con este hashset se evita que se procese una segunda vez.
            ArchivosProcesados.Add(nombre);

            OperacionesEspecialesDatos = true; // Necesario para evitar el mensaje de error al escribir Cliente.ContactoFacturas.Email en el mapeo inverso hacia el objeto venta.
            try {

                var documentoIntegración = ObtenerEnumeraciónDeTexto<DocumentoIntegración>(nombre[0..3]);
                if (documentoIntegración == DocumentoIntegración.Venta) {

                    var datosVenta = Deserializar<DatosVenta>(File.ReadAllText(ruta), Serialización.EnumeraciónEnTexto);
                    var mapeador = new Mapper(ConfiguraciónMapeadorVentaIntegraciónInverso);
                    var venta = mapeador.Map<Venta>(datosVenta); // Se debe tener en cuenta que los productos base cargados desde el DTO datosVenta son diferentes objetos para cada producto, así originalmente sean el mismo. Esto significa que, por ejemplo, el cambio en la descripción de un producto base no afectaría los otros productos que tienen ese producto base. Si se necesitara este comportamiento habría que hacer un procesamiento posterior para unificarlos en un solo objeto.
                    foreach (var l in venta.Líneas) {
                        l.Venta = venta; // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                        if (l.Producto?.TieneBase == false) l.Producto.Base = null; // Necesario porque el Automaper siempre crea el objeto Base.
                    }
                    venta.ConsecutivoDianAnual = venta.Número - Empresa.PrimerNúmeroFacturaAutorizada + 1;
                    ValidarCliente(venta.Cliente, validarDepartamento: true);
                    ProcesarDocumentoCliente(venta, ruta, "factura");

                } else if (documentoIntegración == DocumentoIntegración.NotaCrédito) {

                    var datosNotaCrédito = Deserializar<DatosVenta>(File.ReadAllText(ruta), Serialización.EnumeraciónEnTexto);
                    var mapeador = new Mapper(ConfiguraciónMapeadorNotaCréditoVentaIntegraciónInverso);
                    var notaCréditoVenta = mapeador.Map<NotaCréditoVenta>(datosNotaCrédito);
                    foreach (var l in notaCréditoVenta.Líneas) {
                        l.NotaCréditoVenta = notaCréditoVenta; // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                        if (l.Producto?.TieneBase == false) l.Producto.Base = null; // Necesario porque el Automaper siempre crea el objeto Base.
                    }
                    notaCréditoVenta.ConsecutivoDianAnual = notaCréditoVenta.Número;          
                    ValidarCliente(notaCréditoVenta.Cliente, validarDepartamento: true);
                    ProcesarDocumentoCliente(notaCréditoVenta, ruta, "nota crédito");

                } else if (documentoIntegración == DocumentoIntegración.Catálogo) {

                    var datosCotización = Deserializar<DatosCotización>(File.ReadAllText(ruta), Serialización.EnumeraciónEnTexto);
                    if (datosCotización == null) throw new Exception("El objeto datosCotización está vacío.");
                    var mapeador = new Mapper(ConfiguraciónMapeadorCotizaciónIntegraciónInverso);
                    var cotización = mapeador.Map<Cotización>(datosCotización);
                    cotización.EstablecerTipo(TipoCotización.Catálogo); // Siempre se debe establecer el tipo después del mapeo para agregue los valores predeterminados de algunas propiedades si no fueron mapeadas y quedaron nulas.
                    foreach (var l in cotización.Líneas) {
                        l.Cotización = cotización; // Necesario porque después de ser leídas por el Automapper no quedan automáticamente enlazadas.
                        if (l.Producto?.TieneBase == false) l.Producto.Base = null; // Necesario porque el Automaper siempre crea el objeto Base.
                    }
                    ValidarCliente(cotización.Cliente, validarDepartamento: false);
                    if (CrearPdfCatálogo(cotización, out string rutaPdf)) {
                        File.WriteAllText(ObtenerRutaCambiandoExtensión(ruta, "ok"), $"{rutaPdf}");
                    } else {
                        throw new Exception("No se pudo crear el PDF del catálogo.");
                    }

                } else {
                    throw new Exception(CasoNoConsiderado(documentoIntegración));
                }

            #pragma warning disable CA1031 // No capture tipos de excepción generales. Se acepta porque el error se reporta mediante el archivo .error a la aplicación tercera.
            } catch (Exception ex) {
            #pragma warning restore CA1031

                try {
                    File.WriteAllText(ObtenerRutaCambiandoExtensión(ruta, "error"), ex.Message);
                } catch (Exception ex2) {
                    MostrarError($"Hubo un error en el procedimiento de integración de SimpleOps con el programa tercero y no se pudo copiar el archivo de error. " +
                                 $"Reinice {NombreAplicación} y su aplicación tercera.{DobleLínea}{ex.Message}{DobleLínea}{ex2.Message}");
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


        /// <summary>
        /// Valida los datos del cliente según lo requiera cada tipo de documento.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="validarDepartamento"></param>
        public static void ValidarCliente(Cliente? cliente, bool validarDepartamento) {
            if (validarDepartamento && cliente?.Municipio?.CódigoDepartamento == CódigoDepartamentoNulo)
                throw new Exception("El departamento es incorrecto.");
        } // ValidarCliente>


        #endregion Métodos y Funciones>


    } // Integrador>



} // SimpleOps.Integración>
