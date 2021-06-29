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
    /// Comprobante de entrada de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class ReciboCaja : ComprobanteDinero {


        #region Propiedades

        public List<Venta> Ventas { get; set; } = new List<Venta>();

        #endregion Propiedades>


        #region Constructores

        private ReciboCaja() : base(LugarMovimientoDinero.Desconocido) { } // Solo para que Entity Framework no saque error.

        public ReciboCaja(Proveedor proveedor, LugarMovimientoDinero lugar) : base(proveedor, lugar) { }

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar) : base(cliente, lugar) { }

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar, decimal valorFacturas) : this(cliente, lugar) 
            => (ValorFacturas) = (valorFacturas);

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar, decimal valorFacturas, decimal abono) : this(cliente, lugar, valorFacturas)
            => (Abono) = (abono);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}{ATexto(Proveedor, ProveedorID)}"; // Se agregan juntos en la cadena porque nunca se presenta el caso que tenga tanto Cliente como Proveedor.

        #endregion Métodos y Funciones>


    } // ReciboCaja>



} // SimpleOps.Modelo>
