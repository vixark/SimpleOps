using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de un pedido.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaPedido : LíneaSolicitudProducto {


        #region Propiedades

        public Pedido? Pedido { get; set; } // Obligatorio.
        public int PedidoID { get; set; } // Clave foránea que con ProductoID forma la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaPedido() : base(null, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaPedido(Pedido pedido, Producto producto, int cantidad, decimal precio) : base(producto, cantidad, precio) 
            => (PedidoID, Pedido) = (pedido.ID, pedido);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Pedido, PedidoID)} de {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // LíneaPedido>



} // SimpleOps.Modelo>
