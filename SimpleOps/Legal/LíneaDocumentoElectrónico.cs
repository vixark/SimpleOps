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

using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Legal {


    /// <summary>
    /// Clase auxiliar para pasar información de una línea de un documento electrónico entre los procedimientos en 
    /// <see cref="DocumentoElectrónico{D, M}.Crear(out string)"/>. Solo usa un campo para impuesto que puede ser usado
    /// para IVA o para ImpuestoConsumo.
    /// </summary>
    class LíneaDocumentoElectrónico<M> where M : MovimientoProducto {


        #region Propiedades

        public decimal Cantidad { get; set; }

        /// <summary>
        /// Si es muestra gratis, no es cero.
        /// </summary>
        public decimal SubtotalReal { get; set; }

        public decimal SubtotalRealDian { get; set; }

        public decimal Impuesto { get; set; }

        public ModoImpuesto ModoImpuesto { get; set; }

        public TipoTributo TipoTributo { get; set; }

        #endregion Propiedades


        #region Constructores

        public LíneaDocumentoElectrónico() { }


        public LíneaDocumentoElectrónico(M línea, TipoTributo tipoTributo) {

            if (línea.Producto == null) throw new Exception("No se esperaba que el producto fuera nulo.");
            if (línea.IVA == null) throw new Exception("No se esperaba que el IVA fuera nulo.");
            if (línea.ImpuestoConsumo == null) throw new Exception("No se esperaba que el impuesto al consumo fuera nulo.");

            (Cantidad, TipoTributo, SubtotalReal, SubtotalRealDian) = (línea.Cantidad, tipoTributo, línea.SubtotalBaseReal, línea.SubtotalBaseRealDian);
            Impuesto = línea.ObtenerValorImpuesto(tipoTributo);
            ModoImpuesto = línea.ObtenerModoImpuesto(tipoTributo);

        } // LíneaDocumentoElectrónico>


        #endregion  Constructores>

        public decimal TarifaImpuestoPorcentual => ObtenerTarifaImpuestoPorcentual(Impuesto, SubtotalReal);

        public decimal TarifaImpuestoUnitario => Impuesto / Cantidad;


        #region Propiedades Autocalculadas

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones


        public static List<LíneaDocumentoElectrónico<M>> CrearLista(List<M> líneas, TipoTributo tipoTributo) {

            var respuesta = new List<LíneaDocumentoElectrónico<M>>();
            foreach (var línea in líneas) {
                respuesta.Add(new LíneaDocumentoElectrónico<M>(línea, tipoTributo));
            }
            return respuesta;

        } // CrearLista>


        #endregion  Métodos y Funciones>


    } // LíneaDocumentoElectrónico>



} // SimpleOps.Legal>
