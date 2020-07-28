using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Campaña de mercadeo con la que se obtuvo el cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Campaña : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        #endregion Propiedades>


        #region Constructores

        private Campaña() { } // Solo para que EF Core no saque error.

        public Campaña(string nombre) => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Campaña>



} // SimpleOps.Modelo>
