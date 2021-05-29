using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Solicitud de producto a un proveedor.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class Pedido : SolicitudProducto {


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Pedido() { } // Solo para que Entity Framework no saque error.

        public Pedido(Proveedor proveedor) => (ProveedorID, Proveedor) = (proveedor.ID, proveedor);

        public List<LíneaPedido> Líneas { get; set; } = new List<LíneaPedido>();

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ID} a {ATexto(Proveedor, ProveedorID)}";

        #endregion Métodos y Funciones>


    } // Pedido>



} // SimpleOps.Modelo>
