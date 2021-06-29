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



namespace SimpleOps.Modelo {



    /// <summary>
    /// PrecioCliente o PrecioProveedor.
    /// </summary>
    abstract class Precio : Actualizable { // Es Actualizable porque el precio puede cambiar. No es necesario que sea Rastreable porque el registro de los precios cotizados a clientes se llevará en la tabla Cotizaciones. Para los precios de proveedores por el momento no se considera necesario pero si lo fuera se puede crear una tabla CotizacionesProveedores.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con ClienteID o ProveedorID forman la clave principal.

        public decimal Valor { get; set; } // Obligatorio.

        /// <summary>
        /// Permite proteger el precio para solo permitir su modificación a usuarios que tengan permiso de modificar esta columna (Protegido). En PrecioLista se podría usar para permitir algunos roles actualizar los precios de lista de los no protegidos y a otros roles también los protegidos.
        /// </summary>
        public bool Protegido { get; set; } = false;

        #endregion Propiedades>


        #region Constructores

        public Precio(Producto? producto, decimal valor) => (ProductoID, Valor, Producto) = (Producto.ObtenerID(producto), valor, producto); 

        #endregion Constructores>


    } // Precio>


} // SimpleOps.Modelo>
