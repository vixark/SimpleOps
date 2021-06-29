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



namespace SimpleOps.DocumentosGráficos {



    public class DatosEmpresa {



        #region Propiedades

        public string? Nombre { get; set; } 

        public string? TeléfonoPrincipal { get; set; }

        public string? EmailVentas { get; set; }

        public string? SitioWeb { get; set; }

        public string? MunicipioUbicaciónNombre { get; set; }

        public string? DirecciónUbicaciónEfectiva { get; set; }

        public string? RazónSocial { get; set; }

        public string? NitCompleto { get; set; }

        public string TipoContribuyenteTexto { get; set; } = null!; // Se asegura que no es nulo.

        public bool DetallarImpuestoSiPorcentajesDiferentes { get; set; }

        public bool MostrarUnidad { get; set; }

        public string? EnlaceWebADetalleProducto { get; set; }

        public int? PrimerNúmeroFacturaAutorizada { get; set; }

        public int? ÚltimoNúmeroFacturaAutorizada { get; set; }

        public decimal? NúmeroAutorizaciónFacturación { get; set; }

        public DateTime? FinAutorizaciónFacturación { get; set; }

        #endregion Propiedades>



    } // DatosEmpresa>



} // SimpleOps.DocumentosGráficos>
