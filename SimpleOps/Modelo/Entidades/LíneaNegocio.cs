using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// De producto.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class LíneaNegocio : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        #endregion Propiedades>


        #region Constructores

        private LíneaNegocio() { } // Solo para que EF Core no saque error.

        public LíneaNegocio(string nombre) => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // LíneaNegocio>



} // SimpleOps.Modelo>
