using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Convierte un texto vacío o nulo a brocha transparente y cualquier otro texto a la brocha provista en parameter. 
    /// Es útil para ocultar el fondo del cuadro de texto cuando no hay texto para que se vea el texto indicador que está atrás y
    /// cuando tiene texto para pintarlo con la brocha provista y permitir la aplicacion de bordes redondeados.
    /// </summary>
    class TextoABrocha : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var brochaTransparente = ObtenerBrocha("BrochaTransparente");
            SolidColorBrush brocha = brochaTransparente;
            if (parameter is SolidColorBrush brochaAux) brocha = brochaAux;

            return value switch {
                null => brochaTransparente,
                string texto => string.IsNullOrEmpty(texto) ? brochaTransparente : brocha,
                _ => throw new Exception(CasoNoConsiderado(value?.ToString())),
            };

        } // Convert>


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // TextoABrocha>



} // SimpleOps.Interfaz>
