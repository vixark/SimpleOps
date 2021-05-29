using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de una orden de compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaOrdenCompra : LíneaSolicitudProducto {


        #region Propiedades

        public OrdenCompra? OrdenCompra { get; set; } // Obligatorio.
        public int OrdenCompraID { get; set; } // Clave foránea que con ProductoID forma la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaOrdenCompra() : base(null, 0, 0) { } // Solo para que Entity Framework no saque error.

        public LíneaOrdenCompra(OrdenCompra ordenCompra, Producto producto, int cantidad, decimal precio) : base(producto, cantidad, precio) 
            => (OrdenCompraID, OrdenCompra) = (ordenCompra.ID, ordenCompra);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(OrdenCompra, OrdenCompraID)} por {ATexto(Producto, ProductoID)}"; // Es el único que lleva 'por' porque la orden de compra es del cliente y lleva 'de' su ToString(), entonces se prefiere 'por' para que no sea muy redundante.

        #endregion Métodos y Funciones>


    } // LíneaOrdenCompra>



} // SimpleOps.Modelo>
