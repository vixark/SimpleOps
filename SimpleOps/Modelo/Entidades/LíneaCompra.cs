using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de producto comprado.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaCompra : MovimientoProducto {


        #region Propiedades
   
        public Compra? Compra { get; set; } // Obligatorio.
        public int CompraID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaCompra() : base(null, 0, 0, 0) { } // Solo para que EF Core no saque error.

        public LíneaCompra(Producto producto, Compra compra, int cantidad, decimal precio, decimal costo)  : base(producto, cantidad, precio, costo) 
            => (CompraID, Compra) = (compra.ID, compra);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Compra, CompraID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => ObtenerIVACompra(this);

        #endregion Métodos y Funciones>


    } // LíneaCompra>



} // SimpleOps.Modelo>
