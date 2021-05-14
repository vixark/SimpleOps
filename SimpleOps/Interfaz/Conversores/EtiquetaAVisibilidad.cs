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

                #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
                return tipoEtiqueta switch {
                    EtiquetaCuadroTexto.Mini => Visibility.Collapsed,
                    EtiquetaCuadroTexto.Lateral => Visibility.Collapsed,
                    EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                };
                #pragma warning restore CS8524

            } else {
                throw new Exception(CasoNoConsiderado(value.ToString()));
            }

        } // Convert>


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // EtiquetaAVisibilidad>



} // SimpleOps.Interfaz>
