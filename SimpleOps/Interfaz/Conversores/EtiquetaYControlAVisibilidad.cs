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
using System.Windows.Controls;
using System.Windows.Data;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Convierte un tipo de etiqueta de cuadro de texto y el control generador en un valor de visibilidad que permite
    /// implementar el tipo de comportamiento esperado en el control CuadroTexto.xaml según cada tipo de etiqueta.
    /// </summary>
    class EtiquetaYControlAVisibilidad : IMultiValueConverter {


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

            string? nombreControl = null;
            if (values[0] is TextBlock textBlock) nombreControl = textBlock.Name;
            if (values[0] is Canvas canvas) nombreControl = canvas.Name;
            if (values[0] is Label label) nombreControl = label.Name;

            if (nombreControl != null && values[1] is EtiquetaCuadroTexto tipoEtiqueta) {

                return nombreControl switch {
                    "TblEtiquetaALaIzquierda" => 
                        tipoEtiqueta switch { 
                            EtiquetaCuadroTexto.Mini => Visibility.Collapsed,
                            EtiquetaCuadroTexto.Lateral => Visibility.Visible,
                            EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Collapsed,
                        },
                    "CnvContenedorMiniEtiqueta" => 
                        tipoEtiqueta switch {
                            EtiquetaCuadroTexto.Mini => Visibility.Visible,
                            EtiquetaCuadroTexto.Lateral => Visibility.Collapsed,
                            EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                        },
                    "LblOcultador" =>
                        tipoEtiqueta switch {
                            EtiquetaCuadroTexto.Mini => Visibility.Collapsed,
                            EtiquetaCuadroTexto.Lateral => Visibility.Visible,
                            EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                        },
                    _ => throw new Exception(CasoNoConsiderado(nombreControl))
                };

            } else {
                throw new Exception(CasoNoConsiderado(values[0].ToString()));
            }

        } // Convert>


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // EtiquetaYControlAVisibilidad>



} // SimpleOps.Interfaz>
