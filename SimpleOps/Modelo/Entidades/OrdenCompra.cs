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
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Solicitud de producto de un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class OrdenCompra : SolicitudProducto {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        /// <summary>
        /// Código alfanumérico identificador de la orden de compra. Único para cada cliente.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)] 
        public string Número { get; set; } = null!; // Obligatorio. Se nombra número por que es un ID que usualmente es un número así no siempre lo sea.

        public Sede? Sede { get; set; } 
        public int? SedeID { get; set; }

        public bool EnviadaProforma { get; set; }

        public bool Remisionar { get; set; }

        /// <summary>
        /// Para implementaciones personalizadas. Determina si la información de esta orden de compra ya ha sido sincronizada con la página web.
        /// </summary>
        public bool SincronizadaWeb { get; set; }

        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Se toma de la prioridad del cliente en el momento de la creación de la orden de compra.
        /// </summary>
        public Prioridad Prioridad { get; set; } = Prioridad.Desconocida; // Obligatorio.

        /// <summary>
        /// Si es nulo el pago aún no ha sido confirmado.
        /// </summary>
        public InformePago? InformePago { get; set; }
        public int? InformePagoID { get; set; }

        [NotMapped]
        public EstadoOrdenCompra EstadoOrdenCompra { get; set; } = EstadoOrdenCompra.Lista; // No se guarda en la base de datos porque se calcula en el momento.

        public List<LíneaOrdenCompra> Líneas { get; set; } = new List<LíneaOrdenCompra>();

        #endregion Propiedades>


        #region Constructores

        private OrdenCompra() { } // Solo para que Entity Framework no saque error.

        public OrdenCompra(Cliente cliente, string número) 
            => (ClienteID, Número, Prioridad, Cliente) = (cliente.ID, número, cliente.Prioridad, cliente);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public bool PagoConfirmado => InformePagoID != null;

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Número} de {ATexto(Cliente, ClienteID)}"; // El único que lleva 'de' porque la orden de compra es del cliente.

        #endregion Métodos y Funciones>


    } // OrdenCompra>



} // SimpleOps.Modelo>
