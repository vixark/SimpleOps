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



namespace Vixark {



    /// <summary>
    /// Clase auxiliar que reemplaza la <see cref="Tuple{T1, T2}"/> para propósitos de serialización. Es necesario usarla porque 
    /// la serialización exige una clase con constructor sin parámetros. 
    /// </summary>
    class TuplaSerializable<T1, T2> { // Ver https://stackoverflow.com/a/13739409/8330412.


        #region Propiedades

        public T1 I1 { get; set; } = default!; // Se usa el nombre reducido I1 para reducir el tamaño del texto serializado.

        public T2 I2 { get; set; } = default!; // Se usa el nombre reducido I2 para reducir el tamaño del texto serializado.

        #endregion Propiedades>


        #region Constructores

        public TuplaSerializable() { }

        public TuplaSerializable(T1 i1, T2 i2) => (I1, I2) = (i1, i2);

        #endregion Constructores.


        #region Operadores

        public static implicit operator TuplaSerializable<T1, T2>(Tuple<T1, T2> t) => new TuplaSerializable<T1, T2>() { I1 = t.Item1, I2 = t.Item2 };

        public static implicit operator Tuple<T1, T2>(TuplaSerializable<T1, T2> t) => Tuple.Create(t.I1, t.I2);

        #endregion Operadores>


    } // TuplaSerializable>



} // Vixark>
