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
using SimpleOps.Datos;
using SimpleOps.Modelo;
using System.Text.Json.Serialization;



namespace SimpleOps.Singleton {



    /// <summary>
    /// Opciones, datos y configuraciones propias de la empresa usuaria de SimpleOps. Suelen iniciar nulas o en valores predeterminados muy probables. 
    /// Son propiedades particulares para cada empresa usuaria de SimpleOps que son comunes a todos los usuarios en la misma empresa.
    /// Su modificación está restringida por roles y se actualiza automáticamente en los otros equipos cuando se da un cambio en uno de ellos.
    /// Los valores iniciales en código solo sirven para autogenerar el archivo Empresa.json cuando no exista y para evitar tener que 
    /// declarar las propiedades permitiendo valores nulos.
    /// </summary>
    sealed class OpcionesEmpresa { // No cambiar los nombres de las propiedades porque estos se usan en los archivos de opciones JSON. Si alguna propiedad pudiera tener valores diferentes para diferentes usuarios/equipos de la empresa se debe usar OpcionesEquipo. No debe tener métodos (estos van en Global), pero si se permiten propiedades autocalculadas para algunas propiedades que tiene sentido que le pertenezcan al objeto Empresa.



        #region Patrón Singleton
        // Ver https://csharpindepth.com/Articles/Singleton.

        private static readonly Lazy<OpcionesEmpresa> DatosLazy = new Lazy<OpcionesEmpresa>(() => new OpcionesEmpresa());

        public static OpcionesEmpresa Datos { get { return DatosLazy.Value; } } // Normalmente esta sería la variable que se accede pero se prefiere hacer una variable auxiliar Empresa en Global.cs para tener un acceso más fácil sin necesidad de escribir OpcionesEmpresa.Datos.

        private OpcionesEmpresa() { }

        #endregion Patrón Singleton>



        #region Propiedades y Variables

        public Municipio MunicipioFacturación = null!; // Es un clon de solo lectura. Se garantiza no será nulo porque siempre se carga al iniciar. Datos del municipio de la dirección de facturación de la empresa. Se actualiza al iniciar SimpleOps, al cambiar Empresa.Datos.MunicipioFacturaciónID y al realizar cambios en la tabla municipios. Para que funcione la factura electrónica debe tener un departamento de Colombia y este corresponder a uno de los valores de la columna Nombre en el numeral 13.4.2 de la documentación de factura electrónica. Aunque si no corresponde, no genera Rechazo, si no Notificación. El código del municipio debe corresponder a un valor válido de lista de municipios en el numeral 13.4.3 de la documentación de factura electrónica de la DIAN. Para que funcione la factura electrónica con la DIAN debe ser un municipio de Colombia y su nombre corresponder a uno de los valores del la columna Nombre Municipio en el numeral 13.4.3 de la documentación de factura electrónica. Aunque si no corresponde, no genera Rechazo, si no Notificación.

        private int municipioFacturaciónID = 1; // ID del municipio de la dirección de facturación. 1: Bogotá, 2: Medellín, 3: Cali, 4: Barranquilla, etc. Ver tabla municipios en Guías/IDs Municipios.html. Siempre será establecido. Si no se establece inicia por defecto en Bogotá.
        public int MunicipioFacturaciónID { 

            get => municipioFacturaciónID;

            set {

                if (municipioFacturaciónID != value) {
                    municipioFacturaciónID = value;
                    if (!OperacionesEspecialesDatos) Contexto.LeerMunicipiosDeInterés(); // Solo actualiza los municipios cuando el cambio se realiza después del inicio de SimpleOps.
                }

            }

        } // MunicipioFacturaciónID>


        public Municipio MunicipioUbicación = null!; // Es un clon de solo lectura. Se garantiza que no será nulo porque siempre se carga al iniciar, incluso si MunicipioUbicaciónID es nulo porque en ese caso usa MunicipioFacturaciónID. Municipio de la dirección de ubicación de la empresa. Se actualiza al iniciar SimpleOps, al cambiar Empresa.Datos.MunicipioUbicaciónID y al realizar cambios en la tabla municipios.

        private int? municipioUbicaciónID = null; // Se usa si es necesario especificar un municipioID diferente al de facturación para la ubicación de la empresa. Si es nulo se usa MunicipioFacturaciónID.
        public int? MunicipioUbicaciónID { 

            get => municipioUbicaciónID; 

            set {

                if (municipioUbicaciónID != value) {
                    municipioUbicaciónID = value;
                    if (!OperacionesEspecialesDatos) Contexto.LeerMunicipiosDeInterés(); // Solo actualiza los municipios cuando el cambio se realiza después del inicio de SimpleOps.
                }  
                
            }

        } // MunicipioUbicaciónID>


        public string? RazónSocial { get; set; } // Razón social de la empresa como está registrada en el RUT.

        public string? TeléfonoPrincipal { get; set; } // Línea telefónica principal. Puede ser celular o fijo.

        public string? EmailVentas { get; set; } // Email principal de ventas (para recepción de ordenes de compra de clientes).

        public string? SitioWeb { get; set; } 

        public string? DirecciónFacturación { get; set; } // Dirección de facturación registrada en el RUT.

        public string? DirecciónUbicación { get; set; } // Se usa si es necesario especificar una dirección diferente a la de facturación para la ubicación de la empresa. Si es nula se usa la dirección de facturación.

        public bool ExentoIVA { get; set; } = false; // Si la empresa es exenta de IVA y todos los proveedores le deben vender sin IVA. Podría aplicar a algunas empresas con régimenes especiales de impuestos independiente de su tipo de contribuyente. Si MunicipioFacturación es un municipio exento de IVA será exento sin importar el valor que se establezca aquí.

        public TipoEntidad TipoEntidad { get; set; } = TipoEntidad.Empresa; // Especifica si es empresa o persona natural. No puede ser desconocida porque se necesita para la facturación electrónica.

        public string? Nit { get; set; } // Nit de la empresa sin dígito de verificación.

        public Dictionary<Banco, string> CuentasBancarias { get; set; } = new Dictionary<Banco, string>(); // Su uso principal es evitar agregar la cuenta en la tabla MovimientosBancarios. Solo se agregaría la cuenta en estas tablas en caso que no sea la cuenta bancaria principal para ese banco. Aplica para empresas que tengan más de una cuenta bancaria en el mismo banco.

        public Banco BancoPreferido { get; set; } = Banco.Ninguno; // Si no es Banco.Ninguno este banco se usará por defecto para registrar todas las acciones que requieran un banco de la empresa (carga de movimientos bancarios, registro de pagos, etc). Para realizar acciones sobre los otros bancos la interfaz de usuario provee medios para cambiar temporalmente el banco sobre el que se realizará la acción y hacer un banco como el nuevo preferido. Este banco preferido debe estar en el diccionario CuentasBancarias.

        public string? NombreComercial { get; set; } // Se usa para informarlo a la DIAN al hacer la factura electrónica y para usarlo en representación gráfica de las facturas. Si es nulo se omite en la factura electrónica y en la representación gráfica se usa la razón social.

        public TipoContribuyente TipoContribuyente { get; set; } = TipoContribuyente.Ordinario | TipoContribuyente.ResponsableIVA; // Si se necesitan agregar varios valores se hace así: Global.TipoContribuyente.Autorretenedor | Global.TipoContribuyente.AgenteDeRetenciónIVA. 

        public double PorcentajeIVAPredeterminado { get; set; } = 0.19; // El porcentaje de IVA que se aplica si no se especifica ni en el producto ni en el cliente ni en el municipio del cliente. Si el TipoContribuyente es NoResponsableIVA el PorcentajeIVAPredeterminadoEfectivo que es el que verderamente se usa es cero sin importar el valor que se use aquí.

        public double PorcentajeImpuestoConsumoPredeterminado { get; set; } = 0; // El porcentaje de impuesto de consumo que se aplica si no se especifica en el producto ni se puede obtener del tipo.

        public double PorcentajeRetenciónICA { get; set; } = 0;

        public double PorcentajeRetencionesExtra { get; set; } = 0;

        public decimal MínimoRetenciónICA { get; set; } = 0;

        public decimal MínimoRetencionesExtra { get; set; } = 0;

        #endregion Propiedades y Variables>



        #region Variables Facturación Electrónica

        public AmbienteFacturaciónElectrónica AmbienteFacturaciónElectrónica { get; set; } = AmbienteFacturaciónElectrónica.Pruebas; // Código que describe el ambiente de destino donde será procesada la validación previa de los documentos electrónicos. Siempre inicia en Pruebas porque parte del proceso de habilitación de la facturación electrónica implica iniciar en modo Pruebas y después cambiarlo a Producción.

        public decimal? NúmeroAutorizaciónFacturación { get; set; } // Número del código de la resolución otorgada para la numeración de facturación.

        public DateTime? InicioAutorizaciónFacturación { get; set; } // Fecha inicial de la autorización de la numeración de facturación.

        public DateTime? FinAutorizaciónFacturación { get; set; } // Fecha final de la autorización de la numeración de facturación.

        public string? PrefijoFacturas { get; set; } // Prefijo de la autorización de numeración de facturación dado por el SIE de numeración. 

        public int? PrimerNúmeroFacturaAutorizada { get; set; } // Valor inicial del rango de numeración otorgado por la DIAN.

        public int? ÚltimoNúmeroFacturaAutorizada { get; set; } // Valor final del rango de numeración otorgado por la DIAN.

        public string? IdentificadorPruebas { get; set; } // TestSetID provisto por la DIAN para realizar las pruebas de habilitación de facturación electrónica.

        public int PróximoNúmeroDocumentoElectrónicoPruebas { get; set; } = 1; // Se usa tanto para las facturas como para las notas, no importa que en cada tipo de documento se salten números.

        public string? IdentificadorAplicación { get; set; } // Identificador de la aplicación habilitado para la emisión de facturas electrónicas. Es único para cada empresa que use SimpleOps.

        public string? PinAplicación { get; set; } // Pin elegido por el usuario que habilitó la facturación electrónica en la DIAN.

        public string? ClaveTécnicaAplicación { get; set; } // Clave dada por la DIAN asociada a la aplicación. Se obtiene con ObtenerClaveTécnicaAmbienteProducción().

        public string? NombreContactoFacturación { get; set; } // Nombre de la persona encargada de la facturación. Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public string? TeléfonoContactoFacturación { get; set; } // Teléfono de la persona encargada de la facturación. Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public string? EmailContactoFacturación { get; set; } // Email de la persona encargada de la facturación. Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public decimal ImpuestoConsumoUnitarioPredeterminado { get; set; } = 0; // El valor unitario de impuesto de consumo que se aplica si no se especifica en el producto ni se puede obtener del tipo.

        public TipoImpuestoConsumo TipoImpuestoConsumoPredeterminado { get; set; } = TipoImpuestoConsumo.General; // Se usará como predeterminado para todos los productos que tengan TipoImpuestoConsumoPropio desconocido. Nunca debe ser desconocido, si lo es saca excepción en ObtenerTipoTributo().

        public ConceptoRetención ConceptoRetenciónPredeterminado { get; set; } = ConceptoRetención.Generales; // Concepto de retención en la fuente que se usará cuando el ConceptoRetenciónPropio de un producto sea desconocido.

        public bool GenerarPDFsAdicionalesImpresión { get; set; } = true; // Si es verdadero genera PDFs adicionales de la representación gráfica de los documentos electrónicos usando solo grises y un diseño que no contiene muchos fondos de un solo color. Estos son adecuados para ser impresos en impresoras blanco y negro, pero la generación del PDF adicional hace más lento el proceso de facturación electrónica.

        public int AnchoTotalesFactura { get; set; } = 80; // Con 80 permite acomodar hasta 99 999 999 en los valores de las columnas Subtotal, Total, Consumo, IVA y en los valores de los totales de la factura: Subtotal, IVA, Impuesto Consumo, Descuento y Total. Con 90 permite hasta 999 999 999 sin que el valor quede muy cerca al de las otras columnas, pero se pierde un poco la alineación a la derecha entre los totales de la factura y los números de la última columna. 

        #endregion Variables Facturación Electrónica>



        #region Variables Comerciales

        public bool HabilitarProductosVirtuales { get; set; } = false; // Si la empresa puede proveer productos o servicios que no requieren representación o presencia física, se debe establecer esta propiedad en verdadero para que no genere error al intentar crear un cliente sin municipio. De lo contrario siempre se exigirá el municipio al crear un nuevo cliente.

        public bool PermitirTipoClienteDesconocido { get; set; } = false; // Si para la empresa no es necesario realizar una distinción especial de sus clientes (Distribuidor, Consumidor, etc), esta propiedad se debe establecer en verdadero para que no genere error al intentar agregar un cliente con tipo general. De lo contrario no se permitirá el tipo general.

        public Dictionary<TipoClienteFormaEntrega, decimal> MínimosTransporteGratis { get; set; } 
            = new Dictionary<TipoClienteFormaEntrega, decimal> {
                { TipoClienteFormaEntrega.Consumidor_Desconocida, 3000000 },
                { TipoClienteFormaEntrega.Consumidor_Otra, 3000000 },
                { TipoClienteFormaEntrega.Consumidor_TransportadoraInternacional, 3000000 },
                { TipoClienteFormaEntrega.Consumidor_Transportadora, 300000 },
                { TipoClienteFormaEntrega.Consumidor_Mensajería, 150000 },
                { TipoClienteFormaEntrega.Consumidor_PuntoVenta, 0 },
                { TipoClienteFormaEntrega.Consumidor_Virtual, 0 },
                { TipoClienteFormaEntrega.Distribuidor_Desconocida, decimal.MaxValue },
                { TipoClienteFormaEntrega.Distribuidor_Otra, decimal.MaxValue },
                { TipoClienteFormaEntrega.Distribuidor_TransportadoraInternacional, decimal.MaxValue },
                { TipoClienteFormaEntrega.Distribuidor_Transportadora, decimal.MaxValue },
                { TipoClienteFormaEntrega.Distribuidor_Mensajería, decimal.MaxValue },
                { TipoClienteFormaEntrega.Distribuidor_PuntoVenta, 0 },
                { TipoClienteFormaEntrega.Distribuidor_Virtual, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_Desconocida, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_Otra, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_TransportadoraInternacional, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_Transportadora, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_Mensajería, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_PuntoVenta, 0 },
                { TipoClienteFormaEntrega.GrandesContratos_Virtual, 0 },
                { TipoClienteFormaEntrega.Otro_Desconocida, decimal.MaxValue },
                { TipoClienteFormaEntrega.Otro_Otra, decimal.MaxValue },
                { TipoClienteFormaEntrega.Otro_TransportadoraInternacional, decimal.MaxValue },
                { TipoClienteFormaEntrega.Otro_Transportadora, decimal.MaxValue },
                { TipoClienteFormaEntrega.Otro_Mensajería, decimal.MaxValue },
                { TipoClienteFormaEntrega.Otro_PuntoVenta, 0 },
                { TipoClienteFormaEntrega.Otro_Virtual, 0 },
                { TipoClienteFormaEntrega.Desconocido_Desconocida, decimal.MaxValue },
                { TipoClienteFormaEntrega.Desconocido_Otra, decimal.MaxValue },
                { TipoClienteFormaEntrega.Desconocido_TransportadoraInternacional, decimal.MaxValue },
                { TipoClienteFormaEntrega.Desconocido_Transportadora, decimal.MaxValue },
                { TipoClienteFormaEntrega.Desconocido_Mensajería, decimal.MaxValue },
                { TipoClienteFormaEntrega.Desconocido_PuntoVenta, 0 },
                { TipoClienteFormaEntrega.Desconocido_Virtual, 0 },
            };


        public Dictionary<TipoCliente, Prioridad> PrioridadesClientes { get; set; } = new Dictionary<TipoCliente, Prioridad> {
            { TipoCliente.Desconocido, Prioridad.Baja },
            { TipoCliente.Consumidor, Prioridad.Alta },
            { TipoCliente.Distribuidor, Prioridad.Media },
            { TipoCliente.GrandesContratos, Prioridad.MuyAlta },
            { TipoCliente.Otro, Prioridad.Baja },
        };


        public Dictionary<TipoCliente, double> PorcentajesGananciaClientes { get; set; } = new Dictionary<TipoCliente, double> { // Se usa para los productos que no se encuentren en las listas de precios.
            { TipoCliente.Desconocido, 35 },
            { TipoCliente.Consumidor, 35 },
            { TipoCliente.Distribuidor, 20 },
            { TipoCliente.GrandesContratos, 35 },
            { TipoCliente.Otro, 35 },
        };


        #endregion Variables Comerciales>



        #region Variables de Apariencia y Comportamiento

        public int CopiasFacturaPredeterminada { get; set; } = 1; // Cantidad de copias físicas de la facturas para los clientes que tengan CopiasFacturaPropia nulo.

        /// <summary>
        /// Al generar la representación gráfica de la factura electrónica si el porcentaje de un impuesto (IVA o INC) difiere entre las líneas de factura
        /// y esta variable está en verdadero se agregará la columna del impuesto que difiera y la columna Total. Si esta variable es falso no se agregará
        /// la columna del impuesto ni la de total a la factura electrónica.
        /// </summary>
        public bool DetallarImpuestoSiPorcentajesDiferentes { get; set; } = true;

        /// <summary>
        /// Si es falso se omite la unidad en la representación gráfica de los documentos y en la interfaz de la aplicación. Internamente
        /// la unidad sería siempre <see cref="Unidad.Unidad"/>
        /// </summary>
        public bool MostrarUnidad { get; set; } = false;

        /// <summary>
        /// Si se establece una dirección a una página web usando dentro de ella el texto {referencia}, se agregará un enlace a esta página 
        /// en la referencia de cada producto en las representaciones gráficas de los documentos (email y pdf). Para construir la dirección
        /// se reemplaza {referencia} por la referencia actual. Por ejemplo, se puede asignar 
        /// https://ejemplo.com/Producto.aspx?Ref={Referencia} y cuando se venda el producto referencia UY597, se agregará un enlace en 
        /// el texto de UY597 en la factura electrónica que dirigirá a https://ejemplo.com/Producto.aspx?Ref=UY597.
        /// </summary>
        public string? EnlaceWebADetalleProducto { get; set; } = null;

        /// <summary>
        /// Valor calculado a partir de la base de datos cuando se cargan todos los productos. Sirve para optimizar las consultas de productos
        /// individuales, estimando la probabilidad de que el producto tenga base y por lo tanto se pueda usar un método de lectura de la 
        /// base de datos optimizado.
        /// </summary>
        public double PorcentajeProductosConBase { get; set; } = 1;

        /// <summary>
        /// Cantidad de filas por página para las páginas extra del catálogo. Las páginas extra son las que no tienen diseño HTML personalizado. 
        /// Todo el catálogo podría estar compuesto de solo páginas extra si no se desea realizar un diseño HTML propio. Este valor predeterminado
        /// puede ser ignorado pasando su valor en el archivo de integración.
        /// </summary>
        public int CantidadFilasProductosPorPáginaExtraCatálogo { get; set; } = 4;

        /// <summary>
        /// Cantidad de columnas por página para las páginas extra del catálogo. Las páginas extra son las que no tienen diseño HTML personalizado. 
        /// Todo el catálogo podría estar compuesto de solo páginas extra si no se desea realizar un diseño HTML propio. Este valor predeterminado
        /// puede ser ignorado pasando su valor en el archivo de integración.
        /// </summary>
        public int CantidadColumnasProductosPorPáginaExtraCatálogo { get; set; } = 2;

        /// <summary>
        /// Si es 0, las páginas extra se insertan al final del documento. Si es 1, las páginas extra se insertan antes de la última página
        /// para permitir que esta última página sea la contraportada del catálogo.
        /// </summary>
        public int ÍndiceInversoInserciónPáginasExtraCatálogo { get; set; } = 1;

        /// <summary>
        /// El tamaño al que se dimensionarán las imágenes originales para ser usadas en catálogos y cotizaciones. Entre más grande sea este valor, mejor 
        /// calidad de imágenes tendrán los documentos gráficos, pero mayor será el tamaño del archivo.
        /// </summary>
        public int TamañoImágenesProductosCotizaciones { get; set; } = 200;

        /// <summary>
        /// El tamaño al que se dimensionarán las imágenes originales para ser usadas en las fichas informativas. Entre más grande sea este valor, mejor 
        /// calidad de imágenes tendrán los documentos gráficos, pero mayor será el tamaño del archivo.
        /// </summary>
        public int TamañoImágenesProductosFichas { get; set; } = 600;

        /// <summary>
        /// Cantidad de filas por página para las páginas de las cotizaciones.
        /// </summary>
        public int CantidadFilasProductosPorPáginaCotización { get; set; } = 10;

        /// <summary>
        /// Cantidad de columnas por página para las páginas de las cotizaciones.
        /// </summary>
        public int CantidadColumnasProductosPorPáginaCotización { get; set; } = 1;

        #endregion Variables de Apariencia y Comportamiento>



        #region Valores Predeterminados

        /// <summary>
        /// Se usa esta unidad cuando no se ha establecido su valor. Es necesario manejarla por aparte porque es necesario asignar el valor 
        /// Unidad.Desconocida a Producto.UnidadEspecífica para que esta no reemplace la unidad del producto base.
        /// </summary>
        public Unidad UnidadPredeterminadaProducto = Unidad.Unidad;

        /// <summary>
        /// Se usa esta unidad cuando no se ha establecido su valor. Es necesario manejarla por aparte porque es necesario asignar el valor 
        /// Unidad.Desconocida a Producto.UnidadEmpaqueEspecífica para que esta no reemplace la unidad de empaque en el producto base.
        /// </summary>
        public Unidad UnidadEmpaquePredeterminadaProducto = Unidad.Unidad;

        /// <summary>
        /// Determina si por defecto los productos son o no excluídos de IVA. Se usa este valor cuando no se ha establecido su valor. 
        /// Es necesario manejarlo por aparte porque es necesario asignar el valor null a Producto.ExcluídoIVA para que este no reemplace 
        /// el ExcluídoIVA del producto base.
        /// </summary>
        public bool ExcluídoIVAPredeterminadoProducto = false;

        /// <summary>
        /// Determina si por defecto los productos son o no físicos. Un producto físico es del que se puede mantener un inventario. Se usa este valor 
        /// cuando no se ha establecido su valor. Es necesario manejarlo por aparte porque es necesario asignar el valor null a Producto.Físico 
        /// para que este no reemplace el Físico del producto base.
        /// </summary>
        public bool FísicoPredeterminadoProducto = true;

        /// <summary>
        /// Al habilitar los productos base la tabla productos se divide en dos tablas: Productos y ProductosBase. Esto permite que algunos productos 
        /// que solo difieran en ciertos atributos menores (talla, color, etc) puedan compartir un producto base y tomar de este los valores de algunas 
        /// de sus propiedades comunes (marca, descripción, etc). Así se facilita el mantenimiento de los datos porque en caso de requerir un cambio 
        /// solo se tendría que hacer en el producto base, pero reduce un poco el rendimiento de las consultas de productos a la base de datos. 
        /// Si se identifican problemas de rendimiento y para el caso de uso particular no se necesitan los productos base, se se puede establecer 
        /// este valor en falso. Después, en caso de necesitar la funcionalidad de productos base, se puede establecer en verdadero en cualquier momento.
        /// Esta variable no afecta la estructura de la base de datos, solo afecta la manera en la que se hacen las consultas a ella y permite realizar 
        /// algunas validaciones adicionales durante la ejecución.
        /// </summary>
        public bool HabilitarProductosBase = true;

        /// <summary>
        /// Si está habilitado el uso de los productos base y específicos y este valor es verdadero, se permitirá la asignación de atributos que no 
        /// estén en la tabla AtributosProductos a los productos específicos. Si este valor es falso y se intenta agregar un atributo que no está en
        /// la tabla AtributosProductos, el procedimiento fallará. Incluso si este valor es verdadero en la interfaz de usuario se sugerirá al 
        /// usuario a no usar atributos libres, pero se le permite hacerlo. Si este valor está en falso, no estará disponible la función en la 
        /// interfaz de usuario.
        /// </summary>
        public bool PermitirAtributosProductosLibres = true;

        /// <summary>
        /// Los atributos de estos tipos se resumen usando la palabra 'a' si están consecutivos.
        /// </summary>
        public List<string> TiposAtributosSecuenciales = new List<string> { "Talla Numérica", "Talla Alfabética", "Copa" };

        /// <summary>
        /// Los atributos de este tipo podrán contener finalización en ,5 o .5 para obtener tallas intermedias a las enteras y estas 
        /// serán consideradas correctamente en la secuencia al igual que también lo serán los números enteros entre ellas. Por ejemplo,
        /// si existen estos atributos de tipo Talla Numérica: 11, 11.5, 12, 12.5, 13, 13.5 y 14. Los siguientes grupos de atributos se resumirán
        /// correctamente a rango completo: 12, 13, 14 => 12 a 14 y 12, 12.5, 13, 13.5, 14 => 12 a 14.
        /// </summary>
        public string NombreTipoAtributoTallaNumérica = "Talla Numérica";

        /// <summary>
        /// En los rangos en los que se permite el paso doble en la secuencia de atributos de talla numérica, el rango se considerará completo
        /// incluso si salta 2 valores, así: 38, 40, 42, 44 => 38 a 44.
        /// </summary>
        public Dictionary<string, string> RangosDoblePasoEnSecuenciaTallaNumérica = new Dictionary<string, string> { { "Talla 0", "Talla 80" } }; // Si está el rango de Talla 0 a Talla 80, se permite doble paso en todas las tallas.

        /// <summary>
        /// Entre estos rangos las tallas númericas contienen tallas medias. Esta información es necesaria para permitir secuencias con pasos hasta de
        /// a 2 en estos rangos o hasta de a 4 si también se está en un <see cref="RangosDoblePasoEnSecuenciaTallaNumérica"/>. 
        /// Se establecen los rangos desde las tallas medias iniciales y finales de cada rango.
        /// </summary>
        public Dictionary<string, string> RangosTallasMediasNuméricas = new Dictionary<string, string> { { "Talla 0.5", "Talla 14.5" },
            { "Talla 33.5", "Talla 44.5" } };

        #endregion Valores Predeterminados>



        #region Propiedades Autocalculadas

        [JsonIgnore]
        public string? DirecciónUbicaciónEfectiva => DirecciónUbicación ?? DirecciónFacturación;

        [JsonIgnore]
        public int MunicipioUbicaciónEfectivoID => MunicipioUbicaciónID ?? MunicipioFacturaciónID;

        /// <summary>
        /// El nombre de la empresa. Se usa el nombre comercial si no es nulo, si es nulo se usa la razón social.
        /// </summary>
        [JsonIgnore]
        public string? Nombre => NombreComercial ?? RazónSocial;

        [JsonIgnore]
        public string? DígitoVerificaciónNit => ObtenerDígitoVerificación(Nit);

        [JsonIgnore]
        public string? NitCompleto => $"{Nit}-{DígitoVerificaciónNit}";

        [JsonIgnore]
        public string CódigoDocumentoIdentificación => ObtenerDocumentoIdentificación(TipoEntidad).AValor();

        [JsonIgnore]
        public double PorcentajeIVAPredeterminadoEfectivo => TipoContribuyente.HasFlag(TipoContribuyente.NoResponsableIVA) ? 0 : PorcentajeIVAPredeterminado;

        [JsonIgnore]
        public string TipoContribuyenteTexto => TipoContribuyente.ATexto().Reemplazar("Ordinario, ", ""); // Se omite el Ordinario si va acompañado de otro tipo como responsable IVA porque el ordinario es el más común y para las personas puede ser redundante y confuso. Es más usual especificar si una empresa es Régimen Simple o Gran Contribuyente y no especificar nada cuando no sea ninguno de los dos.

        #endregion Propiedades Autocalculadas>



    } // OpcionesEmpresa>



} // SimpleOps.Singleton>
