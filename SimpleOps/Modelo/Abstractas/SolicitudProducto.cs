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
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Orden de Compra (Clientes) o Pedido (Proveedores).
    /// </summary>
    abstract class SolicitudProducto : Actualizable { // Es Actualizable porque su estado puede cambiar. No es Rastreable porque aunque la fecha de creación es importante tenerla para usos varios, esta se puede obtener de la fecha de creación de alguna de sus LíneaSolicitudProducto, pues ambas entidades son creadas como una sola operación. Es un caso inverso al de MovimientoProducto-Factura.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        public Contacto? Contacto { get; set; } 
        public int? ContactoID { get; set; }

        /// <summary>
        /// Pendiente = 0, Cumplida = 1, Anulada = 2.
        /// </summary>
        public EstadoSolicitudProducto Estado { get; set; } = EstadoSolicitudProducto.Pendiente;

        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        /// <summary>
        /// No se almacena en la base de datos porque es información redundante con LíneaSolicitudProducto.FechaHoraCreación y evitar incrementar el 
        /// tamaño de la base de datos innecesariamente. Se puede escribir cuando se necesite desde la menor FechaHoraCreación de sus LíneaSolicitudProducto.
        /// </summary>
        [NotMapped]
        public DateTime? FechaHoraCreación { get; set; }

        #endregion Propiedades>


    } // SolicitudProducto>



} // SimpleOps.Modelo>
