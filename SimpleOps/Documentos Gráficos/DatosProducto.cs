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

        public List<string> Características { get; set; } = new List<string>(); // Antes tenía #pragma warning disable CA2227: Las propiedades de colección deben ser de solo lectura. Se permite para poder crear los diccionarios Características, Atributos y Precios al crear el objeto.

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
