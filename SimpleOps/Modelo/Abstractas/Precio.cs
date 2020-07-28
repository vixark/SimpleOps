using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace SimpleOps.Modelo {



    /// <summary>
    /// PrecioCliente o PrecioProveedor.
    /// </summary>
    abstract class Precio : Actualizable { // Es Actualizable porque el precio puede cambiar. No es necesario que sea Rastreable porque el registro de los precios cotizados a clientes se llevará en la tabla Cotizaciones. Para los precios de proveedores por el momento no se considera necesario pero si lo fuera se puede crear una tabla CotizacionesProveedores.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con ClienteID o ProveedorID forman la clave principal.

        public decimal Valor { get; set; } // Obligatorio.

        /// <summary>
        /// Permite proteger el precio para solo permitir su modificación a usuarios que tengan permiso de modificar esta columna (Protegido). En PrecioLista se podría usar para permitir algunos roles actualizar los precios de lista de los no protegidos y a otros roles también los protegidos.
        /// </summary>
        public bool Protegido { get; set; } = false;

        #endregion Propiedades>


        #region Constructores

        public Precio(Producto? producto, decimal valor) => (ProductoID, Valor, Producto) = (Producto.ObtenerID(producto), valor, producto); 

        #endregion Constructores>


    } // Precio>


} // SimpleOps.Modelo>
