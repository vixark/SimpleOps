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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Persona autorizada para usar SimpleOps.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Usuario : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio.

        public bool EsRepresentanteComercial { get; set; }

        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string Email { get; set; } = null!; // Obligatorio. Necesario para enviar notificaciones e informes.

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? Teléfono { get; set; }

        /// <summary>
        /// Si es falso el usuario no puede iniciar la aplicación.
        /// </summary>
        public bool Activo { get; set; } = true;

        #endregion Propiedades>


        #region Constructores

        private Usuario() { } // Solo para que Entity Framework no saque error.

        public Usuario(string nombre, string email) => (Nombre, Email) = (nombre, email);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Usuario>



} // SimpleOps.Modelo>
