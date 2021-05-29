using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Último precio de un producto dado por un proveedor.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class PrecioProveedor : Precio {


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private PrecioProveedor() : base(null, 0) { } // Solo para que Entity Framework no saque error.

        public PrecioProveedor(Producto producto, Proveedor proveedor, decimal valor) : base(producto, valor)
            => (ProveedorID, Proveedor) = (proveedor.ID, proveedor);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Producto, ProductoID)} por {ATexto(Proveedor, ProveedorID)}";

        #endregion Métodos y Funciones>


    } // PrecioProveedor>



} // SimpleOps.Modelo>
