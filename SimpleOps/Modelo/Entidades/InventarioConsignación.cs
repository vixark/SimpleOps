using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Producto propiedad de la empresa que el cliente tiene almacenado en sus instalaciones.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Pesimista)]
    class InventarioConsignación : Actualizable { // Es Actualizable porque la cantidad puede cambiar. No es Rastreable porque no es necesario conocer la fecha de creación. Si se necesitara para otros usos se podría rastrear la primera LíneaRemisión de ese producto al cliente.


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con ClienteID forman la clave principal.

        public int Cantidad { get; set; } // Obligatorio.

        /// <summary>
        /// El costo de compra por unidad y gastos asociados a su producción y obtención. Incluye costo de transporte. Si es nulo es desconocido.
        /// </summary>
        public decimal? CostoUnitario { get; set; } = null; // Se permite nulo porque el valor 0 significa costo unitario cero, que podría ser válido en algunas situaciones.

        public decimal Precio { get; set; } // Obligatorio.

        #endregion Propiedades>


        #region Constructores

        private InventarioConsignación() { } // Solo para que EF Core no saque error.

        public InventarioConsignación(Producto producto, Cliente cliente, int cantidad, decimal precio) 
            => (ClienteID, ProductoID, Cantidad, Precio, Producto, Cliente) = (cliente.ID, producto.ID, cantidad, precio, producto, cliente);

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// El costo de compra y gastos asociados a su producción y obtención de la cantidad total en inventario en consignación. Incluye costo de transporte.
        /// </summary>
        public decimal? CostoTotal => ObtenerSubtotal(CostoUnitario, Cantidad);

        /// <summary>
        /// El subtotal de la venta que está pendiente asociada a este producto en consignación.
        /// </summary>
        public decimal VentaPendiente => Precio * Cantidad;

        #endregion Propiedades Autocalculadas>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Cliente, ClienteID)} de {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // InventarioConsignación>



} // SimpleOps.Modelo>
