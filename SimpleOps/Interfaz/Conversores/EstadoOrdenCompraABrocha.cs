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
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Convierte un estado de una orden de compra a una brocha
    /// con un color que lo represente.
    /// </summary>
    class EstadoOrdenCompraABrocha : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var sufijoNombreBrocha = parameter?.ToString();
            return value switch {
                null => Visibility.Collapsed,
                EstadoOrdenCompra estado => ObtenerBrocha(Equipo.BrochasEstadosOrdenesCompra[estado], sufijoNombreBrocha),
                _ => throw new Exception(CasoNoConsiderado(value?.ToString())),
            };

        } // Convert>


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // EstadoOrdenCompraABrocha>



} // SimpleOps.Interfaz>
