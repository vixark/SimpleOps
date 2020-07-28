using System;
using System.Collections.Generic;
using System.Text;



namespace SimpleOps.Modelo {


    /// <summary>
    /// Interface genérica aplicable a todos los documentos que generen movimiento de productos. Admite un parámetro de tipo <typeparamref name="M"/> 
    /// que puede ser cualquier clase hija de <see cref="MovimientoProducto"/> como <see cref="LíneaVenta"/>, <see cref="LíneaVenta"/>, 
    /// <see cref="LíneaNotaCréditoVenta"/>, <see cref="LíneaNotaCréditoCompra"/>, <see cref="LíneaNotaDébitoVenta"/>, <see cref="LíneaNotaDébitoCompra"/> o 
    /// <see cref="LíneaRemisión"/>. Aunque la mayoría de la funcionalidad que esta interface podría proveer la cubre la clase abstracta <see cref="Factura{E, M}"/>, 
    /// se mantiene para permitir la escritura métodos o funciones compartidas que también incluyan las facturas y las remisiones.
    /// </summary>
    interface ITieneLíneas<M> where M : MovimientoProducto {


        #region Propiedades

        public List<M> Líneas { get; set; }

        #endregion Propiedades>


    } // ITieneLíneas>



} // SimpleOps.Modelo>
