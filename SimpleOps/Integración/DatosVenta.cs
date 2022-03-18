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



    class DatosVenta : DatosDocumento {


        #region Propiedades

        public string? OrdenCompraNúmero { get; set; }

        public RazónNotaCrédito Razón { get; set; } = RazónNotaCrédito.Otra; // Siempre es Otra para facturas proforma y para facturas de venta. Solo aplica para las notas crédito de venta.

        public string? VentaPrefijo { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        public int? VentaNúmero { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.

        public string? NúmeroDocumentoRecibido { get; set; }

        #endregion Propiedades>


    } // DatosVenta>



} // SimpleOps.Integración>
