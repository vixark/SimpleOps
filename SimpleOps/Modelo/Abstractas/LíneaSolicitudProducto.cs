using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Línea de Ordenes de Compra (Clientes) o Pedidos (Proveedores).
    /// </summary>
    abstract class LíneaSolicitudProducto : Rastreable { // Es Rastreable porque es una entidad modificable y además es necesario disponer de la fecha de creación para calcular los días de cumplimiento.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con OrdenCompraID o PedidoID forma la clave principal.

        public decimal Precio { get; set; } // Obligatorio.

        public int Cantidad { get; set; } // Obligatorio.

        public int CantidadEntregada { get; set; } 

        public DateTime? FechaHoraCumplimiento { get; set; }

        /// <summary>
        /// Pendiente = 0, Cumplida = 1, Anulada = 2. Puede ser anulado o cumplido individualmente sin anular o cumplir la solicitud completa. No se hace autocalculada para poder mantener los datos de la solucitud al momento de la anulación.
        /// </summary>
        public EstadoSolicitudProducto Estado { get; set; } = EstadoSolicitudProducto.Pendiente;

        #endregion Propiedades>


        #region Constructores

        public LíneaSolicitudProducto(Producto? producto, int cantidad, decimal precio)
            => (ProductoID, Cantidad, Precio, Producto) = (Producto.ObtenerID(producto), cantidad, precio, producto); 

        #endregion Constructores>


        #region Propiedades Autocalculadas

        /// <summary>
        /// Si fue cumplido con la cantidad requerida y en una fecha específica.
        /// </summary>
        public bool Cumplido => (CantidadEntregada >= Cantidad) && (FechaHoraCumplimiento != null);

        /// <summary>
        /// Si aún está pendiente de entrega. No está pendiente si fue anulado o si fue cumplido.
        /// </summary>
        public bool Pendiente => !Cumplido && (Estado != EstadoSolicitudProducto.Anulada);

        public int CantidadPendiente => Pendiente ? Cantidad - CantidadEntregada : 0;

        public double? DíasCumplimiento => (FechaHoraCumplimiento - FechaHoraCreación)?.TotalDays;

        #endregion Propiedades Autocalculadas>


    } // LíneaSolicitudProducto>



} // SimpleOps.Modelo>
