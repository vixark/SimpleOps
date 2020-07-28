using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Orden de Compra (Clientes) o Pedido (Proveedores).
    /// </summary>
    abstract class SolicitudProducto : Actualizable { // Es Actualizable porque su estado puede cambiar. No es Rastreable porque aunque la fecha de creación es importante tenerla para usos varios, esta se puede obtener de la fecha de creación de alguna de sus LíneaSolicitudProducto, pues ambas entidades son creadas como una sola operación. Es un caso inverso al de MovimientoProducto-Factura.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        public Contacto? Contacto { get; set; } 
        public int? ContactoID { get; set; }

        /// <summary>
        /// Pendiente = 0, Cumplida = 1, Anulada = 2.
        /// </summary>
        public EstadoSolicitudProducto Estado { get; set; } = EstadoSolicitudProducto.Pendiente;

        /// <MaxLength>500</MaxLength>
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        /// <summary>
        /// No se almacena en la base de datos porque es información redundante con LíneaSolicitudProducto.FechaHoraCreación y evitar incrementar el 
        /// tamaño de la base de datos innecesariamente. Se puede escribir cuando se necesite desde la menor FechaHoraCreación de sus LíneaSolicitudProducto.
        /// </summary>
        [NotMapped]
        public DateTime? FechaHoraCreación { get; set; }

        #endregion Propiedades>


    } // SolicitudProducto>



} // SimpleOps.Modelo>
