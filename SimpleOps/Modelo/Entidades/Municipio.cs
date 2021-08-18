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
using System.Text;
using static SimpleOps.Global;
using static SimpleOps.Legal.Dian;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Ciudad o pueblo.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Municipio : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio. No es la clave principal porque podría ser cambiado y podrían haber varios con el mismo nombre en diferentes departamentos.

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? NombreOficial { get; set; } // Es el nombre que se envía a la DIAN en la facturación electrónica, pero internamente no se usa para nada más porque es más práctico usar el nombre común de los municipios. Esto es principalmente para acomodar los nombres oficiales incómodamente largos y poco usados en el uso normal de Bogotá, Distrito Capital, Cartagena De Indias, San José De Cúcuta, San Andrés De Tumaco, San Sebastián De Mariquita y Villa De San Diego De Ubaté. 

        /// <MaxLength>60</MaxLength>
        [MaxLength(60)]
        public string Departamento { get; set; } = null!; // Obligatorio en el uso normal. Cuando se carga desde un DTO de integración podría llegar a ser nulo y generar problemas, pero como la integración no es el foco de la aplicación se dejará sin controlar este caso. Aunque se podría pensar en establecer una clave. Es de 60 de largo para cumplir con el caso de 'Archipiélago De San Andrés, Providencia Y Santa Catalina'.

        /// <summary>
        /// Código único del municipio asignado por el gobierno.
        /// </summary>
        /// <MaxLength>10</MaxLength>
        [MaxLength(10)]
        public string? Código { get; set; } // Aunque siempre debería tener un valor para los municipios colombianos porque es obligatorio para la facturación electrónica también puede ser un municipio de otro país y para estos no se requiere proporcionar este código a la DIAN.

        /// <summary>
        /// Si es nulo se usa el país en opciones.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? OtroPaís { get; set; }

        public bool MensajeríaDisponible { get; set; }

        /// <summary>
        /// Si es nulo se usan las reglas en Global.ObtenerPorcentajeIVA(). 0 si es exento de IVA, por ejemplo: San Andrés y Providencia.
        /// </summary>
        public double? PorcentajeIVAPropio { get; set; }

        #endregion Propiedades>


        #region Constructores

        public Municipio() { } // Solo para que Entity Framework no saque error. Se permite público para poderlo pasar nulo al método genérico Vixark.General.CopiarA() y que dentro de este se cree la nueva instancia de Municipio.

        public Municipio(string nombre, string departamento) => (Nombre, Departamento) = (nombre, departamento);

        public Municipio(string nombre, string departamento, string nombreOficial) 
            => (Nombre, Departamento, NombreOficial) = (nombre, departamento, nombreOficial);

        public double PorcentajeIVA => PorcentajeIVAPropio ?? Empresa.PorcentajeIVAPredeterminadoEfectivo; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        public bool ExentoIVA => PorcentajeIVA == 0; // Es solo para fines informativos porque al facturar se debe usar la función Global.ObtenerPorcentajeIVA().

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public string País => OtroPaís ?? Generales.PaísPredeterminado;

        public string CódigoDepartamento => ObtenerCódigoDepartamento(Departamento);

        public string? CódigoPaís => País == "Colombia" ? "CO" : null;

        public string? CódigoLenguajePaís => País == "Colombia" ? "es" : null; // Identificador del lenguaje utilizado en el nombre del país. Para español, utilizar el literal "es". Ver lista de valores posibles en el numeral 0, columna ISO639-1. Debe ser "es" para que funcione la facturación electrónica.

        public string? NombreOficialEfectivo => NombreOficial ?? Nombre; // Para los que no tengan nombre oficial, se usa el nombre común.

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Municipio>



} // SimpleOps.Modelo>
