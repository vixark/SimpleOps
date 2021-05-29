using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Conjunto de permisos que tiene un usuario.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Rol : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único. No es la clave principal porque podría ser cambiado.

        /// <summary>
        /// Permisos de lectura, modificación, eliminación e inserción. Si es vacío no tiene ningún permiso.
        /// </summary>
        /// <MaxLength>10000</MaxLength>
        [MaxLength(10000)]
        public List<Permiso> Permisos { get; set; } = new List<Permiso>(); // Para evitar crear más tablas los permisos se almacenan directamente como un objeto serializado en JSON.

        #endregion Propiedades


        #region Constructores

        private Rol() { } // Solo para que Entity Framework no saque error.

        public Rol(string nombre) => (Nombre) = (nombre);

        #endregion Constructores


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Rol>



} // SimpleOps.Modelo>
