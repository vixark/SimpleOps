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



namespace SimpleOps.Modelo {



    /// <summary>
    /// Determina como se controlan los conflictos de concurrencia en la creación de nuevas entidades (inserciones de filas). 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    class ControlInserciónAttribute : Attribute {

        /// <summary>
        /// <b>Ninguno:</b> Se aplica a entidades que pueden ser creadas por usuarios diferentes incluso teniendo igual todas sus propiedades. 
        /// No tienen claves adicionales. Por ejemplo, <see cref="Cobro"/> o <see cref="Cotización"/>. Las inserciones se pueden realizar 
        /// concurrentemente sin generar ningún conflicto o excepción.<br/>
        /// <b>Optimista:</b> Se aplica a entidades que requieren controles adicionales sobre sus datos para impedir duplicados. Estas entidades 
        /// pueden o no tener un ID autocalculado (no se tiene en cuenta en el control de concurrencia de inserción), pero deben tener
        /// una clave o grupo de claves con restricción de unicidad. Por ejemplo, no deben existir dos <see cref="PrecioCliente"/> con el mismo ProductoID y 
        /// ClienteID. Esto se restringe a nivel de la base de datos con la creación de la clave ProductoID-ClienteID, pero también se hace 
        /// explícito con este atributo para que al guardar se controle el caso de clave ya existente y se proceda con la resolución de conflicto 
        /// de inserción con ayuda del usuario. Si no se hace explícito se produce una excepción.<br/>
        /// <b>Pesimista:</b> Se aplica a entidades que requieren llevar un estricto número consecutivo diferente del ID (Por ejemplo, <see cref="Venta"/>), 
        /// entidades donde el ID es relacionado en otras tablas en la misma transacción (Por ejemplo, <see cref="Pedido"/>) y/o 
        /// entidades donde su creación (adición de fila) genera múltiples cambios en la base de datos que no se pueden producir doblemente por 
        /// usuarios diferentes (Por ejemplo, <see cref="LíneaRemisión"/>). Al iniciar una transacción de inserción se activa un bloqueo en la 
        /// tabla y tablas relacionadas para impedir inserciones y/o cambios de otros usuarios. Si se detecta un conflicto de inserción al 
        /// segundo usuario se le impide realizar la acción hasta que el bloqueo sea liberado. Este tipo de control se implementa 
        /// con lógica personalizada para cada caso.<br/>
        /// <b>NoPermitido:</b> No se permite la inserción de filas por ningún usuario.
        /// </summary>
        public ControlConcurrencia Tipo { get; set; }

        public ControlInserciónAttribute(ControlConcurrencia tipo) => (Tipo) = (tipo);


    } // ControlInserciónAttribute>



} // SimpleOps.Modelo>
