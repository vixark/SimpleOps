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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Barra de desplazamiento personalizada que permite el desplazamiento horizontal con la rueda del mouse y personalizar
    /// la velocidad de ese desplazamiento.
    /// </summary>
    class BarraDesplazamiento : ScrollViewer {


        #region Propiedades

        public static readonly DependencyProperty PropiedadVelocidad  
            = DependencyProperty.Register(nameof(Velocidad), typeof(double), typeof(BarraDesplazamiento), new PropertyMetadata((double)100));

        /// <summary>
        /// 100 es la velocidad normal.
        /// </summary>
        public double Velocidad {
            get => (double)GetValue(PropiedadVelocidad);
            set => SetValue(PropiedadVelocidad, value);
        } // Velocidad>

        #endregion Propiedades>


        #region Eventos


        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {

            if (e.Handled) return;

            if (e.Source is ListBox) { // Cuando el cursor esté encima de un ListBox evitará hacer desplazamiento para dar prioridad al dezplazamiento por los ítems del ListBox.
                e.Handled = false;
            } else {

                if (ScrollInfo is ScrollContentPresenter scp) {

                    if (ComputedVerticalScrollBarVisibility != Visibility.Collapsed) { // Si hay barra de desplazamiento vertical se le da prioridad al desplazamiento en esa dirección
                        scp.SetVerticalOffset(VerticalOffset - e.Delta * Velocidad / 100);
                    } else if (ComputedHorizontalScrollBarVisibility != Visibility.Collapsed) { // Si no hay barra de desplazamiento vertical verificará si se desplazará horizontalmente.
                        scp.SetHorizontalOffset(HorizontalOffset - e.Delta * Velocidad /100);
                    }
                    e.Handled = true;

                }

            }

        } // OnPreviewMouseWheel>


        #endregion Eventos>


    } // BarraDesplazamiento>



} // SimpleOps.Interfaz>
