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



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Establece el margen de todos los controles hijos.
    /// </summary>
    public static class EstablecedorMargen { // Ver https://web.archive.org/web/20120711175633/http://blogs.microsoft.co.il/blogs/eladkatz/archive/2011/05/29/what-is-the-easiest-way-to-set-spacing-between-items-in-stackpanel.aspx.


        #region Propiedades

        public static Thickness GetMargen(DependencyObject objeto) => objeto != null ? (Thickness)objeto.GetValue(PropiedadMargen) : new Thickness(); // Se debe mantener el término de inicio Get porque lo requiere WPF.

        public static void SetMargen(DependencyObject objeto, Thickness value) => objeto?.SetValue(PropiedadMargen, value); // Se debe mantener el término de inicio Set porque lo requiere WPF.

        public static readonly DependencyProperty PropiedadMargen = DependencyProperty.RegisterAttached("Margen", typeof(Thickness),
            typeof(EstablecedorMargen), new UIPropertyMetadata(new Thickness(), Cambió));

        #endregion Propiedades>


        #region Métodos


        public static void Cambió(object sender, DependencyPropertyChangedEventArgs e) {
            if (!(sender is Panel panel)) return;
            panel.Loaded += new RoutedEventHandler(EstablecerMargenHijos); // Adjunta el método para cambiar los márgenes después que el panel cargue completamente para pueda encontrar los hijos.
        } // Cambió>


        public static void EstablecerMargenHijos(object sender, RoutedEventArgs e) {

            if (!(sender is Panel panel)) return;
            foreach (var hijo in panel.Children) {
                if (!(hijo is FrameworkElement feHijo)) continue;
                feHijo.Margin = GetMargen(panel);
            }

        } // EstablecerMargenHijos>


        #endregion Métodos>


    } // EstablecedorMargen>



} // SimpleOps.Interfaz>
