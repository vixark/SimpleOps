using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de una remisión.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class LíneaRemisión : MovimientoProducto {


        #region Propiedades

        public Remisión? Remisión { get; set; } // Obligatorio.
        public int RemisiónID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private LíneaRemisión() : base(null, 0, 0, 0) { } // Solo para que Entity Framework no saque error.

        public LíneaRemisión(Producto producto, Remisión remisión, int cantidad, decimal precio, decimal costo)
            : base(producto, cantidad, precio, costo) 
            => (RemisiónID, Remisión) = (remisión.ID, remisión);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Remisión, RemisiónID)} de {ATexto(Producto, ProductoID)}";

        public override decimal? IVA => 0;

        #endregion Métodos y Funciones>


    } // LíneaRemisión>



} // SimpleOps.Modelo>
