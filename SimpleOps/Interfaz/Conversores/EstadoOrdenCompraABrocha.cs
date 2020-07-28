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
