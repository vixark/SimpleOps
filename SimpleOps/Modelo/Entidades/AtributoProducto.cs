using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cuando se usan <see cref="ProductoBase"/>, los atributos sirven para diferenciar productos que comparten el mismo producto base entre si.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class AtributoProducto : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Cómo el nombre del atributo se usa para diferenciar productos específicos entre sí en los documentos gráficos, este debe ser descriptivo 
        /// por si mismo sin necesidad del tipo de atribruto. Por ejemplo, un nombre de atributo correcto es "Talla 10", en vez de "10".
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único.

        public TipoAtributoProducto? Tipo { get; set; } // Obligatorio. Siempre un atributo debe tener un tipo, incluso si este es de tipo "Sin Tipo".
        public int TipoID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private AtributoProducto() { } // Solo para que EF Core no saque error.

        public AtributoProducto(string nombre, TipoAtributoProducto tipo) => (Nombre, Tipo, TipoID) = (nombre, tipo, tipo.ID);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // AtributoProducto>



} // SimpleOps.Modelo>
