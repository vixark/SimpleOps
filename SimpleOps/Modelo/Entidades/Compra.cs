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
    /// Recepción de productos a cambio de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class Compra : Factura<Proveedor, LíneaCompra> {


        #region Propiedades

        [NotMapped]
        public override Proveedor? EntidadEconómica => Proveedor;

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        public override List<LíneaCompra> Líneas { get; set; } = new List<LíneaCompra>();

        public ComprobanteEgreso? ComprobanteEgreso { get; set; }
        public int? ComprobanteEgresoID { get; set; }

        public Pedido? Pedido { get; set; } 
        public int? PedidoID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Compra() { } // Solo para que Entity Framework no saque error.

        public Compra(Proveedor proveedor, Pedido pedido) : this(proveedor) => (PedidoID, Pedido) = (pedido.ID, pedido);

        public Compra(Proveedor proveedor) { 
            (ProveedorID, Proveedor) = (proveedor.ID, proveedor);
            VerificarDatosEntidad();
        } // Compra>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public DateTime? FechaVencimiento => Proveedor == null ? (DateTime?)null : FechaHora.AddDays(Proveedor.DíasCrédito);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Proveedor, ProveedorID)}";

        public override void VerificarDatosEntidad() => Proveedor.VerificarDatosCompra(Proveedor);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => "?"; // No se conoce para el proveedor ni habría que conocerlo porque nunca se requiere calcular el CUDE para documentos del proveedor. Este método se agrega solo por compatibilidad con la clase Factura.

        #endregion Métodos y Funciones>


    } // Compra>



} // SimpleOps.Modelo>
