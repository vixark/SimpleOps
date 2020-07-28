using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de producto devuelto de una compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaNotaCréditoCompra : MovimientoProducto {


        #region Propiedades

        public NotaCréditoCompra? NotaCréditoCompra { get; set; } // Obligatorio.
        public int NotaCréditoCompraID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaNotaCréditoCompra() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaNotaCréditoCompra(Producto producto, NotaCréditoCompra notaCréditoCompra, int cantidad, decimal precio, decimal costo)
            : base(producto, cantidad, precio, costo) 
            => (NotaCréditoCompraID, NotaCréditoCompra) = (notaCréditoCompra.ID, notaCréditoCompra);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(NotaCréditoCompra, NotaCréditoCompraID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVACompra(this);

        #endregion Métodos y Funciones>


    } // LíneaNotaCréditoCompra>



} // SimpleOps.Modelo>
