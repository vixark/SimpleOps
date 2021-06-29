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
    /// Asocia los contactos a cada proveedor.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ContactoProveedor : Actualizable { // Es Actualizable porque puede cambiar el Tipo. No es de interés conocer la fecha de creación entonces no se hace Rastreable.


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; } // Clave foránea que con ContactoID forman la clave principal.

        public Contacto? Contacto { get; set; } // Obligatorio.
        public int ContactoID { get; set; } // Clave foránea que con ProveedorID forman la clave principal.

        /// <summary>
        /// Desconocido = 0, Vendedor = 1, Despachos = 2, Tesorería = 5, JefeVentas = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255.
        /// </summary>
        public TipoContactoProveedor Tipo { get; set; } = TipoContactoProveedor.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private ContactoProveedor() { } // Solo para que Entity Framework no saque error.

        public ContactoProveedor(Proveedor proveedor, Contacto contacto) 
            => (ProveedorID, ContactoID, Proveedor, Contacto) = (proveedor.ID, contacto.ID, proveedor, contacto);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Contacto, ContactoID)} de {ATexto(Proveedor, ProveedorID)}";

        #endregion Métodos y Funciones>


    } // ContactoProveedor>



} // SimpleOps.Modelo>
