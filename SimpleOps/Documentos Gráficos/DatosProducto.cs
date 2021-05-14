using System;
using System.Collections.Generic;
using System.Text;
using SimpleOps.Modelo;
using static Vixark.General;
using System.Linq;



namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// Encapsula información de un producto o producto base para ser usada en un documento gráfico. Se puede usar para el catálogo, cotizaciones, 
    /// sitio web y fichas técnicas. Es útil para consolidar la información de todos los productos específicos en un producto base y para proveer 
    /// textos formateados para las características y atributos.
    /// </summary>
    public class DatosProducto {



        #region Propiedades

        public string Referencia { get; set; } // Obligatoria. Nunca puede ser nula porque tanto los productos base como los productos específicos tienen una referencia no nula.

        public bool EsProductoBase { get; set; }

        public string? Descripción { get; set; }

        public string? DescripciónBase { get; set; }

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se permite para poder crear los diccionarios Características, Atributos y Precios al crear el objeto.
        public List<string> Características { get; set; } = new List<string>();

        /// <summary>
        /// Si es un producto base, estos atributos son los que están en productos que no difieren en precio. Si difieren en precio, estos atributos
        /// se agregan en <see cref="Precios"/>.
        /// </summary>
        public List<string> Atributos { get; set; } = new List<string>();

        /// <summary>
        /// Contiene los precios aplicables al producto. Es útil en el caso de productos base porque permite mostrar precios diferentes para productos 
        /// específicos con mismo producto base. En estos casos, las claves del diccionario son el resumen de los atributos que difieren en precio.
        /// En el caso de productos específicos, la clave puede ser cualquier valor pues no se muestra y el valor es el único precio a mostrar para 
        /// el producto.
        /// </summary>    
        public Dictionary<List<string>, decimal?> Precios { get; set; } = new Dictionary<List<string>, decimal?>(); // Se permite que sea nulo el decimal para los casos especiales en los que se requiera especificar que no se conoce el precio de un producto, pero al este objeto ser usado principalmente en páginas extra de catálogo y de cotizaciones, lo más normal es que el precio siempre sea un decimal no nulo.
        #pragma warning restore CA2227

        public string? InformaciónHtml { get; set; }

        /// <summary>
        /// Alternativa a <see cref="InformaciónHtml"/> que especifica una ruta en vez de contener el texto de información completo.
        /// Es útil para servir el archivo desde un CDN al sitio web.
        /// </summary>
        public string? RutaInformaciónHtml { get; set; }

        /// <summary>
        /// La referencia que se usará al buscar la imagen en el diccionario ImágenesProductosBase64.
        /// </summary>
        public string? ReferenciaImagenBase64 { get; set; }

        /// <summary>
        /// Si no se dispone del diccionario DatosDocumento.ImágenesProductosBase64, se puede usar esta variable para codificar la imagen en base 64.
        /// </summary>
        public string? ImagenBase64 { get; set; }

        /// <summary>
        /// Alternativa a <see cref="ReferenciaImagenBase64"/> y <see cref="ImagenBase64"/> que especifica una ruta en vez de contener la imagen 
        /// codificada en Base 64. Es útil para servir el archivo desde un CDN al sitio web.
        /// </summary>
        public string? RutaImagen { get; set; }

        #endregion Propiedades>



        #region Propiedades Autocalculadas

        public List<string> CaracterísticasYAtributos => CombinarListas(Características, Atributos);

        public string? CaracterísticasListaHtml => Características.ATextoListaHtml();

        public string? CaracterísticasLíneasHtml => Características.ATextoLíneasHtml();

        public string? CaracterísticasTexto => Características.ATextoConComas();

        public string? AtributosListaHtml => Atributos.ATextoListaHtml();

        public string? AtributosLíneasHtml => Atributos.ATextoLíneasHtml();

        public string? AtributosTexto => Atributos.ATextoConComas();

        public string? CaracterísticasYAtributosListaHtml => CaracterísticasYAtributos.ATextoListaHtml();

        public string? CaracterísticasYAtributosLíneasHtml => CaracterísticasYAtributos.ATextoLíneasHtml();

        public string? CaracterísticasYAtributosTexto => CaracterísticasYAtributos.ATextoConComas();

        #endregion Propiedades Autocalculadas>



        #region Métodos y Funciones

        public Dictionary<string, decimal?> ObtenerPreciosLíneasHtml 
            => Precios.ToDictionary(kv => kv.Key.ATextoLíneasHtml(finalizarLíneasConPunto: false) ?? "", kv => kv.Value);

        #endregion Métodos y Funciones>



        #region Constructores

        public DatosProducto(string referencia, bool esProductoBase) => (Referencia, EsProductoBase) = (referencia, esProductoBase);
               
        #endregion Constructores>



    } // DatosProducto>



} // SimpleOps.DocumentosGráficos>
