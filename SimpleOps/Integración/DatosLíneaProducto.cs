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



namespace SimpleOps.Integración {



    /// <summary>
    /// DTO adecuado para escribir en ambas direcciones: desde DatosLíneaProducto a Producto y desde Producto a DatosLíneaProducto.
    /// Tiene el inconveniente que cuando se escriben propiedades de un producto desde DatosLíneaProducto los productos base que sean
    /// comunes a varios productos se crean como productos base independientes. Esto significa que si después de escrito, se hace
    /// un cambio en la descripción de un producto base esto no afectaría la descripción de otros productos que originalmente tenían ese producto base. 
    /// Si se necesitara este comportamiento habría que hacer un procesamiento posterior a la escritura para unificar los productos base comunes en un 
    /// solo objeto.
    /// </summary>
    class DatosLíneaProducto {


        #region Propiedades de Producto Directas
        // Estas propiedades no tienen conflicto, se escriben y se leen sin problemas en ambas direcciones.

        public int Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public string ProductoReferencia { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia.

        public List<string> ProductoAtributos { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. No se usa private set cómo en SimpleOps.DocumentosGráficos.DatosLíneaProducto porque para la integración se requiere cargar los valores de este DTO desde un archivo JSON, entonces se requiere que el set no sea privado.

        public bool ProductoTieneBase { get; set; }

        #endregion Propiedades de Producto Directas>


        #region Propiedades de Producto Específicas
        // A diferencia de SimpleOps.DocumentosGráficos.DatosLíneaProducto, en este DTO se requiere la escritura de los datos de vuelta a la entidad Producto y cómo no hay manera fácil de controlar el orden en el que son escritos de vuelta, si se almacenaran los valores en propiedades de enlace en este DTO como ProductoUnidad, al estar escribiendo en la entidad producto se podría dar el caso que no se ha cargado aún la propiedad TieneBase y erróneamente (en el caso de los que si tengan producto base) se escribiría la Unidad en UnidadEspecífica cuando se debía escribir en ProductoBase.Unidad. Si fuera necesario, se podría proveer propiedades de solo lectura en este DTO (cómo Unidad) manejando la misma lógica en el Producto para fácil lectura de las propiedades efectivas para este producto, pero al ser un DTO principalmente de transporte (en este momento) que en ambos extremos de su uso siempre se tiene la entidad Producto, por el momento no se implementa. Si se fuera a implementar la posibilidad de tener propiedades efectivas aquí, habría que generar funciones equivalentes a las existentes para la asignación y lectura de las propiedades en la entidad producto y requeriría encapsular esa lógica para ser usada en la entidad producto y en este DTO.

        public string? ProductoDescripciónEspecífica { get; set; }

        public Unidad ProductoUnidadEspecífica { get; set; }

        public double? ProductoPorcentajeIVAPropioEspecífico { get; set; }

        public bool? ProductoExcluídoIVAEspecífico { get; set; }

        public string? ProductoArchivoInformaciónEspecífica { get; set; }

        public string? ProductoArchivoImagenEspecífica { get; set; }

        public List<string> ProductoCaracterísticasEspecíficas { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. No se usa private set cómo en SimpleOps.DocumentosGráficos.DatosLíneaProducto porque para la integración se requiere cargar los valores de este DTO desde un archivo JSON, entonces se requiere que el set no sea privado.

        #endregion Propiedades de Producto Específicas>


        #region Propiedades de ProductoBase
        // Estas propiedades se escriben en ambas direcciones sin problemas, pero se debe tener en cuenta que si se está cargando desde DatosLíneaProducto a Producto una entidad con ProductoBase vacío, se debe establecer manualmente Base en vacío después del mapeado para que la entidad no quede inconsistente.

        public string? ProductoBaseReferencia { get; set; }

        public string? ProductoBaseDescripción { get; set; }

        public Unidad ProductoBaseUnidad { get; set; }

        public double? ProductoBasePorcentajeIVAPropio { get; set; }

        public bool ProductoBaseExcluídoIVA { get; set; }

        public string? ProductoBaseArchivoInformación { get; set; }

        public string? ProductoBaseArchivoImagen { get; set; }

        public List<string> ProductoBaseCaracterísticas { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. No se usa private set cómo en SimpleOps.DocumentosGráficos.DatosLíneaProducto porque para la integración se requiere cargar los valores de este DTO desde un archivo JSON, entonces se requiere que el set no sea privado.

        #endregion Propiedades de ProductoBase>


    } // DatosLíneaProducto>



} // SimpleOps.Integración>
