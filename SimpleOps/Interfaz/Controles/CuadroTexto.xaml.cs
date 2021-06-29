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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace SimpleOps.Interfaz {



    public enum EtiquetaCuadroTexto { Mini, MiniSiempreVisible, Lateral } // Se presenta un error de accesibilidad de esta enumeración si se declara en Global.cs.



    /// <summary>
    /// Cuadro de texto personalizado que muestra un texto indicador de su contenido cuando está vacío. 
    /// </summary>
    public partial class CuadroTexto : UserControl {


        #region Propiedades

        public static readonly DependencyProperty PropiedadTextoDescriptivo 
            = DependencyProperty.Register(nameof(TextoDescriptivo), typeof(string), typeof(CuadroTexto));

        public string TextoDescriptivo {
            get => (string)GetValue(PropiedadTextoDescriptivo);
            set => SetValue(PropiedadTextoDescriptivo, value);
        } // TextoDescriptivo>


        public static readonly DependencyProperty PropiedadTexto = DependencyProperty.Register(nameof(Texto), typeof(string), typeof(CuadroTexto), null);

        public string Texto {
            get => (string)GetValue(PropiedadTexto);
            set => SetValue(PropiedadTexto, value);
        } // Texto>


        public static readonly DependencyProperty PropiedadAlto = DependencyProperty.Register(nameof(Alto), typeof(double), typeof(CuadroTexto), 
                new PropertyMetadata((double)Application.Current.FindResource("AltoCuadroTexto")));

        public double Alto {
            get => (double)GetValue(PropiedadAlto);
            set => SetValue(PropiedadAlto, value);
        } // Alto>


        public static readonly DependencyProperty PropiedadTipoEtiqueta 
            = DependencyProperty.Register(nameof(TipoEtiqueta), typeof(EtiquetaCuadroTexto), typeof(CuadroTexto), new PropertyMetadata(EtiquetaCuadroTexto.Mini));

        public EtiquetaCuadroTexto TipoEtiqueta {
            get => (EtiquetaCuadroTexto)GetValue(PropiedadTipoEtiqueta);
            set => SetValue(PropiedadTipoEtiqueta, value);
        } // TipoEtiqueta>


        public static readonly DependencyProperty PropiedadAnchoEtiquetaLateral 
            = DependencyProperty.Register(nameof(AnchoEtiquetaLateral), typeof(double), typeof(CuadroTexto), new PropertyMetadata((double)100));

        public double AnchoEtiquetaLateral {
            get => (double)GetValue(PropiedadAnchoEtiquetaLateral);
            set => SetValue(PropiedadAnchoEtiquetaLateral, value);
        } // AnchoEtiquetaLateral>

        #endregion Propiedades>


        #region Métodos

        public CuadroTexto() => InitializeComponent();

        #endregion Métodos>


    } // CuadroTexto>



} // SimpleOps.Interfaz>
