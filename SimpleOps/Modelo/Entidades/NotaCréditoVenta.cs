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
using SimpleOps.DocumentosGráficos;
using AutoMapper;
using System.IO;
using static SimpleOps.DocumentosGráficos.DocumentosGráficos;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Descuento al valor de una venta por devolución de productos.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaCréditoVenta : Factura<Cliente, LíneaNotaCréditoVenta> {


        #region Propiedades

        [NotMapped]
        public override Cliente? EntidadEconómica => Cliente;

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public Venta? Venta { get; set; } // Obligatorio.
        public int VentaID { get; set; }

        public override List<LíneaNotaCréditoVenta> Líneas { get; set; } = new List<LíneaNotaCréditoVenta>();

        /// <summary>
        /// DevoluciónParcial = 1, AnulaciónFactura = 2, Descuento = 3, AjustePrecio = 4, Otra = 5.
        /// </summary>
        public RazónNotaCrédito Razón { get; set; } = RazónNotaCrédito.Otra;

        #endregion Propiedades>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos.
        /// </summary>
        public string RazónNotaCréditoTexto => Razón.ATexto();

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos.
        /// </summary>
        public string CódigoVenta => Venta?.Código!; // Se asegura que nunca será nulo porque Venta nunca lo será.

        #endregion Propiedades Autocalculadas>


        #region Constructores

        private NotaCréditoVenta() { } // Solo para que Entity Framework no saque error.

        public NotaCréditoVenta(Cliente cliente, Venta venta) {
            (ClienteID, VentaID, Cliente, Venta) = (cliente.ID, venta.ID, cliente, venta);
            VerificarDatosEntidad();
        } // NotaCréditoVenta>

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Cliente, ClienteID)}";

        public override void VerificarDatosEntidad() => Cliente.VerificarDatosVenta(Cliente);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => Empresa.PinAplicación;


        public DatosVenta ObtenerDatos(OpcionesDocumento opcionesDocumento) {

            var mapeador = new Mapper(ConfiguraciónMapeadorNotaCréditoVenta);
            var datos = mapeador.Map<DatosVenta>(this);
            datos.NombreDocumento = "Nota Crédito";
            if (datos.CódigoDocumento.EmpiezaPorNúmero()) datos.PrefijoNombreArchivo = Legal.Dian.PrefijoNotasCréditoPredeterminado; // Solo se agrega el prefijo cuando inicia por número y puede haber riesgo de colisión con otros documentos.
            CompletarDatosVenta(opcionesDocumento, datos, Líneas);
            return datos;

        } // ObtenerDatos>


        public Integración.DatosVenta ObtenerDatosIntegración() {

            var mapeador = new Mapper(ConfiguraciónMapeadorNotaCréditoVentaIntegración);
            var datos = mapeador.Map<Integración.DatosVenta>(this);
            return datos;

        } // ObtenerDatosIntegración>


        #endregion Métodos y Funciones>


    } // NotaCréditoVenta>



} // SimpleOps.Modelo>
