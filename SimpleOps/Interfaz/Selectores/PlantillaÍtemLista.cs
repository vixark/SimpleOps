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

using SimpleOps.Modelo;
using System.Windows;
using System.Windows.Controls;
using SimpleOps;
using static Vixark.General;
using System;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Selecciona la plantilla del ítem apropiada para una lista dependiendo del tipo de objeto que contiene.
    /// </summary>
    public class PlantillaÍtemLista : DataTemplateSelector {


        public override DataTemplate? SelectTemplate(object ítem, DependencyObject contenedor) {

            if (contenedor is FrameworkElement elemento && ítem != null) {

                return ítem switch {
                    OrdenCompra _ => elemento.FindResource("PlantillaOrdenCompraEnLista") as DataTemplate,
                    Pedido _ => elemento.FindResource("PlantillaPedidoEnLista") as DataTemplate,
                    _ => throw new Exception(CasoNoConsiderado(ítem.GetType().Name))
                };

            } else {
                return null;
            }

        } // SelectTemplate>


    } // PlantillaÍtemLista>



} // SimpleOps.Interfaz>