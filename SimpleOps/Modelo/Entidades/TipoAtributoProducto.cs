using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cuando se usan <see cref="ProductoBase"/>, los <see cref="AtributoProducto"/> sirven para diferenciar productos que comparten el mismo producto 
    /// base entre si. Estos pueden ser clasificados por <see cref="TipoAtributoProducto"/> para presentación al usuario.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class TipoAtributoProducto : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        #endregion Propiedades>


        #region Constructores

        private TipoAtributoProducto() { } // Solo para que EF Core no saque error.

        public TipoAtributoProducto(string nombre) => (Nombre) = (nombre);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // TipoAtributoProducto>



} // SimpleOps.Modelo>
