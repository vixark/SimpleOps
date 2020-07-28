using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de cargo añadido a una compra.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaNotaDébitoCompra : MovimientoProducto {


        #region Propiedades

        public NotaDébitoCompra? NotaDébitoCompra { get; set; } // Obligatorio.
        public int NotaDébitoCompraID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaNotaDébitoCompra() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaNotaDébitoCompra(Producto producto, NotaDébitoCompra notaDébitoCompra, int cantidad, decimal precio, decimal costo)
            : base(producto, cantidad, precio, costo) 
            => (NotaDébitoCompraID, NotaDébitoCompra) = (notaDébitoCompra.ID, notaDébitoCompra);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(NotaDébitoCompra, NotaDébitoCompraID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVACompra(this);

        #endregion Métodos y Funciones>


    } // LíneaNotaDébitoCompra>



} // SimpleOps.Modelo>
