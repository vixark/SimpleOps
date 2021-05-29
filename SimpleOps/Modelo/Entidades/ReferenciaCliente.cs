using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Referencia propia del cliente asociada a un producto. Es de utilidad para realizar informes y agregarlas a las facturas o remisiones de entrega para facilidad para los clientes.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ReferenciaCliente : ReferenciaEntidadEconómica {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private ReferenciaCliente() : base(null, null!) { } // Solo para que Entity Framework no saque error.

        public ReferenciaCliente(Producto producto, Cliente cliente, string referencia) : base(producto, referencia) 
            => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Cliente, ClienteID)} para {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // ReferenciaCliente>



} // SimpleOps.Modelo>
