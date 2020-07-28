using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using static SimpleOps.Global;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Convierte un texto vacío o nulo a Collapsed y cualquier otro texto a Visible. 
    /// Es útil para ocultar controles cuando no hay texto.
    /// Al pasar parameter = true (ConverterParameter={StaticResource Verdadero}) convierte un texto vacío o nulo a Visible y cualquier otro texto a Collapsed. 
    /// Es útil para mostrar y ocultar en los cuadros de texto un texto indicador que describe el contenido que deben llevar. Este texto
    /// indicador solo se muestra cuando el cuadro de texto está vacío.
    /// </summary>
    class TextoAVisibilidad : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var inverso = false;
            if (parameter is bool inversoAux) inverso = inversoAux;
            return ObtenerVisibilidad(value, inverso);

        } // Convert>


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


    } // TextoAVisibilidad>



} // SimpleOps.Interfaz>
