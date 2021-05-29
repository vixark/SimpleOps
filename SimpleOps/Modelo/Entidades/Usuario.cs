using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Persona autorizada para usar SimpleOps.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Usuario : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio.

        public bool EsRepresentanteComercial { get; set; }

        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string Email { get; set; } = null!; // Obligatorio. Necesario para enviar notificaciones e informes.

        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? Teléfono { get; set; }

        /// <summary>
        /// Si es falso el usuario no puede iniciar la aplicación.
        /// </summary>
        public bool Activo { get; set; } = true;

        #endregion Propiedades>


        #region Constructores

        private Usuario() { } // Solo para que Entity Framework no saque error.

        public Usuario(string nombre, string email) => (Nombre, Email) = (nombre, email);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Usuario>



} // SimpleOps.Modelo>
