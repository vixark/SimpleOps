using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Movimiento de dinero en efectivo.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class MovimientoEfectivo : MovimientoDinero {


        #region Propiedades

        #endregion Propiedades>


        #region Constructores

        private MovimientoEfectivo() : base(AhoraUtcAjustado, 0) { } // Solo para que EF Core no saque error.

        public MovimientoEfectivo(DateTime fechaHora, decimal valor, ReciboCaja reciboCaja) : base(fechaHora, valor) 
            => (ReciboCajaID, ReciboCaja) = (reciboCaja.ID, reciboCaja);

        public MovimientoEfectivo(DateTime fechaHora, decimal valor, ComprobanteEgreso comprobanteEgreso) : base(fechaHora, valor) 
            => (ComprobanteEgresoID, ComprobanteEgreso) = (comprobanteEgreso.ID, comprobanteEgreso);

        #endregion Constructores


        #region Métodos y Funciones

        public override string ToString() => $"el {FechaHora.ToShortDateString()} por {Valor.ATextoDinero()}";

        #endregion Métodos y Funciones>


    } // MovimientoEfectivo>



} // SimpleOps.Modelo>
