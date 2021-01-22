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
        // Tomado de https://csharpindepth.com/Articles/Singleton.

        private static readonly Lazy<OpcionesEmpresa> DatosLazy = new Lazy<OpcionesEmpresa>(() => new OpcionesEmpresa());

        public static OpcionesEmpresa Datos { get { return DatosLazy.Value; } } // Normalmente esta sería la variable que se accede pero se prefiere hacer una variable auxiliar Empresa en Global.cs para tener un acceso más fácil sin necesidad de escribir OpcionesEmpresa.Datos.

        private OpcionesEmpresa() { }

        #endregion Patrón Singleton>


        #region Propiedades y Variables

        public Municipio MunicipioFacturación = null!; // Es un clon de solo lectura. Se garantiza no será nulo porque siempre se carga al iniciar. Datos del municipio de la dirección de facturación de la empresa. Se actualiza al iniciar SimpleOps, al cambiar Empresa.Datos.MunicipioFacturaciónID y al realizar cambios en la tabla municipios. Para que funcione la factura electrónica debe tener un departamento de Colombia y este corresponder a uno de los valores de la columna Nombre en el numeral 13.4.2 de la documentación de factura electrónica. Aunque si no corresponde no genera Rechazo si no Notificación. El código del municipio debe corresponder a un valor válido de lista de municipios en el numeral 13.4.3 de la documentación de factura electrónica de la DIAN. Para que funcione la factura electrónica con la DIAN debe ser un municipio de Colombia y su nombre corresponder a uno de los valores del la columna Nombre Municipio en el numeral 13.4.3 de la documentación de factura electrónica. Aunque si no corresponde no genera Rechazo si no Notificación.

        private int municipioFacturaciónID = 1; // ID del municipio de la dirección de facturación. 1: Bogotá, 2: Medellín, 3: Cali, 4: Barranquilla, etc. Ver tabla municipios. Siempre será establecido. Si no se establece inicia por defecto en Bogotá.
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

        public string? NombreComercial { get; set; } // Se usa para notificar a la DIAN en la factura electrónica y para usarlo en representación gráfica de las facturas. Si es nulo se omite en la factura electrónica y en la representación gráfica se usa la razón social.

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

        public string? ClaveTécnicaAplicación { get; set; } // Clave dada por la DIAN al habilitar la facturación electrónica.

        public string? NombreContactoFacturación { get; set; } // Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public string? TeléfonoContactoFacturación { get; set; } // Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public string? EmailContactoFacturación { get; set; } // Se usa para escribirlo en la factura electrónica. No es obligatorio, pero los clientes podrían hacer uso de él.

        public decimal ImpuestoConsumoUnitarioPredeterminado { get; set; } = 0; // El valor unitario de impuesto de consumo que se aplica si no se especifica en el producto ni se puede obtener del tipo.

        public TipoImpuestoConsumo TipoImpuestoConsumoPredeterminado { get; set; } = TipoImpuestoConsumo.General; // Se usará como predeterminado para todos los productos que tengan TipoImpuestoConsumoPropio desconocido. Nunca debe ser desconocido, si lo es saca excepción en ObtenerTipoTributo().

        public ConceptoRetención ConceptoRetenciónPredeterminado { get; set; } = ConceptoRetención.Generales; // Concepto de retención en la fuente que se usará cuando el ConceptoRetenciónPropio de un producto sea desconocido.

        public bool GenerarPDFsAdicionalesImpresión { get; set; } = true; // Si es verdadero genera PDFs adicionales de la representación gráfica de los documentos electrónicos usando solo grises y un diseño que no contiene muchos fondos de un solo color. Estos son adecuados para ser impresos en impresoras blanco y negro, pero la generación del PDF adicional hace más lento el proceso de facturación electrónica.

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

        #endregion Variables de Apariencia y Comportamiento>


        #region Propiedades Autocalculadas>

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
