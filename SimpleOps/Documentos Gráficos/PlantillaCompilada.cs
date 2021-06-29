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

using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SimpleOps.DocumentosGráficos {



    public class PlantillaCompilada<T> where T : class { // Tomada de https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout.


        #region Propiedades

        private readonly IRazorEngineCompiledTemplate<PlantillaBase<T>> Cuerpo;

        private readonly Dictionary<string, IRazorEngineCompiledTemplate<PlantillaBase<T>>> Partes;

        #endregion Propiedades>


        #region Constructores

        public PlantillaCompilada(IRazorEngineCompiledTemplate<PlantillaBase<T>> cuerpo, 
            Dictionary<string, IRazorEngineCompiledTemplate<PlantillaBase<T>>> partes) => (Cuerpo, Partes) = (cuerpo, partes);

        #endregion Constructores>


        #region Métodos y Funciones

        public string ObtenerHtml(T modelo) => ObtenerHtml(Cuerpo, modelo);


        public string ObtenerHtml(IRazorEngineCompiledTemplate<PlantillaBase<T>> plantillaCompilada, T modelo) {

            if (plantillaCompilada == null) return "";

            var plantillaReferencia = default(PlantillaBase<T>);
  
            var htmlCuerpo = plantillaCompilada.Run(p => { // Ejecuta la plantillaCompilada (puede ser el cuerpo o una parte) y ejecuta todos los Incluir() que tenga y ejecuta ObtenerHtml() nuevamente de manera recursiva para esa parte. 
                p.Model = modelo;
                p.IncluirCallback = (clave, modeloIncluído) => Partes.ContainsKey(clave) ? ObtenerHtml(Partes[clave], modeloIncluído) : ""; // Se obtiene el HTML de cada una de las partes y se inserta en el HTML de respuesta.
                plantillaReferencia = p;
            });

            if (plantillaReferencia == null || plantillaReferencia.ClaveMarco == null) return htmlCuerpo;

            return Partes[plantillaReferencia.ClaveMarco].Run(p => { // Si la plantillaCompilada tiene un marco, el HTML devuelto es el del marco al que se le inserta el HTML del cuerpo obtenido anteriormente. 
                p.Model = modelo;
                p.IncluirCallback = (clave, modeloIncluído) => Partes.ContainsKey(clave) ? ObtenerHtml(Partes[clave], modeloIncluído) : "";
                p.CrearCuerpoCallback = () => htmlCuerpo;
            });

        } // ObtenerHtml>


        #endregion Métodos y Funciones>


    } // PlantillaCompilada>



} // SimpleOps.DocumentosGráficos>
