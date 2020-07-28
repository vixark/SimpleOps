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
