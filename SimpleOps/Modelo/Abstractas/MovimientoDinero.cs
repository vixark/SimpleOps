﻿// Copyright Notice:
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
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Bancario o Efectivo.
    /// </summary>
    abstract class MovimientoDinero : Actualizable { // Es Actualizable porque se pueden realizar cambios en su estado y observaciones. No es Rastrable porque dispone de una FechaHora propia que indicaría cuando se hizo el movimiento de dinero. Una FechaHoraCreación no daría mucho valor adicional porque en el caso de los movimientos bancarios indicaría cuando se cargaron los movimientos desde el banco, lo cual no sirve mucho y en el caso de los movimientos en efectivo es el mismo valor de FechaHora, entonces sería redundante.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Fecha y hora del movimiento. Puede no coincidir con FechaHoraCreación porque puede ser un movimiento de dinero de caja del día anterior o un movimiento de dinero cargado del banco.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Positivo si el dinero entra a las cuentas de la empresa, negativo si sale.
        /// </summary>
        public decimal Valor { get; set; } // Obligatorio.

        public ReciboCaja? ReciboCaja { get; set; }
        public int? ReciboCajaID { get; set; }

        public ComprobanteEgreso? ComprobanteEgreso { get; set; }
        public int? ComprobanteEgresoID { get; set; }

        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        /// <summary>
        /// Pendiente = 0. Procesado = 1: Si al movimiento de dinero ya se le realizó el comprobante de egreso o recibo de caja correspondiente. Dividido = 2: Si el movimiento de dinero (típicamente bancario) se ha dividido en varias líneas hijas. Anulado = 3: Si el movimiento de dinero no existió, es útil en el caso de movimientos de efectivo cuando se anula un comprobante de dinero y se desea repetir realizando otra operación diferente a la inicial (abonando dinero o cancelando otras facturas).
        /// </summary>
        public EstadoMovimientoDinero Estado { get; set; } = EstadoMovimientoDinero.Pendiente;

        #endregion Propiedades>


        #region Constructores

        public MovimientoDinero(DateTime fechaHora, decimal valor) => (FechaHora, Valor) = (fechaHora, valor);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public decimal ValorAbsoluto => Math.Abs(Valor);

        public TipoMovimientoDinero Tipo 
            => Valor > 0 ? TipoMovimientoDinero.Ingreso : (Valor < 0 ? TipoMovimientoDinero.Egreso : TipoMovimientoDinero.Ninguno);

        #endregion Propiedades Autocalculadas>


    } // MovimientoDinero>



} // SimpleOps.Modelo>
