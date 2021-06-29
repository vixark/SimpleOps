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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Entidad a la que se le compran productos.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Proveedor : EntidadEconómica {


        #region Propiedades

        /// <summary>
        /// El porcentaje sobre el subtotal de la compra que típicamente representa el valor del transporte de los envíos. Si es cero el transporte es gratis o se trata de un producto o servicio virtual. Si es nulo es desconocido y se solicitará agregarlo cuando sea necesario.
        /// </summary>
        public double? PorcentajeCostoTransporte { get; set; }

        /// <summary>
        /// Descuento sobre el subtotal aplicable a todas las compras. 
        /// </summary>
        public double PorcentajeDescuento { get; set; }

        /// <summary>
        /// El subtotal de compra mínima permitida. Si es 0 es sin restricción.
        /// </summary>
        public decimal CompraMínima { get; set; }

        /// <summary>
        /// Los días de entrega típicos. Es necesario para calcular los inventarios máximos y mínimos. Si es nulo es desconocido y se solicitará agregarlo cuando sea necesario.
        /// </summary>
        public int? DíasEntrega { get; set; }

        /// <summary>
        /// Desconocida = 0, Ninguna = 1, MuyBaja = 10, Baja = 20, Media = 30, Alta = 40, MuyAlta = 50. Si es desconocida se usan las reglas en opciones. Al realizar la programación de los pedidos se seleccionan proveedores de más alta prioridad.
        /// </summary>
        public Prioridad PrioridadPropia { get; set; } = Prioridad.Desconocida; 

        /// <summary>
        /// Otro = -2, Ninguno = -1, Desconocido = 0, Bogotá = 1, Popular = 2, ItaúCorpbanca = 6, Bancolombia = 7, Citibank = 9, GNBSudameris = 12, BBVA = 13, Occidente = 23, CajaSocial = 30, Davivienda = 39, ScotiabankColpatria = 42, Agrario = 43, AVVillas = 49, CredifinancieraProcredit = 51, Bancamía = 52, W = 53, Bancoomeva = 54, Finandina = 55, Falabella = 56, Pichincha = 57, Coopcentral = 58, SantanderDeNegocios = 59, MundoMujer = 60, Multibank = 61, Bancompartir = 62, Serfinanza = 63, Corficolombiana = 2011, InversiónBancolombia = 2037, JPMorgan = 2041, BNPParibas = 2042, CorfiGNBSudameris = 2048, CorporaciónFinancieraDavivienda = 2049, GirosYFinanzas = 4008, Tuya = 4026, GMFinancial = 4031, Coltefinanciera = 4046, Bancoldex = 4101, FinancieraDann = 4108, FinancieraPagos = 4115, Credifamilia = 4117, Crezcamos = 4118, LaHipotecaria = 4120, Juriscoop = 4121, RCI = 4122, FinancieraDeAntioquia = 32001, CooperativaFinancieraJFK = 32002, Coofinep = 32003, Cotrafa = 32004, Confiar = 32005, Daviplata = 1001551, Nequi = 1001507, FinanciamientoItau = 1001014.
        /// </summary>
        public Banco Banco { get; set; } = Banco.Desconocido;

        /// <summary>
        /// Desconocida = 0, Ahorros = 1, Corriente = 2.
        /// </summary>
        public TipoCuentaBancaria TipoCuentaBancaria { get; set; } = TipoCuentaBancaria.Desconocida;

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? NúmeroCuentaBancaria { get; set; }

        /// <summary>
        /// Al que se enviarán los pedidos.
        /// </summary>
        [ForeignKey("ContactoPedidosID")] 
        public Contacto? ContactoPedidos { get; set; }
        public int? ContactoPedidosID { get; set; }

        /// <summary>
        /// Al que se enviarán los informes de los pagos realizados.
        /// </summary>
        [ForeignKey("ContactoInformesPagosID")] 
        public Contacto? ContactoInformesPagos { get; set; }
        public int? ContactoInformesPagosID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Proveedor() : base(null!) { } // Solo para que Entity Framework no saque error.

        public Proveedor(string nombre) : base(nombre) { }

        public Proveedor(string nombre, Municipio municipio) : base(nombre, municipio) { }

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public Prioridad Prioridad => PrioridadPropia != Prioridad.Desconocida ? PrioridadPropia : ObtenerPrioridadProveedor();

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => Nombre;


        /// <summary>
        /// Cumple una función de segunda verificación que la compra no se genere con datos incorrectos porque en la 
        /// interfaz de usuario se hace el control principal.
        /// </summary>
        public static void VerificarDatosCompra(Proveedor? proveedor) {

            var éxito = true;
            if (proveedor != null) {
                if (proveedor.TipoEntidad == TipoEntidad.Desconocido) éxito = false;
                if (proveedor.Identificación == null) éxito = false;
            } else {
                éxito = false;
            }
            if (!éxito) throw new Exception("Faltan datos de cliente para poder generar una compra.");

        } // VerificarDatosCompra>


        #endregion Métodos y Funciones>


    } // Proveedor>



} // SimpleOps.Modelo>
