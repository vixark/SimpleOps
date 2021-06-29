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



namespace SimpleOps.DocumentosGráficos {



    /// <summary>
    /// DTO de lectura. Adecuado solo para escribir desde Producto a DatosLíneaProducto. Si se requiere un DTO para escribir en ambas direcciones, se 
    /// puede usar <see cref="Integración.DatosLíneaProducto"/>.
    /// </summary>
    public class DatosLíneaProducto {


        #region Propiedades

        public int Cantidad { get; set; }

        public decimal PrecioBase { get; set; } // Se pasa de manera redundante con PrecioBaseTexto para dar opción al usuario del código de usar el valor que prefiera al diseñar el HTML. Puede elegir PrecioBase (numérico) para presentarlo como desee o PrecioBaseTexto con el formato predeterminado de moneda en la configuración de SimpleOps.

        public string? PrecioBaseTexto { get; set; }

        public string? IVATexto { get; set; }

        public string? ImpuestoConsumoTexto { get; set; }

        public string? SubtotalBaseTexto { get; set; }

        public string? SubtotalBaseConImpuestosTexto { get; set; }

        public string ProductoReferencia { get; set; } = null!; // Nunca es nulo, solo es para que no saque advertencia.

        /// <summary>
        /// Descripción del producto completa incluyendo los atributos. Para cambiarla, cambia <see cref="Modelo.Producto.DescripciónBase"/>
        /// y <see cref="Modelo.Producto.Atributos"/> en el <see cref="Modelo.Producto"/> de origen.
        /// </summary>
        public string? ProductoDescripción { get; private set; } // Se usa private set porque la propiedad Descripción de Producto es de solo lectura. Se necesita el set para poder ser escrito desde el Automapper, pero al ser privado no podrá ser escrito por el usuario del código una vez cargado. A diferencia de SimpleOps.Integración.DatosLíneaProducto este DTO no requiere escribir de vuelta los datos en el objeto original.

        public string? ProductoDescripciónBase { get; set; }

        public List<string> ProductoAtributos { get; private set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. Se usa private set porque la colección no debería ser reemplazada por otra, solo sus ítems.

        public string? ProductoUnidadTexto { get; set; }

        public string? ProductoArchivoInformación { get; set; }

        public string? ProductoArchivoImagen { get; set; }

        public List<string> ProductoCaracterísticas { get; private set; } = null!; // Nunca es nulo, solo es para que no saque advertencia. Se usa private set porque la colección no debería ser reemplazada por otra, solo sus ítems.

        #endregion Propiedades>


    } // DatosLíneaProducto>



} // SimpleOps.DocumentosGráficos>
