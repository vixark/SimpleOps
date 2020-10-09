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

        public enum ConectorCoordinante { Y = 0, O = 1, Ni = 2 } // https://gramaticalobasico.blogspot.com/2010/01/conectores-coordinantes.html.

        public enum ConectorLógico { Y = 0, O = 1 }

        [Flags] public enum Serialización { EnumeraciónEnTexto = 1, DiccionarioClaveEnumeración = 2 } // Se puede establecer una o varias serializaciones especiales con el operador |.

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
                { "cotización", (Género.Femenino, NúmeroSustantivo.Singular) },
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

        public static NumberFormatInfo FormatoPesosColombianos = null!;  // Se inicia en IniciarVariablesGenerales.


        /// <summary>
        /// Inicia algunas variables que no se pueden iniciar en el cuerpo de la clase Vixark porque requieren unos pasos adicionales para establecer su valor.
        /// </summary>
        public static void IniciarVariablesGenerales() {

            DobleLínea = Environment.NewLine + Environment.NewLine;
            NuevaLínea = Environment.NewLine;
            FormatoPesosColombianos = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            FormatoPesosColombianos.NumberGroupSeparator = " "; // Otros espacios más pequeños que se podrían utilizar: \u200A y \u202F.
            FormatoPesosColombianos.NumberDecimalDigits = 0;

        } // IniciarVariablesGenerales>


        #endregion Variables y Constantes>


        #region Textos y Excepciones

        public static string CasoNoConsiderado(string? valor) => $"No se ha considerado el caso para el valor {valor ?? "nulo"}.";

        public static string CasoNoConsiderado<T>(T enumeración) where T : struct, Enum
            => $"No se ha considerado el valor {enumeración} para la enumeración {enumeración.GetType()}.";

        #endregion Textos y Excepciones>


        #region Métodos y Funciones


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

            return xmlRespuesta;

        } // ObtenerXml>


        #endregion Web>


        #region Archivos y Carpetas


        /// <summary>
        /// Obtiene la ruta de una carpeta y provee la opción de crearla si no existe.
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


        /// <summary>
        /// Devuelve una ruta del archivo con extensión diferente. La <paramref name="nuevaExtensión"/> se pasa sin el punto.
        /// </summary>
        public static string ObtenerRutaCambiandoExtensión(string ruta, string nuevaExtensión)
            => $"{Path.Combine(Path.GetDirectoryName(ruta)!, Path.GetFileNameWithoutExtension(ruta))}.{nuevaExtensión}";

        /// <summary>
        /// Devuelve una ruta del archivo manteniendo la extensión pero agregando un  <paramref name="textoAdicional"/> al nombre del archivo.
        /// </summary>
        public static string ObtenerRutaAgregandoTexto(string ruta, string textoAdicional)
            => $"{Path.Combine(Path.GetDirectoryName(ruta)!, Path.GetFileNameWithoutExtension(ruta) + textoAdicional)}{Path.GetExtension(ruta)}";

        /// <summary>
        /// Abre un archivo en Windows.
        /// </summary>
        /// <param name="rutaArchivo"></param>
        public static void AbrirArchivo(string rutaArchivo) => Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });


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


        public static string ObtenerHashArchivo(string rutaArchivo) {

            using var FileCheck = File.OpenRead(rutaArchivo);
            #pragma warning disable CA5351 // No usar algoritmos criptográficos dañados. Es aceptable porque solo es para identificar el archivo.
            using var md5 = new MD5CryptoServiceProvider();
            #pragma warning restore CA5351
            var md5Hash = md5.ComputeHash(FileCheck);
            return BitConverter.ToString(md5Hash).Reemplazar("-", "").AMinúscula()!;

        } // ObtenerHashArchivo>


        #endregion Archivos y Carpetas>


        #region Compresión


        /// <summary>
        /// Obtiene un vector de bytes con la información de un archivo ZIP obtenido del empaquetado del archivo en <paramref name="ruta"/>. 
        /// </summary>
        /// <returns></returns>
        public static byte[] ObtenerBytesZip(string ruta) { // Tomado de https://stackoverflow.com/a/30068762/8330412.

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


        #region Imagenes


        public static string ObtenerBase64(string rutaImagen, bool paraHtml) {

            var base64 = Convert.ToBase64String(File.ReadAllBytes(rutaImagen));
            if (paraHtml) {

                var extensión = Path.GetExtension(rutaImagen).AMinúscula();
                return extensión switch {
                    ".png" => "data:image/png;base64," + base64,
                    var e when e.EstáEn(".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi") => "data:image/jpeg;base64," + base64, // Posibles extensiones tomadas de https://blog.filestack.com/thoughts-and-knowledge/complete-image-file-extension-list/#jpg.
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


        #endregion


        #region Textos, Formatos y Patrones


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


        public static string ExtraerConPatrón(string valor, string patrón, int grupo, out int coincidencias, bool errorEnNoCoincidenciaDePatrón = true,
            bool devolverValorSiNoHayCoincidencia = false) {

            coincidencias = 0;
            if (string.IsNullOrEmpty(patrón)) {
                return valor;
            } else {

                if (!string.IsNullOrEmpty(valor)) {

                    var regex = new Regex(patrón, RegexOptions.IgnoreCase); // En terminos generales no se necesita diferenciar mayúsculas de minúsculas. Si fuera necesario se podría pasar estas opciones vía parámetros opcionales.
                    var coincidenciasObj = regex.Match(valor);
                    coincidencias = coincidenciasObj.Captures.Count;
                    if (coincidencias > 0) {
                        valor = coincidenciasObj.Groups[grupo].Value;
                    } else {

                        if (errorEnNoCoincidenciaDePatrón)
                            throw new Exception("Expresión regular sin coincidencias. Alternativas de solución: Arreglar expresión regular, propagar error o ignorar " +
                                " con errorEnNoCoincidenciaDePatrón = false.");
                        if (!devolverValorSiNoHayCoincidencia) valor = "";

                    }

                }

            }

            return valor;

        } // ExtraerConPatrón>


        public static bool CoincideConPatrón(string valor, string patrón) {

            ExtraerConPatrón(valor, patrón, 0, out int coincidencias, errorEnNoCoincidenciaDePatrón: false);
            return coincidencias > 0;

        } // CoincideConPatrón>


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
        /// Función muy rápida de conversión de texto a entero. No realiza verificaciones de errores, ni control de nulidad para máximizar la velocidad.
        /// </summary>
        public static int AEntero(this string texto) { // El método más rápido encontrado. Para 1 000 000 de conversiones en Debug da 15 ms TryParse, 18 ms Parse, 18 ms Convert.ToInt32() y 32 ms este código personalizado. Sin embargo, en release los resultados cambian mucho, para 10 000 000 conversiones se obtiene 60 ms para este código personalizado y 150 ms para TryParse. Código de https://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int.

            var y = 0;
            for (int i = 0; i < texto.Length; i++) {
                y = y * 10 + (texto[i] - '0');
            }
            return y;

        } // AEntero>


        /// <summary>
        /// Función muy rápida de conversión de texto a entero. No realiza verificaciones de errores.
        /// </summary>
        public static int AEntero(this char carácter) => (carácter - '0');


        /// <summary>
        /// Devuelve la representación de una lista en un texto separado por comas y un <paramref name="conector"/> entre el penúltimo y último elemento.
        /// </summary>
        public static string? ATexto<T>(this List<T> lista, ConectorCoordinante conector = ConectorCoordinante.Y) {

            if (lista == null || lista.Count == 0) return null;
            if (lista.Count == 1) return lista.SingleOrDefault()?.ToString();

            var cTexto = new StringBuilder();
            cTexto.Append(lista[0]);
            for (int i = 1; i < lista.Count - 1; i++) {
                cTexto.Append(", ");
                cTexto.Append(lista[i]);
            }
            string? últimoElemento = lista[^1]?.ToString().AMinúscula(); // lista[^1] = lista[lista.Count - 1].
            var textoConector = conector switch {
                ConectorCoordinante.Y => últimoElemento.EmpiezaPor("i") ? "e" : "y",
                ConectorCoordinante.O => últimoElemento.EmpiezaPor("o") ? "u" : "o",
                ConectorCoordinante.Ni => "ni",
                _ => throw new Exception(CasoNoConsiderado(conector))
            };
            cTexto.Append(' ');
            cTexto.Append(textoConector);
            cTexto.Append(' ');
            cTexto.Append(lista[^1]);

            return cTexto.ToString();

        } // ATexto>


        public static SizeF ObtenerTamañoTexto(string? texto, Font fuente, int ancho) {

            if (string.IsNullOrEmpty(texto)) texto = " ";
            SizeF respuesta;
            using var imagen = new Bitmap(1, 1);
            using var gráficos = Graphics.FromImage(imagen);
            gráficos.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            respuesta = gráficos.MeasureString(texto, fuente, ancho, StringFormat.GenericTypographic); // Ver razón en https://stackoverflow.com/questions/1203087/why-is-graphics-measurestring-returning-a-higher-than-expected-number.
            return respuesta;

        } // ObtenerTamañoTexto>


        #endregion Textos, Formatos y Patrones>


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
        public static T? DeserializarEstructura<T>(string json) where T : struct => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);

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
                o => Deserializar<T>(Serializar(o))!); // // Se permitirá que los objetos tipo T sean nulos así esto impida la verificación de nulidad. Esto no es problema porque esta función solo es usada internamente por EF Core. Es suficiente para las necesidades actuales aunque es desaconsejado crear comparadores genéricos usando JSON: https://stackoverflow.com/questions/38411221/compare-two-objects-using-serialization-c-sharp. Tomado de https://stackoverflow.com/questions/44829824/how-to-store-json-in-an-entity-field-with-ef-core/59185869#59185869 y https://stackoverflow.com/questions/53050419/json-serialization-value-conversion-not-tracking-changes-with-ef-core/53051419#53051419.

        #endregion Serialización JSON>


        #region Encapsulaciones Varias

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
        /// Encapsulación de rápido acceso de la negación de Contains() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código y para facilitar la lectura.
        /// </summary>
        public static bool NoContiene(this string texto, string textoContenido, bool ignorarCapitalización = true)
            => !texto.Contains(textoContenido, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        /// <summary>
        /// Encapsulación de rápido de Replace() usando StringComparison.Ordinal. Es útil para omitir la advertencia CA1307 sin saturar el código.
        /// </summary>
        public static string Reemplazar(this string texto, string anteriorTexto, string? nuevoTexto, bool ignorarCapitalización = true)
            => texto.Replace(anteriorTexto, nuevoTexto, ignorarCapitalización ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this DateTime fechaHora, string formato) => fechaHora.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string? ATexto(this DateTime? fechaHora, string formato)
            => fechaHora == null ? null : ((DateTime)fechaHora).ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de acceso rápido para convertir un <paramref name="texto"/> en una fecha dado un <paramref name="formato"/>.
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="formato"></param>
        /// <returns></returns>
        public static DateTime? AFecha(this string? texto, string formato)
            => texto == null ? (DateTime?)null : DateTime.ParseExact(texto, formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de IndexOf() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static int ÍndiceDe(this string texto, string textoAEncontrar, int índiceInicial)
            => texto.IndexOf(textoAEncontrar, índiceInicial, StringComparison.Ordinal);

        /// <summary>
        /// Encapsulación de acceso rápido para convertir un texto en una enumeración.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static T AEnumeración<T>(this string texto) where T : struct, Enum => (T)Enum.Parse(typeof(T), texto);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this int número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string? ATexto(this int? número) => número == null ? null : ((int)número).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this int número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this decimal número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this decimal número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this long número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this char carácter) => carácter.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this double número) => número.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this double número, string formato) => número.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToString() usando CultureInfo.InvariantCulture. Es útil para omitir la advertencia CA1305 sin saturar el código.
        /// </summary>
        public static string ATexto(this TimeSpan intervalo, string formato) => intervalo.ToString(formato, CultureInfo.InvariantCulture);

        /// <summary>
        /// Encapsulación de rápido acceso de ToLowerInvariant(). Es útil para omitir la advertencia CA1308 sin saturar el código.
        /// No se puede crear un método con el mismo nombre para string (sin ?) porque el compilador no lo permite. Si se necesitara 
        /// se podría hacer otro método con otro nombre. Una solución fácil es usar este método con un string y poner ! después de () 
        /// para informarle al compilador que se asegura que el resultado no será nulo.
        /// </summary>
#pragma warning disable CA1308 // Normalizar las cadenas en mayúsculas
        public static string? AMinúscula(this string? texto) => texto?.ToLowerInvariant();
#pragma warning restore CA1308 // Normalizar las cadenas en mayúsculas

        public static string ATexto(this bool booleano) => booleano ? "true" : "false";

        public static string ASíONo(this bool booleano) => booleano ? "Sí" : "No";

        public static string ATexto(this byte[] bytes) => BitConverter.ToString(bytes).AMinúscula()!; // Se asegura que nunca es nulo porque así el vector sea vacío devuelve una cadena vacía no nula.

        public static string ATextoDinero(this decimal número, bool agregarMoneda = true)
            => $"{número.ToString("#,0", FormatoPesosColombianos)}{(agregarMoneda ? " $" : "")}";

        /// <summary>
        /// Usa Convert.ToDecimal que permite mantener las posiciones decimales del texto en el decimal, así estas sean 00. Por ejemplo, convierte "158.0100" en 158.0100.
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static decimal ADecimal(this string texto) => Convert.ToDecimal(texto, CultureInfo.InvariantCulture);


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


        /// <summary>
        /// Una función auxiliar útil para los métodos que requieren devolver un valor booleano que representa si fueron exitosos o no y además establecer 
        /// un <paramref name="texto"/> de respuesta en la variable <paramref name="mensaje"/>.
        /// </summary>
        public static bool Falso(out string? mensaje, string? texto) {
            mensaje = texto;
            return false;
        } // Falso>


        public static bool Iguales(decimal? número1, decimal? número2, decimal? tolerancia = null) => (número1 != null && número2 != null)
            ? Math.Abs((decimal)número1 - (decimal)número2) < (tolerancia ?? ToleranciaDecimalesPredeterminada) : (número1 == null && número2 == null);

        public static bool Iguales(double? número1, double? número2, double? tolerancia = null) => (número1 != null && número2 != null)
            ? Math.Abs((double)número1 - (double)número2) < (tolerancia ?? ToleranciaDoblesPredeterminada) : (número1 == null && número2 == null);


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


        #endregion Encapsulaciones Varias>


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
        /// <typeparam name="T"></typeparam>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static T ObtenerEnumeraciónDeTexto<T>(string texto) where T : struct, Enum {

            foreach (var enumeración in ObtenerValores<T>()) {
                if (enumeración.ATexto() == texto) return enumeración;
            }
            throw new Exception($"No encontrado el texto {texto} en alguno los atributos 'Display' de los valores de la enumeración {typeof(T).Name}"); // Lanza excepción porque en las enumeraciones no se manejan como nullables y no se puede asegurar para este método genérico que la enumeración tenga un valor por defecto. Si se quiere implementar un valor por defecto habría que pasarlo como parámetro en otra función similar pero con el parámetro (no se podría usar parámetro opcional por la misma razón que a las enumeraciones no suelen aceptar nulos).

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
        /// y en otras no. O cuando ya se dispone de la entidad de una consulta anterior. Aunque si se necesita para solo lectura
        /// el beneficio puede ser más en facilidad de uso y mantenimiento que de rendimiento porque la duración de CargarPropiedad 
        /// puede llegar a ser 30% mayor que la de una consulta de la entidad con AsNoTracking + Include. En caso que ya se disponga 
        /// de la entidad y se necesite la propiedad para modificar si mejora considerablemente el rendimiento usar CargarPropiedad 
        /// porque se puede tardar 60% del tiempo de la consulta de entidad usando Include.</para>
        /// <para>Cuando se necesita obtener la entidad y su propiedad para solo lectura la mejor opción en rendimiento es usar AsNoTracking 
        /// + Include (39% del tiempo de consulta de entidad + CargarPropiedad). Si se necesitan hacer cambios sobre las entidades aún sigue siendo mejor 
        /// opción usar Include (90% del tiempo de consulta de entidad + CargarPropiedad).</para>
        /// <para>Nota: Los rendimientos son aproximados. Para consultas donde se requiere mucho rendimiento se recomienda hacer las pruebas para
        /// elegir la mejor opción.</para>
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


        public static string? ObtenerNombreTabla<T>(this DbSet<T> dbSet) where T : class { // Tomado de https://dev.to/j_sakamoto/how-to-get-the-actual-table-name-from-dbset-in-entityframework-core-20-56k0.

            var contexto = dbSet.ObtenerContexto();
            if (contexto == null) return null;
            var modelo = contexto.Model;
            var tiposEntidades = modelo.GetEntityTypes();
            var tipoEntidad = tiposEntidades.First(t => t.ClrType == typeof(T));
            var anotaciónTabla = tipoEntidad.GetAnnotation("Relational:TableName");
            var nombreTabla = anotaciónTabla.Value.ToString();
            return nombreTabla;

        } // ObtenerNombreTabla>


        public static DbContext? ObtenerContexto<T>(this DbSet<T> dbSet) where T : class { // Tomado de https://dev.to/j_sakamoto/how-to-get-a-dbcontext-from-a-dbset-in-entityframework-core-c6m.

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


        #region Where con Or - Tomado de https://github.com/dotnet/efcore/issues/15670.


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
                    _ => throw new Exception(CasoNoConsiderado(conector)),
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
        public static ModelBuilder UsarConvertidorGlobal<T, B>(this ModelBuilder constructor, ValueConverter? convertidor = null) { // Tomado de https://github.com/dotnet/efcore/issues/10784.

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

        /// <summary>
        /// Agrega a un diccionario de manera limpia, se crea el diccionario si es nulo y se actualiza el valor si ya existe en la clave. 
        /// Para que funcione correctamente la creación si es nulo se debe llamar asignándolo a si mismo así diccionario = diccionario.Agregar(clave, valor).
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
        /// objeto/clase. Si se necesita la misma función para valores/estructuras se debe usar <see cref="ObtenerValor{K, V}(Dictionary{K, V}, K)"/>.
        /// </summary>
        public static V? ObtenerValorObjeto<K, V>(this Dictionary<K, V> diccionario, K clave) where V : class where K : notnull { // Aunque se podría implementar usando un atributo [return: MaybeNull] con el que se indica que el resultado de la función tal vez pueda ser nulo y el tipo devuelto hacerlo V en vez de V?, esto trae un inconveniente porque no se podría devolver nulo cuando no encuentre un ítem, solo se podría devolver default, el cual es nulo para objetos pero es cero para números y esto podría traer comportamientos no deseados porque indicaría que el elemento encontrado fue cero y no nulo. Leer más en https://stackoverflow.com/questions/54593923/nullable-reference-types-with-generic-return-type.

            if (diccionario.ContainsKey(clave)) {
                return diccionario[clave];
            } else {
                return null;
            }

        } // ObtenerValorObjeto>


        /// <summary>
        /// Obtiene el valor asociado a la clave o devuelve nulo si no lo encuentra. Aplicable cuando <typeparamref name="V"/> es un 
        /// valor/estructura. Si se necesita la misma función para objetos/clases se debe usar <see cref="ObtenerValorObjeto{K, V}(Dictionary{K, V}, K)"/>.
        /// </summary>
        public static V? ObtenerValor<K, V>(this Dictionary<K, V> diccionario, K clave) where V : struct where K : notnull {

            if (diccionario.ContainsKey(clave)) {
                return diccionario[clave];
            } else {
                return null;
            }

        } // ObtenerValor>


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

        public static MessageBoxResult MostrarDiálogo(string? mensaje, string? título, MessageBoxButton botones = MessageBoxButton.OK,
            MessageBoxImage imagen = MessageBoxImage.None) => MessageBox.Show(mensaje, título, botones, imagen); // Este debe ser el único MessageBox.Show en todo el código.

        public static MessageBoxResult MostrarError(string? mensaje, string? título = "Error")
            => MostrarDiálogo(mensaje, título, MessageBoxButton.OK, MessageBoxImage.Error);

        public static MessageBoxResult MostrarInformación(string? mensaje, string? título = "Información")
            => MostrarDiálogo(mensaje, título, MessageBoxButton.OK, MessageBoxImage.Information);

        public static MessageBoxResult MostrarAlerta(string? mensaje, string? título = "Alerta")
            => MostrarDiálogo(mensaje, título, MessageBoxButton.OK, MessageBoxImage.Warning);

        #endregion Interfaz>


        #region Depuración


        public static void SuspenderEjecuciónEnModoDesarrollo() {

            #if DEBUG
            Debugger.Break();
            #endif

        } // SuspenderEjecuciónEnModoDesarrollo>


        #endregion Depuración>


        #endregion Métodos y Funciones>


    } // Vixark>



} // SimpleOps>
