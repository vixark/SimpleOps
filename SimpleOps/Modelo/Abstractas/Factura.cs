using SimpleOps.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using static SimpleOps.Legal.Dian;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Documento que produce un cambio de propiedad de un producto entre empresa y cliente o entre proveedor y empresa.
    /// </summary>
    abstract class Factura<E, M> : Actualizable, ITieneLíneas<M> where M : MovimientoProducto where E : EntidadEconómica { // Si bien una vez hecha la factura esta no debería aceptar cambios, para efectos de SimpleOps si se generan cambios en su estado y en su liquidación. Es Actualizable y no Rastreable porque ya dispone de una FechaHora propia que informa de su creación, que aunque en algunas ocasiones podría ser actualizada después de la creación por lo general no se hace.


        #region Propiedades Abstractas

        public abstract E? EntidadEconómica { get; } // En cada una de las clases derivadas se mantienen las propiedades con el nombre Cliente o Proveedor pero se implementa de forma trivial Entidad => Cliente o Proveedor únicamente para su uso dentro de esta clase abtracta. Para el resto de usos se debe usar las propiedades Cliente o Proveedor.

        public abstract List<M> Líneas { get; set; } // Se usa el término Líneas en vez de Detalles porque es lo más usado y más obvio a que se refiere.

        #endregion Propiedades Abstractas>


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Número de la factura que con el prefijo forman el identificador único de la factura. Si la empresa no usa prefijos este número debe ser único.
        /// </summary>
        public int Número { get; set; } // No se requiere unicidad del número de la factura ni se establece como clave porque este puede ser repetido en caso que la empresa use prefijos.

        /// <summary>
        /// Opcional. Código alfanumérico antes del número de la factura.
        /// </summary>
        /// <MaxLength>10</MaxLength>
        [MaxLength(10)]
        public string? Prefijo { get; set; }

        /// <summary>
        /// Fecha y hora legal de la factura.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// No afecta la base gravable para el IVA ni retenciones. Es un descuento que depende que se de cierta condición para ser aplicado 
        /// y hasta que no se de esa condición hay incertidumbre de su aplicación, por lo tanto los impuestos y retenciones son aplicados 
        /// sin tenerlo en cuenta. Por ejemplo, se usa para los descuentos financieros aplicables solo si se paga antes de cierta fecha. 
        /// Contablemente se registra como un gasto operacional financiero para el vendedor (https://puc.com.co/530535) y un ingreso no
        /// operacional financiero para el comprador (https://puc.com.co/421040). En la factura electrónica se reporta como un descuento 
        /// a nivel de factura siguiendo las indicaciones de la documentación de la DIAN.
        /// </summary>
        public decimal DescuentoCondicionado { get; set; }

        /// <summary>
        /// Afecta la base gravable para el IVA y retenciones. Es un descuento del que se tiene certeza que se va aplicar en el momento de
        /// la facturación sin depender de ninguna condición futura, por lo tanto los impuestos y retenciones son aplicados teniéndolo en 
        /// cuenta. Por ejemplo, se usa para descuentos comerciales aplicables si se compra más de cierta cantidad. Es equivalente a reducir 
        /// el precio de los productos, por lo tanto se da la posibilidad de aplicarlo como un descuento comercial a pie de factura o 
        /// aplicarlo como una reducción directa en los precios de los productos y en este valor usar 0. En la factura electrónica se reporta 
        /// como un descuento por línea (independiente si se ha hecho por línea o como descuento comercial a pie de factura) siguiendo las 
        /// indicaciones de la documentación de la DIAN.
        /// </summary>
        public decimal DescuentoComercial { get; set; }

        /// <summary>
        /// No lo provee el usuario se calcula como la suma de los subtotales de todas las líneas (<see cref="MovimientoProducto"/>). También se conoce como valor bruto. No incluye el valor de las muestras gratis.
        /// </summary>
        public decimal Subtotal { get; set; } // Obligatorio. Se prefiere el término subtotal a valor bruto porque es más tradicional en el contexto de pequeños negocios.

        public decimal IVA { get; set; }

        /// <summary>
        /// Suma de los impuestos al consumo. Su tipo depende de los productos. Puede ser varios tipos si la factura incluye productos que tienen diferentes 
        /// impuestos al consumo.
        /// </summary>
        public decimal ImpuestoConsumo { get; set; }

        public decimal RetenciónFuente { get; set; }

        public decimal RetenciónIVA { get; set; }

        public decimal RetenciónICA { get; set; }

        public decimal RetencionesExtra { get; set; }

        /// <summary>
        /// PendientePago = 0. Pagada = 1: La entidad económica ya pagó la factura. Anulada = 2: Aunque la anulación de facturas legalmente ya no es válida en Colombia se mantiene porque es un concepto ampliamente aceptado y es una abstracción útil para el usuario. La anulación equivale a aplicar una nota crédito por la totalidad de la factura.
        /// </summary>
        public EstadoFactura Estado { get; set; } = EstadoFactura.PendientePago;

        /// <summary>
        /// Número consecutivo del documento electrónico. Se reinicia automáticamente cada año. Se genera autoincrementa automáticamente para los 
        /// documentos de venta y se pueden cargar desde el archivo xml para los documentos de compra.
        /// </summary>
        public int? ConsecutivoDianAnual { get; set; } // Puede ser nulo para permitir la migración de datos anteriores y para compras de proveedores que aún no tengan implementada la factura electrónica o si no se manejan las compras mediante archivos xml de facturas electrónicas.

        /// <summary>
        /// Código único de documento electrónico.
        /// </summary>
        /// <MaxLength>96</MaxLength>
        [MaxLength(96)]
        public string? Cude { get; set; } // Puede ser nulo para permitir la migración de ventas anteriores a la implementación de la factura electrónica y para compras a proveedores que aún no tengan implementada la factura electrónica.

        /// <summary>
        /// Una observación libre sobre el documento que no se almacena en la base de datos para evitar crezca de tamaño innecesariamente. 
        /// Se usa como almacenamiento intermedio y queda escrita en la representación gráfica de la factura.
        /// </summary>
        [NotMapped]
        public string? Observación { get; set; }

        /// <summary>
        /// No se almacena en la base de datos para evitar que crezca de tamaño innecesariamente. Se usa para mostrar información adicional legal que no 
        /// se muestra siempre (para evitar saturar el diseño) para algunos algunos clientes que requieren que la tenga.
        /// </summary>
        [NotMapped]
        public bool MostrarInformaciónAdicional { get; set; } = false;

        /// <summary>
        /// Usuario que creó la factura. Se usa principalmente como almacenamiento intermedio entre los archivos de integración o el usuario actual
        /// de SimpleOps y las representaciones gráficas. Al crear un nuevo objeto Factura, se inicia esta variable con Global.UsuarioActual.
        /// </summary>
        [NotMapped]
        public Usuario? Usuario { get; set; }

        #endregion Propiedades>


        #region Propiedades de Cálculo Forzado 
        // Ver descripción de los términos Base y Real en MovimientoProducto. Se diferencian de las propiedades autocalculables que no se calculan siempre que se acceden si no únicamente la primera vez que se acceden. Esto se hace porque su cálculo es más costoso. No mantienen su valor actualizado con los valores de las otras propiedades, para forzar su actualización se debe ejecutar CalcularTodo().

        private decimal? _SubtotalBase;
        /// <summary>
        /// Subtotal incluyendo el descuento comercial y tomando las muestras gratis con valor cero.
        /// </summary>
        public decimal SubtotalBase { get => SiNulo(ref _SubtotalBase, () => CalcularTodo(calcularQR: false)); set => _SubtotalBase = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _SubtotalBaseIVA;
        /// <summary>
        /// Subtotal base para el cálculo del IVA, incluye el descuento comercial, suma el valor comercial de las muestras gratis y omite el subtotal de 
        /// los productos excluídos de IVA.
        /// </summary>
        public decimal SubtotalBaseIVA { get => SiNulo(ref _SubtotalBaseIVA, () => CalcularTodo(calcularQR: false)); set => _SubtotalBaseIVA = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _SubtotalBaseIVADian;
        /// <summary>
        /// Subtotal base para el cálculo del IVA, incluye el descuento comercial, suma el valor comercial de las muestras gratis y omite el subtotal de 
        /// los productos excluídos y exentos de IVA. Es necesario hacerlo diferenciado de SubtotalBaseIVA porque este es el valor que la DIAN solicita 
        /// cuando pide el subtotal sin impuestos.
        /// </summary>
        public decimal SubtotalBaseIVADian { get => SiNulo(ref _SubtotalBaseIVADian, () => CalcularTodo(calcularQR: false)); set => _SubtotalBaseIVADian = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _SubtotalBaseReal;
        /// <summary>
        /// Incluye el descuento comercial y suma el valor de las muestras gratis.
        /// </summary>
        public decimal SubtotalBaseReal { get => SiNulo(ref _SubtotalBaseReal, () => CalcularTodo(calcularQR: false)); set => _SubtotalBaseReal = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _SubtotalFinal;
        /// <summary>
        /// Incluye el descuento comercial y el condicionado, y no tiene en cuenta el valor de las muestras. 
        /// </summary>
        public decimal SubtotalFinal { get => SiNulo(ref _SubtotalFinal, () => CalcularTodo(calcularQR: false)); set => _SubtotalFinal = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _Margen;
        public decimal Margen { get => SiNulo(ref _Margen, () => CalcularTodo(calcularQR: false)); set => _Margen = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _Costo;
        public decimal Costo { get => SiNulo(ref _Costo, () => CalcularTodo(calcularQR: false)); set => _Costo = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private decimal? _SubtotalFinalConImpuestos;
        /// <summary>
        /// SubtotalFinal + IVA + Impuesto Consumo. Se calcula para registrarlo en la factura electrónica, es al que típicamente se le llama Total. 
        /// Se evita usar el nombre Total para evitar crear confusión con APagar.
        /// </summary>
        public decimal SubtotalFinalConImpuestos { // No se almacena en la base de datos, se ignora en OnModelCreating().
            get => SiNulo(ref _SubtotalFinalConImpuestos, () => CalcularTodo(calcularQR: false)); 
            set => _SubtotalFinalConImpuestos = value; 
        }

        private decimal? _APagar;
        /// <summary>
        /// Valor a pagar definitivo, incluyendo todos los descuentos, impuestos y retenciones.
        /// </summary>
        public decimal APagar { get => SiNulo(ref _APagar, () => CalcularTodo(calcularQR: false)); set => _APagar = value; } // No se almacena en la base de datos, se ignora en OnModelCreating().

        private string? _QR;
        /// <summary>
        /// Código QR de la factura en base 64.
        /// </summary>
        public string? QR { get => SiNulo(ref _QR, () => CalcularTodo(calcularQR: true)); set => _QR = value; } // Es redundante calcularQR: true porque ya verdadero el valor por defecto pero se deja para establecer la claridad con las demás propiedades. No se almacena en la base de datos, se ignora en OnModelCreating().

        #endregion Propiedades de Cálculo Forzado>


        #region Constructores

        public Factura() => (FechaHora, Usuario) = (AhoraUtcAjustado, UsuarioActual);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Código alfanumérico que incluye el prefijo y el número de la factura. Si no hay prefijo es una representación en texto del número.
        /// </summary>
        public string Código => Prefijo + Número.ATexto();

        public decimal? PorcentajeDescuentoCondicionado => Subtotal == 0 ? (decimal?)null : DescuentoCondicionado / Subtotal;

        public decimal? PorcentajeDescuentoComercial => Subtotal == 0 ? (decimal?)null : DescuentoComercial / Subtotal;

        public string NombreCude => this is Venta ? "CUFE" : "CUDE";

        /// <summary>
        /// Es nulo cuando el precio es cero o es una muestra gratis.
        /// </summary>
        public decimal? PorcentajeMargen => ObtenerPorcentajeMargen(SubtotalFinal, Costo);

        /// <summary>
        /// El porcentaje aplicado al costo unitario para obtener el precio de venta. En inglés: Markup Percentage. Es nulo cuando el precio es cero o es una muestra gratis..
        /// </summary>
        public decimal? PorcentajeGanancia => ObtenerPorcentajeGanancia(SubtotalFinal, Costo);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string SubtotalFinalConImpuestosTexto => SubtotalFinalConImpuestos.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string IVATexto => IVA.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string ImpuestoConsumoTexto => ImpuestoConsumo.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string SubtotalBaseTexto => SubtotalBase.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string DescuentoCondicionadoTexto => DescuentoCondicionado.ATextoDinero(agregarMoneda: false);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        private decimal ObtenerRetención(Func<ReglasImpuesto> obtenerReglas) => ObtenerRetención<TipoCliente>(tc => obtenerReglas()); // Se usa cualquier enumeración porque de todas maneras no se usa.


        /// <summary>
        /// <para>Obtiene el valor de una retención de cualquier tipo usando las líneas de la factura actual, la función 
        /// (<paramref name="obtenerPropiedad"/>) para obtener la propiedad que será usada para clasificar cada producto en grupos de productos 
        /// con el mismo valor y una función <paramref name="obtenerReglas"/> para obtener las <see cref="ReglasImpuesto"/> aplicables para cada 
        /// valor de  enumeración de tipo <typeparamref name="C"/>. Si <paramref name="obtenerPropiedad"/> es nula significa que la retención se aplica 
        /// sin distinción de cualquier clasificación en el producto, en estos casos se puede especificar <see cref="object"/> como parámetro de 
        /// tipo para <typeparamref name="C"/>.</para>
        /// <para>Por ejemplo para una <see cref="Venta"/>, se puede provee la función <paramref name="obtenerPropiedad"/> que 
        /// devuelva la propiedad <see cref="Producto.ConceptoRetención"/> de cada producto y la función <paramref name="obtenerReglas"/> que 
        /// tome como parámetro un <see cref="ConceptoRetención"/>. Se calcula formando grupos de productos que tengan el mismo valor de 
        /// <see cref="Producto.ConceptoRetención"/>. Para cada grupo se calcula el subtotal y se obtienen las <see cref="ReglasImpuesto"/>. 
        /// El subtotal de cada grupo se compara con el <see cref="ReglasImpuesto.Mínimo"/> y si es mayor se aplica el 
        /// <see cref="ReglasImpuesto.Porcentaje"/> para obtener el valor de la retención.</para>
        /// <para>De manera predeterminada se usa como base para el cálculo <see cref="MovimientoProducto.SubtotalBase"/>, pero si se desea
        /// usar una base diferente, por ejemplo en el caso de la retención de IVA, esta base/propiedad se puede pasar en 
        /// <paramref name="obtenerOtraBase"/>.</para>
        /// </summary>
        private decimal ObtenerRetención<C>(Func<C, ReglasImpuesto> obtenerReglas, Func<M, C>? obtenerPropiedad = null, 
            Func<M, decimal>? obtenerOtraBase = null) where C : struct, Enum {

            if (obtenerPropiedad != null) {

                var líneasPorClasificación = Líneas.GroupBy(d => obtenerPropiedad(d)).ToDictionary(g => g.Key, g => g.ToList()); // Puede ser ConceptoRetención o TipoProducto.
                var retención = 0M;
                foreach (var kv in líneasPorClasificación) {

                    var clasificación = kv.Key;
                    var líneasPorConcepto = kv.Value;
                    var reglas = obtenerReglas(clasificación);
                    var subtotalConcepto = líneasPorConcepto.Sum(d => d.SubtotalBase);
                    var baseCálculo = obtenerOtraBase == null ? subtotalConcepto : líneasPorConcepto.Sum(d => obtenerOtraBase(d));      
                    if (subtotalConcepto >= reglas.Mínimo) retención += reglas.Porcentaje * baseCálculo;

                }
                return retención;

            } else {

                var reglas = obtenerReglas(default);
                var subtotal = Líneas.Sum(d => d.SubtotalBase);
                var baseCálculo = obtenerOtraBase == null ? subtotal : Líneas.Sum(d => obtenerOtraBase(d));
                if (subtotal >= reglas.Mínimo) {
                    return reglas.Porcentaje * baseCálculo;
                } else {
                    return 0;
                }

            }

        } // ObtenerRetención>


        /// <summary>
        /// <para>Calcula todos los totales, impuestos y retenciones de la factura. Devuelve verdadero si el cálculo fue exitoso. 
        /// Si se pasa <paramref name="cargarLíneas"/>, <paramref name="cargarEntidadEconómica"/> o <paramref name="cargarProductos"/> en verdadero 
        /// se debe pasar un <paramref name="ctx"/> no nulo.</para>
        /// <para>Si se pasa <paramref name="cargarLíneas"/> en verdadero
        /// y <see cref="Factura{E, M}.Líneas"/> está vacío se consultará la base de datos para intentar llenarlos. Si se pasa 
        /// <paramref name="cargarLíneas"/> en falso y <see cref="Factura{E, M}.Líneas"/> está vacío, no se calculará nada.</para>
        /// <para>Si se pasa <paramref name="cargarEntidadEconómica"/> en verdadero
        /// y <see cref="Factura{E, M}.EntidadEconómica"/> es nula se consultará la base de datos para intentar llenarla. Si se pasa 
        /// <paramref name="cargarEntidadEconómica"/> en falso y <see cref="Factura{E, M}.EntidadEconómica"/> es nula, no se calculará nada.</para>
        /// <para>Si se pasa <paramref name="cargarProductos"/> en verdadero
        /// y algún Factura{E, M}.Líneas.Producto es nulo se consultará la base de datos para intentar llenarlo. Si se pasa 
        /// <paramref name="cargarProductos"/> en falso y algún Factura{E, M}.Líneas.Producto es nulo, no se calculará nada.</para>
        /// <para>La posibilidad de cargar las propiedades faltantes en esta función se provee por flexibilidad, pero puede 
        /// generar una disminución importante del rendimiento. Siempre es preferible disponer previamente de la <see cref="Factura{E, M}"/> con sus propiedades 
        /// de navegación completas: <see cref="Factura{E, M}.Líneas"/>, <see cref="Factura{E, M}.EntidadEconómica"/> y todos los 
        /// Factura{E, M}.Líneas.Producto.</para>
        /// </summary>
        public bool CalcularTodo(Contexto? ctx = null, bool cargarLíneas = false, bool cargarEntidadEconómica = false, bool cargarProductos = false, 
            bool calcularQR = true) {

            (Subtotal, SubtotalBase, SubtotalBaseReal, SubtotalBaseIVA, SubtotalBaseIVADian, IVA, ImpuestoConsumo, Margen, Costo, APagar, SubtotalFinalConImpuestos, 
                RetenciónFuente, RetenciónIVA, RetenciónICA, RetencionesExtra, SubtotalFinal) = (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); // Reinicio de valores.

            #region Verificaciones de nulidad de propiedades necesarias

            static bool cargarLíneasYProductos(Factura<E,M> factura, Contexto ctx) 
                => factura switch {
                    Venta venta => ctx.Ventas.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == venta.ID).Líneas.ToList().Count == 0, // Se usa el == 0 solo para forzar una variable igual para todo el switch. Con la consulta .Líneas.ToList() se asegura que se cargan las líneas al contexto y por lo tanto a la variable Líneas, no es necesario asignarla.
                    Compra compra => ctx.Compras.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == compra.ID).Líneas.ToList().Count == 0,
                    NotaCréditoVenta notaCréditoVenta => ctx.NotasCréditoVenta.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == notaCréditoVenta.ID).Líneas.ToList().Count == 0,
                    NotaCréditoCompra notaCréditoCompra => ctx.NotasCréditoCompra.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == notaCréditoCompra.ID).Líneas.ToList().Count == 0,
                    NotaDébitoVenta notaDébitoVenta => ctx.NotasDébitoVenta.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == notaDébitoVenta.ID).Líneas.ToList().Count == 0,
                    NotaDébitoCompra notaDébitoCompra => ctx.NotasDébitoCompra.Include(v => v.Líneas).ThenInclude(dv => dv.Producto)
                        .Single(a => a.ID == notaDébitoCompra.ID).Líneas.ToList().Count == 0,
                    _ => throw new Exception(CasoNoConsiderado(factura.GetType().ToString())),
                };

            if (!Líneas.Any()) {

                if (cargarLíneas) {
                    if (ctx == null) throw new Exception("Si se pasa cargarLíneas en verdadero es obligatorio " +
                                                         "pasar un objeto ctx de tipo Contexto no nulo.");
                    cargarLíneasYProductos(this, ctx);
                } else {
                    return false; // No hay líneas y tampoco se forzó su carga. No calcula nada.
                }

            }

            if (Líneas.Any(d => d.Producto == null)) {

                if (cargarProductos) {
                    if (ctx == null) throw new Exception("Si se pasa cargarProductos en verdadero es obligatorio " +
                                                         "pasar un objeto ctx de tipo Contexto no nulo.");
                    cargarLíneasYProductos(this, ctx);
                } else {
                    return false; // Hay al menos un producto nulo en las líneas y tampoco se forzó su carga. No calcula nada.
                }

            }

            if (EntidadEconómica == null) {

                if (cargarEntidadEconómica) {

                    if (ctx == null) throw new Exception("Si se pasa cargarEntidadEconómica en verdadero es obligatorio " +
                                                         "pasar un ctx de tipo Contexto no nulo.");
                    _ = this switch {
                        Venta venta => ctx.CargarPropiedad(venta, f => f.Cliente),
                        Compra compra => ctx.CargarPropiedad(compra, f => f.Proveedor),
                        NotaCréditoVenta notaCréditoVenta => ctx.CargarPropiedad(notaCréditoVenta, f => f.Cliente),
                        NotaCréditoCompra notaCréditoCompra => ctx.CargarPropiedad(notaCréditoCompra, f => f.Proveedor),
                        NotaDébitoVenta notaDébitoVenta => ctx.CargarPropiedad(notaDébitoVenta, f => f.Cliente),
                        NotaDébitoCompra notaDébitoCompra => ctx.CargarPropiedad(notaDébitoCompra, f => f.Proveedor),
                        _ => throw new Exception(CasoNoConsiderado(EntidadEconómica?.GetType().ToString())),
                    };

                } else {
                    return false; // No hay entidad económica cargada y tampoco se forzó su carga. No calcula nada.
                }

            }

            #endregion Verificaciones de nulidad de propiedades necesarias>


            #region Cálculo

            Subtotal = Líneas.Sum(d => d.Subtotal);
            if (Subtotal == 0) return false; // Si no hay subtotal directo, no hay nada que calcular, todo es cero.

            foreach (var línea in Líneas) {

                línea.PorcentajeDescuentoComercial = (decimal)PorcentajeDescuentoComercial!; // Se garantiza que no es nulo porque se asegura que Subtotal no es cero. Necesario asignarlo antes porque este valor se necesita para el correcto cálculo de las otras asignaciones.
                línea.PorcentajeDescuentoCondicionado = (decimal)PorcentajeDescuentoCondicionado!; // Se garantiza que no es nulo porque se asegura que Subtotal no es cero. Igual que el anterior.
                SubtotalBaseReal += línea.SubtotalBaseReal;
                SubtotalBase += línea.SubtotalBase;
                SubtotalBaseIVA += (decimal)línea.SubtotalBaseIVA!; // Si ningún línea.Producto es nulo se asegura que la SubtotalBaseIVA tampoco lo sea.
                SubtotalBaseIVADian += (decimal)línea.SubtotalBaseIVADian!; // Si ningún línea.Producto es nulo se asegura que la SubtotalBaseIVADian tampoco lo sea.
                IVA += (decimal)línea.IVA!; // Si EntidadEconómica no es nula se asegura que el IVA tampoco lo sea.
                ImpuestoConsumo += (decimal)línea.ImpuestoConsumo!; // Si ningún línea.Producto es nulo se asegura que el ImpuestoConsumo tampoco lo sea.
                Costo += línea.Costo;
                Margen += línea.Margen;
                SubtotalFinal += línea.SubtotalFinal;

            }

            RetenciónFuente = ObtenerRetención(
                clasificación => EntidadEconómica switch {
                    Cliente cliente => ObtenerReglasRetenciónFuenteVenta(cliente, clasificación),
                    Proveedor proveedor => ObtenerReglasRetenciónFuenteCompra(proveedor, clasificación),
                    _ => throw new Exception(CasoNoConsiderado(EntidadEconómica?.GetType().ToString())),
                },
                m => m.Producto!.ConceptoRetención // Se asegura que ningún Producto será nulo porque ya se verificó anteriormente. 
            );

            RetenciónIVA = ObtenerRetención(
                clasificación => EntidadEconómica switch {
                    Cliente cliente => ObtenerReglasRetenciónIVAVenta(cliente, clasificación),
                    Proveedor proveedor => ObtenerReglasRetenciónIVACompra(proveedor, clasificación),
                    _ => throw new Exception(CasoNoConsiderado(EntidadEconómica?.GetType().ToString())),
                },
                m => m.Producto!.TipoProducto, d => (decimal)d.IVA! // Se asegura que ningún Producto será nulo porque ya se verificó anteriormente. Además también se verificó que EntidadEconómica no es nula entonces el IVA tampoco lo es. 
            );

            RetenciónICA = ObtenerRetención(
                () => EntidadEconómica switch {
                    Cliente cliente => new ReglasImpuesto((decimal)cliente.PorcentajeRetenciónICA, cliente.MínimoRetenciónICA),
                    Proveedor proveedor => new ReglasImpuesto((decimal)Empresa.PorcentajeRetenciónICA, Empresa.MínimoRetenciónICA),
                    _ => throw new Exception(CasoNoConsiderado(EntidadEconómica?.GetType().ToString())),
                }
            );

            RetencionesExtra = ObtenerRetención(
                () => EntidadEconómica switch {
                    Cliente cliente => new ReglasImpuesto((decimal)cliente.PorcentajeRetencionesExtra, cliente.MínimoRetencionesExtra),
                    Proveedor proveedor => new ReglasImpuesto((decimal)Empresa.PorcentajeRetencionesExtra, Empresa.MínimoRetencionesExtra),
                    _ => throw new Exception(CasoNoConsiderado(EntidadEconómica?.GetType().ToString())),
                }
            );

            SubtotalFinalConImpuestos = SubtotalFinal + IVA + ImpuestoConsumo;
            APagar = SubtotalBase + IVA + ImpuestoConsumo - DescuentoCondicionado - RetenciónFuente - RetenciónIVA - RetenciónICA - RetencionesExtra; // El descuento comercial ya está incluído en el subtotalÍtems porque se incluye a nivel de movimiento de producto.       
            #endregion Cálculo>

            #region Cálculo del CUDE
            // Requiere tener actualizado el informe de pago, si el informe de pago se agrega posteriormente a CalcularTodo() se modifica este valor. Calcula el Código Único de Documento Electrónico (CUDE) para la DIAN. Se llama CUFE cuando es factura de venta.

            var formatoFecha = "yyyy-MM-dd";
            var formatoNúmero = "0.00";
            var formatoHora = $"HH:mm:ss{Generales.HorasAjusteUtc.ATexto("00")}:00";
            var textoFechaFactura = FechaHora.ATexto(formatoFecha);
            var textoHoraFactura = FechaHora.ATexto(formatoHora);
            var textoSubtotalBase = (SubtotalBase).ATexto(formatoNúmero);
            var textoIVA = IVA.ATexto(formatoNúmero);
            var textoImpuestoConsumo = ImpuestoConsumo.ATexto(formatoNúmero);
            var impuestoICA = 0; // Por el momento el ICA se toma en 0.
            var textoImpuestoICA = (impuestoICA).ATexto(formatoNúmero);
            var textoAPagar = (SubtotalFinalConImpuestos - ObtenerAnticipo()).ATexto(formatoNúmero);
            if (EntidadEconómica == null) throw new Exception("No se ha cargado la entidad económica.");
            if (this is NotaCréditoVenta && Prefijo == null) Prefijo = PrefijoNotasCréditoPredeterminado; // Es necesario establecer un prefijo obligatorio para las notas crédito por un error que presenta el servidor de la DIAN en 2021 con la aceptación de la numeración de estas si no llevan prefijo.
            if (this is NotaDébitoVenta && Prefijo == null) Prefijo = PrefijoNotasDébitoPredeterminado; // Es necesario establecer un prefijo obligatorio para las notas débito por un error que presenta el servidor de la DIAN en 2021 con la aceptación de la numeración de estas si no llevan prefijo.
            var textoCude = $"{Código}{textoFechaFactura}{textoHoraFactura}{textoSubtotalBase}01{textoIVA}04{textoImpuestoConsumo}03{textoImpuestoICA}" + 
                            $"{textoAPagar}{Empresa.Nit}{EntidadEconómica.Identificación}{ObtenerClaveParaCude()}" + 
                            $"{Empresa.AmbienteFacturaciónElectrónica.AValor()}";
            Cude = ObtenerSHA384(textoCude);

            #endregion Cálculo del CUDE>

            #region Cálculo del QR
            // Este es el paso más costoso de Requiere tener el CUDE actualizado.

            if (calcularQR) { // El cálculo del QR es considerablemente más demorado que el resto del procedimiento (se demora 200 ms vs 36 ms) entonces se omite cuando no se ha llamado de manera explícita el CalcularTodo si no qoue se está llamando desde una de las propiedades de cálculo forzado que no sea QR.

                var textoOtrosImpuestos = (ImpuestoConsumo + impuestoICA).ATexto(formatoNúmero);
                var textoQR = $"NumFac: {Código}{NuevaLínea}FecFac: {textoFechaFactura}{NuevaLínea}HorFac: {textoHoraFactura}{NuevaLínea}" +
                              $"NitFac: {Empresa.Nit}{NuevaLínea}DocAdq: {EntidadEconómica.NitLegalEfectivo}{NuevaLínea}ValFac: {textoSubtotalBase}" +
                              $"{NuevaLínea}ValIva: {textoIVA}{NuevaLínea}ValOtroIm: {textoOtrosImpuestos}{NuevaLínea}ValTolFac: {textoAPagar}{NuevaLínea}" +
                              $"{NombreCude}: {Cude}{NuevaLínea}QRCode: {ObtenerRutaQR(Cude)}";
                QR = ObtenerCódigoQRBase64(textoQR, paraHtml: true);

            }

            #endregion Cálculo del CUDE>

            return true;

        } // CalcularTodo>


        #endregion Métodos y Funciones>


        #region Métodos y Funciones Estáticas

        private static ReglasImpuesto ObtenerReglasRetenciónIVACompra(Proveedor proveedor, TipoProducto tipoProducto)
            => ObtenerReglasRetenciónIVA(null, null, Empresa.TipoContribuyente, proveedor.TipoContribuyente, tipoProducto);

        private static ReglasImpuesto ObtenerReglasRetenciónIVAVenta(Cliente cliente, TipoProducto tipoProducto)
            => ObtenerReglasRetenciónIVA(cliente.PorcentajeRetenciónIVAPropio, cliente.MínimoRetenciónIVAPropio, cliente.TipoContribuyente, 
                Empresa.TipoContribuyente, tipoProducto);

        private static ReglasImpuesto ObtenerReglasRetenciónFuenteCompra(Proveedor proveedor, ConceptoRetención conceptoRetención)
            => ObtenerReglasRetenciónFuente(null, null, proveedor.TipoEntidad, conceptoRetención, proveedor.TipoContribuyente);

        private static ReglasImpuesto ObtenerReglasRetenciónFuenteVenta(Cliente cliente, ConceptoRetención conceptoRetención)
            => ObtenerReglasRetenciónFuente(cliente.PorcentajeRetenciónFuentePropio, cliente.MínimoRetenciónFuentePropio, Empresa.TipoEntidad,
                conceptoRetención, Empresa.TipoContribuyente);


        private static ReglasImpuesto ObtenerReglasRetenciónIVA(double? porcentajeForzado, decimal? mínimoForzado, 
            TipoContribuyente tipoContribuyenteComprador, TipoContribuyente tipoContribuyenteVendedor, TipoProducto tipoProducto) { // Ver https://dianhoy.com/reteiva-retencion-en-la-fuente-por-iva/ y https://www.gerencie.com/retencion-en-la-fuente-por-iva-reteiva.html. No tiene en cuenta el caso 'No residente o no domiciliado en el país'.

            var mínimo = tipoProducto switch {
                TipoProducto.Desconocido => throw new Exception("No se esperaba tipoProducto = Desconocido."),
                TipoProducto.Producto => Generales.MínimoUVTRetenciónIVAProductosLegal * Generales.UVT,
                TipoProducto.Servicio => Generales.MínimoUVTRetenciónIVAServiciosLegal * Generales.UVT,
            };

            double porcentaje;
            if (tipoContribuyenteVendedor.HasFlag(TipoContribuyente.ResponsableIVA)) {

                if (tipoContribuyenteComprador.HasFlag(TipoContribuyente.GranContribuyente) 
                    || tipoContribuyenteComprador.HasFlag(TipoContribuyente.RetenedorIVA)) { //  El tipo RetenedorIVA es explícito que si lo debe retener, pero los grandes también, así que se aceptan ambos tipos.
                    porcentaje = Generales.PorcentajeRetenciónIVALegal;
                } else {
                    porcentaje = 0;
                }

            } else {
                porcentaje = 0;
            }

            return new ReglasImpuesto((decimal)(porcentajeForzado ?? porcentaje), mínimoForzado ?? mínimo);

        } // ObtenerReglasRetenciónIVA>


        /// <summary>
        /// <b>En Compra:</b> Cuando por la compra de un producto la empresa (agente retención) paga a un proveedor (sujeto pasivo) le retiene un porcentaje del valor dependiendo
        /// del <paramref name="tipoEntidadVendedor"/> y del <paramref name="tipoContribuyenteVendedor"/> del proveedor y del tipo de producto 
        /// (<paramref name="concepto"/>).<br/>
        /// <b>En Venta:</b> Cuando por la venta de un producto la empresa (sujeto pasivo) recibe el pago de un cliente (agente retención), este le retiene un porcentaje del valor 
        /// dependiendo del <paramref name="tipoEntidadVendedor"/> y del <paramref name="tipoContribuyenteVendedor"/> de la empresa y del tipo de 
        /// producto (<paramref name="concepto"/>).
        /// </summary>
        private static ReglasImpuesto ObtenerReglasRetenciónFuente(double? porcentajeForzado, decimal? mínimoForzado, 
            TipoEntidad tipoEntidadVendedor, ConceptoRetención concepto, TipoContribuyente tipoContribuyenteVendedor) {

            if (tipoContribuyenteVendedor.HasFlag(TipoContribuyente.Autorretenedor)) return new ReglasImpuesto((decimal)(porcentajeForzado ?? 0), mínimoForzado ?? 0);

            var tipoDeclarante = tipoEntidadVendedor switch { // Se usará la clasificación entre Empresa y Persona para diferenciar entre Declarante y No Declarante. No hay claridad completa sobre si esto es o no correcto, sobretodo en el caso de personas, pero es una aceptable aproximación. Esto es necesario porque la información más fácilmente disponible de las entidades económicas es el tipo de entidad.
                TipoEntidad.Desconocido => TipoDeclarante.Desconocido,
                TipoEntidad.Empresa => TipoDeclarante.Declarante,
                TipoEntidad.Persona => TipoDeclarante.NoDeclarante,
            };

            var mínimo = 0M;
            var porcentaje = 0D;

            var retencionesConcepto = Generales.RetencionesFuente.Where(rf => rf.Concepto == concepto).ToList();
            if (retencionesConcepto.Any()) {

                if (retencionesConcepto.Count == 1 && retencionesConcepto[0].TipoDeclarante == TipoDeclarante.Desconocido) { // Si no hay distinción entre declarantes y no declarantes.

                    porcentaje = retencionesConcepto[0].Porcentaje;
                    mínimo = retencionesConcepto[0].MínimoUVT * Generales.UVT;

                } else if (retencionesConcepto.Count == 2) { // Si hay dos es porque está la regla para declarante y la de no declarante.

                    var retenciónConceptoYTipoDeclarante = retencionesConcepto.Where(rf => rf.TipoDeclarante == tipoDeclarante).ToList();
                    if (retenciónConceptoYTipoDeclarante.Count == 1) {

                        porcentaje = retenciónConceptoYTipoDeclarante[0].Porcentaje;
                        mínimo = retenciónConceptoYTipoDeclarante[0].MínimoUVT * Generales.UVT;

                    } else {
                        throw new Exception($"El diccionario RetencionesFuente está mal formado. No hay solo una coincidencia para " + 
                            $"{concepto} y {tipoDeclarante}");
                    }

                } else {
                    throw new Exception($"El diccionario RetencionesFuente está mal formado. Hay más de dos coincidencias para {concepto}");
                }

            } else {
                throw new Exception(CasoNoConsiderado(concepto));
            }

            return new ReglasImpuesto((decimal)(porcentajeForzado ?? porcentaje), mínimoForzado ?? mínimo);

        } // ObtenerParámetrosRetenciónCompra>


        #endregion Métodos y Funciones Estáticas


        #region Métodos y Funciones Abstractas y Virtuales

        /// <summary>
        /// Es necesario implementarlo y llamarlo en cada constructor de cada clase que se derive de <see cref="Factura{E, M}"/> para verificar la necesidad de datos
        /// en las entidades para la generación de la factura.
        /// </summary>
        /// <returns></returns>
        public abstract void VerificarDatosEntidad();

        public abstract decimal ObtenerAnticipo();

        public abstract string? ObtenerClaveParaCude();

        #endregion Métodos y Funciones Abstractas y Virtuales>


    } // Factura>



} // SimpleOps.Modelo>
