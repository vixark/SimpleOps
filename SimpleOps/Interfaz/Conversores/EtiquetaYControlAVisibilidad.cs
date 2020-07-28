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
                            _ => throw new Exception(CasoNoConsiderado(tipoEtiqueta))
                        },
                    "CnvContenedorMiniEtiqueta" => 
                        tipoEtiqueta switch {
                            EtiquetaCuadroTexto.Mini => Visibility.Visible,
                            EtiquetaCuadroTexto.Lateral => Visibility.Collapsed,
                            EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                            _ => throw new Exception(CasoNoConsiderado(tipoEtiqueta))
                        },
                    "LblOcultador" =>
                        tipoEtiqueta switch {
                            EtiquetaCuadroTexto.Mini => Visibility.Collapsed,
                            EtiquetaCuadroTexto.Lateral => Visibility.Visible,
                            EtiquetaCuadroTexto.MiniSiempreVisible => Visibility.Visible,
                            _ => throw new Exception(CasoNoConsiderado(tipoEtiqueta))
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
