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
    /// Convierte un tipo de etiqueta en un valor de visibilidad que permite
    /// implementar el tipo de comportamiento esperado en el control CuadroTexto.xaml según cada tipo de etiqueta.
    /// </summary>
    class EtiquetaAVisibilidad : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is EtiquetaCuadroTexto tipoEtiqueta) {

                return tipoEtiqueta switch {
                    EtiquetaCuadroTexto.Mini => Visibility.Collapsed,
                    EtiquetaCuadroTexto.Lateral => Visibility.Collapsed,
                    EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                };

            } else {
                throw new Exception(CasoNoConsiderado(value.ToString()));
            }

        } // Convert>


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // EtiquetaAVisibilidad>



} // SimpleOps.Interfaz>
