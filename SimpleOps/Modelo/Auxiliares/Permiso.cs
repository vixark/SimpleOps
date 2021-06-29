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
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    [ControlInserción(ControlConcurrencia.Ninguno)]
    class Permiso {


        #region Propiedades

        /// <summary>
        /// Ninguno = -1, Lectura = 1, Modificación = 2, Inserción = 4, Eliminación = 8. Los valores son sumables, es decir, para un permiso de Lectura, Modificación y Eliminación pero no de Inserción, el valor es: 1 + 2 + 8 = 11.
        /// </summary>
        public TipoPermiso Tipo { get; set; } = TipoPermiso.Ninguno;

        /// <summary>
        /// Puede ser Todas para aplicar permisos iniciales.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Tabla { get; set; } = null!; // Obligatorio.

        /// <summary>
        /// Si es nulo el permiso aplica para toda la tabla. Si no es nulo el permiso aplica para estas columnas.
        /// </summary>
        public List<string>? Columnas { get; set; }

        #endregion Propiedades>


        #region Constructores

        public Permiso(TipoPermiso tipo, string tabla, List<string>? columnas) => (Tipo, Tabla, Columnas) = (tipo, tabla, columnas); // Requiere Global.Permiso para que no entre en conflicto con Modelo.Permiso.

        private Permiso() { } // Necesario para que no saque error la serialización.

        #endregion Constructores>


    } // Permiso>



} // SimpleOps.Modelo>
