using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Comprobante de movimiento de dinero: Recibo de Caja o Comprobante de Egreso.
    /// </summary>
    abstract class ComprobanteDinero : Actualizable { // Si bien una vez hecho el comprobante de dinero este no debería aceptar cambios, para efectos de SimpleOps si se generan cambios en su estado. Es Actualizable y no Rastreable porque ya dispone de una FechaHora propia que informa de su creación, que aunque en algunas ocasiones podría ser actualizada después de la creación por lo general no se hace. Por ejemplo, la FechaHora se puede actualizar si son comprobantes de movimientos bancarios hechos al inicio del mes que corresponden contablemente al mes anterior.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Fecha y hora legal del comprobante.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// El valor del comprobante que se usó para la cancelación de facturas.
        /// </summary>
        public decimal ValorFacturas { get; set; } = 0; // Obligatorio.

        /// <summary>
        /// El valor del comprobante que se usó para abonar al saldo a favor de la entidad económica. Si es negativo es la parte del saldo pendiente de la entidad económica que se sumó al movimiento de dinero para cancelar las facturas.
        /// </summary>
        public decimal Abono { get; set; } = 0; // Obligatorio.

        /// <summary>
        /// Realizado = 0: Se generó el comprobante de dinero. ReportadoContabilidad = 1: Se reportó el comprobante de dinero a la contabilidad, esto por lo general implica el envío por email o impresión de un lote con la relación de los comprobantes de dinero de cierto período. Si un comprobante de dinero ya reportado es anulado esta anulación debe ser reportada nuevamente a contabilidad. Anulado = 2. AnuladoReportado = 3: Si el comprobante de dinero se anuló y además ya se reportó a contabilidad su anulación.
        /// </summary>
        public EstadoComprobanteDinero Estado { get; set; } = EstadoComprobanteDinero.Realizado;

        /// <summary>
        /// Desconocido = 0, Banco = 1, Caja = 2.
        /// </summary>
        public LugarMovimientoDinero Lugar { get; set; } = LugarMovimientoDinero.Desconocido; // Obligatorio. Podría tomar el valor desconocido en casos de carga de datos anteriores donde no se conoce el lugar de un comprobante de dinero anulado.

        public Cliente? Cliente { get; set; } // Es necesario establecerlo para ambos comprobantes de dinero porque también se le puede enviar dinero a un cliente (comprobante egreso) en el caso de devoluciones de dinero por pagos realizados y producto devuelto.
        public int? ClienteID { get; set; }

        public Proveedor? Proveedor { get; set; } // Es necesario establecerlo para ambos comprobantes de dinero porque también se puede recibir dinero de un proveedor (recibo de caja) en el caso de devoluciones de dinero por pagos realizados y producto devuelto.
        public int? ProveedorID { get; set; }

        #endregion Propiedades>


        #region Constructores

        public ComprobanteDinero(LugarMovimientoDinero lugar) => (FechaHora, Lugar) = (AhoraUtcAjustado, lugar);

        public ComprobanteDinero(Proveedor proveedor, LugarMovimientoDinero lugar) : this(lugar) => (ProveedorID, Proveedor) = (proveedor.ID, proveedor);

        public ComprobanteDinero(Cliente cliente, LugarMovimientoDinero lugar) : this(lugar) => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// El total de dinero de la transacción. No modificable, modifica Abono ó Valor Facturas.
        /// </summary>
        public decimal Total => ValorFacturas + Abono; // Es autocalculado para evitar redundancias e inconsistencias en los datos.

        #endregion Propiedades Autocalculadas>


    } // ComprobanteDinero>



} // SimpleOps.Modelo>
