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
    class Marca : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        /// <summary>
        /// Si es verdadero se espera que la marca se agregue a la descripción del producto lo que permite su adición automática y otras funciones.
        /// </summary>
        public bool EnDescripción { get; set; } = true; 

        /// <summary>
        /// Si es verdadero los productos de estas marcas tendrán prioridad en el buscador de productos sobre los otros.
        /// </summary>
        public bool PriorizarEnBuscador { get; set; } = true;

        #endregion Propiedades>


        #region Constructores

        private Marca() { } // Solo para que EF Core no saque error.

        public Marca(string nombre) => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // Marca>



} // SimpleOps.Modelo>
