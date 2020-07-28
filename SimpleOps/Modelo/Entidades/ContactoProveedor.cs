using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {


    /// <summary>
    /// Asocia los contactos a cada proveedor.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ContactoProveedor : Actualizable { // Es Actualizable porque puede cambiar el Tipo. No es de interés conocer la fecha de creación entonces no se hace Rastreable.


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; } // Clave foránea que con ContactoID forman la clave principal.

        public Contacto? Contacto { get; set; } // Obligatorio.
        public int ContactoID { get; set; } // Clave foránea que con ProveedorID forman la clave principal.

        /// <summary>
        /// Desconocido = 0, Vendedor = 1, Despachos = 2, Tesorería = 5, JefeVentas = 10, AltoDirectivo = 15, Gerente = 20, Propietario = 25, Otro = 255.
        /// </summary>
        public TipoContactoProveedor Tipo { get; set; } = TipoContactoProveedor.Desconocido;

        #endregion Propiedades>


        #region Constructores

        private ContactoProveedor() { } // Solo para que EF Core no saque error.

        public ContactoProveedor(Proveedor proveedor, Contacto contacto) 
            => (ProveedorID, ContactoID, Proveedor, Contacto) = (proveedor.ID, contacto.ID, proveedor, contacto);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{ATexto(Contacto, ContactoID)} de {ATexto(Proveedor, ProveedorID)}";

        #endregion Métodos y Funciones>


    } // ContactoProveedor>



} // SimpleOps.Modelo>
