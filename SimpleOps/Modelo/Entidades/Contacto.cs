using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Datos de contacto asociados a una dirección de email. No está restringido a una entidad económica porque un contacto puede ser válido para varias.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Contacto : Actualizable { // Es Actualizable porque todos sus datos excepto el email pueden actualizarse, pero no se hace Rastreable porque no es de mucho interés conocer la fecha de creación y es una tabla que puede crecer mucho.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        private string _Email = null!;
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string Email { // Obligatorio y de solo lectura. Solo se puede establecer en la creación. 
            get => _Email;
            set {
                if (!OperacionesEspecialesDatos) throw new Exception($"No se permite cambiar el email de un contacto.");
                _Email = value;
            }
        } // Email>

        /// <summary>
        /// Móvil o fijo.
        /// </summary>
        /// <MaxLength>30</MaxLength>
        [MaxLength(30)]
        public string? Teléfono { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string? Nombre { get; set; } // El nombre puede ser cambiado porque puede pasar que la dirección de emails corporativos permanezcan cuando cambie la persona que los atiende.

        /// <summary>
        /// Algunos contactos se pueden querer mantener en la base de datos así el email esté desactivado o eliminado. Es útil para evitar enviar comunicaciones a este email.
        /// </summary>
        public bool EmailActivo { get; set; } = true;

        #endregion Propiedades>


        #region Constructores

        private Contacto() { } // Solo para que EF Core no saque error.

        public Contacto(string email) => (_Email) = (email); // Para evitar la excepción ExcepciónNoPermitidoCambiarEmailContacto.

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Email;

        #endregion Métodos y Funciones>


    } // Contacto>



} // SimpleOps.Modelo>
