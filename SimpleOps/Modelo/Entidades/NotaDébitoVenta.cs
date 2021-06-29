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
    /// Cargo al valor de una venta.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaDébitoVenta : Factura<Cliente, LíneaNotaDébitoVenta> {


        #region Propiedades

        [NotMapped]
        public override Cliente? EntidadEconómica => Cliente;

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public Venta? Venta { get; set; } // Obligatorio.
        public int VentaID { get; set; }

        public override List<LíneaNotaDébitoVenta> Líneas { get; set; } = new List<LíneaNotaDébitoVenta>();

        /// <summary>
        /// Intereses = 1, Gastos = 2, AjustePrecio = 3, Otra = 4.
        /// </summary>
        public RazónNotaDébito Razón { get; set; } = RazónNotaDébito.Otra;

        #endregion Propiedades>


        #region Constructores

        private NotaDébitoVenta() { } // Solo para que Entity Framework no saque error.

        public NotaDébitoVenta(Cliente cliente, Venta venta) {
            (ClienteID, VentaID, Cliente, Venta) = (cliente.ID, venta.ID, cliente, venta);
            VerificarDatosEntidad();
        } // NotaDébitoVenta>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public DateTime? FechaVencimiento => Cliente == null ? (DateTime?)null : FechaHora.AddDays(Cliente.DíasCrédito);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Cliente, ClienteID)}";

        public override void VerificarDatosEntidad() => Cliente.VerificarDatosVenta(Cliente);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => Empresa.PinAplicación;

        #endregion Métodos y Funciones>


    } // NotaDébitoVenta>



} // SimpleOps.Modelo>
