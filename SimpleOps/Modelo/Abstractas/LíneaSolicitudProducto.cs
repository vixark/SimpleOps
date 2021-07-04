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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de Órdenes de Compra (Clientes) o Pedidos (Proveedores).
    /// </summary>
    abstract class LíneaSolicitudProducto : Rastreable { // Es Rastreable porque es una entidad modificable y además es necesario disponer de la fecha de creación para calcular los días de cumplimiento.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con OrdenCompraID o PedidoID forma la clave principal.

        public decimal Precio { get; set; } // Obligatorio.

        public int Cantidad { get; set; } // Obligatorio.

        public int CantidadEntregada { get; set; } 

        public DateTime? FechaHoraCumplimiento { get; set; }

        /// <summary>
        /// Pendiente = 0, Cumplida = 1, Anulada = 2. Puede ser anulado o cumplido individualmente sin anular o cumplir la solicitud completa. No se hace autocalculada para poder mantener los datos de la solucitud al momento de la anulación.
        /// </summary>
        public EstadoSolicitudProducto Estado { get; set; } = EstadoSolicitudProducto.Pendiente;

        /// <MaxLength>200</MaxLength>
        [MaxLength(200)]
        public Dictionary<string, string> Personalizaciones { get; set; } = new Dictionary<string, string>();

        #endregion Propiedades>


        #region Constructores

        public LíneaSolicitudProducto(Producto? producto, int cantidad, decimal precio)
            => (ProductoID, Cantidad, Precio, Producto) = (Producto.ObtenerID(producto), cantidad, precio, producto); 

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Si fue cumplido con la cantidad requerida y en una fecha específica.
        /// </summary>
        public bool Cumplido => (CantidadEntregada >= Cantidad) && (FechaHoraCumplimiento != null);

        /// <summary>
        /// Si aún está pendiente de entrega. No está pendiente si fue anulado o si fue cumplido.
        /// </summary>
        public bool Pendiente => !Cumplido && (Estado != EstadoSolicitudProducto.Anulada);

        public int CantidadPendiente => Pendiente ? Cantidad - CantidadEntregada : 0;

        public double? DíasCumplimiento => (FechaHoraCumplimiento - FechaHoraCreación)?.TotalDays;

        #endregion Propiedades Autocalculadas>


    } // LíneaSolicitudProducto>



} // SimpleOps.Modelo>
