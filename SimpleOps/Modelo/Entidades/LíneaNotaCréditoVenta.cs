using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de producto devuelto de una venta.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaNotaCréditoVenta : MovimientoProducto {


        #region Propiedades

        public NotaCréditoVenta? NotaCréditoVenta { get; set; } // Obligatorio.
        public int NotaCréditoVentaID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaNotaCréditoVenta() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaNotaCréditoVenta(Producto producto, NotaCréditoVenta notaCréditoVenta, int cantidad, decimal precio, decimal costo)
            : base(producto, cantidad, precio, costo) 
            => (NotaCréditoVentaID, NotaCréditoVenta) = (notaCréditoVenta.ID, notaCréditoVenta);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(NotaCréditoVenta, NotaCréditoVentaID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVAVenta(NotaCréditoVenta?.Cliente, this);

        #endregion Métodos y Funciones>


    } // LíneaNotaCréditoVenta>



} // SimpleOps.Modelo>
