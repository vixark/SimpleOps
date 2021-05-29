using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {


    /// <summary>
    /// Asocia los contactos a cada cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ContactoCliente : Actualizable { // Es Actualizable porque puede cambiar el Tipo. No es de interés conocer la fecha de creación entonces no se hace Rastreable.


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ContactoID forman la clave principal.

        public Contacto? Contacto { get; set; } // Obligatorio.
        public int ContactoID { get; set; } // Clave foránea que con ClienteID forman la clave principal.

        /// <summary>
        /// Observaciones adicionales a Cliente.ObservacionesFactura que se escribirán en el campo de observaciones de la factura y se mostrarán en la interfaz al hacer una factura a la empresa de este contacto.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)] 
        public string? ObservacionesFactura { get; set; }

        /// <summary>
        /// Desconocido = 0, Comprador = 1, Almacenista = 2, Tesorería = 5, JefeCompras = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255.
        /// </summary>
        public TipoContactoCliente Tipo { get; set; } = TipoContactoCliente.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private ContactoCliente() { } // Solo para que Entity Framework no saque error. 

        public ContactoCliente(Cliente cliente, Contacto contacto) 
            => (ClienteID, ContactoID, Cliente, Contacto) = (cliente.ID, contacto.ID, cliente, contacto);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Contacto, ContactoID)} de {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // ContactoCliente>



} // SimpleOps.Modelo>
