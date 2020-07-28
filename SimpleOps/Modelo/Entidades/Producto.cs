using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Producto o servicio que la empresa vende.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Producto : Rastreable { // Es Rastreable porque los productos son actualizados frecuentemente y es de interés tener la información de su creación.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string Referencia { get; set; } = null!; // Obligatoria y única. No es la clave principal porque podría ser cambiada y aumentaría mucho el tamaño de las tablas que la relacionan.

        /// <summary>
        /// La unidad de venta del producto. Se escribe en campo Unidad de la factura. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        public Unidad Unidad { get; set; } = Unidad.Unidad;

        /// <summary>
        /// Permite ajustar los pedidos a los proveedores para que se hagan en unidades de empaque. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        public Unidad UnidadEmpaque { get; set; } = Unidad.Desconocida;

        /// <summary>
        /// En kg. Se usa porque es unidad básica del sistema internacional.
        /// </summary>
        public double? PesoUnidadEmpaque { get; set; }

        /// <summary>
        /// En m x m x m. Se usa m porque es unidad básica del sistema internacional.
        /// </summary>
        public Dimensión? DimensiónUnidadEmpaque { get; set; }

        /// <MaxLength>200</MaxLength>
        [MaxLength(200)]
        public string? Descripción { get; set; }

        /// <summary>
        /// Cantidad en inventario.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Cantidad mínima recomendada en inventario. Cuando el inventario baja de este nivel se sugiere pedirlo.
        /// </summary>
        public int CantidadMínima { get; set; }

        /// <summary>
        /// Cantidad máxima recomendada en inventario. Al programar un pedido se calculan las cantidades para no superar por mucho este valor.
        /// </summary>
        public int CantidadMáxima { get; set; }

        /// <summary>
        /// Cantidad reservada por ordenes de compra activas.
        /// </summary>
        public int CantidadReservada { get; set; }

        /// <summary>
        /// Si es verdadero el producto existe en el mundo físico. Si es falso no maneja inventario como en el caso de de los servicios y productos virtuales.
        /// </summary>
        public bool Físico { get; set; } = true;

        /// <summary>
        /// El costo de compra por unidad y gastos asociados a su producción y obtención. Incluye costo de transporte. Si es nulo es desconocido.
        /// </summary>
        public decimal? CostoUnitario { get; set; } = null; // Se permite nulo porque el valor 0 significa costo unitario cero, que podría ser válido en algunas situaciones.

        /// <summary>
        /// Un tipo personalizado para clasificarlo.
        /// </summary>
        public Subcategoría? Subcategoría { get; set; }
        public int? SubcategoríaID { get; set; }

        /// <summary>
        /// La línea de negocio a la que pertenece.
        /// </summary>
        public LíneaNegocio? LíneaNegocio { get; set; }
        public int? LíneaNegocioID { get; set; }

        public Marca? Marca { get; set; }
        public int? MarcaID { get; set; }

        /// <summary>
        /// Material principal del que está hecho.
        /// </summary>
        public Material? Material { get; set; }
        public int? MaterialID { get; set; }

        /// <summary>
        /// El uso principal que se le da.
        /// </summary>
        public Aplicación? Aplicación { get; set; }
        public int? AplicaciónID { get; set; }

        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Si es desconocida se usan las reglas en opciones. Para implementaciones personalizadas. Establece la prioridad de los productos que se sincronizan con el sitio web. Si la prioridad es Ninguna no se sincroniza con el sitio web.
        /// </summary>
        public Prioridad PrioridadWebPropia { get; set; } = Prioridad.Desconocida;

        /// <summary>
        /// El proveedor, que independiente de cualquier otra regla, será el que se use en la programación de sus pedidos.
        /// </summary>
        [ForeignKey("ProveedorPreferidoID")] 
        public Proveedor? ProveedorPreferido { get; set; }
        public int? ProveedorPreferidoID { get; set; }

        /// <summary>
        /// Texto que describe su ubicación en el almacén. Por ejemplo, A5-E3-S2 se puede usar para describir almacén 5, estantería 3 y sección 2.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)] 
        public string? UbicaciónAlmacén { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). Si es cero es exento de IVA.
        /// </summary>
        public double? PorcentajeIVAPropio { get; set; }

        /// <summary>
        /// Si es verdadero es excluído de IVA y tiene un tratamiento tributario diferente porque no suma a la base tributable. Si es falso y 
        /// PorcentajeIVAPropio = 0 (exentos) o PorcentajeIVAPropio > 0 si suma a la base tributable. Si se establece este valor en verdadero se le dará 
        /// prioridad sin importar el valor en PorcentajeIVAPropio y el porcentaje de IVA efectivo (PorcentajeIVA) será cero.
        /// </summary>
        public bool ExcluídoIVA { get; set; } = false;

        /// <summary>
        /// Si es nulo se usa el porcentaje en opciones. Si es cero es exento de cualquier tipo de impuesto al consumo porcentual.
        /// </summary>
        public double? PorcentajeImpuestoConsumoPropio { get; set; }

        /// <summary>
        /// Si es nulo se usa el el valor en opciones. Si es cero es exento de cualquier tipo de impuesto al consumo por unidad.
        /// </summary>
        public decimal? ImpuestoConsumoUnitarioPropio { get; set; }

        /// <summary>
        /// Si es desconocido se usa el tipo en opciones. Se usa para relacionarlo con TipoTributo que será enviado a la DIAN en la 
        /// factura electrónica. También se usa para establecer el valor de ImpuestoConsumoUnitario y PorcentajeImpuestoConsumo si el tipo
        /// se encuentra en los diccionarios en opciones <see cref="Singleton.OpcionesGenerales.PorcentajesImpuestosConsumo"/> o 
        /// <see cref="Singleton.OpcionesGenerales.ValoresUnitariosImpuestosConsumo" />.
        /// Posibles valores: Desconocido = 0, General = 1 (Valor general que no tiene asociado una tasa automáticamente, se debe especificar por producto), 
        /// BolsasPlásticas = 2, Carbono = 3, Combustibles = 4, DepartamentalNominal = 5, 
        /// DepartamentalPorcentual = 6, SobretasaCombustibles = 7, TelefoníaCelularYDatos = 8, Otro = 255.
        /// </summary>
        public TipoImpuestoConsumo TipoImpuestoConsumoPropio { get; set; } = TipoImpuestoConsumo.Desconocido; // Aunque TipoTributo puede no puede ser desconocido se permite aquí porque es el valor que permite usar el predeterminado en opciones.

        /// <summary>
        /// Referencias de otros productos asociados o similares o recomendados alternativos al producto. Son agregados a la cotización cuando se agrega el producto. Además permite otras funciones en implementaciones personalizadas al sincronizar con el sitio web.
        /// </summary>
        /// <MaxLength>1000</MaxLength>
        [MaxLength(1000)]
        public List<string> ProductosAsociados { get; set; } = new List<string>(); // No se crea otra tabla para manejar estos valores porque es una función particular.

        /// <summary>
        /// Si es nulo se usan las reglas en opciones. El porcentaje de ganancia que se le sumará al porcentaje de ganancia del cliente para obtener el porcentaje de ganancia total. El porcentaje de ganancia total se le aplica a los costos de los productos para obtener sus precios de venta. Es útil cuando no se quieren usar las listas de precios o cuando el producto no está en ellas.
        /// </summary>
        public double? PorcentajeAdicionalGananciaPropio { get; set; }

        /// <summary>
        /// <para>Si es nulo se usa el <see cref="Singleton.OpcionesEmpresa.ConceptoRetenciónPredeterminado"/> de opciones. Es el concepto de retención aplicable 
        /// a este producto según la tabla de retención en la fuente. Al establecer este concepto también se está estableciendo si el producto es un 
        /// Producto o Servicio, información que se usa en otros lugares, como en el cálculo del mínimo para la retención del IVA.</para>
        /// Desconocido = 0, Generales = 1, TarjetaDébitoOCrédito = 2, AgrícolasOPecuariosSinProcesamiento = 3, AgrícolasOPecuaríosConProcesamiento = 4, 
        /// CaféPergaminoOCereza = 5, CombustiblesDerivadosPetróleo = 6, ActivosFijosPersonasNaturales = 7, Vehículos = 8, BienesRaícesVivienda = 9, 
        /// BienesRaícesNoVivienda = 10, ServiciosGenerales = 11, EmolumentosEclesiásticos = 12, TransporteCarga = 13, 
        /// TransporteNacionalTerrestrePasajeros = 14, TransporteNacionalAéreoOMarítimoPasajeros = 15, ServiciosPorEmpresasTemporales = 16, 
        /// ServiciosPorEmpresasVigilanciaYAseo = 16, SaludPorIPS = 17, HotelesYRestaurantes = 18, ArrendamientoBienesMuebles = 19, 
        /// ArrendamientoBienesInmuebles = 20, OtrosIngresosTributarios = 21, HonorariosYComisiones = 22, LicenciamientoSoftware = 23, Intereses = 24, 
        /// RendimientosFinacierosRentaFija = 25, LoteríasRifasYApuestas = 26, ColocaciónIndependienteJuegosAzar = 27, ContratosConstruccionYUrbanización = 28.
        /// </summary>
        public ConceptoRetención ConceptoRetenciónPropio { get; set; } = ConceptoRetención.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private Producto() { } // Solo para que EF Core no saque error.

        public Producto(string referencia) => (Referencia) = (referencia);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Cantidad disponible en inventario para ordenes de compra nuevas.
        /// </summary>
        public int CantidadDisponible => Cantidad - CantidadReservada;

        /// <summary>
        /// El costo de compra y gastos asociados a su producción y obtención de la cantidad total en inventario. Incluye costo de transporte.
        /// </summary>
        public decimal? CostoTotal => ObtenerSubtotal(CostoUnitario, Cantidad);

        public double? GramosUnidadEmpaque => ObtenerGramos(PesoUnidadEmpaque);

        /// <summary>
        /// En m3. Se usa porque es unidad básica del sistema internacional.
        /// </summary>
        public double? VolumenUnidadEmpaque => DimensiónUnidadEmpaque?.Volumen;

        public double? CentímetrosCúbicosUnidadEmpaque => ObtenerCentimétrosCúbicos(VolumenUnidadEmpaque);

        public double PorcentajeIVA => ExcluídoIVA ? 0 : (PorcentajeIVAPropio ?? Empresa.PorcentajeIVAPredeterminadoEfectivo); // Al realizar ventas se debe usar la función Global.ObtenerPorcentajeIVAVenta() que tiene en cuenta el cliente y el municipio.
       
        public bool ExentoIVA => !ExcluídoIVA && PorcentajeIVA == 0;

        public TipoImpuestoConsumo TipoImpuestoConsumo 
            => TipoImpuestoConsumoPropio == TipoImpuestoConsumo.Desconocido ? Empresa.TipoImpuestoConsumoPredeterminado : TipoImpuestoConsumoPropio; 

        public TipoTributo TipoTributoConsumo => ObtenerTipoTributo(TipoImpuestoConsumo);

        public double PorcentajeImpuestoConsumo => Generales.PorcentajesImpuestosConsumo.ObtenerValor(TipoImpuestoConsumo) ?? PorcentajeImpuestoConsumoPropio 
            ?? Empresa.PorcentajeImpuestoConsumoPredeterminado;

        public decimal ImpuestoConsumoUnitario => Generales.ValoresUnitariosImpuestosConsumo.ObtenerValor(TipoImpuestoConsumo) ?? ImpuestoConsumoUnitarioPropio 
            ?? Empresa.ImpuestoConsumoUnitarioPredeterminado;

        public ModoImpuesto ModoImpuestoConsumo => PorcentajeImpuestoConsumo > 0 ? ModoImpuesto.Porcentaje
            : (ImpuestoConsumoUnitario > 0 ? ModoImpuesto.Unitario : ModoImpuesto.Exento);

        public Prioridad PrioridadWeb => PrioridadWebPropia != Prioridad.Desconocida ? PrioridadWebPropia : ObtenerPrioridadWebProducto();

        public ConceptoRetención ConceptoRetención 
            => ConceptoRetenciónPropio != ConceptoRetención.Desconocido ? ConceptoRetenciónPropio : Empresa.ConceptoRetenciónPredeterminado;

        public double PorcentajeAdicionalGanancia => PorcentajeAdicionalGananciaPropio ?? ObtenerPorcentajeGananciaAdicionalProducto();


        public TipoProducto TipoProducto
            => ConceptoRetención switch {
                ConceptoRetención.Desconocido => throw new Exception("No se esperaba ConceptoRetención = Desconocido."),
                ConceptoRetención.Generales => TipoProducto.Producto,
                ConceptoRetención.TarjetaDébitoOCrédito => TipoProducto.Producto, // No permite establecer si es producto o servicio entonces se deja producto.
                ConceptoRetención.AgrícolasOPecuariosSinProcesamiento => TipoProducto.Producto,
                ConceptoRetención.AgrícolasOPecuaríosConProcesamiento => TipoProducto.Producto,
                ConceptoRetención.CaféPergaminoOCereza => TipoProducto.Producto,
                ConceptoRetención.CombustiblesDerivadosPetróleo => TipoProducto.Producto,
                ConceptoRetención.ActivosFijosPersonasNaturales => TipoProducto.Producto,
                ConceptoRetención.Vehículos => TipoProducto.Producto,
                ConceptoRetención.BienesRaícesVivienda => TipoProducto.Producto,
                ConceptoRetención.BienesRaícesNoVivienda => TipoProducto.Producto,
                ConceptoRetención.ServiciosGenerales => TipoProducto.Servicio,
                ConceptoRetención.EmolumentosEclesiásticos => TipoProducto.Servicio,
                ConceptoRetención.TransporteCarga => TipoProducto.Servicio,
                ConceptoRetención.TransporteNacionalTerrestrePasajeros => TipoProducto.Servicio,
                ConceptoRetención.TransporteNacionalAéreoOMarítimoPasajeros => TipoProducto.Servicio,
                ConceptoRetención.ServiciosPorEmpresasTemporales => TipoProducto.Servicio,
                ConceptoRetención.ServiciosPorEmpresasVigilanciaYAseo => TipoProducto.Servicio,
                ConceptoRetención.SaludPorIPS => TipoProducto.Servicio,
                ConceptoRetención.HotelesYRestaurantes => TipoProducto.Servicio,
                ConceptoRetención.ArrendamientoBienesMuebles => TipoProducto.Servicio,
                ConceptoRetención.ArrendamientoBienesInmuebles => TipoProducto.Servicio,
                ConceptoRetención.OtrosIngresosTributarios => TipoProducto.Servicio, // Duda.
                ConceptoRetención.HonorariosYComisiones => TipoProducto.Servicio,
                ConceptoRetención.LicenciamientoSoftware => TipoProducto.Servicio, // Licenciamiento es servicio https://www.portafolio.co/economia/finanzas/pago-iva-adquirir-software-389488. 
                ConceptoRetención.Intereses => TipoProducto.Servicio,
                ConceptoRetención.RendimientosFinacierosRentaFija => TipoProducto.Servicio,
                ConceptoRetención.LoteríasRifasYApuestas => TipoProducto.Servicio,
                ConceptoRetención.ColocaciónIndependienteJuegosAzar => TipoProducto.Servicio,
                ConceptoRetención.ContratosConstruccionYUrbanización => TipoProducto.Servicio,
                _ => throw new Exception(CasoNoConsiderado(ConceptoRetención)),
            };


        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string UnidadTexto => Unidad.ToString();

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones Estáticas

        public static int ObtenerID(Producto? producto) => producto?.ID ?? 0; // El 0 es un índice inválido se usará solo en casos donde se necesita crear esta entidad sin conexión, como en migraciones, carga de CSVs y otros. 

        #endregion Métodos y Funciones Estáticas>


        #region Métodos y Funciones

        public override string ToString() => Referencia;

        #endregion Métodos y Funciones>


    } // Producto>



} //  SimpleOps.Modelo>
