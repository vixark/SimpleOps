﻿// Copyright Notice:
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

using SimpleOps.Datos;
using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static Vixark.General;
using System.IO;
using SimpleOps.Singleton;
using AutoMapper;
using SimpleOps.DocumentosGráficos;
using static SimpleOps.Configuración;
using SimpleOps.Legal;
using static SimpleOps.Legal.Dian;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps {



    /// <summary>
    /// Métodos, funciones y constantes estáticas de uso global para todo SimpleOps. Se diferencian de Vixark.General porque son propias del funcionamiento de SimpleOps y no son de uso general compartido con otros proyectos.
    /// </summary>
    static class Global {



        #region Constantes
        // Configuraciones del comportamiento de SimpleOps que solo se pueden hacer desde este código. No estarán accesibles a los usuarios en Opciones. Las configuraciones que requieran ser modificadas más frecuentemente por los usuarios del código deben ir en Configuración.cs para evitar generarles conflictos de sincronización desde el repositorio cuando se realicen cambios en este archivo Global.cs.

        public const string NombreAplicación = "SimpleOps";

        public const string NombreAplicaciónRegistrada = "SimpleOps®"; // Al cambiarlo aquí también se debe cambiar en MarcoPdf.cshtml > AnuncioAplicación().

        public const string WebAplicación = "SimpleOps.net"; // Al cambiarlo aquí también se debe cambiar en MarcoPdf.cshtml > AnuncioAplicación().

        public const string DirecciónWebAplicación = "https://simpleops.net"; // Al cambiarlo aquí también se debe cambiar en MarcoPdf.cshtml > AnuncioAplicación().

        public const string CarpetaDatosJson = "Datos JSON";

        public const string CarpetaCopiasSeguridad = "Copias de Seguridad";

        public const string NombreBaseDatosSQLite = "Datos";

        public const string CarpetaLibrerías = "Librerías";

        public const string CarpetaLicencias = "Licencias"; // Carpeta que va dentro de CarpetaLibrerías y contiene las licencias de las librerías en ella.

        public const string CarpetaDatos = "Datos";

        public const string CarpetaDatosDesarrollo = "Datos"; // Nombre de la carpeta en el repositorio que contiene Contexto.cs, Datos [Vacía].db (que se actualiza manualmente cada vez que hay un cambio en la estructura de la base de datos) y la carpeta de los datos JSON, ver CarpetaDatosJsonDesarrollo. 

        public const string CarpetaDatosJsonDesarrollo = "Datos JSON"; // En esta carpeta se proveen datos iniciales comunes a todos los usuarios para iniciar automáticamente su propia base de datos (Datos.db) si esta no se encuentra, principalmente contiene Municipios.json.

        public const string CarpetaOpciones = "Opciones";

        public const string CarpetaPlantillas = "Plantillas Documentos";

        public const string CarpetaCopiasSeguridadPlantillas = "Copias de Seguridad"; // Está dentro de CarpetaPlantillas.

        public const string CarpetaImágenesPlantillas = "Imágenes"; // Tiene el mismo nombre para el modo desarrollo y producción.

        public const string CarpetaProductos = "Productos"; // Tiene el mismo nombre para el modo desarrollo y producción.

        public const string CarpetaProductosImágenes = "Imágenes"; // Está dentro de la CarpetaProductos y tiene el mismo nombre para el modo desarrollo y producción.

        public const string CarpetaProductosInformación = "Información"; // Está dentro de la CarpetaProductos y tiene el mismo nombre para el modo desarrollo y producción.

        public const string CarpetaProductosInformaciónImágenes = "Imágenes"; // Está dentro de la CarpetaProductosInformación y tiene el mismo nombre para el modo desarrollo y producción.

        public const string CarpetaProductosInformaciónCompilados = "Compilados"; // Está dentro de la CarpetaProductosInformación y tiene el mismo nombre para el modo desarrollo y producción. En esta carpeta se almacenan los HTMLs compilados con información de de cada producto, que se pueden subir al servidor del sitio web o al servicio de CDN.

        public const string CarpetaProductosInformaciónFragmentos = "Fragmentos"; // Está dentro de la CarpetaProductosInformación y tiene el mismo nombre para el modo desarrollo y producción. En esta carpeta se almacenan los textos planos o HTMLs con fragmentos de información que se pueden reutilizar en los archivos de información de los productos. Estos fragmentos se pueden cargar desde cada archivo de información con <object data="ArchivoAInsertar.html" data-tipoproducto="camiseta" data-marca="Gato"> para archivos de información en HTML y {TipoProducto=camiseta}{Marca=Gato}ArchivoAInsertar.txt para archivos de información en texto plano. Además, estos fragmentos permiten el uso de variables; siguiendo con el ejemplo anterior, si dentro del fragmento se agrega {marca}, este texto será reemplazado por Gato.

        public const string CarpetaPlantillasDesarrollo = "Plantillas";

        public const string CarpetaDocumentosElectrónicos = "Documentos Electrónicos";

        public const string CarpetaCotizaciones = "Cotizaciones"; // Carpeta donde se almacena el PDF de las cotizaciones y catálogos realizados.

        public const string ArchivoLogoEmpresa = "LogoEmpresa.png"; // El nombre del archivo del logo 2 y 3 se forman agregando 2 y 3 antes de la extensión de este nombre.

        public const string ArchivoLogoExcel = "LogoExcel.png";

        public const string ArchivoLogoEmpresaImpresión = "LogoEmpresaImpresión.png"; // El nombre del archivo del logo 2 y 3 se forman agregando 2 y 3 antes de la extensión de este nombre.

        public const string ArchivoLogoExcelImpresión = "LogoExcelImpresión.png";

        public const string ArchivoCertificadoEmpresa = "Certificado.png";

        public const string ArchivoCertificadoEmpresaImpresión = "CertificadoImpresión.png";

        public const string ArchivoImagenProductoNoDisponible = "ImagenNoDisponible.jpg";

        public static string TextoPruebas = AleatorizarTexto("Nlulyhkv'jvu'ZptwslVwz'o{{wA66zptwslvwz5ul{"); // Texto auxiliar aleatorio para algunas pruebas.

        public const string TipoAtributoProductoLibre = "Libre"; // Se usa en algunos diccionarios, principalmente de manera interna. Si se agrega un tipo de atributo a la tabla TiposAtributosProductos con este nombre tanto los atributos clasificados con ese tipo como atributos libres que no están en la tabla AtributosProductos tendrán tipo de atributo el valor de TipoAtributoProductoLibre.

        public const string PrefijoFacturasPredeterminado = ""; // Al menos uno de los documentos puede estar sin prefijo para evitar colisiones en los nombres de archivos, entonces se elije que sea la factura. Se permite para darle la flexibilidad al usuario final que sus facturas no lleven prefijo. En el caso de los otros documentos no se le está quitando flexibilidad al usuario porque la DIAN exige prefijos para las notas crédito y débito.

        public const string PrefijoNotasDébitoPredeterminado = "ND"; 

        public const string PrefijoNotasCréditoPredeterminado = "NC";

        #endregion Constantes>



        #region Variables Constantes

        public static OpcionesEmpresa Empresa = OpcionesEmpresa.Datos; // Variable auxiliar para almacenar el objeto singleton OpcionesEmpresa.Datos y poderlo acceder más fácilmente. Se inicia la variable singleton OpcionesEmpresa.Datos con alias global Empresa de la clase OpcionesEmpresa para que no sea iniciada en otro lugar. Aunque eso no debería generar problemas se prefiere evitarlo y tener esta variable lista lo más pronto posible en la ejecución. Los valores iniciales son temporales pues son sobreescritos con los valores en Empresa.json en CargarOpciones().

        public static OpcionesGenerales Generales = OpcionesGenerales.Datos; // Variable auxiliar para almacenar el objeto singleton OpcionesGenerales.Datos y poderlo acceder más fácilmente. Se inicia la variable singleton OpcionesGenerales.Datos con alias global Generales de la clase OpcionesGenerales para que no sea iniciada en otro lugar. Aunque eso no debería generar problemas se prefiere evitarlo y tener esta variable lista lo más pronto posible en la ejecución. Los valores iniciales son temporales pues son sobreescritos con los valores en GeneralesPredeterminados.json y GeneralesPropios.json en CargarOpciones().

        public static OpcionesEquipo Equipo = OpcionesEquipo.Datos; // Variable auxiliar para almacenar el objeto singleton OpcionesEquipo.Datos y poderlo acceder más fácilmente. Se inicia la variable singleton OpcionesEquipo.Datos con alias global Equipo de la clase OpcionesEquipo para que no sea iniciada en otro lugar. Aunque eso no debería generar problemas se prefiere evitarlo y tener esta variable lista lo más pronto posible en la ejecución. Los valores iniciales son temporales pues son sobreescritos con los valores en Equipo.json en CargarOpciones().

        #endregion Variables Constantes>



        #region Estados
        // Variables que pueden cambiar durante la ejecución.

        public static bool OperacionesEspecialesDatos = true; // Se establece en falso al iniciar y así se habilita la generación de excepciones al asignar datos inválidos al modelo. Es verdadero durante migraciones de Entity Framework y durante la carga inicial de datos para evitar que haga algunas verificaciones y que lance errores.

        public static Usuario UsuarioActual = new Usuario("David", "david@simpleops.net") { ID = 1 }; // Temporalmente mientras se implementa se usará usuario 1. Al implementar usuarios lo debería obtener de la base de datos.


        private static bool _ModoDesarrolloPlantillas = false;
        public static bool ModoDesarrolloPlantillas { // Se usa verdadero para permitir que los cambios que se hagan a los archivos CSHTML en la carpeta Plantillas sean copiados a la ruta de la aplicación y para habilitar algunas líneas de código que facilitan el desarrollo de estas plantillas. En producción se deben usar directamente los archivos en la ruta de la aplicación porque no se tienen los de desarrollo. Se usa cómo una variable de estado porque al ser algo más relacionado con el desarrollo y no la operación normal de parte de un usuario no vale la pena generar cadenas de parámetros para llevar este valor de una manera más segura (sin problemas de estado) hasta las funciones que lo usan.

            get => _ModoDesarrolloPlantillas;

            set {

                _ModoDesarrolloPlantillas = value;
                if (_ModoDesarrolloPlantillas) {

                    var rutaDesarrollo = ObtenerRutaPlantillas(forzarRutaDesarrollo: true);
                    var rutaAplicación = ObtenerRutaPlantillas(forzarRutaAplicación: true);
                    var rutaCopiasSeguridad = ObtenerRutaCarpeta(rutaAplicación, CarpetaCopiasSeguridadPlantillas, crearSiNoExiste: true);
                    var rutaCopiaSeguridadHoy = ObtenerRutaCarpeta(rutaCopiasSeguridad, FechaActualNombresArchivos, crearSiNoExiste: true);
                    var cantidadArchivosCopiados = CopiarArchivos(rutaAplicación, rutaCopiaSeguridadHoy, sobreescribir: false, "cshtml");
                    MostrarInformación($"Activado el modo de desarrollo de plantillas.{DobleLínea}Este modo suspende la ejecución del código después de " +
                        $"crear cada archivo PDF. Permite la edición de los archivos CSHTML durante esta suspensión y al reanudar genera " +
                        $"nuevamente el mismo archivo PDF con los cambios realizados en el CSHTML. Esto facilita la edición y desarrollo de archivos " +
                        $"CSHTML.{DobleLínea}Los archivos de plantillas CSHTML serán copiados de '{rutaDesarrollo}' a '{rutaAplicación}' cada vez que " +
                        $"se genere un documento." + (cantidadArchivosCopiados == 0 ? "" : $"{DobleLínea}Para evitar la pérdida de datos accidental, " +
                        $"se ha hecho una copia de seguridad de los archivos CSHTML en '{rutaAplicación}' en '{rutaCopiaSeguridadHoy}'."));

                }

            }

        } // ModoDesarrolloPlantillas>


        #endregion Estados>



        #region Variables Calculadas en Inicio
        // Variables que son constantes durante la ejecución pero se calculan al iniciar SimpleOps.

        public static List<int> MunicipiosConMensajería = new List<int>(); // Se actualiza al iniciar la aplicación y al realizar cambios en la tabla municipios.

        public static Dictionary<string, string> AtributosProductosYTipos = new Dictionary<string, string>(); // La clave es cada uno de los atributos en la base de datos y el valor es su tipo. Se actualiza al iniciar la aplicación y al realizar cambios en las tablas AtributosProductos o TiposAtributosProductos. Se agregan todos los posibles valores de atributos de producto que serán relacionados con su tipo para filtros u otros usos, por rendimiento en esas aplicaciones se prefiere tener esta variable en caché. Los atributos toman estos valores preferiblemente, pero también pueden tomar valores libres, esto da flexibilidad de manejo al usuario del código y de la aplicación. También es usado para las personalizaciones.

        public static SortedDictionary<int, string> ÍndicesYAtributos = new SortedDictionary<int, string>(); // La clave es el ID de cada atributo en la base de datos. Se usa para tener una lista ordenada y poder obtener textos que denoten los rangos de atributos secuenciales, así: Talla 10 a 15. No se usa AtributosProductosYTipos para esto porque según la documentación los diccionarios no garantizan el orden de inserción de los elementos, aunque experimentalmente algunas personas dicen que si se mantiene si solo se realizan inserciones, se prefiere no correr el riesgo. Ver https://stackoverflow.com/questions/2722767/order-preserving-data-structures-in-c-sharp.

        public static Dictionary<int, int> ÍndicesRangosTallasMediasNuméricas = new Dictionary<int, int>();

        public static Dictionary<int, int> ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica = new Dictionary<int, int>();

        public static MapperConfiguration ConfiguraciónMapeadorEmpresa = new MapperConfiguration(c => c.CreateMap<OpcionesEmpresa, DatosEmpresa>());

        public static iText.Html2pdf.ConverterProperties OpcionesConversiónPdf = new iText.Html2pdf.ConverterProperties(); // Se termina de configurar en IniciarVariablesGlobales().


        public static MapperConfiguration ConfiguraciónMapeadorVenta
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaVenta, DatosLíneaProducto>();
                c.CreateMap<Venta, DatosVenta>().ForMember(vg => vg.CódigoDocumento, mce => mce.MapFrom(v => v.Código));
                c.CreateMap<Cliente, DatosCliente>();
                c.CreateMap<Usuario, DatosUsuario>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorVentaIntegración
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaVenta, Integración.DatosLíneaProducto>();
                c.CreateMap<Venta, Integración.DatosVenta>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorVentaIntegraciónInverso
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaVenta, Integración.DatosLíneaProducto>().ReverseMap();
                c.CreateMap<Venta, Integración.DatosVenta>().ReverseMap();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaCréditoVenta // Se crean mapeadores propios para las notas crédito para evitar complejizar con objetos genéricos estos objetos de AutoMapper.
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaCréditoVenta, DatosLíneaProducto>();
                c.CreateMap<NotaCréditoVenta, DatosVenta>().ForMember(vg => vg.CódigoDocumento, mce => mce.MapFrom(v => v.Código));
                c.CreateMap<Cliente, DatosCliente>();
                c.CreateMap<Usuario, DatosUsuario>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaDébitoVenta // Se crean mapeadores propios para las notas débito para evitar complejizar con objetos genéricos estos objetos de AutoMapper.
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaDébitoVenta, DatosLíneaProducto>();
                c.CreateMap<NotaDébitoVenta, DatosVenta>().ForMember(vg => vg.CódigoDocumento, mce => mce.MapFrom(v => v.Código));
                c.CreateMap<Cliente, DatosCliente>();
                c.CreateMap<Usuario, DatosUsuario>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaCréditoVentaIntegración
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaCréditoVenta, Integración.DatosLíneaProducto>();
                c.CreateMap<NotaCréditoVenta, Integración.DatosNotaCrédito>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaCréditoVentaIntegraciónInverso
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaCréditoVenta, Integración.DatosLíneaProducto>().ReverseMap();
                c.CreateMap<NotaCréditoVenta, Integración.DatosNotaCrédito>().ReverseMap();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaDébitoVentaIntegración
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaDébitoVenta, Integración.DatosLíneaProducto>();
                c.CreateMap<NotaDébitoVenta, Integración.DatosNotaDébito>();
            });


        public static MapperConfiguration ConfiguraciónMapeadorNotaDébitoVentaIntegraciónInverso
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaNotaDébitoVenta, Integración.DatosLíneaProducto>().ReverseMap();
                c.CreateMap<NotaDébitoVenta, Integración.DatosNotaDébito>().ReverseMap();
            });


        public static MapperConfiguration ConfiguraciónMapeadorCotizaciónIntegración
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaCotización, Integración.DatosLíneaProducto>();
                c.CreateMap<Cotización, Integración.DatosCotización>().ForMember(dcz => dcz.FechaHora, m => m.MapFrom(cz => cz.FechaHoraCreación));
            });


        public static MapperConfiguration ConfiguraciónMapeadorCotizaciónIntegraciónInverso
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaCotización, Integración.DatosLíneaProducto>().ReverseMap();
                c.CreateMap<Cotización, Integración.DatosCotización>().ReverseMap()
                    .ForMember(cz => cz.FechaHoraCreación, m => m.MapFrom(dcz => dcz.FechaHora));
            });


        public static MapperConfiguration ConfiguraciónMapeadorCotización
            = new MapperConfiguration(c => {
                c.CreateMap<LíneaCotización, DatosLíneaProducto>().ForMember(dlp => dlp.PrecioBaseTexto, m => m.MapFrom(lc => lc.PrecioTexto));
                c.CreateMap<LíneaCotización, DatosLíneaProducto>().ForMember(dlp => dlp.PrecioBase, m => m.MapFrom(lc => lc.Precio));
                c.CreateMap<Cotización, DatosCotización>().ForMember(dcz => dcz.CódigoDocumento, m => m.MapFrom(cz => cz.Código))
                    .ForMember(dc => dc.FechaHora, m => m.MapFrom(cz => cz.FechaHoraCreación));
                c.CreateMap<Cliente, DatosCliente>();
                c.CreateMap<Usuario, DatosUsuario>();
            });


        #endregion Variables Calculadas en Inicio>



        #region Textos y Excepciones

        public const string ExcepciónSQLiteFallóRestricciónÚnica = "SQLite Error 19: 'UNIQUE constraint failed"; // No se cierra la comilla sencilla porque sigue texto que es único por cada tabla.

        #endregion Textos y Excepciones>



        #region Enumeraciones 
        // Si la enumeración se usa en una propiedad de la base de datos, se debe declarar byte si es posible y debe llevar los valores numéricos. Si se agregan nuevos elementos, se debe agregar el detalle a la documentación de todas las propiedades de entidades de estos tipos de enumeraciones. Se hacen explícitos los valores para tener conciencia de estos y reducir la posibilidad de crear errores con los datos en la base de datos al añadir nuevos elementos. Cualquier nuevo elemento deberá ser añadido al final de la enumeración a no ser que hayan números libres intermedios. El primer elemento casi siempre debe ser Desconocido que es el equivalente en función a nulo. Agregar las enumeraciones que caben en una línea al principio y las múltilínea después.

        public enum TipoEntidad : byte { Desconocido = 0, Empresa = 1, Persona = 2 } // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Identificador de tipo de organización jurídica. Los valores deben ser coincidentes con numeral 13.2.3 de la guía de facturación electrónica de la DIAN. 1 Persona jurídica y asimiladas, 2 Persona natural. Usar Desconocido para personas anónimas de las que no se tiene la cédula.

        public enum TipoCliente : byte { Desconocido = 0, Consumidor = 1, Distribuidor = 2, GrandesContratos = 3, Otro = 255 } // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Nuevos elementos se añaden antes de Otro. Cada vez que se añada un elemento se deben agregar los elementos necesarios en TipoClienteFormaEntrega.

        public enum Prioridad : byte { Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50 } // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Ninguna es necesario cuando no existe ninguna prioridad para el elemento y no debe ser tenido en cuenta. Si fuera necesario nuevos elementos de prioridad intermedia se pueden añadir entre los valores existentes.

        public enum EstadoFactura : byte { PendientePago = 0, Pagada = 1, Anulada = 2 }

        public enum EstadoSolicitudProducto : byte { Pendiente = 0, Cumplida = 1, Anulada = 2 }

        public enum EstadoRemisión : byte { PendienteFacturación = 0, Facturada = 1, Anulada = 2, Descartada = 3 }

        public enum EstadoMovimientoDinero : byte { Pendiente = 0, Procesado = 1, Dividido = 2, Anulado = 3 }

        public enum EstadoComprobanteDinero : byte { Realizado = 0, ReportadoContabilidad = 1, Anulado = 2, AnuladoReportado = 3 }

        public enum TipoCuentaBancaria : byte { Desconocida = 0, Ahorros = 1, Corriente = 2 }

        [Flags] public enum TipoPermiso { Ninguno = -1, Lectura = 1, Modificación = 2, Inserción = 4, Eliminación = 8 }

        public enum TipoMovimientoDinero { Ninguno, Ingreso, Egreso }

        public enum LugarMovimientoDinero : byte { Desconocido = 0, Banco = 1, Caja = 2 }

        public enum TipoCobro : byte { Desconocido = 0, Email = 1, Telefónico = 2, Personal = 3, AgenciaDeCobros = 4, Prejurídico = 5, Jurídico = 6, Otro = 255 } // Nuevos elementos se añaden antes de Otro.

        public enum ModoImpuesto { Exento, Porcentaje, Unitario }

        public enum ControlConcurrencia { Ninguno, Optimista, Pesimista, NoPermitido }

        public enum TipoContexto { Lectura, Escritura, LecturaConRastreo }; // Se prefieren los términos Lectura y Escritura porque describen claramente la intención más común al crear el contexto y equivalen a NoTracking y Tracking respectivamente. Se añade LecturaConRastreo porque para poder usar la caché de las consultas se requiere activar el tracking entonces para mejorar el rendimiento de algunas consultas de solo lectura podrían iniciarse con TipoContexto.LecturaConRastreo. Esto es principalmente es para facilidad de revisión y lectura del código y para lanzar una excepción cuando se intente guardar cambios en contextos que sean de tipo lectura.

        public enum BrochaInformativa { Éxito, Peligro, Alerta, Información, Indiferente }; // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON.

        public enum TamañoLetra { XS, S, M, L, XL }

        public enum TipoImpuesto { IVA, INC, [Display(Name = "IVA e INC")] IVAeINC, [Display(Name = "No aplica")] NoAplica }; // Tomados de la tabla 13.2.6.2. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN.

        public enum FormaPago { Contado = 1, Crédito = 2 } // Tomados del numeral 13.3.4.1 de la documentación de la DIAN para la facturación electrónica.

        public enum TipoFacturaVenta { Venta = 1, Exportación = 2, ContingenciaFacturador = 3, ContingenciaDian = 4 };

        public enum TipoDescuento { Comercial = 0, Condicionado = 1 }; // Tomados del numeral 13.3.7 de la documentación de la DIAN para la facturación electrónica.

        public enum TipoDeclarante { Desconocido, Declarante, NoDeclarante }

        public enum TipoProducto { Desconocido, Producto, Servicio };

        public enum TipoFirma {[Display(Name = "fv")] Factura, [Display(Name = "nd")] NotaDébito, [Display(Name = "nc")] NotaCrédito, Evento }

        public enum TipoReglaDian { Rechazo, Notificación }

        public enum TipoCotización { Cotización, Catálogo }

        public enum AmbienteFacturaciónElectrónica { Producción = 1, Pruebas = 2 }; // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Tomados del numeral 13.1.1 de la documentación de la DIAN para la facturación electrónica: 1 Producción, 2 Pruebas. Se debe mantener el valor de cada enumeración igual al código en la tabla de la DIAN.

        public enum RazónNotaDébito { Intereses = 1, Gastos = 2, AjustePrecio = 3, Otra = 4 } // Tomados del archivo '13.2.5. Concepto de Corrección para Notas débito cac DiscrepancyResponsecbc ResponseCode.xlsx' de la documentación de la DIAN para la facturación electrónica.


        public enum FormaEntrega : byte { // Virtual es útil para productos o servicios que se proveen sin necesidad de representación o presencia física. Cada vez que se añada un elemento se deben agregar los elementos necesarios en TipoClienteFormaEntrega.
            Desconocida = 0, Virtual = 1, PuntoVenta = 2, Mensajería = 3, Transportadora = 4, TransportadoraInternacional = 5, Otra = 255
        }


        public enum TipoClienteFormaEntrega { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Enumeración auxiliar para permitir la serialización del objeto OpcionesEmpresa.MínimosTransporteGratis. Al agregar un elemento aquí también agregarlo en OpcionesEmpresa.MínimosTransporteGratis y a ObtenerMínimoTransporteGratis().
            Desconocido_Desconocida, Consumidor_Desconocida, Distribuidor_Desconocida, GrandesContratos_Desconocida, Otro_Desconocida, // FormaEntrega = Desconocida.
            Desconocido_Virtual, Consumidor_Virtual, Distribuidor_Virtual, GrandesContratos_Virtual, Otro_Virtual, // FormaEntrega = Virtual.
            Desconocido_PuntoVenta, Consumidor_PuntoVenta, Distribuidor_PuntoVenta, GrandesContratos_PuntoVenta, Otro_PuntoVenta, // FormaEntrega = PuntoVenta.
            Desconocido_Mensajería, Consumidor_Mensajería, Distribuidor_Mensajería, GrandesContratos_Mensajería, Otro_Mensajería, // FormaEntrega = Mensajería.
            Desconocido_Transportadora, Consumidor_Transportadora, Distribuidor_Transportadora, GrandesContratos_Transportadora, Otro_Transportadora, // FormaEntrega = Transportadora.
            Desconocido_TransportadoraInternacional, Consumidor_TransportadoraInternacional, Distribuidor_TransportadoraInternacional,
            GrandesContratos_TransportadoraInternacional, Otro_TransportadoraInternacional, // FormaEntrega = TransportadoraInternacional.
            Desconocido_Otra, Consumidor_Otra, Distribuidor_Otra, GrandesContratos_Otra, Otro_Otra, // FormaEntrega = Otra.
        }


        public enum EstadoOrdenCompra { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Lista: Ya está lista para facturación porque todos sus productos están en inventario y no hay problemas con las cuentas del cliente ni el tamaño de la orden. EsperandoProducto: Se está esperando que llegue producto pedido a un proveedor. EsperandoPago: Es de un cliente sin crédito y se está esperando su reporte de pago para poder facturar. CupoCréditoInsuficiente: Es de un cliente con crédito que alcanzó su cupo de crédito. BajoMonto: Su valor es menor al establecido en opciones. FacturasVencidas: Es de un cliente con crédito que tiene facturas vencidas sin pagar (Se usa vencidas aunque en la práctica son las vencidas después de cierto umbral porque comercialmente es aceptado vender a clientes con facturas moderadamente vencidas). PendientePedido: Al menos un producto de la orden de compra no está en inventario ni está en camino, se debe pedir a un proveedor.  
            Lista = 0, EsperandoProducto = 1, EsperandoPago = 2, CupoCréditoAlcanzado = 3, BajoMonto = 4, FacturasVencidas = 5, PendientePedido = 6 // No se usa desconocida porque nunca tomará ese estado. Todas inician con Lista y se modifican posteriormente.
        }


        public enum TipoContactoCliente : byte { // Nuevos elementos podrían ser agregados entre los actuales, conservando los valores.
            Desconocido = 0, Comprador = 1, Almacenista = 2, Tesorería = 5, JefeCompras = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255
        }


        public enum TipoContactoProveedor : byte { // Nuevos elementos podrían ser agregados entre los actuales, conservando los valores.
            Desconocido = 0, Vendedor = 1, Despachos = 2, Tesorería = 5, JefeVentas = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255
        }


        public enum Unidad { // Aunque un nombre más apropiado sería TamañoPaquete se prefiere Unidad porque está más difundido así no sea del todo correcto porque Unidad se podría referir a cualquier unidad de medida, como metro, kilogramo, etc. En caso que se necesite usar esas unidades de medida se puede agregar una nueva enumeración con nombre UnidadFísica.
            Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Septeto = 7, Octeto = 8, Decena = 10,
            Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
            CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
            DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
            TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000
        }


        public enum Banco { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Nombres y códigos tomados de https://www.superfinanciera.gov.co/descargas/institucional/pubFile1004010/entidades_general.xls. El valor de la enumeración es el Tipo * 1000 + el Código, excepto para los tipo 1 que es directamente el código.
            Otro = -2, Ninguno = -1, Desconocido = 0, Bogotá = 1, Popular = 2, ItaúCorpbanca = 6, Bancolombia = 7, Citibank = 9, GNBSudameris = 12, BBVA = 13,
            Occidente = 23, CajaSocial = 30, Davivienda = 39, ScotiabankColpatria = 42, Agrario = 43, AVVillas = 49, CredifinancieraProcredit = 51,
            Bancamía = 52, W = 53, Bancoomeva = 54, Finandina = 55, Falabella = 56, Pichincha = 57, Coopcentral = 58, SantanderDeNegocios = 59,
            MundoMujer = 60, Multibank = 61, Bancompartir = 62, Serfinanza = 63, Corficolombiana = 2011, InversiónBancolombia = 2037, JPMorgan = 2041,
            BNPParibas = 2042, CorfiGNBSudameris = 2048, CorporaciónFinancieraDavivienda = 2049, GirosYFinanzas = 4008, Tuya = 4026, GMFinancial = 4031,
            Coltefinanciera = 4046, Bancoldex = 4101, FinancieraDann = 4108, FinancieraPagos = 4115, Credifamilia = 4117, Crezcamos = 4118,
            LaHipotecaria = 4120, Juriscoop = 4121, RCI = 4122,
            // Los intermedios se agregarán a medida que se necesiten.
            FinancieraDeAntioquia = 32001, CooperativaFinancieraJFK = 32002, Coofinep = 32003, Cotrafa = 32004, Confiar = 32005,
            // Otros que no están en el archivo de la superfinanciera pero si están en las listas de códigos de bancos de otros bancos. El valor será 1 000 000 + el código del banco en la lista del otro banco. Se debe verificar que no hayan duplicados, si los hay usar cualquiera de los dos códigos.
            Daviplata = 1001551, Nequi = 1001507, FinanciamientoItau = 1001014 // Hay dudas sobre FinanciamientoItau que aparece en la lista de Bancolombia, podría coincidir con alguno de la lista de la superfinanciera pero no hay claridad.
        } // Otro es cuando el banco de la empresa no está en esta enumeración, no se podría usar para realizar pagos. Ninguno es para indicar que cierta entidad económica no tiene banco. Desconocido es para indicar que cierta entidad económica sí tiene banco pero aún no se conoce cuál es.


        [Flags]
        public enum TipoContribuyente { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Tomados de la tabla 13.2.6.1. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN. Se usa en el primer elemento 'Ordinario' en vez de 'No aplica' y 'Retenedor IVA' en vez de 'Agente de Retención de IVA' porque se entienden más que como están en la tabla de la DIAN. Si se agregaran nuevos elementos que no constituyen una responsabilidad fiscal según la tabla 13.2.6.1. (como los de las responsabilidades de IVA) se deben omitir en Dian.ObtenerResponsabilidadFiscal().
            [Display(Name = "Ordinario")] Ordinario = 1, [Display(Name = "Gran Contribuyente")] GranContribuyente = 2, Autorretenedor = 4, // Ordinario es el que le aplica a una empresa que no se ha acogido a uno de los otros régimenes como Simple o Gran Contribuyente.
            [Display(Name = "Retenedor de IVA")] RetenedorIVA = 8, [Display(Name = "Régimen Simple")] RégimenSimple = 16,
            [Display(Name = "Responsable de IVA")] ResponsableIVA = 32, [Display(Name = "No Responsable de IVA")] NoResponsableIVA = 64 // Se complementa esta enumeración con las responsabilidades de IVA para forzar a realizar un manejo integrado en esta enumeración de todas las responsabilidades actuales y futuras. Tomadas de la tabla 'Modificación del anexo técnico (06-09-2019)' de la documentación de la DIAN para la facturación electrónica. ResponsableIVA es el equivalente al antiguo régimen común y NoResponsableIVA al antiguo régimen simplificado.
        }


        public enum TipoImpuestoConsumo : byte { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Tomados parcialmente de la tabla TipoTributo. Es el tipo que realmente se guarda en la base de datos. No se incluyen todos los tributos porque hay muchos que pueden confundir a los usuarios y sus nombres según deben enviarse a la DIAN no son muy claros. Se pueden incluir tantos impuestos al consumo como se deseen incluso si son del mismo tipo general porque estos serán relacionados con TipoTributo antes de enviar a la DIAN. General no tiene asociado una tasa automáticamente, se debe especificar por producto. Las opciones de para los que son INC (Hasta TelefoníaCelularYDatos) se tomaron de https://www.gerencie.com/que-es-el-impuesto-al-consumo.html.
            Desconocido = 0, General = 1, VehículosLujo = 2, Aeronaves = 3, Vehículos = 4, MotocicletasLujo = 5, Embarcaciones = 6,
            ServiciosRestaurante = 7, TelefoníaCelularYDatos = 8, BolsasPlásticas = 9, Carbono = 10, Combustibles = 11, DepartamentalNominal = 12,
            DepartamentalPorcentual = 13, SobretasaCombustibles = 14, Otro = 255
        }


        public enum ConceptoRetención : byte { // No cambiar los nombres de las enumeración ni de los elementos porque estos se usan en los archivos de opciones JSON. Tomados de https://www.gerencie.com/tabla-de-retencion-en-la-fuente-2020.html.
            Desconocido = 0, Generales = 1, TarjetaDébitoOCrédito = 2, AgrícolasOPecuariosSinProcesamiento = 3, AgrícolasOPecuaríosConProcesamiento = 4,
            CaféPergaminoOCereza = 5, CombustiblesDerivadosPetróleo = 6, ActivosFijosPersonasNaturales = 7, Vehículos = 8, BienesRaícesVivienda = 9,
            BienesRaícesNoVivienda = 10, ServiciosGenerales = 11, EmolumentosEclesiásticos = 12, TransporteCarga = 13,
            TransporteNacionalTerrestrePasajeros = 14, TransporteNacionalAéreoOMarítimoPasajeros = 15, ServiciosPorEmpresasTemporales = 16,
            ServiciosPorEmpresasVigilanciaYAseo = 17, SaludPorIPS = 18, HotelesYRestaurantes = 19, ArrendamientoBienesMuebles = 20,
            ArrendamientoBienesInmuebles = 21, OtrosIngresosTributarios = 22, HonorariosYComisiones = 23, LicenciamientoSoftware = 24, Intereses = 25,
            RendimientosFinacierosRentaFija = 26, LoteríasRifasYApuestas = 27, ColocaciónIndependienteJuegosAzar = 28, ContratosConstruccionYUrbanización = 29
        }


        public enum TipoTributo { // Tomados de la tabla 13.2.2. del 'Anexo técnico de factura electrónica de venta validación previa.pdf' de la DIAN.
            IVA = 1, INC = 4, [Display(Name = "INC Bolsas")] Bolsas = 22, [Display(Name = "INCarbono")] Carbono = 23,
            [Display(Name = "INCombustibles")] Combustibles = 24, [Display(Name = "ReteIVA")] RetenciónIVA = 5,
            [Display(Name = "ReteRenta")] RetenciónRenta = 6, [Display(Name = "IC")] DepartamentalNominal = 2,
            [Display(Name = "IC Porcentual")] DepartamentalPorcentual = 8, ICA = 3, [Display(Name = "ReteICA")] RetenciónICA = 7,
            [Display(Name = "FtoHorticultura")] Horticultura = 20, Timbre = 21, [Display(Name = "Sobretasa Combustibles")] SobretasaCombustibles = 25,
            Sordicom = 26, [Display(Name = "IC Datos")] Datos = 30, [Display(Name = "Licores")] ICL = 32, [Display(Name = "Plásticos")] INPP = 33,
            [Display(Name = "Bebidas Azucaradas")] IBUA = 34, [Display(Name = "Ultraprocesados")] ICUI = 35, [Display(Name = "Ad Valorem")] ADV = 36,
            Otro = 999
        }


        public enum DocumentoIdentificación { // Tomados de la tabla 13.2.1 de la documentación de la DIAN para la facturación electrónica.
            RegistroCivil = 11, TarjetaIdentidad = 12, CédulaCiudadanía = 13, TarjetaExtranjería = 21, CédulaExtranjería = 22, Nit = 31, Pasaporte = 41,
            DocumentoIdentificaciónExtranjero = 42, NitOtroPaís = 50, Nuip = 91
        }


        public enum PagoTransporte {
            Desconocido, [Display(Name = "Transporte Gratis")] Gratis, [Display(Name = "Transporte Pago Contraentrega")] Contraentrega
        }


        public enum TipoDocumentoElectrónico { // Ver lista de valores posibles en el numeral 13.1.3 de la documentación de la DIAN para la facturación electrónica.
            FacturaVenta = 1, FacturaExportación = 2, FacturaContingenciaFacturador = 3, FacturaContingenciaDian = 4, NotaCrédito = 91, NotaDébito = 92
        }


        public enum TarifaIVA { // Tomados del numeral 13.3.9 de la documentación de la DIAN para la facturación electrónica.
            [Display(Name = "0 %")] Cero = 0, [Display(Name = "5 %")] Cinco = 5, [Display(Name = "16 %")] Dieciseis = 16,
            [Display(Name = "19 %")] Diecinueve = 19
        }


        public enum TarifaINC { // Tomados del numeral 13.3.9 de la documentación de la DIAN para la facturación electrónica.
            [Display(Name = "0 %")] Cero = 0, [Display(Name = "2 %")] Dos = 2, [Display(Name = "4 %")] Cuatro = 4, [Display(Name = "8 %")] Ocho = 8,
            [Display(Name = "16 %")] Dieciseis = 16
        }


        public enum BrochaTema { // Principalmente se usa para ser llamadas desde código. Según el tema de color actual toman valor diferente.
            [Display(Name = "BrochaFondo30")] Fondo, [Display(Name = "BrochaFrente200")] Texto, [Display(Name = "BrochaFrente220")] TextoTítulo
        }

        public enum RazónNotaCrédito { // Tomados del archivo '13.2.4 Concepto de Corrección para Notas crédito cac DiscrepancyResponse cbc ResponseCode.xlsx' de la documentación de la DIAN para la facturación electrónica. AjustePrecio se supondrá que es para corrección de errores (hacia abajo) en el precio de algún producto y Descuento para descuentos acordados. Valores anteriores al anexo 1.9: [Display(Name = "Devolución Parcial")] DevoluciónParcial = 1, [Display(Name = "Anulación Factura")] AnulaciónFactura = 2, [Display(Name = "Descuento")] Descuento = 3, [Display(Name = "Ajuste Precio")] AjustePrecio = 4, [Display(Name = "Otra")] Otra = 5. Antes del anexo 1.9 se usaba Otra de manera predeterminada. Como este valor fue eliminado y no se permite especificar la anulación completa de facturas con notas crédito que no relacionan facturas, ahora se usa el 1 (Devolución Parcial) de manera predeterminada. Preferiblemente no usar anulación de factura para no tener que referenciar la factura con su CUFE.
            [Display(Name = "Devolución Parcial")] DevoluciónParcial = 1, [Display(Name = "Anulación Factura")] AnulaciónFactura = 2, // A partir del anexo 1.9 de 2024 no se puede hacer anulación de factura para notas crédito que no referencian una factura.
            [Display(Name = "Descuento")] Descuento = 3, [Display(Name = "Ajuste Precio")] AjustePrecio = 4, 
            [Display(Name = "Descuento por Pronto Pago")] DescuentoProntoPago = 5, [Display(Name = "Descuento por Volumen")] DescuentoVolumen = 6
        }

        public enum PlantillaDocumento { // Las plantillas ListaProductos son auxiliares de uso interno dentro de otras plantillas. Existen 3 posibles lugares donde se podría presentar la información de los documentos: 1. PDF: Usualmente se envía por email y es la forma más común de compartir documentos. 2. Web: Se presentarían los documentos directamente en el sitio web. Estas plantillas se pueden realizar con tecnologías modernas de desarrollo web pues estarían diseñadas para presentarse en navegadores que lo más normal es que estén actualizados. 3. Email: El documento es directamente el contenido del email. Aunque se podrían usar las plantillas web para esto, es posible que los clientes de correo no soporten las tecnologías de desarrollo web más modernas, entonces se permite especificar un diseño HTML distinto para estos casos. Si no se desea mantener dos versiones de HTML diferentes (Web e Email), se puede desarrollar solo la versión Web, pero desarrollándola con tecnologías web compatibles con los clientes de correo.
            VentaPdf, ProformaPdf, NotaCréditoPdf, NotaDébitoPdf, CotizaciónPdf, PedidoPdf, ComprobanteEgresoPdf, CobroPdf, RemisiónPdf, CatálogoPdf,
            MarcoPdf, FichaInformativaPdf, ListaProductosPdf,
            VentaWeb, ProformaWeb, NotaCréditoWeb, NotaDébitoWeb, CotizaciónWeb, PedidoWeb, ComprobanteEgresoWeb, CobroWeb, RemisiónWeb, CatálogoWeb,
            MarcoWeb, FichaInformativaWeb, ListaProductosWeb,
            VentaEmail, ProformaEmail, NotaCréditoEmail, NotaDébitoEmail, CotizaciónEmail, PedidoEmail, ComprobanteEgresoEmail, CobroEmail, RemisiónEmail, CatálogoEmail,
            MarcoEmail, FichaInformativaEmail, ListaProductosEmail
        }


        public enum DocumentoIntegración {
            [Display(Name = "VT-")] Venta, [Display(Name = "NC-")] NotaCrédito, [Display(Name = "ND-")] NotaDébito, [Display(Name = "CZ-")] Cotización,
            [Display(Name = "PD-")] Pedido, [Display(Name = "CE-")] ComprobanteEgreso, [Display(Name = "CB-")] Cobro, [Display(Name = "RS-")] Remisión,
            [Display(Name = "CT-")] Catálogo, [Display(Name = "FI-")] FichasInformativas, [Display(Name = "FP-")] FacturaProforma // El documento de integración de fichas informativas se escribe en plural porque por lo general trae información para realizar fichas de varios productos.
        }


        #endregion Enumeraciones>



        #region Variables Autocalculadas

        public static string ArchivoBaseDatosSQLite => NombreBaseDatosSQLite + ".db";

        public static string ArchivoBaseDatosVacíaSQLite => NombreBaseDatosSQLite + " [Vacía].db";

        public static DateTime AhoraUtcAjustado => AhoraUtc(Generales.HorasAjusteUtc);

        public static string AhoraNombresArchivos => AhoraUtcAjustado.ATexto(FormatoFechaHora).Reemplazar(":", "-");

        public static string MesDíaActualNombresArchivos => AhoraUtcAjustado.ATexto(FormatoNúmeroMesDía);

        public static string FechaActualNombresArchivos => AhoraUtcAjustado.ATexto(FormatoFecha);

        public static string RutaBaseDatosSQLite => Path.Combine(Equipo.RutaAplicación, CarpetaDatos, ArchivoBaseDatosSQLite);

        public static string RutaBaseDatosVacíaSQLite => Path.Combine(Equipo.RutaAplicación, CarpetaDatos, ArchivoBaseDatosVacíaSQLite);

        public static string RutaFirmador => Path.Combine(Equipo.RutaAplicación, CarpetaLibrerías, "Firmador.exe");

        #endregion Variables Autocalculadas>



        #region Métodos y Funciones

        public static decimal? ObtenerSubtotal(decimal? precio, int cantidad) => precio == null ? null : precio * cantidad;

        public static decimal? ObtenerPorcentajeMargen(decimal venta, decimal costo) => venta == 0 ? (decimal?)null : (venta - costo) / venta;

        public static decimal? ObtenerPorcentajeGanancia(decimal venta, decimal costo) => costo == 0 ? (decimal?)null : (venta - costo) / costo; // Término de https://es.wikipedia.org/wiki/Margen_de_beneficio y https://es.wikipedia.org/wiki/Margen_de_ganancia.

        public static decimal? ObtenerPorcentajeImpuesto(decimal? venta, decimal? impuesto)
            => impuesto == null || venta == null || venta == 0 ? null : impuesto / venta;

        public static double? ObtenerGramos(double? kilogramos) => kilogramos * 1000;

        public static double? ObtenerCentimétrosCúbicos(double? metrosCúbicos) => metrosCúbicos * 1000000;

        public static Prioridad ObtenerPrioridadWebProducto() => Prioridad.Media; // Originalmente en Opciones. Se movió aquí porque por el momento no accede a campos en Opciones. Pendiente establecer reglas más complejas.

        public static Prioridad ObtenerPrioridadProveedor() => Prioridad.Media; // Originalmente en Opciones. Se movió aquí porque por el momento no accede a campos en Opciones. Si fuera necesario Se pueden establecer reglas más complejas.

        public static double ObtenerPorcentajeGananciaAdicionalProducto() => 0; // Originalmente en Opciones. Se movió aquí porque por el momento no accede a campos en Opciones. Si fuera necesario se pueden establecer reglas más complejas que dependan de la marca, línea de negocio, subcategoría, etc.

        public static Prioridad ObtenerPrioridadCliente(TipoCliente tipoCliente) => Empresa.PrioridadesClientes[tipoCliente];

        public static double ObtenerPorcentajeGananciaCliente(TipoCliente tipoCliente) => Empresa.PorcentajesGananciaClientes[tipoCliente];

        public static string ObtenerRutaDatosJson() => ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaDatosJson, crearSiNoExiste: true);

        public static string ObtenerRutaDocumentosElectrónicos()
            => ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaDocumentosElectrónicos, crearSiNoExiste: true);

        public static string ObtenerRutaCotizaciones() => ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaCotizaciones, crearSiNoExiste: true);

        public static string ObtenerRutaDocumentosElectrónicosDeHoy() => ObtenerRutaDocumentosDeHoy(ObtenerRutaDocumentosElectrónicos());

        public static string ObtenerRutaCotizacionesDeHoy() => ObtenerRutaDocumentosDeHoy(ObtenerRutaCotizaciones());

        public static string ObtenerRutaDocumentosDeHoy(string rutaBase) => ObtenerRutaCarpeta(ObtenerRutaCarpeta(rutaBase,
            AhoraUtcAjustado.Year.ATexto(), crearSiNoExiste: true), MesDíaActualNombresArchivos, crearSiNoExiste: true);

        public static string ObtenerRutaCopiasSeguridad() => ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaCopiasSeguridad, crearSiNoExiste: true);

        public static string ObtenerRutaOpciones() => ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaOpciones, crearSiNoExiste: true);

        public static string ObtenerRutaImágenesPlantillas() => ObtenerRutaCarpeta(ObtenerRutaPlantillas(forzarRutaAplicación: true,
            forzarRutaDesarrollo: false), CarpetaImágenesPlantillas, crearSiNoExiste: true); // No es necesario modificarlas desde el Visual Studio, entonces se manejan directamente en la ruta de la aplicación. Las plantillas si se manejan en ambos lugares porque si se requiere trabajar en ellas y se deben agregar al repositorio y también se requieren tener las propias por fuera del repositorio en la ruta de la aplicación.

        public static string ObtenerRutaProductos()
            => Equipo.RutaProductos ?? ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaProductos, crearSiNoExiste: true);

        public static string ObtenerRutaProductosDesarrollo() => ObtenerRutaCarpeta(RutaDesarrollo, CarpetaProductos, crearSiNoExiste: false);

        public static string ObtenerRutaImágenesProductos()
            => ObtenerRutaCarpeta(ObtenerRutaProductos(), CarpetaProductosImágenes, crearSiNoExiste: true);

        public static string? ObtenerRutaImagenProductoNoDisponible()
            => ObtenerRutaArchivo(ArchivoImagenProductoNoDisponible, ObtenerRutaImágenesProductos(), ExtensionesImágenes);

        public static string ObtenerRutaInformaciónProductos()
            => ObtenerRutaCarpeta(ObtenerRutaProductos(), CarpetaProductosInformación, crearSiNoExiste: true);

        public static string ObtenerRutaInformaciónImágenesProductos()
            => ObtenerRutaCarpeta(ObtenerRutaInformaciónProductos(), CarpetaProductosInformaciónImágenes, crearSiNoExiste: true);

        public static string ObtenerRutaInformaciónCompiladosProductos()
            => ObtenerRutaCarpeta(ObtenerRutaInformaciónProductos(), CarpetaProductosInformaciónCompilados, crearSiNoExiste: true);

        public static string ObtenerRutaInformaciónFragmentosProductos()
            => ObtenerRutaCarpeta(ObtenerRutaInformaciónProductos(), CarpetaProductosInformaciónFragmentos, crearSiNoExiste: true);

        public static string ObtenerRutaPlantillasAplicación()
            => Equipo.RutaPlantillasDocumentos ?? ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaPlantillas, crearSiNoExiste: true);

        public static double? ObtenerPorcentajeIVAVenta(Cliente? cliente, Producto? producto) => (producto == null || cliente == null)
            ? (double?)null : (cliente.Municipio?.PorcentajeIVAPropio ?? cliente?.PorcentajeIVAPropio ?? producto.PorcentajeIVA); // La máxima prioridad la tiene el municipio, principalmente se usa para municipios con PorcentajeIVAPropio = 0 para omitir el cobro de IVA para todos los clientes ubicados en ese municipio. Después el cliente, algunos clientes pueden ser exentos de IVA. En estos casos PorcentajeIVAPropio es 0 y será aplicado a todos los productos que se le venda. Y finalmente se evalua si el producto no tiene IVA por ser excento o excluído.

        public static double? ObtenerPorcentajeIVACompra(Producto? producto) => producto == null
            ? (double?)null : (Empresa.MunicipioFacturación.PorcentajeIVAPropio ?? (Empresa.ExentoIVA ? 0 : producto.PorcentajeIVA));

        public static decimal? ObtenerIVAVenta(Cliente? cliente, MovimientoProducto? movimientoProducto) => (movimientoProducto?.Producto == null
            || cliente == null) ? null : ((decimal?)ObtenerPorcentajeIVAVenta(cliente, movimientoProducto.Producto) * movimientoProducto.SubtotalBaseIVA);

        public static decimal? ObtenerIVACompra(MovimientoProducto? movimientoProducto) => movimientoProducto?.Producto == null
            ? null : (decimal?)ObtenerPorcentajeIVACompra(movimientoProducto.Producto) * movimientoProducto.SubtotalBaseIVA;

        public static decimal ObtenerTarifaImpuestoPorcentual(decimal valorImpuesto, decimal subtotal) => Math.Round(100 * valorImpuesto / subtotal, 0);

        public static bool EsPáginaExtra(string nombreArchivoPlantilla)
            => nombreArchivoPlantilla.FinalizaCon("Extra.cshtml") || nombreArchivoPlantilla.FinalizaCon("ExtraPropia.cshtml");

        /// <summary>
        /// Inicia algunas variables de Configuración.cs (configuración del usuario del código) que no se pueden iniciar en el cuerpo porque requieren unos pasos adicionales para establecer su valor. 
        /// </summary>
        public static void IniciarVariablesConfiguración() { } // IniciarVariablesConfiguración>

        /// <summary>
        /// Algunos objetos DTO, como los de DocumentosGráficos o Integración, tienen configuraciones globales que deben ser iniciadas desde otras variables,
        /// usualmente desde OpcionesEmpresa. Se inician de esta manera para no agregar referencias a espacios de nombres que puedan interferir
        /// en la generación de los PDF y para mantener el desacoplamiento entre estos objetos y el resto del código.
        /// </summary>
        public static void IniciarVariablesDTO() => OpcionesColumnas.AnchoTotalesFactura = Empresa.AnchoTotalesFactura;

        /// <summary>
        /// Inicia algunas variables que no se pueden iniciar en el cuerpo de la clase Global porque requieren unos pasos adicionales para establecer su valor.
        /// </summary>
        public static void IniciarVariablesGlobales() =>
            OpcionesConversiónPdf.SetFontProvider(new iText.Html2pdf.Resolver.Font.DefaultFontProvider(true, true, true)); // Necesario para poder usar la fuente Calibri, se podría tardar algunos segundos. Si llega a ser un problema de rendimiento revisar las opciones en SimpleOps.xlsx > Tareas > Rendimiento Generación de PDF.

        public static bool UsarRutaPlantillasDesarrollo(bool forzarRutaAplicación = false, bool forzarRutaDesarrollo = false)
            => forzarRutaDesarrollo || (!forzarRutaAplicación && ModoDesarrolloPlantillas);


        public static string ObtenerRutaPlantillas(bool forzarRutaAplicación = false, bool forzarRutaDesarrollo = false)
            => UsarRutaPlantillasDesarrollo(forzarRutaAplicación, forzarRutaDesarrollo) // Para facilitar el desarrollo se devuelve directamente la ruta de la plantilla de desarrollo cuando no se esté forzando que la tome de la ruta de la aplicación (típicamente solo al iniciar cuando ReemplazarPlantillasDocumentos es verdadero).
                ? ObtenerRutaCarpeta(RutaDesarrollo, CarpetaPlantillasDesarrollo, crearSiNoExiste: false)
                : ObtenerRutaPlantillasAplicación();


        public static string ObtenerRutaPlantilla(PlantillaDocumento plantilla, bool forzarRutaAplicación = false, bool forzarRutaDesarrollo = false,
            int númeroPágina = 1, bool extra = false) {

            var rutaPlantilla = Path.Combine(ObtenerRutaPlantillas(forzarRutaAplicación, forzarRutaDesarrollo),
                $"{plantilla}{(númeroPágina == 1 ? "" : númeroPágina.ATexto())}{(extra ? "Extra" : "")}.cshtml");

            if (HabilitarPlantillasPropias && UsarRutaPlantillasDesarrollo(forzarRutaAplicación, forzarRutaDesarrollo)) {
                var rutaPlantillaPropia = rutaPlantilla.Reemplazar(".cshtml", "Propia.cshtml");
                if (File.Exists(rutaPlantillaPropia)) rutaPlantilla = rutaPlantillaPropia;
            }

            return rutaPlantilla;

        } // ObtenerRutaPlantilla>


        public static Dictionary<int, string> ObtenerRutasPáginasPlantilla(PlantillaDocumento plantilla, bool omitirPrimera,
            bool forzarRutaAplicación = false, bool forzarRutaDesarrollo = false, int cantidadPáginasExtra = 0) {

            var númeroPágina = 0;
            bool existe;
            var rutasPáginasPlantilla = new Dictionary<int, string>();

            do {

                númeroPágina++;
                var rutaPlantilla = ObtenerRutaPlantilla(plantilla, forzarRutaAplicación, forzarRutaDesarrollo, númeroPágina);
                existe = File.Exists(rutaPlantilla); // No importa que se repita File.Exists de ObtenerRutaPlantilla() si se está usando plantilla personalizada (CatálogoPdf2.cshtml, CatálogoPdf3.cshtml, etc). No afecta en gran medida el rendimiento. Es necesario siempre calcular si existe o no la plantilla para poder salir del ciclo. Si llega a afectar el rendimiento, habría que usar una variable de salida en el procedimiento para indicar cuando se ha encontrado una plantilla personalizada y no se debe volver a verificar su existencia.
                if (existe && (númeroPágina != 1 || !omitirPrimera)) rutasPáginasPlantilla.Add(númeroPágina, rutaPlantilla);

            } while (existe);

            if (plantilla == PlantillaDocumento.CatálogoPdf && cantidadPáginasExtra > 0) {

                var rutaPlantillaExtra = ObtenerRutaPlantilla(plantilla, forzarRutaAplicación, forzarRutaDesarrollo, extra: true);
                if (File.Exists(rutaPlantillaExtra)) {
                    for (int p = 0; p < cantidadPáginasExtra; p++) {
                        rutasPáginasPlantilla.Add(númeroPágina + p, rutaPlantillaExtra);
                    }
                }

            }

            return rutasPáginasPlantilla;

        } // ObtenerRutasPáginasPlantilla>


        public static FormaEntrega ObtenerFormaEntrega(Municipio? municipio) {

            FormaEntrega formaEntrega;
            if (municipio == null) {

                if (Empresa.HabilitarProductosVirtuales) {
                    formaEntrega = FormaEntrega.Virtual;
                } else {
                    if (OperacionesEspecialesDatos) {
                        formaEntrega = FormaEntrega.Virtual; // Cualquiera. Solo es para que no saque error en procesos de migración y carga inicial de datos.
                    } else {
                        formaEntrega = FormaEntrega.Desconocida;
                    }
                }

            } else {
                formaEntrega = municipio.OtroPaís != null ? FormaEntrega.TransportadoraInternacional
                    : MunicipiosConMensajería.Contains(municipio.ID) ? FormaEntrega.Mensajería : FormaEntrega.Transportadora;
            }
            return formaEntrega;

        } // ObtenerFormaEntrega>


        public static string ObtenerRutaOpciones<T>(T opciones) where T : class
            => opciones switch {
                OpcionesEmpresa _ => Path.Combine(ObtenerRutaOpciones(), "Empresa.json"),
                OpcionesGenerales _ => Path.Combine(ObtenerRutaOpciones(), "Generales.json"),
                OpcionesEquipo _ => Path.Combine(ObtenerRutaOpciones(), "Equipo.json"),
                _ => throw new ArgumentException(CasoNoConsiderado(typeof(T).ToString()))
            };


        public static DocumentoIdentificación ObtenerDocumentoIdentificación(TipoEntidad tipoEntidad)
            => tipoEntidad switch {
                TipoEntidad.Desconocido => DocumentoIdentificación.CédulaCiudadanía,
                TipoEntidad.Empresa => DocumentoIdentificación.Nit,
                TipoEntidad.Persona => DocumentoIdentificación.CédulaCiudadanía,
            };


        public static TipoTributo ObtenerTipoTributo(TipoImpuestoConsumo tipoImpuestoConsumo)
            => tipoImpuestoConsumo switch {
                TipoImpuestoConsumo.Desconocido => throw new ArgumentException("No se esperaba TipoImpuestoConsumo desconocido."),
                TipoImpuestoConsumo.General => TipoTributo.INC,
                TipoImpuestoConsumo.VehículosLujo => TipoTributo.INC,
                TipoImpuestoConsumo.Aeronaves => TipoTributo.INC,
                TipoImpuestoConsumo.Vehículos => TipoTributo.INC,
                TipoImpuestoConsumo.MotocicletasLujo => TipoTributo.INC,
                TipoImpuestoConsumo.Embarcaciones => TipoTributo.INC,
                TipoImpuestoConsumo.ServiciosRestaurante => TipoTributo.INC,
                TipoImpuestoConsumo.TelefoníaCelularYDatos => TipoTributo.Datos,
                TipoImpuestoConsumo.BolsasPlásticas => TipoTributo.Bolsas,
                TipoImpuestoConsumo.Carbono => TipoTributo.Carbono,
                TipoImpuestoConsumo.Combustibles => TipoTributo.Combustibles,
                TipoImpuestoConsumo.DepartamentalNominal => TipoTributo.DepartamentalNominal,
                TipoImpuestoConsumo.DepartamentalPorcentual => TipoTributo.DepartamentalPorcentual,
                TipoImpuestoConsumo.SobretasaCombustibles => TipoTributo.SobretasaCombustibles,
                TipoImpuestoConsumo.Otro => TipoTributo.Otro,
            };


        public static decimal ObtenerMínimoTransporteGratis(TipoCliente tipoCliente, Municipio? municipio) {

            var formaEntrega = ObtenerFormaEntrega(municipio);
            var tipoClienteFormaEntrega = formaEntrega switch {
                FormaEntrega.Desconocida => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_Desconocida,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_Desconocida,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_Desconocida,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_Desconocida,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_Desconocida,
                },
                FormaEntrega.Virtual => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_Virtual,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_Virtual,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_Virtual,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_Virtual,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_Virtual,
                },
                FormaEntrega.PuntoVenta => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_PuntoVenta,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_PuntoVenta,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_PuntoVenta,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_PuntoVenta,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_PuntoVenta,
                },
                FormaEntrega.Mensajería => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_Mensajería,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_Mensajería,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_Mensajería,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_Mensajería,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_Mensajería,
                },
                FormaEntrega.Transportadora => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_Transportadora,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_Transportadora,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_Transportadora,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_Transportadora,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_Transportadora,
                },
                FormaEntrega.TransportadoraInternacional => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_TransportadoraInternacional,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_TransportadoraInternacional,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_TransportadoraInternacional,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_TransportadoraInternacional,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_TransportadoraInternacional,
                },
                FormaEntrega.Otra => tipoCliente switch {
                    TipoCliente.Desconocido => TipoClienteFormaEntrega.Desconocido_Otra,
                    TipoCliente.Consumidor => TipoClienteFormaEntrega.Consumidor_Otra,
                    TipoCliente.Distribuidor => TipoClienteFormaEntrega.Distribuidor_Otra,
                    TipoCliente.GrandesContratos => TipoClienteFormaEntrega.GrandesContratos_Otra,
                    TipoCliente.Otro => TipoClienteFormaEntrega.Otro_Otra,
                },
            };

            return Empresa.MínimosTransporteGratis[tipoClienteFormaEntrega];

        } // ObtenerMínimoTransporteGratis>


        public static string? ObtenerDígitoVerificación(string? nit) {

            if (nit == null) return null;
            var primos = new[] { 3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71 }; // Algunos números primos seleccionados por la DIAN.
            var largoNit = nit.Length;
            var suma = 0;

            for (var i = 0; i < largoNit; i++) {
                suma += nit[i].AEntero() * primos[largoNit - i - 1];
            }

            int residuo = suma % 11; // Módulo el restante después de una división entera, por ejemplo 13 % 3 da 4,33.. con 4 como parte entera. El módulo se obtiene de 13 - 4 * 3 = 1.
            return residuo <= 1 ? residuo.ATexto() : (11 - residuo).ATexto();

        } // ObtenerDígitoVerificación>


        /// <summary>
        /// Carga todas las opciones desde los archivos JSON. Si no existen los crea usando los valores predeterminados.
        /// </summary>
        public static void CargarOpciones() {

            CargarOpciones(ref Empresa);
            CargarOpciones(ref Generales);
            CargarOpciones(ref Equipo);

        } // CargarOpciones>


        public static void CopiarPlantillasARutaAplicación(bool sobreescribir) {

            foreach (var plantilla in ObtenerValores<PlantillaDocumento>()) {

                var rutasPáginasDesarrollo = ObtenerRutasPáginasPlantilla(plantilla, omitirPrimera: false, forzarRutaDesarrollo: true,
                    cantidadPáginasExtra: 1);
                var rutasPáginasAplicación = ObtenerRutasPáginasPlantilla(plantilla, omitirPrimera: false, forzarRutaAplicación: true,
                    cantidadPáginasExtra: 1);
                var páginasAplicación = rutasPáginasAplicación.Select(kv => Path.GetFileName(kv.Value).AMinúscula()).ToList();

                foreach (var kv in rutasPáginasDesarrollo) {

                    var rutaPáginaDesarrollo = kv.Value;
                    var númeroPáginaDesarrollo = kv.Key;
                    var páginaExtra = EsPáginaExtra(rutaPáginaDesarrollo);
                    var páginaDesarrollo = Path.GetFileName(rutaPáginaDesarrollo).AMinúscula()?.Reemplazar("propia.cshtml", ".cshtml");
                    if (sobreescribir || !páginasAplicación.Contains(páginaDesarrollo))
                        File.Copy(rutaPáginaDesarrollo, ObtenerRutaPlantilla(plantilla, forzarRutaAplicación: true,
                            númeroPágina: páginaExtra ? 1 : númeroPáginaDesarrollo, extra: páginaExtra), overwrite: sobreescribir);

                }

            }

        } // CopiarPlantillasARutaAplicación>


        /// <summary>
        /// Crea o reemplaza algunas carpetas y archivos necesarios.
        /// </summary>
        public static void ConfigurarCarpetasYArchivos() {

            var carpetaPlantillasDesarrollo = ObtenerRutaCarpeta(RutaDesarrollo, CarpetaPlantillasDesarrollo, crearSiNoExiste: false);
            var carpetaPlantillas = ObtenerRutaPlantillasAplicación();

            var carpetaImágenesDesarrollo = ObtenerRutaCarpeta(carpetaPlantillasDesarrollo, CarpetaImágenesPlantillas, crearSiNoExiste: false);
            var carpetaImágenes = ObtenerRutaCarpeta(carpetaPlantillas, CarpetaImágenesPlantillas, crearSiNoExiste: true);

            var carpetaProductosDesarrollo = ObtenerRutaProductosDesarrollo();
            var carpetaProductos = ObtenerRutaProductos();

            var carpetaImágenesProductosDesarrollo = ObtenerRutaCarpeta(carpetaProductosDesarrollo, CarpetaProductosImágenes, crearSiNoExiste: false);
            var carpetaImágenesProductos = ObtenerRutaCarpeta(carpetaProductos, CarpetaProductosImágenes, crearSiNoExiste: true);

            var carpetaInformaciónProductosDesarrollo
                = ObtenerRutaCarpeta(carpetaProductosDesarrollo, CarpetaProductosInformación, crearSiNoExiste: false);
            var carpetaInformaciónProductos = ObtenerRutaCarpeta(carpetaProductos, CarpetaProductosInformación, crearSiNoExiste: true);

            var carpetaInformaciónImágenesProductosDesarrollo
                = ObtenerRutaCarpeta(carpetaInformaciónProductosDesarrollo, CarpetaProductosInformaciónImágenes, crearSiNoExiste: false);
            var carpetaInformaciónImágenesProductos
                = ObtenerRutaCarpeta(carpetaInformaciónProductos, CarpetaProductosInformaciónImágenes, crearSiNoExiste: true);

            var carpetaInformaciónFragmentosProductosDesarrollo
                = ObtenerRutaCarpeta(carpetaInformaciónProductosDesarrollo, CarpetaProductosInformaciónFragmentos, crearSiNoExiste: false);
            var carpetaInformaciónFragmentosProductos
                = ObtenerRutaCarpeta(carpetaInformaciónProductos, CarpetaProductosInformaciónFragmentos, crearSiNoExiste: true);

            CopiarArchivos(carpetaImágenesDesarrollo, carpetaImágenes, sobreescribir: false); // Solo se copia la imagen si no está porque es posible que el usuario la haya personalizado.
            CopiarArchivos(carpetaImágenesProductosDesarrollo, carpetaImágenesProductos, sobreescribir: false); // Solo se copia la imagen si no está porque es posible que el usuario la haya personalizado. Aunque las imágenes de ejemplo no serán útiles para el usuario, si terminan en su carpeta se nombran iniciando con ZZ para que al menos aparezcan al final de la carpeta ordenada alfabéticamente. El usuario podría modificar estos archivos para que sean imágenes en blanco y no le estorben visualmente y al ser archivos existentes no serían reemplazados por el código.
            CopiarArchivos(carpetaInformaciónProductosDesarrollo, carpetaInformaciónProductos, sobreescribir: false);
            CopiarArchivos(carpetaInformaciónImágenesProductosDesarrollo, carpetaInformaciónImágenesProductos, sobreescribir: false);
            CopiarArchivos(carpetaInformaciónFragmentosProductosDesarrollo, carpetaInformaciónFragmentosProductos, sobreescribir: false);

            CopiarPlantillasARutaAplicación(sobreescribir: false);

            ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaDatos, crearSiNoExiste: true); // Se ejecuta para crear la carpeta de Datos si no existe.
            if (!string.IsNullOrEmpty(Equipo.RutaIntegración)) ObtenerRutaCarpeta(Equipo.RutaIntegración, "", crearSiNoExiste: true); // Se ejecuta para crear la carpetas de integración con terceros si no existen. Al pasar una carpeta vacía usa la rutaPadre. Si no se ha establecido la RutaIntegración puede ser que el usuario no va a usar la integración de terceros entonces no es necesario crear esta carpeta. 

            if (!File.Exists(RutaFirmador)) {

                var rutaLibrerías = ObtenerRutaCarpeta(Equipo.RutaAplicación, CarpetaLibrerías, crearSiNoExiste: true);
                var rutaLibreríasDesarrollo = ObtenerRutaCarpeta(RutaDesarrollo, CarpetaLibrerías, crearSiNoExiste: false);
                var rutaLicencias = ObtenerRutaCarpeta(rutaLibrerías, CarpetaLicencias, crearSiNoExiste: true);
                var rutaLicenciasDesarrollo = ObtenerRutaCarpeta(rutaLibreríasDesarrollo, CarpetaLicencias, crearSiNoExiste: false);
                foreach (var rutaLibrería in Directory.GetFiles(rutaLibreríasDesarrollo)) {
                    File.Copy(rutaLibrería, Path.Combine(rutaLibrerías, Path.GetFileName(rutaLibrería)));
                }
                foreach (var rutaLicencia in Directory.GetFiles(rutaLicenciasDesarrollo)) {
                    File.Copy(rutaLicencia, Path.Combine(rutaLicencias, Path.GetFileName(rutaLicencia)));
                }

            }

            if (!File.Exists(RutaBaseDatosSQLite) && UsarSQLite) { // Si la base de datos SQLite no existe y se quiere usar SQLite, copiará una base de datos vacía desde la ruta de desarrollo y la llenará con datos básicos comunes a todos los usuarios, principalmente datos de los municipios de Colombia.

                var rutaDatosDesarrollo = ObtenerRutaCarpeta(RutaDesarrollo, CarpetaDatosDesarrollo, crearSiNoExiste: false);
                var rutaDatosJsonDesarrollo = ObtenerRutaCarpeta(rutaDatosDesarrollo, CarpetaDatosJsonDesarrollo, crearSiNoExiste: false);
                var rutaBaseDatosVacíaDesarrollo = Path.Combine(rutaDatosDesarrollo, ArchivoBaseDatosVacíaSQLite);
                var rutaDatosJson = ObtenerRutaDatosJson();

                if (File.Exists(rutaBaseDatosVacíaDesarrollo)) {

                    File.Copy(rutaBaseDatosVacíaDesarrollo, RutaBaseDatosSQLite);
                    foreach (var rutaJson in Directory.GetFiles(rutaDatosJsonDesarrollo)) {
                        var jsonDestino = Path.Combine(rutaDatosJson, Path.GetFileName(rutaJson));
                        if (!File.Exists(jsonDestino)) File.Copy(rutaJson, jsonDestino);
                    }
                    var éxito = Contexto.CargarDatosIniciales(rutaDatosJson, out string error);
                    if (éxito) {
                        MostrarÉxito("Se creó la base de datos SQLite y se cargaron exitosamente los datos iniciales.");
                    } else {
                        MostrarError(error);
                    }

                } else {
                    throw new InvalidOperationException($"No se encontró la base de datos vacía en {rutaBaseDatosVacíaDesarrollo}");
                }

            }

        } // ConfigurarCarpetasYArchivos>


        /// <summary>
        /// Carga un tipo de opciones desde un archivo JSON. Si no existe lo crea usando los valores predeterminados.
        /// </summary>
        public static void CargarOpciones<T>(ref T opciones,
            Serialización serialización = Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto) where T : class {

            var rutaJson = ObtenerRutaOpciones(opciones);

            otraVez:
            if (File.Exists(rutaJson)) {

                try {
                    opciones = Deserializar<T>(File.ReadAllText(rutaJson), serialización) ?? opciones;
                } catch (Exception ex) { // Antes se estaba suprimiendo la alerta CA1031. No capture tipos de excepción generales. Cualquier tipo de excepción se tratará de la misma manera: haciendo una copia del archivo con error y borrándolo.

                    var rutaCopia = ObtenerRutaAgregandoTexto(rutaJson, $" - Copia con Error en {AhoraUtcAjustado.ATexto(FormatoFecha)}");
                    File.Copy(rutaJson, rutaCopia);
                    IntentarEliminar(rutaJson);
                    MostrarError($"No se pudo leer el archivo de opciones {rutaJson}.{DobleLínea}{NombreAplicación} iniciará usando los valores " +
                                 $"predeterminados. Si necesitas migrar los valores anteriores los puedes tomar de la copia realizada {rutaCopia}." +
                                 $"{DobleLínea}{ex.Message}.");
                    goto otraVez;

                }

            } else {
                GuardarOpciones(opciones, serialización);
            }

        } // CargarOpciones>


        public static void GuardarOpciones<T>(T opciones,
            Serialización serialización = Serialización.DiccionarioClaveEnumeración | Serialización.EnumeraciónEnTexto) where T : class {

            var rutaJson = ObtenerRutaOpciones(opciones);
            try {
                File.WriteAllText(rutaJson, Serializar(opciones, serialización));
            } catch (Exception) { // Antes se estaba suprimiendo la alerta CA1031. No capture tipos de excepción generales. Cualquier tipo de excepción se tratará de la misma manera: Informando al usuario y omitiendo el guardado.
                MostrarError($"No se pudo guardar las opciones en {rutaJson}.");
            }

        } // GuardarOpciones>


        public static bool CrearPdfYRespuestaElectrónica<D, M>(D? documento, DocumentoElectrónico<Factura<Cliente, M>, M>? documentoElectrónico, 
            out string? mensaje) where D : Factura<Cliente, M> where M : MovimientoProducto {

            mensaje = null;

            var mensajeRespuesta = "documentoElectrónico es nulo.";

            if (documentoElectrónico != null && CrearRespuestaElectrónica(out mensajeRespuesta, documentoElectrónico, out string? rutaXml)) {

                if (documento != null && CrearPdfVenta(documento, documentoElectrónico, out string rutaPdf)) {

                    if (rutaXml != null && rutaPdf != null && File.Exists(rutaPdf) && File.Exists(rutaXml)) {

                        var rutaZipACrear = ObtenerRutaCambiandoExtensión(rutaPdf, "zip");
                        if (CrearZip(new List<string> { rutaPdf, rutaXml }, rutaZipACrear)) {

                            documentoElectrónico.RutaZip = rutaZipACrear;
                            documentoElectrónico.RutaPdf = rutaPdf;
                            IntentarEliminar(rutaXml);
                            return true;

                        } else {
                            mensaje = "No se pudo crear el archivo ZIP con el PDF de la representación gráfica y el XML de respuesta electrónica.";
                            return false;
                        }

                    } else {
                        mensaje = "No existe el archivo PDF de la representación gráfica o el XML de respuesta electrónica.";
                        return false;
                    }

                } else {
                    mensaje = "No se pudo crear la representación gráfica de la factura electrónica.";
                    return false;
                }

            } else {
                mensaje = $"No se pudo crear la respuesta electrónica de la factura electrónica. {mensajeRespuesta}";
                return false;
            }

        } // CrearPdfYRespuestaElectrónica>


        #endregion Métodos y Funciones>



        #region Métodos y Funciones de Interfaz

        public static SolidColorBrush ObtenerBrocha(BrochaInformativa brochaPersonalizada, string? sufijoNombre = "")
            => (SolidColorBrush)Application.Current.FindResource($"Brocha{brochaPersonalizada}{sufijoNombre}");

        public static SolidColorBrush ObtenerBrocha(BrochaTema brochaTema) => (SolidColorBrush)Application.Current.FindResource(brochaTema.ATexto());

        public static double ObtenerTamañoLetra(TamañoLetra tamañoLetra) => (double)Application.Current.FindResource($"TamañoLetra{tamañoLetra}");

        public static SolidColorBrush ObtenerBrocha(string nombreBrocha) => (SolidColorBrush)Application.Current.FindResource(nombreBrocha);

        public static string? ObtenerNúmeroCuentaBancaria(Banco banco, string? otroNúmeroCuenta)
            => otroNúmeroCuenta ?? Empresa.CuentasBancarias.ObtenerValorObjeto(banco);


        public static Visibility ObtenerVisibilidad(object value, bool inverso, bool forzarOculto = false) {

            var respuesta = value switch {
                null => inverso ? Visibility.Visible : Visibility.Collapsed,
                string texto => (inverso && string.IsNullOrEmpty(texto)
                    || (!inverso && !string.IsNullOrEmpty(texto))) ? Visibility.Visible : Visibility.Collapsed,
                _ => throw new ArgumentException(CasoNoConsiderado(value?.ToString())),
            };
            if (forzarOculto) respuesta = Visibility.Collapsed;
            return respuesta;

        } // ObtenerVisibilidad>


        #endregion Métodos y Funciones de Interfaz>



    } // Global>



} // SimpleOps>
