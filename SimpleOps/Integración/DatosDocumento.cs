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
using static SimpleOps.Global;



namespace SimpleOps.Integración {



    class DatosDocumento {



        #region Propiedades Documento

        public int? Número { get; set; } // Para facturas, notas crédito y documentos legales que requieren un consecutivo.

        public string? Prefijo { get; set; }

        public int? ID { get; set; } // Para documentos que no exigen consecutivos y que su entidad asociada hereda de Registro, por lo que tienen un ID interno. Por ejemplo, la cotización.

        public string? CódigoPropio { get; set; } // Para documentos que implementan esta propiedad que permite reemplazar el código predeterminado númerico (ID) por un código personalizado de texto libre. Por ejemplo, la cotización.

        public string? Observación { get; set; }

        public bool MostrarInformaciónAdicional { get; set; }

        public DateTime? FechaHora { get; set; } // Puede ser nulo para documentos que no la requieren como fichas técnicas.

        #endregion Propiedades Documento



        #region Propiedades Cliente 
        // Estas propiedades de Cliente se escriben planas, sin objeto auxiliar de tipo DatosCliente. En el DocumentosGráficos.DatosDocumento si se usa un objeto Cliente de tipo DatosCliente porque permite un uso más orientado a objetos que facilita la escritura y lectura del código de las plantillas. Como esta clase Integración.DatosDocumento no se usa directamente en ninguna parte del código entonces crear una propiedad Cliente que encapsule las propiedades del cliente no aporta valor. Aunque, si se quisiera por estandarización, se podría hacer el cambio, pero se tendría que configurar el mapeo en los objetos MapperConfiguration y si se quisiera ser exhaustivo, habría que agregar también DatosUsuario, Contacto en DatosCotización, Productos y ProductosBase en DatosLíneaProducto y otros. Se considera que hacer estos cambios sería agregar complejidades innecesarias, entonces no se realizan.

        public string? ClienteNombre { get; set; } // Puede ser nulo para documentos que no son específicos para cierto cliente, como los catálogos.

        public string? ClienteTeléfono { get; set; }

        public string? ClienteMunicipioNombre { get; set; }

        public string? ClienteMunicipioDepartamento { get; set; }

        public string? ClienteDirección { get; set; }

        public string? ClienteContactoFacturasEmail { get; set; }

        public int? ClienteDíasCrédito { get; set; }

        public double? ClienteMunicipioPorcentajeIVAPropio { get; set; }

        public double? ClientePorcentajeIVAPropio { get; set; }

        public TipoEntidad ClienteTipoEntidad { get; set; }

        public TipoCliente ClienteTipoCliente { get; set; }

        public string? ClienteIdentificación { get; set; }

        #endregion Propiedades Cliente>



        #region Propiedades Usuario

        public string? UsuarioNombre { get; set; } 

        public string? UsuarioEmail { get; set; } 

        public string? UsuarioTeléfono { get; set; }

        #endregion Propiedades Usuario>



    } // DatosDocumento>



} // SimpleOps.Integración>
