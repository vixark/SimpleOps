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



namespace SimpleOps.Modelo {



    /// <summary>
    /// Referencia propia de la entidad económica (cliente o proveedor) asociada a un producto.
    /// </summary>
    abstract class ReferenciaEntidadEconómica : Actualizable { // Es Actualizable porque la referencia puede cambiar. No es de mucho interés la fecha de creación entonces con ser Actualizable basta.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con ClienteID o ProveedorID forman la clave principal.

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string Valor { get; set; } // Obligatorio.

        #endregion Propiedades>


        #region Constructores

        public ReferenciaEntidadEconómica(Producto? producto, string referencia) 
            => (ProductoID, Valor, Producto) = (Producto.ObtenerID(producto), referencia, producto); 

        #endregion Constructores>


    } // ReferenciaEntidadEconómica>



} // SimpleOps.Modelo>
