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
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SimpleOps.Interfaz;
using System.Windows;
using System.IO;
using System.Text.Json.Serialization;



namespace SimpleOps.Singleton {



    /// <summary>
    /// Opciones, datos y configuraciones propias del equipo donde está instalado SimpleOps. Son personalizaciones de apariencia o comportamiento que no 
    /// tienen que ser comunes a todos los equipos en la misma empresa. Inician con unos valores predeterminados recomendados pero cada
    /// equipo los puede cambiar sin afectar el funcionamiento en otros equipos. Un equipo puede ser usado por diferentes usuarios, aunque lo más usual es que
    /// lo use solo uno. Los valores iniciales en código solo sirven para autogenerar el archivo Equipo.json cuando no exista y para evitar tener que 
    /// declarar las propiedades permitiendo valores nulos.
    /// </summary>
    sealed class OpcionesEquipo { // No cambiar los nombres de las propiedades porque estos se usan en los archivos de opciones JSON. No debe tener métodos ni propiedades autocalculadas (estos van en Global).


        #region Patrón Singleton
        // Ver https://csharpindepth.com/Articles/Singleton.

        private static readonly Lazy<OpcionesEquipo> DatosLazy = new Lazy<OpcionesEquipo>(() => new OpcionesEquipo());

        public static OpcionesEquipo Datos { get { return DatosLazy.Value; } } // Normalmente esta sería la variable que se accede pero se prefiere hacer una variable auxiliar Equipo en Global.cs para tener un acceso más fácil sin necesidad de escribir OpcionesEquipo.Datos.

        private OpcionesEquipo() { }

        #endregion Patrón Singleton>


        #region Comportamiento

        public bool MostrarMensajeErrorFirmador { get; set; } = true;

        #endregion Comportamiento>


        #region Apariencia


        public Dictionary<EstadoOrdenCompra, BrochaInformativa> BrochasEstadosOrdenesCompra { get; set; }
            = new Dictionary<EstadoOrdenCompra, BrochaInformativa> {
                { EstadoOrdenCompra.Lista, BrochaInformativa.Éxito },
                { EstadoOrdenCompra.EsperandoProducto, BrochaInformativa.Indiferente },
                { EstadoOrdenCompra.EsperandoPago, BrochaInformativa.Información },
                { EstadoOrdenCompra.CupoCréditoAlcanzado, BrochaInformativa.Alerta },
                { EstadoOrdenCompra.BajoMonto, BrochaInformativa.Alerta },
                { EstadoOrdenCompra.FacturasVencidas, BrochaInformativa.Alerta },
                { EstadoOrdenCompra.PendientePedido, BrochaInformativa.Peligro },
            };


        #endregion Apariencia>


        #region Rutas

        public string RutaAplicación { get; set; } = Configuración.RutaAplicación; // Se actualiza en Rutas para permitir que los usuarios del código cambien este valor sin que sus cambios sean reemplazados con una nueva versión del código de OpcionesEquipo.cs.
        
        public string? RutaIntegración { get; set; } // Ruta de la carpeta dentro de un programa tercero usada para almacenar archivos de comunicación entre SimpleOps y este programa tercero.

        public string? RutaProductos { get; set; } // Ruta de la carpeta donde se almacenarán las imágenes y los archivos de información de los productos. Si se deja en nulo, se almacenan en [RutaAplicación]/[CarpetaProductos]. Se permite personalizar la ruta para permitir establecer esta ruta en una carpeta compartida a la que puedan acceder todos los equipos de la empresa. Esta carpeta compartida puede habilitarse con OneDrive, Dropbox y servicios simlares o cómo una carpeta compartida de red.

        public string? RutaPlantillasDocumentos { get; set; } // Ruta de la carpeta donde se almacenarán las plantillas de los documentos. Si se deja en nulo, se almacenan en [RutaAplicación]/[PlantillasDocumentos]. Se permite personalizar la ruta para permitir establecer esta ruta en una carpeta compartida a la que puedan acceder todos los equipos de la empresa. Esta carpeta compartida puede habilitarse con OneDrive, Dropbox y servicios simlares o cómo una carpeta compartida de red.

        #endregion Rutas>


        #region Facturación Electrónica
        // No todos los usuarios tendrían acceso a la posibilidad de facturar electrónicamente, ni a la clave del certificado. Se establecen estas rutas como personalización de cada equipo/usuario.

        public string? RutaCertificado { get; set; } // Ruta del archivo PFX con el certificado de firma digital. No se autocalcula con RutaAplicación porque es posible que este archivo que es delicado se necesite guardar en otra ubicación. 

        public string? RutaClaveCertificado { get; set; } // Ruta del archivo TXT con una sola línea sin ningún espacio al frente ni atrás que contiene la clave del certificado de firma digital. No se autocalcula con RutaAplicación porque es posible que este archivo que es delicado se necesite guardar en otra ubicación.

        public float RelaciónFuentesPdfPantalla { get; set; } = 0.7422F; // 0.5938 = 9.5 / 16 para pantallas grandes con 125% de escala en Windows. 0.7422 para pantallas medianas con escala 100% en Windows. Es un factor de conversión aproximado entre el tamaño de la fuente en puntos de HTML y el tamaño de la letra usando medidas la librería System.Drawing. Es necesario para poder calcular el alto de la lista de productos y la cantidad de páginas de los documentos gráficos.


        public string? _ClaveCertificado;
        [JsonIgnore] 
        public string? ClaveCertificado { // La clave del certificado no se debe guardar en otro lugar que el usuario no conozca por su seguridad. Esta propiedad permite que se pida la clave del certificado cada vez que se vaya a iniciar la facturación o tomarla de RutaClaveCertificado.

            get {

                return SiNulo(ref _ClaveCertificado, () => {

                    if (File.Exists(RutaClaveCertificado)) {

                        ClaveCertificado = File.ReadAllText(RutaClaveCertificado);
                        return true;

                    } else {

                        if (!ExisteRuta(TipoElementoRuta.Archivo, RutaCertificado, "certificado de firma digital", out string? mensaje, "No se podrá facturar electrónicamente")) {

                            MostrarError(mensaje);
                            return false; // No hay certificado entonces tampoco pide la clave.  
                            
                        } else {

                            otraVez:
                            var clave =
                                CuadroDiálogoContraseña.Mostrar($"Ingresa la clave del certificado de facturación electrónica.{DobleLínea}Si no deseas " +
                                                                "ingresarla cada vez que inicie la aplicación la puedes guardar en un archivo de texto (.txt) " +
                                                                "y seleccionarlo en 'Opciones > Facturación Electrónica > Ruta Clave Certificado'.",
                                                                "Clave Certificado Firma Electrónica");

                            if (string.IsNullOrEmpty(clave)) {
                                MostrarError($"No se ingresó una clave del certificado de facturación electrónica.{DobleLínea}No se podrá facturar " +
                                                   $"electrónicamente.");
                                return false;
                            }

                            try {
                                using var certificado = new X509Certificate2(RutaCertificado, clave); // Verifica que la clave sea correcta.
                            } catch (CryptographicException) {

                                MostrarError($"La clave ingresada para el certificado {RutaCertificado} es incorrecta.{DobleLínea}Intenta nuevamente.");
                                goto otraVez;

                            } catch (Exception) {
                                throw;
                            }

                            ClaveCertificado = clave;
                            return true;

                        }

                    }

                }); // SiNulo>

            } // get>

            set => _ClaveCertificado = value;

        } // ClaveCertificado>


        #endregion Facturacion Electrónica>


    } // OpcionesEquipo>



} // SimpleOps.Singleton>
