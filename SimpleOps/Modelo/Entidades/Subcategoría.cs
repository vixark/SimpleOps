using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// De producto.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Subcategoría : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        /// <summary>
        /// Categorización personalizada más amplia que la subcategoría. Cada producto tiene una subcategoría y varias subcategorías pueden pertenecer a la misma categoría. Puede ser nula si no se desea ampliar más esta categorización y basta con la subcategoría.
        /// </summary>
        public Categoría? Categoría { get; set; }
        public int? CategoríaID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private Subcategoría() { } // Solo para que Entity Framework no saque error.

        public Subcategoría(string nombre)  => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // TipoProducto>



} // SimpleOps.Modelo>
