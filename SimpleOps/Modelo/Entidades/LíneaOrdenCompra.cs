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
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de una orden de compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaOrdenCompra : LíneaSolicitudProducto {


        #region Propiedades

        public OrdenCompra? OrdenCompra { get; set; } // Obligatorio.
        public int OrdenCompraID { get; set; } // Clave foránea que con ProductoID forma la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaOrdenCompra() : base(null, 0, 0) { } // Solo para que Entity Framework no saque error.

        public LíneaOrdenCompra(OrdenCompra ordenCompra, Producto producto, int cantidad, decimal precio) : base(producto, cantidad, precio) 
            => (OrdenCompraID, OrdenCompra) = (ordenCompra.ID, ordenCompra);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(OrdenCompra, OrdenCompraID)} por {ATexto(Producto, ProductoID)}"; // Es el único que lleva 'por' porque la orden de compra es del cliente y lleva 'de' su ToString(), entonces se prefiere 'por' para que no sea muy redundante.

        #endregion Métodos y Funciones>


    } // LíneaOrdenCompra>



} // SimpleOps.Modelo>
