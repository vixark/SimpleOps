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



namespace SimpleOps.Modelo {



    /// <summary>
    /// Precio dado a cliente. Se diferencia de PrecioCliente en que la tabla Cotizaciones es un registro histórico mientras que la tabla PreciosClientes tiene solo los últimos precios.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)] 
    class LíneaCotización { // No necesitan ser rastreadas porque son entidades de tipo Línea[EntidadPadre] que se crean en una sola operación con la EntidadPadre (Cotización) entonces su información de creación es la misma. Además, no admite más cambios después de creada por lo que no hay necesidad de crear propiedades para la información de actualización. No se admite actualización ni anulación de una cotización, solo admite recotización que se hace con una cotización nueva.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } 

        public decimal Precio { get; set; } // Obligatorio.

        public Cotización? Cotización { get; set; } // Obligatorio.
        public int CotizaciónID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero().
        /// </summary>
        public string PrecioTexto => Precio.ATextoDinero(agregarMoneda: true);

        #endregion Propiedades Autocalculadas>


        #region Constructores

        private LíneaCotización() { } // Solo para que Entity Framework no saque error.

        public LíneaCotización(Cotización cotización, Producto producto, decimal precio) 
            => (Cotización, CotizaciónID, ProductoID, Producto, Precio) = (cotización, cotización.ID, producto.ID, producto, precio);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Cotización, CotizaciónID)} de {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // LíneaCotización>



} // SimpleOps.Modelo>
