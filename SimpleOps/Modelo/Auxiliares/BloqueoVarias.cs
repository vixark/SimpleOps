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
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Clase auxiliar que sirve como intermediaria cuando se necesita hacer el mismo bloqueo para varias entidades del mismo tipo.
    /// El ID heredado no tiene significado en esta clase.
    /// </summary>
    class BloqueoVarias : Bloqueo {


        #region Propiedades

        public List<int>? IDs { get; set; }

        public BloqueoVarias(string nombreEntidad) : base(nombreEntidad, 0) { } // Al ser una entidad auxiliar no se necesita obligar el usuarioID en el constructor para que cumpla su función. Cuando se copian los datos de BloqueoVarias a Bloqueo se escribe el usuarioID desde Global.UsuarioActual.

        public BloqueoVarias(Bloqueo bloqueo) : this(bloqueo.NombreEntidad) => bloqueo.CopiarA(this);

        #endregion Propiedades>


    } // BloqueoVarias>



} // SimpleOps.Modelo>
