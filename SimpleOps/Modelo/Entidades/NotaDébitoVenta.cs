using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cargo al valor de una venta.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class NotaDébitoVenta : Factura<Cliente, LíneaNotaDébitoVenta> {


        #region Propiedades

        [NotMapped]
        public override Cliente? EntidadEconómica => Cliente;

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        public Venta? Venta { get; set; } // Obligatorio.
        public int VentaID { get; set; }

        public override List<LíneaNotaDébitoVenta> Líneas { get; set; } = new List<LíneaNotaDébitoVenta>();

        /// <summary>
        /// Intereses = 1, Gastos = 2, AjustePrecio = 3, Otra = 4.
        /// </summary>
        public RazónNotaDébito Razón { get; set; } = RazónNotaDébito.Otra;

        #endregion Propiedades>


        #region Constructores

        private NotaDébitoVenta() { } // Solo para que EF Core no saque error.

        public NotaDébitoVenta(Cliente cliente, Venta venta) {
            (ClienteID, VentaID, Cliente, Venta) = (cliente.ID, venta.ID, cliente, venta);
            VerificarDatosEntidad();
        } // NotaDébitoVenta>

        #endregion Constructores>


        #region Propiedades Autocalculadas

        public DateTime? FechaVencimiento => Cliente == null ? (DateTime?)null : FechaHora.AddDays(Cliente.DíasCrédito);

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"{Código} a {ATexto(Cliente, ClienteID)}";

        public override void VerificarDatosEntidad() => Cliente.VerificarDatosVenta(Cliente);

        public override decimal ObtenerAnticipo() => 0;

        public override string? ObtenerClaveParaCude() => Empresa.PinAplicación;

        #endregion Métodos y Funciones>


    } // NotaDébitoVenta>



} // SimpleOps.Modelo>
