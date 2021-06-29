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

using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;



namespace SimpleOps.DocumentosGráficos {



    public class DatosVenta : DatosDocumento, IConLíneasProductos { // Común para ventas y para notas crédito de ventas.


        #region Propiedades

        public string? Cude { get; set; } // Puede ser nulo para las facturas proforma.

        public string? QR { get; set; } // Puede ser nulo para las facturas proforma.

        public DateTime? FechaVencimiento { get; set; } // Puede ser nulo para las facturas proforma.

        public string? OrdenCompraNúmero { get; set; }

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; } = new OpcionesColumnas();

        public string? SubtotalBaseTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? IVATexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? SubtotalFinalConImpuestosTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? ImpuestoConsumoTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? DescuentoCondicionadoTexto { get; set; } // Puede ser nulo para las facturas proforma.

        public string? RazónNotaCréditoTexto { get; set; } // Puede ser nulo para facturas proforma y para facturas de venta. Solo aplica para las notas crédito de venta.

        public string? NombreCude { get; set; } // Puede ser nulo para las facturas proforma.

        public string? CódigoVenta { get; set; } // Puede ser nulo para las facturas proforma y para ventas.

        #endregion Propiedades>


    } // DatosVenta>



} // SimpleOps.DocumentosGráficos>
