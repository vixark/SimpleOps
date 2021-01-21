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
        // Tomado de https://csharpindepth.com/Articles/Singleton.

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

        public string RutaAplicación { get; set; } = @"D:\Programas\SimpleOps"; // En el computador de producción está en D:\Programas\SimpleOps. En el computador de desarrollo en casa está en D:\Programas\SimpleOps. En el computador de desarrollo de la empresa está en E:\SimpleOps. Esta ruta debe ser la misma que está almacenada en '[RutaAplicación]\Opciones\Equipo.json', se necesita escribir aquí principalmente para poder ubicar estos archivos de opciones.

        #endregion Rutas>


        #region Facturación Electrónica
        // No todos los usuarios tendrían acceso a la posibilidad de facturar electrónicamente, ni a la clave del certificado. Se establecen estas rutas como personalización de cada equipo/usuario.

        public string RutaCertificado { get; set; } = @"D:\Programas\SimpleOpsxxxxxxxxxxx\Firma Electrónica\Certificado.pfx"; // Archivo .pfx con el certificado para la firma digital.

        public string RutaClaveCertificado { get; set; } = @"D:\Programas\SimpleOps\Firma Electrónica\Clave.txt"; // Un archivo .txt con una sola línea y en ella la clave del certificado sin ningún espacio al frente ni atrás.

        public float RelaciónFuentesPdfPantalla { get; set; } = 0.59375F; // 0,59375 = 9.5 / 16 para pantallas grandes con 125% de zoom en Windows. 0.740625 = 11.85 / 16 para pantallas medianas con zoom 100% en Windows. Es un factor de conversión aproximado entre el tamaño de la fuente en puntos de HTML y el tamaño de la letra usando medidas la librería System.Drawing. Es necesario para poder calcular el alto de la lista de productos y la cantidad de páginas de los documentos gráficos.

        public string RutaIntegración { get; set; } = @"D:\Programas\SimpleOps\Programa Tercero\Integración SimpleOps"; // Carpeta dentro de un programa tercero usada para almacenar archivos de comunicación entre SimpleOps y este programa tercero.


        public string? _ClaveCertificado;
        [JsonIgnore] // La clave del certificado no se debe guardar en otro lugar que el usuario no conozca por su seguridad.
        public string? ClaveCertificado {

            get {

                return SiNulo(ref _ClaveCertificado, () => {

                    if (File.Exists(RutaClaveCertificado)) {

                        ClaveCertificado = File.ReadAllText(RutaClaveCertificado);
                        return true;

                    } else {

                        if (File.Exists(RutaCertificado)) {

                            otraVez:
                            var clave =
                                CuadroDiálogoContraseña.Mostrar($"Ingresa la clave del certificado de facturación electrónica.{DobleLínea}Si no deseas " +
                                                                "ingresarla cada vez que inicie la aplicación la puedes guardar en un archivo de texto (.txt) " +
                                                                "y seleccionarlo en 'Opciones > Facturación Electrónica > Ruta Clave Certificado'.",
                                                                "Clave Certificado Firma Electrónica");

                            if (string.IsNullOrEmpty(clave)) {
                                MostrarInformación($"No se ingresó una clave del certificado de facturación electrónica.{DobleLínea}No se podrá facturar " +
                                                   $"electrónicamente.", "Sin Clave Certificado");
                                return false;
                            }

                            try {
                                using var certificado = new X509Certificate2(RutaCertificado, clave); // Verifica que la clave sea correcta.
                            } catch (CryptographicException) {

                                MostrarError($"La clave ingresada para el certificado {RutaCertificado} es incorrecta.{DobleLínea}Intenta nuevamente.",
                                    "Clave Certificado Incorrecta");
                                goto otraVez;

                            } catch (Exception) {
                                throw;
                            }

                            ClaveCertificado = clave;
                            return true;

                        } else {
                            MostrarInformación($"No se encontró el certificado de facturación electrónica en {RutaCertificado}.{DobleLínea}" +
                                               $"No se podrá facturar electrónicamente.", "Sin Certificado");
                            return false; // No hay certificado entonces tampoco pide la clave. 
                        }

                    }

                }); // SiNulo>

            } // get>

            set => _ClaveCertificado = value;

        } // ClaveCertificado>


        #endregion Facturacion Electrónica>


    } // OpcionesEquipo>



} // SimpleOps.Singleton>
