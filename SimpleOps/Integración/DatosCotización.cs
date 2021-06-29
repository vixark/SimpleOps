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



    class DatosCotización : DatosDocumento {


        #region Propiedades

        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.

        public int TamañoImágenes { get; set; }

        public string? CondicionesComerciales { get; set; }

        public string? EnlaceDescargaXlsx { get; set; }

        public List<string> ReferenciasProductosPáginasExtra { get; set; } = new List<string>(); // No se puede usar private set; porque el automapper requiere un set público para funcionar correctamente. Referencias de los productos que se añadirán a las páginas extra del catálogo. Estas referencias pueden ser de productos base o de productos específicos. Si se da el caso que un producto base tiene la misma referencia que uno específico, se prefiere el base. Se manejan por fuera de DatosLíneaProducto/Producto porque son datos que solo le corresponden a la generación del catálogo y no es necesario agregarlos a la tabla Productos para uso general.

        public int? CantidadFilasProductos { get; set; } 

        public int? CantidadColumnasProductos { get; set; } 

        public int? ÍndiceInversoInserciónPáginasExtra { get; set; }

        public string? ContactoNombre { get; set; }

        public string? ContactoTeléfono { get; set; }

        public string? ContactoEmail { get; set; } // Puede ser nulo porque el contacto puede ser nulo.

        #endregion Propiedades>


    } // DatosCotización>



} // SimpleOps.Integración>
