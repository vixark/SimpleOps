using System;
using System.Collections.Generic;
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Incluye el municipio y la dirección. No se usa en las propiedades que se escriben en la base de datos pero si en propiedades de solo lectura.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Ninguno)]
    class DirecciónCompleta {


        #region Propiedades

        public Municipio Municipio { get; set; }

        public string Dirección { get; set; }

        #endregion  Propiedades>


        #region Constructores

        public DirecciónCompleta(Municipio municipio, string dirección) => (Municipio, Dirección) = (municipio, dirección);

        #endregion Constructores>


        #region Métodos y Funciones

        /// <summary>
        /// Si no se dispone del municipio o de la dirección no se puede crear la dirección completa y esta será nula.
        /// </summary>
        public static DirecciónCompleta? CrearDirecciónCompleta(Municipio? municipio, string? dirección) 
            => municipio == null || dirección == null ? null : new DirecciónCompleta(municipio, dirección);

        #endregion Métodos y Funciones>


    } // DirecciónCompleta>



} // SimpleOps.Modelo>
