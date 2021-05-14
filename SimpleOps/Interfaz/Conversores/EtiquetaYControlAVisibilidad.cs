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

                #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
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
                #pragma warning restore CS8524

            } else {
                throw new Exception(CasoNoConsiderado(values[0].ToString()));
            }

        } // Convert>


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // EtiquetaYControlAVisibilidad>



} // SimpleOps.Interfaz>
