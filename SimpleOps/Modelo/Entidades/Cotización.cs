using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Precio dado a cliente. Se diferencia de PrecioCliente en que la tabla Cotizaciones es un registro histórico mientras que la tabla PreciosClientes tiene solo los últimos precios.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class Cotización : Registro { // Es registro porque una vez generado no admite cambios, no es necesario rastrear información de actualización.


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } 

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } 

        public decimal Precio { get; set; } // Obligatorio.

        #endregion Propiedades>


        #region Constructores

        private Cotización() { } // Solo para que EF Core no saque error.

        public Cotización(Producto producto, Cliente cliente, decimal precio) 
            => (ProductoID, ClienteID, Precio, Producto, Cliente) = (producto.ID, cliente.ID, precio, producto, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"el {FechaHoraCreación.ToShortDateString()} de {ATexto(Producto, ProductoID)} a {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // Cotización>


} // SimpleOps.Modelo>
