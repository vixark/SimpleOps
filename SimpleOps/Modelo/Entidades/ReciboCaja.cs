using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Comprobante de entrada de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class ReciboCaja : ComprobanteDinero {


        #region Propiedades

        public List<Venta> Ventas { get; set; } = new List<Venta>();

        #endregion Propiedades>


        #region Constructores

        private ReciboCaja() : base(LugarMovimientoDinero.Desconocido) { } // Solo para que Entity Framework no saque error.

        public ReciboCaja(Proveedor proveedor, LugarMovimientoDinero lugar) : base(proveedor, lugar) { }

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar) : base(cliente, lugar) { }

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar, decimal valorFacturas) : this(cliente, lugar) 
            => (ValorFacturas) = (valorFacturas);

        public ReciboCaja(Cliente cliente, LugarMovimientoDinero lugar, decimal valorFacturas, decimal abono) : this(cliente, lugar, valorFacturas)
            => (Abono) = (abono);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Cliente, ClienteID)}{ATexto(Proveedor, ProveedorID)}"; // Se agregan juntos en la cadena porque nunca se presenta el caso que tenga tanto Cliente como Proveedor.

        #endregion Métodos y Funciones>


    } // ReciboCaja>



} // SimpleOps.Modelo>
