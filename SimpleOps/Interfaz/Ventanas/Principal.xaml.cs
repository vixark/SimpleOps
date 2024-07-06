﻿// Copyright Notice:
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleOps.Datos;
using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SimpleOps.Global;
using static Vixark.General;
using static SimpleOps.Configuración;
using SimpleOps.Interfaz;
using System.IO;
using SimpleOps.Integración;



namespace SimpleOps.Interfaz {



    public partial class Principal : Window {


        readonly ILogger Rastreador = FábricaRastreadores.CreateLogger<Principal>();

        Integrador? Integrador { get; set; } // Se necesita el objeto público siempre activo durante la ejecución de la aplicación para que pueda interceptar los cambios de archivos en la carpeta de integración.


        /// <summary>
        /// Ventana principal.
        /// </summary>
        public Principal() {

            // 1. Iniciaciones.
            FinalizarSiExisteOtraInstanciaAbierta(NombreAplicación);
            InitializeComponent();
            Rastreador.LogInformation("Iniciando");
            IniciarVariablesGenerales();
            IniciarVariablesGlobales();
            IniciarVariablesConfiguración();
            CargarOpciones();
            IniciarVariablesDTO(); // Se debe ejecutar después de CargarOpciones() porque usa sus valores.
            ConfigurarCarpetasYArchivos();
            OperacionesEspecialesDatos = false; // Se debe establecer en falso después de cargar las opciones.
            
            // 2. Modos Especiales.
            #pragma warning disable CS0162 // Se detectó código inaccesible. Se omite la advertencia porque las variables de este bloque que son constantes pueden ser modificado por el usuario del código en Configuración.cs.
            if (HabilitarRastreoDeDatosSensibles) 
                LblAlerta.Content = $"Habilitado el rastreo de datos sensibles. Esta función se debe desactivar en producción.{NuevaLínea}";

            if (ModoIntegraciónTerceros) {

                Visibility = Visibility.Visible;
                Integrador = new Integrador();
                if (Integrador.Iniciado) {
                    LblAlerta.Content += $"Habilitada la integración con terceros para facturar electrónicamente, generar catálogos, cotizaciones " +
                        $"y fichas desde otro programa o desde Excel."; // No incrementar mucho este texto para no generar barras de desplazamiento horizontales en pantallas de tamaño medio.
                } else {
                    LblAlerta.Content += $"Sucedió un error habilitando el modo de integración con programas terceros.";
                }
                
            }

            if (string.IsNullOrEmpty(LblAlerta.Content?.ToString())) LblAlerta.Visibility = Visibility.Collapsed;
            #pragma warning restore CS0162

            // 3. Inicio de Variables de Estado Globales  .     
            using var ctx = new Contexto(TipoContexto.Lectura);
            Contexto.CalcularVariablesEstáticas(ctx);

            // 4. Enlace de Datos a Interfaz.
            var órdenesCompraPendientes = ctx.ObtenerÓrdenesCompraPendientes();
            var aleatorio = new Random();
            órdenesCompraPendientes.ForEach(oc => oc.EstadoOrdenCompra = (EstadoOrdenCompra)aleatorio.Next(0, 6));
            if (órdenesCompraPendientes.Any()) órdenesCompraPendientes.First().EstadoOrdenCompra = EstadoOrdenCompra.PendientePedido;
            DataContext = órdenesCompraPendientes;
            // DataContext = ctx.Clientes.Find(aleatorio.Next(1, 100));

            // 5. Pruebas.
            var hacerPruebas = false; // Necesario para que el compilador permita saltar manualmente a la línea Pruebas.Ejecutar(); y para que no saque la advertencia CS0162.
            if (hacerPruebas) Pruebas.Ejecutar(); // Poner punto de interrupción y saltar manualmente la ejecución a las pruebas o usar el botón HacerPruebasUnitarias_Clic.

        } // Principal>


        #region Eventos


        void CargarDatosIniciales_Clic(object sender, RoutedEventArgs e) {

            Rastreador.LogInformation("CargarDatosIniciales_Clic");

            MostrarInformación("A continuación se cargarán los datos iniciales. Este proceso puede tardar varios minutos");

            var éxito = Contexto.CargarDatosIniciales(ObtenerRutaDatosJson(), out string error);
            if (éxito) {
                MostrarInformación("Se cargaron exitosamente los datos iniciales.");
            } else {
                MostrarError(error);
            }

        } // CargarDatosIniciales_Clic>


        void ReiniciarBaseDatosSQLite_Clic(object sender, RoutedEventArgs e) {

            Rastreador.LogInformation("ReiniciarBaseDatosSQLite_Clic");

            var rutaCopiasSeguridad = ObtenerRutaCopiasSeguridad();
            if (MostrarDiálogo($"Se moverá la base de datos actual a {rutaCopiasSeguridad} y se iniciará una base de datos nueva vacía.{DobleLínea}" +
                    $"¿Deseas continuar?", "¿Reiniciar Base de Datos?", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                var nuevaRuta = System.IO.Path.Combine(rutaCopiasSeguridad, ArchivoBaseDatosSQLite.Reemplazar(".db", " [" + AhoraNombresArchivos + "].db"));
                File.Move(RutaBaseDatosSQLite, nuevaRuta);
                File.Copy(RutaBaseDatosVacíaSQLite, RutaBaseDatosSQLite);
                MostrarInformación("Se ha reiniciado la base de datos SQLite exitosamente.");

            }

        } // ReiniciarBaseDatosSQLite_Clic>


        private void ObtenerClaveTécnicaProducción_Clic(object sender, RoutedEventArgs e) {

            var claveTécnica = Legal.Dian.ObtenerClaveTécnicaAmbienteProducción();
            if (claveTécnica != null && claveTécnica.Contains(", ")) {
                MostrarInformación($"Existe más de una resolución de facturación disponible. A continuación se muestran las claves técnicas (número antes " +
                    $"de la coma) para cada resolución (número después de la coma). Asegúrate de usar la correcta en cada equipo. Las claves técnicas " +
                    $"son:{DobleLínea}{claveTécnica}");
            } else {
                MostrarInformación($"La clave técnica es:{DobleLínea}{claveTécnica}");
            }      

        } // ObtenerClaveTécnicaProducción_Clic>


        private void PruebasHabilitaciónFacturación_Clic(object sender, RoutedEventArgs e) => Pruebas.Habilitación();
           
        private void HacerPruebasInternasFacturación_Clic(object sender, RoutedEventArgs e) => Pruebas.Facturación(pruebaHabilitación: false);

        private void GenerarCatálogo_Clic(object sender, RoutedEventArgs e) => Pruebas.GeneraciónCatálogoIntegración();

        private void GenerarFichasInformativas_Clic(object sender, RoutedEventArgs e) => Pruebas.GeneraciónFichasInformativasIntegración();

        private void GenerarCotización_Clic(object sender, RoutedEventArgs e) => Pruebas.GenerarCotizaciónIntegración();

        private void HacerPruebasUnitarias_Clic(object sender, RoutedEventArgs e) => Pruebas.Ejecutar();

        #endregion Eventos>


    } // Principal>



} // SimpleOps.Interfaz>
