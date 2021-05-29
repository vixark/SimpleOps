using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Comprobante de salida de dinero.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class ComprobanteEgreso : ComprobanteDinero {


        #region Propiedades

        public List<Compra> Compras { get; set; } = new List<Compra>();

        #endregion Propiedades>


        #region Constructores

        private ComprobanteEgreso() : base(LugarMovimientoDinero.Desconocido) { } // Solo para que Entity Framework no saque error.

        public ComprobanteEgreso(Cliente cliente, LugarMovimientoDinero lugar) : base(cliente, lugar) { }

        public ComprobanteEgreso(Proveedor proveedor, LugarMovimientoDinero lugar) : base(proveedor, lugar) { }

        public ComprobanteEgreso(Proveedor proveedor, LugarMovimientoDinero lugar, decimal valorFacturas) : this(proveedor, lugar)
            => (ValorFacturas) = (valorFacturas);

        public ComprobanteEgreso(Proveedor proveedor, LugarMovimientoDinero lugar, decimal valorFacturas, decimal abono) 
            : this(proveedor, lugar, valorFacturas)
            => (Abono) = (abono);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Proveedor, ProveedorID)}{ATexto(Cliente, ClienteID)}"; // Se agregan juntos en la cadena porque nunca se presenta el caso que tenga tanto Cliente como Proveedor.

        #endregion Métodos y Funciones>


    } // ComprobanteEgreso>



} // SimpleOps.Modelo>