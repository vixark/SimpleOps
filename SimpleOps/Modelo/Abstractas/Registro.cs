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



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad que representa un registro histórico. Estas entidades solo se crean no se actualizan, por lo tanto no tienen propiedades para rastrear la fecha de su actualización ni su actualizador. Tienen ID como clave única y no tienen restricciones adicionales sobre las otras columnas.
    /// </summary>
    abstract class Registro : IRegistro {


        #region Propiedades

        [Key]
        public int ID { get; set; }

        public int CreadorID { get; set; } // Para evitar complejizar innecesariamente las relaciones de cada tabla en la base de datos no se hará una propiedad de navegación al usuario creador. Estos registros son informativos y además es fácil obtener el usuario a partir del ID cuando se requiera.

        public DateTime FechaHoraCreación { get; set; }

        #endregion Propiedades


        #region Métodos

        public abstract override string ToString(); // Obliga a las entidades derivadas a implementar un método ToString() propio para describirse de una manera entendible para el usuario. También se podría hacer a nivel de interface (ver https://stackoverflow.com/a/50930292/8330412) pero habría que crear otro método entonces se prefiere hacer por clase abstracta.

        #endregion Métodos>


    } // Registro>



} // SimpleOps.Modelo>
