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