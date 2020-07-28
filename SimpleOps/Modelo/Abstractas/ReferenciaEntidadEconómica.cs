using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Referencia propia de la entidad económica (cliente o proveedor) asociada a un producto.
    /// </summary>
    abstract class ReferenciaEntidadEconómica : Actualizable { // Es Actualizable porque la referencia puede cambiar. No es de mucho interés la fecha de creación entonces con ser Actualizable basta.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con ClienteID o ProveedorID forman la clave principal.

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string Valor { get; set; } // Obligatorio.

        #endregion Propiedades>


        #region Constructores

        public ReferenciaEntidadEconómica(Producto? producto, string referencia) 
            => (ProductoID, Valor, Producto) = (Producto.ObtenerID(producto), referencia, producto); 

        #endregion Constructores>


    } // ReferenciaEntidadEconómica>



} // SimpleOps.Modelo>
