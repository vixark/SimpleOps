using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SimpleOps.Global;



namespace SimpleOps.Interfaz {



    public class CuadroDiálogoContraseña {


        #region Propiedades

        private Window Ventana { get; set; }

        private bool ClicEnAceptar { get; set; }

        private PasswordBox CuadroContraseña { get; set; }

        #endregion Propiedades>


        #region Métodos y Funciones


        private CuadroDiálogoContraseña(string mensaje, string título) {

            var contenedorPrincipal = new StackPanel();
            var tamañoLetra = ObtenerTamañoLetra(TamañoLetra.M);
            var margen = new Thickness(30);

            Ventana = new Window {
                Height = 350,
                Width = 600,
                Background = ObtenerBrocha(BrochaTema.Fondo),
                Title = título,
                FontSize = tamañoLetra,
                Content = contenedorPrincipal,
                WindowStyle = WindowStyle.SingleBorderWindow,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var cuadroDescripción = new TextBlock {
                Margin = margen,
                TextWrapping = TextWrapping.Wrap,
                Foreground = ObtenerBrocha(BrochaTema.Texto),
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = mensaje
            };

            contenedorPrincipal.Children.Add(cuadroDescripción);

            CuadroContraseña = new PasswordBox { HorizontalAlignment = HorizontalAlignment.Center, Width = 200 };
            CuadroContraseña.KeyDown += CuadroContraseña_KeyDown;

            contenedorPrincipal.Children.Add(CuadroContraseña);

            var botónAceptar = new Button { Width = 100, Margin = margen };
            botónAceptar.Click += BotónAceptar_Click;
            botónAceptar.Content = "Aceptar";

            var contenedorBotónAceptar = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            contenedorPrincipal.Children.Add(contenedorBotónAceptar);
            contenedorBotónAceptar.Children.Add(botónAceptar);

            CuadroContraseña.Focus();

        } // CuadroDiálogoContraseña>


        private void CuadroContraseña_KeyDown(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter && ClicEnAceptar == false) {
                e.Handled = true;
                BotónAceptar_Click(CuadroContraseña, null);
            }

        } // CuadroContraseña_KeyDown>


        private void BotónAceptar_Click(object sender, RoutedEventArgs? e) {

            ClicEnAceptar = true;
            if (!string.IsNullOrEmpty(CuadroContraseña.Password)) Ventana.Close();
            ClicEnAceptar = false;

        } // Aceptar_Clic>


        public static string Mostrar(string mensaje, string título) {

            var diálogo = new CuadroDiálogoContraseña(mensaje, título);
            diálogo.Ventana.ShowDialog();
            return diálogo.CuadroContraseña.Password;

        } // Mostrar>


        #endregion Métodos y Funciones>


    } // CuadroDiálogoContraseña>



} // SimpleOps.Interfaz>
