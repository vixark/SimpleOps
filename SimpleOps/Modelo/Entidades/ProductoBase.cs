using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using SimpleOps.DocumentosGráficos;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;
using System.Linq;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Varias entidades de tipo <see cref="Producto"/> pueden contener características en común cómo 
    /// la marca, el material, especificaciones técnicas, etc. A estos productos se les asigna un <see cref="ProductoBase"/> común y 
    /// estas características se manejan centralizadas en este producto base. Esto evita errores de escritura y la repetición de la información
    /// en la tabla Productos. Los atributos en los que los productos que comparten el mismo producto base difieran
    /// se manejan como una o varias columnas adicionales en la tabla producto (ver comentario en clase Producto).
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ProductoBase : Rastreable { // No se usa la herencia tradicional porque se permitirá que los productos no necesariamente tengan producto base para dar flexibilidad al usuario si quiere usar o no la tabla productos base. Es Rastreable porque los productos base son actualizados frecuentemente y es de interés tener la información de su creación.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string Referencia { get; set; } = null!; // Obligatoria y única. Es obligatoria porque aunque en términos generales la referencia del producto (no base) es la importante para manejo de inventario, tener una referencia por producto base es muy recomendable porque permite realizar estadísticas agrupando todos los productos de este producto base y se requiere para nombrar las imágenes y los archivos de información del producto base.

        /// <summary>
        /// Referencias de otros productos base asociados, similares o recomendados alternativos. Permite varias funciones en implementaciones 
        /// personalizadas. Es una propiedad equivalente a Producto.ProductosAsociados, pero a nivel de productos base.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public List<string> ProductosBaseAsociados { get; set; } = new List<string>(); // No se combinan ProductosAsociados en ProductosBase ni ProductosBaseAsociados en Productos porque la lógica resultante sería complicada de manejar. Se considera que agregando esta propiedad de manera independiente a las tablas ProductosBase y a Productos se pueden cubrir todos los casos de uso. No se crea otra tabla para manejar estos valores porque es una función particular.

        #endregion Propiedades>


        #region Propiedades en Producto
        // Estas propiedades se encuentran duplicadas en Producto para dar la flexibilidad de usar la tabla Productos sin necesidad de la tabla ProductosBase y poder usarla simultáneamente para unos productos y para otros no. Cualquier propiedad que se agregue aquí también se deberá agregar a la tabla Producto con el nombre [Propiedad]Específico(a). Toda propiedad tenga un valor inicial diferente de null/desconocido debe implementar el patrón de valor de la propiedad predeterminado para permitir que las propiedades de productos que no tengan producto base la puedan usar cuando no se les haya establecido el valor de la propiedad específica. Para el desarrollo general de la aplicación solo se deben agregar aquí nuevas propiedades que puedan ser ampliamente usadas por cualquier tipo de producto. Por ejemplo, Material puede ser solo útil para algunos productos, pero podría ser aplicable a todos los productos posibles. Una propiedad posible que no se ha agregado por el momento sería Color,y una propiedad no posible sería Talla porque solo le aplica a un tipo de productos muy particular (ropa) y no tendría sentido ampliar la cantidad de columnas de la base de datos para todos los usuarios de la aplicación para acomodarla. Aún así, en una adaptación personalizada del código a cierto tipo de empresa la propiedad Talla si podría ser agregada, pero no en el código base general. Para manejar propiedades como Talla en el desarrollo general se usa la propiedad Atributos en Producto. Una propiedad se agrega aquí y en Producto cuando normalmente el valor de esta propiedad es aplicable para todos lo productos que comparten el mismo producto base y se quiere tener la opción de personalizarla para cada producto si se requiere. Por ejemplo, PesoUnidadEmpaque podría ser igual para todos los productos que compartan un producto base, pero también podría ser diferente. Una propiedad se agrega solo en la tabla Producto si esta propiedad permite la diferenciación entre productos con el mismo producto base, por ejemplo una columna Talla (en una adaptación del código a una empresa que lo requiera) que podría tener como tipo una enumeración de tallas posibles, si se añade una nueva columna de este tipo a la tabla productos se debe considerar si es necesario agregar el valor al cálculo de Producto.Descripción. Se deben copiar y pegar los comentarios XML para mantenerlos iguales en ProductoBase y Producto.

        /// <MaxLength>200</MaxLength>
        [MaxLength(200)]
        public string? Descripción { get; set; }

        /// <summary>
        /// La unidad de venta del producto. Se escribe en campo Unidad de la factura. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        public Unidad Unidad { get; set; } = Empresa.UnidadPredeterminadaProducto;

        /// <summary>
        /// Permite ajustar los pedidos a los proveedores para que se hagan en unidades de empaque. Desconocida = 0, Unidad = 1, Par = 2, Trío = 3, Cuarteto = 4, Quinteto = 5, MediaDocena = 6, Decena = 10,
        /// Docena = 12, DocenaLarga = 13, Quincena = 15, Veintena = 20, DobleDocena = 24, CuartoDeCentena = 25, Treintena = 30, Cuarentena = 40,
        /// CuatroDocenas = 48, MediaCentena = 50, OchoDecenas = 80, OchoDocenas = 96, DiezDocenas = 120, Centena = 100, Gruesa = 144,
        /// DobleCentena = 200, VeinteDocenas = 240, TripleCentena = 300, CuatroCentenas = 400, MedioMillar = 500, Millar = 1000, DobleMillar = 2000,
        /// TripleMillar = 3000, CuatroMillares = 4000, Miríada = 10000, Millón = 1000000, Millardo = 1000000000.
        /// </summary>
        public Unidad UnidadEmpaque { get; set; } = Empresa.UnidadEmpaquePredeterminadaProducto;

        /// <summary>
        /// En kg.
        /// </summary>
        public double? PesoUnidadEmpaque { get; set; }

        /// <summary>
        /// En m x m x m.
        /// </summary>
        public Dimensión? DimensiónUnidadEmpaque { get; set; }

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
        /// Si es verdadero el producto existe en el mundo físico. Si es falso no maneja inventario como en el caso de de los servicios y 
        /// productos virtuales.
        /// </summary>
        public bool Físico { get; set; } = Empresa.FísicoPredeterminadoProducto;

        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). Si es cero es exento de IVA.
        /// </summary>
        public double? PorcentajeIVAPropio { get; set; }

        /// <summary>
        /// Si es verdadero es excluído de IVA y tiene un tratamiento tributario diferente porque no suma a la base tributable. Si es falso y 
        /// PorcentajeIVAPropio = 0 (exentos) o PorcentajeIVAPropio > 0 si suma a la base tributable. Si se establece este valor en verdadero se le dará 
        /// prioridad sin importar el valor en PorcentajeIVAPropio y el porcentaje de IVA efectivo (PorcentajeIVA) será cero.
        /// </summary>
        public bool ExcluídoIVA { get; set; } = Empresa.ExcluídoIVAPredeterminadoProducto;

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

        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Si es desconocida se usan las reglas en opciones.
        /// Para implementaciones personalizadas. Establece la prioridad de los productos que se sincronizan con el sitio web. 
        /// Si la prioridad es Ninguna no se sincroniza con el sitio web.
        /// </summary>
        public Prioridad PrioridadWebPropia { get; set; } = Prioridad.Desconocida;

        /// <summary>
        /// El proveedor, que independiente de cualquier otra regla, será el que se use en la programación de sus pedidos.
        /// </summary>
        [ForeignKey("ProveedorPreferidoID")]
        public Proveedor? ProveedorPreferido { get; set; }
        public int? ProveedorPreferidoID { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas en opciones. El porcentaje de ganancia que se le sumará al porcentaje de ganancia del cliente para obtener 
        /// el porcentaje de ganancia total. El porcentaje de ganancia total se le aplica a los costos de los productos para obtener sus precios 
        /// de venta. Es útil cuando no se quieren usar las listas de precios o cuando el producto no está en ellas.
        /// </summary>
        public double? PorcentajeAdicionalGananciaPropio { get; set; }

        /// <summary>
        /// Lista con las características principales del producto que serán usadas en el catálogo autogenerado, en las fichas técnicas autogeneradas 
        /// y en el sitio web. Estas características normalmente son especificaciones adicionales que no se encuentran en la descripción ni en los 
        /// atributos, pero podrían mencionarlos para ampliarlos un poco. No deben ser textos muy largos, deben ser poder ser leídos en unos pocos 
        /// segundos y obtener una idea más completa que la que provee la descripción sobre el producto. No confundir con los atributos,
        /// las características pueden ser compartidas por todos los productos que compartan el mismo producto base, mientras los atributos son
        /// los que los diferencian entre si.
        /// </summary>
        /// <MaxLength>2000</MaxLength>
        [MaxLength(2000)]
        public List<string> Características { get; set; } = new List<string>();

        /// <summary>
        /// Nombre del archivo de la imagen del producto. Si no se especifica, el nombre del archivo será la referencia.
        /// La extensión del nombre del archivo de imagen se puede usar u omitir en el valor de esta propiedad. 
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? ArchivoImagen { get; set; }

        /// <summary>
        /// Nombre del archivo con la información completa del producto. Si no se especifica, el nombre del archivo será la referencia.
        /// El contenido del archivo puede ser un texto plano o HTML y su extensión puede ser .txt, .html o .htm. 
        /// La extensión se puede usar u omitir en el valor de esta propiedad, SimpleOps buscará el que coincida. 
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? ArchivoInformación { get; set; }

        #endregion Propiedades en Producto>


        #region Constructores

        private ProductoBase() { } // Solo para que Entity Framework no saque error.

        public ProductoBase(string referencia) => (Referencia) = (referencia);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public string UnidadTexto => Unidad.ToString();

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => Referencia;

        public string? ObtenerRutaImagen() => ObtenerRutaArchivo(ArchivoImagen, ObtenerRutaImágenesProductos(), ExtensionesImágenes); // Se maneja como función porque internamente tiene un procedimiento que podría ser costoso en tiempo de cálculo que es la verificación de existencia de los posibles archivos de imágenes con las diferentes extensiones posibles.

        public string? ObtenerRutaInformación() => ObtenerRutaArchivo(ArchivoInformación, ObtenerRutaInformaciónProductos(), ExtensionesHtmlYTextoPlano);

        public string? ObtenerRutaInformaciónCompiladaHtml() => Producto.ObtenerRutaInformaciónCompiladaHtml(ObtenerRutaInformación());

        private string? ObtenerRutaArchivo(string? nombreArchivo, string rutaCarpeta, string[] extensionesVálidas)
            => Producto.ObtenerRutaArchivo(nombreArchivo, rutaCarpeta, extensionesVálidas, Referencia, usarPropiedadesBase: false);

        public string ObtenerImagenBase64(int? tamaño) => Producto.ObtenerImagenBase64(tamaño, ObtenerRutaImagen());


        public string? ObtenerInformaciónHtml(bool codificarImágenes, string? rutaCarpetaImágenes = null, string? rutaCarpetaFragmentos = null)
                => Producto.ObtenerInformaciónHtml(codificarImágenes, ObtenerRutaInformaciónCompiladaHtml(), ObtenerRutaInformación(), 
                       rutaCarpetaImágenes, rutaCarpetaFragmentos);


        /// <summary>
        /// Devuelve un texto que enumera todos los atributos del <paramref name="tipoAtributo"/> en la lista de <paramref name="productos"/>.
        /// Si los atributos son de tipo secuencial, como las tallas, se devuelven textos con 'a' para denotar los rangos
        /// incluídos, por ejemplo: Talla 8 a 11. Si no son secuenciales, se devuelven los atributos del mismo tipo separados por coma y por 'y'.
        /// Si un producto tiene varios atributos del mismo tipo, estos se unen usando <paramref name="separadorAtributosIgualTipoEnProducto"/>, 
        /// por ejemplo, si un producto 2 valores para el tipo de atributo color: Rojo y Verde, se devolverá Rojo+Verde.
        /// </summary>
        /// <returns></returns>
        public static string? CompilarAtributos(List<Producto> productos, string tipoAtributo, string separadorAtributosIgualTipoEnProducto = "+") {

            if (Empresa.TiposAtributosSecuenciales.Existe(tipoAtributo)) {

                // 1. Crea el diccionario índicesYAtributos que contiene los índices de los atributos aplicables ordenados de menor a mayor.
                var índicesYAtributos = new SortedDictionary<int, string>();
                foreach (var producto in productos) {

                    var atributosProducto = producto.ObtenerAtributosNoRepetidos(tipoAtributo);
                    foreach (var atributo in atributosProducto) {
                        var índiceAtributo = Producto.ObtenerIDAtributo(atributo);
                        if (!índicesYAtributos.ContainsKey(índiceAtributo)) índicesYAtributos.Add(índiceAtributo, atributo);
                    }

                }

                // 2. Obtiene los rangos de los atributos consecutivos y 'rangos' individuales de los atributos no consecutivos.
                int? índiceAnterior = null;
                string atributoInicialActual = "";
                string atributoAnterior = "";
                var rangos = new Dictionary<string, string>();
                var largoRangos = new Dictionary<string, decimal>();
                
                foreach (var kv in índicesYAtributos) {

                    var índice = kv.Key;
                    var atributo = kv.Value;
                    var iniciarRango = false;

                    if (índiceAnterior == null) { // El primer atributo.
                        iniciarRango = true;
                    } else {
                 
                        if (tipoAtributo == Empresa.NombreTipoAtributoTallaNumérica) {
    
                            var estáEnRangoTallasMedias = false;
                            var permitirDoblePaso = false;
                            foreach (var rangoDoblePaso in ÍndicesRangosDoblePasoEnSecuenciaTallaNumérica) {
                                if (índice >= rangoDoblePaso.Key && índice <= rangoDoblePaso.Value) { // Por lo general no es necesario ser muy exacto con el límite del doble paso, si se encontraran casos límite que produzcan resultados incorrectos se puede implementar el ajuste con variaciónPaso como se implementó para las tallas medias.
                                    permitirDoblePaso = true;
                                    break;
                                }
                            }

                            var variaciónPaso = 0;
                            foreach (var rangoTallasMedias in ÍndicesRangosTallasMediasNuméricas) {
                                if (índice > rangoTallasMedias.Key && índice <= (rangoTallasMedias.Value + (permitirDoblePaso ? 2 : 1))) { // El final del rango de la talla media siempre es la media, entonces el rango se permitirá hasta la siguiente talla entera (+1) o hasta la próxima siguiente talla entera (+2). Al principio del rango no tiene en cuenta el valor inicial para considerarlo del rango de medidas medias porque antes de la primera talla media solo puede haber 1 talla (la entera inmediatamente anterior) o 2 tallas (las dos enteras inmediatamente anteriores), por esto en ambos casos se puede tratar esta talla media como una talla normal para efectos de establecimiento de la secuencia.
                                    
                                    estáEnRangoTallasMedias = true;
                                    if (permitirDoblePaso) {
                                        if (índice == rangoTallasMedias.Value + 2) variaciónPaso = -1; // Cuando está justo por fuera del límite de las tallas medias y se permiten dobles pasos, se resta un paso para quitar la talla media inexistente más allá del límite. Por ejemplo, si el rango de tallas medias termina en 14.5 y la talla actual es 16, se descuenta la inexistencia de la talla 15.5 para permitir que los máximos pasos sean 3 y que el primer atributo en secuencia con 16 sea 14 (a 3 pasos: 14.5, 15 y 16) y no 13.5 (a 4 pasos: 14, 14.5, 15 y 16).
                                        if (índice == rangoTallasMedias.Key + 1) variaciónPaso = -1; // Cuando está en una talla más adentro del límite de las tallas medias (en la talla entera inmediatamente superior),  se resta un paso para quitar la talla media inexistente una talla antes del límite. Por ejemplo, si el rango de las tallas medias inicia en 33.5 y el atributo actual es 34, se descuenta la inexistencia de la talla 32.5 para permitir que los máximos pasos sean 3 y que el primer atributo en secuencia con 34 sea 32 (a 3 pasos: 33, 33.5 y 34) y no 31 (a 4 pasos: 32, 33, 33.5 y 34).
                                    }
                                    break;

                                }
                            }

                            var máximoPaso = (estáEnRangoTallasMedias ? (permitirDoblePaso ? 4 : 2) : (permitirDoblePaso ? 2 : 1)) + variaciónPaso; // Suponiendo que el anterior es 10 puede pasar a 11 o a 10.5 y ambas se considerarán en secuencia. Suponiendo que está en 38 y se permite pasos dobles puede pasar a 38.5, 39, 39.5 o 40 y en cualquier caso se considerará en secuencia.
                            if (índice - índiceAnterior > máximoPaso) {
                                rangos[atributoInicialActual] = atributoAnterior; // Finaliza el rango con el atributo anterior.
                                iniciarRango = true;
                            } else {
                                largoRangos.AgregarSumando(atributoInicialActual, 1);
                            }
                       
                        } else {

                            if (índice > índiceAnterior + 1) { // Cuando encuentra un atributo que no es consecutivo con el anterior.
                                rangos[atributoInicialActual] = atributoAnterior; // Finaliza el rango con el atributo anterior.
                                iniciarRango = true;
                            } else { // Si índice == índiceAnterior + 1, significa que el atributo actual es consecutivo al anterior, por lo tanto se mantiene el mismo rango actual e iniciarRango debe mantenerse en falso.
                                largoRangos.AgregarSumando(atributoInicialActual, 1);
                            }

                        }

                    }

                    if (iniciarRango) {
                        rangos.Add(atributo, "*"); // Inicia un nuevo rango sin conocer aún el final.
                        largoRangos.Agregar(atributo, 1);
                        atributoInicialActual = atributo;
                    }

                    índiceAnterior = índice;
                    atributoAnterior = atributo;

                }
                rangos[atributoInicialActual] = atributoAnterior; // Cierra el último rango.

                // 3. Analiza los rangos y construye los atributos efectivos por rangos.
                var atributos = new List<string>();
                foreach (var kv in rangos) {

                    var atributoInicial = kv.Key;
                    var atributoFinal = kv.Value;
                    var largoRango = largoRangos[atributoInicial];
                    switch (largoRango) {
                        case 1:
                            atributos.Add(atributoInicial);
                            break;
                        case 2:
                            atributos.Add(atributoInicial);
                            atributos.Add(atributoFinal);
                            break;
                        default:

                            var resumenLíneas = ResumirLíneasTexto(new List<string> { atributoInicial, atributoFinal });
                            var palabrasComunes = resumenLíneas.Item1;
                            var listaResumida = resumenLíneas.Item2;
                            atributos.Add($"{(string.IsNullOrEmpty(palabrasComunes) ? "" : palabrasComunes + " ")}" +
                                $"{listaResumida[0]} a {listaResumida[1]}");
                            break;

                    }

                }

                return atributos.ATextoConComas(resumir: true);

            } else {

                var atributosYCuenta = new Dictionary<string, decimal>();
                foreach (var producto in productos) {

                    var atributos = producto.ObtenerAtributosNoRepetidos(tipoAtributo);
                    var textoAtributos = atributos.ATexto(separador: separadorAtributosIgualTipoEnProducto, conector: ConectorCoordinante.Ninguno);         
                    if (textoAtributos != null) atributosYCuenta.AgregarSumando(textoAtributos, 1);
                }

                var atributosOrdenados = atributosYCuenta.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value).Keys.ToList(); // Ordenados de más apariciones a menos.        
                return atributosOrdenados.ATextoConComas(resumir: true);

            }

        } // CompilarAtributos>


        /// <summary>
        /// De cada tipo de atributo en <paramref name="tiposAtributos"/> obtiene los atributos compilados de todos los <paramref name="productos"/>
        /// y devuelve un diccionario con una línea con los atributos compilados por cada tipo.
        /// </summary>
        /// <param name="productos"></param>
        /// <param name="tiposAtributos"></param>
        /// <returns></returns>
        public static List<string> CompilarAtributos(List<Producto> productos, List<string> tiposAtributos) {

            var atributosCompilados = new List<string>();
            foreach (var tipoAtributo in tiposAtributos) {
                var atributosCompiladosDelTipo = CompilarAtributos(productos, tipoAtributo);
                if (atributosCompiladosDelTipo != null) atributosCompilados.Add(atributosCompiladosDelTipo);
            }
            return atributosCompilados;

        } // CompilarAtributos>


        public DatosProducto ObtenerDatosProducto(List<(Producto, decimal)> productosYPrecios, PlantillaDocumento plantillaDocumento, 
            bool leerInformaciónHtml = false, bool codificarImagen = false, int? tamañoImagenCodificada = null, 
            bool codificarImágenesEnInformaciónHtml = true) { // productosYPrecios no permite nulos para el precio (decimal) porque este precio es requerido para construir un diccionario con los precios como clave y los diccionarios no aceptan valores nulos cómo clave. Si llegar a ser necesario implementarlo, se podría pasar un precio con un valor de -1 y al construir preciosEspecíficos cambiarlo por null. De todas maneras este procedimiento normalmente se usa desde un objeto Cotización, el cuál exige que todas sus líneas tengan un precio no nulo, entonces no se espera que sea un requerimiento que se pudiera presentar.

            var rutaImagen = ObtenerRutaImagen();
            tamañoImagenCodificada ??= ObtenerTamañoPredeterminadoImágenesProductos(plantillaDocumento);
            var imagenBase64 = codificarImagen ? ObtenerImagenBase64(tamañoImagenCodificada) : "";
            var informaciónHtml = leerInformaciónHtml ? ObtenerInformaciónHtml(codificarImágenes: codificarImágenesEnInformaciónHtml) : "";

            var atributosSinEfectoEnPrecio = new List<string>(); // En cada elemento se resumen los atributos de cada tipo de atributo que no tiene efecto en los precios.
            var preciosEspecíficos = new Dictionary<List<string>, decimal?>(); // Cada elemento es un precio y su clave es un texto de varias líneas donde cada línea representa un tipo de atributo y sus valores de atributos aplicables para ese precio.

            // 1. Se detecta cuáles tipos de atributos no tienen efecto en el precio. Eso se hace iterando por cada tipo de atributo y encontrando los grupos de productos que tengan iguales los otros atributos de los otros tipos. Si en ninguno de esos grupos, donde lo único que varía son los atributos del tipo de atributo en cuestión, hay variaciones de precios, se puede considerar que ese tipo de atributo no afecta el precio y todos los atributos de este tipo podrán ser agregados a atributosSinEfectoEnPrecio.
            var tiposAtributos = new List<string>();
            foreach (var pp in productosYPrecios) {
                foreach (var ta in pp.Item1.ObtenerTiposAtributos()) {
                    tiposAtributos.Agregar(ta, permitirRepetidos: false);
                }
            }

            var tiposAtributosSinEfectoEnPrecio = new List<string>();
            var tiposAtributosConEfectoEnPrecio = new List<string>();

            foreach (var tipoAtributo in tiposAtributos) {

                var atributosRestantesYPrecios = new Dictionary<string, decimal?>();
                var alMenosDosGruposDeAtributosRestantesDifierenEnPrecio = false;

                foreach (var pp in productosYPrecios) {

                    var producto = pp.Item1;
                    var precio = pp.Item2;
                    if (producto.Base == null) 
                        throw new Exception($"No se esperaba que el producto {producto} en productosYPrecios no tuviera base.");

                    var productoSinTipoAtributo = new Producto(producto.Referencia, producto.Base, producto.Atributos);
                    productoSinTipoAtributo.EliminarAtributos(tipoAtributo); // Queda el producto únicamente con los atributos de los tipos restantes.

                    var atributosRestantes = productoSinTipoAtributo.Atributos.ATextoConEspacios() ?? "";
                    if (atributosRestantesYPrecios.ContainsKey(atributosRestantes)) {

                        if (precio != atributosRestantesYPrecios[atributosRestantes]) {
                            alMenosDosGruposDeAtributosRestantesDifierenEnPrecio = true;
                            break; // Fin de la búsqueda, valores diferentes en este tipo de atributo si afecta el precio.
                        } else {
                            // No pasa nada, puede continuar la búsqueda.
                        }

                    } else {
                        atributosRestantesYPrecios.Add(atributosRestantes, precio);
                    }

                }

                if (alMenosDosGruposDeAtributosRestantesDifierenEnPrecio) {
                    tiposAtributosConEfectoEnPrecio.Add(tipoAtributo);
                } else {
                    tiposAtributosSinEfectoEnPrecio.Add(tipoAtributo);
                }

            }

            // 2. Se compilan los atributos sin efecto en los precios en atributosSinEfectoEnPrecio. 
            atributosSinEfectoEnPrecio = CompilarAtributos(productosYPrecios.Select(pp => pp.Item1).ToList(), tiposAtributosSinEfectoEnPrecio);

            // 3. Se agrupan los productos por precio.
            var preciosYProductos = productosYPrecios.GroupBy(pp => pp.Item2).ToDictionary(ig => ig.Key, ig => ig.Select(e => e.Item1).ToList()); // Genera un diccionario con clave cada precio y con valor una lista con todos los productos específicos que tienen ese precio.
            var preciosYProductosOrdenados = preciosYProductos.OrderByDescending(kv => kv.Value.Count).ThenBy(kv => kv.Key)
                .ToDictionary(kv => kv.Key, kv => kv.Value); // Ordena el diccionario preciosYProductos. Primero, por la cantidad de productos que tienen ese precio, primero los precios con más productos y después los precios con menos productos. Y segundo, por el valor del precio de menor a mayor.
            
            // 4. Se obtienen los atributos compilados de todos los productos de cada precio.
            foreach (var precioYProductos in preciosYProductosOrdenados) {
                var precio = precioYProductos.Key;
                var productos = precioYProductos.Value;
                preciosEspecíficos.Add(CompilarAtributos(productos, tiposAtributosConEfectoEnPrecio), precio);
            }

            return new DatosProducto(Referencia, esProductoBase: true) { Descripción = Descripción, DescripciónBase = Descripción, 
                Precios = preciosEspecíficos, RutaImagen = rutaImagen, ImagenBase64 = imagenBase64, 
                RutaInformaciónHtml = ObtenerRutaInformaciónCompiladaHtml(), InformaciónHtml = informaciónHtml, Atributos = atributosSinEfectoEnPrecio, 
                Características = Características, ReferenciaImagenBase64 = Referencia };

        } // ObtenerDatosProducto>


        #endregion Métodos y Funciones>


    } // ProductoBase>



} // SimpleOps.Modelo>
