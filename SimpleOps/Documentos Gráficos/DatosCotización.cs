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
using System.Text;
using System.Linq;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.DocumentosGráficos {



    public class DatosCotización : DatosDocumento, IConLíneasProductos {


        #region Propiedades

        #pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura. Se acepta porque es necesario asignarla en el método CopiarA().
        public List<DatosLíneaProducto> Líneas { get; set; } = null!; // Nunca es nulo (podría ser lista vacía), solo es para que no saque advertencia.
        #pragma warning restore CA2227

        public OpcionesColumnas Columnas { get; set; } = new OpcionesColumnas();

        public string? CondicionesComerciales { get; set; }

        public string? EnlaceDescargaXlsx { get; set; }

        public List<string> ReferenciasProductosPáginasExtra { get; private set; } = new List<string>();

        public Dictionary<string, DatosProducto> DatosProductos { get; private set; } = new Dictionary<string, DatosProducto>(); // Contiene la información que se escribe en el catálogo o cotización de todos los productos base y productos en Líneas. Es útil principalmente para acceder a la información de los productos base con su detalle de precios por producto específico.

        public int? CantidadFilasProductos { get; set; } 

        public int? CantidadColumnasProductos { get; set; }  

        public string? TítuloPáginasExtra { get; set; }

        public string? ContactoNombre { get; set; }

        public string? ContactoTeléfono { get; set; }

        public string? ContactoEmail { get; set; } // Puede ser nulo porque el contacto puede ser nulo.

        #endregion Propiedades>


        #region Constructores

        public DatosCotización() => (NombreDocumento) = ("Cotización");

        #endregion Constructores>


        #region Métodos y Funciones


        public decimal? ObtenerPrecio(string referencia) {

            if (Líneas == null || Líneas.Count == 0) return null;
            var línea = Líneas.FirstOrDefault(l => l.ProductoReferencia.IgualA(referencia));
            if (línea == null || línea.PrecioBase <= 0) return null;
            return línea.PrecioBase;

        } // ObtenerPrecio>


        #endregion Métodos y Funciones>


    } // DatosCotización>



} // SimpleOps.DocumentosGráficos>
