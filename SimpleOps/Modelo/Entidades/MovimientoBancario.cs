using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Movimiento de dinero en una cuenta bancaria.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class MovimientoBancario : MovimientoDinero {


        #region Propiedades

        /// <summary>
        /// Otro = -2, Ninguno = -1, Desconocido = 0, Bogotá = 1, Popular = 2, ItaúCorpbanca = 6, Bancolombia = 7, Citibank = 9, GNBSudameris = 12, BBVA = 13, Occidente = 23, CajaSocial = 30, Davivienda = 39, ScotiabankColpatria = 42, Agrario = 43, AVVillas = 49, CredifinancieraProcredit = 51, Bancamía = 52, W = 53, Bancoomeva = 54, Finandina = 55, Falabella = 56, Pichincha = 57, Coopcentral = 58, SantanderDeNegocios = 59, MundoMujer = 60, Multibank = 61, Bancompartir = 62, Serfinanza = 63, Corficolombiana = 2011, InversiónBancolombia = 2037, JPMorgan = 2041, BNPParibas = 2042, CorfiGNBSudameris = 2048, CorporaciónFinancieraDavivienda = 2049, GirosYFinanzas = 4008, Tuya = 4026, GMFinancial = 4031, Coltefinanciera = 4046, Bancoldex = 4101, FinancieraDann = 4108, FinancieraPagos = 4115, Credifamilia = 4117, Crezcamos = 4118, LaHipotecaria = 4120, Juriscoop = 4121, RCI = 4122, FinancieraDeAntioquia = 32001, CooperativaFinancieraJFK = 32002, Coofinep = 32003, Cotrafa = 32004, Confiar = 32005, Daviplata = 1001551, Nequi = 1001507, FinanciamientoItau = 1001014.
        /// </summary>
        public Banco Banco { get; set; } = Banco.Desconocido; // Obligatorio.

        /// <summary>
        /// Si es nulo la cuenta del movimiento bancario es la establecida en opciones para el banco del movimiento bancario. 
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? OtroNúmeroCuenta { get; set; } // Si bien puede darse el caso de cambio de cuenta principal y por lo tanto perder el historial de que dinero salió de una u otra cuenta, se considera que es un caso muy especial y fácilmente solucionable con información disponbile fuera de SimpleOps. Se dará prioridad a disminuir la cantidad de datos en esta tabla al usar nulo cuando se está usando la cuenta principal.

        /// <summary>
        /// 'Oficina' en bancolombia.com.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)] 
        public string? Sucursal { get; set; }

        /// <summary>
        /// 'Descripción' en bancolombia.com.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)] 
        public string? Descripción { get; set; }

        /// <summary>
        /// Referencia identificadora de la entidad económica en los movimientos bancarios. Nombre basado en columna 'Referencia' en bancolombia.com.
        /// </summary>
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)] 
        public string? Referencia { get; set; }

        /// <summary>
        /// Si no es nulo se trata de una línea hija derivada de la línea padre aquí indicada. En ocasiones es necesario dividir un movimiento bancario en varias líneas hijas (con valores que suman el total del movimiento padre) para poder realizar los recibos de caja o comprobantes de egreso correspondientes. Por ejemplo es útil cuando se requieren hacer dos comprobantes de egreso para el pago de una deuda para distinguir entre abono e intereses.
        /// </summary>
        [ForeignKey("PadreID")] 
        public MovimientoBancario? Padre { get; set; }
        public int? PadreID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private MovimientoBancario() : base(AhoraUtcAjustado, 0) { } // Solo para que Entity Framework no saque error.

        public MovimientoBancario(DateTime fechaHora, decimal valor) : base(fechaHora, valor) => (Banco) = (Empresa.BancoPreferido);

        public MovimientoBancario(DateTime fechaHora, decimal valor, Banco banco) : base(fechaHora, valor) => (Banco) = (banco);

        #endregion Constructores


        #region Métodos y Funciones

        public override string ToString() => $"en {Banco} el {FechaHora.ToShortDateString()} por {Valor.ATextoDinero()}";

        public string? CuentaBancaria => ObtenerNúmeroCuentaBancaria(Banco, OtroNúmeroCuenta);

        #endregion Métodos y Funciones>


    } // MovimientoBancario>



} // SimpleOps.Modelo>
