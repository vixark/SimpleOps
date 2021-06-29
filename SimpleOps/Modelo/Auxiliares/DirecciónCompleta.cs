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
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Incluye el municipio y la dirección. No se usa en las propiedades que se escriben en la base de datos pero si en propiedades de solo lectura.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class DirecciónCompleta {


        #region Propiedades

        public Municipio Municipio { get; set; }

        public string Dirección { get; set; }

        #endregion  Propiedades>


        #region Constructores

        public DirecciónCompleta(Municipio municipio, string dirección) => (Municipio, Dirección) = (municipio, dirección);

        #endregion Constructores>


        #region Métodos y Funciones

        /// <summary>
        /// Si no se dispone del municipio o de la dirección no se puede crear la dirección completa y esta será nula.
        /// </summary>
        public static DirecciónCompleta? CrearDirecciónCompleta(Municipio? municipio, string? dirección) 
            => municipio == null || dirección == null ? null : new DirecciónCompleta(municipio, dirección);

        #endregion Métodos y Funciones>


    } // DirecciónCompleta>



} // SimpleOps.Modelo>
