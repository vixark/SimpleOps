using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de una venta.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaVenta : MovimientoProducto {


        #region Propiedades

        public Venta? Venta { get; set; } // Obligatorio.
        public int VentaID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaVenta() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaVenta(Producto producto, Venta venta, int cantidad, decimal precio, decimal costo) : base(producto, cantidad, precio, costo) 
            => (VentaID, Venta) = (venta.ID, venta);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Venta, VentaID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVAVenta(Venta?.Cliente, this);

        #endregion Métodos y Funciones>


    } // LíneaVenta>



} // SimpleOps.Modelo>
