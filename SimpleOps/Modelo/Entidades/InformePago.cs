using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// De un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class InformePago : Registro { // Es Registro porque no cambia una vez generado. No se necesita rastrear la información de actualización.


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } 

        public decimal Valor { get; set; } // Obligatorio.

        public DateTime FechaHoraPago { get; set; } // Obligatorio.

        /// <summary>
        /// Otro = -2, Ninguno = -1, Desconocido = 0, Bogotá = 1, Popular = 2, ItaúCorpbanca = 6, Bancolombia = 7, Citibank = 9, GNBSudameris = 12, BBVA = 13, Occidente = 23, CajaSocial = 30, Davivienda = 39, ScotiabankColpatria = 42, Agrario = 43, AVVillas = 49, CredifinancieraProcredit = 51, Bancamía = 52, W = 53, Bancoomeva = 54, Finandina = 55, Falabella = 56, Pichincha = 57, Coopcentral = 58, SantanderDeNegocios = 59, MundoMujer = 60, Multibank = 61, Bancompartir = 62, Serfinanza = 63, Corficolombiana = 2011, InversiónBancolombia = 2037, JPMorgan = 2041, BNPParibas = 2042, CorfiGNBSudameris = 2048, CorporaciónFinancieraDavivienda = 2049, GirosYFinanzas = 4008, Tuya = 4026, GMFinancial = 4031, Coltefinanciera = 4046, Bancoldex = 4101, FinancieraDann = 4108, FinancieraPagos = 4115, Credifamilia = 4117, Crezcamos = 4118, LaHipotecaria = 4120, Juriscoop = 4121, RCI = 4122, FinancieraDeAntioquia = 32001, CooperativaFinancieraJFK = 32002, Coofinep = 32003, Cotrafa = 32004, Confiar = 32005, Daviplata = 1001551, Nequi = 1001507, FinanciamientoItau = 1001014.
        /// </summary>
        public Banco Banco { get; set; } = Banco.Desconocido; // Obligatorio.

        /// <summary>
        /// Si es nulo la cuenta del movimiento bancario es la establecida en opciones para el banco del movimiento bancario. 
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? OtroNúmeroCuenta { get; set; } 

        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public List<Venta> Ventas { get; set; } = new List<Venta>();

        public List<OrdenCompra> OrdenesCompra { get; set; } = new List<OrdenCompra>();

        #endregion Propiedades>


        #region Constructores

        private InformePago() { } // Solo para que Entity Framework no saque error.

        public InformePago(decimal valor, DateTime fechaHoraPago, Cliente cliente, Banco banco) 
            => (ClienteID, Valor, FechaHoraPago, Cliente, Banco) = (cliente.ID, valor, fechaHoraPago, cliente, banco);

        public InformePago(decimal valor, DateTime fechaHoraPago, Cliente cliente)
            => (ClienteID, Valor, FechaHoraPago, Cliente, Banco) = (cliente.ID, valor, fechaHoraPago, cliente, Empresa.BancoPreferido);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"del {FechaHoraPago.ToShortDateString()} de {ATexto(Cliente, ClienteID)} por {Valor.ATextoDinero()}";

        public string? CuentaBancaria => ObtenerNúmeroCuentaBancaria(Banco, OtroNúmeroCuenta);

        public string? Lugar => $"Recibido en {Banco} {CuentaBancaria}"; // Usada en el ID del elemento PrePaidPayment de la factura electrónica. Este es un uso personalizado de SimpleOps solo para completar la información del pago realizado por el cliente en la factura electrónica.

        #endregion Métodos y Funciones>


    } // InformePago>



} // SimpleOps.Modelo>
