using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de cargo añadido a una venta.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaNotaDébitoVenta : MovimientoProducto {


        #region Propiedades

        public NotaDébitoVenta? NotaDébitoVenta { get; set; } // Obligatorio.
        public int NotaDébitoVentaID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaNotaDébitoVenta() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaNotaDébitoVenta(Producto producto, NotaDébitoVenta notaDébitoVenta, int cantidad, decimal precio, decimal costo)
            : base(producto, cantidad, precio, costo) 
            => (NotaDébitoVentaID, NotaDébitoVenta) = (notaDébitoVenta.ID, notaDébitoVenta);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(NotaDébitoVenta, NotaDébitoVentaID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVAVenta(NotaDébitoVenta?.Cliente, this);

        #endregion Métodos y Funciones>


    } // LíneaNotaDébitoVenta>



} // SimpleOps.Modelo>
