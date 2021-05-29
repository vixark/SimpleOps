using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Último precio de un producto para un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class PrecioCliente : Precio {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private PrecioCliente() : base(null, 0) { } // Solo para que Entity Framework no saque error.

        public PrecioCliente(Producto producto, Cliente cliente, decimal valor) : base(producto, valor) 
            => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Producto, ProductoID)} a {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // PrecioCliente>



} // SimpleOps.Modelo>
