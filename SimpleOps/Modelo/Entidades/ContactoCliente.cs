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
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {


    /// <summary>
    /// Asocia los contactos a cada cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ContactoCliente : Actualizable { // Es Actualizable porque puede cambiar el Tipo. No es de interés conocer la fecha de creación entonces no se hace Rastreable.


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ContactoID forman la clave principal.

        public Contacto? Contacto { get; set; } // Obligatorio.
        public int ContactoID { get; set; } // Clave foránea que con ClienteID forman la clave principal.

        /// <summary>
        /// Observaciones adicionales a Cliente.ObservacionesFactura que se escribirán en el campo de observaciones de la factura y se mostrarán en la interfaz al hacer una factura a la empresa de este contacto.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)] 
        public string? ObservacionesFactura { get; set; }

        /// <summary>
        /// Desconocido = 0, Comprador = 1, Almacenista = 2, Tesorería = 5, JefeCompras = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255.
        /// </summary>
        public TipoContactoCliente Tipo { get; set; } = TipoContactoCliente.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private ContactoCliente() { } // Solo para que Entity Framework no saque error. 

        public ContactoCliente(Cliente cliente, Contacto contacto) 
            => (ClienteID, ContactoID, Cliente, Contacto) = (cliente.ID, contacto.ID, cliente, contacto);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Contacto, ContactoID)} de {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // ContactoCliente>



} // SimpleOps.Modelo>
