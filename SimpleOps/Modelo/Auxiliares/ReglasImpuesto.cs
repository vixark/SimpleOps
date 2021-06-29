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
using System.Text;



namespace SimpleOps.Modelo {



    class ReglasImpuesto {


        #region Propiedades

        /// <summary>
        /// Si la suma de los <see cref="MovimientoProducto.SubtotalBase"/> es mayor a este mínimo, se aplica la retención.
        /// </summary>
        public decimal Mínimo { get; set; }

        /// <summary>
        /// Guardado como fracción. Puede ser un porcentaje con respecto a cualquier base, como SubtotalBase o IVA, porque la base se puede proveer 
        /// a <see cref="Factura{E, M}.ObtenerRetención{C}(Func{C, ReglasImpuesto}, Func{M, C}, Func{M, decimal})"/>
        /// </summary>
        public decimal Porcentaje { get; set; }

        #endregion Propiedades>


        #region Constructores

        public ReglasImpuesto(decimal porcentaje, decimal mínimo) => (Mínimo, Porcentaje) = (mínimo, porcentaje);

        #endregion Constructores>


    } // ReglasImpuesto>



} // SimpleOps.Modelo>
