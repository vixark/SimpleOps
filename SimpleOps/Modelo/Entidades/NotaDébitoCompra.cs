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
    /// Cargo al valor de una compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaDébitoCompra : Factura<Proveedor, LíneaNotaDébitoCompra> {


        #region Propiedades

        [NotMapped]
        public override Proveedor? EntidadEconómica => Proveedor;

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        public Compra? Compra { get; set; } // Obligatorio.
        public int CompraID { get; set; }

        public override List<LíneaNotaDébitoCompra> Líneas { get; set; } = new List<LíneaNotaDébitoCompra>();

        /// <summary>
        /// Intereses = 1, Gastos = 2, AjustePrecio = 3, Otra = 4.
        /// </summary>
        public RazónNotaDébito Razón { get; set; } = RazónNotaDébito.Otra;

        #endregion Propiedades>


        #region Constructores

        private NotaDébitoCompra() { } // Solo para que Entity Framework no saque error.

        public NotaDébitoCompra(Proveedor proveedor, Compra compra) {
            (ProveedorID, CompraID, Proveedor, Compra) = (proveedor.ID, compra.ID, proveedor, compra);
            VerificarDatosEntidad();
        } // NotaDébitoCompra>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Proveedor, ProveedorID)}";

        public override void VerificarDatosEntidad() => Proveedor.VerificarDatosCompra(Proveedor);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => "?"; // No se conoce para el proveedor ni habría que conocerlo porque nunca se requiere calcular el CUDE para documentos del proveedor. Este método se agrega solo por compatibilidad con la clase Factura.

        #endregion Métodos y Funciones>


    } // NotaDébitoCompra>



} // SimpleOps.Modelo>
