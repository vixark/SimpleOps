using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;
using SimpleOps.DocumentosGráficos;
using AutoMapper;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Ofrecimiento de precios de venta de varios productos a un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)] // Desde la lógica de la operación no hay problema en realizar dos cotizaciones al mismo tiempo al mismo cliente desde dos computadores diferentes, aunque no sería deseable que se hicieran con precios diferentes la concurrencia no es tanto problema.
    class Cotización : Registro {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public List<LíneaCotización> Líneas { get; set; } = new List<LíneaCotización>();

        /// <summary>
        /// Cotización o Catálogo. Al usar Catálogo, permite registrar cuándo se le envió el catálogo a un cliente. En estos casos <see cref="Líneas"/>
        /// puede ser vacío.
        /// </summary>
        public TipoCotización Tipo { get; set; } = TipoCotización.Cotización;

        /// <summary>
        /// Un texto libre que contiene las condiciones comerciales de la cotización. No se almacena en la base de datos para evitar crezca de tamaño 
        /// innecesariamente. Se usa como almacenamiento intermedio y queda escrita en el PDF de la cotización o catálogo.
        /// </summary>
        [NotMapped]
        public string? CondicionesComerciales { get; set; }

        /// <summary>
        /// Enlace que apunta a un archivo XLSX (archivo de Excel) que contiene la lista de los precios cotizados para que el cliente pueda 
        /// analizar la cotización más fácilmente. No se almacena en la base de datos para evitar crezca de tamaño 
        /// innecesariamente. Se usa como almacenamiento intermedio para ser usada en el PDF de la cotización o catálogo.
        /// </summary>
        [NotMapped]
        public string? EnlaceDescargaXlsx { get; set; }

        /// <summary>
        /// Alto y ancho al que se redimensionarán las imágenes. Si es nulo, no se redimensionan. Redimensionar
        /// las imágenes ayuda a que el tamaño final del PDF sea más pequeño y además las imágenes son recortadas inteligentemente (ajustadas a su
        /// contenido) antes de ser redimensionadas, esto permite que todas las imágenes de los productos tengan un tamaño homogéneo independiente
        /// de como estén recortadas las imágenes originales.
        /// </summary>
        [NotMapped]
        public int? TamañoImágenes { get; set; } = Empresa.TamañoImágenesProductosCotizaciones; // Este valor predeterminado se puede asignar aquí porque es el único que aplica tanto para cotizaciones como para catálogo.

        [NotMapped]
        public int? CantidadFilasProductos { get; set; }

        [NotMapped]
        public int? CantidadColumnasProductos { get; set; }

        #endregion Propiedades>


        #region Propiedades Páginas Extra Catálogo

        /// <summary>
        /// Referencias de los productos que se añadirán a las páginas extra del catálogo. Estas referencias pueden ser de productos base o de productos 
        /// específicos. Si se da el caso que un producto base tiene la misma referencia que uno específico, se prefiere el base. 
        /// En el caso de catálogos sin páginas personalizadas (CatálogoPdf2.cshtml, CatálogoPdf3.cshtml, etc), las páginas extra son las únicas
        /// páginas del catálogo.
        /// </summary>
        [NotMapped]
        public List<string> ReferenciasProductosPáginasExtra { get; set; } = new List<string>(); // Se manejan por fuera de LíneaCotización porque son datos que solo le corresponden a la generación del catálogo y no es necesario agregarlos a la tabla Productos para uso general.

        /// <summary>
        /// Si es cero, las páginas extra se insertan al final del documento. Si es 1 las páginas extra se insertan antes de la última página 
        /// para permitir que esta última página sea la contraportada del catálogo. Si es cualquier otro valor, las páginas extra se insertan 
        /// justo antes de ese número de páginas finales personalizadas (CatálogoPdf2.cshtml, CatálogoPdf3.cshtml, etc).
        /// </summary>
        [NotMapped]
        public int? ÍndiceInversoInserciónPáginasExtra { get; set; }

        #endregion Propiedades Páginas Extra Catálogo>


        #region Constructores

        private Cotización() { } // Solo para que EF Core no saque error.

        public Cotización(Cliente cliente) { 
            (ClienteID, Cliente) = (cliente.ID, cliente);
            EstablecerTipo(TipoCotización.Cotización);
        } // Cotización>

        public Cotización(Cliente cliente, TipoCotización tipo) {
            (ClienteID, Cliente) = (cliente.ID, cliente);
            EstablecerTipo(tipo);
        } // Cotización>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}";


        /// <summary>
        /// Al establecer tipo se debe usar este método para cambiar también los valores predeterminados de algunas propiedades.
        /// Se debe llamar siempre después de mapear el objeto.
        /// </summary>
        /// <param name="tipo"></param>
        public void EstablecerTipo(TipoCotización tipo) {

            Tipo = tipo;
            switch (Tipo) {
                case TipoCotización.Cotización:

                    CantidadColumnasProductos ??= Empresa.CantidadColumnasProductosPorPáginaCotización;
                    CantidadFilasProductos ??= Empresa.CantidadFilasProductosPorPáginaCotización;
                    break;

                case TipoCotización.Catálogo:

                    CantidadColumnasProductos ??= Empresa.CantidadColumnasProductosPorPáginaExtraCatálogo;
                    CantidadFilasProductos ??= Empresa.CantidadFilasProductosPorPáginaExtraCatálogo;
                    break;

                default:
                    break;
            }

        } // EstablecerPredeterminadosTipo>


        public DatosCotización ObtenerDatos(OpcionesDocumento opcionesDocumento, PlantillaDocumento plantillaDocumento, int totalPáginasCatálogo = 1) {

            var mapeador = new Mapper(ConfiguraciónMapeadorCotización);
            var datos = mapeador.Map<DatosCotización>(this);
            datos.NombreDocumento = Tipo.ToString();
      
            var mapeadorEmpresa = new Mapper(ConfiguraciónMapeadorEmpresa);
            datos.Empresa = mapeadorEmpresa.Map<DatosEmpresa>(Empresa);
            datos.Columnas = ObtenerOpcionesColumnas(datos);

            datos.LogoBase64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                ArchivoLogoEmpresaImpresión : ArchivoLogoEmpresa), paraHtml: true);
            datos.Logo2Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "2") : AgregarSufijo(ArchivoLogoEmpresa,"2")), paraHtml: true);
            datos.Logo3Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "3") : AgregarSufijo(ArchivoLogoEmpresa, "3")), paraHtml: true);
            datos.Logo4Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "4") : AgregarSufijo(ArchivoLogoEmpresa, "4")), paraHtml: true);
            datos.Logo5Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "5") : AgregarSufijo(ArchivoLogoEmpresa, "5")), paraHtml: true);
            datos.Logo6Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "6") : AgregarSufijo(ArchivoLogoEmpresa, "6")), paraHtml: true);
            datos.Logo7Base64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                AgregarSufijo(ArchivoLogoEmpresaImpresión, "7") : AgregarSufijo(ArchivoLogoEmpresa, "7")), paraHtml: true);
            datos.LogoExcelBase64 = ObtenerBase64(Path.Combine(ObtenerRutaImágenesPlantillas(), opcionesDocumento.ModoImpresión ? 
                ArchivoLogoExcelImpresión : ArchivoLogoExcel), paraHtml: true);

            (datos.ModoDocumento, datos.TotalPáginas) = Tipo switch {
                TipoCotización.Cotización => (DatosDocumento.Modo.CuerpoContinuo, 1),
                TipoCotización.Catálogo => (DatosDocumento.Modo.PáginasIndependientes, totalPáginasCatálogo),
            };

            foreach (var línea in Líneas) {
                if (línea.Producto == null) continue;
                datos.ImágenesProductosBase64.Add(línea.Producto.Referencia, línea.Producto.ObtenerImagenBase64(TamañoImágenes)); 
            }

            datos.ModoImpresión = opcionesDocumento.ModoImpresión;
            datos.MostrarInformaciónAdicional = opcionesDocumento.MostrarInformaciónAdicional;
            datos.NombreArchivoPropio = $"{datos.NombreDocumento} - {Cliente?.Nombre}";

            var productosProductosBase = new Dictionary<string, List<(Producto, decimal)>>();
            var productosBase = new Dictionary<string, ProductoBase>(); // Es necesario hacerlo en un diccionario independiente porque los productos base podrían venir de una fuente de integración entonces podrían ser diferentes objetos así sean el mismo producto base. Al ser diferentes objetos no podrían funcionar como una clave del diccionario.

            foreach (var línea in Líneas) {

                var producto = línea.Producto;
                if (producto == null) continue;
                if (producto.Base != null) { 
                    productosProductosBase.Agregar(producto.Base.Referencia, (producto, línea.Precio));
                    productosBase.Agregar(producto.Base.Referencia, producto.Base); // Se queda con el último producto base con esa referencia. Los productos base podrían venir de una fuente de integración entonces podrían ser diferentes entre sí, aunque no deberían serlo.
                }
                datos.DatosProductos.Add(producto.Referencia, producto.ObtenerDatosProducto(línea.Precio, plantillaDocumento)); // No se inicia el diccionario porque siempre tiene un valor inicial de lista vacía.
            
            }

            foreach (var kv in productosBase) {

                var productoBase = kv.Value;
                var referencia = kv.Key;
                datos.DatosProductos.Agregar(referencia, productoBase.ObtenerDatosProducto(productosProductosBase[referencia], plantillaDocumento)); // Se usa Agregar porque es posible que ya exista la misma referencia de un producto específico en el diccionario. Aunque es algo que no debería suceder en una base de datos bien formada, en estos casos se sobreescribe la información con la del producto base. 
                datos.ImágenesProductosBase64.Agregar(productoBase.Referencia, productoBase.ObtenerImagenBase64(TamañoImágenes)); // Se usa Agregar porque es posible que ya exista la misma referencia de un producto específico en el diccionario. Aunque es algo que no debería suceder en una base de datos bien formada, en estos casos se usa la imagen del producto base.

            }

            return datos;

        } // ObtenerDatos>


        public Integración.DatosCotización ObtenerDatosIntegración() {

            var mapeador = new Mapper(ConfiguraciónMapeadorCotizaciónIntegración);
            var datos = mapeador.Map<Integración.DatosCotización>(this);
            return datos;

        } // ObtenerDatosIntegración>


        #endregion Métodos y Funciones>


    } // Cotización>



} // SimpleOps.Modelo>
