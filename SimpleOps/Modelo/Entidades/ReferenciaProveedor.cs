using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Referencia propia del proveedor asociada a un producto. Es de utilidad para realizar informes y agregarlas a los pedidos para facilidad para los proveedores.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ReferenciaProveedor : ReferenciaEntidadEconómica {


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private ReferenciaProveedor() : base(null, null!) { } // Solo para que Entity Framework no saque error.

        public ReferenciaProveedor(Producto producto, Proveedor proveedor, string referencia) : base(producto, referencia)
            => (ProveedorID, Proveedor) = (proveedor.ID, proveedor);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Proveedor, ProveedorID)} para {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // ReferenciaProveedor>



} // SimpleOps.Modelo>
