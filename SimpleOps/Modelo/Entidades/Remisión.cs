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
    /// Documento temporal de entrega de productos a clientes. 
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class Remisión : Rastreable, ITieneLíneas<LíneaRemisión> { // Si bien una vez hecha la remisión esta no debería aceptar cambios, para efectos de SimpleOps si se generan cambios en su estado. No es únicamente Actualizable porque no dispone de una FechaHora propia que informe de su creación.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public decimal Subtotal { get; set; } // Obligatorio.

        /// <summary>
        /// Pendiente = 0: La remisión se realizó y está pendiente tomar una acción sobre ella. Facturada = 1: Los productos entregados fueron facturados. Los productos ya no están en inventario en consignación ni en el inventario interno, ya son propiedad del cliente porque los compró. Anulada = 2: La remisión se ha anulado. Es equivalente a que nunca se hubiera realizado. Los productos se han devuelto del inventario en consignación al inventario interno. Descartada = 3: Los productos entregados con esta remisión fueron regalados. Los productos ya no están en inventario en consignación del cliente ni en el inventario interno de la empresa. Legalmente las muestras gratis a clientes se deben facturar con precio cero entonces esta función principalmente debe ser usada para dar salida a productos consumidos dentro de la propia empresa, se realiza una remisión a nombre propio y se descarta.
        /// </summary>
        public EstadoRemisión Estado { get; set; } = EstadoRemisión.PendienteFacturación;

        /// <summary>
        /// Información de la transportadora y la guía con la que se entregó. 
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? DetalleEntrega { get; set; }

        /// <summary>
        /// La venta que facturó la remisión. Si aún no se ha facturado, es nula.
        /// </summary>
        public Venta? Venta { get; set; }
        public int? VentaID { get; set; }

        public OrdenCompra? OrdenCompra { get; set; }
        public int? OrdenCompraID { get; set; }

        public List<LíneaRemisión> Líneas { get; set; } = new List<LíneaRemisión>();

        #endregion Propiedades>


        #region Constructores

        private Remisión() { } // Solo para que Entity Framework no saque error.

        public Remisión(decimal subtotal, Cliente cliente) => (Subtotal, ClienteID, Cliente) = (subtotal, cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // Remisión>



} // SimpleOps.Modelo>
