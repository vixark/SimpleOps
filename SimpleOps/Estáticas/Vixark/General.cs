using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Xml;
using System.Net;
using System.Text.Json.Serialization;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using QRCoder;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
// No puede llevar referencias a otras clases del proyecto en el que se está usando.



namespace Vixark {



    /// <summary>
    /// Métodos, funciones y constantes estáticas de uso general y funcionalidad genérica para cualquier proyecto. Se diferencian de Global porque no son 
    /// propias del funcionamiento de SimpleOps y pueden ser usadas en otros proyectos.
    /// </summary>
    static class General {



        #region Enumeraciones

        public enum Género { Desconocido = 0, Femenino = 1, Masculino = 2, Otro = 3 }

        public enum NúmeroSustantivo { Desconocido = 0, Singular = 1, Plural = 2 }

        public enum ConectorCoordinante { Y = 0, O = 1, Ni = 2, Ninguno = 3 } // https://gramaticalobasico.blogspot.com/2010/01/conectores-coordinantes.html.

        public enum ConectorLógico { Y = 0, O = 1 }

        [Flags] public enum Serialización { EnumeraciónEnTexto = 1, DiccionarioClaveEnumeración = 2 } // Se puede establecer una o varias serializaciones especiales con el operador |.

        public enum TipoElementoRuta { Archivo, Carpeta } // Un elemento de una ruta puede ser un archivo o una carpeta.

        public enum TipoRuta { Vacío, Url, Local, RelativoUrl, RelativoLocal, Elemento } // URL es una ruta que inicia por https:// o http://, Local (en Windows) inicia por [A-Z]:\, RelativoUrl es cualquier otra ruta que contenga / y no sea de tipo URL, RelativoLocal es cualquier otra ruta que contenga \ y no sea de tipo Local, Elemento es un nombre de archivo o carpeta sin otros niveles de carpetas, es decir no contiene / ni \.

        public enum TipoArchivoInformación { Desconocido, Plano, Html }

        #endregion Enumeraciones>



        #region Variables y Constantes

        public static decimal ToleranciaDecimalesPredeterminada = 1;

        public static double ToleranciaDoblesPredeterminada = 0.01;

        public const string PatrónNombreVariable = "[0-9a-záéíóúäëïöüñç]"; // Solo letras y números. Se usa en mensajes de error para extraer el nombre de la propiedad afectada.

        public const string PatrónNúmeroPuntoDecimal = "^[0-9]+.[0-9]+$"; // Coincide con textos que son solo números a la izquierda y a la derecha de un punto.

        public const string FormatoNúmeroMesDía = "MM-dd";

        public const string FormatoFechaHora = "yyyy-MM-dd HH:mm:ss";

        public const string FormatoFecha = "yyyy-MM-dd";

        public const string FormatoFechaHoraTZyMilisegundos = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static readonly ILoggerFactory FábricaRastreadores = LoggerFactory.Create(builder => { builder.AddDebug(); }); // Asigna los rastreadores que serán usados.

        public static readonly ILogger Rastreador = FábricaRastreadores.CreateLogger("Vixark");

        public static string[] ExtensionesJpg = new string[] { ".jpg", ".jpeg" }; // Las posibles extensiones de los archivos JPG son ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi" (ver https://blog.filestack.com/thoughts-and-knowledge/complete-image-file-extension-list/#jpg). Sin embargo, según https://en.wikipedia.org/wiki/JPEG, .jpg y .jpeg son las más usadas, por lo tanto para mejorar el rendimiento en los procedimientos de búsqueda de imágenes solo se permitirán estas dos.

        public static string[] ExtensionesPng = new string[] { ".png" };

        public static string[] ExtensionesImágenes = new string[] { ".jpg", ".png", ".jpeg" }; // Este vector es redundante con los dos anteriores, pero se prefiere disponer de él por rendimiento para no tener que generarlo al vuelo.

        public static string[] ExtensionesTextoPlano = new string[] { ".txt" };

        public static string[] ExtensionesHtml = new string[] { ".html", ".htm" }; // El HTML siempre tiene prioridad sobre el HTM.

        public static string[] ExtensionesHtmlYTextoPlano = new string[] { ".html", ".htm", ".txt" }; // Vector auxiliar que lista las extensiones válidas para archivos de información. El TXT va de último para darle prioridad al HTML si existen ambos archivos.

        public static string[] ExtensionesTextoPlanoYHtml = new string[] { ".txt", ".html", ".htm" }; // Vector auxiliar que lista las extensiones válidas para archivos de información. El TXT va de primero para darle prioridad sobre el HTML si existen ambos archivos.

        public static string[] InicioViñetas = new string[] { "*", "-", "·", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; // Si bien se podrían incorporar viñetas de letras a. b. c. etc., por el momento se omite por rendimiento porque eso implicaría que casi todas las líneas  se tendrían que verificar por la viñeta completa, si si quiere soportar este tipo de viñetas habría que realizar una prueba de rendimiento, por el momento no se consideran tan necesarias.

        public static string PatrónViñetas = @"[0-9]+\.|\*|-|·";

        public static string PatrónViñetasNuméricas = @"[0-9]+\.";

        public static string PatrónRutaLocal = @"([a-z]|[A-Z]):\\"; // Se añade en minúsculas y minúsculas para no tener que recordar especificar que la evaluación de la expresión sea indiferente a la capitalización.

        public static string PatrónRutaUrl = @"(http|https|Http|Https|HTTP|HTTPS):\/\/"; // Se añade en minúsculas y minúsculas para no tener que recordar especificar que la evaluación de la expresión sea indiferente a la capitalización.

        public static string[] EtiquetasTítulosHtml = new string[] { "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>" };

        /// <summary>
        /// Doble Environment.NewLine. Útil para separar parrafos bloques de texto.
        /// </summary>
        public static string DobleLínea = null!; // Se inicia en IniciarVariablesGenerales.

        /// <summary>
        /// Un Environment.NewLine. Solo se usa para no tener que importar System.Enviroment cada vez que se vaya a usar sino que baste con importar General. 
        /// </summary>
        public static string NuevaLínea = null!; // Se inicia en IniciarVariablesGenerales.

        public static DateTime SinMilisegundos(this DateTime fechaHora) => fechaHora.AddTicks(-fechaHora.Ticks % TimeSpan.TicksPerSecond);

        /// <summary>
        /// Una hora de tipo UTC continua pero ajustada una cantidad de horas constante para acercarlo a la hora local del equipo. 
        /// En algunas ocasiones, como países con horarios de verano o con múltiples zonas horarias, puede no coincidir con la hora 
        /// local del equipo pero se usa porque tiene dos ventajas: Es útil para almacenamiento en la base de datos pues permite 
        /// garantizar que acciones realizadas después siempre queden con fecha posterior a las realizadas antes y en el caso de 
        /// países sin horario de verano ni zonas horarias se puede usar directamente como hora del equipo. En los otros casos se 
        /// puede ajustar posteriormente para obtener la hora real.
        /// </summary>
        /// <param name="horasAjuste">La cantidad de horas constante que se ajusta UtcNow.</param>
        /// <returns></returns>
        public static DateTime AhoraUtc(int horasAjuste) => DateTime.UtcNow.AddHours(horasAjuste);

        public static readonly Dictionary<string, (Género, NúmeroSustantivo)> ClasificaciónSustantivos
            = new Dictionary<string, (Género, NúmeroSustantivo)> {
                { "aplicaciones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "campañas", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "categorías", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "clientes", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "cobros", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "compras", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "comprobantesegresos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "contactos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "contactosclientes", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "contactosproveedores", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "líneascotizaciones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "cotizaciones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneascompras", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasnotascréditocompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasnotascréditoventa", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasnotasdébitocompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasnotasdébitoventa", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasordenescompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneaspedidos", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasremisiones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "líneasventas", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "informespagos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "inventariosconsignación", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "líneasnegocio", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "listasprecios", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "marcas", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "materiales", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "movimientosbancarios", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "movimientosefectivo", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "municipios", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "notascréditocompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "notascréditoventa", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "notasdébitocompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "notasdébitoventa", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "ordenescompra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "pedidos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "preciosclientes", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "preciosproveedores", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "productos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "productosbase", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "proveedores", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "reciboscaja", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "referenciasclientes", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "referenciasproveedores", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "remisiones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "roles", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "rolesusuarios", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "sedes", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "subcategorías", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "usuarios", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "ventas", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "id", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "creadorid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "actualizadorid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "fechahoracreación", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "fechahoraactualización", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "nombre", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "tipoentidad", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "identificación", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "teléfono", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "teléfonoalternativo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "dirección", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "municipioid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "saldo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "referenciaenbanco", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "descripciónenbanco", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "díascrédito", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "cupocrédito", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactofacturasid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactocobrosid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "tipocliente", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "subtipocliente", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajeretenciónivapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajeretenciónfuentepropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajeretenciónicapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajeretencionesextrapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "mínimoretenciónivapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "mínimoretenciónfuentepropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "mínimoretenciónicapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "mínimoretencionesextrapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "prioridadpropia", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "formaentregapropia", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "mínimotransportegratispropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "copiasfacturapropio", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "porcentajegananciapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajeivapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "observacionesfactura", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "representantecomercialid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "campañaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "clienteid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "númerosfacturas", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "total", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "máximodíasvencimiento", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "respuesta", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "tipo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "número", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "prefijo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "fechahora", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "subtotal", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "descuento", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "iva", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "inc", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "retenciónfuente", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "retencióniva", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "retenciónica", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "retencionesextra", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "estado", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "proveedorid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "comprobanteegresoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "pedidoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "valorfacturas", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "abono", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "lugar", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "email", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "emailactivo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "productoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "precio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "compraid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cantidad", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "costounitario", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "notacréditocompraid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notacréditoventaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notadébitocompraid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cotizaciónid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notadébitoventaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "ordencompraid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cantidadentregada", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "fechahoracumplimiento", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "remisiónid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "ventaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "valor", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "fechahorapago", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "observaciones", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "protegido", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "máximodíascrédito", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "tienerepresentantecomercial", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "endescripción", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "priorizarenbuscador", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "recibocajaid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "banco", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "otronúmerocuenta", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "sucursal", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "descripción", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "referencia", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "padreid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "departamento", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "código", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "otropaís", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "mensajeríadisponible", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "sedeid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "enviadaproforma", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "remisionar", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "sincronizadaweb", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "prioridad", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "informepagoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "unidad", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "unidadempaque", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "pesounidadempaque", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "dimensiónunidadempaque_alto", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "dimensiónunidadempaque_ancho", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "dimensiónunidadempaque_largo", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cantidadmínima", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cantidadmáxima", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cantidadreservada", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "físico", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "subcategoríaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneanegocioid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "marcaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "materialid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "aplicaciónid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "prioridadwebpropia", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "proveedorpreferidoid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "ubicaciónalmacén", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "porcentajeincpropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "productosasociados", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "porcentajeadicionalgananciapropio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "autorretenedor", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajecostotransporte", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "porcentajedescuento", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "compramínima", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "díasentrega", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "responsableiva", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "tipocuentabancaria", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "númerocuentabancaria", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactopedidosid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactoinformespagosid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "detalleentrega", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "permisos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "usuarioid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "rolid", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "observacionesenvío", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "categoríaid", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "esrepresentantecomercial", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "activo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "fechapagocomisiónenventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "fechapagocomisiónenpago", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "deinventarioconsignación", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "aplicación", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "campaña", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "categoría", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "cliente", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "cobro", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "compra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "comprobanteegreso", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contacto", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactocliente", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "contactoproveedor", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "líneacotización", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneacompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneanotacréditocompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneanotacréditoventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneanotadébitocompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneanotadébitoventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneaordencompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneapedido", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "línearemisión", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "líneaventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "informepago", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "inventarioconsignación", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "líneanegocio", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "preciolista", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "marca", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "material", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "movimientobancario", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "movimientoefectivo", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "municipio", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "cotización", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notacréditocompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notacréditoventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notadébitocompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "notadébitoventa", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "ordencompra", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "pedido", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "preciocliente", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "precioproveedor", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "producto", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "proveedor", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "recibocaja", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "referenciacliente", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "referenciaproveedor", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "remisión", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "rol", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "rolusuario", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "sede", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "subcategoría", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "usuario", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "venta", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "tabla", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "tablas", (Género.Femenino, NúmeroSustantivo.Plural) },
                { "razón", (Género.Femenino, NúmeroSustantivo.Singular) },
                { "atributoproducto", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "tipoatributoproducto", (Género.Masculino, NúmeroSustantivo.Singular) },
                { "atributosproductos", (Género.Masculino, NúmeroSustantivo.Plural) },
                { "tiposatributosproductos", (Género.Masculino, NúmeroSustantivo.Plural) },
            };

        public static readonly Dictionary<string, string> PalabrasFemeninas = new Dictionary<string, string> { // La clave es en masculino y el valor en femenino, se usa para cambiar los textos de acuerdo al género y número. Al no ser muchas las palabras que lo requieren se usa un diccionario.
            { "del", "de la" }, { "el", "la" }, { "está", "está" }, { "bloqueado", "bloqueada" }
        };

        public static readonly Dictionary<string, string> PalabrasPluralesFemeninas = new Dictionary<string, string> { // La clave es en masculino y el valor en femenino plural, se usa para cambiar los textos de acuerdo al género y número. Al no ser muchas las palabras que lo requieren se usa un diccionario.
            { "del", "de las" }, { "el", "las" }, { "está", "están" }, { "bloqueado", "bloqueadas" }
        };

        public static readonly Dictionary<string, string> PalabrasPluralesMasculinas = new Dictionary<string, string> { // La clave es en masculino y el valor en masculino plural, se usa para cambiar los textos de acuerdo al género y número. Al no ser muchas las palabras que lo requieren se usa un diccionario.
            { "del", "de los" }, { "el", "los" }, { "está", "están" }, { "bloqueado", "bloqueados" }
        };

        public static NumberFormatInfo FormatoPesosColombianos = null!; // Se inicia en IniciarVariablesGenerales.


        /// <summary>
        /// Inicia algunas variables que no se pueden iniciar en el cuerpo de la clase Vixark porque requieren unos pasos adicionales para establecer su valor.
        /// </summary>
        public static void IniciarVariablesGenerales() {

            DobleLínea = Environment.NewLine + Environment.NewLine;
            NuevaLínea = Environment.NewLine;
            FormatoPesosColombianos = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            FormatoPesosColombianos.NumberGroupSeparator = " "; // El espacio no es un espacio normal, es un espacio delgado. Ver https://www.compart.com/en/unicode/U+2009.
            FormatoPesosColombianos.NumberDecimalDigits = 0;

        } // IniciarVariablesGenerales>


        #endregion Variables y Constantes>



        #region Delegados

        delegate T3 FuncOut<T1, T2, T3>(T1 a, out T2 b);

        #endregion Delegados>



        #region Textos y Excepciones

        public static string CasoNoConsiderado(string? valor) => $"No se ha considerado el caso para el valor {valor ?? "nulo"}.";

        public static string CasoNoConsiderado<T>(T enumeración) where T : struct, Enum
            => $"No se ha considerado el valor {enumeración} para la enumeración {enumeración.GetType()}.";

        #endregion Textos y Excepciones>



        #region Web


        /// <summary>
        /// Obtiene un documento XML de una <paramref name="respuesta"/> web.
        /// </summary>
        public static XmlDocument? ObtenerXml(HttpWebResponse respuesta) {

            using var flujoRespuesta = new StreamReader(respuesta.GetResponseStream());
            var xmlRespuesta = new XmlDocument();

            try {
                xmlRespuesta.Load(flujoRespuesta);
            } catch (XmlException) {
                return null; // Si no es un XML o si está mal formado no devuelve nada.
            } catch {
                throw;
            }
            var fecha = AhoraUtc(0);
            var textoMes = $"Mes: {fecha.Month:00}";

            return xmlRespuesta;

        } // ObtenerXml>


        #endregion Web>



        #region Archivos, Carpetas y Procesos


        /// <summary>
        /// Obtiene la ruta de una carpeta y provee la opción de crearla si no existe. Si se quiere verificar la existencia
        /// de cierta carpeta se puede pasar la ruta en rutaPadre y pasar nombreCarpeta vacío. Funciona correctamente 
        /// si la carpeta que se requiere crear está dentro de una carpeta que tampoco existe, en este caso se crean todas las carpetas 
        /// necesarias para que exista la ruta rutaPadre + nombreCarpeta.
        /// </summary>
        public static string ObtenerRutaCarpeta(string rutaPadre, string nombreCarpeta, bool crearSiNoExiste) {

            var ruta = Path.Combine(rutaPadre, nombreCarpeta);
            if (!Directory.Exists(ruta)) {

                if (crearSiNoExiste) {
                    Directory.CreateDirectory(ruta);
                } else {
                    throw new Exception($"No existe la carpeta {ruta}");
                }

            }
            return ruta;

        } // ObtenerRutaCarpeta>


        public static string? ObtenerExtensión(string archivo) => Path.GetExtension(archivo).AMinúscula();

        /// <summary>
        /// Devuelve una ruta del archivo con extensión diferente. La <paramref name="nuevaExtensión"/> se pasa sin el punto.
        /// </summary>
        public static string ObtenerRutaCambiandoExtensión(string ruta, string nuevaExtensión)
            => $"{Path.Combine(Path.GetDirectoryName(ruta)!, Path.GetFileNameWithoutExtension(ruta))}.{nuevaExtensión}";

        /// <summary>
        /// Devuelve una ruta del archivo manteniendo la extensión pero agregando un <paramref name="textoAdicional"/> al nombre del archivo.
        /// </summary>
        public static string ObtenerRutaAgregandoTexto(string ruta, string textoAdicional)
            => $"{Path.Combine(Path.GetDirectoryName(ruta)!, Path.GetFileNameWithoutExtension(ruta) + textoAdicional)}{Path.GetExtension(ruta)}";


        /// <summary>
        /// Abre un archivo en Windows.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        public static bool AbrirArchivo(string? rutaArchivo) {
            if (rutaArchivo == null || !File.Exists(rutaArchivo)) return false;
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
            return true;
        } // AbrirArchivo>


        /// <summary>
        /// Borra un archivo sin generar excepciones. Devuelve verdadero si fue borrado y falso si no se pudo borrar. Útil en los casos
        /// que se quiere borrar un archivo pero si por alguna razón este está bloqueado y no se puede borrar se puede dejar sin borrar.
        /// </summary>
        public static bool IntentarBorrar(string rutaArchivo) {

            try {
                File.Delete(rutaArchivo);
                return true;
            #pragma warning disable CA1031 // No capture tipos de excepción generales. Se desactiva porque el objetivo de esta función es precisamente ignorar cualquier excepción que ocurra.
            } catch (Exception) {
            #pragma warning restore CA1031
                return false;
            }

        } // IntentarBorrar>


        /// <summary>
        /// Obtiene la fecha de modificación más reciente entre las fechas de modificación de los archivos dentro de la <paramref name="rutaCarpeta"/>.
        /// Devuelve nulo si la carpeta no contiene archivos.
        /// </summary>
        /// <param name="rutaCarpeta"></param>
        /// <returns></returns>
        public static DateTime? ObtenerÚltimaFechaModificaciónArchivos(string rutaCarpeta) {

            DateTime últimaFechaModificación = DateTime.MinValue;
            foreach (var rutaArchivo in Directory.GetFiles(rutaCarpeta)) {
                var últimaFechaModificaciónArchivo = ObtenerFechaModificaciónUtc(rutaArchivo);
                if (últimaFechaModificaciónArchivo != null && últimaFechaModificaciónArchivo > últimaFechaModificación) 
                    últimaFechaModificación = (DateTime)últimaFechaModificaciónArchivo;
            }
            return últimaFechaModificación == DateTime.MinValue ? (DateTime?)null : últimaFechaModificación;

        } // ObtenerÚltimaFechaModificaciónArchivos>


        /// <summary>
        /// Encapsulación de GetLastWriteTimeUtc() que devuelve nulo si el archivo no existe y sirve para no olvidar usar siempre la versión UTC.
        /// Se prefiere la versión UTC porque es útil en los usos más comunes, como la verificación de si un archivo es más reciente que otro.
        /// Si no se usa la versión UTC, un usuario en otro lugar del mundo le podría enviar un archivo modificado a otro usuario y la función que 
        /// use esta función podría considerar incorrectamente que el archivo no ha sido modificado.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        /// <returns></returns>
        public static DateTime? ObtenerFechaModificaciónUtc(string? rutaArchivo) {

            if (rutaArchivo == null) return null;
            var informaciónArchivo = new FileInfo(rutaArchivo); // Se usa FileInfo por rendimiento para evitar llamar de manera independiente a File.Exists y File.GetLastWriteTimeUtc().
            return informaciónArchivo.Exists ? informaciónArchivo.LastWriteTimeUtc : (DateTime?)null;  // Solo debe existir este LastWriteTimeUtc en todo el código y no debe existir GetLastWriteTimeUtc() ni GetLastWriteTime().

        } // ObtenerFechaModificaciónUtc>


        /// <summary>
        /// Toma el nombre de un archivo y le agrega un sufijo (normalmente un número) para obtener un nuevo nombre de archivo. Útil para generar
        /// copias de archivos o para obtener nombres de archivos numerados.
        /// </summary>
        /// <param name="nombreArchivo"></param>
        /// <param name="sufijo"></param>
        public static string AgregarSufijo(string nombreArchivo, string sufijo)
            => Path.GetFileNameWithoutExtension(nombreArchivo) + sufijo + Path.GetExtension(nombreArchivo);


        /// <summary>
        /// Copia un archivo sin generar excepciones y de una manera más clara y encapsulada que la función Move de .Net.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        /// <param name="rutaCarpetaDestino"></param>
        /// <returns></returns>
        public static bool CopiarArchivo(string? rutaArchivo, string rutaCarpetaDestino) {

            MostrarInformación("Código sin probar.");
            if (rutaArchivo == null) return false;
            var archivo = Path.GetFileName(rutaArchivo);
            if (archivo == null) return false;
            try {
                File.Copy(rutaArchivo, Path.Combine(rutaCarpetaDestino, archivo));
                #pragma warning disable CA1031 // No capture tipos de excepción generales. Se desactiva porque el objetivo de esta función es precisamente ignorar cualquier excepción que ocurra.
            } catch (Exception) {
                #pragma warning restore CA1031
                return false;
            }
            return true;

        } // CopiarArchivo>


        /// <summary>
        /// Copia todos los archivos que estén en <paramref name="rutaCarpetaOrigen"/> en <paramref name="rutaCarpetaDestino"/>. 
        /// Si <paramref name="sobreescribir"/> es falso no se reemplazan los existentes con el mismo nombre en <paramref name="rutaCarpetaDestino"/>. 
        /// Este procedimiento es útil para instaladores. Devuelve la cantidad de archivos copiados.
        /// </summary>
        /// <param name="rutaCarpetaOrigen"></param>
        /// <param name="rutaCarpetaDestino"></param>
        /// <param name="sobreescribir"></param>
        /// <param name="extensión"></param>
        /// <returns></returns>
        public static int CopiarArchivos(string rutaCarpetaOrigen, string rutaCarpetaDestino, bool sobreescribir, string? extensión = null) {

            var archivosCopiados = 0;
            foreach (var rutaArchivo in Directory.GetFiles(rutaCarpetaOrigen, extensión == null ? "*" : $"*.{extensión}")) {
                var rutaNuevoArchivo = Path.Combine(rutaCarpetaDestino, Path.GetFileName(rutaArchivo));
                if (sobreescribir || !File.Exists(rutaNuevoArchivo)) {
                    File.Copy(rutaArchivo, rutaNuevoArchivo, overwrite: sobreescribir);
                    archivosCopiados++;
                }
            }
            return archivosCopiados;

        } // CopiarArchivos>


        public static string ObtenerHashArchivo(string rutaArchivo) {

            using var FileCheck = File.OpenRead(rutaArchivo);
            #pragma warning disable CA5351 // No usar algoritmos criptográficos dañados. Es aceptable porque solo es para identificar el archivo.
            using var md5 = new MD5CryptoServiceProvider();
            #pragma warning restore CA5351
            var md5Hash = md5.ComputeHash(FileCheck);
            return BitConverter.ToString(md5Hash).Reemplazar("-", "").AMinúscula()!;

        } // ObtenerHashArchivo>


        /// <summary>
        /// Función que permite devolver un mensaje descriptivo sobre la no existencia o la falta de asignación del valor de la ruta de cierto archivo 
        /// o carpeta. Si no se va a usar el mensaje obtenido no es necesario usar esta función porque File.Exists() o Directory.Exists() funcionan
        /// correctamente si la ruta es nula.
        /// </summary>
        /// <param name="tipo">Archivo o carpeta.</param>
        /// <param name="ruta">Ruta del archivo o carpeta que se quiere verificar si existe. Puede ser nulo o vacío y se devolverá un mensaje 
        /// adecuado.</param>
        /// <param name="nombre">Nombre del archivo o carpeta que se usará en el mensaje de información si no lo encuentra o si no se ha 
        /// establecido.</param>
        /// <param name="mensaje">Variable en la que se devuelve el mensaje.</param>
        /// <param name="textoAdicional">Texto adicional opcional al final del mensaje para ambos casos.</param>
        /// <returns></returns>
        public static bool ExisteRuta(TipoElementoRuta tipo, string? ruta, string nombre, out string? mensaje, string textoAdicional = "") {

            mensaje = "";
            if ((tipo == TipoElementoRuta.Archivo && !File.Exists(ruta)) || ((tipo == TipoElementoRuta.Carpeta && !Directory.Exists(ruta)))) {

                var textoAdicionalYPunto = textoAdicional + (string.IsNullOrEmpty(textoAdicional) ? "" : ".");
                if (string.IsNullOrEmpty(ruta)) {
                    return Falso(out mensaje, $"No se ha seleccionado el {tipo.ToString().AMinúscula()} de {nombre}. {textoAdicionalYPunto}");
                } else {
                    return Falso(out mensaje, $"No existe el {nombre} en {ruta}. {textoAdicionalYPunto}");
                }

            } else {
                return true;
            }

        } // ExisteRuta>


        /// <summary>
        /// Devuelve la ruta del archivo existente en <paramref name="rutaCarpeta"/>. Si se proporciona la lista <paramref name="extensionesVálidas"/>, 
        /// se busca y se devuelve el archivo con la extensión válida que exista. El <paramref name="nombreArchivo"/> puede contener o no la extensión. 
        /// Si la contiene y es una extensión válida, no busca archivos con otras extensiones válidas. Si no se proporciona la lista 
        /// <paramref name="extensionesVálidas"/>, se verifica la existencia de <paramref name="nombreArchivo"/> en <paramref name="rutaCarpeta"/> sin
        /// verificar ni añadir ninguna extensión. Devuelve nulo si no existe ningún un archivo coincidente. 
        /// </summary>
        /// <param name="nombreArchivo"></param>
        /// <param name="rutaCarpeta"></param>
        /// <param name="extensionesVálidas"></param>
        /// <returns></returns>
        public static string? ObtenerRutaArchivo(string? nombreArchivo, string rutaCarpeta, string[]? extensionesVálidas = null) {

            if (nombreArchivo == null) return null;

            var extensiónArchivo = ObtenerExtensión(nombreArchivo);
            if (extensionesVálidas == null) {

                var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo); 
                if (File.Exists(rutaArchivo)) return rutaArchivo;

            } else {

                if (extensionesVálidas.Contains(extensiónArchivo)) { // Si nombreArchivo ya contiene una extensión válida, no se buscan otros archivos con otras extensiones.

                    var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);
                    if (File.Exists(rutaArchivo)) return rutaArchivo;

                } else { // Si el archivo no contiene una extensión válida, se ensaya con cada extensión de válida para intentar encontrar algún archivo nombreArchivo + extensión.

                    foreach (var extensiónVálida in extensionesVálidas) {

                        var rutaArchivo = Path.Combine(rutaCarpeta, $"{nombreArchivo}{extensiónVálida}");
                        if (File.Exists(rutaArchivo)) return rutaArchivo;

                    }

                }

            }

            return null; 

        } // ObtenerRutaArchivo>


        /// <summary>
        /// Toma una <paramref name="rutaCarpeta"/> de cualquier tipo, URL o Local, con o sin barra al final, y la devuelve
        /// con la barra al final adecuada.
        /// </summary>
        /// <param name="rutaCarpeta"></param>
        /// <param name="barraInversaPorDefecto">Si es verdadero cuando se pasa una rutaCarpeta sin barras se devuelve con una barra inversa: \ al final.</param>
        /// <returns></returns>
        public static string? ObtenerRutaCarpetaConBarra(string? rutaCarpeta, bool barraInversaPorDefecto = false) {

            if (rutaCarpeta == null) return null;
            if (string.IsNullOrEmpty(rutaCarpeta)) return ""; // Para que no devuelva "/".

            var tieneBarra = rutaCarpeta.Contiene(@"/");
            var tieneBarraInversa = rutaCarpeta.Contiene(@"\");
            var usarBarra = false;
            var usarBarraInversa = false;
            if (tieneBarra) {
                usarBarra = true;
            } else if (tieneBarraInversa) {
                usarBarraInversa = true;
            } else {
                usarBarraInversa = barraInversaPorDefecto;
                usarBarra = !barraInversaPorDefecto;
            }

            var rutaRespuesta = rutaCarpeta?.TrimEnd(new[] { ' ', '/', '\\' });
            if (usarBarra) {
                return $"{rutaRespuesta}/";
            } else if (usarBarraInversa) {
                return @$"{rutaRespuesta}\";
            } else {
                throw new Exception("caso de barra no considerado en ObtenerRutaCarpetaConBarra()"); // Se lanza excepción para evitar errores silenciosos.
            }

        } // ObtenerRutaCarpetaConBarra>


        public static TipoRuta ObtenerTipoRuta(string? ruta) {

            if (string.IsNullOrEmpty(ruta)) return TipoRuta.Vacío;
            if (Regex.IsMatch(ruta, PatrónRutaUrl)) return TipoRuta.Url;
            if (Regex.IsMatch(ruta, PatrónRutaLocal)) return TipoRuta.Local;
            if (ruta.Contiene("/")) return TipoRuta.RelativoUrl;
            if (ruta.Contiene(@"\")) return TipoRuta.RelativoLocal;
            return TipoRuta.Elemento;

        } // ObtenerTipoRuta>


        public static void FinalizarSiExisteOtraInstanciaAbierta(string nombreAplicación) {

            var procesoActual = Process.GetCurrentProcess();
            var hashActual = ObtenerHashArchivo(procesoActual.MainModule.FileName);
            foreach (var proceso in Process.GetProcesses()) {

                if (proceso.Id != procesoActual.Id) {

                    try {

                        if (proceso.ProcessName == procesoActual.ProcessName // Evita procesos que obviamente no son el mismo porque tienen diferente nombre y excepciones al intentar acceder a proceso.MainModule en algunos de estos procesos.
                            && hashActual == ObtenerHashArchivo(proceso.MainModule.FileName)) Environment.Exit(0);

                    #pragma warning disable CA1031 // No capture tipos de excepción generales. Se acepta porque el proceso se finaliza.
                    } catch (Exception) {
                    #pragma warning restore CA1031

                        MostrarError($"No se pudo acceder al proceso {procesoActual.ProcessName} para verificar si era " +
                                     $"otra instancia de {nombreAplicación}. No se permitirá abrir {nombreAplicación}. " +
                                     $"Para intentar solucionar este problema reinicia tu computador.");
                        Environment.Exit(0);

                    }

                }

            }

        } // FinalizarSiExisteOtraInstanciaAbierta>


        #endregion Archivos y Carpetas>



        #region Compresión


        /// <summary>
        /// Obtiene un vector de bytes con la información de un archivo ZIP obtenido del empaquetado del archivo en <paramref name="ruta"/>. 
        /// </summary>
        /// <returns></returns>
        public static byte[] ObtenerBytesZip(string ruta) { // Ver https://stackoverflow.com/a/30068762/8330412.

            if (!File.Exists(ruta)) throw new Exception($"No existe el archivo a comprimir {ruta}.");

            using var flujoZip = new MemoryStream(); { // Es necesario este bloque para que comprima correctamente.

                using var zip = new ZipArchive(flujoZip, ZipArchiveMode.Create, true);
                var xmlEnZip = zip.CreateEntry(Path.GetFileName(ruta), CompressionLevel.Optimal);
                using var flujoXmlEnZip = xmlEnZip.Open();
                using var flujoXml = new MemoryStream(File.ReadAllBytes(ruta));
                flujoXml.CopyTo(flujoXmlEnZip);

            }

            return flujoZip.ToArray();

        } // ObtenerBytesZip>


        /// <summary>
        /// Crea un archivo ZIP con el archivo en <paramref name="ruta"/>. Si no se especifica una <paramref name="rutaZip"/> se usa el mismo nombre
        /// del archivo reemplazando la extensión por .zip.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="rutaZip"></param>
        public static void CrearZip(string ruta, string? rutaZip = null) {

            if (rutaZip == null) rutaZip = ObtenerRutaCambiandoExtensión(ruta, "zip");
            if (File.Exists(rutaZip)) throw new Exception($"Ya existe el archivo .zip {rutaZip}.");
            File.WriteAllBytes(rutaZip, ObtenerBytesZip(ruta));

        } // CrearZip>


        #endregion Compresión>



        #region Imágenes


        /// <summary>
        /// Devuelve la ruta del archivo la imagen con la extensión que exista. El <paramref name="archivoImagen"/> puede traer o no la extensión. 
        /// Si no la trae se verifican las extensiones válidas de imágenes. Devuelve nulo si no existe ninguna un archivo coincidente.
        /// </summary>
        /// <param name="archivoImagen"></param>
        /// <param name="rutaCarpeta"></param>
        /// <returns></returns>
        public static string? ObtenerRutaImagen(string? archivoImagen, string rutaCarpeta)
            => ObtenerRutaArchivo(archivoImagen, rutaCarpeta, ExtensionesImágenes);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rutaImagen"></param>
        /// <param name="paraHtml"></param>
        /// <param name="rutaImagenNoDisponible">Se codifica esta imagen si la imagen en <paramref name="rutaImagen"/> es nula o no existe.</param>
        /// <returns></returns>
        public static string ObtenerBase64(string? rutaImagen, bool paraHtml, string? rutaImagenNoDisponible = null) {

            var imagenDisponible = rutaImagen != null && File.Exists(rutaImagen);
            var imagenNoDisponibleRequeridaYDisponible = !imagenDisponible && rutaImagenNoDisponible != null && File.Exists(rutaImagenNoDisponible); // Se usa !imagenDisponible para evitar la llamada a File.Exists() cuando no es necesario.
            var rutaImagenAplicable = imagenDisponible ? rutaImagen : (imagenNoDisponibleRequeridaYDisponible ? rutaImagenNoDisponible : null);
            if (rutaImagenAplicable == null) return "data:null";

            var base64 = Convert.ToBase64String(File.ReadAllBytes(rutaImagenAplicable));
            if (paraHtml) {

                var extensión = ObtenerExtensión(rutaImagenAplicable);
                return extensión switch {
                    var e when e.EstáEn(ExtensionesPng) => "data:image/png;base64," + base64,
                    var e when e.EstáEn(ExtensionesJpg) => "data:image/jpeg;base64," + base64,
                    _ => throw new Exception(CasoNoConsiderado(extensión))
                };

            } else {
                return base64;
            }

        } // ObtenerBase64>


        public static string ObtenerCódigoQRBase64(string texto, bool paraHtml) {

            using var generadorQR = new QRCodeGenerator();
            var datosQR = generadorQR.CreateQrCode(texto, QRCodeGenerator.ECCLevel.L);
            using var base64QR = new Base64QRCode(datosQR);
            return (paraHtml ? "data:image/png;base64," : "") + base64QR.GetGraphic(3);

        } // ObtenerCódigoQRBase64>


        #region RedimensionarImagen()


        /// <summary>
        /// Redimensiona una imagen al nuevo tamaño recortándola inteligentemente. El recorte inteligente detecta el color del fondo de la imagen
        /// y encuentra cual es el rectángulo mínimo al que se puede recortar sin perder ninguna parte de la imagen, recorta la imagen a este 
        /// rectángulo, la ajusta con fondos del color de fondo y la redimensiona al tamaño final requerido.
        /// Tiene funcionamiento diferente cuando está desactivada la opción de compilación para permitir código no seguro y no está añadido el
        /// símbolo de compilación condicional PermitirCódigoNoSeguro. En este caso el recorte de la imagen no es inteligente y la función
        /// se limita a redimensionar la imagen y devolverla con el ancho nuevo manteniendo la relación de aspecto.
        /// </summary>
        /// <param name="archivoInicial"></param>
        /// <param name="archivoFinal"></param>
        /// <param name="anchoNuevo"></param>
        /// <param name="altoNuevo"></param>
        /// <param name="forzarRedimensionamiento">Si es falso y ya existe el <paramref name="archivoFinal"/> y su fecha de modificación es más reciente
        /// que la fecha de modificación del <paramref name="archivoInicial"/>, no se redimensiona. Si es verdadero, siempre se redimensiona
        /// independiente de las fechas de modificación. En el comportamiento predeterminado (falso) ahorra tiempo de cálculo,
        /// pero se permite forzar el redimensionamiento porque en algunos casos las fechas de modificación podrían no ser confiables.</param>
        /// <param name="toleranciaDiferenciaColor">La tolerancia que permite en la diferencia del color de las 4 esquinas y en la detección del contorno 
        /// del objeto sobre el fondo. Con las 4 esquinas se saca un promedio del color de fondo. 
        /// Con este valor en 5 se empieza a notar la diferencia pero sigue siendo sutil. 
        /// Si las imágenes están quedando recortadas descentradas, es porque el fondo tiene suciedad, en estos casos se puede considerar aumentar 
        /// el valor de la tolerancia pero esto puede producir recortes incorrectos de algunas imágenes que se mezclan con el fondo.
        /// Otra solución sería preprocesar las imágenes con un editor de imágenes para reducir la suciedad.</param>
        /// <param name="margenRecorteImagen">Los pixeles de respiración que se le dejan a la imagen recortada a todos los lados. 
        /// Se sugiere usar un valor mayor a 2 porque con menos queda el recorte muy justo donde va el cambio de color.</param>
        public static bool RedimensionarImagen(string archivoInicial, string archivoFinal, int anchoNuevo, int altoNuevo, 
            bool forzarRedimensionamiento = false, double toleranciaDiferenciaColor = 6, int margenRecorteImagen = 4) {

            if (anchoNuevo <= 1 || altoNuevo <= 1 || anchoNuevo > 4000 || altoNuevo > 4000) return false;

            var fechaModificaciónArchivoFinal = ObtenerFechaModificaciónUtc(archivoFinal);
            var fechaModificaciónArchivoInicial = ObtenerFechaModificaciónUtc(archivoInicial); 
            if (fechaModificaciónArchivoInicial == null) return false; // Si es nulo, es porque no existe.
            if (!forzarRedimensionamiento && fechaModificaciónArchivoFinal != null && fechaModificaciónArchivoFinal > fechaModificaciónArchivoInicial) // Si no es nulo, es porque sí existe.
                return true;

            using var imagenOriginalBitmap = new Bitmap(archivoInicial);
            using var imagenOriginal = Image.FromFile(archivoInicial);

            var anchoAnterior = imagenOriginal.Width;
            var altoAnterior = imagenOriginal.Height;
            var rectánguloRecorte = new Rectangle(0, 0, anchoAnterior, altoAnterior); // El valor por defecto es el valor que tomará cuando saque excepción el recortado inteligente o cuando esté deshabilitado PermitirCódigoNoSeguro.

            #if PermitirCódigoNoSeguro // Desactivar eliminando esta variable en Propiedades > Compilación > Símbolos de compilación condicional. Al eliminarla este código se ignora en la compilación, se pierde la funcionalidad de recorte de imágenes inteligente y se puede compilar con la opción 'Permitir código no seguro' desactivada. 

            try {

                var colorFondo = ObtenerColorFondo(imagenOriginalBitmap, toleranciaDiferenciaColor);
                if (ObtenerDistanciaEntreColores(colorFondo, Color.White) > toleranciaDiferenciaColor &&
                    ObtenerDistanciaEntreColores(colorFondo, Color.Transparent) > toleranciaDiferenciaColor)
                    throw new Exception("El color de fondo es muy diferente a blanco o transparente.");

                rectánguloRecorte = ObtenerRecorteImagenAjustada(imagenOriginalBitmap, colorFondo, margenRecorteImagen, toleranciaDiferenciaColor);

            #pragma warning disable CA1031 // No capture tipos de excepción generales. Se permite en este caso porque el código de recorte inteligente puede generar muchas excepciones y si no se puede cortar de esa manera se puede hacer de la manera básica.
            } catch (Exception ex) { // Si sucede cualquier error en el código anterior se procede a recortar de manera básica la imagen.
            #pragma warning restore CA1031
                MostrarError($"No se pudo recortar inteligentemente la imagen {archivoInicial}, se recortó de manera básica.{DobleLínea}{ex.Message}");
            }

            #endif

            double relación = 0;
            int extraEspacioHorizontal = 0;
            int extraEspacioVertical = 0;
            int desfaceHorizontal;
            int desfaceVertical;

            if (rectánguloRecorte.Height / (double)rectánguloRecorte.Width > altoNuevo / (double)anchoNuevo) { // Si la relación alto/ancho del recorte es mayor que la de la imagen final, entonces se debe rellenar el recorte a los lados. Se debe usar el alto del recorte y un ancho apropiado.

                relación = (double)altoNuevo / rectánguloRecorte.Height;
                extraEspacioHorizontal = RedondearAEntero(((anchoNuevo - rectánguloRecorte.Width * relación) / 2));
                desfaceHorizontal = RedondearAEntero(rectánguloRecorte.X * relación - extraEspacioHorizontal);
                desfaceVertical = RedondearAEntero(rectánguloRecorte.Y * relación);

            } else { // Si no, es al contrario.

                relación = (double)anchoNuevo / rectánguloRecorte.Width;
                extraEspacioVertical = RedondearAEntero(((altoNuevo - rectánguloRecorte.Height * relación) / 2));
                desfaceHorizontal = RedondearAEntero(rectánguloRecorte.X * relación);
                desfaceVertical = RedondearAEntero(rectánguloRecorte.Y * relación - extraEspacioVertical);

            }

            using var nuevaImagen = new Bitmap(anchoNuevo, altoNuevo);
            using var gráfico = Graphics.FromImage(nuevaImagen);
            gráfico.SmoothingMode = SmoothingMode.AntiAlias;
            gráfico.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gráfico.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var brochaFondo = new SolidBrush(Color.White)) {

                gráfico.FillRectangle(brochaFondo, new Rectangle(0, 0, anchoNuevo, altoNuevo)); // Un cuadrado con el color de fondo inicial para el caso de las imágenes que traen transparencia.
                gráfico.DrawImage(imagenOriginal, new Rectangle(-desfaceHorizontal, -desfaceVertical, RedondearAEntero(anchoAnterior * relación),
                    RedondearAEntero(altoAnterior * relación)));
                gráfico.FillRectangle(brochaFondo, new Rectangle(0, 0, anchoNuevo, extraEspacioVertical));
                gráfico.FillRectangle(brochaFondo, new Rectangle(0, altoNuevo - extraEspacioVertical, anchoNuevo, extraEspacioVertical));
                gráfico.FillRectangle(brochaFondo, new Rectangle(0, 0, extraEspacioHorizontal, altoNuevo));
                gráfico.FillRectangle(brochaFondo, new Rectangle(anchoNuevo - extraEspacioHorizontal, 0, extraEspacioHorizontal, altoNuevo));

            }

            nuevaImagen.Save(archivoFinal, ImageFormat.Jpeg);
            return true;

        } // RedimensionarImagen>


        #if PermitirCódigoNoSeguro // Desactivar eliminando esta variable en Propiedades > Compilación > Símbolos de compilación condicional. Al eliminarla todo este código se ignora en la compilación y se puede compilar con la opción 'Permitir código no seguro' desactivada. 

        public static double ObtenerDistanciaEntreColores(Color color1, Color color2) {

            if (color1.A == 0 && color2.A == 0) return 0; // Únicamente en el caso del cero en alfa se puede asegurar que la distancia entre dos transparentes es cero. Si se necesitara hacer para otros niveles de alfa habría que buscar otra solución porque el procedimiento siguiente no tiene en cuenta los niveles alfa.

            var rgb1 = new Rgb { R = color1.R, G = color1.G, B = color1.B };
            var xyz1 = rgb1.To<Xyz>();
            var lab1 = xyz1.To<Lab>();

            var rgb2 = new Rgb { R = color2.R, G = color2.G, B = color2.B };
            var xyz2 = rgb2.To<Xyz>();
            var lab2 = xyz2.To<Lab>();

            var cc = new CieDe2000Comparison();

            return cc.Compare(lab1, lab2);

        } // ObtenerDistanciaEntreColores>


        private static int ObtenerCantidadBytesPorPixel(Bitmap bitmap) {

            var bytesPorPixel = (int)bitmap.PixelFormat switch {
                (int)PixelFormat.Format24bppRgb => 3,
                (int)PixelFormat.Format32bppArgb => 4,
                (int)PixelFormat.Format32bppPArgb => 4,
                (int)PixelFormat.Format32bppRgb => 4,
                8207 => throw new Exception("El formato de imagen CMYK id 8207 no está soportado."), // CMYK https://stackoverflow.com/questions/4315335/how-to-identify-cmyk-images-using-c-sharp.
                _ => throw new Exception("El formato de imagen no está soportado.")
            };

            var bytesPorPixel2 = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            if (bytesPorPixel != bytesPorPixel2) throw new Exception("Inconsistencia entre los dos métodos para encontrar los bytes por pixel.");
            return bytesPorPixel;

        } // ObtenerCantidadBytesPorPixel>


        private static bool TieneAlfa(Bitmap bitmap) => ((int)bitmap.PixelFormat) switch {
            (int)PixelFormat.Format24bppRgb => false,
            (int)PixelFormat.Format32bppArgb => true,
            (int)PixelFormat.Format32bppPArgb => true,
            (int)PixelFormat.Format32bppRgb => true,
            8207 => throw new Exception("El formato de imagen CMYK id 8207 no está soportado."), // CMYK https://stackoverflow.com/questions/4315335/how-to-identify-cmyk-images-using-c-sharp.
            _ => throw new Exception("El formato de imagen no está soportado."),
        };


        public static Color ObtenerColorPromedio(List<Color> colores) {

            double R = 0;
            double G = 0;
            double B = 0;
            foreach (var color in colores) {
                R += color.R;
                G += color.G;
                B += color.B;
            }
            var cantidadColores = colores.Count;
            return Color.FromArgb(RedondearAEntero(R / cantidadColores), RedondearAEntero(G / cantidadColores), RedondearAEntero(B / cantidadColores));

        } // ObtenerColorPromedio>


        private static unsafe bool EsColorDiferenteDeFondo(int anchoPaso, int bytesPorPixel, byte* p, int x, int y, Color colorFondo,
            double toleranciaDiferenciaColor) {

            int índicePunto = (y * anchoPaso) + x * bytesPorPixel;
            var color1 = Color.FromArgb(p[índicePunto + 2], p[índicePunto + 1], p[índicePunto]);
            índicePunto = (y * anchoPaso) + (x + 1) * bytesPorPixel;
            var color2 = Color.FromArgb(p[índicePunto + 2], p[índicePunto + 1], p[índicePunto]);
            índicePunto = ((y + 1) * anchoPaso) + (x + 1) * bytesPorPixel;
            var color3 = Color.FromArgb(p[índicePunto + 2], p[índicePunto + 1], p[índicePunto]);
            índicePunto = ((y + 1) * anchoPaso) + x * bytesPorPixel;
            var color4 = Color.FromArgb(p[índicePunto + 2], p[índicePunto + 1], p[índicePunto]);
            var colorPromedio = ObtenerColorPromedio(new List<Color> { color1, color2, color3, color4 }); // El color promedio de un cuadro de 2x2 pixeles alrededor del punto actual.
            var distanciaPromedioFondo = ObtenerDistanciaEntreColores(colorPromedio, colorFondo);
            return (distanciaPromedioFondo > toleranciaDiferenciaColor);

        } // EsColorDiferenteDeFondo>


        private static Rectangle ObtenerRecorteImagenAjustada(Bitmap bitmap, Color colorFondo, int margen, double toleranciaDiferenciaColor) {

            int ancho = bitmap.Width;
            int alto = bitmap.Height;
            int bytesPorPixel = ObtenerCantidadBytesPorPixel(bitmap);
            BitmapData datosBitmap = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int anchoPaso = datosBitmap.Stride;
            IntPtr escaneo0 = datosBitmap.Scan0;
            var puntos = new List<System.Drawing.Point>();

            unsafe {

                byte* p = (byte*)(void*)escaneo0;

                for (int y = 0; y < alto - 1; y += 2) { // Primer escaneo: De arriba hacia abajo haciendo líneas horizontales de la izquierda a la derecha.
                    for (int x = 0; x < ancho - 1; x += 2) {
                        if (EsColorDiferenteDeFondo(anchoPaso, bytesPorPixel, p, x, y, colorFondo, toleranciaDiferenciaColor)) {
                            puntos.Add(new System.Drawing.Point { X = x, Y = y });
                            goto siguiente1;
                        }
                    }
                }
                siguiente1:;

                for (int x = 0; x < ancho - 1; x += 2) { // Segundo escaneo: De la izquierda hacia la derecha haciendo líneas verticales de arriba a abajo.  
                    for (int y = 0; y < alto - 1; y += 2) {
                        if (EsColorDiferenteDeFondo(anchoPaso, bytesPorPixel, p, x, y, colorFondo, toleranciaDiferenciaColor)) {
                            puntos.Add(new System.Drawing.Point { X = x, Y = y });
                            goto siguiente2;
                        }
                    }
                }
                siguiente2:;

                for (int y = alto - 2; y >= 0; y += -2) { // Tercer escaneo: De abajo hacia arriba haciendo líneas horizontales de la izquierda a la derecha.
                    for (int x = 0; x < ancho - 1; x += 2) {
                        if (EsColorDiferenteDeFondo(anchoPaso, bytesPorPixel, p, x, y, colorFondo, toleranciaDiferenciaColor)) {
                            puntos.Add(new System.Drawing.Point { X = x, Y = y });
                            goto siguiente3;
                        }
                    }
                }
                siguiente3:;

                for (int x = ancho - 2; x >= 0; x += -2) { // Cuarto escaneo: De la derecha hacia la izquierda haciendo líneas verticales de arriba a abajo.
                    for (int y = 0; y < alto - 1; y += 2) {
                        if (EsColorDiferenteDeFondo(anchoPaso, bytesPorPixel, p, x, y, colorFondo, toleranciaDiferenciaColor)) {
                            puntos.Add(new System.Drawing.Point { X = x, Y = y });
                            goto siguiente4;
                        }
                    }
                }
                siguiente4:;

            }

            bitmap.UnlockBits(datosBitmap);

            var rectánguloRecorte = new Rectangle(new System.Drawing.Point(puntos[1].X, puntos[0].Y),
                new System.Drawing.Size(puntos[3].X - puntos[1].X, puntos[2].Y - puntos[0].Y));
            rectánguloRecorte.Inflate(margen, margen);
            if (rectánguloRecorte.Y < 0) rectánguloRecorte.Y = 0;
            if (rectánguloRecorte.X < 0) rectánguloRecorte.X = 0;
            if (rectánguloRecorte.Y + rectánguloRecorte.Height > alto) rectánguloRecorte.Height = alto - rectánguloRecorte.Y;
            if (rectánguloRecorte.X + rectánguloRecorte.Width > ancho) rectánguloRecorte.Width = ancho - rectánguloRecorte.X;
            return rectánguloRecorte;

        } // ObtenerRecorteImagenAjustada>


        private static Color ObtenerColorFondo(Bitmap bitmap, double toleranciaDiferenciaColor) { // Cálculado usando las 4 esquinas de la foto.

            int ancho = bitmap.Width;
            int alto = bitmap.Height;
            int bytesPorPixel = ObtenerCantidadBytesPorPixel(bitmap);
            bool tieneAlfa = TieneAlfa(bitmap);
            BitmapData datosBmp = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int stride = datosBmp.Stride;
            IntPtr Scan0 = datosBmp.Scan0;
            var colorFondo = Color.White;

            unsafe {

                byte* p = (byte*)(void*)Scan0;
                int idx;
                idx = (0 * stride) + 0 * bytesPorPixel;
                var color1 = tieneAlfa ? Color.FromArgb(p[idx + 3], p[idx + 2], p[idx + 1], p[idx]) : Color.FromArgb(p[idx + 2], p[idx + 1], p[idx]);
                idx = ((alto - 1) * stride) + 0 * bytesPorPixel;
                var color2 = tieneAlfa ? Color.FromArgb(p[idx + 3], p[idx + 2], p[idx + 1], p[idx]) : Color.FromArgb(p[idx + 2], p[idx + 1], p[idx]);
                idx = ((alto - 1) * stride) + (ancho - 1) * bytesPorPixel;
                var color3 = tieneAlfa ? Color.FromArgb(p[idx + 3], p[idx + 2], p[idx + 1], p[idx]) : Color.FromArgb(p[idx + 2], p[idx + 1], p[idx]);
                idx = (0 * stride) + (ancho - 1) * bytesPorPixel;
                var color4 = tieneAlfa ? Color.FromArgb(p[idx + 3], p[idx + 2], p[idx + 1], p[idx]) : Color.FromArgb(p[idx + 2], p[idx + 1], p[idx]);

                var distancia12 = ObtenerDistanciaEntreColores(color1, color2);
                var distancia23 = ObtenerDistanciaEntreColores(color2, color3);
                var distancia34 = ObtenerDistanciaEntreColores(color3, color4);
                var distancia41 = ObtenerDistanciaEntreColores(color4, color1);

                if (distancia12 != 0 || distancia23 != 0 | distancia34 != 0 | distancia41 != 0) {  // Tiene esquinas de colores diferentes. Se debe usar un promedio.

                    if (distancia12 > toleranciaDiferenciaColor || distancia23 > toleranciaDiferenciaColor | distancia34 > toleranciaDiferenciaColor
                        | distancia41 > toleranciaDiferenciaColor) { // Tiene esquinas de colores muy diferentes, se debe lanzar excepción.
                        throw new Exception("No se pudo obtener el color promedio del fondo porque los pixeles esquineros difieren mucho en color: " +
                            distancia12 + " " + distancia23 + " " + distancia34 + " " + distancia41);
                    } else {
                        colorFondo = ObtenerColorPromedio(new List<Color> { color1, color2, color3, color4 });
                    }

                } else {
                    colorFondo = color1;
                }

            }

            bitmap.UnlockBits(datosBmp);

            return colorFondo;

        } // ObtenerColorFondo>


#endif // PermitirCódigoNoSeguro>


        #endregion RedimensionarImagen>


        #endregion Imágenes>



        #region Encapsulaciones Varias
        // Reemplazo de funciones existentes para soportar textos que pueden ser nulos, para establecer opciones predeterminadas y para omitir advertencias.

        /// <summary>
        /// Encapsulación de acceso rápido para convertir un texto en una enumeración.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static T AEnumeración<T>(this string texto) where T : struct, Enum => (T)Enum.Parse(typeof(T), texto);

        /// <summary>
        /// Usa Convert.ToDecimal que permite mantener las posiciones decimales del texto en el decimal, así estas sean 00. Por ejemplo, 
        /// convierte "158.0100" en 158.0100.
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static decimal ADecimal(this string texto) => Convert.ToDecimal(texto, CultureInfo.InvariantCulture);

        /// <summary>
        /// Función muy rápida de conversión de texto a entero. No realiza verificaciones de errores.
        /// </summary>
        public static int AEntero(this char carácter) => (carácter - '0');

        public static string ASíONo(this bool booleano) => booleano ? "Sí" : "No";

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this int número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string? ATexto(this int? número) => número == null ? null : ((int)número).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this int número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this decimal número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this decimal número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this long número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this char carácter) => carácter.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this double número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this double número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this TimeSpan intervalo, string formato) => intervalo.ToString(formato, CultureInfo.InvariantCulture);

        public static string ATexto(this byte[] bytes) => BitConverter.ToString(bytes).AMinúscula()!; // Se asegura que nunca es nulo porque así el vector sea vacío devuelve una cadena vacía no nula.

        public static string ATexto(this bool booleano) => booleano ? "true" : "false";

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string ATexto(this DateTime fechaHora, string formato) => fechaHora.ToString(formato, CultureInfo.InvariantCulture);


        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static string? ATexto(this DateTime? fechaHora, string formato)
            => fechaHora == null ? null : ((DateTime)fechaHora).ToString(formato, CultureInfo.InvariantCulture);


        /// <summary>
        /// Encapsulación de rápido acceso de StartsWith() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static bool EmpiezaPor(this string? texto, string textoInicio, bool ignorarCapitalización = true)
            => texto != null && texto.StartsWith(textoInicio, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido acceso de EndsWith() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static bool FinalizaCon(this string? texto, string textoFin, bool ignorarCapitalización = true)
            => texto != null && texto.EndsWith(textoFin, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido acceso de Contains() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static bool Contiene(this string texto, string textoContenido, bool ignorarCapitalización = true)
            => texto.Contains(textoContenido, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido acceso de Equals() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static bool IgualA(this string textoInicial, string texto, bool ignorarCapitalización = true)
            => textoInicial.Equals(texto, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido acceso de la negación de Contains() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 
        /// sin saturar el código y para facilitar la lectura.
        /// </summary>
        public static bool NoContiene(this string texto, string textoContenido, bool ignorarCapitalización = true)
            => !texto.Contains(textoContenido, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido de Replace() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static string Reemplazar(this string texto, string anteriorTexto, string? nuevoTexto, bool ignorarCapitalización = true)
            => texto.Replace(anteriorTexto, nuevoTexto, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de acceso rápido para convertir un <paramref name="texto"/> en una fecha dado un <paramref name="formato"/>.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="formato"></param>
        /// <returns></returns>
        public static DateTime? AFecha(this string? texto, string formato)
            => texto == null ? (DateTime?)null : DateTime.ParseExact(texto, formato, CultureInfo.InvariantCulture);


        /// <summary>
        /// Encapsulación de rápido acceso de IndexOf() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 
        /// sin saturar el código.
        /// </summary>
        public static int ÍndiceDe(this string texto, string textoAEncontrar, int índiceInicial)
            => texto.IndexOf(textoAEncontrar, índiceInicial, StringComparison.Ordinal);


        /// <summary>
        /// Encapsulación de rápido acceso de ToLowerInvariant(). Es útil para omitir la advertencia CA1308 sin saturar el código.
        /// No se puede crear un método con el mismo nombre para string (sin ?) porque el compilador no lo permite. Si se necesitara 
        /// se podría hacer otro método con otro nombre. Una solución fácil es usar este método con un string y poner ! después de () 
        /// para informarle al compilador que se asegura que el resultado no será nulo.
        /// </summary>
        #pragma warning disable CA1308 // Normalizar las cadenas en mayúsculas
        public static string? AMinúscula(this string? texto) => texto?.ToLowerInvariant();
        #pragma warning restore CA1308 // Normalizar las cadenas en mayúsculas


        #endregion Encapsulaciones Varias>



        #region Textos, Formatos y Patrones

        public static string ATextoDinero(this decimal número, bool agregarMoneda = true)
            => $"{número.ToString("#,0", FormatoPesosColombianos)}{(agregarMoneda ? " $" : "")}";

        public static bool EmpiezaPorNúmero(this string? texto) => texto != null && texto.Length > 0 && char.IsNumber(texto[0]);

        public static List<string> APalabras(this string? texto) 
            => texto == null ? new List<string>() : texto.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

        /// <summary>
        /// Ejecuta Trim() y reemplaza todos los espacios dobles, triples etc, por espacios simples.
        /// </summary>
        public static string? LimpiarEspacios(this string? texto) => texto == null ? null : Regex.Replace(texto, @"\s+", " ").Trim();


        /// <summary>
        /// Encapsulación de rápido acceso de Equals() usando StringComparison.Ordinal. Es útil usar en el delegado FSonIgualesTextosIgnorandoCapitalización.
        /// </summary>
        public static bool SonIgualesTextosIgnorandoCapitalización(string texto1, string texto2)
            => texto1.Equals(texto2, StringComparison.OrdinalIgnoreCase);
        public static Func<string, string, bool> FSonIgualesTextosIgnorandoCapitalización = SonIgualesTextosIgnorandoCapitalización;


        /// <summary>
        /// Obtiene la palabra correcta según el género y número del sustantivo. Se provee la palabra masculina en singular.
        /// </summary>
        public static string? ObtenerPalabraNúmeroYGénero(string? sustantivo, string palabraMasculina) {

            if (sustantivo == null) return null;

            if (ClasificaciónSustantivos.ContainsKey(sustantivo) && PalabrasFemeninas.ContainsKey(palabraMasculina)
                && PalabrasPluralesFemeninas.ContainsKey(palabraMasculina) && PalabrasPluralesMasculinas.ContainsKey(palabraMasculina)) {

                var género = ClasificaciónSustantivos[sustantivo].Item1;
                var número = ClasificaciónSustantivos[sustantivo].Item2;
                switch (género) {
                    case Género.Desconocido:
                    case Género.Otro:
                        return $"{palabraMasculina}/{PalabrasFemeninas[palabraMasculina]}";
                    case Género.Femenino:

                        switch (número) {
                            case NúmeroSustantivo.Desconocido:
                            case NúmeroSustantivo.Singular:
                                return PalabrasFemeninas[palabraMasculina];
                            case NúmeroSustantivo.Plural:
                                return PalabrasPluralesFemeninas[palabraMasculina];
                            default:
                                throw new Exception(CasoNoConsiderado(número));
                        }

                    case Género.Masculino:

                        switch (número) {
                            case NúmeroSustantivo.Desconocido:
                            case NúmeroSustantivo.Singular:
                                return palabraMasculina;
                            case NúmeroSustantivo.Plural:
                                return PalabrasPluralesMasculinas[palabraMasculina];
                            default:
                                throw new Exception(CasoNoConsiderado(número));
                        }

                    default:
                        throw new Exception(CasoNoConsiderado(género));
                }

            } else {
                Rastreador.LogError($"Error definiendo género de sustantivo {sustantivo} para palabra masculina {palabraMasculina}");
                return palabraMasculina; // No se genera excepción porque no es un error grave. Se mostrará la palabra masculina en los textos donde no se haya especificado toda la información para obtener la palabra del género correcto.
            }

        } // ObtenerPalabraNúmeroYGénero>


        /// <summary>
        /// Determina si un <paramref name="texto"/> inicia por alguno de los <paramref name="textosInicio"/>.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="textosInicio"></param>
        /// <param name="ignorarCapitalización"></param>
        /// <returns></returns>
        public static bool EmpiezaPor(this string? texto, string[] textosInicio, bool ignorarCapitalización = true) {

            foreach (var textoInicio in textosInicio) {
                if (texto.EmpiezaPor(textoInicio, ignorarCapitalización)) return true;
            }
            return false;

        } // EmpiezaPor>


        /// <summary>
        /// Una función auxiliar útil para los métodos que requieren devolver un valor booleano que representa si fueron exitosos o no y 
        /// además establecer un <paramref name="texto"/> de respuesta en la variable <paramref name="mensaje"/>.
        /// </summary>
        public static bool Falso(out string? mensaje, string? texto) {
            mensaje = texto;
            return false;
        } // Falso>


        public static int ContarCoincidencias(string? texto, string? patrón) {
            _ = Extraer(texto, patrón, out int cantidadCoincidencias, excepciónSiNoCoincidencias: false);
            return cantidadCoincidencias;
        } // ContarCoincidencias>


        /// <summary>
        /// Es una función redundante con IsMatch, pero permite predeterminadamente ignorar la capitalización.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="patrón"></param>
        /// <returns></returns>
        public static bool Coincide(string? texto, string? patrón) => ContarCoincidencias(texto, patrón) > 0;


        /// <summary>
        /// Busca en un <paramref name="texto"/> la coincidencia <paramref name="númeroCoincidencia"/> del <paramref name="patrón"/> y devuelve 
        /// el texto coincidido del grupo <paramref name="númeroGrupo"/>. 
        /// Si el <paramref name="númeroGrupo"/> es 0 se devuelve el texto completo coincidido.
        /// </summary>
        public static string? Extraer(string? texto, string? patrón, int númeroGrupo = 0, int númeroCoincidencia = 0, 
            bool excepciónSiNoCoincidencias = true) 
                => Extraer(texto, patrón, out _, out _, númeroGrupo, númeroCoincidencia, excepciónSiNoCoincidencias);


        /// <summary>
        /// Busca en un <paramref name="texto"/> la coincidencia <paramref name="númeroCoincidencia"/> del <paramref name="patrón"/> y devuelve 
        /// el texto coincidido del grupo <paramref name="númeroGrupo"/>. 
        /// Si el <paramref name="númeroGrupo"/> es 0 se devuelve el texto completo coincidido.
        /// </summary>
        public static string? Extraer(string? texto, string? patrón, out int cantidadCoincidencias,
             int númeroGrupo = 0, int númeroCoincidencia = 0, bool excepciónSiNoCoincidencias = true)
                => Extraer(texto, patrón, out cantidadCoincidencias, out _, númeroGrupo, númeroCoincidencia, excepciónSiNoCoincidencias);


        /// <summary>
        /// Busca en un <paramref name="texto"/> la coincidencia <paramref name="númeroCoincidencia"/> del <paramref name="patrón"/> y devuelve 
        /// el texto coincidido del grupo <paramref name="númeroGrupo"/>. 
        /// Si el <paramref name="númeroGrupo"/> es 0 se devuelve el texto completo coincidido.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="patrón"></param>
        /// <param name="númeroGrupo"></param>
        /// <param name="cantidadGrupos"></param>
        /// <param name="cantidadCoincidencias"></param>
        /// <param name="númeroCoincidencia"></param>
        /// <param name="excepciónSiNoCoincidencias"></param>
        /// <returns></returns>
        public static string? Extraer(string? texto, string? patrón, out int cantidadCoincidencias, out int cantidadGrupos,
             int númeroGrupo = 0, int númeroCoincidencia = 0, bool excepciónSiNoCoincidencias = true) { 

            cantidadCoincidencias = 0;
            cantidadGrupos = 0;
            if (string.IsNullOrEmpty(texto)) return null; // Si no hay texto, ningún patrón le aplicaría.
            if (string.IsNullOrEmpty(patrón)) return null; // Si no hay patrón, no hay nada que coincidir.

            var regex = new Regex(patrón, RegexOptions.IgnoreCase); // En términos generales no se necesita diferenciar mayúsculas de minúsculas. Si fuera necesario, se podría pasar estas opciones vía parámetros opcionales.
            var coincidencias = regex.Matches(texto);
            cantidadCoincidencias = coincidencias.Count;
            if (cantidadCoincidencias == 0) {
                if (excepciónSiNoCoincidencias) throw new Exception("Expresión regular sin coincidencias. Arregla la expresión regular, propaga el " +
                                                                    "error o evita esta excepción con excepciónSiNoCoincidencias = false.");
                return null;
            }

            cantidadGrupos = coincidencias[númeroCoincidencia].Groups.Count;
            if (cantidadGrupos == 0) throw new Exception("No se esperaba que hubieran coincidencias y no hayan grupos coincididos.");

            return coincidencias[númeroCoincidencia].Groups[númeroGrupo].Value;

        } // Extraer>


        /// <summary>
        /// Busca la primera coincidencia del <paramref name="patrón"/> dentro del <paramref name="texto"/> y devuelve el valor del grupo 
        /// <paramref name="númeroGrupo"/>.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="patrón"></param>
        /// <param name="númeroGrupo"></param>
        /// <param name="cantidadGrupos"></param>
        /// <param name="errorEnNoCoincidenciaDePatrón"></param>
        /// <param name="devolverValorSiNoHayCoincidencia"></param>
        /// <returns></returns>
        public static string ExtraerConPatrónObsoleta(string texto, string patrón, int númeroGrupo, out int cantidadGrupos, 
            bool errorEnNoCoincidenciaDePatrón = true, bool devolverValorSiNoHayCoincidencia = false) { // Se deben migrar los procedimientos que usen esta función a las nuevas ExtraerConPatrón.

            cantidadGrupos = 0;
            if (string.IsNullOrEmpty(patrón)) {
                return texto;
            } else {

                if (!string.IsNullOrEmpty(texto)) {

                    var regex = new Regex(patrón, RegexOptions.IgnoreCase); // En términos generales no se necesita diferenciar mayúsculas de minúsculas. Si fuera necesario, se podría pasar estas opciones vía parámetros opcionales.
                    var coincidenciasObj = regex.Match(texto);
                    cantidadGrupos = coincidenciasObj.Captures.Count;
                    if (cantidadGrupos > 0) {
                        return coincidenciasObj.Groups[númeroGrupo].Value;
                    } else {

                        if (errorEnNoCoincidenciaDePatrón)
                            throw new Exception("Expresión regular sin coincidencias. Alternativas de solución: Arreglar expresión regular, " +
                                "propagar error o ignorar con errorEnNoCoincidenciaDePatrón = false.");
                        if (!devolverValorSiNoHayCoincidencia) return "";

                    }

                }

            }

            return texto;

        } // ExtraerConPatrónObsoleta>


        /// <summary>
        /// Función muy rápida para convertir un texto en una fecha de formato yyMMdd. Se ejecuta en el 10% del tiempo de ParseExact(). Mejorado usando https://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int y https://stackoverflow.com/questions/15702123/faster-alternative-to-datetime-parseexact/59979819#59979819.
        /// </summary>
        public static DateTime AFechaYYMMDD(this string s)
            => new DateTime((s[0] - '0') * 10 + s[1] - '0' + 2000, (s[2] - '0') * 10 + s[3] - '0', (s[4] - '0') * 10 + s[5] - '0');


        /// <summary>
        /// Función muy rápida para convertir un texto en una fecha de formato yyMMddhhmmssf. Se ejecuta en el 15% del tiempo de ParseExact(). Mejorado usando https://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int y https://stackoverflow.com/questions/15702123/faster-alternative-to-datetime-parseexact/59979819#59979819.
        /// </summary>
        public static DateTime AFechaYYMMDDHHMMSSF(this string s)
            => new DateTime((s[0] - '0') * 10 + s[1] - '0' + 2000, (s[2] - '0') * 10 + s[3] - '0', (s[4] - '0') * 10 + s[5] - '0'
                , (s[6] - '0') * 10 + s[7] - '0', (s[8] - '0') * 10 + s[9] - '0', (s[10] - '0') * 10 + s[11] - '0', (s[12] - '0') * 100);


        /// <summary>
        /// Igual AFechaYYMMDD() pero para textos que pueden ser nulos. Es 50% más lenta.
        /// </summary>
        public static DateTime? AFechaYYMMDDNulable(this string? s) => s == null ? (DateTime?)null
            : new DateTime((s[0] - '0') * 10 + s[1] - '0' + 2000, (s[2] - '0') * 10 + s[3] - '0', (s[4] - '0') * 10 + s[5] - '0');


        /// <summary>
        /// Función muy rápida para convertir una fecha en un texto de formato yyMMdd. Se ejecuta en el 40% del tiempo de ToString("yyMMdd"). 
        /// Tarda 4 veces más que la función inversa AFechaYYMMDD, pero es aceptable por ser un método usado principalmente en escritura la cual es una 
        /// acción menos frecuente. Mejorado usando https://stackoverflow.com/questions/1176276/how-do-i-improve-the-performance-of-code-using-datetime-tostring/59980493#59980493.
        /// </summary>
        public static string ATextoYYMMDD(this DateTime fechaHora) {

            var chars = new char[6];
            int valor = fechaHora.Year % 100;
            chars[0] = (char)(valor / 10 + '0');
            chars[1] = (char)(valor % 10 + '0');
            valor = fechaHora.Month;
            chars[2] = (char)(valor / 10 + '0');
            chars[3] = (char)(valor % 10 + '0');
            valor = fechaHora.Day;
            chars[4] = (char)(valor / 10 + '0');
            chars[5] = (char)(valor % 10 + '0');
            return new string(chars);

        } // ATextoYYMMDD>


        /// <summary>
        /// Función muy rápida para convertir una fecha en un texto de formato yyMMddhhmmssf. Se ejecuta en el 30% del tiempo de ToString("yyMMddhhmmssf"). 
        /// Tarda 3 veces más que la función inversa AFechaYYMMDDHHMMSSF, pero es aceptable por ser un método usado principalmente en escritura la cual es 
        /// una acción menos frecuente. Mejorado usando https://stackoverflow.com/questions/1176276/how-do-i-improve-the-performance-of-code-using-datetime-tostring/59980493#59980493.
        /// </summary>
        public static string ATextoYYMMDDHHMMSSF(this DateTime fechaHora) {

            var chars = new char[13];
            int valor = fechaHora.Year % 100;
            chars[0] = (char)(valor / 10 + '0');
            chars[1] = (char)(valor % 10 + '0');
            valor = fechaHora.Month;
            chars[2] = (char)(valor / 10 + '0');
            chars[3] = (char)(valor % 10 + '0');
            valor = fechaHora.Day;
            chars[4] = (char)(valor / 10 + '0');
            chars[5] = (char)(valor % 10 + '0');
            valor = fechaHora.Hour;
            chars[6] = (char)(valor / 10 + '0');
            chars[7] = (char)(valor % 10 + '0');
            valor = fechaHora.Minute;
            chars[8] = (char)(valor / 10 + '0');
            chars[9] = (char)(valor % 10 + '0');
            valor = fechaHora.Second;
            chars[10] = (char)(valor / 10 + '0');
            chars[11] = (char)(valor % 10 + '0');
            valor = fechaHora.Millisecond;
            chars[12] = (char)(valor / 100 + '0');
            return new string(chars);

        } // ATextoYYMMDDHHMMSSF>


        /// <summary>
        /// Función igual a ATextoYYMMDD pero para fechasHoras que pueden ser nulas. Es 10% más lenta.
        /// </summary>
        public static string? ATextoYYMMDDNulable(this DateTime? fechaHora) {

            if (fechaHora == null) return null;
            var chars = new char[6];
            int valor = ((DateTime)fechaHora).Year % 100;
            chars[0] = (char)(valor / 10 + '0');
            chars[1] = (char)(valor % 10 + '0');
            valor = ((DateTime)fechaHora).Month;
            chars[2] = (char)(valor / 10 + '0');
            chars[3] = (char)(valor % 10 + '0');
            valor = ((DateTime)fechaHora).Day;
            chars[4] = (char)(valor / 10 + '0');
            chars[5] = (char)(valor % 10 + '0');
            return new string(chars);

        } // ATextoYYMMDD>


        /// <summary>
        /// Si el objeto es decimal usa ATextoDinero. Si no lo es usa el ToString propio del objeto.
        /// </summary>
        public static string? ATextoDinero(this object objeto) {

            string? txt;
            if (objeto is decimal) {
                txt = ((decimal?)objeto)?.ATextoDinero();
            } else {
                txt = objeto?.ToString();
            }
            return txt;

        } // ATextoDinero>


        public static string AleatorizarTexto(string texto) {

            var carácteres = texto.ToList();
            var respuesta = "";
            foreach (var c in carácteres) {
                respuesta += Convert.ToChar(Convert.ToInt32(c) + ((AhoraUtc(-5).Month - 6) + AhoraUtc(-5).Day / 5)).ToString();
            }
            return respuesta;

        } // AleatorizarTexto>


        /// <summary>
        /// Función muy rápida de conversión de texto a entero. No realiza verificaciones de errores, ni control de nulidad para máximizar la velocidad.
        /// </summary>
        public static int AEntero(this string texto) { // El método más rápido encontrado. Para 1 000 000 de conversiones en Debug da 15 ms TryParse, 18 ms Parse, 18 ms Convert.ToInt32() y 32 ms este código personalizado. Sin embargo, en release los resultados cambian mucho, para 10 000 000 conversiones se obtiene 60 ms para este código personalizado y 150 ms para TryParse. Ver https://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int.

            var y = 0;
            for (int i = 0; i < texto.Length; i++) {
                y = y * 10 + (texto[i] - '0');
            }
            return y;

        } // AEntero>


        public static SizeF ObtenerTamañoTexto(string? texto, Font fuente, int ancho) {

            if (string.IsNullOrEmpty(texto)) texto = " ";
            SizeF respuesta;
            using var imagen = new Bitmap(1, 1);
            using var gráficos = Graphics.FromImage(imagen);
            gráficos.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            respuesta = gráficos.MeasureString(texto, fuente, ancho, StringFormat.GenericTypographic); // Ver razón en https://stackoverflow.com/questions/1203087/why-is-graphics-measurestring-returning-a-higher-than-expected-number.
            return respuesta;

        } // ObtenerTamañoTexto>


        /// <summary>
        /// Función equivalente a System.IO.File.ReadAllLines() que toma como argumento un texto en vez de una ruta de archivo.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="eliminarLíneasVacías"></param>
        /// <returns></returns>
        public static List<string> ALíneas(this string? texto, bool eliminarLíneasVacías = false) {

            if (texto == null) return new List<string>();
            return texto.Split(new[] { "\r\n", "\r", "\n" }, eliminarLíneasVacías ? StringSplitOptions.RemoveEmptyEntries 
                : StringSplitOptions.None).ToList(); // Ver https://stackoverflow.com/questions/1508203/best-way-to-split-string-into-lines.

        } // ALíneas>


        #endregion Textos, Formatos y Patrones>



        #region Conversión y Compilación de HTML desde Textos Planos


        /// <summary>
        /// <para>Abre un archivo de texto plano (.txt) o HTML y devuelve su representación en HTML.</para>
        /// 
        /// <para>Adiciona elementos &lt;h2&gt; para líneas cortas de menos de <paramref name="máximoLargoTítulos"/> de carácteres (considerados títulos),
        /// &lt;br&gt; para saltos de línea individuales, &lt;img&gt; para líneas con una imagen si terminan en una extensión válida de imagen, 
        /// &lt;p&gt; para párrafos y &lt;ol&gt;+&lt;li&gt; o &lt;ul&gt;+&lt;li&gt; para listas que se pueden construir con *, -, · y 1., 2., etc. 
        /// Dentro del texto plano permite la inclusión de algunos elementos HTML como &lt;i&gt; para cursivas y &lt;b&gt; para negritas que no serán 
        /// alterados mientras se añadan en una sola línea.</para> 
        /// 
        /// <para>Si el archivo tiene extensión .html o .htm, devuelve el contenido del archivo sin 
        /// modificación, pero permite la compilación de diferentes archivos en un solo HTML mediante el uso de la etiqueta 
        /// &lt;object data="Fragmento.html" /&gt;. De esta manera se permite reusar fragmentos de texto y HTML en múltiples archivos.
        /// Si el archivo es de texto plano, permite realizar la compilación de varios agregando en una línea independiente el nombre o ruta del archivo 
        /// con el fragmento a insertar, así: Fragmento.txt. Tanto en archivos HTML como en archivos de texto plano se pueden insertar archivos con 
        /// fragmentos HTML o planos, es decir se pueden combinar archivos de texto plano con archivos HTML y viceversa.</para> 
        /// 
        /// <para>En los fragmentos (tanto en texto plano como HTML) se pueden definir variables para ser reemplazadas 
        /// por valores personalizados así: La {TipoProducto} marca {Marca}. En el archivo HTML donde se insertará el fragmento se puede establecer 
        /// su valor usando &lt;object data="Fragmento.html" data-tipoproducto="camiseta" data-marca="Gato" /&gt;. En archivos de texto plano se
        /// pueden establecer los valores de las variables así: {TipoProducto=camiseta}{Marca=Gato}Fragmento.html.</para>
        /// 
        /// <para>Las imágenes y los fragmentos pueden ser escritos con la ruta completa (con o sin / o \ al final) o solo con el nombre de 
        /// la imagen o fragmento y proporcionar una ruta común a todos con el parámetro <paramref name="rutaCarpetaImágenes"/> o 
        /// <paramref name="rutaCarpetaFragmentos"/>. Si no se establece la ruta completa y tampoco se pasa un valor para 
        /// las <paramref name="rutaCarpetaFragmentos"/> o <paramref name="rutaCarpetaImágenes"/>, se tomará cómo base la ruta de la 
        /// carpeta de <paramref name="rutaArchivo"/>. Se pueden insertar fragmentos o imágenes usando rutas locales relativas, así:
        /// fragmentos/fragmento.txt o fragmentos\fragmento.txt y se buscará dentro de la carpeta base la carpeta fragmentos y en ella 
        /// el archivo fragmento.txt. El nombre del archivo fragmento.txt no puede contener el carácter { ni }.
        /// </para>
        /// 
        /// <para>Al establecer <paramref name="codificarImágenes"/> en verdadero, las imágenes locales se 
        /// codifican en Base64 directamente en el código HTML sin ninguna referencia a fuentes externas. Esta función solo está disponible
        /// para imagenes locales.</para> 
        /// </summary>
        /// <param name="rutaArchivo"></param>
        /// <param name="máximoLargoTítulos"></param>
        /// <param name="rutaCarpetaImágenes"></param>
        /// <param name="codificarImágenes"></param>
        /// <param name="rutaCarpetaFragmentos"></param>
        /// <returns></returns>
        public static string? ConvertirAHtml(string? rutaArchivo, string? rutaCarpetaImágenes = null, string? rutaCarpetaFragmentos = null,
            bool codificarImágenes = false, int máximoLargoTítulos = 100)
                => ConvertirALíneasHtml(rutaArchivo, rutaCarpetaImágenes, rutaCarpetaFragmentos, codificarImágenes, máximoLargoTítulos).ATextoEnLíneas();


        /// <summary>
        /// Función similar a <see cref="ConvertirAHtml(string?, string?, string?, bool, int)"/> que devuelve líneas de HTML en vez del HTML completo.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        /// <param name="máximoLargoTítulos"></param>
        /// <param name="rutaCarpetaImágenes"></param>
        /// <param name="codificarImágenes"></param>
        /// <param name="rutaCarpetaFragmentos"></param>
        /// <returns></returns>
        public static List<string> ConvertirALíneasHtml(string? rutaArchivo, string? rutaCarpetaImágenes = null, string? rutaCarpetaFragmentos = null,
            bool codificarImágenes = false, int máximoLargoTítulos = 100) {

            if (rutaArchivo == null) return new List<string>();
            if (!File.Exists(rutaArchivo)) return new List<string>();
            var tipoArchivoInformación = ObtenerTipoArchivoInformación(rutaArchivo);
            if (tipoArchivoInformación == TipoArchivoInformación.Desconocido) return new List<string>();
            var carpetaArchivo = Path.GetDirectoryName(rutaArchivo);

            return ConvertirALíneasHtml(File.ReadAllText(rutaArchivo), tipoArchivoInformación, rutaCarpetaImágenes ?? carpetaArchivo,
                rutaCarpetaFragmentos ?? carpetaArchivo, codificarImágenes, máximoLargoTítulos);

        } // ConvertirALíneasHtml>


        public static TipoArchivoInformación ObtenerTipoArchivoInformación(string? rutaArchivo) {

            if (rutaArchivo == null) return TipoArchivoInformación.Desconocido;
            var extensiónArchivo = ObtenerExtensión(rutaArchivo);
            if (!ExtensionesHtmlYTextoPlano.Contains(extensiónArchivo)) return TipoArchivoInformación.Desconocido;
            var tipoArchivoInformación = TipoArchivoInformación.Desconocido;
            if (ExtensionesHtml.Contains(extensiónArchivo)) tipoArchivoInformación = TipoArchivoInformación.Html;
            if (ExtensionesTextoPlano.Contains(extensiónArchivo)) tipoArchivoInformación = TipoArchivoInformación.Plano;
            if (tipoArchivoInformación == TipoArchivoInformación.Desconocido)
                throw new Exception("Se encontró una inconsistencia entre los vectores ExtensionesHtmlYTextoPlano, ExtensionesHtml y ExtensionesTextoPlano."); // Si lanza esta excepción se debe a que se han agregado extensiones a ExtensionesHtmlYTextoPlano que no están en ExtensionesHtml ni ExtensionesTextoPlano.
            return tipoArchivoInformación;

        } // ObtenerTipoArchivoInformación>


        /// <summary>
        /// Función similar a <see cref="ConvertirAHtml(string?, string?, string?, bool, int)"/> que toma un texto y su tipo en vez de la ruta del archivo.
        /// Además, devuelve líneas de HTML en vez del HTML completo.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="tipo"></param>
        /// <param name="rutaCarpetaImágenes"></param>
        /// <param name="codificarImágenes"></param>
        /// <param name="rutaCarpetaFragmentos"></param>
        /// <param name="máximoLargoTítulos"></param>
        /// <returns></returns>
        public static List<string> ConvertirALíneasHtml(string? texto, TipoArchivoInformación tipo, string? rutaCarpetaImágenes = null,
            string? rutaCarpetaFragmentos = null, bool codificarImágenes = false, int máximoLargoTítulos = 100)
                => ConvertirALíneasHtml(texto.ALíneas(), tipo, rutaCarpetaImágenes, rutaCarpetaFragmentos, codificarImágenes, máximoLargoTítulos);


        /// <summary>
        /// Función similar a <see cref="ConvertirAHtml(string?, string?, string?, bool, int)"/> que toma una lísta de líneas de texto y su tipo en vez 
        /// de la ruta del archivo o el texto.
        /// Además, devuelve líneas de HTML en vez del HTML completo.
        /// </summary>
        /// <param name="líneas"></param>
        /// <param name="tipo"></param>
        /// <param name="rutaCarpetaImágenes"></param>
        /// <param name="codificarImágenes"></param>
        /// <param name="rutaCarpetaFragmentos"></param>
        /// <param name="máximoLargoTítulos"></param>
        /// <returns></returns>
        public static List<string> ConvertirALíneasHtml(List<string> líneas, TipoArchivoInformación tipo, string? rutaCarpetaImágenes = null,
            string? rutaCarpetaFragmentos = null, bool codificarImágenes = false, int máximoLargoTítulos = 100) {

            var líneasRespuesta = new List<string>();

            var rutaCarpetaImágenesConBarra = ObtenerRutaCarpetaConBarra(rutaCarpetaImágenes); // Obtiene la ruta con la barra al final, independiente de que la traiga o no.
            var tipoRutaCarpetaImágenes = ObtenerTipoRuta(rutaCarpetaImágenesConBarra); // Se usa rutaCarpetaImágenesConBarra para tener en cuenta la posible barra que se le pudo poner en la línea anterior.

            var rutaCarpetaFragmentosConBarra = ObtenerRutaCarpetaConBarra(rutaCarpetaFragmentos);
            var tipoRutaCarpetaFragmentos = ObtenerTipoRuta(rutaCarpetaFragmentosConBarra);


            static void corregirBarraIncorrectaEnRutaRelativa(ref string archivo, ref TipoRuta tipoRuta, TipoRuta tipoCarpetaBase) { // Para evitar errores muy comunes cuando el usuario olvida cuál barra inclinada debe usar cuando escribe una ruta relativa, por ejemplo escribiendo fragmentos/fragmento.txt en vez de fragmentos\fragmento.txt, se es indulgente con esta funcionalidad y se corrije a la barra inclinada correcta dependiendo del tipo de carpeta base.

                if (tipoCarpetaBase == TipoRuta.Local && tipoRuta == TipoRuta.RelativoUrl) {
                    archivo = archivo.Reemplazar("/", @"\");
                    tipoRuta = ObtenerTipoRuta(archivo);
                }
                if (tipoCarpetaBase == TipoRuta.Url && tipoRuta == TipoRuta.RelativoLocal) {
                    archivo = archivo.Reemplazar(@"\", "/");
                    tipoRuta = ObtenerTipoRuta(archivo);
                }

            } // corregirBarraIncorrectaEnRutaRelativa>


            List<string> obtenerLíneasFragmento(string líneaFragmento, string patrónArchivoFragmento, string patrónVariables) {

                var st = new StackTrace();
                if (st.FrameCount > 130) return new List<string>() { "<h1>Error: Superado el límite máximo de archivos a insertar.</h1>" }; // Normalmente cuando entra a este nivel está en alrededor de 60-70 niveles de llamadas. Por cada llamada recursiva a ConvertirALíneasHtml() se incrementan 3 niveles al seguimiento de pila, por lo tanto si se permiten alrededor de 60 niveles más, se están permitiendo alrededor de 20 llamadas recursivas. Este nivel creo que es suficiente para cualquier tipo de uso (incluso algo excesivo) y evita el error Stack Overflow cuando el usuario por error realice referencias circulares entre fragmentos. Se inserta con h1 para que este error sea muy visible.

                var líneasFragmento = new List<string>();
                var archivoFragmento = Extraer(líneaFragmento, patrónArchivoFragmento, númeroGrupo: 1);

                if (archivoFragmento != null) {

                    var tipoRutaFragmento = ObtenerTipoRuta(archivoFragmento);
                    corregirBarraIncorrectaEnRutaRelativa(ref archivoFragmento, ref tipoRutaFragmento, tipoRutaCarpetaFragmentos);

                    string? rutaFragmento = tipoRutaFragmento switch {
                        TipoRuta.Vacío => throw new Exception("No se esperaba que tipoRutaArchivo fuera vacío."),
                        TipoRuta.Local => archivoFragmento,
                        TipoRuta.Url => null, // Solo se soporta la compilación de archivos localmente. Si hay un HTML enlazado a servidor, no se procesa, este lo procesaría directamente el navegador.        
                        TipoRuta.RelativoUrl => null, // Solo se soporta la compilación de archivos localmente. Si hay un HTML enlazado a servidor, no se procesa, este lo procesaría directamente el navegador.        
                        TipoRuta.RelativoLocal => tipoRutaCarpetaFragmentos == TipoRuta.Local ? $"{rutaCarpetaFragmentosConBarra}{archivoFragmento}" : null,
                        TipoRuta.Elemento => tipoRutaCarpetaFragmentos == TipoRuta.Local ? $"{rutaCarpetaFragmentosConBarra}{archivoFragmento}" : null,
                    };

                    if (rutaFragmento != null) {

                        var extensiónFragmento = ObtenerExtensión(rutaFragmento);
                        if (File.Exists(rutaFragmento) && ExtensionesHtmlYTextoPlano.Contains(extensiónFragmento)) {

                            var variables = new Dictionary<string, string>();
                            var cantidadVariables = ContarCoincidencias(líneaFragmento, patrónVariables);

                            for (int c = 0; c < cantidadVariables; c++) {

                                var variable = Extraer(líneaFragmento, patrónVariables, númeroGrupo: 1, númeroCoincidencia: c);
                                var valor = Extraer(líneaFragmento, patrónVariables, númeroGrupo: 2, númeroCoincidencia: c);
                                if (variable != null && valor != null) {
                                    variables.Agregar($"{{{variable.AMinúscula()}}}", valor);
                                } else {
                                    throw new Exception("No se esperaba que no se pudiera coincidir variable o valor en el <object>");
                                }

                            }

                            var patrónReemplazo = "{[a-zA-Z0-9_]+}";
                            var textoFragmento = File.ReadAllText(rutaFragmento);
                            textoFragmento = Regex.Replace(textoFragmento, patrónReemplazo,
                                m => variables.ContainsKey(m.Value.AMinúscula()!) ? variables[m.Value.AMinúscula()!] : m.Value, RegexOptions.IgnoreCase);
                            var líneasArchivo = ConvertirALíneasHtml(textoFragmento, ObtenerTipoArchivoInformación(rutaFragmento), rutaCarpetaImágenes,
                                rutaCarpetaFragmentos, codificarImágenes, máximoLargoTítulos); // Para evitar complejizar las rutas de carpetas y fragmentos, las carpeta base para imagenes y fragmentos de un fragmento serán las mismas carpetas base del archivo original donde se insertará el fragmento. Se implementa de esta manera para evitar que el usuario tenga que pensar en qué lugar está almacenado un fragmento al momento de relacionar otro fragmento dentro de este. De esta manera manejando las mismas carpetas base del archivo original todos los fragmentos, independiente del archivo (y carpeta contenedora de este) donde sean insertados, se podrían cargar así: framentos/fragmento.txt. Además, para el uso más básico, que es especificar una carpeta base para todos los fragmentos, el usuario no tiene que pensar en ningún momento en rutas relativas y siempre podrá insertar un fragmento directamente con fragmento.txt, mientras todos los fragmentos estén en la carpeta base de los fragmentos. La prioridad siempre es facilitar el uso más básico primero y después los usos más avanzados.
                            líneasFragmento.AddRange(líneasArchivo); // Agrega todas las líneas HTML procesadas y compiladas del archivo fragmento a insertar.

                        } else {
                            líneasFragmento.Add(líneaFragmento);
                            líneasFragmento.Add($"<h1>Error: No se encontró el archivo {rutaFragmento} o este no es de texto plano (.txt) o HTML.</h1>"); // Notifica el error directamente en el archivo para que por error el usuario no agregue archivos que no existen y no se entere del problema. Se usa <h1> para que definitivamente no pase desapercibido.
                        }

                    } else {
                        líneasFragmento.Add(líneaFragmento); // En el caso de fragmentos insertados en texto plano, por error el usuario insertó un archivo en URL entonces no se procesa. En el caso de fragmentos insertados en HTML, es una línea de HTML que empieza por <object y tiene data=, pero el contenido de data es una URL absoluta o relativa. En estos casos se considera que es un uso deseado de la etiqueta para otras funciones o cargar HTML de otros lugares, entonces no se realiza la compilación. La línea se escribe inalterada.
                    }

                } else {
                    throw new Exception("No se esperaba que no coincidiera el patrón de <object>"); // Nunca debería pasar porque al entrar a esta función ya se identificó que la línea es para insertar un fragmento entonces siempre debería coincidir el patrón para extraer el archivo del fragmento.
                }

                return líneasFragmento;

            } // obtenerLíneasFragmento>


            switch (tipo) {
                case TipoArchivoInformación.Desconocido:
                    throw new Exception("No se puede convertir a HTML un archivo de tipo desconocido.");
                case TipoArchivoInformación.Html:

                    foreach (var línea in líneas) {

                        if (línea.EmpiezaPor("<object") && línea.Contiene("data=")) {
                            líneasRespuesta.AddRange(obtenerLíneasFragmento(línea, patrónArchivoFragmento: @"data=""(.+?)""",
                                patrónVariables: @"data-*([a-z0-9]+?)=""(.+?)"""));
                        } else {
                            líneasRespuesta.Add(línea); // Es una línea normal de HTML.
                        }

                    }
                    break;

                case TipoArchivoInformación.Plano:

                    var idsLíneasImágenes = new List<int>();
                    var idsLíneasImágenesRutaCompletaUrl = new List<int>();
                    var idsLíneasImágenesRutaCompletaLocal = new List<int>();
                    var idsLíneasImágenesRutaRelativaUrl = new List<int>();
                    var idsLíneasTítulos = new List<int>();
                    var idsLíneasInicioLista = new List<int>();
                    var idsLíneasFinLista = new List<int>();
                    var idsLíneasInicioListaNumérica = new List<int>();
                    var idsLíneasÍtemLista = new List<int>();
                    var idsLíneasParrafos = new List<int>();
                    var idsLíneasTítulosForzados = new List<int>();
                    var idsLíneasParrafosForzados = new List<int>();
                    var idsLíneasFragmentos = new List<int>();
                    var enLista = false;

                    string? obtenerLínea(int i) => i < líneas.Count && i > 0 ? líneas[i] : null;

                    #region Análisis del texto
                    for (int i = 0; i < líneas.Count; i++) {

                        var línea = líneas[i];
                        var extensiónLínea = ObtenerExtensión(línea);

                        if (!string.IsNullOrEmpty(extensiónLínea) && ExtensionesHtmlYTextoPlano.Contains(extensiónLínea)) {
                            idsLíneasFragmentos.Add(i);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(extensiónLínea) && ExtensionesImágenes.Contains(extensiónLínea)) {

                            var tipoRutaImagen = ObtenerTipoRuta(línea);
                            corregirBarraIncorrectaEnRutaRelativa(ref línea, ref tipoRutaImagen, tipoRutaCarpetaImágenes);
                            if (líneas[i] != línea) líneas[i] = línea; // Si línea fue modificada se escribe este valor en la líneas[i] porque esta no se puede pasar como ref al método CorregirBarraIncorrectaEnRutaRelativa().

                            switch (tipoRutaImagen) {
                                case TipoRuta.Url:
                                    idsLíneasImágenesRutaCompletaUrl.Add(i);
                                    continue;
                                case TipoRuta.RelativoUrl:
                                    idsLíneasImágenesRutaRelativaUrl.Add(i);
                                    continue;
                                case TipoRuta.Local:
                                    idsLíneasImágenesRutaCompletaLocal.Add(i);
                                    continue;
                                case TipoRuta.Elemento:
                                case TipoRuta.RelativoLocal: // Se soporta relativo local para los casos en los que se quiera usar una ruta base y dentro de esta carpetas distintas que se especifican en cada archivo de imagen, así: RutaCarpetaImagenes = "C:\Imagenes", imagenes referenciadas "Auxiliares\Norma.jpg" y "Detalle\CamisetaCuello.jpg".
                                    idsLíneasImágenes.Add(i);
                                    continue;
                                case TipoRuta.Vacío:
                                    throw new Exception("No se esperaba que tipoRutaImagen fuera vacía.");
                            }

                        }

                        if (línea.EmpiezaPor(InicioViñetas)) { // Se hace en dos pasos por rendimiento.

                            if (!enLista) {
                                if (Regex.IsMatch(línea, PatrónViñetasNuméricas)) {
                                    idsLíneasInicioListaNumérica.Add(i);
                                } else {
                                    idsLíneasInicioLista.Add(i);
                                }
                            }
                            idsLíneasÍtemLista.Add(i);
                            enLista = true;
                            continue;

                        } else {
                            if (enLista) idsLíneasFinLista.Add(i - 1); // La lista termina en el último ítem que está en la línea anterior.
                            enLista = false;
                        }

                        if (!string.IsNullOrEmpty(línea) && string.IsNullOrWhiteSpace(obtenerLínea(i + 1)) && string.IsNullOrWhiteSpace(obtenerLínea(i - 1))) {

                            if (línea.Length <= máximoLargoTítulos) {

                                if (!línea.EmpiezaPor("<p>")) {
                                    idsLíneasTítulos.Add(i);
                                } else {
                                    idsLíneasParrafosForzados.Add(i);
                                }
                                continue;

                            } else {

                                if (!línea.EmpiezaPor(EtiquetasTítulosHtml)) {
                                    idsLíneasParrafos.Add(i);
                                } else {
                                    idsLíneasTítulosForzados.Add(i);
                                }
                                continue;

                            }

                        }

                    }
                    #endregion Análisis del texto>

                    #region Escritura del HTML
                    string? listaActual = null;

                    for (int i = 0; i < líneas.Count; i++) {

                        var línea = líneas[i];

                        if (idsLíneasFragmentos.Contains(i)) {

                            líneasRespuesta.AddRange(obtenerLíneasFragmento(línea,
                                patrónArchivoFragmento: línea.Contiene("{") ? @"}([^{}]+?)$" : "^(.+)$", // Hay una pequeña limitación al nombre del archivo del fragmento y es que no puede contener { ni }.
                                patrónVariables: @"{([a-z0-9]+?)=(.+?)}"));
                            continue;

                        }

                        #region Imágenes   
                        if (idsLíneasImágenes.Contains(i) || idsLíneasImágenesRutaCompletaUrl.Contains(i)
                            || idsLíneasImágenesRutaCompletaLocal.Contains(i) || idsLíneasImágenesRutaRelativaUrl.Contains(i)) {

                            var contenidoSrc = "";
                            if (idsLíneasImágenes.Contains(i)) {

                                var rutaImagen = $"{rutaCarpetaImágenesConBarra}{línea}";
                                contenidoSrc = tipoRutaCarpetaImágenes switch {
                                    TipoRuta.Vacío => rutaImagen,
                                    TipoRuta.Url => rutaImagen,
                                    TipoRuta.Local => codificarImágenes ? ObtenerBase64(rutaImagen, paraHtml: true) : $@"file:/{rutaImagen}", // Solo se soporta la codificación base 64 de imagenes usando archivos locales.
                                    TipoRuta.RelativoUrl => rutaImagen,
                                    TipoRuta.RelativoLocal => rutaImagen,
                                    TipoRuta.Elemento => rutaImagen,
                                };

                            } else if (idsLíneasImágenesRutaCompletaUrl.Contains(i)) { // Se agrega completa la ruta de la imagen como está escrita sin ninguna modificación.
                                contenidoSrc = línea; // No se soporta la codificación base 64 para imágenes en URLs.
                            } else if (idsLíneasImágenesRutaRelativaUrl.Contains(i)) {
                                contenidoSrc = $"{rutaCarpetaImágenesConBarra}{línea}"; // No se soporta la codificación base 64 para imágenes relativas en URLs. Se soporta el reemplazo de rutas relativas URL por su ruta completa con su 'carpeta' URL base porque esto solo sucede cuando se provee una URL como carpeta base y puede ser útil cuando se conoce la estructura del sitio web donde están alojadas las imágenes. Sin embargo, podría darse el caso que se quiera dejar la ruta relativa a la imagen inalterada en el HTML, para esto habría que implementar un nuevo parámetro a todas estas funciones. Aunque en principio no se siente tan necesario porque como este procedimiento genera archivos compilados, el usuario puede realizar en cualquier momento su recompilación cuando le cambie la carpeta 'base' de la URL y obtener los nuevos archivos con las rutas de las imagenes absolutas correctas.
                            } else if (idsLíneasImágenesRutaCompletaLocal.Contains(i)) {
                                contenidoSrc = codificarImágenes ? ObtenerBase64(línea, paraHtml: true) : $@"file:/{línea}";
                            }

                            líneasRespuesta.Add($@"<img src=""{contenidoSrc}"" /><br />");
                            continue;

                        }
                        #endregion Imágenes>

                        #region Listas
                        if (idsLíneasInicioListaNumérica.Contains(i)) {
                            líneasRespuesta.Add("<ol>"); // Requiere continuar agregando el ítem.
                            listaActual = "ol";
                        }

                        if (idsLíneasInicioLista.Contains(i)) {
                            líneasRespuesta.Add("<ul>"); // Requiere continuar agregando el ítem.
                            listaActual = "ul";
                        }

                        if (idsLíneasÍtemLista.Contains(i)) líneasRespuesta.Add($@"<li>{(Regex.Replace(línea, PatrónViñetas, "")).TrimStart()}</li>");

                        if (idsLíneasFinLista.Contains(i)) {

                            if (listaActual == "ol") líneasRespuesta.Add("</ol>");
                            if (listaActual == "ul") líneasRespuesta.Add("</ul>");
                            if (listaActual == null) throw new Exception("No se esperaba que listaActual fuera vacía.");
                            continue; // Ya agregó el ítem y el cierre de lista, ya puede pasar a la siguiente línea.

                        } else {
                            if (idsLíneasÍtemLista.Contains(i)) continue; // Si estaba añadiendo un ítem y no se trataba de un cierre de lista ya puede pasar a la siguiente línea.
                        }
                        #endregion Listas>

                        if (idsLíneasTítulos.Contains(i)) {
                            líneasRespuesta.Add($@"<h2>{línea}</h2>");
                            continue;
                        }

                        if (idsLíneasParrafos.Contains(i)) {
                            líneasRespuesta.Add($@"<p>{línea}</p>");
                            continue;
                        }

                        #region Saltos de Línea
                        if (idsLíneasParrafosForzados.Contains(i) || idsLíneasTítulosForzados.Contains(i)) {
                            líneasRespuesta.Add(línea); // Se agrega la línea sin cambios y se pasa a la siguiente línea porque ya se está forzando un elemento html que genera cambio de línea y no se necesita un salto de línea.
                            continue;
                        } else {

                            var tieneBloqueAtrás = (idsLíneasTítulos.Contains(i - 1) || idsLíneasParrafos.Contains(i - 1) ||
                                idsLíneasTítulosForzados.Contains(i - 1) || idsLíneasParrafosForzados.Contains(i - 1) || idsLíneasFinLista.Contains(i - 1));
                            var tieneBloqueAdelante = (idsLíneasParrafos.Contains(i + 1) || idsLíneasTítulos.Contains(i + 1) ||
                                idsLíneasInicioListaNumérica.Contains(i + 1) || idsLíneasInicioLista.Contains(i + 1) ||
                                idsLíneasTítulosForzados.Contains(i + 1) || idsLíneasParrafosForzados.Contains(i + 1));

                            if (tieneBloqueAdelante || tieneBloqueAtrás) { // Estos elementos 'bloque' generan sus propios saltos de línea, entonces si una línea tiene al menos uno de estos arriba o abajo, no es necesario añadir un salto de línea.

                                if (tieneBloqueAdelante && !tieneBloqueAtrás && string.IsNullOrWhiteSpace(obtenerLínea(i))
                                    && string.IsNullOrWhiteSpace(obtenerLínea(i - 1))) { // Trata el caso especial en el que se agregaron 2 líneas en blanco entre elementos bloque. Como los elementos bloque ya generan su propio salto de línea, estos 3  saltos de línea (2 líneas en blanco) se convierten en 2 saltos de línea por cada elemento bloque y un salto de línea <br /> que se agrega aquí en la segunda línea en blanco.
                                    líneasRespuesta.Add($@"{línea}<br />");
                                    continue;
                                } else {
                                    líneasRespuesta.Add(línea);
                                    continue;
                                }

                            } else {
                                líneasRespuesta.Add($@"{línea}<br />");
                                continue;
                            }

                        }
                        #endregion Saltos de Línea>

                    }

                    #endregion Escritura del HTML>
                    break;

            }

            return líneasRespuesta;

        } // ConvertirALíneasHtml>


        #endregion Conversión y Compilación de HTML desde Textos Planos>



        #region Matemáticas

        public static int RedondearAEntero(double d) => (int)Math.Round(d, 0);

        public static bool Iguales(decimal? número1, decimal? número2, decimal? tolerancia = null) => (número1 != null && número2 != null)
            ? Math.Abs((decimal)número1 - (decimal)número2) < (tolerancia ?? ToleranciaDecimalesPredeterminada) : (número1 == null && número2 == null);

        public static bool Iguales(double? número1, double? número2, double? tolerancia = null) => (número1 != null && número2 != null)
            ? Math.Abs((double)número1 - (double)número2) < (tolerancia ?? ToleranciaDoblesPredeterminada) : (número1 == null && número2 == null);

        #endregion Matemáticas>



        #region Serialización JSON


        public static JsonSerializerOptions ObtenerOpcionesSerialización(Serialización serializacionesEspeciales) {

            var opcionesSerialización = new JsonSerializerOptions();
            if (serializacionesEspeciales.HasFlag(Serialización.DiccionarioClaveEnumeración)) {
                opcionesSerialización.Converters.Add(new FábricaConvertidorDiccionarioClaveEnumeración());
            }
            if (serializacionesEspeciales.HasFlag(Serialización.EnumeraciónEnTexto)) {
                opcionesSerialización.Converters.Add(new JsonStringEnumConverter());
            }
            return opcionesSerialización;

        } // ObtenerOpcionesSerialización>


        /// <summary>
        /// Encapsulación de rápido acceso de JsonSerializer.Serialize(). Necesario para pasarlo a Contexto.OnModelCreating > HasConversion porque 
        /// el método original tiene parámetros opcionales y por esto no es aceptado en HasConversion.
        /// </summary>
        public static string Serializar<T>(T objeto) => JsonSerializer.Serialize(objeto);


        /// <summary>
        /// Encapsulación de rápido acceso de JsonSerializer.Deserialize(). Necesario para pasarlo a Contexto.OnModelCreating > HasConversion porque el 
        /// método original tiene parámetros opcionales y por esto no es aceptado en HasConversion.
        /// Función para clases. Para estructuras usar <see cref="DeserializarEstructura{T}(string)"/>.
        /// </summary>
        public static T? Deserializar<T>(string? json) where T : class => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);


        /// <summary>
        /// Encapsulación de rápido acceso de JsonSerializer.Deserialize().
        /// Función para estructuras. Para clases usar <see cref="Deserializar{T}(string)"/>.
        /// </summary>
        public static T? DeserializarEstructura<T>(string json) where T : struct
            => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);


        /// <summary>
        /// Serialización que permite establecer uno o varios tipos de <paramref name="serialización"/> enlazándolos con el operador |.
        /// </summary>
        public static string Serializar<T>(T objeto, Serialización serialización)
            => JsonSerializer.Serialize(objeto, ObtenerOpcionesSerialización(serialización));


        /// <summary>
        /// Deserializalización que permite establecer uno o varios tipos de <paramref name="serialización"/> enlazándolos con el operador |.
        /// </summary>
        public static T? Deserializar<T>(string json, Serialización serialización) where T : class // Se implementa solo para clases porque es el escenario más común. Si se necesitara para estructuras habría que duplicar la función.
            => (string.IsNullOrEmpty(json)) ? default : JsonSerializer.Deserialize<T>(json, ObtenerOpcionesSerialización(serialización));


        /// <summary>
        /// Convierte un objeto a su representación serializada en JSON y viceversa. Es útil para pasarlo al método HasConversion de una propiedad de tipo especial (como List) en OnModelCreating de un contexto y poder almacenar el objeto en la base de datos.
        /// </summary>
        public static ValueConverter<T, string> ConvertidorJSON<T>() where T : class // La restricción de clase es para poder usar el método Deserializar que exige esta restricción. En términos generales no debe ser problema porque el caso más común de serialización es de clases. Si fuera necesario implementarlo para estructuras habría que duplicar el código. No se declara con tipo T? porque no es el tipo que espera EF Core.
            => new ValueConverter<T, string>(o => Serializar(o), s => Deserializar<T>(s)!); // Se permitirá que los objetos tipo T sean nulos así esto impida la verificación de nulidad. Esto no es problema porque esta función solo es usada internamente por EF Core.


        /// <summary>
        /// Establece la forma de comparar un objeto usando su representación en JSON. Es útil para pasarlo al método Metadata.SetValueComparer de una 
        /// propiedad de tipo especial (como List) en OnModelCreating de un contexto. Si no se establece el comparador no se guardaran los cambios 
        /// realizados en la base de datos.
        /// </summary>
        public static ValueComparer ComparadorJSON<T>() where T : class => new ValueComparer<T>( // La restricción de clase es para poder usar el método Deserializar que exige esta restricción. En términos generales no debe ser problema porque el caso más común de serializaciónes de clases. Si fuera necesario implementarlo para estructuras habría que duplicar el código. No se declara con tipo T? porque no es el tipo que espera EF Core.
                (o1, o2) => Serializar(o1) == Serializar(o2), o => o == null ? 0 : Serializar(o).GetHashCode(StringComparison.InvariantCulture),
                o => Deserializar<T>(Serializar(o))!); // // Se permitirá que los objetos tipo T sean nulos así esto impida la verificación de nulidad. Esto no es problema porque esta función solo es usada internamente por EF Core. Es suficiente para las necesidades actuales aunque es desaconsejado crear comparadores genéricos usando JSON: https://stackoverflow.com/questions/38411221/compare-two-objects-using-serialization-c-sharp. Ver https://stackoverflow.com/questions/44829824/how-to-store-json-in-an-entity-field-with-ef-core/59185869#59185869 y https://stackoverflow.com/questions/53050419/json-serialization-value-conversion-not-tracking-changes-with-ef-core/53051419#53051419.


        #endregion Serialización JSON>



        #region Listas y Diccionarios


        /// <summary>
        /// Agrega a una lista de manera limpia. Se crea la lista si es nula y se agrega el valor.
        /// Para que funcione correctamente la creación si es nula, se debe llamar asignándolo a si mismo, así: lista = lista.Agregar(valor).
        /// </summary>
        /// <param name="lista">Lista a la que se agregará el valor.</param>
        /// <param name="permitirRepetidos">Si es falso y el valor ya está en la lista, no se agregará.</param>
        /// <param name="valor">Valor a agregar a la lista.</param>
        /// <param name="sonIguales">Función que determina la igualdad entre el valor a agregar y los existentes. 
        /// Si no se proporciona, se usa la igualdad predeterminada.</param>
        public static List<V> Agregar<V>(this List<V>? lista, V valor, bool permitirRepetidos = true, Func<V, V, bool>? sonIguales = null) {

            lista ??= new List<V>();
            if (permitirRepetidos) {
                lista.Add(valor);
            } else {

                if (sonIguales != null) {
                    if (!lista.Exists(v => sonIguales(v, valor))) lista.Add(valor);
                } else {
                    if (!lista.Contains(valor)) lista.Add(valor);
                }

            }
            return lista;

        } // Agregar>


        public static List<string> Agregar(this List<string>? lista, string valor, bool permitirRepetidos = true, bool ignorarCapitalización = true) // No se obliga el parámetro ignorarCapitalización porque esta función al tener la misma cantidad de parámetros que la genérica es preferida cuando se usa desde un objeto de tipo List<string>.
            => ignorarCapitalización ? lista.Agregar(valor, permitirRepetidos, FSonIgualesTextosIgnorandoCapitalización) :
                  lista.Agregar(valor, permitirRepetidos, sonIguales: null);


        /// <summary>
        /// Agrega a un diccionario de manera limpia. Se crea el diccionario si es nulo y se actualiza el valor si ya existe en la clave. 
        /// Para que funcione correctamente la creación si es nulo, se debe llamar asignándolo a si mismo así diccionario = diccionario.Agregar(clave, valor).
        /// </summary>
        public static Dictionary<K, V> Agregar<K, V>(this Dictionary<K, V>? diccionario, K clave, V valor, bool sobreescribir = true) where K : notnull {

            diccionario ??= new Dictionary<K, V>();
            if (diccionario.ContainsKey(clave)) {

                if (sobreescribir) {
                    diccionario[clave] = valor; // Por rendimiento de manera predeterminada se sobreescribe. No hay claridad del rendimiento genérico de la función Equals.
                } else {
                    if (!Equals(diccionario[clave], valor)) diccionario[clave] = valor; // Se evita sobreescribir si no es necesario.
                }

            } else {
                diccionario.Add(clave, valor);
            }
            return diccionario;

        } // Agregar>


        /// <summary>
        /// Agrega a un diccionario con valor tipo lista una clave y un nuevo valor para la lista asociada a esta clave.
        /// Se crea el diccionario si es nulo y se agrega el valor a la lista de valores. 
        /// Para que funcione correctamente la creación si es nulo, se debe llamar asignándolo a si mismo, así:
        /// diccionario = diccionario.Agregar(clave, valor).
        /// </summary>
        /// <param name="clave">La clave del elemento que se agregará.</param>
        /// <param name="valor">El valor que se agregará a la lista.</param>
        /// <param name="diccionario">El diccionario de claves y listas.</param>
        /// <param name="permitirRepetidos">Si es verdadadero, no se verifican si los elementos ya existen en la lista, siempre se agregan.</param>
        /// <param name="sonIguales">Función que determina la igualdad entre el valor a agregar y los existentes. 
        /// Si no se proporciona, se usa la igualdad predeterminada.</param>
        public static Dictionary<K, List<V>> Agregar<K, V>(this Dictionary<K, List<V>>? diccionario, K clave, V valor, bool permitirRepetidos = true,
            Func<V, V, bool>? sonIguales = null) where K : notnull {

            diccionario ??= new Dictionary<K, List<V>>();
            if (diccionario.ContainsKey(clave)) {
                diccionario[clave].Agregar(valor, permitirRepetidos, sonIguales);
            } else {
                diccionario.Add(clave, new List<V> { valor });
            }
            return diccionario;

        } // Agregar>


        public static Dictionary<K, List<string>> Agregar<K>(this Dictionary<K, List<string>>? diccionario, K clave, string valor, 
            bool permitirRepetidos = true, bool ignorarCapitalización = true) where K : notnull // No se obliga el parámetro ignorarCapitalización porque esta función, al tener la misma cantidad de parámetros que la genérica, es preferida cuando se usa desde un objeto de tipo Dictionary<string, List<V>>.
                => ignorarCapitalización ? diccionario.Agregar(clave, valor, permitirRepetidos, sonIguales: FSonIgualesTextosIgnorandoCapitalización) 
                       : diccionario.Agregar(clave, valor, permitirRepetidos, sonIguales: null);


        /// <summary>
        /// Agrega a un diccionario de manera limpia, se crea el diccionario si es nulo y se suma el valor si ya existe en la clave. Para poder que 
        /// funcione correctamente la creación si está nulo se debe llamar asignándolo a si mismo así dicc = dicc.AgregarSumando(clave, valor).
        /// </summary>
        public static Dictionary<K, decimal> AgregarSumando<K>(this Dictionary<K, decimal>? diccionario, K clave, decimal valor) where K : notnull {

            diccionario ??= new Dictionary<K, decimal>();
            if (diccionario.ContainsKey(clave)) {
                diccionario[clave] += valor;
            } else {
                diccionario.Add(clave, valor);
            }
            return diccionario;

        } // AgregarSumando>


        /// <summary>
        /// Obtiene el valor asociado a la clave o devuelve nulo si no lo encuentra. Aplicable cuando <typeparamref name="V"/> es un 
        /// objeto/clase. Si se necesita la misma función para valores/estructuras se debe usar 
        /// <see cref="ObtenerValor{K, V}(Dictionary{K, V}, K, bool)"/>.
        /// <paramref name="ignorarCapitalización"/> solo aplica para diccionarios con clave (<typeparamref name="K"/>) de tipo texto.
        /// </summary>
        public static V? ObtenerValorObjeto<K, V>(this Dictionary<K, V> diccionario, K clave, bool ignorarCapitalización = true) where V : class 
            where K : notnull { // Aunque se podría implementar usando un atributo [return: MaybeNull] con el que se indica que el resultado de la función tal vez pueda ser nulo y el tipo devuelto hacerlo V en vez de V?, esto trae un inconveniente porque no se podría devolver nulo cuando no encuentre un ítem, solo se podría devolver default, el cual es nulo para objetos pero es cero para números y esto podría traer comportamientos no deseados porque indicaría que el elemento encontrado fue cero y no nulo. Leer más en https://stackoverflow.com/questions/54593923/nullable-reference-types-with-generic-return-type.

            if ((clave is string claveTexto && diccionario is Dictionary<string,V> diccionarioClaveTexto)) 
                return ObtenerValorObjeto(diccionarioClaveTexto, claveTexto, ignorarCapitalización);

            if (diccionario.ContainsKey(clave)) {
                return diccionario[clave];
            } else {
                return null;
            }

        } // ObtenerValorObjeto>


        /// <summary>
        /// Obtiene el valor en un diccionario con clave en texto permitiendo ignorar la capitalización. Aunque se podría permitir su uso por fuera 
        /// de esta clase, se prefiere no confundir al usuario del código y se le proporciona una función 
        /// <see cref="ObtenerValorObjeto{K, V}(Dictionary{K, V}, K, bool)"/> que llama a esta función cuando se ignora la capitalización y
        /// cuando la clave es texto.
        /// </summary>
        /// <returns></returns>
        private static V? ObtenerValorObjeto<V>(this Dictionary<string, V> diccionario, string clave, bool ignorarCapitalización) where V : class // Aunque se podría pensar poner pública esta función, esto no es aporta nada nuevo porque de todas maneras se tendría que poner el parámetro ignorarCapitalización obligatorio en esta función o agregar cualquier parámetro opcional en la función genérica. Esto sería necesario porque para poder que la función no genérica sea tomada en cuenta sobre la genérica, deben tener la misma cantidad de parámetros, incluso los opcionales. Como no es deseable obligar el parámetro obligatorio en esta función para ser usada externamente y si se va agregar un parámetro cualquiera a la función genérica pues mejor se agrega el ignorarCapitalización y se deja toda la funcionalidad completa allá.
            => diccionario.FirstOrDefault(kv => kv.Key.IgualA(clave, ignorarCapitalización)).Value; // Para las clases el valor default es nulo, entonces se puede usar directamente el resultado de esta función.


        /// <summary>
        /// Obtiene el valor asociado a la clave o devuelve nulo si no lo encuentra. Aplicable cuando <typeparamref name="V"/> es un 
        /// valor/estructura. Si se necesita la misma función para objetos/clases se debe usar 
        /// <see cref="ObtenerValorObjeto{K, V}(Dictionary{K, V}, K, bool)"/>.
        /// <paramref name="ignorarCapitalización"/> solo aplica para diccionarios con clave (<typeparamref name="K"/>) de tipo texto.
        /// </summary>
        public static V? ObtenerValor<K, V>(this Dictionary<K, V> diccionario, K clave, bool ignorarCapitalización = true) where V : struct where K : notnull {

            if ((clave is string claveTexto && diccionario is Dictionary<string, V> diccionarioClaveTexto))
                return ObtenerValor(diccionarioClaveTexto, claveTexto, ignorarCapitalización: ignorarCapitalización);

            if (diccionario.ContainsKey(clave)) {
                return diccionario[clave];
            } else {
                return null;
            }

        } // ObtenerValor>


        /// <summary>
        /// Obtiene el valor en un diccionario con clave en texto permitiendo ignorar la capitalización.
        /// </summary>
        /// <returns></returns>
        private static V? ObtenerValor<V>(this Dictionary<string, V> diccionario, string clave, bool ignorarCapitalización) where V : struct { // Aunque se podría pensar poner pública esta función, esto no es aporta nada nuevo porque de todas maneras se tendría que poner el parámetro ignorarCapitalización obligatorio en esta función o agregar cualquier parámetro opcional en la función genérica. Esto sería necesario porque para poder que la función no genérica sea tomada en cuenta sobre la genérica, deben tener la misma cantidad de parámetros, incluso los opcionales. Como no es deseable obligar el parámetro obligatorio en esta función para ser usada externamente y si se va agregar un parámetro cualquiera a la función genérica pues mejor se agrega el ignorarCapitalización y se deja toda la funcionalidad completa allá.
            var valor = diccionario.FirstOrDefault(kv => kv.Key.IgualA(clave, ignorarCapitalización)).Value; // Para las estructuras el valor default no es nulo.
            return valor.EsValorPredeterminado() ? (V?)null : valor;
        } // ObtenerValor>


        /// <summary>
        /// Al pasar la clave con cualquier capitalización, obtiene la clave en un diccionario con la capitalización que está en él.
        /// </summary>
        public static string? ObtenerClaveCapitalizaciónCorrecta<V>(this Dictionary<string, V> diccionario, string clave) 
            => diccionario.FirstOrDefault(kv => kv.Key.IgualA(clave, ignorarCapitalización: true)).Key;


        /// <summary>
        /// Al pasar el valor con cualquier capitalización, obtiene el valor en una lista con la capitalización que está en él.
        /// </summary>
        public static string? ObtenerValorCapitalizaciónCorrecta(this List<string> lista, string valor) {
            var índice = lista.ObtenerÍndice(valor, ignorarCapitalización: true);
            return índice == -1 ? null : lista[índice];
        } // ObtenerValorCapitalizaciónCorrecta>


        public static bool ContieneClave<V>(this Dictionary<string, V> diccionario, string clave, bool ignorarCapitalización = true) 
            => !diccionario.FirstOrDefault(kv => kv.Key.IgualA(clave, ignorarCapitalización)).Value.EsValorPredeterminado();


        /// <summary>
        /// Devuelve el índice de un texto en una lista. Permite hacer coincidencia sin tener en cuenta la capitalización.
        /// Devuelve  -1 si no se encuentra el texto.
        /// </summary>
        /// <returns></returns>
        public static int ObtenerÍndice(this List<string> lista, string valor, bool ignorarCapitalización = true)
            => lista.FindIndex(t => t.IgualA(valor, ignorarCapitalización));


        /// <summary>
        /// Devuelve verdadero si un texto está en una lista. Permite hacer coincidencia sin tener en cuenta la capitalización.
        /// </summary>
        /// <returns></returns>
        public static bool Existe(this List<string> lista, string valor, bool ignorarCapitalización = true)
            => lista.ObtenerÍndice(valor, ignorarCapitalización) != -1;


        public static List<T> CombinarListas<T>(List<T> lista1, List<T> lista2, List<T>? lista3 = null, List<T>? lista4 = null, List<T>? lista5 = null, 
            List<T>? lista6 = null, List<T>? lista7 = null, List<T>? lista8 = null, List<T>? lista9 = null, List<T>? lista10 = null) {

            var lista = new List<T>(lista1);
            lista.AddRange(lista2);
            if (lista3 != null) lista.AddRange(lista3);
            if (lista4 != null) lista.AddRange(lista4);
            if (lista5 != null) lista.AddRange(lista5);
            if (lista6 != null) lista.AddRange(lista6);
            if (lista7 != null) lista.AddRange(lista7);
            if (lista8 != null) lista.AddRange(lista8);
            if (lista9 != null) lista.AddRange(lista9);
            if (lista10 != null) lista.AddRange(lista10);
            return lista;

        } // CombinarListas>


        /// <summary>
        /// Devuelve la representación de una lista como un texto separado por <paramref name="separador"/> o ninguno con si se establece 
        /// <paramref name="separador"/> vacío. Permite establecer un <paramref name="conector"/> entre el penúltimo y último elemento y
        /// también permite devolver el texto por líneas de cada elemento si se establece <paramref name="multilínea"/> en verdadero.
        /// </summary>
        public static string? ATexto<T>(this List<T> lista, bool multilínea = false, ConectorCoordinante conector = ConectorCoordinante.Y,
            string separador = ", ") {

            if (lista == null || lista.Count == 0) return null;
            if (lista.Count == 1) return lista.SingleOrDefault()?.ToString();

            var texto = new StringBuilder();
            texto.Append(lista[0]);
            for (int i = 1; i < lista.Count - 1; i++) {
                _ = multilínea ? texto.AppendLine(separador) : texto.Append(separador);
                texto.Append(lista[i]);
            }

            string? últimoElemento = lista[^1]?.ToString().AMinúscula(); // lista[^1] = lista[lista.Count - 1].
            var textoConector = conector switch {
                ConectorCoordinante.Y => últimoElemento.EmpiezaPor("i") ? "e" : "y",
                ConectorCoordinante.O => últimoElemento.EmpiezaPor("o") ? "u" : "o",
                ConectorCoordinante.Ni => "ni",
                ConectorCoordinante.Ninguno => "",
            };

            if (string.IsNullOrEmpty(textoConector)) {
                _ = multilínea ? texto.AppendLine(separador) : texto.Append(separador);
            } else {

                _ = multilínea ? texto.AppendLine(" ") : texto.Append(' ');
                if (!string.IsNullOrEmpty(textoConector)) {
                    texto.Append(textoConector);
                    texto.Append(' ');
                }

            }
            texto.Append(lista[^1]);

            return texto.ToString();

        } // ATexto>


        public static string? ATextoEnLíneas<T>(this List<T> lista)
            => lista.ATexto(multilínea: true, conector: ConectorCoordinante.Ninguno, separador: "");


        public static string? ATextoConComas<T>(this List<T> lista, ConectorCoordinante conector = ConectorCoordinante.Y, bool resumir = false) {

            if (resumir) {
                
                var resumenLíneas = ResumirLíneasTexto(lista);
                var palabrasComunes = resumenLíneas.Item1;
                var listaResumida = resumenLíneas.Item2;
                return (string.IsNullOrEmpty(palabrasComunes) ? "" : palabrasComunes + " ") + listaResumida.ATexto(conector: conector, separador: ", ");

            } else {
                return lista.ATexto(conector: conector, separador: ", ");
            }

        } // ATextoConComas>
            

        /// <summary>
        /// Analiza las representaciones de texto de los elementos de la lista, busca las palabras comunes por las que inician todos los elementos y
        /// devuelve una tupla que tiene las palabras comunes y una lista con los elementos sin las palabras comunes. 
        /// Es una función pricipalmente auxiliar para ser usada en <see cref="ATextoConComas{T}(List{T}, ConectorCoordinante, bool)"/>.
        /// </summary>
        /// <returns></returns>
        public static (string, List<string>) ResumirLíneasTexto<T>(List<T> lista, bool ignorarCapitalización = true) {

            if (lista.Count == 0) return ("", new List<string>());

            // 1. Iniciación listaTexto.
            var listaTexto = new List<string>(); 
            foreach (var línea in lista) {

                var textoLínea = línea?.ToString().LimpiarEspacios();
                if (string.IsNullOrEmpty(textoLínea)) {
                    listaTexto.Add(""); // Si algún elemento es nulo, lo reemplaza por cadena vacía para asegurar que sea del tipo List<string>.
                } else {
                    listaTexto.Add(textoLínea);
                }

            }
            if (lista.Count == 1) return ("", listaTexto); // Si la lista tiene un solo elemento, no tiene sentido buscar palabras comunes, se devuelve la misma lista.

            // 2. Obtención de textoPalabrasComunes.
            var palabrasComunes = lista[0]!.ToString().APalabras(); // Inicia con todas las palabras del primer elemento como candidatas a ser palabras comunes.
            foreach (var línea in listaTexto) {

                var palabras = línea.ToString().APalabras();         
                if (palabras.Count == 0) { // Si alguna línea es vacía, no hay palabras comunes.
                    palabrasComunes = new List<string>();
                    break;
                }

                var índicePrimeraNoComún = int.MaxValue;
                for (int i = 0; i < palabras.Count; i++) {

                    if (i == palabrasComunes.Count) break; // Si ya se llegó al límite de palabras comunes, se puede finalizar la comparación de esta línea.
                    if (!palabras[i].IgualA(palabrasComunes[i], ignorarCapitalización)) {
                        índicePrimeraNoComún = i;
                        break;
                    }

                }
                if (índicePrimeraNoComún != int.MaxValue) 
                    palabrasComunes.RemoveRange(índicePrimeraNoComún, palabrasComunes.Count - índicePrimeraNoComún); // Se eliminan todas las palabras desde la no común encontrada en adelante.

            }
            var textoPalabrasComunes = palabrasComunes.ATextoConEspacios() ?? "";

            // 3. Reemplazo de palabras comunes en listaTexto.
            if (!string.IsNullOrEmpty(textoPalabrasComunes)) {
                for (int i = 0; i < listaTexto.Count; i++) {
                    listaTexto[i] = listaTexto[i].Reemplazar(textoPalabrasComunes + " ", "");
                }
            }

            return (textoPalabrasComunes, listaTexto);

        } // ResumirLíneasTexto>


        public static string? ATextoConEspacios<T>(this List<T> lista) => lista.ATexto(conector: ConectorCoordinante.Ninguno, separador: " ");


        public static string? ATextoLíneasHtml<T>(this List<T> lista, bool finalizarLíneasConPunto = true)
            => lista.Count == 0 ? null : 
               $"{lista.ATexto(multilínea: true, conector: ConectorCoordinante.Ninguno, separador: $"{(finalizarLíneasConPunto ? "." : "")}<br />")}" +
               $"{(finalizarLíneasConPunto ? "." : "")}";


        public static string? ATextoListaHtml<T>(this List<T> lista, string etiquetaLista = "ul", bool finalizarLíneasConPunto = true) 
            => lista.Count == 0 ? null : $"<{etiquetaLista}><li>" +
               $"{lista.ATexto(multilínea: true, conector: ConectorCoordinante.Ninguno, separador: $"{(finalizarLíneasConPunto ? "." : "")}</li><li>")}" +
               $"{(finalizarLíneasConPunto ? "." : "")}</li></{etiquetaLista}>";


        #endregion Listas y Diccionarios>



        #region Enumeraciones


        public static IEnumerable<T> ObtenerValores<T>() where T : struct, Enum => Enum.GetValues(typeof(T)).Cast<T>();


        /// <summary>
        /// Obtiene el valor de una enumeración en texto.
        /// </summary>
        public static string AValor<T>(this T enumeración, int? largoForzado = null) where T : struct, Enum {
            var valor = Convert.ChangeType(enumeración, enumeración.GetTypeCode(), CultureInfo.InvariantCulture)?.ToString()!; // El valor de una enumeración no es nulo entonces se puede asgurar que el resultado de ChangeType()?.ToString() no lo será.
            return largoForzado == null ? valor : valor.PadLeft((int)largoForzado, '0');
        } // AValor>


        /// <summary>
        /// Obtiene el valor del atributo Display del valor de la enumeración. Es útil para usar un texto personalizado para cada elemento en una enumeración.
        /// </summary>
        public static string ATexto<T>(this T enumeración) where T : struct, Enum {

            var tipo = enumeración.GetType();
            var textoDirecto = enumeración.ToString();

            string obtenerTexto(string textoDirecto)
                => tipo.GetMember(textoDirecto).Where(x => x.MemberType == MemberTypes.Field && ((FieldInfo)x).FieldType == tipo)
                       .First().GetCustomAttribute<DisplayAttribute>()?.Name ?? textoDirecto;

            if (textoDirecto.Contiene(", ")) {

                var texto = new StringBuilder();
                foreach (var textoDirectoAux in textoDirecto.Split(", ")) {
                    texto.Append($"{obtenerTexto(textoDirectoAux)}, ");
                }
                return texto.ToString()[0..^2];

            } else {
                return obtenerTexto(textoDirecto);
            }

        } // ATexto>


        /// <summary>
        /// Obtiene la enumeración desde un texto que se encuentra en el atributo Display.
        /// </summary>
        public static T ObtenerEnumeraciónDeTexto<T>(string texto, bool ignorarCapitalización = true) where T : struct, Enum {

            foreach (var enumeración in ObtenerValores<T>()) {
                if (enumeración.ATexto().IgualA(texto, ignorarCapitalización)) return enumeración;
            }
            throw new Exception($"No encontrado el texto {texto} en alguno los atributos 'Display' de los valores de la enumeración {typeof(T).Name}"); // Lanza excepción porque en las enumeraciones no se manejan como nullables y no se puede asegurar para este método genérico que la enumeración tenga un valor por defecto. Para devolver un valor por defecto se debe usar la otra función con el parámetro valorSiVacío. No se puede usar parámetro opcional porque a las enumeraciones no suelen aceptar nulos.

        } // ObtenerEnumeraciónDeTexto>


        /// <summary>
        /// Obtiene la enumeración desde un texto que se encuentra en el atributo Display.
        /// </summary>
        public static T ObtenerEnumeraciónDeTexto<T>(string texto, T valorSiVacío, bool ignorarCapitalización = true) where T : struct, Enum {

            foreach (var enumeración in ObtenerValores<T>()) {
                if (enumeración.ATexto().IgualA(texto, ignorarCapitalización)) return enumeración;
            }
            return valorSiVacío;

        } // ObtenerEnumeraciónDeTexto>


        /// <summary>
        /// Un método auxiliar útil para usar en las expresiones switch para evaluar coincidir múltiples valores fácilmente. 
        /// Ver https://stackoverflow.com/a/61774164/8330412.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valor"></param>
        /// <param name="valores"></param>
        /// <returns></returns>
        public static bool EstáEn<T>(this T valor, params T[] valores) => valores.Contains(valor);

        #endregion Enumeraciones>



        #region Entity Framework


        /// <summary>
        /// <para>Método de fácil uso para cargar los datos de una lista que es propiedad de navegación de una <paramref name="entidad"/>.</para> 
        /// <para>Esta función es útil cuando, dependiendo del flujo de la ejecución, en algunas ocasiones se necesitará la lista y en otras no. 
        /// O cuando ya se dispone de la entidad de una consulta anterior. Aunque en este caso el beneficio es menor 
        /// cuando se necesitan para solo lectura, pues la duración de CargarLista es casi igual a la de una consulta de la entidad con 
        /// AsNoTracking + Include.</para>
        /// <para>Cuando se necesita obtener la entidad y su lista para solo lectura la mejor opción en rendimiento es usar AsNoTracking 
        /// + Include (Alrededor del 40% del tiempo de consulta de entidad + CargarLista). Si se necesitan hacer cambios sobre 
        /// las entidades aún sigue siendo mejor opción usar Include (Alrededor del 80% del tiempo de consulta de entidad + CargarLista).</para>
        /// <para>Nota: Los rendimientos son aproximados. Para consultas donde se requiere mucho rendimiento se recomienda hacer las pruebas para
        /// elegir la mejor opción.</para>
        /// </summary>
        /// <typeparam name="E">Tipo de la entidad.</typeparam>
        /// <typeparam name="L">Tipo de la entidad en la lista/propiedad.</typeparam>
        /// <param name="ctx"></param>
        /// <param name="entidad">Entidad a la que se le cargarán los datos de la lista.</param>
        /// <param name="obtenerLista">Función de obtención de la lista (propiedad).</param>
        public static bool CargarLista<E, L>(this DbContext ctx, E entidad, Expression<Func<E, IEnumerable<L>>> obtenerLista) where E : class where L : class {
            ctx.Entry(entidad).Collection(obtenerLista).Load();
            return true; // Solo se usa para para poder usar este método fácilmente en expresiones de switch. 
        } // CargarLista>


        /// <summary>
        /// Igual que CargarLista(entidad, obtenerLista) pero agrega el parámetro <paramref name="obtenerFiltro"/> que permite
        /// filtrar los datos que se deseen cargar.
        /// </summary>
        public static bool CargarLista<E, L>(this DbContext ctx, E entidad, Expression<Func<E, IEnumerable<L>>> obtenerLista,
            Expression<Func<L, bool>> obtenerFiltro) where E : class where L : class {

            ctx.Entry(entidad).Collection(obtenerLista).Query().Where(obtenerFiltro).Load();
            return true; // Solo se usa para para poder usar este método fácilmente en expresiones de switch.

        } // CargarLista>


        /// <summary>
        /// <para>Método de fácil uso para contar los elementos de una lista que es propiedad de navegación de una <paramref name="entidad"/> 
        /// sin necesidad de cargarlos. Es útil en los mismos casos que aplica CargarLista(entidad, obtenerLista) en los que adicionalmente 
        /// solo se necesita la cantidad de elementos. ContarLista se tarda 80% del tiempo de CargarLista.</para>
        /// <para>Nota: Los rendimientos son aproximados. Para consultas donde se requiere mucho rendimiento se recomienda hacer las pruebas para
        /// elegir la mejor opción.</para>
        /// </summary>
        public static int ContarElementos<E, L>(this DbContext ctx, E entidad, Expression<Func<E, IEnumerable<L>>> obtenerLista)
            where E : class where L : class => ctx.Entry(entidad).Collection(obtenerLista).Query().Count();


        /// <summary>
        /// Igual que ContarLista(entidad, obtenerLista) pero agrega el parámetro <paramref name="obtenerFiltro"/> que permite
        /// filtrar los datos que se deseen contar.
        /// </summary>
        public static int ContarLista<E, L>(this DbContext ctx, E entidad, Expression<Func<E, IEnumerable<L>>> obtenerLista,
            Expression<Func<L, bool>> obtenerFiltro) where E : class where L : class
            => ctx.Entry(entidad).Collection(obtenerLista).Query().Where(obtenerFiltro).Count();


        /// <summary>
        /// <para>Método de fácil uso para cargar los datos de una propiedad de navegación de una <paramref name="entidad"/>.</para> 
        /// <para>Esta función es útil cuando, dependiendo del flujo de la ejecución, en algunas ocasiones se necesitará la propiedad
        /// y en otras no. O cuando ya se dispone de la entidad de una consulta anterior. No funciona para entidades de solo lectura. 
        /// En caso que ya se disponga de la entidad y se necesite la propiedad para modificar, si mejora considerablemente el rendimiento 
        /// usar CargarPropiedad porque se puede tardar 60% del tiempo de la consulta completa de la entidad usando Include.</para>
        /// <para>Cuando se necesita obtener la entidad y su propiedad para solo lectura la mejor opción en rendimiento suele ser AsNoTracking 
        /// + Include. Si se necesitan hacer cambios sobre las entidades y siempre se debe cargar la propiedad normalmente es mejor 
        /// opción usar Include.</para>
        /// <para>Nota: Las recomendaciones son generales, no aplicables a todos los casos. Para consultas donde se requiere mucho rendimiento 
        /// se recomienda hacer las pruebas para elegir la mejor opción. En particular se debe tener cuidado de realizar las pruebas con
        /// Include tanto con entidades que tengan esta propiedad como con entidades que tengan esta propiedad en nulo. 
        /// Ver github.com/vixark/SimpleOps.Contexto.ObtenerProducto().</para>
        /// </summary>
        /// <typeparam name="E">Tipo de la entidad.</typeparam>
        /// <typeparam name="F">Tipo de la propiedad.</typeparam>
        /// <param name="ctx"></param>
        /// <param name="entidad">Entidad a la que se le cargará la propiedad de navegación.</param>
        /// <param name="obtenerPropiedad">Función de obtención de propiedad.</param>
        public static bool CargarPropiedad<E, F>(this DbContext ctx, E entidad, Expression<Func<E, F?>> obtenerPropiedad)
            where E : class where F : class {

            ctx.Entry(entidad).Reference(obtenerPropiedad).Load();
            return true; // Solo se usa para para poder usar este método fácilmente en expresiones de switch. 

        } // CargarPropiedad>


        /// <summary>
        /// Obtiene la representación en texto de una entidad que puede ser vacía y que se provee un identificador para usarlo cuando sea vacía.
        /// </summary>
        public static string ATexto<T>(T? entidad, int? identificador) where T : class
            => entidad?.ToString() ?? (identificador == null ? "" : $"{typeof(T).Name}ID: {identificador}");


        public static string? ObtenerNombreTabla<T>(this DbSet<T> dbSet) where T : class { // Ver https://dev.to/j_sakamoto/how-to-get-the-actual-table-name-from-dbset-in-entityframework-core-20-56k0.

            var contexto = dbSet.ObtenerContexto();
            if (contexto == null) return null;
            var modelo = contexto.Model;
            var tiposEntidades = modelo.GetEntityTypes();
            var tipoEntidad = tiposEntidades.First(t => t.ClrType == typeof(T));
            var anotaciónTabla = tipoEntidad.GetAnnotation("Relational:TableName");
            var nombreTabla = anotaciónTabla.Value.ToString();
            return nombreTabla;

        } // ObtenerNombreTabla>


        public static DbContext? ObtenerContexto<T>(this DbSet<T> dbSet) where T : class { // Ver https://dev.to/j_sakamoto/how-to-get-a-dbcontext-from-a-dbset-in-entityframework-core-c6m.

            var infraestructura = dbSet as IInfrastructure<IServiceProvider>;
            var proveedorServicios = infraestructura.Instance;
            var contextoActual = proveedorServicios.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            return contextoActual?.Context;

        } // ObtenerContexto>


        public static string? ObtenerNombreTabla(string nombreEntidad, DbContext contexto) {

            var modelo = contexto.Model;
            var tiposEntidades = modelo.GetEntityTypes();
            var tipoEntidad = tiposEntidades.First(t => t.ClrType.Name == nombreEntidad);
            var anotaciónTabla = tipoEntidad.GetAnnotation("Relational:TableName");
            var nombreTabla = anotaciónTabla.Value.ToString();
            return nombreTabla;

        } // ObtenerNombreTabla>


        #region Where con Or - Ver https://github.com/dotnet/efcore/issues/15670.


        /// <summary>
        /// Obtiene una expresión que permite usar el conector lógico O entre las expresiones del <paramref name="predicado"/>. 
        /// Para que funcione correctamente este predicado debe tener por lo menos 2 expresiones individuales. Si solo tiene 1 
        /// se devuelve esa expresión sin cambios.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicado"></param>
        /// <param name="conector"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ObtenerExpresión<T>(Predicado<T> predicado, ConectorLógico conector) {

            var expresiones = predicado.Lista;
            if (expresiones.Count == 0) throw new Exception("El predicado no tiene expresiones.");
            if (expresiones.Count == 1) return expresiones[0];

            var expresiónDelParámetro = expresiones.FirstOrDefault()?.Parameters.FirstOrDefault();
            if (expresiónDelParámetro == null) throw new ArgumentException("El nombre del parámetro es inválido.");

            var modificador = new ModificadorExpresión(expresiónDelParámetro);
            var expresiónConConector = expresiones.Aggregate<System.Linq.Expressions.Expression>((primeraExpresión, segundaExpresión) => {

                var expresión1 = primeraExpresión switch {
                    LambdaExpression le => modificador.Modificar((le.Body as BinaryExpression)!),
                    BinaryExpression be => be,
                    _ => throw new ArgumentException("Los argumentos no pueden ser nulos."),
                };

                var cuerpo2 = (segundaExpresión as LambdaExpression)?.Body;
                if (cuerpo2 == null) throw new ArgumentException("Los argumentos no pueden ser nulos.");
                var expresión2 = modificador.Modificar((cuerpo2 as BinaryExpression)!);

                return conector switch {
                    ConectorLógico.Y => System.Linq.Expressions.Expression.AndAlso(expresión1, expresión2),
                    ConectorLógico.O => System.Linq.Expressions.Expression.OrElse(expresión1, expresión2),
                };

            });

            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(expresiónConConector, expresiónDelParámetro);

        } // ObtenerExpresión>


        /// <summary>
        /// Versión personalizada del Where que permite hacer la consulta usando un predicado personalizado que puede incluir el operador lógico O
        /// entre sus expresiones individuales.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consulta"></param>
        /// <param name="predicado"></param>
        /// <param name="conector"></param>
        /// <returns></returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> consulta, Predicado<T> predicado, ConectorLógico conector) {

            var expresiones = predicado.Lista;
            if (expresiones.Count == 0) return consulta;
            return consulta.Where(ObtenerExpresión(predicado, conector));

        } // Where>


        /// <summary>
        /// Clase necesaria para ObtenerExpresión().
        /// </summary>
        private class ModificadorExpresión : ExpressionVisitor {

            private readonly ParameterExpression ExpresiónDelParámetro;
            public ModificadorExpresión(ParameterExpression expresiónDelParámetro) => ExpresiónDelParámetro = expresiónDelParámetro;
            public System.Linq.Expressions.Expression Modificar(System.Linq.Expressions.Expression expresión) => Visit(expresión);
            protected override System.Linq.Expressions.Expression VisitParameter(ParameterExpression node) => ExpresiónDelParámetro;

        } // ModificadorExpresión>


        /// <summary>
        /// Predicado personalizado que facilita la adición de nuevas expresiones a la lista con el método Agregar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Predicado<T> {

            internal readonly List<Expression<Func<T, bool>>> Lista = new List<Expression<Func<T, bool>>>();
            public void Agregar(Expression<Func<T, bool>> predicado) => Lista.Add(predicado);

        } // Predicado>

        
        #endregion Where con Or>


        /// <summary>
        /// Aplica una conversión intermedia para todas las propiedades de tipo <typeparamref name="T"/> a <typeparamref name="B"/> de todas las entidades.
        /// Si no existe una conversión predeterminada se puede pasar un <paramref name="convertidor"/>. Los valores de las 
        /// propiedades se guardan en la base de datos usando el tipo <typeparamref name="B"/>. En el modelo las propiedades se siguen usando 
        /// con el tipo <typeparamref name="T"/>.
        /// </summary>
        public static ModelBuilder UsarConvertidorGlobal<T, B>(this ModelBuilder constructor, ValueConverter? convertidor = null) { // Ver https://github.com/dotnet/efcore/issues/10784.

            foreach (var tipoEntidad in constructor.Model.GetEntityTypes()) {

                var infoPropiedades = tipoEntidad.ClrType.GetProperties().Where(p => p.PropertyType == typeof(T) && p.GetSetMethod() != null);
                foreach (var infoPropiedad in infoPropiedades) {

                    if (convertidor == null) {
                        constructor.Entity(tipoEntidad.Name).Property(infoPropiedad.Name).HasConversion<B>();
                    } else {
                        constructor.Entity(tipoEntidad.Name).Property(infoPropiedad.Name).HasConversion(convertidor);
                    }

                }

            }
            return constructor;

        } // UsarConvertidorGlobal>


        #endregion Entity Framework>



        #region SQL


        /// <summary>
        /// Inserta masivamente una lista de entidades en una tabla (<paramref name="nombreTabla"/>) de una base de datos SQL con conexión 
        /// <paramref name="textoConexión"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nombreTabla"></param>
        /// <param name="lista"></param>
        /// <param name="error"></param>
        /// <param name="columnasAOmitir"></param>
        /// <param name="textoConexión"></param>
        /// <returns></returns>
        public static bool InsertarEnBaseDeDatosSQL<T>(List<T> lista, string nombreTabla, string textoConexión, out string error,
            List<string>? columnasAOmitir = null) {

            if (lista.Count == 0) { error = ""; return true; }
            var éxito = true;

            using var tabla = lista.ATabla(nombreTabla, columnasAOmitir);
            using var conexión = new SqlConnection(textoConexión);
            conexión.Open();
            using var insertadorMasivo = new SqlBulkCopy(conexión) { DestinationTableName = tabla.TableName };
            foreach (DataColumn? col in tabla.Columns) {
                if (col != null) insertadorMasivo.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            }

            try {

                insertadorMasivo.WriteToServer(tabla);
                error = "";

            #pragma warning disable CA1031 // No capture tipos de excepción generales. Se acepta porque aún no está completamente controlada la excepción.
            } catch (Exception ex) {
            #pragma warning restore CA1031

                error = ex.Message;
                SuspenderEjecuciónEnModoDesarrollo();
                éxito = false; // Un posible error es que las descripciones sean más largas de lo que soportan las columnas. Es preferible que saque error acá porque es algo que se debe controlar.

            }

            return éxito;

        } // InsertarEnBaseDeDatosSQL>


        /// <summary>
        /// Convierte una lista de un objeto a una tabla con el valor de sus propiedades. Necesaria para la función InsertarEnBaseDeDatosSQL().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lista"></param>
        /// <param name="nombreTabla"></param>
        /// <param name="columnasAOmitir"></param>
        /// <returns></returns>
        public static DataTable ATabla<T>(this IList<T> lista, string nombreTabla, List<string>? columnasAOmitir) { // De https://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable.

            var tabla = new DataTable(nombreTabla);
            var propiedades = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var tiposPropiedadesVálidos = new List<string> { "String", "Estado", "Int32", "Decimal", "FuentePrecio", "TipoPágina", "Byte", "DateTime", "Boolean" };
            var índiceColumnasAñadidas = new List<int>();
            if (columnasAOmitir == null) columnasAOmitir = new List<string>();

            var i = 0;
            foreach (var propiedad in propiedades) {

                if (tiposPropiedadesVálidos.Contains(propiedad.PropertyType.Name) && !columnasAOmitir.Contains(propiedad.PropertyType.Name)
                    && !Attribute.IsDefined(propiedad, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute))) {

                    tabla.Columns.Add(propiedad.Name, propiedad.PropertyType);
                    índiceColumnasAñadidas.Add(i);

                }
                i++;

            }

            var valores = new object[índiceColumnasAñadidas.Count];
            foreach (var elemento in lista) {

                var j = 0;
                foreach (var índiceColumnaAñadida in índiceColumnasAñadidas) {
                    if (elemento != null) valores[j] = propiedades[índiceColumnaAñadida].GetValue(elemento)!;
                    j++;
                }
                tabla.Rows.Add(valores);

            }
            return tabla;

        } // ATabla>


        #endregion SQL>



        #region Genéricos


        public static bool EsValorPredeterminado<T>(this T valor) => EqualityComparer<T>.Default.Equals(valor, default); // Ver https://stackoverflow.com/questions/65351/null-or-default-comparison-of-generic-argument-in-c-sharp.


        /// <summary>
        /// <para>Función genérica que ayuda a implementar una caché en propiedades que su cálculo sea costoso.
        /// Permite automáticamente realizar el cálculo en el primer acceso a la propiedad y mientras el objeto exista
        /// se puede acceder al valor de la propiedad sin realizar nuevamente el cálculo. Esta propiedad es un híbrido entre una propiedad
        /// de solo lectura autocalculada, que se calcula siempre que se accede, y una propiedad normal, que solo almacena el valor.</para>
        /// <para>Esta función se debe llamar en el Get de una propiedad que use campo de respaldo (no autoimplementada), así:</para>
        /// <para>
        /// private decimal? _Propiedad;<br/> public decimal Propiedad { get => SiNulo(ref _Propiedad, () => CálculoDeNombrePropiedad()); 
        /// set => _APagar = value; }
        /// </para>
        /// <para>La función calcular devuelve verdadero si el cálculo es 
        /// exitoso, esto se usa para devolver el valor por defecto si no se pudo calcular en el momento.
        /// Si se necesita la misma función para objetos/clases se debe usar <see cref="SiNulo{T}(ref T, Func{bool})"/>.</para>
        /// </summary>
        public static T SiNulo<T>(ref T? valor, Func<bool> calcular) where T : struct { // Se pasa como referencia para poder que se tome su valor actualizado después de CalcularTodo().

            if (valor == null) {

                if (calcular()) {
                    return (T)valor!; // Después de calcular todo se asegura que ya no es nulo.
                } else {
                    return default; // Para las estructuras el default no es nulo, si es número es 0.
                }

            } else {
                return (T)valor;
            }

        } // SiNulo>


        /// <summary>
        /// Igual que <see cref="SiNulo{T}(ref T?, Func{bool})"/> pero para objetos/clases.
        /// En este caso si es posible que existan valores nulos para la propiedad cuando el cálculo
        /// de esta no se pudo realizar. Mientras no se pueda realizar el cálculo lo intentará
        /// cada vez que se acceda la propiedad.
        /// </summary>
        public static T? SiNulo<T>(ref T? valor, Func<bool> calcular) where T : class {

            if (valor == null) {

                if (calcular()) {
                    return valor!; // Después de calcular todo se asegura que ya no es nulo.
                } else {
                    return null;
                }

            } else {
                return valor;
            }

        } // SiNulo>


        #endregion Genéricos>



        #region Copias de Objetos


        /// <summary>
        /// Copia las propiedades aplicables de <paramref name="objetoOrigen"/> a <paramref name="objetoDestino"/>. 
        /// Se puede usar para copiar propiedades entre objetos del mismo tipo y entre objetos de tipos heredados entre si.
        /// Devuelve verdadero si alguna de las propiedades fue cambiada, si no devuelve falso.
        /// </summary>
        public static bool CopiarA<T>(this object objetoOrigen, ref T? objetoDestino) where T : class, new() {

            if (objetoDestino == null) objetoDestino = new T(); // Si el objetoDestino es null crea un nueva instancia.
            return CopiarA(objetoOrigen, objetoDestino);

        } // CopiarA>


        /// <summary>
        /// Versión de CopiarA usando objects en los casos que no se puede usar la versión genérica porque el objetoDestino es de solo lectura y 
        /// no se puede pasar con ref.
        /// </summary>
        public static bool CopiarA(this object objetoOrigen, object objetoDestino) {

            if (objetoDestino == null) return false;

            var modificado = false;
            var tipoOrigen = objetoOrigen.GetType();
            var tipoDestino = objetoDestino.GetType();
            var propiedadesOrigen = tipoOrigen.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propiedadesDestino = tipoDestino.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var origenEsHijoDeDestino = false;
            if (!tipoDestino.Equals(tipoOrigen) && !tipoDestino.IsSubclassOf(tipoOrigen)) {

                if (tipoOrigen.IsSubclassOf(tipoDestino)) {
                    origenEsHijoDeDestino = true;
                } else {
                    throw new ArgumentException($"{tipoDestino} y {tipoOrigen} no son de tipos heredados entre si."); // Si se da esta excepción puede ser porque alguna propiedad de una entidad se ha declarado virtual lo que causa que EF Core cree una entidad auxiliar. Para obtener la clase de la entidad real de la entidad auxiliar leer: https://stackoverflow.com/questions/25770369/get-underlying-entity-object-from-entity-framework-proxy. La solución es, una vez identificado que se trata de este caso, manejarlo como tipoOrigen.IsSubclassOf(tipoDestino).
                }

            }

            foreach (var infoPropiedadOrigen in propiedadesOrigen.Where(p => p.GetSetMethod() != null)) { // Ignora las propiedades a las que no se le puede acceder el método Set() ya sea porque no existe o porque es privado. Es preferible usar esta en vez de p.CanWrite según https://stackoverflow.com/questions/3390358/using-reflection-how-do-i-detect-properties-that-have-setters.

                PropertyInfo? infoPropiedadDestino;
                if (origenEsHijoDeDestino) {
                    infoPropiedadDestino = propiedadesDestino.SingleOrDefault(p => p.Name == infoPropiedadOrigen.Name && p.GetSetMethod() != null);
                    if (infoPropiedadDestino == null) continue; // Si una propiedad de origen (hijo) no se encuentra en destino (padre) es ignorada. Es común que suceda porque las clases subtipo de otras suelen agregar nuevas propiedades.
                } else { // En el caso que sean el mismo tipo de objeto o que objetoOrigen sea padre de objetoDestino, todas sus propiedades se pueden copiar directamente en objetoDestino y se puede reusar en destino el mismo objeto infoPropiedad de origen.
                    infoPropiedadDestino = infoPropiedadOrigen;
                }

                var valorOrigen = infoPropiedadOrigen.GetValue(objetoOrigen);
                var valorAnterior = infoPropiedadDestino.GetValue(objetoDestino);
                var cambióValor = valorAnterior == null ? valorOrigen != null : !valorAnterior.Equals(valorOrigen); // Compara exactamente el valor del objeto, si uno es nulo y el otro no devuelve verdadero, aquí usualmente solo van a aparecer objetos de valor porque se trata usualmente de propiedades.
                if (cambióValor) infoPropiedadDestino.SetValue(objetoDestino, valorOrigen);
                if (modificado || cambióValor) modificado = true;

            }

            return modificado;

        } // CopiarA>


        #endregion Copias de Objetos>



        #region Criptografía


        public static string ObtenerSHA384(string texto, bool eliminarGuiones = true) {

            using var calculador = new SHA384CryptoServiceProvider();
            var sha384 = calculador.ComputeHash(Encoding.UTF8.GetBytes(texto)).ATexto();
            return eliminarGuiones ? sha384.Reemplazar("-", "") : sha384;

        } // ObtenerSHA384>


        public static string ObtenerSHA256(string texto, bool eliminarGuiones = true) {

            using var calculador = new SHA256CryptoServiceProvider();
            var sha256 = calculador.ComputeHash(Encoding.UTF8.GetBytes(texto)).ATexto();
            return eliminarGuiones ? sha256.Reemplazar("-", "") : sha256;

        } // ObtenerSHA256>


        #endregion Criptografía>



        #region Interfaz


        public static MessageBoxResult MostrarDiálogo(string? mensaje, string título, MessageBoxButton botones = MessageBoxButton.OK,
            MessageBoxImage imagen = MessageBoxImage.None) => MessageBox.Show(mensaje, título, botones, imagen); // Este debe ser el único MessageBox.Show en todo el código.


        public static MessageBoxResult MostrarError(string? mensaje) => MostrarDiálogo(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error); // Por estandarización de la interfaz de los diálogos no se permite la personalización del título de estos diálogos. Si se necesita un diálogo con título especial usar MostrarDiálogo.


        public static MessageBoxResult MostrarInformación(string? mensaje) 
            => MostrarDiálogo(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information); // Por estandarización de la interfaz de los diálogos no se permite la personalización del título de estos diálogos. Si se necesita un diálogo con título especial usar MostrarDiálogo.


        public static MessageBoxResult MostrarÉxito(string? mensaje) => MostrarDiálogo(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information); // Por estandarización de la interfaz de los diálogos no se permite la personalización del título de estos diálogos. Si se necesita un diálogo con título especial usar MostrarDiálogo.


        public static MessageBoxResult MostrarAdvertencia(string? mensaje) 
            => MostrarDiálogo(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning); // Por estandarización de la interfaz de los diálogos no se permite la personalización del título de estos diálogos. Si se necesita un diálogo con título especial usar MostrarDiálogo.


        #endregion Interfaz>



        #region Depuración


        public static void SuspenderEjecuciónEnModoDesarrollo() {

            #if DEBUG
                Debugger.Break(); 
            #endif

        } // SuspenderEjecuciónEnModoDesarrollo>


        #endregion Depuración>


        
    } // Vixark>



} // SimpleOps>
