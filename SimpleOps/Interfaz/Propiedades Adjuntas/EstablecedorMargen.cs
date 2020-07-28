using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;



namespace SimpleOps.Interfaz {



    /// <summary>
    /// Establece el margen de todos los controles hijos.
    /// </summary>
    public static class EstablecedorMargen { // Tomado de https://web.archive.org/web/20120711175633/http://blogs.microsoft.co.il/blogs/eladkatz/archive/2011/05/29/what-is-the-easiest-way-to-set-spacing-between-items-in-stackpanel.aspx.


        #region Propiedades

        public static Thickness GetMargen(DependencyObject objeto) => objeto != null ? (Thickness)objeto.GetValue(PropiedadMargen) : new Thickness(); // Se debe mantener el término de inicio Get porque lo requiere WPF.

        public static void SetMargen(DependencyObject objeto, Thickness value) => objeto?.SetValue(PropiedadMargen, value); // Se debe mantener el término de inicio Set porque lo requiere WPF.

        public static readonly DependencyProperty PropiedadMargen = DependencyProperty.RegisterAttached("Margen", typeof(Thickness),
            typeof(EstablecedorMargen), new UIPropertyMetadata(new Thickness(), Cambió));

        #endregion Propiedades>


        #region Métodos


        public static void Cambió(object sender, DependencyPropertyChangedEventArgs e) {
            if (!(sender is Panel panel)) return;
            panel.Loaded += new RoutedEventHandler(EstablecerMargenHijos); // Adjunta el método para cambiar los márgenes después que el panel cargue completamente para pueda encontrar los hijos.
        } // Cambió>


        public static void EstablecerMargenHijos(object sender, RoutedEventArgs e) {

            if (!(sender is Panel panel)) return;
            foreach (var hijo in panel.Children) {
                if (!(hijo is FrameworkElement feHijo)) continue;
                feHijo.Margin = GetMargen(panel);
            }

        } // EstablecerMargenHijos>


        #endregion Métodos>


    } // EstablecedorMargen>



} // SimpleOps.Interfaz>
