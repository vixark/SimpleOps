using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using Vixark;
using System.IO;
using SimpleOps.DocumentosGráficos;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Producto o servicio que la empresa vende. Si tiene <see cref="Producto.Base"/>, se le llama Producto Específico.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Producto : Rastreable { // Los atributos que hacen diferentes a productos con el mismo producto base se pueden manejar usando diferentes técnicas o con una combinación de ellas: 1. Con una columna que contenga todos los atributos adicionales en List<string> o texto plano completo o texto separado por algún carácter. 2. Con múltiples columnas que contengan el valor de cada atributo adicional asociandolos a una tabla con valores posibles para cada atributo o sin ninguna asociación o asociándolas a una enumeración, 3. Con una tabla adicional para registrar todos los atributos. Ver 5752/how-to-design-a-product-table-for-many-kinds-of-product-where-each-product-has-m). Es Rastreable porque los productos son actualizados frecuentemente y es de interés tener la información de su creación.


        #region Propiedades 
        // Solo se deben agregar aquí las propiedades que claramente no tenga sentido que sean compartidas mediante un ProductoBase. Propiedades como la cantidad en inventario claramente están asociadas a una entidad física y no tienen sentido ser compartidas en el producto base. Otras cómo la marca o el IVA que aplica al producto si son compartibles porque se espera que todos los productos del producto base compartan el mismo valor.

        [Key]
        public int ID { get; set; }

        /// <MaxLength>30</MaxLength>
        /// <summary>
        /// Identificador único del producto en inventario. También llamado SKU.
        /// </summary>
        [MaxLength(30)]
        public string Referencia { get; set; } = null!; // Obligatoria y única. No es la clave principal porque podría ser cambiada y aumentaría mucho el tamaño de las tablas que la relacionan.

        /// <summary>
        /// El producto base con el que este producto comparte varias características comunes. 
        /// </summary>
        public ProductoBase? Base { get; set; } // Es opcional para permitir flexibilidad de manejar la base de datos solo con la tabla Productos o usando la tabla ProductosBase para todos o solo para algunos productos. A nivel de interfaz se le permite al usuario de manera transparente crear un producto sin pensar si necesita o no producto base, solo cuando se agregan atributos el usuario se entera de esta posibilidad. El uso del objeto Producto es transparente a si este tiene o no tiene producto base, la funcionalidad principal del producto base es agrupar información repetida para poder cambiarla en un solo lugar.
        public int? BaseID { get; set; }

        /// <summary>
        /// Permite identificar si un producto es independiente de los otros o tiene un producto base en común con otros mediante el cual comparten
        /// características. 
        /// </summary>
        public bool TieneBase { get; set; } // Es importante a nivel de desarrollo porque permite detectar errores en los que no se ha cargado el Producto base de la base de datos y este si exista, o en los que se tenga un producto base cargado y TieneBase sea falso. Al leer o escribir las propiedades del producto base en el producto se verifican estas inconsistencias para evitar comportamientos no deseados silenciosos.

        /// <summary>
        /// <see cref="AtributoProducto"/> que diferencian productos que comparten el mismo producto base. Por ejemplo, puede tener atributos Talla XL 
        /// y Verde. Los atributos se agregan al final de la descripción del producto base para formar la descripción del producto. Preferiblemente
        /// no se debe modificar este objeto directamente, si no hacerlo mediante <see cref="AgregarAtributo(string, bool?)" /> y
        /// <see cref="EliminarAtributo(string)"/> para evitar duplicados y mantenerlos con la capitalización correcta de la tabla AtributosProductos. 
        /// No se debe confundir con <see cref="Características"/>, las características normalmente se agregan al producto base y pueden ser compartidas
        /// por los productos específicos que tienen el mismo producto base.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public List<string> Atributos { get; set; } = new List<string>(); // Se consideró la opción de hacer esta lista de tipo AtributoProducto, pero esto implicaría dos escenarios: uno, se perdería la posibilidad de agregar atributos libres o dos, se tendría que agregar una nueva propiedad de lista tipo texto de atributos libres. Para evitar agregar más columnas y complejizar (tal vez inneceseariamente) la tabla producto no se implementó la segunda opción y cómo se quiere permitir los atributos libres, la primera tampoco se consideró. Además, almacenar los atributos con el ID de su valor en la enumeración es peligroso porque un usuario del código poco cuidadoso podría añadir un nuevo elemento a la enumeración AtributosProductos y haría que los valores almacenados en la base de datos apunten a valores de atributos diferentes. También, al tener los atributos almacenados como texto se permite buscar más fácil directamente en la base de datos, por ejemplo se puede buscar Amarillo y se devolverían todos los productos que tengan algún amarillo en sus atributos. Si se usara una lista con enumeraciones esto solo se podría hacer después de cargar todos los productos usando la propiedad Descripción o buscando coincidencias de un ID de enumeración particular (el ID del amarillo), con lo que se perdería la posibilidad de obtener resultados de productos que contengan otros amarillos. Se decidió entonces escribirlos como texto e implementar métodos y funciones auxiliares para mantenerlos estandarizados y no repetidos, ver la región Métodos y Funciones de Atributos. Si es muy importante tener un atributo tipado directamente en la base de datos, se puede considerar crear una propiedad con tipo enumeración para la propiedad requerida y tenerla en cuenta en el cálculo de la descripción, por ejemplo se podría crear la propiedad Talla con valor de una enumeración TallasProductos o se podría crear una columna Colores con valor una lista de enumeración de tipo ColoresProductos y usar estos valores de estos "atributos especiales tipados" para construir la descripción del producto. Aunque como los atributos no solo afectan la descripción si no que son usados en multitud de lugares como la generación de catálogos, fichas, etc, la mejor manera de implementar columnas nuevas con atributos tipados como Talla sería crear una nueva propiedad AtributosEfectivos() que agrupe los atributos de esta propiedad y los otros atributos tipados con su columna propia. Por el momento esto no se considera necesario para el desarrollo general del código. 

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
        /// El costo de compra por unidad y gastos asociados a su producción y obtención. Incluye costo de transporte. Si es nulo es desconocido.
        /// </summary>
        public decimal? CostoUnitario { get; set; } = null; // Se permite nulo para indicar que no se conoce el costo, esto porque el valor 0 significa costo unitario cero, que podría ser válido en algunas situaciones.

        /// <summary>
        /// Texto que describe su ubicación en el almacén. Por ejemplo, A5-E3-S2 se puede usar para describir almacén 5, estantería 3 y sección 2.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? UbicaciónAlmacén { get; set; }

        /// <summary>
        /// Referencias de otros productos asociados, similares o recomendados alternativos al producto. Permite varias funciones en implementaciones 
        /// personalizadas.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public List<string> ProductosAsociados { get; set; } = new List<string>(); // No se crea otra tabla para manejar estos valores porque es una función particular.

        #endregion Propiedades>


        #region Propiedades de Producto Base
        // Todas las propiedades en la sección Propiedades en Producto de ProductoBase deben ser replicadas aquí con su versión de enlace y con su versión específica. La propiedad de enlace (la que no es específica) sirve para encapsular la información solicitada o a escribir y decidir si se debe usar la específica o la del producto base. La propiedad específica sirve para escribir un valor que solo aplica al producto actual y no es igual a al valor del resto de los productos que comparten el mismo producto base. Si el valor fuera igual para todos los productos que comparten el mismo producto base, se debe asignar este valor en la propiedad correspondiente del producto base. Por ejemplo, si se quisiera asignar un PesoUnidadEmpaque diferente a un producto con atributo talla XXL, se añadiría este valor a PesoUnidadEmpaqueEspecífico y este se usaría en vez del valor del producto base que comparten el resto de los productos con otras tallas. La propiedad específica es la que se escribe en la base de datos y debe tener el mismo tipo y longitud que las propiedades en el ProductoBase y debe permitir nulo o si es una enumeración, permitir el elemento Ninguno/Desconocido. La propiedad de enlace debe tener el mismo tipo, longitud y tipo de nulidad que la propiedad en ProductoBase. Si una propiedad tiene un valor predeterminado diferente de nulo/desconocido se debe implementar el patrón de asignación de valor predeterminado, ver ejemplo de implementación en UnidadEmpaque.

        public Unidad UnidadEspecífica { get; set; } = Unidad.Desconocida;
        /// <summary>
        /// La unidad de venta del producto. Se escribe en campo Unidad de la factura. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        [NotMapped]
        public Unidad Unidad { // Aunque se consideró usar funciones genéricas para esta propiedad, se prefiere hacer copiado y pegado del código para evitar agregar más complejidades. Con funciones genéricas surgen problemas en el procedimiento de asignación al no poder pasar propiedades como parámetros ref en la función genérica, requiriendo la adición de variables de campo de tipo _Unidad, no se logra una redución importante del código escrito y se añade complejidad innecesaria. 
            get => (UsarPropiedadesBase() && UnidadEspecífica == Unidad.Desconocida) // La propiedad UsarPropiedadesBase ya verifica la no nulidad de Base.
                       ? Base!.Unidad : (UnidadEspecífica == Unidad.Desconocida ? Empresa.UnidadPredeterminadaProducto : UnidadEspecífica); // Es necesario usar la unidad predeterminada porque esta propiedad tiene una unidad predeterminada establecida en el producto base y si no tiene producto base no tiene de donde sacar el valor predeterminado. No se puede establecer como predeterminada en Producto.UnidadEspecífica porque este valor reemplazaría siempre el del producto base.
            set { if (!UsarPropiedadesBase(escritura: true)) { UnidadEspecífica = value; } }
        } 

        public Unidad UnidadEmpaqueEspecífica { get; set; } = Unidad.Desconocida;
        /// <summary>
        /// Permite ajustar los pedidos a los proveedores para que se hagan en unidades de empaque. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        [NotMapped]
        public Unidad UnidadEmpaque { 
            get => (UsarPropiedadesBase() && UnidadEmpaqueEspecífica == Unidad.Desconocida) ? Base!.UnidadEmpaque : 
                (UnidadEmpaqueEspecífica == Unidad.Desconocida ? Empresa.UnidadEmpaquePredeterminadaProducto : UnidadEmpaqueEspecífica); // Es necesario usar la unidad predeterminada porque esta propiedad tiene una unidad predeterminada establecida en el producto base y si no tiene producto base no tiene de donde sacar el valor predeterminado. No se puede establecer como predeterminada en Producto.UnidadEspecífica porque reemplazaría la del base.
            set { if (!UsarPropiedadesBase(escritura: true)) { UnidadEmpaqueEspecífica = value; } }
        } 

        public double? PesoUnidadEmpaqueEspecífica { get; set; }
        /// <summary>
        /// En kg.
        /// </summary>
        [NotMapped]
        public double? PesoUnidadEmpaque { // Se usa kg porque es unidad básica del sistema internacional.
            get => (UsarPropiedadesBase() && PesoUnidadEmpaqueEspecífica == null) ? Base!.PesoUnidadEmpaque : PesoUnidadEmpaqueEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { PesoUnidadEmpaqueEspecífica = value; } }
        }

        public Dimensión? DimensiónUnidadEmpaqueEspecífica { get; set; }
        /// <summary>
        /// En m x m x m.
        /// </summary>
        [NotMapped]
        public Dimensión? DimensiónUnidadEmpaque { // Se usa m porque es unidad básica del sistema internacional.
            get => (UsarPropiedadesBase() && DimensiónUnidadEmpaqueEspecífica == null) ? Base!.DimensiónUnidadEmpaque : DimensiónUnidadEmpaqueEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { DimensiónUnidadEmpaqueEspecífica = value; } }
        }

        public Subcategoría? SubcategoríaEspecífica { get; set; }
        /// <summary>
        /// Un tipo personalizado para clasificarlo.
        /// </summary>
        [NotMapped]
        public Subcategoría? Subcategoría {
            get => (UsarPropiedadesBase() && SubcategoríaEspecífica == null) ? Base!.Subcategoría : SubcategoríaEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { SubcategoríaEspecífica = value; } }
        }
        public int? SubcategoríaEspecíficaID { get; set; }
        [NotMapped]
        public int? SubcategoríaID {
            get => (UsarPropiedadesBase() && SubcategoríaEspecíficaID == null) ? Base!.SubcategoríaID : SubcategoríaEspecíficaID;
            set { if (!UsarPropiedadesBase(escritura: true)) { SubcategoríaEspecíficaID = value; } }
        }

        public LíneaNegocio? LíneaNegocioEspecífica { get; set; }
        /// <summary>
        /// La línea de negocio a la que pertenece.
        /// </summary>
        [NotMapped]
        public LíneaNegocio? LíneaNegocio {
            get => (UsarPropiedadesBase() && LíneaNegocioEspecífica == null) ? Base!.LíneaNegocio : LíneaNegocioEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { LíneaNegocioEspecífica = value; } }
        }
        public int? LíneaNegocioEspecíficaID { get; set; }
        [NotMapped]
        public int? LíneaNegocioID {
            get => (UsarPropiedadesBase() && LíneaNegocioEspecíficaID == null) ? Base!.LíneaNegocioID : LíneaNegocioEspecíficaID;
            set { if (!UsarPropiedadesBase(escritura: true)) { LíneaNegocioEspecíficaID = value; } }
        }

        public Marca? MarcaEspecífica { get; set; }
        [NotMapped]
        public Marca? Marca {
            get => (UsarPropiedadesBase() && MarcaEspecífica == null) ? Base!.Marca : MarcaEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { MarcaEspecífica = value; } }
        }
        public int? MarcaEspecíficaID { get; set; }
        [NotMapped]
        public int? MarcaID {
            get => (UsarPropiedadesBase() && MarcaEspecíficaID == null) ? Base!.MarcaID : MarcaEspecíficaID;
            set { if (!UsarPropiedadesBase(escritura: true)) { MarcaEspecíficaID = value; } }
        }

        public Material? MaterialEspecífico { get; set; }
        /// <summary>
        /// Material principal del que está hecho.
        /// </summary>
        [NotMapped]
        public Material? Material {
            get => (UsarPropiedadesBase() && MaterialEspecífico == null) ? Base!.Material : MaterialEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { MaterialEspecífico = value; } }
        }
        public int? MaterialEspecíficoID { get; set; }
        [NotMapped]
        public int? MaterialID {
            get => (UsarPropiedadesBase() && MaterialEspecíficoID == null) ? Base!.MaterialID : MaterialEspecíficoID;
            set { if (!UsarPropiedadesBase(escritura: true)) { MaterialEspecíficoID = value; } }
        }

        public Aplicación? AplicaciónEspecífica { get; set; }
        /// <summary>
        /// El uso principal que se le da.
        /// </summary>
        [NotMapped]
        public Aplicación? Aplicación {
            get => (UsarPropiedadesBase() && AplicaciónEspecífica == null) ? Base!.Aplicación : AplicaciónEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { AplicaciónEspecífica = value; } }
        }
        public int? AplicaciónEspecíficaID { get; set; }
        [NotMapped]
        public int? AplicaciónID {
            get => (UsarPropiedadesBase() && AplicaciónEspecíficaID == null) ? Base!.AplicaciónID : AplicaciónEspecíficaID;
            set { if (!UsarPropiedadesBase(escritura: true)) { AplicaciónEspecíficaID = value; } }
        }

        public bool? FísicoEspecífico { get; set; }
        /// <summary>
        /// Si es verdadero el producto existe en el mundo físico. Si es falso no maneja inventario como en el caso de de los servicios y 
        /// productos virtuales.
        /// </summary>
        [NotMapped]
        public bool Físico {
            get => (UsarPropiedadesBase() && FísicoEspecífico == null) ? Base!.Físico : FísicoEspecífico ?? Empresa.FísicoPredeterminadoProducto; // Se usa el valor predeterminado de la propiedad base (ProductoFísicoPredeterminado) porque se requiere que FísicoEspecífico pueda ser nulo para los casos de productos con productos base en los que se usa el valor del producto base.
            set { if (!UsarPropiedadesBase(escritura: true)) { FísicoEspecífico = value; } }
        }

        public double? PorcentajeIVAPropioEspecífico { get; set; }
        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). Si es cero es exento de IVA.
        /// </summary>
        [NotMapped]
        public double? PorcentajeIVAPropio {
            get => (UsarPropiedadesBase() && PorcentajeIVAPropioEspecífico == null) ? Base!.PorcentajeIVAPropio : PorcentajeIVAPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { PorcentajeIVAPropioEspecífico = value; } }
        }

        public bool? ExcluídoIVAEspecífico { get; set; }
        /// <summary>
        /// Si es verdadero es excluído de IVA y tiene un tratamiento tributario diferente porque no suma a la base tributable. Si es falso y 
        /// PorcentajeIVAPropio = 0 (exentos) o PorcentajeIVAPropio > 0 si suma a la base tributable. Si se establece este valor en verdadero se le dará 
        /// prioridad sin importar el valor en PorcentajeIVAPropio y el porcentaje de IVA efectivo (PorcentajeIVA) será cero.
        /// </summary>
        [NotMapped]
        public bool ExcluídoIVA {
            get => (UsarPropiedadesBase() && ExcluídoIVAEspecífico == null) 
                ? Base!.ExcluídoIVA : ExcluídoIVAEspecífico ?? Empresa.ExcluídoIVAPredeterminadoProducto; // Se usa el valor predeterminado de la propiedad base (ProductoExcluídoIVAPredeterminado) porque se requiere que ExcluídoIVAEspecífico pueda ser nulo para los casos de productos con productos base en los que se usa el valor del producto base.
            set { if (!UsarPropiedadesBase(escritura: true)) { ExcluídoIVAEspecífico = value; } }
        }

        public double? PorcentajeImpuestoConsumoPropioEspecífico { get; set; }
        /// <summary>
        /// Si es nulo se usa el porcentaje en opciones. Si es cero es exento de cualquier tipo de impuesto al consumo porcentual.
        /// </summary>
        [NotMapped]
        public double? PorcentajeImpuestoConsumoPropio {
            get => (UsarPropiedadesBase() && PorcentajeImpuestoConsumoPropioEspecífico == null) 
                ? Base!.PorcentajeImpuestoConsumoPropio : PorcentajeImpuestoConsumoPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { PorcentajeImpuestoConsumoPropioEspecífico = value; } }
        }

        public decimal? ImpuestoConsumoUnitarioPropioEspecífico { get; set; }
        /// <summary>
        /// Si es nulo se usa el el valor en opciones. Si es cero es exento de cualquier tipo de impuesto al consumo por unidad.
        /// </summary>
        [NotMapped]
        public decimal? ImpuestoConsumoUnitarioPropio {
            get => (UsarPropiedadesBase() && ImpuestoConsumoUnitarioPropioEspecífico == null) 
                ? Base!.ImpuestoConsumoUnitarioPropio : ImpuestoConsumoUnitarioPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { ImpuestoConsumoUnitarioPropioEspecífico = value; } }
        }

        public TipoImpuestoConsumo TipoImpuestoConsumoPropioEspecífico { get; set; } = TipoImpuestoConsumo.Desconocido;
        /// <summary>
        /// Si es desconocido se usa el tipo en opciones. Se usa para relacionarlo con TipoTributo que será enviado a la DIAN en la 
        /// factura electrónica. También se usa para establecer el valor de ImpuestoConsumoUnitario y PorcentajeImpuestoConsumo si el tipo
        /// se encuentra en los diccionarios en opciones <see cref="Singleton.OpcionesGenerales.PorcentajesImpuestosConsumo"/> o 
        /// <see cref="Singleton.OpcionesGenerales.ValoresUnitariosImpuestosConsumo" />.
        /// Posibles valores: Desconocido = 0, General = 1 (Valor general que no tiene asociado una tasa automáticamente, se debe especificar por producto), 
        /// BolsasPlásticas = 2, Carbono = 3, Combustibles = 4, DepartamentalNominal = 5, 
        /// DepartamentalPorcentual = 6, SobretasaCombustibles = 7, TelefoníaCelularYDatos = 8, Otro = 255.
        /// </summary>
        [NotMapped]
        public TipoImpuestoConsumo TipoImpuestoConsumoPropio {
            get => (UsarPropiedadesBase() && TipoImpuestoConsumoPropioEspecífico == TipoImpuestoConsumo.Desconocido) 
                ? Base!.TipoImpuestoConsumoPropio : TipoImpuestoConsumoPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { TipoImpuestoConsumoPropioEspecífico = value; } }
        }

        public ConceptoRetención ConceptoRetenciónPropioEspecífico { get; set; } = ConceptoRetención.Desconocido;
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
        [NotMapped] 
        public ConceptoRetención ConceptoRetenciónPropio {
            get => (UsarPropiedadesBase() && ConceptoRetenciónPropioEspecífico == ConceptoRetención.Desconocido) 
                ? Base!.ConceptoRetenciónPropio : ConceptoRetenciónPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { ConceptoRetenciónPropioEspecífico = value; } }
        }

        public Prioridad PrioridadWebPropiaEspecífica { get; set; } = Prioridad.Desconocida;
        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Si es desconocida se usan las reglas en opciones.
        /// Para implementaciones personalizadas. Establece la prioridad de los productos que se sincronizan con el sitio web. 
        /// Si la prioridad es Ninguna no se sincroniza con el sitio web.
        /// </summary>
        [NotMapped]
        public Prioridad PrioridadWebPropia {
            get => (UsarPropiedadesBase() && PrioridadWebPropiaEspecífica == Prioridad.Desconocida) 
                ? Base!.PrioridadWebPropia : PrioridadWebPropiaEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { PrioridadWebPropiaEspecífica = value; } }
        }


        [ForeignKey("ProveedorPreferidoEspecíficoID")]
        public Proveedor? ProveedorPreferidoEspecífico { get; set; }
        /// <summary>
        /// El proveedor, que independiente de cualquier otra regla, será el que se use en la programación de sus pedidos.
        /// </summary>
        [NotMapped]
        public Proveedor? ProveedorPreferido {
            get => (UsarPropiedadesBase() && ProveedorPreferidoEspecífico == null) ? Base!.ProveedorPreferido : ProveedorPreferidoEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { ProveedorPreferidoEspecífico = value; } }
        }
        public int? ProveedorPreferidoEspecíficoID { get; set; }
        [NotMapped]
        public int? ProveedorPreferidoID {
            get => (UsarPropiedadesBase() && ProveedorPreferidoEspecíficoID == null) ? Base!.ProveedorPreferidoID : ProveedorPreferidoEspecíficoID;
            set { if (!UsarPropiedadesBase(escritura: true)) { ProveedorPreferidoEspecíficoID = value; } }
        }

        public double? PorcentajeAdicionalGananciaPropioEspecífico { get; set; }
        /// <summary>
        /// Si es nulo se usan las reglas en opciones. El porcentaje de ganancia que se le sumará al porcentaje de ganancia del cliente para obtener 
        /// el porcentaje de ganancia total. El porcentaje de ganancia total se le aplica a los costos de los productos para obtener sus precios 
        /// de venta. Es útil cuando no se quieren usar las listas de precios o cuando el producto no está en ellas.
        /// </summary>
        [NotMapped]
        public double? PorcentajeAdicionalGananciaPropio {
            get => (UsarPropiedadesBase() && PorcentajeAdicionalGananciaPropioEspecífico == null) 
                ? Base!.PorcentajeAdicionalGananciaPropio : PorcentajeAdicionalGananciaPropioEspecífico;
            set { if (!UsarPropiedadesBase(escritura: true)) { PorcentajeAdicionalGananciaPropioEspecífico = value; } }
        }

        /// <MaxLength>200</MaxLength>
        [MaxLength(200)]
        public string? DescripciónEspecífica { get; set; }
        /// <summary>
        /// Esta propiedad devuelve la descripción de productos sin producto base o la descripción del producto base de productos con producto 
        /// base. Por ejemplo, si el valor de Producto.Base.Descripción es "Camiseta 3 Botones", este será el valor devuelto por esta función
        /// para todos los productos específicos derivados de ese producto base.
        /// </summary>
        [NotMapped]
        public string? DescripciónBase { // No usa el nombre plano Descripción (que se usa para la propiedad autocalculada Descripción) porque la descripción del producto y la descripción del producto base son diferentes. Los atributos son agregados al final de la descripción del producto base para formar la descripción autocalculada del producto.
            get => (UsarPropiedadesBase() && DescripciónEspecífica == null) ? Base!.Descripción : DescripciónEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { DescripciónEspecífica = value; } }
        }

        /// <MaxLength>2000</MaxLength>
        [MaxLength(2000)]
        public List<string> CaracterísticasEspecíficas { get; set; } = new List<string>(); // Aunque lo más correcto sería permitir que esta variable acepte valores nulos para diferenciar entre establecer su valor en una lista vacía y entre no establecerlo, implementarlo es un poco complicado porque implica hacer nuevos códigos que permitan nulos para ConvertidorJSON y funciones relacionadas, y considerando que la recomendación más común al usar el Entity Framework es no permitir que las listas sean admitan nulos, se prefiere establecer el valor predeterminado en la lista vacía. El único inconveniente de esta decisión es que no se podrían 'borrar' las características del producto base estableciendo una lista de características vacías en las características específicas, pero esto no parece una mayor limitación pues no se considera que sea un caso de uso común.
        /// <summary>
        /// Lista con las características principales del producto que serán usadas en el catálogo autogenerado, en las fichas técnicas autogeneradas 
        /// y en el sitio web. Estas características normalmente son especificaciones adicionales que no se encuentran en la descripción ni en los 
        /// atributos, pero podrían mencionarlos para ampliarlos un poco. No deben ser textos muy largos, deben ser poder ser leídos en unos pocos 
        /// segundos y obtener una idea más completa que la que provee la descripción sobre el producto. No confundir con <see cref="Atributos"/>,
        /// las características pueden ser compartidas por todos los productos que compartan el mismo producto base, mientras los atributos son
        /// los que los diferencian entre si.
        /// </summary>
        [NotMapped]
        public List<string> Características {
            get => (UsarPropiedadesBase() && CaracterísticasEspecíficas.Count == 0) ? Base!.Características : CaracterísticasEspecíficas;
            set { if (!UsarPropiedadesBase(escritura: true)) { CaracterísticasEspecíficas = value; } }
        }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? ArchivoImagenEspecífica { get; set; }
        /// <summary>
        /// Nombre del archivo de la imagen del producto. Si no se especifica, el nombre del archivo será la referencia.
        /// La extensión del nombre del archivo de imagen se puede usar u omitir en el valor de esta propiedad. 
        /// </summary>
        [NotMapped]
        public string? ArchivoImagen {
            get => (UsarPropiedadesBase() && ArchivoImagenEspecífica == null) ? Base!.ArchivoImagen : ArchivoImagenEspecífica; 
            set { if (!UsarPropiedadesBase(escritura: true)) { ArchivoImagenEspecífica = value; } }
        }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? ArchivoInformaciónEspecífica { get; set; }
        /// <summary>
        /// Nombre del archivo con la información completa del producto. Si no se especifica, el nombre del archivo será la referencia.
        /// El contenido del archivo puede ser un texto plano o HTML y su extensión puede ser .txt, .html o .htm. 
        /// La extensión se puede usar u omitir en el valor de esta propiedad, SimpleOps buscará el que coincida. 
        /// </summary>
        [NotMapped]
        public string? ArchivoInformación {
            get => (UsarPropiedadesBase() && ArchivoInformaciónEspecífica == null) ? Base!.ArchivoInformación : ArchivoInformaciónEspecífica;
            set { if (!UsarPropiedadesBase(escritura: true)) { ArchivoInformaciónEspecífica = value; } }
        }

        #endregion Propiedades de Producto Base>


        #region Constructores

        private Producto() { } // Solo para que EF Core no saque error.

        public Producto(string referencia) => (Referencia, TieneBase) = (referencia, false);

        /// <summary>
        /// Permite la creación de un producto usando como base un producto base existente con el que comparte los valores de las propiedades
        /// en este producto base con otros productos. En los atributos se pasan los elementos que diferencian al producto que se está creando de los 
        /// otros productos que comparten el mismo producto base. Por ejemplo, el producto base puede tener descripción "Camiseta Manga Corta" que es compartida
        /// por todos los productos que lo tienen de base y el producto que se está creando puede ser "Camiseta Manga Corta Roja Talla M", por lo tanto se 
        /// pasaría en los atributos una lista con AtributoProducto.TallaM y AtributoProducto.Roja.
        /// </summary>
        /// <param name="referencia"></param>
        /// <param name="base">Producto base que comparte propiedades con el producto a crear y otros.</param>
        /// <param name="atributos">Lista de atributos que diferencian el producto a crear de los otros que comparten el mismo producto base.</param>
        public Producto(string referencia, ProductoBase @base, List<string> atributos) { 

            (Referencia, Base, BaseID, TieneBase) = (referencia, @base, @base.ID, true);
            atributos.ForEach(a => AgregarAtributo(a)); // Se hace individualmente para evitar agregar atributos repetidos.

        } // Producto>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Si el producto tiene atributos, la descripción del producto se obtiene automáticamente con la descripción del producto base 
        /// más los atributos separados por espacio. Es posible agregar atributos a un producto que no tenga producto base, pero
        /// no es lo usual. Si deseas modificar la descripción asigna el valor de <see cref="DescripciónBase"/> y/o de <see cref="Atributos"/>.
        /// </summary>
        public string? Descripción => DescripciónEspecífica ?? (Atributos.Count == 0 ? DescripciónBase 
            : (string.IsNullOrEmpty(DescripciónBase) ? Atributos.ATextoConEspacios() : $"{DescripciónBase} {Atributos.ATextoConEspacios()}"));

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
       
        public bool ExentoIVA => ExcluídoIVA && PorcentajeIVA == 0;

        public TipoImpuestoConsumo TipoImpuestoConsumo => TipoImpuestoConsumoPropio == TipoImpuestoConsumo.Desconocido 
            ? Empresa.TipoImpuestoConsumoPredeterminado : TipoImpuestoConsumoPropio; 

        public TipoTributo TipoTributoConsumo => ObtenerTipoTributo(TipoImpuestoConsumo);

        public double PorcentajeImpuestoConsumo => Generales.PorcentajesImpuestosConsumo.ObtenerValor(TipoImpuestoConsumo) 
            ?? PorcentajeImpuestoConsumoPropio ?? Empresa.PorcentajeImpuestoConsumoPredeterminado;

        public decimal ImpuestoConsumoUnitario => Generales.ValoresUnitariosImpuestosConsumo.ObtenerValor(TipoImpuestoConsumo) 
            ?? ImpuestoConsumoUnitarioPropio ?? Empresa.ImpuestoConsumoUnitarioPredeterminado;

        public ModoImpuesto ModoImpuestoConsumo => PorcentajeImpuestoConsumo > 0 ? ModoImpuesto.Porcentaje
            : (ImpuestoConsumoUnitario > 0 ? ModoImpuesto.Unitario : ModoImpuesto.Exento);

        public Prioridad PrioridadWeb => PrioridadWebPropia != Prioridad.Desconocida ? PrioridadWebPropia : ObtenerPrioridadWebProducto();

        public ConceptoRetención ConceptoRetención => ConceptoRetenciónPropio != ConceptoRetención.Desconocido 
            ? ConceptoRetenciónPropio : Empresa.ConceptoRetenciónPredeterminado;

        public double PorcentajeAdicionalGanancia => PorcentajeAdicionalGananciaPropio ?? ObtenerPorcentajeGananciaAdicionalProducto();


        #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
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
            };
        #pragma warning restore CS8524


        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string UnidadTexto => Unidad.ToString();

        public string UnidadEspecíficaTexto => UnidadEspecífica.ToString();

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones Estáticas

        public static int ObtenerID(Producto? producto) => producto?.ID ?? 0; // El 0 es un índice inválido se usará solo en casos donde se necesita crear esta entidad sin conexión, como en migraciones, carga de CSVs y otros. 

        #endregion Métodos y Funciones Estáticas>


        #region Métodos y Funciones

        public override string ToString() => Referencia;

        public string? ObtenerRutaImagen() => ObtenerRutaArchivo(ArchivoImagen, ObtenerRutaImágenesProductos(), ExtensionesImágenes); // Se maneja como función porque internamente tiene un procedimiento que podría ser costoso en tiempo de cálculo que es la verificación de existencia de los posibles archivos de imágenes con las diferentes extensiones posibles.

        public string? ObtenerRutaInformación() => ObtenerRutaArchivo(ArchivoInformación, ObtenerRutaInformaciónProductos(), ExtensionesHtmlYTextoPlano);

        public string? ObtenerRutaInformaciónCompiladaHtml() => ObtenerRutaInformaciónCompiladaHtml(ObtenerRutaInformación());

        private string? ObtenerRutaArchivo(string? nombreArchivo, string rutaCarpeta, string[] extensionesVálidas)
            => ObtenerRutaArchivo(nombreArchivo, rutaCarpeta, extensionesVálidas, Referencia, UsarPropiedadesBase(), Base?.Referencia);

        public string ObtenerImagenBase64(int? tamaño) => ObtenerImagenBase64(tamaño, ObtenerRutaImagen());


        public string? ObtenerInformaciónHtml(bool codificarImágenes, string? rutaCarpetaImágenes = null, string? rutaCarpetaFragmentos = null)
            => ObtenerInformaciónHtml(codificarImágenes, ObtenerRutaInformaciónCompiladaHtml(), ObtenerRutaInformación(), rutaCarpetaImágenes, 
                    rutaCarpetaFragmentos);


        /// <summary>
        /// Devuelve verdadero para usar el producto base para obtener valor de la propiedad si la propiedad específica no es nula
        /// y falso para obtenerlo siempre de la propiedad específica. 
        /// Se debe pasar el parámetro <paramref name="escritura"/> en verdadero cuando se quiera hacer una escritura del valor
        /// para controlar el caso no deseado en el que se esté intentando modificar el valor del producto base desde el producto.
        /// </summary>
        private bool UsarPropiedadesBase(bool escritura = false) {

            if (Base == null) {

                if (TieneBase) {
                    return OperacionesEspecialesDatos ? false : throw new Exception("No se esperaba que TieneBase fuera verdadero y Base nulo.");
                } else {
                    return false;
                }

            } else { // Base != null.

                if (TieneBase) {

                    if (!Empresa.HabilitarProductosBase) 
                        throw new Exception("No se esperaba que la base de datos contenga productos con productos base y que HabilitarProductosBase sea " +
                            "falso. Habilita los productos base o modifica la base de datos para que no contenga productos base.");

                    return (!escritura) ? true : 
                        throw new Exception("No se permite modificar el valor de una propiedad del producto base desde el producto específico. " +
                            "Esto es para evitar que se pueda cambiar por error el valor de una propiedad del producto base (aplicable para todos los " +
                            "productos que compartan ese producto base) desde un producto específico. Si quieres modificar el valor de la propiedad en " +
                            "el producto base, usa Producto.Base.Propiedad = NuevoValorPropiedad. Si quieres modificar el valor de la propiedad para el " +
                            "producto específico, usa Producto.PropiedadEspecífica = NuevoValorPropiedad."); // Esta excepción es necesaria para evitar que el usuario del código escriba código que no tenga en cuenta la posibilidad de que un producto tenga producto base y cambie por error la propiedad en el producto base al estar asignando el valor de esta en el producto específico. Se hace con excepción y no con private set en la propiedad de enlace porque el set público se necesita para los casos en los que no se usen productos base. En estos casos la propiedad es correctamente asignada a la propiedad específica del producto. Esto permite que se realice una implementación personalizada simple y transparente para una empresa que no use productos base sin introducir lógica innecesaria en el código para verificar la existencia o no de producto base para cada producto. Para el caso de empresas que si usen producto base, esta excepción obliga a escribir código que sea claro en su intención de modificar la propiedad del producto base o la propiedad del producto específico.

                } else {
                    return OperacionesEspecialesDatos ? false : throw new Exception("No se esperaba que TieneBase fuera falso y Base no nulo.");
                }

            }

        } // UsarPropiedadesBase> 


        public DatosProducto ObtenerDatosProducto(decimal? precio, PlantillaDocumento plantillaDocumento, bool leerInformaciónHtml = false, 
            bool codificarImagen = false, int? tamañoImagenCodificada = null, bool codificarImágenesEnInformaciónHtml = true) {

            var rutaImagen = ObtenerRutaImagen();
            tamañoImagenCodificada ??= ObtenerTamañoPredeterminadoImágenesProductos(plantillaDocumento);
            var imagenBase64 = codificarImagen ? ObtenerImagenBase64(tamañoImagenCodificada) : "";
            var informaciónHtml = leerInformaciónHtml ? ObtenerInformaciónHtml(codificarImágenes: codificarImágenesEnInformaciónHtml) : "";

            return new DatosProducto(Referencia, esProductoBase: false) { Descripción = Descripción, DescripciónBase = DescripciónBase, 
                Precios = new Dictionary<List<string>, decimal?> { { new List<string> { "" }, precio } }, RutaImagen = rutaImagen, // No se agrega referencia en el diccionario de precios para evitar que esta sea agregada al catálogo. Este diccionario siempre lleva un solo elemento entonces esto no es problema.
                ImagenBase64 = imagenBase64, RutaInformaciónHtml = ObtenerRutaInformaciónCompiladaHtml(), InformaciónHtml = informaciónHtml,
                Atributos = Atributos, Características = Características, ReferenciaImagenBase64 = Referencia };

        } // ObtenerDatosProducto>


        #endregion Métodos y Funciones>


        #region Métodos y Funciones Estáticas
        // Principalmente compartidas con ProductoBase.


        public static string? ObtenerRutaArchivo(string? nombreArchivo, string rutaCarpeta, string[] extensionesVálidas, string? referencia,
            bool usarPropiedadesBase, string? referenciaBase = null) { // Función estática compartida con ProductoBase.

            var rutaArchivo = General.ObtenerRutaArchivo(nombreArchivo, rutaCarpeta, extensionesVálidas);
            if (rutaArchivo != null) return rutaArchivo;

            rutaArchivo = General.ObtenerRutaArchivo(referencia, rutaCarpeta, extensionesVálidas); // Da prioridad al archivo que tenga la referencia del producto específico. Por ejemplo, si existen RefBase.html, Ref.html y Ref.txt, se le da prioridad a Ref.html porque se prioriza el más específico y el HTML.
            if (rutaArchivo != null) return rutaArchivo;

            if (usarPropiedadesBase) rutaArchivo = General.ObtenerRutaArchivo(referenciaBase, rutaCarpeta, extensionesVálidas);

            return rutaArchivo;

        } // ObtenerRutaArchivo>


        public static string ObtenerImagenBase64(int? tamaño, string? rutaImagenProducto) 
            => ObtenerBase64(RedimensionarImagen(rutaImagenProducto, tamaño), paraHtml: true, 
                   rutaImagenNoDisponible: RedimensionarImagen(ObtenerRutaImagenProductoNoDisponible(), tamaño));


        public static string? RedimensionarImagen(string? rutaImagenProducto, int? tamaño) {

            var rutasImágenes = ObtenerRutaImágenesProductos();
            string? rutaImagen;

            if (tamaño != null && rutaImagenProducto != null) { 

                var rutaImagenRedimensionada = Path.Combine(ObtenerRutaCarpeta(rutasImágenes, ((int)tamaño).ATexto(),
                    crearSiNoExiste: true), Path.GetFileName(rutaImagenProducto));
                if (General.RedimensionarImagen(rutaImagenProducto, rutaImagenRedimensionada, (int)tamaño, (int)tamaño)) {
                    rutaImagen = rutaImagenRedimensionada;
                } else {
                    MostrarError($"No se pudo redimensionar la imagen {rutaImagenProducto} al tamaño {tamaño}.");
                    rutaImagen = rutaImagenProducto; // Si no se pudo redimensionar permite que la ejecución continúe con la ruta de la imagen original.
                }

            } else {
                rutaImagen = null; // Si no se proporciona tamaño o si es nula la devuelve nula;
            }

            return rutaImagen;

        } // RedimensionarImagen>


        public static string? ObtenerRutaInformaciónCompiladaHtml(string? rutaInformación) => rutaInformación == null ? null : // El nombre de la información compilada tendrá el mismo nombre del archivo de información.
            Path.Combine(ObtenerRutaInformaciónCompiladosProductos(), $"{Path.GetFileNameWithoutExtension(rutaInformación)}{".html"}");


        /// <summary>
        /// Lee el archivo de información (plano o HTML) y devuelve un texto HTML. Si se dejan <paramref name="rutaCarpetaFragmentos"/> y 
        /// <paramref name="rutaCarpetaImágenes"/> en nulo, se usarán las carpetas predeterminadas. Si <paramref name="forzarCompilación"/> es falso,
        /// solo se compilará la información HTML
        /// </summary>
        /// <returns></returns>
        public static string? ObtenerInformaciónHtml(bool codificarImágenes, string? rutaCompilado, string? rutaArchivoInformación, 
            string? rutaCarpetaImágenes = null, string? rutaCarpetaFragmentos = null, bool forzarCompilación = false) {

            var rutaCarpetaFragmentosAplicable = rutaCarpetaFragmentos ?? ObtenerRutaInformaciónFragmentosProductos();
            var rutaCarpetaImágenesAplicable = rutaCarpetaImágenes ?? ObtenerRutaInformaciónImágenesProductos();

            var últimaFechaModificaciónFragmentos = ObtenerÚltimaFechaModificaciónArchivos(rutaCarpetaFragmentosAplicable); // Cada vez que se cambie un fragmento se tomará su fecha de modificación como la fecha de modificación efectiva de la información de este producto así sea posible que este fragmento no se use en el archivo de información de este producto. Se hace de esta manera porque por el momento no se navega el archivo de información para obtener la lista de todos sus fragmentos asociados, esta navegación solo se hace en la construcción del compilado en si. Si se considerara la exploración previa de los fragmentos asociados a un archivo de información para poder devolver la fecha de última modificación de los fragmentos asociados a este archivo, habría que hacer una evaluación cuidadosa de la mejora en rendimiento que se podría dar.
            var fechaModificaciónArchivoInformación = ObtenerFechaModificaciónUtc(rutaArchivoInformación);
            if (fechaModificaciónArchivoInformación == null) return null; // Si la fecha de modificación es nula, el archivo de información no existe.

            var últimaFechaModificaciónArchivosOrigen = últimaFechaModificaciónFragmentos == null ? fechaModificaciónArchivoInformación : 
                (últimaFechaModificaciónFragmentos > fechaModificaciónArchivoInformación ? últimaFechaModificaciónFragmentos : 
                fechaModificaciónArchivoInformación);
            var fechaModificaciónArchivoCompilado = ObtenerFechaModificaciónUtc(rutaCompilado);
            var compilar = forzarCompilación || (fechaModificaciónArchivoCompilado == null) // Si la fecha de modificación es nula, el archivo compilado no existe.
                || (últimaFechaModificaciónArchivosOrigen > fechaModificaciónArchivoCompilado); 

            string? informaciónHtml;
            if (compilar) {

                informaciónHtml = ConvertirAHtml(rutaArchivoInformación, rutaCarpetaImágenesAplicable, rutaCarpetaFragmentosAplicable, codificarImágenes);
                File.WriteAllText(rutaCompilado, informaciónHtml);

            } else {
                informaciónHtml = File.ReadAllText(rutaCompilado);
            }
            return informaciónHtml;

        } // ObtenerInformaciónHtml>


        #endregion Métodos y Funciones Estáticas>


        #region Métodos y Funciones de Atributos


        public bool EliminarAtributo(string atributo) {

            var índice = Atributos.ObtenerÍndice(atributo); // Se hace con la función ObtenerÍndice porque esta función es particular para textos y permite ignorar la capitalización, lo que no se podría hacer en una función genérica.
            if (índice != -1) {
                Atributos.RemoveAt(índice);
                return true;
            } else {
                return false;
            }

        } // EliminarAtributo>


        public static int ObtenerIDAtributo(string atributo) {

            foreach (var kv in ÍndicesYAtributos) {
                if (kv.Value.IgualA(atributo)) return kv.Key;
            }
            return -1;

        } // ObtenerIDAtributo>


        public void EliminarAtributos(List<string> atributos) => atributos.ForEach(a => EliminarAtributo(a));


        public void EliminarAtributos(string tipoAtributo) => EliminarAtributos(ObtenerAtributosNoRepetidos(tipoAtributo));


        public bool TieneAtributo(string atributo) => Atributos.Existe(atributo); // Existe permite ignorar la capitalización.


        public static string ObtenerTipoAtributo(string atributo) => AtributosProductosYTipos.ObtenerValorObjeto(atributo) ?? TipoAtributoProductoLibre; 


        /// <summary>
        /// Obtiene los atributos sin repetir. Si se puede asegurar que los atributos no están repetidos, su resultado es igual al valor de 
        /// la propiedad Atributos.
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerAtributosNoRepetidos(string? tipoAtributo = null) {

            var atributos = new List<string>();
            foreach (var atributo in Atributos) {

                var agregar = false;
                if (tipoAtributo == null) {
                    agregar = true;
                } else {
                    if (ObtenerTipoAtributo(atributo) == tipoAtributo) agregar = true;
                }
                if (agregar) atributos.Agregar(atributo, permitirRepetidos: false);

            }
            return atributos;

        } // ObtenerAtributosNoRepetidos>


        /// <summary>
        /// Agrega un atributo a la lista de atributos, sin repetirlo y usando la capitalización correcta. Permite la no adición
        /// del atributo si no está en la tabla AtributosProductos al pasar <paramref name="permitirAtributosLibres"/> en falso. Devuelve
        /// verdadero si el atributo fue agregado y falso si no se agregó.
        /// </summary>
        /// <returns></returns>
        public bool AgregarAtributo(string atributo, bool? permitirAtributosLibres = null) {

            var permitirAtributosLibresAplicable = permitirAtributosLibres ?? Empresa.PermitirAtributosProductosLibres;
            var atributoCapitalizaciónCorrecta = AtributosProductosYTipos.ObtenerClaveCapitalizaciónCorrecta(atributo);
            if (atributoCapitalizaciónCorrecta == null && !permitirAtributosLibresAplicable) return false;
            var cuentaAnteriorAtributos = Atributos.Count;
            Atributos.Agregar(atributoCapitalizaciónCorrecta ?? atributo, permitirRepetidos: false); // Tiene en cuenta la capitalización y no lo agrega si está repetido.
            return Atributos.Count > cuentaAnteriorAtributos; // Si es mayor, se agregó nuevo atributo. Se hace de esta manera por rendimiento para usar un solo procedimiento Agregar y no dos: Existe y después Agregar.

        } // AgregarAtributo>


        /// <summary>
        /// Devuelve un diccionario clasificando por su tipo los atributos asignados al producto.
        /// Si se puede asegurar que los atributos no están repetidos, o si están repetidos y no importa para la necesidad actual, al establecer
        /// permitirRepetidos en verdadero se obtiene un mejor rendimiento.
        /// </summary>
        /// <param name="permitirRepetidos"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ClasificarAtributos(bool permitirRepetidos = true) { // No cambiar a permitirRepetidos = false. Leer descripción función.

            var clasificación = new Dictionary<string, List<string>>();
            foreach (var atributo in Atributos) {
                clasificación.Agregar(ObtenerTipoAtributo(atributo), atributo, permitirRepetidos); 
            }
            return clasificación;

        } // ClasificarAtributos>


        public List<string> ObtenerTiposAtributos() {

            var tipos = new List<string>();
            foreach (var atributo in Atributos) {
                tipos.Agregar(ObtenerTipoAtributo(atributo), permitirRepetidos: false);
            }
            return tipos;

        } // ObtenerTiposAtributos>


        #endregion Métodos y Funciones de Atributos>


    } // Producto>



} //  SimpleOps.Modelo>
