using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Dian.Factura; // Si se encuentra error aquí se debe agregar la referencia a Dian.dll (generado con Dian.sln) que se encuentra en \SimpleOps\. Esta librería se maneja independiente para evitar disminuir el rendimiento de Visual Studio con las clases enormes y autogeneradas que contiene.
using static SimpleOps.Legal.Dian;
using static SimpleOps.Global;
using static Vixark.General;
using System.Globalization;
using SimpleOps.Datos;
using System.Linq;
using SimpleOps.Modelo;
using System.Diagnostics;



namespace SimpleOps.Legal {



    class DocumentoElectrónico<D, M> where D : Factura<Cliente, M> where M : MovimientoProducto {


        #region Propiedades

        public D Documento { get; set; }

        public TipoDocumentoElectrónico Tipo { get; set; }

        public TipoFirma TipoFirma { get; set; }

        public string? CódigoFacturaContingencia { get; set; }

        public DateTime? FechaFacturaContingencia { get; set; }

        /// <summary>
        /// Código Único de Documento Electrónico. Si es venta se llama CUFE.
        /// </summary>
        public string Cude { get; set; }

        public decimal Anticipo { get; set; }

        public string RutaDocumentosElectrónicosHoy { get; set; } // Se maneja como una propiedad para asegurar que su valor es el mismo durante el tiempo de vida del objeto y evitar casos especiales en los que se inicie el procesamiento antes de media noche, se finalice después y no se pueda realizar porque no encuentre el archivo sin firma. Además, porque también se usa para poder guardar la representación gráfica en PDF en la misma carpeta que el documento electrónico.

        /// <summary>
        /// Ruta del XML del documento electrónico firmado.
        /// </summary>
        public string? Ruta { get; set; }

        public string? RutaSinFirmar { get; set; }

        public bool IdentarXml = false; // Activar para generar los XMLs con formato identado para facilitar la comparación con XMLs de muestra. Se debe desactivar en producción.

        #endregion Propiedades>


        #region Constructores


        public DocumentoElectrónico(D documento, TipoFacturaVenta tipoFactura, string? códigoFacturaContingencia = null, 
            DateTime? fechaFacturaContingencia = null) {

            (Documento, CódigoFacturaContingencia, FechaFacturaContingencia) = (documento, códigoFacturaContingencia, fechaFacturaContingencia);
            Tipo = documento switch {
                Venta _ =>
                    tipoFactura switch {
                        TipoFacturaVenta.Venta => TipoDocumentoElectrónico.FacturaVenta,
                        TipoFacturaVenta.ContingenciaDian => TipoDocumentoElectrónico.FacturaContingenciaDian,
                        TipoFacturaVenta.ContingenciaFacturador => TipoDocumentoElectrónico.FacturaContingenciaFacturador,
                        TipoFacturaVenta.Exportación => TipoDocumentoElectrónico.FacturaExportación,
                    },
                NotaCréditoVenta _ => TipoDocumentoElectrónico.NotaCrédito,
                NotaDébitoVenta _ => TipoDocumentoElectrónico.NotaDébito,
                _ => throw new Exception(CasoNoConsiderado(documento.GetType().ToString()))
            };

            if (documento is Venta venta) Anticipo = venta.ObtenerAnticipo();
            Cude = documento.Cude!; // Se asegura que no es nulo porque si se ejecutó CalcularTodo() antes de iniciar el envío del documento electrónico.
            RutaDocumentosElectrónicosHoy = ObtenerRutaDocumentosElectrónicosDeHoy();

            TipoFirma = Tipo switch {
                TipoDocumentoElectrónico.FacturaVenta => TipoFirma.Factura,
                TipoDocumentoElectrónico.FacturaExportación => TipoFirma.Factura,
                TipoDocumentoElectrónico.FacturaContingenciaFacturador => TipoFirma.Factura,
                TipoDocumentoElectrónico.FacturaContingenciaDian => TipoFirma.Factura,
                TipoDocumentoElectrónico.NotaCrédito => TipoFirma.NotaCrédito,
                TipoDocumentoElectrónico.NotaDébito => TipoFirma.NotaDébito,
            };

        } // DocumentoElectrónico>


        #endregion Constructores>


        #region Propiedades Autocalculadas

        public string TextoTipo => Tipo.AValor(largoForzado: 2);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones


        /// <summary>
        /// Devuelve verdadero si se creo exitosamente.
        /// </summary>
        public bool Crear(out string? mensaje) { // En los comentarios de cada elemento se indica su código en la documentación de la DIAN y su obligatoriedad o no y la cantidad permitida. Por ejemplo, 2..5 significa obligatorio 2 y 5 máximos, 0..3 significa que no es obligatorio y que pueden ser hasta 3 elementos. En los casos que aplique se valida su tamaño en la asignación del valor usando las restricciones provistas por la documentación de la DIAN y la función Dian.Validar().

            mensaje = null;

            // Verificaciones
            if (Empresa.Nit == null || Empresa.DígitoVerificaciónNit == null) 
                return Falso(out mensaje, "No se ha establecido el Nit de la empresa."); // También se verifica el DígitoVerificaciónNit aunque es redundante para eliminar advertencias de que pueda ser nulo en el resto del código.
            if (Empresa.DirecciónFacturación == null || Empresa.DirecciónUbicaciónEfectiva == null) 
                return Falso(out mensaje, "No se ha establecido la dirección de facturación de la empresa."); // La verificación de DirecciónUbicaciónEfectiva es redundante porque se asegura con Dirección pero se hace para omitir advertencias en el resto del código.
            if (Empresa.RazónSocial == null) return Falso(out mensaje, "No se ha establecido la razón social de la empresa.");
            if (Empresa.NúmeroAutorizaciónFacturación == null) 
                return Falso(out mensaje, "No se ha establecido el número de autorización de facturación de parte de la DIAN a la empresa.");
            if (Empresa.InicioAutorizaciónFacturación == null || Empresa.FinAutorizaciónFacturación == null)
                return Falso(out mensaje, "No se ha establecido la fecha de inicio o final de la autorización de facturación de parte de la DIAN " + 
                    "a la empresa.");
            if (Empresa.PrimerNúmeroFacturaAutorizada == null || Empresa.ÚltimoNúmeroFacturaAutorizada == null)
                return Falso(out mensaje, "No se ha establecido el primer o el último número de facturación autorizado de parte de la DIAN a " + 
                    "la empresa.");
            if (Empresa.IdentificadorAplicación == null) 
                return Falso(out mensaje, "No se ha establecido el identificador de la aplicación en la DIAN.");
            if (Empresa.PinAplicación == null) return Falso(out mensaje, "No se ha establecido el pin de la aplicación en la DIAN.");
            if (Empresa.ClaveTécnicaAplicación == null) return Falso(out mensaje, "No se ha establecido la clave técnica de la aplicación en la DIAN.");
            if (Documento.Líneas.Any(d => d.Producto == null))
                return Falso(out mensaje, "No se han cargado todos los productos de las líneas de la factura");
            if (Documento.Líneas.Any(d => d.Producto?.Descripción == null)) 
                return Falso(out mensaje, "No se ha establecido la descripción para al menos un producto.");
            if (!ExisteRuta(TipoElementoRuta.Archivo, Equipo.RutaCertificado, "certificado de firma digital", out string? mensajeExiste)) 
                return Falso(out mensaje, mensajeExiste);
            if (Documento.ConsecutivoDianAnual == null) return Falso(out mensaje, $"El consecutivo de la DIAN anual no puede ser nulo.");
            // Verificaciones>

            // Variables Auxiliares
            var cliente = Documento.EntidadEconómica; // Variable auxiliar para acceder más fácilmente a sus datos.
            if (cliente == null) return Falso(out mensaje, "No se ha cargado el cliente.");

            var venta = Documento as Venta;
            var notaCrédito = Documento as NotaCréditoVenta;
            var notaDébito = Documento as NotaDébitoVenta;
            
            if (venta == null && notaCrédito == null && notaDébito == null)
                return Falso(out mensaje, "El tipo de documento no es venta ni nota crédito ni nota débito.");

            var ventaNota = notaCrédito?.Venta ?? notaDébito?.Venta;

            var fechaHora = Documento.FechaHora;
            var tipoImpuesto = Documento.IVA > 0 ? (Documento.ImpuestoConsumo > 0 ? TipoImpuesto.IVAeINC : TipoImpuesto.IVA) 
                : (Documento.ImpuestoConsumo > 0 ? TipoImpuesto.INC : TipoImpuesto.NoAplica);
            var (códigoImpuestoPredominante, nombreImpuestoPredominante) = tipoImpuesto switch { // Parece que la DIAN cambió su lógica para los campos FAK40 y FAK41... y ahora si que se entiende menos porque solo hay posibilidad de poner un tributo entonces no hay claridad que se hace cuando son varios. Para evitar enredos se usará IVA cuando hay algún IVA, INC si hay INC y no IVA y otro cuando no hay ni IVA ni INC.
                TipoImpuesto.IVA => ("01", "IVA"),
                TipoImpuesto.INC => ("04", "INC"),
                TipoImpuesto.IVAeINC => ("01", "IVA"),
                _ => ("ZZ", "Otro")
            };

            string moneda = Generales.Moneda;
            // Variables Auxiliares>

            // Verificaciones Iniciales
            if (fechaHora < Empresa.InicioAutorizaciónFacturación || fechaHora > Empresa.FinAutorizaciónFacturación)
                return Falso(out mensaje, "La fecha de emisión de la factura es inconsistente con las fechas de autorización de numeración.");
            if (string.IsNullOrEmpty(cliente.NitLegalEfectivo))
                return Falso(out mensaje, $"No se puede facturar a un cliente sin NIT. Cliente: {cliente.Nombre}"); // Este error no debería ocurrir porque desde la interface se debe garantizar que los clientes a los que se les vaya a vender tengan NIT.
            if (cliente.TipoEntidad == TipoEntidad.Desconocido)
                return Falso(out mensaje, $"No se puede facturar a un cliente del que no se sabe si es una persona natural o una empresa. " + 
                    $"Cliente: {cliente.Nombre}"); // Este error no debería ocurrir porque desde la interface se debe garantizar que los clientes a los que se les vaya a vender se les conozca si son personas naturales o jurídicas.
            // Verificaciones Iniciales>

            // Factura Originadada por Notas - No se entendió muy bien su uso entonces se desactivan las variables de prueba y el código que las usaba.
            // var facturaOriginadaPorNotaDébito = false; // Si es verdadero se usa un grupo de información exclusivo para referenciar la nota débito que dio origen a la presente factura electrónica. Se debe diligenciar únicamente cuando la FE se origina a partir de la corrección o ajuste que se da mediante una nota débito.
            // var númeroNotaDébitoOriginadora = "ND8941"; // Prefijo + Número de la nota débito relacionada. Rechazo: Si el ID de la nota débito de referencia no existe.
            // var cudeNotaDébitoOriginadora = "941cf36af62dbbc06f105d2a80e9bfe683a90e84960eae4d351cc3afbe8f848c26c39bac4fbc80fa254824c6369ea694"; // CUDE de la nota débito relacionada. Rechazo: Si el CUDE de la nota débito referenciada no existe.
            // var fechaNotaDébitoOriginadora = fechaHora.AddDays(-7); // Fecha de emisión de la nota débito relacionada si la fecha de la nota débito referenciada es posterior a Invoice / cbc:IssueDate.
            // if (fechaNotaDébitoOriginadora > fechaHora)
            //    return Falso(out mensaje, "La fecha de la nota débito generadora de la factura no puede ser mayor a la fecha de la factura.");
            // var facturaOriginadaPorNotaCrédito = false; // Si es verdadero se usa un grupo de información exclusivo para referenciar la nota crédito que dio origen a la presente factura electrónica. Se debe diligenciar únicamente cuando la FE se origina a partir de la corrección o ajuste que se da mediante una nota crédito.
            // var númeroNotaCréditoOriginadora = "NC8941"; // Prefijo + Número de la nota Crédito relacionada. Rechazo: Si el ID de la nota crédito de referencia no existe.
            // var cudeNotaCréditoOriginadora = "941cf36af62dbbc06f105d2a80e9bfe683a90e84960eae4d351cc3afbe8f848c26c39bac4fbc80fa254824c6369ea694"; // CUDE de la nota crédito relacionada. Rechazo: Si el CUDE de la nota crédito referenciada no existe.
            // var fechaNotaCréditoOriginadora = fechaHora.AddDays(-7); // Fecha de emisión de la nota crédito relacionada si la fecha de la nota crédito referenciada es posterior a Invoice / cbc:IssueDate.
            // if (fechaNotaCréditoOriginadora > fechaHora)
            //    return Falso(out mensaje, "La fecha de la nota Crédito generadora de la factura no puede ser mayor a la fecha de la factura.");
            // Factura Originadada por Notas>

            #region Funciones Locales - El código que se muestra en comentarios es el de su primera aparición.

            string obtenerCódigoSeguridad() => ObtenerSHA384($"{Empresa.IdentificadorAplicación}{Empresa.PinAplicación}{Documento.Código}");  // También llamada Huella, aunque antes era única para la aplicación ya no lo es porque depende también del número del documento.


            static AddressType? obtenerAddress(DirecciónCompleta? direcciónCompleta) {

                if (direcciónCompleta == null) return null;
                var municipio = direcciónCompleta.Municipio;
                if (municipio == null || municipio.CódigoPaís == null || municipio.CódigoLenguajePaís == null || municipio.Código == null) return null; // Si no se conoce el país, el lenguaje del país o el código del municipio no se agregará el Address. Este elemento no siempre es obligatorio.

                return new AddressType { // 1..1 FAJ08.
                    ID = new IDType { Value = Validar(municipio.Código, "5") }, // 1..1 FAJ09.
                    CityName = new CityNameType { Value = Validar(municipio.Nombre, "1..60", true) }, // 1..1 FAJ10.
                    // PostalZone = , // 0..1 FAJ73 T1..10. Código postal. Ver lista de valores posibles en el numeral 13.4.4. Por lo general no se tiene ni se usa.
                    CountrySubentity = new CountrySubentityType { Value = Validar(municipio.Departamento, "1..60", true) }, // 1..1 FAJ11.
                    CountrySubentityCode = new CountrySubentityCodeType { Value = Validar(municipio.CódigoDepartamento, "1..5") }, // 1..1 FAJ12.
                    AddressLine = new AddressLineType[1] { // 1..N FAJ13.
                    new AddressLineType { Line = new LineType { Value = Validar(direcciónCompleta.Dirección ?? " ", "1..300", true) } } // 1..1 FAJ14. Elemento de texto libre, que el emisor puede elegir utilizar para poner toda la información de su dirección, en lugar de utilizar elementos estructurados (los demás elementos de este grupo). Informar la dirección, sin ciudad ni departamento. Estos dos textos de la documentación de la DIAN son contradictorios entonces se escribirá la dirección en este campo y se completarán los otros normalmente. Debido a que es un elemento obligatorio de largo 1 si por alguna razón no se dispone de la dirección se enviará un espacio en blanco.
                },
                    Country = new CountryType { // 1..1 FAJ15.
                        IdentificationCode = new IdentificationCodeType { Value = Validar(municipio.CódigoPaís, "1..3") }, // 1..1 FAJ16. Debe informar literal "CO". El tamaño en la documentación dice 3 pero no puede ser 3, debe ser flexible para no contradecir el requerimiento de poner "CO".
                        Name = new NameType1 {
                            Value = Validar(municipio.País, "4..41", true), // 0..1 FAJ17. Debe informar el literal "Colombia".
                            languageID = Validar(municipio.CódigoLenguajePaís, "2"), // 0..1 FAJ18.
                        },
                    },
                };

            } // ObtenerAddress>


            static TaxTotalType? obtenerTaxTotal(List<LíneaDocumentoElectrónico<M>> líneas, TipoTributo tipoTributo, out bool éxito, 
                out string? mensaje) {

                éxito = true;
                mensaje = null;

                var moneda = Generales.Moneda;
                var totalImpuesto = líneas.Sum(d => d.TipoTributo == tipoTributo ? d.Impuesto : 0);
                if (totalImpuesto == 0) return null; // Para no agregar impuestos que no están en el total de líneas. Es necesario cuando se usa la función para la línea que no debe agregar elementos para impuestos que no le aplican a la línea actual así este impuesto aplique a otras líneas. En el caso de los impuestos generales no es tan importante porque se hace un prefiltrado previo de los tributos disponibles.

                var taxTotalType = new TaxTotalType { 
                    TaxAmount = new TaxAmountType { 
                        Value = Validar(totalImpuesto, "0..15 p (2..6)", 2), // 1..1 FAS02, FAX02. 
                        currencyID = moneda // 1..1 FAS03, FAX03.
                    } 
                };

                var taxSubtotales = new List<TaxSubtotalType>();
                foreach (var línea in líneas) {

                    var tarifaImpuestoPorcentual = línea.TarifaImpuestoPorcentual;
                    if (línea.ModoImpuesto == ModoImpuesto.Porcentaje) {

                        switch (tipoTributo) {
                            case TipoTributo.IVA:

                                if (!Enum.IsDefined(typeof(TarifaIVA), (int)tarifaImpuestoPorcentual)) {
                                    éxito = Falso(out mensaje, $"El porcentaje {tarifaImpuestoPorcentual} no es válido para el IVA.");
                                    return null;
                                }
                                break;

                            case TipoTributo.INC:

                                if (!Enum.IsDefined(typeof(TarifaINC), (int)tarifaImpuestoPorcentual)) {
                                    éxito = Falso(out mensaje, $"El porcentaje {tarifaImpuestoPorcentual} no es válido para el INC.");
                                    return null;
                                }
                                break;

                            default:
                                break; // En los otros casos no se verificará porque aún no se ha implementado la enumeración de tarifas. Si sucede error sucederá en la validación de la DIAN.
                        }

                    }

                    var taxSubtotal = new TaxSubtotalType { // 1..N FAS04, FAX04.      
                        TaxAmount = new TaxAmountType { Value = Validar(línea.Impuesto, "0..15 p (2..6)", 2), currencyID = moneda }, // 1..1 FAS07, FAX07. currencyID: 1..1 FAS08, FAX08.
                        TaxCategory = new TaxCategoryType { // 1..1 FAS13, FAX13.
                            TaxScheme = new TaxSchemeType { // 1..1 FAS15, FAX15.
                                ID = new IDType { Value = Validar(ObtenerCódigoTipoTributo(tipoTributo), "2..10") }, // 1..1 FAS16, FAX16. Aunque la documentación dice tamaño 3..10 esto va en contradicción con el código "ZZ", se usa entonces 2..10.
                                Name = new NameType1 { Value = Validar(tipoTributo.ATexto(), "2..30") }, // 1..1 FAS17, FAX17. Aunque la documentación dice tamaño 10..30 esto va en contradicción con el nombre "IC", se usa entonces 2..30.
                            }
                        },
                    };

                    switch (línea.ModoImpuesto) {
                        case ModoImpuesto.Exento:
                            break;
                        case ModoImpuesto.Porcentaje:

                            taxSubtotal.TaxableAmount = new TaxableAmountType { 
                                Value = Validar(línea.SubtotalReal, "0..15 p (2..6)", 2), currencyID = moneda // 1..1 FAS05, FAX05. currencyID: 1..1 FAS06, FAX06. En el caso de que el tributo sea una porcentaje del valor tributable informar la base imponible en valor monetario.
                            };
                            taxSubtotal.TaxCategory.Percent = new PercentType1 { 
                                Value = Validar(tarifaImpuestoPorcentual, "0..5 p (0..3)", 2) // 0..1 FAS14, FAX14. Aunque en la tabla 13.3.9 no mencionan el 0 como un valor válido para el INC se permitirá el 0 porque se muestra en el ejemplo de XML.
                            }; 
                            break;

                        case ModoImpuesto.Unitario:

                            taxSubtotal.TaxableAmount = new TaxableAmountType { Value = Validar(línea.SubtotalReal, "0..15 p (2..6)", 2) }; // 1..1 FAS05, FAX05. currencyID: 1..1 FAS06, FAX06. Según la documentación en el caso que el tributo sea un valor fijo por unidad tributada se debería informar el número de unidades tributadas (línea.Cantidad), pero el servidor de la DIAN lo rechaza con este error: Rechazo FAU04: Base Imponible es distinto a la suma de los valores de las bases imponibles de todas líneas de detalle, se usa entonces el SubtotalReal normal como en el caso de impuestos porcentuales.
                            taxSubtotal.BaseUnitMeasure = new BaseUnitMeasureType { 
                                Value = Validar(1, "0..2 p (0..2)", 0), unitCode = ObtenerCódigoUnidad(Unidad.Unidad) // 0..1 FAS09, FAX09. Unidad de medida base para el tributo. Usado en el caso que el tributo sea un valor fijo por unidad tributada. Por ejemplo, el impuesto de consumo a las bolsas o los impuestos a los combustibles. 1..1 FAS10, FAX10. Se asumirá que siempre el impuesto es unitario para establecer el código de la unidad, es complicado establecer el código correcto según todos los tipos posibles de impuestos.
                            }; 
                            taxSubtotal.PerUnitAmount = new PerUnitAmountType { 
                                Value = Validar(línea.TarifaImpuestoUnitario, "0..15 p (0..2)", 2), currencyID = moneda // 0..1 FAS11, FAX11. currencyID: 1..1 FAS12, FAX12. Valor del tributo por unidad. 
                            };
                            break;

                        default:
                            throw new Exception(CasoNoConsiderado(línea.ModoImpuesto));
                    }

                    taxSubtotales.Add(taxSubtotal);

                }

                taxTotalType.TaxSubtotal = taxSubtotales.ToArray();
                return taxTotalType;

            } // TaxTotalType>


            static TaxTotalType[]? obtenerTaxTotales(List<M> líneas, out bool éxito, out string? mensaje, 
                AgrupaciónTaxTotales agrupaciónTaxTotales) {

                éxito = true;
                mensaje = null;

                var tiposTributo = líneas.Select(d => d.Producto!.TipoTributoConsumo).Distinct().ToList(); // Ya se verificó al inicio del procedimiento que ningúna línea.Producto fuera nulo.
                tiposTributo.Insert(0, TipoTributo.IVA); // Se agrega el IVA porque en la línea anterior solo se agregaron los diferentes tipos tributo de consumo. Se agrega al inicio solo por convención.
                var taxTotales = new List<TaxTotalType>(); // 0..N FAS01, FAX01.

                foreach (var tipoTributo in tiposTributo) { // Se garantiza que cada producto solo tiene un solo impuesto a consumo por lo tanto el valor subtotal de cada producto es igual al valor subtotal del tributo de la línea.

                    TaxTotalType? taxTotal = null;
                    var éxitoTributo = true;
                    string? mensajeTributo = null;

                    switch (agrupaciónTaxTotales) {
                        case AgrupaciónTaxTotales.Línea:

                            taxTotal = obtenerTaxTotal(LíneaDocumentoElectrónico<M>.CrearLista(líneas, tipoTributo), tipoTributo, 
                                out éxitoTributo, out mensajeTributo);
                            break;

                        case AgrupaciónTaxTotales.Tarifa:

                            var modosImpuesto = new List<ModoImpuesto> { ModoImpuesto.Porcentaje, ModoImpuesto.Unitario };
                            var líneasTarifas = new List<LíneaDocumentoElectrónico<M>>();

                            foreach (var modoImpuesto in modosImpuesto) { // Se realiza de manera independiente para impuestos porcentuales e impuestos unitarios porque se podría generar colisión en el valor de la tarifa al agruparlos por tarifa sin estar relacionados y porque a cada línea de tarifa (consolidacón por varios productos) se le debe agregar el modo impuesto el cual depende del producto, entonces se requiere que todos tengan el mismo modo impuesto para poder asignarle un valor único.

                                var tarifasYLíneas = tipoTributo switch {
                                    TipoTributo.IVA 
                                      => líneas.Where(d => d.ObtenerModoImpuesto(tipoTributo) == modoImpuesto)
                                             .GroupBy(d => d.ObtenerTarifaImpuesto(tipoTributo)).ToDictionary(g => g.Key, g => g.ToList()), // Diccionario que agrupa los productos por en grupos por su tarifa efectiva de IVA. La agrupación por modo de impuesto es necesaria para que solo obtenga líneas válidas en el caso de modo porcentual.
                                    _ => líneas.Where(d => d.ObtenerModoImpuesto(tipoTributo) == modoImpuesto && 
                                             d.Producto!.TipoTributoConsumo == tipoTributo).GroupBy(d => d.ObtenerTarifaImpuesto(tipoTributo))
                                             .ToDictionary(g => g.Key, g => g.ToList()) // Diccionario que agrupa los productos por en grupos por su tarifa efectiva de impuesto al consumo. Es necesario filtrar por el modo de impuesto porque el impuesto al consumo puede ser porcentual o unitario y es necesario filtrar por el tipo tributo porque sin este coincidirían todos los impuestos al consumo en cada tipo tributo.
                                };
   
                                foreach (var tarifaYLíneas in tarifasYLíneas) {

                                    var líneaTarifa = new LíneaDocumentoElectrónico<M> { ModoImpuesto = modoImpuesto, TipoTributo = tipoTributo}; 
                                    foreach (var líneaPorTarifa in tarifaYLíneas.Value) { // Suma el subtotal y el impuesto de todos los productos que tengan la misma tarifa, el mismo tipo impuesto y el mismo modo de impuesto.
                                        líneaTarifa.Impuesto += líneaPorTarifa.ObtenerValorImpuesto(tipoTributo);
                                        líneaTarifa.SubtotalReal += líneaPorTarifa.SubtotalBaseReal;
                                        líneaTarifa.Cantidad += líneaPorTarifa.Cantidad; // Para el cálculo del ImpuestoUnitario. No se usa en el porcentual.
                                    }
                                    líneasTarifas.Add(líneaTarifa);

                                }

                            }

                            taxTotal = obtenerTaxTotal(líneasTarifas, tipoTributo, out éxitoTributo, out mensajeTributo);
                            break;

                        default:
                            throw new Exception(CasoNoConsiderado(agrupaciónTaxTotales));
                    }
                    
                    if (!éxitoTributo) {
                        mensaje = mensajeTributo;
                        éxitoTributo = false;
                        return null;
                    }
                    if (taxTotal != null) taxTotales.Add(taxTotal);   

                }

                return taxTotales.ToArray();

            } // obtenerTaxTotales>

            #endregion Funciones Locales>


            #region Objetos Reusables
            // El código que se muestra en comentarios es el de su primera aparición.

            var companyIDFacturador = new CompanyIDType {
                Value = Validar(Empresa.Nit, "5..12"), // 1..1 FAJ21.
                schemeAgencyID = AgenciaID195, // 0..1 FAJ22.
                schemeAgencyName = NombreAgenciaDian, // 0..1 FAJ23.
                schemeID = Empresa.DígitoVerificaciónNit, // 1..1 FAJ24.
                schemeName = DocumentoIdentificación.Nit.AValor(), // 0..1 FAJ25.
            };

            var companyIDCliente = new CompanyIDType {
                Value = Validar(cliente.NitLegalEfectivo, "5..12"), // 1..1 FAK21.
                schemeAgencyID = AgenciaID195, // 1..1 FAK22.
                schemeAgencyName = NombreAgenciaDian, // 1..1 FAK23.
                schemeID = cliente.DígitoVerificaciónNit, // 0..1 FAK24.
                schemeName = cliente.CódigoDocumentoIdentificación // 1..1 FAK25.
            };

            var registrationNameCliente = new RegistrationNameType { Value = Validar(cliente.Nombre, "5..450", true) }; // 1..1 FAK43.

            var registrationNameFacturador = new RegistrationNameType { Value = Validar(Empresa.RazónSocial, "5..450", true) };

            #endregion Objetos Reusables>


            // Inicio
            dynamic document = Documento switch { // Se usa dynamic porque es manera más fácil de incorporar las 3 clases InvoiceType, CreditNoteType y DebitNoteType sin tener que modificar en exceso las clases en Dian.sln > Factura.cs y poder soportar de mejor manera posibles cambios futuros. Para el desarrollo cambiar el tipo de este objeto y en los otros 3 'switch's a al tipo que se esté implementando y ver en que puntos del código saca error. En esos puntos se debe asegurar que nunca entre la ejecución para el tipo de documento actual.        
                Venta _ => new InvoiceType { UBLExtensions = new UBLExtensionType[2] }, // 1..1 FAA01. 1..1 FAA02.
                NotaCréditoVenta _ => new CreditNoteType { UBLExtensions = new UBLExtensionType[2] }, // 1..1 CAA01. 1..1 CAA02.
                NotaDébitoVenta _ => new DebitNoteType { UBLExtensions = new UBLExtensionType[2] }, // 1..1 DAA01. 1..1 DAA02.
                _ => throw new Exception(CasoNoConsiderado(Documento.GetType().ToString()))
            };
            // Inicio>

            #region Extensión DIAN

            var ublExtension1 = new UBLExtensionType(); // 1 de 2..N FAB01.    
            var extensionContentDian = new ExtensionContentDianType(); // 1..1 FAB02.
            ublExtension1.ExtensionContent = extensionContentDian;
            var dianExtensions = new DianExtensionsType(); // 1..1 FAB03.

            if (venta != null) { // En los archivos XML de ejemplo de la DIAN este campo no está en la nota crédito ni en la nota débito. Tampoco está en la documentación los campos homólogos CAB04 o DAB04.

                dianExtensions.InvoiceControl = new InvoiceControl { // 1..1 FAB04.
                    InvoiceAuthorization = new NumericType1 { Value = Validar((decimal)Empresa.NúmeroAutorizaciónFacturación, "11..14", 0) }, // 1..1 FAB05. La documentación dice que debe ser de largo 14 pero el número obtenido en las pruebas desde la página de la DIAN fue de largo 11. Se validará de 11 a 14.
                    AuthorizationPeriod = new PeriodType { // 1..1 FAB06.
                        StartDate = new StartDateType { Value = (DateTime)Empresa.InicioAutorizaciónFacturación }, // 1..1 FAB07.
                        EndDate = new EndDateType { Value = (DateTime)Empresa.FinAutorizaciónFacturación } // 1..1 FAB08.
                    },
                    AuthorizedInvoices = new AuthrorizedInvoices { // 1..1 FAB09.
                        Prefix = Validar(Empresa.PrefijoFacturas, "0..4"), // 0..1 FAB10.
                        From = Validar((int)Empresa.PrimerNúmeroFacturaAutorizada, "1..9"), // 1..1 FAB11.
                        To = Validar((int)Empresa.ÚltimoNúmeroFacturaAutorizada, "1..9") // 1..1 FAB12.
                    }
                };

            }

            dianExtensions.InvoiceSource = new CountryType { // 1..1 FAB13.
                IdentificationCode = new IdentificationCodeType {
                    Value = Empresa.MunicipioFacturación.CódigoPaís, // 1..1 FAB14.
                    listAgencyID = AgenciaIdentificaciónPaísID, // 1..1 FAB15.
                    listAgencyName = AgenciaIdentificaciónPaís, // 1..1 FAB16.
                    listSchemeURI = UriEsquemaIdentificaciónPaís // 1..1 FAB17.
                }
            };

            dianExtensions.SoftwareProvider = new SoftwareProvider { // 1..1 FAB18.
                ProviderID = new coID2Type {
                    Value = Empresa.Nit, // 1..1 FAB19.
                    schemeAgencyID = AgenciaID195, // 1..1 FAB20.
                    schemeAgencyName = NombreAgenciaDian, // 1..1 FAB21.
                    schemeID = Empresa.DígitoVerificaciónNit, // 1..1 FAB22.
                    schemeName = DocumentoIdentificación.Nit.AValor(), // 1..1 FAB23.
                },
                SoftwareID = new IdentifierType1 {
                    Value = Empresa.IdentificadorAplicación, // 1..1 FAB24.
                    schemeAgencyID = AgenciaID195, // 1..1 FAB25.
                    schemeAgencyName = NombreAgenciaDian, // 1..1 FAB26.     
                }
            };

            dianExtensions.SoftwareSecurityCode = new IdentifierType1 {
                Value = Validar(obtenerCódigoSeguridad(), "96"), // 1..1 FAB27. En la documentación dice que el tamaño debe ser 48 pero la función devuelve un texto de 96 que coincide con los ejemplos en los XMLs, se deja en 96.
                schemeAgencyID = AgenciaID195, // 1..1 FAB28.
                schemeAgencyName = NombreAgenciaDian, // 1..1 FAB29.
            };

            dianExtensions.AuthorizationProvider = new AuthorizationProvider { // 1..1 FAB30.
                AuthorizationProviderID = new coID2Type {
                    Value = Validar(NitDian, "9"), // 1..1 FAB31.
                    schemeAgencyID = AgenciaID195, // 1..1 FAB32.
                    schemeAgencyName = NombreAgenciaDian, // 1..1 FAB33.
                    schemeID = DígitoVerificaciónNitDian, // 1..1 FAB34
                    schemeName = DocumentoIdentificación.Nit.AValor(), // 1..1 FAB35.
                }
            };

            dianExtensions.QRCode = ObtenerRutaQR(Cude); // 1..1 FAB36.

            extensionContentDian.DianExtensions = dianExtensions; // Al no encontrar una clase ExtensionContent en el XSD de la DIAN se genera manualmente. Pendiente verificar si funciona o si se debe hacer algo diferente.  
            document.UBLExtensions[0] = ublExtension1;

            #endregion Extensión DIAN>


            #region Extensión Firma

            var ublExtension2 = new UBLExtensionType(); // 2 de 2..N FAC01.
            var extensionContentFirma = new ExtensionContentFirmaType(); // 1..1 FAC02.
            ublExtension2.ExtensionContent = extensionContentFirma;
            document.UBLExtensions[1] = ublExtension2;

            #endregion Extensión Firma>


            #region Encabezado Factura

            document.UBLVersionID = new UBLVersionIDType { Value = Validar(VersiónUBL, "7..8") }; // 1..1 FAD01.
            document.CustomizationID = new CustomizationIDType { Value = Validar(ObtenerCódigoTipoOperación(venta, notaCrédito, notaDébito, ventaNota), "1..4") }; // 1..1 FAD02. 
            document.ProfileID = new ProfileIDType { Value = NombreYVersiónFacturaElectrónica }; // 1..1 FAD03. Según la documentación debería tener una longitud de 55 única pero el texto que dice en la documentación "Dian 2.1 Factura Electrónica de Venta" y el texto realmente exigido por el servicio web "Dian 2.1" contradicen esa restricción entonces no se validará su largo.
            document.ProfileExecutionID = new ProfileExecutionIDType { Value = Empresa.AmbienteFacturaciónElectrónica.AValor() }; // 1..1 FAD04.
            document.ID = new IDType { Value = Validar(Documento.Código, "1..20") }; // 1..1 FAD05.

            document.UUID = new UUIDType {
                Value = Validar(Cude, "96"), // 1..1 FAD06.
                schemeID = Empresa.AmbienteFacturaciónElectrónica.AValor(), // 1..1 FAD07.
                schemeName = venta != null ? Validar(AlgoritmoCufe, "11") : Validar(AlgoritmoCude, "11"), // 1..1 FAD08.
            };

            document.IssueDate = new IssueDateType { Value = fechaHora }; // 1..1 FAD09.
            document.IssueTime = new IssueTimeType { Value = fechaHora }; // 1..1 FAD10.
            if (venta != null) {
                if (venta.FechaVencimiento != null) document.DueDate = new DueDateType { Value = (DateTime)venta.FechaVencimiento }; // 0..1 FAD11.
                document.InvoiceTypeCode = new InvoiceTypeCodeType { Value = TextoTipo }; // 1..1 FAD12.
            }
            if (notaCrédito != null) document.CreditNoteTypeCode = new CreditNoteTypeCodeType { Value = TextoTipo }; // 1..1 CAD11. No hay equivalente en la documentación para la nota débito.

            document.Note = new NoteType[] { new NoteType { Value = Validar(Documento.Observación, "15..5000", true) } }; // 0..N FAD13.
            document.DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = Validar(moneda, "3") }; // 1..1 FAD15 (En la documentación se saltan la FAD14)..
            document.LineCountNumeric = new LineCountNumericType { Value = Validar(Documento.Líneas.Count, "1..6") }; // 1..1 FAD16.
            // document.InvoicePeriod = new PeriodType { StartDate = , StartTime = , EndDate = , EndTime = }; // 0..1 FAE01. Grupo de campos relativos al Periodo de Facturación: Intervalo de fechas la las que referencia la factura por ejemplo en servicios públicos. Para utilizar en los servicios públicos, contratos de arrendamiento, matriculas en educación, etc.

            if (notaCrédito != null || notaDébito != null) {

                var códigoDiscrepancia = "";
                if (notaCrédito != null) códigoDiscrepancia = notaCrédito.Razón.AValor();
                if (notaDébito != null) códigoDiscrepancia = notaDébito.Razón.AValor();

                document.DiscrepancyResponse = new ResponseType[] { // 1..N CBF01 o DBF01.
                    new ResponseType {
                        // ReferenceID = new ReferenceIDType { Value = "" }, // 0..1 CBF02 o DBF02. Aunque en la documentación dice que se usa para identificar la sección de la factura original a la que se le aplica la corrección en los XML de muestra se escribe aquí la factura. Ante esta inconsistencia se prefiere omitir ya que es opcional.
                        ResponseCode = new ResponseCodeType { Value = códigoDiscrepancia }, // 1..1 CBF03 o DBF03. Concepto de la correción realizada con la nota. Nota Crédito:Ver lista de valores posibles en 13.2.5. Nota Débito: Ver lista de valores posibles en 13.2.5.
                        Description = new DescriptionType[] { new DescriptionType { Value = Validar(Documento.Observación, "20..5000", true) } }
                    }
                };

            }

            if (venta?.OrdenCompra != null) {

                document.OrderReference = new OrderReferenceType { // 0..1  FAF01.
                    ID = new IDType { Value = venta.OrdenCompra.Número }, // 1..1 FAF02.      
                };
                if (venta.OrdenCompra.FechaHoraCreación != null)
                    document.OrderReference.IssueDate = new IssueDateType { Value = (DateTime)venta.OrdenCompra.FechaHoraCreación }; // 0..1 FAF03.

            }

            if (ventaNota != null && ventaNota.Cude != null) { // Si la ventaNota es nula o no se tiene el CUDE el CustomizationID será 22 o 32 y por lo tanto la DIAN no obliga a tener este elemento. Se permite que no se tenga en cuenta una ventaNota que no tenga CUDE porque se puede dar el caso de integración con programas terceros que no lleven el registro del CUFE de la factura electrónica en su base de datos, pero si se quiere permitir que se pase un número de factura para escribirlo en la nota crédito. Estas notas créditos quedarían enlazadas a la factura únicamente en la representación gráfica y no en el XML.

                document.BillingReference = new BillingReferenceType[] {
                    new BillingReferenceType {
                        InvoiceDocumentReference = new DocumentReferenceType {
                            ID = new IDType { Value = Validar(ventaNota.Código, "1..20") }, // 1..1 CBG03 o DBG03. En la documentación la restricción por tamaño es 10. Pero no tiene sentido esta restricción porque siempre podrían ser más cortas y más largas porque incluye el prefijo (según la misma documentación y el XML de ejemplo). Se usa entonces 1..20 que es la misma restricción para el ID principal en FAD05.
                            UUID = new UUIDType {
                                Value = Validar(ventaNota.Cude, "96"), // 1..1 CBG04 o DBG04.
                                schemeName = Validar(AlgoritmoCufe, "11") // 1..1 CBG05 o DBG05.
                            },
                            IssueDate = new IssueDateType { Value = ventaNota.FechaHora } // 1..1 CBG06 o DBG06.
                        }
                    }
                };

            }

            #region Código de facturaOriginadaPorNotaCrédito y facturaOriginadaPorNotaDébito no usado.

            //var índiceBillingReference = 0;
            //if (facturaOriginadaPorNotaCrédito && facturaOriginadaPorNotaDébito) {
            //    document.BillingReference = new BillingReferenceType[2];
            //} else if (facturaOriginadaPorNotaCrédito || facturaOriginadaPorNotaDébito) {
            //    document.BillingReference = new BillingReferenceType[1];
            //}

            //if (facturaOriginadaPorNotaCrédito) {

            //    document.BillingReference[índiceBillingReference] = new BillingReferenceType { // 0..N FHI01.
            //        CreditNoteDocumentReference = new DocumentReferenceType { // 1..1 FHI02.
            //            ID = new IDType { Value = Validar(númeroNotaCréditoOriginadora, "1..10") }, // 1..1 FHI03. En la documentación la restricción por tamaño es 10. Pero no tiene mucho sentido esta restricción porque siempre podrían ser más cortas entonces se usa 1..10.
            //            UUID = new UUIDType {
            //                Value = Validar(cudeNotaCréditoOriginadora, "96"), // 1..1 FHI04.
            //                schemeName = Validar(AlgoritmoCude, "11"), // 1..1 FHI05.
            //            },
            //            IssueDate = new IssueDateType { Value = fechaNotaCréditoOriginadora }, // 0..1 FHI06.
            //        },
            //    };
            //    índiceBillingReference++;

            //}

            //if (facturaOriginadaPorNotaDébito) {

            //    document.BillingReference[índiceBillingReference] = new BillingReferenceType { // 0..N FBI01.
            //        DebitNoteDocumentReference = new DocumentReferenceType { // 1..1 FBI02.
            //            ID = new IDType { Value = Validar(númeroNotaDébitoOriginadora, "1..10") }, // 1..1 FBI03. En la documentación la restricción por tamaño es 10. Pero no tiene mucho sentido esta restricción porque siempre podrían ser más cortas entonces se usa 1..10.
            //            UUID = new UUIDType {
            //                Value = Validar(cudeNotaDébitoOriginadora, "96"), // 1..1 FBI04.
            //                schemeName = Validar(AlgoritmoCude, "11"), // 1..1 FBI05.
            //            },
            //            IssueDate = new IssueDateType { Value = fechaNotaDébitoOriginadora }, // 0..1 FBI06.
            //        },
            //    };

            //}
            #endregion

            // document.DespatchDocumentReference = new DocumentReferenceType[] { new DocumentReferenceType { ID = new IDType { Value = }, IssueDate = new IssueDateType { Value = } } }; // 0..N FAG01. ID: 1..1 FAG02 T20, IssueDate: 0..1 FAG03 T10. Grupo de campos para información que describen uno o más documentos de despacho para esta factura. Referencias no tributarias pero si de interés mercantil. Se utiliza cuando se requiera referenciar uno o más documentos de despacho asociado a la factura realizada. En términos generales no es muy útil porque la factura se suele hacer antes de conocer el documento del despacho. En  el caso de estar haciendo una factura desde una remisión si se puede tener esta información porque esta se almacena en el Remisión.DetalleEntrega, pero el tamaño disponible de 20 para esta información es muy limitado e impediría agregar un dato tan simple como Servientrega: 9876543210, entonces se prefiere no usar.
            // document.ReceiptDocumentReference = new DocumentReferenceType[] { new DocumentReferenceType { ID = new IDType { Value = }, IssueDate = new IssueDateType { Value = } } }; // 0..N FAH01. ID: 1..1 FAH02 T20, IssueDate: 0..1 FAH03 T10. La documentación de la DIAN parece estar incorrecta en estos elementos. Se refieren a recibido. Pero al ser esto algo que se hace en la recepción del producto normalmente no se dispone de estos valores al realizar la factura.
            if (Tipo == TipoDocumentoElectrónico.FacturaContingenciaFacturador) { // Si es una repetición de una factura de contingencia de facturador.

                if (CódigoFacturaContingencia == null || FechaFacturaContingencia == null)
                    return Falso(out mensaje, "Si la factura es una repetición de una factura de contingencia de facturador se debe indicar su " + 
                        "número y fecha.");

                document.AdditionalDocumentReference = new DocumentReferenceType[] { // 0..N FAI01. Grupo de campos para información que describen un documento referenciado por la factura. Obligatorio para factura tipo 03 (Contingencia).
                    new DocumentReferenceType {
                        ID = new IDType { Value = Validar(CódigoFacturaContingencia, "1..20") }, // 1..1 FAI02. La restricción de tamaño en la documentación de la DIAN es 20, pero no tiene mucho sentido esta restricción porque siempre podrían ser más cortas entonces se usa 1..20.
                        IssueDate = new IssueDateType { Value = (DateTime)FechaFacturaContingencia }, // 0..1 FAO05. (La documentación de la DIAN se salta el 3 y 4).
                        DocumentTypeCode = new DocumentTypeCodeType { Value = CódigoFacturaContingencia }, // 1..1 FAI06. El tamaño en la documentación de la DIAN claramente está malo en 10 porque estos códigos son casi todos de 3 letras. No se validará.
                    }
                };

            }

            if (venta?.Remisiones?.Any() == true) { // Si es una factura cargada desde una o varias remisiones.

                var cantidadRemisiones = venta.Remisiones.Count;
                document.AdditionalDocumentReference = new DocumentReferenceType[cantidadRemisiones]; // 0..N FAI01. Grupo de campos para información que describen un documento referenciado por la factura.
                for (int i = 0; i < cantidadRemisiones; i++) {

                    document.AdditionalDocumentReference[i] = new DocumentReferenceType {
                        ID = new IDType { Value = Validar(venta.Remisiones[i].ID.ATexto(), "1..20") }, // 1..1 FAI02.
                        IssueDate = new IssueDateType { Value = venta.Remisiones[i].FechaHoraCreación }, // 0..1 FAO05.
                        DocumentTypeCode = new DocumentTypeCodeType { Value = CódigoRemisión }, // 1..1 FAI06.
                    };

                }

            }

            #endregion Encabezado Factura>


            #region Datos Facturador

            var accountingSupplierParty = new SupplierPartyType { // 1..1 FAJ01.
                AdditionalAccountID = new AdditionalAccountIDType[1] { new AdditionalAccountIDType { Value = Empresa.TipoEntidad.AValor() } } // 1..1 FAJ02.
            };
            var supplierParty = new PartyType(); // 1..1 FAJ03.
            // supplierParty.IndustryClassificationCode = new IndustryClassificationCodeType { Value = } // 0..1 FAJ04. Corresponde al código de actividad económica CIIU. Identifica el código de actividad económica del emisor. Debe informar el código según lista CIIU.Para informar varios códigos, se separan por ejemplo 7020; 5140. Al ser opcional se omite para evitar pedir muchos datos al usuario.
            if (!string.IsNullOrEmpty(Empresa.NombreComercial)) {
                supplierParty.PartyName = new PartyNameType[1] { 
                    new PartyNameType { Name = new NameType1 { Value = Validar(Empresa.NombreComercial, "5..450", true) } } // 0..1 FAJ05. Name: 1..1 FAJ06.
                };
            }

            supplierParty.PhysicalLocation = new LocationType1 { 
                Address = obtenerAddress(new DirecciónCompleta(Empresa.MunicipioUbicación, Empresa.DirecciónUbicaciónEfectiva)) // 0..1 FAJ07. Es opcional pero los campos más sujetos a errores que son los de nombre solo solo generan notificación en caso de no coincidir con los datos de la DIAN entonces se prefiere informar todo.
            }; 

            supplierParty.PartyTaxScheme = new PartyTaxSchemeType[1] { // 1..1 FAJ19.
                new PartyTaxSchemeType {
                    RegistrationName = registrationNameFacturador, // 1..1 FAJ20.
                    CompanyID = companyIDFacturador,
                    TaxLevelCode = new TaxLevelCodeType { 
                        Value = Validar(ObtenerResponsabilidadFiscal(Empresa.TipoContribuyente), "1..30"), // 1..1 FAJ26. El tamaño la documentación dice 30 único pero ese valor no tiene sentido. Se usará 1..30.
                        listName = ObtenerResponsabilidadIVA(Empresa.TipoContribuyente), // Supuestamente FAJ27 es un elemento opcional, pero el servidor de la DIAN si lo exige. Se encontró que se debe usar 48 o 49 dependiendo de si es o no responsable de IVA aquí http://facturasyrespuestas.com/2225/inquietud-campo-taxlevelcode. Estos valores se pueden ver en el numeral 16.1.6. Modificación del anexo técnico (06-09-2019) de la documentación.
                    }, 
                    RegistrationAddress = obtenerAddress(new DirecciónCompleta(Empresa.MunicipioFacturación, Empresa.DirecciónFacturación)), // 0..1 FAJ28. Aquí se incluyen automáticamente los elementos FAJ29, FAJ30, FAJ74, FAJ31, FAJ32, FAJ33, FAJ34, FAJ35, FAJ36, FAJ37 y FAJ38.
                    TaxScheme = new TaxSchemeType { // 1..1 FAJ39.
                        ID = new IDType { Value = Validar(ObtenerCódigoTipoImpuesto(tipoImpuesto, forzar01: true), "2..10") }, // 1..1 FAJ40. Aunque la documentación dice tamaño 3..10 esto va en contradicción con el código "01" o "04" se usa 2..10. La documentación dice usar los valores de la tabla 13.2.6.2 pero a la vez restringe los valores a 01 o 04 sin especificar que se debe hacer en los casos de IVA y INC, por esto se usa el parámetro forzar01 = true en el que si es IVA e INC se devuelve el valor de IVA = 01.
                        Name = new NameType1 { Value = Validar(ObtenerNombreImpuesto(tipoImpuesto, forzarIVA: true), "3..30") } // 1..1 FAJ41. Aunque la documentación dice tamaño 10..30 esto va en contradicción con el tamaño de "IVA", se usa 3..30.
                    },
                }
            };

            supplierParty.PartyLegalEntity = new PartyLegalEntityType[1] { // 1..1 FAJ42.
                new PartyLegalEntityType {
                    RegistrationName = registrationNameFacturador, // 1..1 FAJ43.
                    CompanyID = companyIDFacturador, // Se reusa el objeto creado anteriormente. Aquí se incluyen automáticamente los elementos FAJ44, FAJ45, FAJ46, FAJ47 y FAJ48.   
                    CorporateRegistrationScheme = new CorporateRegistrationSchemeType { // 0..1 FAJ49. Grupo de información de registro del emisor. 
                        ID = new IDType { 
                            Value = Validar((venta != null ? Empresa.PrefijoFacturas : 
                                (notaCrédito != null ? PrefijoNotasCréditoPredeterminado : 
                                    (notaDébito != null ? PrefijoNotasDébitoPredeterminado : ""))), "1..6") 
                        }, // ID: 0..1 FAJ50, prefijo de la facturación usada para el punto de venta. Aunque es opcional agrega para evitar que reporte notificación. Dice que el tamaño debe ser 6 pero claramente es incorrecto porque los prefijos pueden ser de menor tamaño, se valida con 1..6.
                        // Name = new NameType1 { Value = } // Name: 0..1 FAJ51 T9 Número de matrícula mercantil. Al ser opcional y al no haber claridad a que se refiere se omite.
                    }    
                    // ShareholderParty = new ShareholderPartyType { PartecipationPercent = new PartecipationPercentType {} , Party = new PartyType {} } // 0..1 FAJ52. Elementos para consorcios y uniones temporales. No implementado porque es opcional y porque no es el público objetivo de SimpleOps. Incluye los códigos FAJ53, FAJ54, FAJ55, FAJ53, FAJ57, FAJ58, FAJ59, FAJ60, FAJ61, FAJ62, FAJ63, FAJ64, FAJ65 y FAJ66.
                },
            };

            supplierParty.Contact = new ContactType { // 0..1 FAJ67. Aunque su uso no es obligatorio se considera apropiado informarlo porque es información que se dispone. Los clientes pueden hacer uso de esta información en la factura electrónica.
                Name = new NameType1 { Value = Empresa.NombreContactoFacturación }, // 0..1 FAJ68.
                Telephone = new TelephoneType { Value = Empresa.TeléfonoContactoFacturación }, // 0..1 FAJ69.
                // Telefax = new TelefaxType { Value = }, // 0..1 FAJ70. El fax ya es obsoleto, no se requiere informar.
                ElectronicMail = new ElectronicMailType { Value = Empresa.EmailContactoFacturación }, // 0..1 FAJ71.
                // Note = new NoteType { Value = "" }, // 0..1 FAJ72. No es necesario saturar la interfaz requiriendo notas sobre el contacto.
            };

            accountingSupplierParty.Party = supplierParty;
            document.AccountingSupplierParty = accountingSupplierParty;

            #endregion Datos Facturador>


            #region Datos Cliente

            var accountingCustomerParty = new CustomerPartyType { // 1..1 FAK01.
                AdditionalAccountID = new AdditionalAccountIDType[1] { new AdditionalAccountIDType { Value = cliente.TipoEntidad.AValor() } } // 1..1 FAK02.
            };

            var customerParty = new PartyType { // 1..1 FAK03.
                PartyIdentification = new PartyIdentificationType[1] {
                new PartyIdentificationType {  // 1..1 FAK03-1.
                    ID = new IDType {
                            Value =  Validar(cliente.NitLegalEfectivo, "5..12"), // 1..1 FAK03-2.
                            schemeName = cliente.CódigoDocumentoIdentificación, // 1..1 FAK03-3.
                            schemeID = cliente.DígitoVerificaciónNit, // 0..1 FAK03-4.
                        }
                    }
                }
            };

            if (!string.IsNullOrEmpty(cliente.NombreComercial)) // 0..1 FAK05.
                customerParty.PartyName = new PartyNameType[1] { 
                    new PartyNameType { Name = new NameType1 { Value = Validar(cliente.NombreComercial, "5..450", true) } } // 1..1 FAK06.
                }; 

            // customerParty.PhysicalLocation = new LocationType1 { Address =  }; // 0..1 FAK07. Este elemento incluye FAK08, FAK09, FAK10, FAK57, FAK11, FAK12, FAK13, FAK14, FAK15, FAK16, FAK18. La mayoría de las empresas tienen la misma ubicación física que su dirección de facturación y en el caso que no sea así no es fácil ni útil para una empresa obtener esta información de sus clientes. Cuando hay diferentes direcciones de entrega para una empresa SimpleOps lo maneja con las Sedes pero no provee la funcionalidad de marcar una de estas sedes como la principal porque esto no aporta valor adicional. No se incluye en la base de datos ni se reportará a la DIAN.

            customerParty.PartyTaxScheme = new PartyTaxSchemeType[1] {
                new PartyTaxSchemeType { // 0..1 FAK19.
                    RegistrationName = registrationNameCliente, // 1..1 FAK20. 
                    CompanyID = companyIDCliente, // 1..1 FAK21. Incluye FAK22, FAK23, FAK24 y FAK25.
                    // TaxLevelCode = // 0..1 FAK26. Incluye FAK27. No siempre se dispone de la información correcta de que tipo de contribuyente es un cliente entonces se omite.
                }
            };

            customerParty.PartyTaxScheme[0].RegistrationAddress = obtenerAddress(cliente.DirecciónCompleta); // 0..1 FAK28. Incluye FAK29, FAK30, FAK58, FAK31, FAK32, FAK33, FAK34, FAK35, FAK36, FAK37 y FAK38.
            customerParty.PartyTaxScheme[0].TaxScheme = new TaxSchemeType { // 1..1 FAK39.
                ID = new IDType { Value = Validar(códigoImpuestoPredominante, "2..10") }, // Antes era ObtenerCódigoTipoImpuesto(tipoImpuesto). 1..1 FAK40. Dice que para el consumidor final debe informar "ZZ" pero quedan dudas al respecto porque no tiene mucho que ver con los impuestos de la factura, se dejará normal.
                Name = new NameType1 { Value = Validar(nombreImpuestoPredominante, "3..30") } // Antes eratipoImpuesto.ATexto(). 1..1 FAK41.
            }; // 1..1 FAK39. Incluye FAK40 y FAK41.

            customerParty.PartyLegalEntity = new PartyLegalEntityType[1] { // 1..1 FAK42.
                new PartyLegalEntityType {
                    RegistrationName = registrationNameCliente, // 1..1 FAK43.
                    CompanyID = companyIDCliente, // 1..1 FAK44. Incluye FAK45, FAK46, FAK47 y FAK48.
                    // CorporateRegistrationScheme =  // 0..1 FAK49. Incluye FAK50. Es opcional y no hay claridad a que se refiere entonces se omite.
                }
            };

            if (customerParty.PartyLegalEntity[0].RegistrationName.Value != cliente.Nombre)
                MostrarError(customerParty.PartyLegalEntity[0].RegistrationName.Value);

            if (cliente.ContactoFacturas != null) {

                customerParty.Contact = new ContactType { // 0..1 FAK51.
                    Name = new NameType1 { Value = cliente.ContactoFacturas.Nombre }, // 0..1 FAK52.
                    Telephone = new TelephoneType { Value = cliente.ContactoFacturas.Teléfono }, // 0..1 FAK53.
                    // Telefax = new TelefaxType { Value = }, // 0..1 FAK54. Tecnología obsoleta, no es necesario.
                    ElectronicMail = new ElectronicMailType { Value = cliente.ContactoFacturas.Email }, // 0..1 FAK55.
                    // Note = new NoteType { Value = }, // 0..1 FAK56. No es necesario saturar la interfaz pidiendo este dato.
                };

            }

            if (cliente.TipoEntidad != TipoEntidad.Empresa) {

                customerParty.Person = new PersonType[1] { // 0..1 FAK56.
                    new PersonType {
                        // ID = new IDType { Value = } // 0..1 FAK56-1.
                        FirstName = new FirstNameType { Value = NombreUsuarioFinal }, // 1..1 FAK56-2.
                        FamilyName = new FamilyNameType { Value = ApellidoUsuarioFinal }, // 0..1 FAK56-3.
                        MiddleName = new MiddleNameType { Value = cliente.Nombre }, // 0..1 FAK56-4. Aunque dice Middle Name en la documentación dice poner Nombre del adquiriente entonces se pondrá el nombre completo con apellidos.
                        ResidenceAddress = obtenerAddress(cliente.DirecciónCompleta), // 0..1 FAK56-5, Incluye todos los otros FAK56 sin código.
                    }
                };

            }

            accountingCustomerParty.Party = customerParty;
            document.AccountingCustomerParty = accountingCustomerParty;
            // document.TaxRepresentativeParty = new PartyType { }; // 0..1 FAL01. Incluye FAL02, FAL03, FAL04, FA05, FAL07 y FAL06. No hay claridad a que se refieren con 'Persona autorizada para descargar documentos'. Al ser opcional se omite.

            #endregion Datos Cliente>


            #region Datos Entrega
            // Aunque son opcionales para la DIAN si se disponen se añaden porque podrían ser usados por el cliente.

            if (venta != null) {

                var deliveries = new DeliveryType[1];
                var delivery = new DeliveryType { // 0..1 FAM01.
                    DeliveryAddress = obtenerAddress(venta.DirecciónCompletaEntrega) // 0..1 FAM01. Incluye FAM02, FAM03, FAM04, FAM05, FAM06, FAM68, FAM07, FAM09, FAM10, FAM11, FAM12, FAM13 y FAM14.
                }; 
                delivery.ActualDeliveryDate = new ActualDeliveryDateType { Value = venta.FechaHora.AddDays(3) }; // 1..1 FAM02. Aunque normalmente no se dispone de esta información porque la factura se suele generar antes de realizar el envío la DIAN puso este campo obligatorio en la versión del 2020. Se pone entonces un aproximado sumando 3 días después de la facturación sumando además los 3 días hábiles que la DIAN da para aceptación tácita da entre 6 a 8 días calendario para la aceptación tácita después de la fecha de facturación. De todas maneras esto no debería importar mucho porque se espera que lo más normal sean las aceptaciones tácitas y las diferencias que resulten se resuelvan a nivel comercial con notas crédito.
                // delivery.ActualDeliveryTime = ; // 0..1 FAM03. Igual que el anterior.
                // delivery.DeliveryParty = ; // 0..1 FAM15. Incluye FAM16, FAM17, FAM18, FAM19, FAM20, FAM21, FAM69, FAM22, FAM23, FAM24, FAM25, FAM26, FAM27, FAM28, FAM29, FAM30, FAM31, FAM32, FAM33, FAM34, FAM35, FAM36, FAM37, FAM38, FAM39, FAM40, FAM41, FAM70, FAM42, FAM43, FAM44, FAM45, FAM46, FAM47, FAM48, FAM49, FAM50, FAM51, FAM52, FAM53, FAM54, FAM55, FAM56, FAM57, FAM58, FAM59, FAM60, FAM61, FAM62, FAM63, FAM64, FAM65, FAM66 y FAM67. Normalmente en el momento de la facturación no se dispone de la empresa transportadora que se va a usar para el envío. Al ser opcional se omite.
                deliveries[0] = delivery;
                document.Delivery = deliveries;

                document.DeliveryTerms = new DeliveryTermsType {
                    // ID = new IDType { Value = "1" }, // 0..1 FBC02 No ha claridad a que se refiere la documentación con 'número de línea'.
                    SpecialTerms = new SpecialTermsType[1] { new SpecialTermsType { Value = venta.PagoTransporte.ATexto() } }, // 0..1 FBC03.
                    // LossRiskResponsibilityCode = , // 0..1 FBC04 Términos incoterm, no es necesario para el público objetivo de SimpleOps.
                    // LossRisk = , // 0..1 FBC05. Opcional personalizado, se omite.
                };

            }

            #endregion Datos Entrega>


            #region Datos Pago

            var paymentsMeans = new PaymentMeansType[1]; // 1..N FAN01.
            paymentsMeans[0] = new PaymentMeansType { 
                ID = new IDType { Value = (venta != null || notaDébito != null) ? cliente.FormaPago.AValor() : FormaPago.Contado.AValor() }, // 1..1 FAN02. En el caso de notas crédito no tiene mucho sentido la forma de pago a crédito porque son un descuento, la forma de pago será la de la factura asociada o si no hay comercialmente se acepta descontar el valor en el próximo pago.
                PaymentMeansCode = new PaymentMeansCodeType { Value = CódigoMedioPagoPorDefinir }, // 1..1 FAN03. En la documentación dice que el tamaño debe ser 1..2 pero CódigoMedioPagoPorDefinir es ZZZ, entonces no se realiza ninguna validación.
                // PaymentID = new PaymentIDType { Value = } // 0..N no hay claridad a que se refiere y es opcional. El FAN06 PaymentTerms ni está documentado.
            };
            if (venta != null && venta.FechaVencimiento != null) 
                paymentsMeans[0].PaymentDueDate = new PaymentDueDateType { Value = (DateTime)venta.FechaVencimiento }; // 0..1 FAN04.
            if (notaDébito != null && notaDébito.FechaVencimiento != null)
                paymentsMeans[0].PaymentDueDate = new PaymentDueDateType { Value = (DateTime)notaDébito.FechaVencimiento }; // 0..1 DAN04.

            document.PaymentMeans = paymentsMeans;

            if (venta?.InformePago != null) {

                var prepaidPayments = new PaymentType[1]; // 0..N FBD01.
                prepaidPayments[0] = new PaymentType {
                    ID = new IDType { Value = Validar(venta.InformePago.Lugar, "1..150", true) }, // 1..1 FBD02.
                    PaidAmount = new PaidAmountType { Value = Validar(Anticipo, "4..15 p (2..6)", 2), currencyID = moneda }, // 1..1 FBD03. Incluye FBD04.
                    ReceivedDate = new ReceivedDateType { Value = venta.InformePago.FechaHoraPago }, // 1..1 FBD05.
                    // PaidDate = , // 0..1 FBD06 No se registra en la base de datos la fecha de realización del pago, se registra es la fecha de recepción. Lo importante para efectos de la empresa es cuando se recibió. Es opcional, entonces no se registra.
                    // PaidTime = , // 0..1 FBD07. Igual que el anterior.
                    // InstructionID = , // 0..1 FBD08. Instrucciones relativas al pago T15..5000. Las observaciones del pago se consideran información de uso privado de la empresa y no se comparten con los clientes en la factura electrónica.
                };
                document.PrepaidPayment = prepaidPayments; // Aunque esta también puede aplicar para nota débito, no se soporta porque las notas débito tendrán un soporte muy mínimo que no incluye información de Informe de Pago.

            }

            var descuentoCondicionado = 0M;
            if (venta != null && venta.PorcentajeDescuentoCondicionado != null) {

                descuentoCondicionado = venta.DescuentoCondicionado;
                document.AllowanceCharge = new AllowanceChargeType[1] { new AllowanceChargeType { // 0..N FAQ01. Según la documentación son descuentos o cargos a nivel de factura que no afectan las bases gravables es decir están hablando de descuentos condicionados o financieros (Leer más en https://www.globalcontable.com/tratamiento-de-los-descuentos-en-la-facturacion-electronica-oficio-dian-20067-de-2019/). Además en la tabla 13.3.7 lo vuelven a aclarar cuando dicen que los 00. Descuento no condicionado es para descuentos a nivel de línea y 01. Descuento condicionado, son los descuentos a pie de factura. Aunque el término a pie de factura se podría interpretar como descuento comercial, la aclaración sobre que los no condicionados se usan a nivel de línea compensa esa poca claridad y refuerza la indicación de la tabla de documentación para que el elemento FAQ01 se debe use solo para descuentos condicionados.
                    ID = new IDType { Value = "1" }, // 1..1 FAQ02. Solo se agrega un descuento condicionado.
                    ChargeIndicator = new ChargeIndicatorType { Value = false }, // 1..1 FAQ03. Solo se soportarán descuentos, no cargos.
                    AllowanceChargeReasonCode = new AllowanceChargeReasonCodeType { Value = TipoDescuento.Condicionado.AValor(largoForzado: 2) }, // 0..1 FAQ04.
                    AllowanceChargeReason = new AllowanceChargeReasonType[]
                        { new AllowanceChargeReasonType { Value = Validar(TipoDescuento.Condicionado.ATexto(), "10..5000", true) } }, // 1..1 FAQ05.
                    MultiplierFactorNumeric = new MultiplierFactorNumericType { 
                        Value = Validar((decimal)venta.PorcentajeDescuentoCondicionado * 100, "1..6 p (0..2)", 2) // 1..1 FAQ06. 
                    }, 
                    Amount = new AmountType2 { Value = Validar(descuentoCondicionado, "4..15 p (2..6)", 2), currencyID = moneda }, // 1..1 FAQ07. currencyID: 1..1 FAQ08.
                    BaseAmount = new BaseAmountType { Value = Validar(venta.SubtotalBase, "4..15 p (2..6)", 2), currencyID = moneda } // 1..1 FAQ09. currencyID: 1..1 FAQ10. Valor base para calcular el descuento o el cargo.
                }};

            }

            // document.PaymentExchangeRate = new ExchangeRateType { }; // 0..1 FAR01. Incluye FAR02, FAR03, FAR04, FAR05, FAR06, FAR07, FGB01, FGB02, FGB03, FGB04, FGB05, FGB06 y FGB07. SimpleOps no implementa la posibilidad de múltiples monedas y manejo de tasas de cambio por lo tanto este elemento siempre se omite.

            #endregion Datos Pago>


            #region Datos Impuestos 

            document.TaxTotal = obtenerTaxTotales(Documento.Líneas, out bool éxitoTaxTotales, out string? mensajeTaxTotales, 
                AgrupaciónTaxTotales.Tarifa);
            if (!éxitoTaxTotales) return Falso(out mensaje, mensajeTaxTotales);
            // document.WithholdingTaxTotal = new TaxTotalType[1] { obtenerTaxTotal(, TipoTributo.RetenciónRenta) }; // 0..N FAT01. Incluye FAT02, FAT03, FAT04, FAT05, FAT06, FAT07, FAT08, FAT09, FAT10, FAT11, FAT12 y FAT13. Grupo de campos para información relacionada con los tributos retenidos. Se usa para los casos que la empresa sea autorretenedor según la documentación de la DIAN. Sin embargo no se agregará por 4 razones: 1. El público objetivo de SimpleOps es improbable que sea autorretenedor. 2. Aún si alguno lo fuera este elemento no es de utilidad para el cliente pues los valores autorretenidos son importantes solo para la empresa y su declaración de impuestos, el cliente lo único que debe saber es que no debe aplicar retenciones. 3 el elemento es opcional, entonces para la DIAN tampoco es importante. 4. Es extraño lo de la autorretención, se supone que cuando un facturador es autorretenedor esto implica que el cliente no le debe aplicar ninguna retención al pagarle la factura, entonces si para los autorretenedores las retenciones son cero, ¿Qué se supone que se informaría aquí?
           
            #endregion Datos Impuestos>


            #region Datos Totales

            var legalMonetaryTotal = new MonetaryTotalType { // 1..1 FAU01.
                LineExtensionAmount = new LineExtensionAmountType { Value = Validar(Documento.SubtotalBase, "4..15 p (2..6)", 2), currencyID = moneda }, // 1..1 FAU02. currencyID: 1..1 FAU03. Total valor bruto antes de tributos. Es la suma de los valores subtotales de las líneas de la factura.
                TaxExclusiveAmount = new TaxExclusiveAmountType { 
                    Value = Validar(Documento.ImpuestoConsumo > 0 ? Documento.SubtotalBaseReal : Documento.SubtotalBaseIVADian, "4..15 p (2..6)", 2), // 1..1 FAU05. Total valor base imponible. Es la suma de la base para los impuestos. La DIAN tiene un comportamiento irregular con este valor, cuando la factura tiene al menos un impuesto al consumo aquí requiere el subtotal real normal, pero cuando no hay ningún impuesto al consumo requiere el subtotal base del IVA. Si vuelve a suceder algún error con este campo se puede deber a correcciones en este comportamiento realizadas por la DIAN. Al realizar cualquier cambio se deben realizar todas las pruebas de las Pruebas.Facturación() para asegurar que quede bien.
                    currencyID = moneda // 1..1 FAU04. currencyID
                }, 
                TaxInclusiveAmount = new TaxInclusiveAmountType { 
                    Value = Validar(Documento.SubtotalBase + Documento.IVA + Documento.ImpuestoConsumo, "4..15 p (2..6)", 2), currencyID = moneda // 1..1 FAU06. Moneda: 1..1 FAU07. Total de valor bruto más tributos.
                },
                AllowanceTotalAmount = new AllowanceTotalAmountType { 
                    Value = Validar(Documento.DescuentoCondicionado, "4..15 p (2..6)", 2), currencyID = moneda // 0..1 FAU08. currencyID 1..1 FAU09. Suma de todos los descuentos aplicados a nivel de la factura (son los de AllowanceCharge, es decir los condicionados).
                }, 
                // ChargeTotalAmount = new ChargeTotalAmountType { Value = 0, currencyID = Opciones.Moneda }; // 0..1 FAU10. currencyID 1..1 FAU11. Suma de todos los cargos aplicados a nivel de la factura. No se soportan cargos. La manera más fácil e intuitiva de generarlos facturando un servicio dentro de la factura.
                PrepaidAmount = new PrepaidAmountType { Value = Validar(Anticipo, "4..15 p (2..6)", 2), currencyID = moneda }, // 0..1 FAU12. currencyID 1..1 FAU13. Suma de todos los pagos anticipados.
                PayableAmount = new PayableAmountType { 
                    Value = Validar(Documento.SubtotalFinalConImpuestos - Anticipo, "0..15 p (2..6)", 2), currencyID = moneda// 1..1 FAU14. El valor a pagar es igual a la suma del valor bruto + tributos - valor del descuento total + valor del cargo total - valor del anticipo total. Moneda: 1..1 FAU15. 
                } 
            };

            if (venta != null || notaCrédito != null) document.LegalMonetaryTotal = legalMonetaryTotal; // 1..1 FAU01 y CAU01.
            if (notaDébito != null) document.RequestedMonetaryTotal = legalMonetaryTotal; // 1..1 DAU01.
            
            #endregion Datos Totales>


            #region Líneas

            dynamic lines = Documento switch { // Se usa dynamic porque es manera más fácil de incorporar las 3 clases InvoiceLineType, CreditNoteLineType y DebitNoteLineType sin tener que modificar en exceso las clases en Dian.sln > Factura.cs y poder soportar de mejor manera posibles cambios futuros. Para el desarrollo cambiar el tipo de este objeto y en los otros 3 'switch's a al tipo que se esté implementando y ver en que puntos del código saca error. En esos puntos se debe asegurar que nunca entre la ejecución para el tipo de documento actual.        
                Venta _ => new InvoiceLineType[Documento.Líneas.Count],
                NotaCréditoVenta _ => new CreditNoteLineType[Documento.Líneas.Count],
                NotaDébitoVenta _ => new DebitNoteLineType[Documento.Líneas.Count],
                _ => throw new Exception(CasoNoConsiderado(Documento.GetType().ToString()))
            };
            var líneaID = 1;

            foreach (var línea in Documento.Líneas) { // 1..N FAV01, CAV01 o DAV01.

                dynamic line = Documento switch { // Se usa dynamic porque es manera más fácil de incorporar las 3 clases InvoiceLineType, CreditNoteLineType y DebitNoteLineType sin tener que modificar en exceso las clases en Dian.sln > Factura.cs y poder soportar de mejor manera posibles cambios futuros. Para el desarrollo cambiar el tipo de este objeto y en los otros 3 'switch's a al tipo que se esté implementando y ver en que puntos del código saca error. En esos puntos se debe asegurar que nunca entre la ejecución para el tipo de documento actual.        
                    Venta _ => new InvoiceLineType(),
                    NotaCréditoVenta _ => new CreditNoteLineType(),
                    NotaDébitoVenta _ => new DebitNoteLineType(),
                    _ => throw new Exception(CasoNoConsiderado(Documento.GetType().ToString()))
                };
                var producto = línea.Producto!; // Un documento electrónico siempre viene de una previa ejecución de CalcularTodo() y si esta fue exitosa se puede asegurar que ningún producto de las líneas es nulo.

                line.ID = new IDType { Value = Validar(líneaID.ATexto(), "1..4") }; // 1..1 FAV02.
                // line.Note = new NoteType[] { new NoteType { Value = , "20..5000", true) } }; // 0..N FAV03. Una nota cualquiera sobre la línea. No se agrega para evitar saturar la interfaz pidiendo esta información.
                
                if (venta != null) {
                    line.InvoicedQuantity = new InvoicedQuantityType {
                        Value = línea.Cantidad, // 1..1 FAV04.
                        unitCode = Validar(ObtenerCódigoUnidad(producto.Unidad), "2..5"), // 1..1 FAV05.  
                    };
                }
                if (notaCrédito != null) {
                    line.CreditedQuantity = new CreditedQuantityType {
                        Value = línea.Cantidad, // 1..1 CAV04.
                        unitCode = Validar(ObtenerCódigoUnidad(producto.Unidad), "2..5"), // 1..1 CAV05.  
                    };
                }
                if (notaDébito != null) {
                    line.DebitedQuantity = new DebitedQuantityType {
                        Value = línea.Cantidad, // 1..1 DAV04.
                        unitCode = Validar(ObtenerCódigoUnidad(producto.Unidad), "2..5"), // 1..1 DAV05.  
                    };
                }

                line.LineExtensionAmount = new LineExtensionAmountType { 
                    Value = Validar(línea.SubtotalBase, "0..15 p (2..6)", 2), currencyID = moneda // 1..1 FAV06. currencyID: 1..1 FAV07. 
                };

                if (venta != null) { // Aunque se podría agregar a las notas, no tiene mucho sentido en estos documentos porque estos no deberían traer muestras gratis, entonces se prefiere omitir al ser opcional.

                    line.PricingReference = new PricingReferenceType { // 0..1 FAW01. Obligatorio informar si se trata de muestras comerciales.
                        AlternativeConditionPrice = new PriceType[] {  // 1..1 FAW02.
                            new PriceType {
                                PriceAmount = new PriceAmountType {
                                    Value = Validar(línea.Precio, "0..15 p (2..6)", 2), currencyID = moneda // 1..1 FAW03. currencyID: 1..1 FAW04.
                                },
                                PriceTypeCode =  new PriceTypeCodeType { Value = CódigoPrecioReferencia }, // 1..1 FAW05.
                            }
                        },
                    };

                }

                if (venta != null) { // Aunque se podría agregar a las notas, no tiene mucho sentido en estos documentos porque no hay razón para combinar descuentos adicionales dentro de una nota crédito, es un enredo, entonces se prefiere omitir al ser opcional.

                    line.AllowanceCharge = new AllowanceChargeType[] { 
                        new AllowanceChargeType { // 0..N FBE01. Este grupo se debe informar a nivel de ítem si el cargo o descuento afecta la base gravable del ítem (Es decir es un descuento comercial).
                            ID = new IDType { Value = "1" }, // FBE02.
                            ChargeIndicator = new ChargeIndicatorType { Value = false }, // 1..1 FAB03.
                            // AllowanceChargeReason = , // 0..1 FBE04. Por lo general es un descuento comercial. No se saturará la interfaz requiriendo esta información.
                            MultiplierFactorNumeric = new MultiplierFactorNumericType { 
                                Value = Validar(0, "1..6 p (0..2)", 2) // 1..1 FBE05. Aunque debería ser línea.PorcentajeDescuentoComercial * 100, por simplicidad al informar a la DIAN no se reportará porque este está incluído en el SubtotalBase.
                            }, 
                            Amount = new AmountType2 { 
                                Value = Validar(0, "4..15 p (2..6)", 2), currencyID = moneda // 1..1 FBE06. currencyID: 1..1 FBE07. Aunque debería ser línea.DescuentoComercial, por simplicidad al informar a la DIAN no se reportará porque este está incluído en el SubtotalBase.
                            }, 
                            BaseAmount = new BaseAmountType { 
                                Value = Validar(línea.SubtotalBase, "4..15 p (2..6)", 2), currencyID = moneda // 1..1 FBE08. currencyID: 1..1 FBE09.
                            },
                        }
                    };

                }

                if (!Empresa.TipoContribuyente.HasFlag(TipoContribuyente.RégimenSimple)) { // FAX01 no debe ser informado para facturas del régimen simple grupo I ni para ítems cuyo concepto en contratos de AIU no haga parte de la base gravable.
                   
                    line.TaxTotal = obtenerTaxTotales(new List<M> { línea }, out bool éxitoTaxTotalesLínea, out string? mensajeTaxTotalesLínea, 
                        AgrupaciónTaxTotales.Línea); // 0..N FAX01.
                    if (!éxitoTaxTotalesLínea) return Falso(out mensaje, mensajeTaxTotalesLínea);

                }
                // line.WithholdingTaxTotal = ; // 0..N FAY01. Igual que en FAT01. Incluye FAY02, FAY03, FAY04, FAY05, FAY06, FAY07, FAY08, FAY09, FAY10, FAY11, FAY12 y FAY13.

                line.Item = new ItemType {
                    Description = new DescriptionType[] { new DescriptionType { Value = Validar(producto.Descripción, "5..300", true) } }, // 1..3 FAZ02.
                    PackSizeNumeric = (int)producto.Unidad >= 1000 ? null : new PackSizeNumericType { Value = (int)producto.Unidad }, // 0..1 FAZ03. Como la documentación de la DIAN restringe este campo a 3 números cuando la unidad sea mayor o igual a 1000 se omite.
                    BrandName = producto.Marca == null ? null : new BrandNameType[]
                        { new BrandNameType { Value = Validar(producto.Marca.Nombre, "1..100", true) } }, // 0..3 FAZ04. Se agrega la marca si se tiene. Dependiendo del flujo del programa se puede o no tener la marca en este punto. Lo más probable es que si se tenga pero no se generará error si no.
                    // ModelName = new ModelNameType { Value = }, // 0..3 FAZ05. Tal vez se refiere a un modelo general aplicable para todos los vendedores de este producto. Por el momento no hay campo para especificar este valor en SimpleOps. Las empresas usuarias que deseen pueden usar la referencia para esto, pero esta referencia puede tener un uso diferente otra empresa usuaria entonces se prefiere no agregarla en la factura electrónicacomo ModelName, se agrega en SellersItemIdentiication.ID.
                    SellersItemIdentification = new ItemIdentificationType { // 0..1 FAZ06.
                        ID = new IDType { Value = Validar(producto.Referencia, "1..50", true) }, // 0..1 FAZ07.
                        // ExtendedID = new ExtendedIDType { Value = }, // 0..1 FAZ08 T1..50. Código del vendedor correspondiente a una subespecificación del producto.
                    },
                    StandardItemIdentification = new ItemIdentificationType { // 1..N FAZ09.
                        ID = new IDType { // 1..1 FAZ10.
                            Value = producto.Referencia, // No hay claridad a que se refiere con el estándar pero se asume como un código internacional, se usa entonces la referencia para este ID si se usa un estándar propio.
                            schemeID = CódigoEstándarAdopciónContribuyente, // 1..1 FAZ12. Para 0..1 FAZ13 no dice nada la documentación.
                            // schemeAgencyID = , // 0..1 FAZ14. Solo aplicable para tipo estándar GTIN.
                            // schemeAgencyName = , // 0..1 FAZ11. Solo aplicable para tipo estándar GTIN.
                        },
                    },
                    // AdditionalItemProperty = new ItemPropertyType { }; // 0..N FBF01. Incluye FBF02 y FBF03. Grupo de información para adicionar información específica del ítem que puede ser solicitada por autoridades o entidades diferentes a la DIAN. Se omite porque no se sabe en que casos es necesario.
                    // InformationContentProviderParty = new PartyType { }; // 0..1 FBA01. Incluye FBA02, FBA03, FAB04, FAB05, FAB06, FAB09, FAB07 y FBA08. Grupo de información que describen el mandante de la operación de venta. Aplica solo para mandatos y se debe informar a nivel de ítem. SimpleOps aún no soporta este tipo de operaciones, se omite.
                };

                line.Price = new PriceType { // 1..1 FBB01.
                    PriceAmount = new PriceAmountType { 
                        Value = Validar(línea.PrecioBase, "0..15 p (2..6)", 2), currencyID = moneda // 1..1 FBB02. currencyID: FBB03.
                    }, 
                    BaseQuantity = new BaseQuantityType { Value = línea.Cantidad, unitCode = ObtenerCódigoUnidad(producto.Unidad) }, // 1..1 FBB04. 1..1 FBB05.
                };

                lines[líneaID - 1] = line;
                líneaID++;

            }

            if (venta != null) document.InvoiceLine = lines;
            if (notaCrédito != null) document.CreditNoteLine = lines;
            if (notaDébito != null) document.DebitNoteLine = lines;

            #endregion Líneas>


            #region Escritura XML

            Ruta = ObtenerRuta(firmado: true);
            RutaSinFirmar = ObtenerRuta(firmado: false);
            var espaciosNombres = new XmlSerializerNamespaces();
            espaciosNombres.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            espaciosNombres.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            espaciosNombres.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            espaciosNombres.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            espaciosNombres.Add("sts", "dian:gov:co:facturaelectronica:Structures-2-1");
            espaciosNombres.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
            espaciosNombres.Add("xades141", "http://uri.etsi.org/01903/v1.4.1#");
            espaciosNombres.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            if (venta != null) espaciosNombres.Add("schemaLocation", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2     " + 
                "http://docs.oasis-open.org/ubl/os-UBL-2.1/xsd/maindoc/UBL-Invoice-2.1.xsd");
            if (notaCrédito != null) espaciosNombres.Add("schemaLocation", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2    " +
                "http://docs.oasis-open.org/ubl/os-UBL-2.1/xsd/maindoc/UBL-CreditNote-2.1.xsd");
            if (notaDébito != null) espaciosNombres.Add("schemaLocation", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2    " +
                "http://docs.oasis-open.org/ubl/os-UBL-2.1/xsd/maindoc/UBL-DebitNote-2.1.xsd");

            XmlSerializer? serializadorXml = Documento switch { // Se usa dynamic porque es manera más fácil de incorporar las 3 clases InvoiceType, CreditNoteType y DebitNoteType sin tener que modificar en exceso las clases en Dian.sln > Factura.cs y poder soportar de mejor manera posibles cambios futuros. Para el desarrollo cambiar el tipo de este objeto y en los otros 3 'switch's a al tipo que se esté implementando y ver en que puntos del código saca error. En esos puntos se debe asegurar que nunca entre la ejecución para el tipo de documento actual.        
                Venta _ => new XmlSerializer(typeof(InvoiceType), new Type[] { typeof(ExtensionContentDianType), typeof(ExtensionContentFirmaType) }), 
                NotaCréditoVenta _ 
                    => new XmlSerializer(typeof(CreditNoteType), new Type[] { typeof(ExtensionContentDianType), typeof(ExtensionContentFirmaType) }),
                NotaDébitoVenta _ 
                    => new XmlSerializer(typeof(DebitNoteType), new Type[] { typeof(ExtensionContentDianType), typeof(ExtensionContentFirmaType) }),
                _ => throw new Exception(CasoNoConsiderado(Documento.GetType().ToString()))
            };

            using var flujoEscritura = new StreamWriter(RutaSinFirmar);
            using var escritorXml = new XmlTextWriterSinXsi(flujoEscritura); // Para omitir los atributos automáticamente añadidos con nombre ExtensionContentFirmaType y ExtensionContentDianType.           
            if (IdentarXml) escritorXml.Formatting = Formatting.Indented;
            serializadorXml.Serialize(escritorXml, document, espaciosNombres);
            flujoEscritura.Close();

            if (IdentarXml) {

                MostrarInformación("El archivo XML ha sido identado para facilitar la verificación de problemas que hayan surgido en su creación." +
                    $"{DobleLínea}Ruta:{NuevaLínea}{RutaSinFirmar}");
                return Falso(out mensaje, "El archivo XML fue identado. No se soporta la firma de documentos XML identados. Cambia 'identarXml = true' a " +
                                          "'identarXml = false'.");

            }

            #endregion Escritura XML>


            #region Firma

            if (!Firmar())
                return Falso(out mensaje, $"No se pudo firmar el documento electrónico XML {RutaSinFirmar}. " + (!Equipo.MostrarMensajeErrorFirmador ? 
                    "Activa 'Opciones > Mostrar Mensaje Error en Firmador' e intenta nuevamente para ver el detalle del error." : ""));

            #endregion Firma>


            return true;

        } // Crear>

        
        public string ObtenerRuta(bool firmado) {

            if (Empresa.Nit == null) throw new Exception("El nit de la empresa es nulo.");
            if (Documento.ConsecutivoDianAnual == null) throw new Exception("El consecutivo de la DIAN anual es nulo.");
            var prefijo = TipoFirma.ATexto();
            var nombreArchivo = $"{prefijo}{Empresa.Nit.PadLeft(10, '0')}000{AhoraUtcAjustado.ATexto("yy")}" +
                $"{((int)Documento.ConsecutivoDianAnual).ATexto().PadLeft(8, '0')}{(firmado ? "" : "-sf")}{".xml"}";

            return Path.Combine(RutaDocumentosElectrónicosHoy, nombreArchivo);

        } // ObtenerRuta>


        /// <summary>
        /// Ejecuta Firmador.exe y firma el documento electrónico.
        /// </summary>
        public bool Firmar() {

            if (!ExisteRuta(TipoElementoRuta.Archivo, Equipo.RutaCertificado, "certificado de firma digital", out string? mensaje)) throw new Exception(mensaje); // Se maneja como excepción porque no debería llegar a este punto sin este archivo.

            var informaciónInicio = new ProcessStartInfo(RutaFirmador) {
                Arguments = @$"""{Equipo.RutaCertificado}"" {Equipo.ClaveCertificado} ""{ObtenerRuta(firmado: false)}"" ""{Ruta}"" " +
                @$"""{AhoraUtcAjustado.ATexto(FormatoFechaHora)}"" {TipoFirma} {Equipo.MostrarMensajeErrorFirmador.ATexto()}"
            };
  
            using var proceso = Process.Start(informaciónInicio);
            proceso.WaitForExit();

            return File.Exists(Ruta);

        } // Firmar>


        /// <summary>
        /// Útil cuando se están realizando pruebas con SoapUI.
        /// </summary>
        /// <returns></returns>
        public bool CrearZip() {

            if (Ruta == null) throw new Exception("No se esperaba que la ruta estuviera nula. Ejecuta primero Crear().");
            Vixark.General.CrearZip(Ruta);
            return true;

        } // CrearZip>


        /// <summary>
        /// Envía el documento electrónico empaquetado en un ZIP a la DIAN.
        /// </summary>
        public bool Enviar(out string? mensaje, out XmlDocument? respuestaXml) {

            respuestaXml = null;
            if (Ruta == null) return Falso(out mensaje, "No se esperaba que la ruta estuviera nula. Ejecuta primero Crear().");

            var cuerpo = $"<wcf:SendBillSync>" +
                             $"<wcf:fileName>{Path.GetFileName(Ruta).Reemplazar("xml", "zip")}</wcf:fileName>" +
                             $"<wcf:contentFile>{Convert.ToBase64String(ObtenerBytesZip(Ruta))}</wcf:contentFile>" +
                         $"</wcf:SendBillSync>";

            return EnviarSolicitud(cuerpo, Operación.SendBillSync, out mensaje, out respuestaXml);

        } // Enviar>


        /// <summary>
        /// Envía el documento electrónico empaquetado en un ZIP a al set de pruebas de la DIAN.
        /// </summary>
        public bool EnviarPrueba(out string? mensaje, out XmlDocument? respuestaXml) {

            respuestaXml = null;
            if (Ruta == null) return Falso(out mensaje, "No se esperaba que la ruta estuviera nula. Ejecuta primero Crear().");
            if (Empresa.IdentificadorPruebas == null) return Falso(out mensaje, "El identificador de pruebas está vacío.");

            var cuerpo = $"<wcf:SendTestSetAsync>" +
                             $"<wcf:fileName>{Path.GetFileName(Ruta).Reemplazar("xml", "zip")}</wcf:fileName>" +
                             $"<wcf:contentFile>{Convert.ToBase64String(ObtenerBytesZip(Ruta))}</wcf:contentFile>" +
                             $"<wcf:testSetId>{Empresa.IdentificadorPruebas}</wcf:testSetId>" +
                         $"</wcf:SendTestSetAsync>";

            return EnviarSolicitud(cuerpo, Operación.SendTestSetAsync, out mensaje, out respuestaXml);

        } // EnviarPrueba>

        #endregion Métodos y Funciones>


    } // DocumentoElectrónico>



} // SimpleOps.Legal>