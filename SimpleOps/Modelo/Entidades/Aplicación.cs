using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// El uso principal que se le da a un producto.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Aplicación : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        #endregion Propiedades>


        #region Constructores

        private Aplicación() { } // Solo para que EF Core no saque error.

        public Aplicación(string nombre) => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Aplicación>



} // SimpleOps.Modelo>
