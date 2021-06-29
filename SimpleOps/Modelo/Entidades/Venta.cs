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

using AutoMapper;
using iText.Html2pdf;
using iText.Kernel;
using iText.Kernel.Pdf;
using RazorEngineCore;
using SimpleOps.Legal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using SimpleOps.DocumentosGráficos;
using static Vixark.General;
using static SimpleOps.Global;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entrega de productos a cambio de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class Venta : Factura<Cliente, LíneaVenta> {


        #region Propiedades

        [NotMapped]
        public override Cliente? EntidadEconómica => Cliente;

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        /// <summary>
        /// La fecha del pago de la comisión a la venta al representante comercial. Si es nula no se ha pagado.
        /// </summary>
        public DateTime? FechaPagoComisiónEnVenta { get; set; }

        /// <summary>
        /// La fecha del pago de la comisión a al pago del cliente al representante comercial. Si es nula no se ha pagado.
        /// </summary>
        public DateTime? FechaPagoComisiónEnPago { get; set; }

        /// <summary>
        /// Información de la transportadora y la guía con la que se entregó.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? DetalleEntrega { get; set; }

        /// <summary>
        /// Si es verdadero se están vendiendo productos que el cliente tiene en consignación y no del inventario del almacén propio.
        /// </summary>
        public bool DeInventarioConsignación { get; set; }

        public override List<LíneaVenta> Líneas { get; set; } = new List<LíneaVenta>();

        public ReciboCaja? ReciboCaja { get; set; }
        public int? ReciboCajaID { get; set; }

        public InformePago? InformePago { get; set; }
        public int? InformePagoID { get; set; }

        public OrdenCompra? OrdenCompra { get; set; }
        public int? OrdenCompraID { get; set; }

        public List<Remisión> Remisiones { get; set; } = new List<Remisión>(); // Se puede hacer una sola factura por varias remisiones entregadas.

        #endregion Propiedades>


        #region Constructores

        private Venta() { } // Solo para que Entity Framework no saque error.

        public Venta(Cliente cliente, OrdenCompra ordenCompra) : this(cliente) => (OrdenCompraID, OrdenCompra) = (ordenCompra.ID, ordenCompra);

        public Venta(Cliente cliente) {
            (ClienteID, Cliente) = (cliente.ID, cliente);
            VerificarDatosEntidad();
        } // Venta>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public DateTime? FechaVencimiento => Cliente == null ? (DateTime?)null : FechaHora.AddDays(Cliente.DíasCrédito);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Cliente, ClienteID)}";

        /// <summary>
        /// Si no se dispone de una orden de compra con una sede de entrega la entrega se hace en la dirección del cliente.
        /// </summary>
        public DirecciónCompleta? DirecciónCompletaEntrega => OrdenCompra?.Sede?.DirecciónCompleta ?? Cliente?.DirecciónCompleta;

        public PagoTransporte PagoTransporte => DirecciónCompletaEntrega == null || Cliente == null ? PagoTransporte.Desconocido : 
            (Subtotal >= Cliente.MínimoTransporteGratis) ? PagoTransporte.Gratis : PagoTransporte.Contraentrega;

        public override void VerificarDatosEntidad() => Cliente.VerificarDatosVenta(Cliente);

        public override string? ObtenerClaveParaCude() => Empresa.ClaveTécnicaAplicación;


        public override decimal ObtenerAnticipo() {

            if (InformePago != null) {

                var anticipo = InformePago.Valor;
                if (anticipo > APagar) anticipo = APagar; // Se establece esta regla únicamente porque la DIAN impide que el valor a pagar definitivo sea menor que cero. Pero en la operación normal queda un saldo a favor del cliente.
                return anticipo;

            } else {
                return 0;
            }

        } // ObtenerAnticipo>


        public DatosVenta ObtenerDatos(OpcionesDocumento opcionesDocumento) {

            var mapeador = new Mapper(ConfiguraciónMapeadorVenta);
            var datos = mapeador.Map<DatosVenta>(this);
            datos.NombreDocumento = "Factura";
            CompletarDatosVenta(opcionesDocumento, datos, Líneas);
            return datos;

        } // ObtenerDatos>


        public Integración.DatosVenta ObtenerDatosIntegración() {

            var mapeador = new Mapper(ConfiguraciónMapeadorVentaIntegración);
            var datos = mapeador.Map<Integración.DatosVenta>(this);
            return datos;

        } // ObtenerDatosIntegración>


        #endregion Métodos y Funciones>


    } // Venta>



} // SimpleOps.Modelo>
